using System;
using System.Collections.Generic;
using SuperFight;
using UnityEngine;
public interface IDamage
{
    Controller controller { get; }
    void TakeDamage(DamageInfo damageInfo);
}
[System.Serializable]
public struct DamageInfo
{
    public int damage;
    public DamageType damageType;
    public int hitDirection;
    public bool isKnockBack;
    public Vector2 stunForce;
    public float stunTime;
    public int idSender;
    public CharacterType characterType;
    public AudioClip impactSound;
    public Controller owner;
    public Action<bool> onHitSuccess;
    public List<StatusEffectData> listEffect;
}
public enum CharacterType
{
    Mob, Character, Boss
}
public enum DamageType
{
    NORMAL, CRITICAL, TRUEDAMAGE
}