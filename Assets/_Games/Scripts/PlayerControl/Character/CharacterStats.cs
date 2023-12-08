using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    [System.Serializable]
    public class CharacterStats
    {
        public CharacterStats()
        {
            Reset();
        }
        public void Reset()
        {
            health = 0;
            damage = 0;
            armor = 0;
            critRate = 0;
            critDamage = 0;
            lifeSteal = 0;
            exp = 0;
        }
        public static int GetDamageAfterReduction(int damage, int armor)
        {
            // float delta = 0.06f;
            // float damageReduce = 1 - ((delta * armor) / (1 + delta * Mathf.Abs(armor)));
            // damage = (int)(damage * damageReduce);
            damage = (int)(damage * (100f / (100f + armor)));
            return damage;
        }
        public CharacterStats(CharacterStats reference)
        {
            this.health = reference.health;
            this.damage = reference.damage;
            this.armor = reference.armor;
            this.critRate = reference.critRate;
            this.critDamage = reference.critDamage;
            this.lifeSteal = reference.lifeSteal;
            this.exp = reference.exp;
        }
        public static CharacterStats operator +(CharacterStats a, CharacterStats b)
        {
            CharacterStats newStats = new CharacterStats();
            newStats.health = a.health + b.health;
            newStats.damage = a.damage + b.damage;
            newStats.armor = a.armor + b.armor;
            newStats.critRate = a.critRate + b.critRate;
            newStats.critDamage = a.critDamage + b.critDamage;
            newStats.lifeSteal = a.lifeSteal + b.lifeSteal;
            newStats.exp = a.exp + b.exp;
            return newStats;
        }
        public int health;
        public int damage;
        public int armor;
        public int critRate;
        public int critDamage;
        public int lifeSteal;
        public int exp;
    }
}
