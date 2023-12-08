using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
namespace SuperFight
{
    public class PlayerController : Controller
    {
        [Header("Refs")]
        private IngameScreenUI ingameScreen;
        private PlayerManager player;
        private InputHandle input;
        private Character character;
        private CharacterEquipment characterEquipment;
        public Transform characterHolder;
        [Header("Movement Properties")]
        private float runtimeMovementSpeed;
        public float speed = 8f;
        [Header("Jump Properties")]
        public float jumpForce = 10f;
        public float jumpBufferTime = 0.2f;
        private float jumpBufferTimer;
        public float maxFallVelocity = -10f;
        public int jumpCount;
        [Header("Dash Properties")]
        public float dashForce = 10f;
        public float dashTime = 0.2f;
        private float dashTimer = 0;
        public float dashRecoveryTime = 0.5f;
        private float dashRecoveryTimer = 0f;
        [Header("Check Properties")]
        public float footOffset = 0.2f;
        public float groundRayDistance = 0.2f;
        public LayerMask groundLayer;
        public LayerMask blockLayer;
        [Header("Status Flags")]
        private bool isRun;
        public bool isOnGround;
        public bool isDashing;
        private Vector3 lastPositionOnGround;
        public float moveAmount;
        public ParticleSystem dashFx;
        public ParticleSystem bloodFx;
        public ParticleSystem levelUpFx;
        public ParticleSystem reviveFx;
        [Header("Weapons")]
        private Weapon currentWeapon;
        public void Initialize(PlayerManager playerManager)
        {
            player = playerManager;
            input = player.input;
            moveAmount = 0;
            jumpCount = 0;
            runtimeMovementSpeed = speed;
            ingameScreen = UIManager.Instance.GetScreen<IngameScreenUI>();
            characterEquipment = GetComponent<CharacterEquipment>();
            SetSkin();
            characterEquipment.Initialize(this, character);
            core.Initialize(this);
            GameManager.OnPause += Pause;
            GameManager.OnResume += Resume;
            DataManager.OnUpgrade += OnLevelUp;
            Inventory.OnEquipmentChange += RefreshEquipment;
        }

        public void OnWin()
        {
            if (currentWeapon)
            {
                currentWeapon.SetActive(false);
            }
            isDashing = false;
            core.movement.SetVelocityZero();
            input.ResetInput();
            animatorHandle.PlayAnimation("Dance", 0.1f, 1);
        }

        public void Revive()
        {
            transform.DOMove(lastPositionOnGround, 1f).SetEase(Ease.Linear);
            reviveFx.Play();
        }
        public void OnLevelUp()
        {
            levelUpFx.Play();
            AudioManager.Instance.PlayOneShot("LevelUp", 1f);
            ResetController();
        }
        public override void ResetController()
        {
            animatorHandle.OnAnimatorUpdate += AnimatorUpdate;
            runtimeMovementSpeed = speed;
            base.ResetController();
            animatorHandle.ResetAnimator();
            isActive = true;
            RefreshEquipment();
            runtimeStats = new CharacterStats(originalStats);
            ingameScreen.playerHealthBar.InitializeBar(true);
            ingameScreen.playerHealthBar.UpdateHealthText(runtimeStats.health + "/" + originalStats.health);
        }
        private void AnimatorUpdate(Vector3 delta)
        {
            if (isApplyRootMotion)
            {
                int facingDirection = core.movement.facingDirection;
                delta /= Time.deltaTime;
                RaycastHit2D raycast = RayCast(Vector3.up * 0.1f, Vector2.right * facingDirection, 1.2f, blockLayer);
                if (!raycast)
                {
                    core.movement.SetVelocityX(Mathf.Abs(delta.z) * facingDirection);
                }
                else
                {
                    core.movement.SetVelocityX(0);
                }
            }
        }

