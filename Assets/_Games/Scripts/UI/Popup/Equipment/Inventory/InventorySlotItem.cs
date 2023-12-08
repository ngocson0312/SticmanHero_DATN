using System;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class InventorySlotItem : MonoBehaviour
    {
        [SerializeField] Image backgroundGrade;
        [SerializeField] Image icon;
        [SerializeField] Image typeBG;
        [SerializeField] Image type;
        [SerializeField] Text levelText;
        [SerializeField] ItemUtilitiesSO utilities;
        public EquipmentData equipmentData { get; private set; }
        public event Action<InventorySlotItem> OnClick;
        public Button button { get; private set; }
        public void Initialize()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Click);
            Inventory.OnModifyEquipment += Refresh;
            Inventory.OnDeleteItem += OnDelete;
        }
        private void OnDelete(EquipmentData data)
        {
            if (IsEmpty()) return;
            if (data.idInventory == equipmentData.idInventory)
            {
                equipmentData = null;
                gameObject.SetActive(false);
            }
        }
        private void Refresh(EquipmentData data)
        {
            if (IsEmpty()) return;
            if (data.idInventory == equipmentData.idInventory)
            {
                EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(equipmentData.itemID);
                levelText.text = "LV." + equipmentData.level;
                icon.sprite = equipmentObject.icon;
                type.sprite = utilities.GetTypeItem(equipmentObject.equipmentType);
                typeBG.sprite = utilities.GetGradeType(equipmentData.grade);
                backgroundGrade.sprite = utilities.GetGradeBackGround(equipmentData.grade);
            }
        }
        private void Click()
        {
            OnClick?.Invoke(this);
        }
        public void SetSlot(EquipmentData equipmentData)
        {
            this.equipmentData = equipmentData;
            if (equipmentData == null)
            {
                return;
            }
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(equipmentData.itemID);
            levelText.text = "LV." + equipmentData.level;
            icon.sprite = equipmentObject.icon;
            type.sprite = utilities.GetTypeItem(equipmentObject.equipmentType);
            typeBG.sprite = utilities.GetGradeType(equipmentData.grade);
            backgroundGrade.sprite = utilities.GetGradeBackGround(equipmentData.grade);
        }
        public EquipmentType GetEquipmentType()
        {
            return DataManager.Instance.equipmentContainer.GetEquipmentObject(equipmentData.itemID).equipmentType;
        }
        public bool IsEmpty()
        {
            return equipmentData == null;
        }
        public int GetItemInventoryID()
        {
            if (!IsEmpty())
            {
                return equipmentData.idInventory;
            }
            return -1;
        }
    }
}
