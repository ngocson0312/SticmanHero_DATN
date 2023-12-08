using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class ElderRoamingState : State
    {
        private BossElderDragon bossElderDragon;
        private float roamingTimer;
        private int indexPos;
        private float idleTimer;
        int maxAttackInTurn;
        private Transform animatorTransform;
        List<Collider2D> collider2Ds;

        [Header("Roaming")]
        public float roamingTime = 2.7f;
        private int roamingCount;
        private bool breakState;
        public ElderRoamingState(BossElderDragon controller, string stateName) : base(controller, stateName)
        {
            this.bossElderDragon = controller;
            animatorTransform = controller.animatorHandle.transform;
            indexPos = 0;


        }

        public override void EnterState()
        {
            maxAttackInTurn = Random.Range(5, 7);
            idleTimer = 1f;
            collider2Ds = new List<Collider2D>();
            //state = 0;
            roamingTimer = roamingTime;
            roamingCount = Random.Range(2, 3);
            breakState = false;

        }

        public override void ExitState()
        {
            indexPos++;
            if (indexPos >= bossElderDragon.posMoves.Length)
            {
                indexPos = 0;
            }
        }

        public override void UpdateLogic()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer > 0) return;
            HandleFlyAround();
        }


        void HandleFlyAround()
        {
            bossElderDragon.animatorHandle.SetBool("IsWalk", false);
            // bossElderDragon.animatorHandle.SetFloat("MoveAmount", 1f);
            if (roamingTimer > 0)
            {
                roamingTimer -= Time.deltaTime;
                if (roamingTimer <= 0)
                {
                    indexPos++;
                    if (indexPos >= bossElderDragon.posMoves.Length)
                    {
                        bossElderDragon.SwitchState(bossElderDragon.meleeState);
                        breakState = true;
                    }
                    roamingTimer = roamingTime;
                    roamingCount--;
                    collider2Ds = new List<Collider2D>();
                    if (roamingCount <= 0)
                    {
                        // state = 1;
                        //spawnTimer = 1f;
                        // bossElderDragon.turnCount = Random.Range(1, 2);
                        bossElderDragon.flame.Stop();
                    }
                }
            }

            if (Vector2.Distance(transform.position, bossElderDragon.posMoves[indexPos].position) > 0.5f && breakState == false)
            {
                Vector3 dir = bossElderDragon.posMoves[indexPos].position - transform.position;
                transform.position += dir.normalized * bossElderDragon.speed * Time.deltaTime;
                if (dir.x < 0)
                {
                    bossElderDragon.direction = -1;
                    bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
                }
                else if (dir.x > 0)
                {
                    bossElderDragon.direction = 1;
                    bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
                }
                HandleBreathFlame();
                bossElderDragon.flame.Play();
                HandleRotation(dir.x);

                bossElderDragon.animatorHandle.SetFloat("MoveAmount", 1f);
            }
            else
            {
                if (bossElderDragon.direction == -1)
                {
                    bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
                }
                else
                {
                    bossElderDragon.animatorHandle.transform.rotation = Quaternion.Lerp(bossElderDragon.animatorHandle.transform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
                }
                bossElderDragon.flame.Stop();
                // bossElderDragon.animatorHandle.SetFloat("MoveAmount", 0f);
            }

        }

        void HandleBreathFlame()
        {
            RaycastHit2D[] r = Physics2D.RaycastAll(bossElderDragon.flamePosition.position, bossElderDragon.flamePosition.forward, bossElderDragon.rayLenght, bossElderDragon.targetLayer);
            List<Collider2D> targets = new List<Collider2D>();
            for (int i = 0; i < r.Length; i++)
            {
                if (!core.combat.IsSelfCollider(r[i].collider))
                {
                    targets.Add(r[i].collider);
                }
            }
            if (targets.Count > 0 && !collider2Ds.Contains(targets[0]))
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = bossElderDragon.runtimeStats.damage;
                damageInfo.isKnockBack = true;
                damageInfo.hitDirection = bossElderDragon.direction;
                damageInfo.stunTime = 1f;
                targets[0].transform.GetComponent<IDamage>()?.TakeDamage(damageInfo);
                collider2Ds.Add(targets[0]);
            }
        }


        private void HandleRotation(float direction)
        {
            if (direction < 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 240, 0), 0.2f);
            }
            else if (direction > 0)
            {
                animatorTransform.rotation = Quaternion.Lerp(animatorTransform.rotation, Quaternion.Euler(0, 110, 0), 0.2f);
            }
        }
        public override void UpdatePhysic()
        {

        }
    }
}
