using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    [CreateAssetMenu(fileName = "EquipmentContainer", menuName = "Data/EquipmentContainer")]
    public class EquipmentContainerSO : ScriptableObject
    {
        public EquipmentObjectSO defaultWeapon;
        public EquipmentObjectSO[] container;
        public EquipmentObjectSO GetEquipmentObject(int id)
        {
            for (int i = 0; i < container.Length; i++)
            {
                if (container[i].id == id)
                {
                    return container[i];
                }
            }
            return null;
        }
    }
}
