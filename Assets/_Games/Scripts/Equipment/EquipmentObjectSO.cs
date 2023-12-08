using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
    [CreateAssetMenu(fileName = "EquipmentObject", menuName = "Data/EquipmentObject", order = 0)]
    public class EquipmentObjectSO : ItemObjectSO
    {
        public Equipment prefab;
        public EquipmentType equipmentType;
        public string[] gradeSkillDesc;
    }
}