        public void RefreshEquipment()
        {
            originalStats = new CharacterStats();
            originalStats.health = 200 + (DataManager.PlayerLevel - 1) * 10;
            originalStats.damage = 20 + (DataManager.PlayerLevel - 1) * 2;
            originalStats.critDamage = 50;

            characterEquipment.RefreshEquipment();

            currentWeapon = characterEquipment.primaryWeapon;
            currentWeapon.SetActive(true);

            if (characterEquipment.primaryWeapon)
            {
                originalStats += characterEquipment.primaryWeapon.equipmentStats;
            }
            // if (characterEquipment.secondaryWeapon)
            // {
            //     originalStats += characterEquipment.secondaryWeapon.equipmentStats;
            // }
            if (characterEquipment.currentArmor)
            {
                originalStats += characterEquipment.currentArmor.equipmentStats;
            }
            if (characterEquipment.currentBoots)
            {
                originalStats += characterEquipment.currentBoots.equipmentStats;
            }
            if (characterEquipment.currentNecklace)
            {
                originalStats += characterEquipment.currentNecklace.equipmentStats;
            }
            if (characterEquipment.currentRing)
            {
                originalStats += characterEquipment.currentRing.equipmentStats;
            }

            CharacterStats stats = new CharacterStats(originalStats);

            if (characterEquipment.primaryWeapon)
            {
                characterEquipment.primaryWeapon.SetUpPassive(stats);
                originalStats += characterEquipment.primaryWeapon.equipmentStats;
            }
            // if (characterEquipment.secondaryWeapon)
            // {
            //     characterEquipment.secondaryWeapon.SetUpPassive(stats);
            //     originalStats += characterEquipment.secondaryWeapon.equipmentStats;
            // }
            if (characterEquipment.currentArmor)
            {
                characterEquipment.currentArmor.SetUpPassive(stats);
                originalStats += characterEquipment.currentArmor.equipmentStats;
            }
            if (characterEquipment.currentBoots)
            {
                characterEquipment.currentBoots.SetUpPassive(stats);
                originalStats += characterEquipment.currentBoots.equipmentStats;
            }
            if (characterEquipment.currentNecklace)
            {
                characterEquipment.currentNecklace.SetUpPassive(stats);
                originalStats += characterEquipment.currentNecklace.equipmentStats;
            }
            if (characterEquipment.currentRing)
            {
                characterEquipment.currentRing.SetUpPassive(stats);
                originalStats += characterEquipment.currentRing.equipmentStats;
            }
            input.secondaryButton.SetActive(characterEquipment.secondaryWeapon != null);
        }
        private void SetSkin()
        {
            if (character != null)
            {
                Destroy(character.gameObject);
            }
            Character c = null;
            if (c == null)
            {
                c = Resources.Load<Character>("DefaultCharacter");
            }
            character = Instantiate(c, characterHolder);
            character.OnEventAnimation += OnEvent;
            c.transform.localPosition = new Vector3(0, -0.05f, 0);
            animatorHandle = character;
            character.Initialize(this);
            character.ResetAnimator();
        }

        private void OnEvent(string obj)
        {
            if (isOnGround && !isInteracting)
            {
                if (obj.Equals("FootStep1"))
                {
                    AudioManager.Instance.PlayOneShot("footstep1", 0.5f);
                }
                if (obj.Equals("FootStep2"))
                {
                    AudioManager.Instance.PlayOneShot("footstep2", 0.5f);
                }
            }

        }

        public void UpdateScript()
        {
            if (GameManager.GameState == GameState.PAUSE) return;
            moveAmount = Mathf.Abs(input.horizontal);
            animatorHandle.SetFloat("MoveAmount", moveAmount);
            animatorHandle.SetFloat("VelocityY", core.movement.currentVelocity.y);
            animatorHandle.SetBool("IsStunning", isStunning);
            isInteracting = animatorHandle.GetBool("IsInteracting");
            isApplyRootMotion = animatorHandle.GetBool("IsApplyRootMotion");
            core.UpdateLogicCore();
            if (isOnGround && core.movement.currentVelocity.y <= 0.001f)
            {
                animatorHandle.SetBool("IsOnGround", true);
            }
            else
            {
                animatorHandle.SetBool("IsOnGround", false);
            }
            animatorHandle.SetBool("IsDashing", isDashing);

            transform.localScale = new Vector3(core.movement.facingDirection, 1, 1);

            PhysicCheck();
            UpdateStatusEffects();
            HandleMovement();
            HandleJump();
            HandleFalling();
            HandleDash();
            HandleCombo();

            if (characterEquipment.currentArmor)
            {
                characterEquipment.currentArmor.OnUpdate();
            }
            if (characterEquipment.currentBoots)
            {
                characterEquipment.currentBoots.OnUpdate();
            }
            if (characterEquipment.currentNecklace)
            {
                characterEquipment.currentNecklace.OnUpdate();
            }
            if (characterEquipment.currentRing)
            {
                characterEquipment.currentRing.OnUpdate();
            }
        }

