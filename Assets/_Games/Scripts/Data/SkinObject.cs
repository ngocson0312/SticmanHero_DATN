using UnityEditor;
using UnityEngine;
namespace SuperFight
{
    [CreateAssetMenu(fileName = "SkinObject", menuName = "Data/SkinObject", order = 0)]
    public class SkinObject : ScriptableObject
    {

        
        
        public int id;
        public string skinName;
        public Sprite avatar;
        public Character character;

        public int bonusDamage, bonusHealth, bonusArmor, bonusCritical, bonusCritdamage;
        
        private void OnValidate()
        {
           
        }

    }
    
}
