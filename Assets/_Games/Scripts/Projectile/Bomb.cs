using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;
using SuperFight;

public class Bomb : MonoBehaviour
{
    public float speed;
    private DamageInfo damageInfo;
    public ParticleSystem explosionFx;
    public GameObject display;
    public Rigidbody2D rb2d;

    public void ActiveBomb(Vector3 target, DamageInfo _damageInfo)
    {
        damageInfo = _damageInfo;
        Vector3 force = GetForce(45, target);
        force.x *= _damageInfo.hitDirection;
        rb2d.velocity = force;
        // float distance = Vector3.Distance(transform.position, target);
        // if (distance >= 3.2f)
        // {
        //     float time = distance / speed;
        //     transform.DOMoveY(transform.position.y + 2, time).SetEase(Ease.OutQuad).OnComplete(() =>
        //     {
        //         transform.DOMoveY(target.y, time).SetEase(Ease.InQuad).OnComplete(() =>
        //         {
        //             Explosion();
        //         });
        //     });
        //     transform.DOMoveX(target.x, time * 2).SetEase(Ease.Linear);
        // }
        // else
        // {
        //     transform.DOMove(target + Vector3.up, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        //     {
        //         Explosion();
        //     });
        // }
    }
    private Vector3 GetForce(float angle, Vector3 targetPosition)
    {
        Vector3 projectileXZPos = new Vector3(transform.position.x, 0.0f, 0);
        Vector3 targetXZPos = new Vector3(targetPosition.x, 0.0f, 0);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics2D.gravity.y;
        float tanAlpha = Mathf.Tan(angle * Mathf.Deg2Rad);
        float H = 0;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(Vz, Vy, 0);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        return globalVelocity;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamage idamage = other.GetComponent<IDamage>();
        if (idamage == null) return;
        if (idamage.controller != damageInfo.owner && idamage.controller.characterType != damageInfo.characterType)
        {
            other.GetComponent<IDamage>()?.TakeDamage(damageInfo);
            Explosion();
        }
    }

    void Explosion()
    {
        display.SetActive(false);
        explosionFx.Play();
        transform.DOKill();
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }
}
