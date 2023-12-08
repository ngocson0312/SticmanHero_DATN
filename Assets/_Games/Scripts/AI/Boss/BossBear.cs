using DG.Tweening;

using SuperFight;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class BossBear : Boss
{
    public ParticleSystem smokeDust;
    public ParticleSystem growlFx;
    public ParticleSystem slashFx;
    public ParticleSystem flameFx;
    public ParticleSystem fxDeath;
    public Vector2 sizeGroundLand;
    public Vector2 offsetLandCollider;

    public Vector2 sizeSlash;
    public Vector2 offsetSlashCollider;

    public Vector2 sizeFlame;
    public Vector2 offsetFlameCollider;
    public Transform[] spikePos;
    public GameObject spikeUp;
    public GameObject door;
    private List<Transform> curSpike = new List<Transform>();
    private int dir;
    public override void Initialize(BossFightArena arena)
    {
        base.Initialize(arena);
        animatorHandle.OnEventAnimation += OnEventAnim;
        door.SetActive(true);
    }
    private void OnEventAnim(string obj)
    {
        if (obj.Equals("Jump"))
        {
            JumpToPlayer();
        }
        if (obj.Equals("Growl"))
        {
            Growl();
        }
        if (obj.Equals("Slash"))
        {
            Slash();
        }
        if (obj.Equals("Flame"))
        {
            Flame();
        }
    }

    void Action1()
    {
        animatorHandle.PlayAnimation("Action3", 0.1f, 1, true);
    }

    IEnumerator AttackAcction()
    {
        yield return new WaitForSeconds(2.5f);
        Action1();
        yield return new WaitForSeconds(2.5f);
        Action1();
        yield return new WaitForSeconds(5);
        StartCoroutine(AttackAcction());
    }

    public void JumpToPlayer()
    {
        CheckRotate();
        float time = 0.5f;
        float playerPos = PlayerManager.Instance.transform.position.x;
        transform.DOMoveY(transform.position.y + 5, time).SetEase(Ease.OutQuart).SetLoops(2, LoopType.Yoyo);
        transform.DOMoveX(playerPos, time * 2).SetEase(Ease.Linear).OnComplete(() =>
        {
            Landing();
        });
    }

    public void Landing()
    {
        smokeDust.Play();
        CheckRotate();
        CameraController.Instance.ShakeCamera(.5f, 1f, 10, 90, true);
        SpawnSpike();
        Vector3 direction = new Vector3(dir, 0);
        Vector3 point = transform.position + new Vector3(offsetLandCollider.x * direction.x, offsetLandCollider.y, 0);
        Bounds b = new Bounds(point, sizeGroundLand);

        DamageInfo damageInfo = new DamageInfo();
        damageInfo.damage = runtimeStats.damage;
        damageInfo.owner = this;
        damageInfo.characterType = characterType;
        if (b.Intersects(PlayerManager.GetBoundPlayer()))
        {
            PlayerManager.Instance.playerController.OnTakeDamage(damageInfo);
        }
    }
    public override void Resume()
    {
        base.Resume();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }
    public void Growl()
    {
        growlFx.Play();
        CameraController.Instance.ShakeCamera(1.5f, 0.5f, 10, 90, true);
        for (int i = 0; i < 5; i++)
        {
            ItemSpikeFallTrap[] a = curSpike[i].GetChild(0).GetComponentsInChildren<ItemSpikeFallTrap>();
            for (int j = 0; j < a.Length; j++)
            {
                a[j].Active();
            }
        }
    }

    public void Slash()
    {
        slashFx.Play();
        Vector3 direction = new Vector3(dir, 0);
        Vector3 point = transform.position + new Vector3(offsetSlashCollider.x * direction.x, offsetSlashCollider.y, 0);
        Bounds b = new Bounds(point, sizeSlash);

        DamageInfo damageInfo = new DamageInfo();
        damageInfo.damage = runtimeStats.damage;
        damageInfo.owner = this;
        damageInfo.characterType = characterType;

        if (b.Intersects(PlayerManager.GetBoundPlayer()))
        {
            PlayerManager.Instance.playerController.OnTakeDamage(damageInfo);
        }
    }

    public void Flame()
    {
        flameFx.Play();
        float dealDmgTimer = 0;

        DamageInfo damageInfo = new DamageInfo();
        damageInfo.damage = (int)(runtimeStats.damage * 0.2f);
        damageInfo.owner = this;
        damageInfo.stunTime = 0.1f;
        Vector3 direction = new Vector3(dir, 0);
        Vector3 point = transform.position + new Vector3(offsetFlameCollider.x * direction.x, offsetFlameCollider.y, 0);
        Bounds b = new Bounds(point, sizeFlame);
        StartCoroutine(IEFlame());
        IEnumerator IEFlame()
        {
            float timer = 1;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                dealDmgTimer -= Time.deltaTime;
                if (dealDmgTimer <= 0 && b.Intersects(PlayerManager.GetBoundPlayer()))
                {
                    dealDmgTimer = 0.2f;
                    PlayerManager.Instance.playerController.OnTakeDamage(damageInfo);
                }
                yield return null;
            }
        }
    }

    void SpawnSpike()
    {
        for (int i = 0; i < curSpike.Count; i++)
        {
            Destroy(curSpike[i].GetChild(0).gameObject);
        }
        curSpike.Clear();

        List<Transform> a = spikePos.ToList();
        for (int i = 0; i < 5; i++)
        {
            int RanNum = Random.Range(0, a.Count);
            curSpike.Add(a[RanNum]);
            GameObject b = Instantiate(spikeUp, a[RanNum]);
            b.transform.localPosition = Vector3.zero + Vector3.up * 2;
            b.transform.DOLocalMove(Vector3.zero, 0.3f);
            a.RemoveAt(RanNum);
        }
    }

    void CheckRotate()
    {
        float playerPos = PlayerManager.Instance.transform.position.x;
        if (playerPos <= transform.position.x)
        {
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 220, 0);
            dir = -1;
        }
        else
        {
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 140, 0);
            dir = 1;
        }
    }

    public override void OnTakeDamage(DamageInfo damageInfo)
    {
        if (!isActive) return;
        base.OnTakeDamage(damageInfo);
        if (runtimeStats.health <= 0)
        {
            animatorHandle.PlayAnimation("Die", 0.1f, 1, true);
            door.SetActive(false);
            Die(true);
        }
        else
        {
            animatorHandle.PlayAnimation("Hit", 0.1f, 0, true);
        }
    }

    public override void Active()
    {
        isActive = true;
        // boxSlash.GetComponent<Electric>().SetDamage(stats.damage);
        // flameFx.GetComponent<Laser>().SetDamage(stats.damage / 3);
        StartCoroutine(AttackAcction());
        SpawnSpike();
        CheckRotate();
    }

    protected override void UpdateLogic()
    {
    }

    protected override void UpdatePhysic()
    {
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)offsetLandCollider, sizeGroundLand);
        Gizmos.DrawWireCube(transform.position + (Vector3)offsetSlashCollider, sizeSlash);
        Gizmos.DrawWireCube(transform.position + (Vector3)offsetFlameCollider, sizeFlame);
    }
}
