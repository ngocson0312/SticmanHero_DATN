using PathologicalGames;
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Projectile : MonoBehaviour
{
    public float speed;
    public GameObject display;
    public GameObject impact;
    public bool isActive;
    protected bool isPooled;
    protected DamageInfo damageInfo;
    protected Vector2 direction;
    public LayerMask layerContact;
    private float lifeTimer;
    public event Action<List<IDamage>> OnContact;
    protected Collider2D damageCollider;
    public virtual void Initialize(Vector2 direction, DamageInfo damageInfo)
    {
        this.damageInfo = damageInfo;
        this.direction = direction;
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
        }
        damageCollider.enabled = true;
        if (display != null)
            display.SetActive(true);
        if (impact != null)
            impact.SetActive(false);
        isActive = true;
        isPooled = false;
        lifeTimer = 0;
    }
    public virtual void UpdateLogic()
    {
        if (isActive)
        {
            transform.position += speed * Time.deltaTime * (Vector3)direction;
        }
    }
    private void Update()
    {
        UpdateLogic();
        HandleLifeTime();
    }
    protected abstract void OnCollision(Collider2D other);
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnCollision(other);
    }
    protected void Deactive(float time)
    {
        if (isPooled) return;
        lifeTimer = time;
        if (time <= 0)
        {
            isPooled = true;
            PoolManager.Pools["Projectile"].Despawn(transform, PoolManager.Pools["Projectile"].transform);
            OnContact = null;
        }
    }
    private void HandleLifeTime()
    {
        if (lifeTimer > 0 && !isPooled)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0)
            {
                lifeTimer = 0;
                Deactive(0);
            }
        }
    }
    protected void OnContactCollision(List<IDamage> listContact)
    {
        OnContact?.Invoke(listContact);
        OnContact = null;
    }
}