        public void FixedUpdateScript()
        {
            if (GameManager.GameState == GameState.PAUSE) return;
        }
        private void HandleCombo()
        {
            core.movement.SetGravityScale(1);
            if (characterEquipment.primaryWeapon)
            {
                characterEquipment.primaryWeapon.OnUpdate();
                input.primaryButton.UpdateFill(characterEquipment.primaryWeapon.GetDurability());
                input.primaryButton.Reload(characterEquipment.primaryWeapon.GetReloadNormalizedTime());
                if (input.primaryAttackPressed)
                {
                    if (currentWeapon == null)
                    {
                        currentWeapon = characterEquipment.primaryWeapon;
                        currentWeapon.SetActive(true);
                    }
                    else
                    {
                        if (currentWeapon != characterEquipment.primaryWeapon && isInteracting)
                        {
                            return;
                        }
                        if (currentWeapon != characterEquipment.primaryWeapon)
                        {
                            currentWeapon.SetActive(false);
                            currentWeapon = characterEquipment.primaryWeapon;
                            currentWeapon.ResetAnimation();
                            currentWeapon.SetActive(true);
                        }
                    }
                    characterEquipment.primaryWeapon.TriggerWeapon();
                }
                if (characterEquipment.primaryWeapon.isActiveCombo && currentWeapon == characterEquipment.primaryWeapon)
                {
                    core.movement.SetVelocityY(0);
                    if (core.movement.currentVelocity.y <= 0)
                        core.movement.SetGravityScale(0);
                }
            }
            if (characterEquipment.secondaryWeapon)
            {
                characterEquipment.secondaryWeapon.OnUpdate();
                input.secondaryButton.UpdateFill(characterEquipment.secondaryWeapon.GetDurability());
                input.secondaryButton.Reload(characterEquipment.secondaryWeapon.GetReloadNormalizedTime());
                if (input.secondaryAttackPressed)
                {
                    if (currentWeapon == null)
                    {
                        currentWeapon = characterEquipment.secondaryWeapon;
                        currentWeapon.SetActive(true);
                    }
                    else
                    {
                        if (currentWeapon != characterEquipment.secondaryWeapon && isInteracting)
                        {
                            return;
                        }
                        if (currentWeapon != characterEquipment.secondaryWeapon)
                        {
                            currentWeapon.SetActive(false);
                            currentWeapon = characterEquipment.secondaryWeapon;
                            currentWeapon.ResetAnimation();
                            currentWeapon.SetActive(true);
                        }
                    }
                    characterEquipment.secondaryWeapon.TriggerWeapon();
                }
                if (characterEquipment.secondaryWeapon.isActiveCombo && currentWeapon == characterEquipment.secondaryWeapon)
                {
                    core.movement.SetVelocityY(0);
                    if (core.movement.currentVelocity.y <= 0)
                        core.movement.SetGravityScale(0);
                }
            }
        }
        private void PhysicCheck()
        {
            isOnGround = false;
            SetParentPlayer(null);
            RaycastHit2D rightFoot = RayCast(new Vector2(footOffset, 0), Vector2.down, groundRayDistance, groundLayer);
            RaycastHit2D leftFoot = RayCast(new Vector2(-footOffset, 0), Vector2.down, groundRayDistance, groundLayer);
            if (rightFoot || leftFoot)
            {
                isOnGround = true;

                if (rightFoot && rightFoot.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    SetParentPlayer(rightFoot.collider.transform);
                }
                else if (leftFoot && leftFoot.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    SetParentPlayer(leftFoot.collider.transform);
                }
                if (rightFoot && leftFoot)
                {
                    lastPositionOnGround = transform.position;
                }
            }
        }
        public override void SetHealth(int amount)
        {
            base.SetHealth(amount);
            ingameScreen.ActiveWarning(NormalizeHealth() <= 0.1f);
            ingameScreen.playerHealthBar.UpdateFillBar(NormalizeHealth());
            ingameScreen.playerHealthBar.UpdateHealthText(runtimeStats.health + "/" + originalStats.health);
        }
        private void SetParentPlayer(Transform _parent)
        {
            transform.SetParent(_parent);
        }
        private void HandleMovement()
        {
            float xVelocity = player.input.horizontal * runtimeMovementSpeed;
            float minLimit = CameraController.Instance.GetMinLimitX();
            float maxLimit = CameraController.Instance.GetMaxLimitX();
            if (transform.position.x <= minLimit + 1f && xVelocity < 0 || transform.position.x >= maxLimit - 1f && xVelocity > 0)
            {
                xVelocity = 0;
            }
            if (Mathf.Abs(xVelocity) > 0 && !isRun)
            {
                isRun = true;
            }
            if (Mathf.Abs(xVelocity) == 0 && isRun)
            {
                isRun = false;
            }
            if (!isInteracting && !isStunning)
            {
                core.movement.SetVelocityX(xVelocity);
            }
            if (player.input.horizontal != 0 && player.input.horizontal != core.movement.facingDirection)
            {
                core.movement.Flip();
            }
        }

