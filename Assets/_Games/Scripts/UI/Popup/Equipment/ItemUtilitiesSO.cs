using UnityEngine;
namespace SuperFight
{
    [CreateAssetMenu(fileName = "ItemUtilities", menuName = "Utilities/ItemUtilities")]
    public class ItemUtilitiesSO : ScriptableObject
    {
        [SerializeField] Sprite[] gradeBackGrounds;
        [SerializeField] Sprite[] gradeTypes;
        [SerializeField] Sprite[] gradeLevels;
        [SerializeField] Sprite[] typeItems;
        public Sprite GetGradeBackGround(int level)
        {
            return gradeBackGrounds[level - 1];
        }
        public Sprite GetGradeType(int level)
        {
            return gradeTypes[level - 1];
        }
        public Sprite GetGradeLevel(int level)
        {
            return gradeLevels[level - 1];
        }
        public Sprite GetTypeItem(EquipmentType equipmentType)
        {
            return typeItems[(int)equipmentType];
        }
    }
}
