using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    [CreateAssetMenu(fileName = "RewardMission", menuName = "Data/RewardMission", order = 0)]
    public class RewardMission : ScriptableObject
    {
        public ItemRewardMission[] itemRewards;
    }

    [System.Serializable]
    public class ItemRewardMission
    {
        public TypeGift type;
        public int amout;
        public EquipmentData equipmentData;
    }
}