        void HandleJump()
        {
            if (isInteracting || isStunning) return;
            if (isOnGround && core.movement.currentVelocity.y <= 0.001f)
            {
                jumpCount = 0;
            }
            
            if (jumpBufferTimer > 0 && jumpCount < 2)
            {
                jumpCount++;
                AudioManager.Instance.PlayOneShot("Jump", 1f);
                core.movement.SetVelocityY(jumpForce);
                if (jumpCount > 1)
                {
                    animatorHandle.PlayAnimation("JumpRoll", 0.1f, 0);
                }
                jumpBufferTimer = 0;
            }

            if (input.jumpPressed /* && isOnGround */)
            {
                // jumpCount++;
                // isOnGround = false;
                // core.movement.SetVelocityY(jumpForce);
                // AudioManager.Instance.PlayOneShot("Jump", 1f);
                jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                jumpBufferTimer -= Time.deltaTime;
            }
            // else if (input.jumpPressed && !isOnGround && jumpCount < 2)
            // {
            //     jumpCount++;
            //     AudioManager.Instance.PlayOneShot("Jump", 1f);
            //     animatorHandle.PlayAnimation("JumpRoll", 0.1f, 0);
            //     core.movement.SetVelocityY(jumpForce);
            // }


        }
        void HandleFalling()
        {
            if (core.movement.currentVelocity.y < maxFallVelocity)
            {
                core.movement.SetVelocityY(maxFallVelocity);
            }
        }

        public override void OnTakeDamage(DamageInfo damageInfo)
        {
            if (GameManager.GameState != GameState.PLAYING) return;
            if (!isActive || isInvincible) return;
            // if (currentWeapon)
            //     currentWeapon.ResetAnimation();
            if (damageInfo.stunTime > 0)
            {
                animatorHandle.PlayAnimation("Hit", 0.1f, 1, true);
                bloodFx.Play();
            }
            if (damageInfo.impactSound)
            {
                AudioManager.Instance.PlayOneShot(damageInfo.impactSound, 1f);
            }
            else
            {
                AudioManager.Instance.PlayOneShot("eff_be_hit" + Random.Range(1, 4), 1f);
            }
            if (damageInfo.listEffect != null)
            {
                for (int i = 0; i < damageInfo.listEffect.Count; i++)
                {
                    AddStatusEffect(damageInfo.listEffect[i]);
                }
            }
            if (damageInfo.damageType != DamageType.TRUEDAMAGE)
            {
                damageInfo.damage = CharacterStats.GetDamageAfterReduction(damageInfo.damage, runtimeStats.armor);
            }
            runtimeStats.health -= damageInfo.damage;
            if (runtimeStats.health <= 0)
            {
                runtimeStats.health = 0;
                if (deathDenyCount > 0)
                {
                    runtimeStats.health = 1;
                }
            }
            ingameScreen.ActiveWarning(NormalizeHealth() <= 0.1f);
            ingameScreen.playerHealthBar.UpdateFillBar(NormalizeHealth());
            ingameScreen.playerHealthBar.UpdateHealthText(runtimeStats.health + "/" + originalStats.health);
            if (runtimeStats.health <= 0 && deathDenyCount <= 0)
            {
                runtimeStats.health = 0;
                animatorHandle.PlayAnimation("Dead", 0f, 1, false, true);
                Die(false);
                player.PlayerDie();
            }
            base.OnTakeDamage(damageInfo);
        }
        public override void AddHealth(int amount)
        {
            if (amount == 0) return;
            base.AddHealth(amount);
            ingameScreen.ActiveWarning(NormalizeHealth() <= 0.1f);
            ingameScreen.playerHealthBar.UpdateFillBar(NormalizeHealth());
            ingameScreen.playerHealthBar.UpdateHealthText(runtimeStats.health + "/" + originalStats.health);
        }
        void HandleDash()
        {
            if (input.dashPressed && dashTimer <= Time.time && dashRecoveryTimer <= Time.time)
            {
                dashTimer = Time.time + dashTime;
                dashRecoveryTimer = Time.time + dashRecoveryTime;
                character.PlayAnimation("Dash", 0.1f, 1, false, false);
                isDashing = true;
                isInvincible = true;
                AudioManager.Instance.PlayOneShot("Dash", 1f);
                dashFx.Play();
            }
            if (Time.time < dashTimer)
            {
                core.movement.SetVelocityX(dashForce * core.movement.facingDirection);
                core.movement.SetVelocityY(0);
            }
            if (Time.time >= dashTimer || input.primaryAttackPressed || input.secondaryAttackPressed)
            {
                dashFx.Stop();
                isInvincible = false;
                isDashing = false;
            }
        }
        public override void SetControllerSpeed(float speed)
        {
            base.SetControllerSpeed(speed);
        }
        RaycastHit2D RayCast(Vector2 offset, Vector2 direction, float distance, LayerMask layerMask)
        {
            RaycastHit2D raycast = Physics2D.Raycast((Vector2)transform.position + offset, direction, distance, layerMask);
#if UNITY_EDITOR
            Color color = raycast ? Color.red : Color.green;
            Debug.DrawRay((Vector2)transform.position + offset, direction * distance, color);
#endif
            return raycast;
        }
    }
}