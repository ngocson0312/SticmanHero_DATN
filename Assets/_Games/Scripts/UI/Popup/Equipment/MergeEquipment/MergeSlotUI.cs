using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class MergeSlotUI : MonoBehaviour
    {
        [SerializeField] Image backgroundGrade;
        [SerializeField] Image icon;
        [SerializeField] Text levelText;
        public bool isMainSlot;
        public EquipmentData equipmentData { get; private set; }
        public InventorySlotItem inventorySlot { get; private set; }
        private MergeEquipmentPanelUI mergeEquipment;
        public void Initialize(MergeEquipmentPanelUI mergeEquipment)
        {
            this.mergeEquipment = mergeEquipment;
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (isMainSlot)
            {
                mergeEquipment.ResetMerge();
            }
            else
            {
                ResetSlot();
                mergeEquipment.UndoStep(inventorySlot);
            }
        }

        public void SetSlot(InventorySlotItem inventorySlot, bool isMainSlot)
        {
            this.inventorySlot = inventorySlot;
            this.equipmentData = inventorySlot.equipmentData;
            this.isMainSlot = isMainSlot;
            inventorySlot.gameObject.SetActive(false);
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(equipmentData.itemID);
            backgroundGrade.enabled = true;
            icon.enabled = true;

            levelText.text = "Lv." + equipmentData.level;
            backgroundGrade.sprite = mergeEquipment.GetGradeBackGround(equipmentData.grade);
            icon.sprite = equipmentObject.icon;
        }
        public void ResetSlot()
        {
            levelText.text = "";
            backgroundGrade.enabled = false;
            icon.enabled = false;
            equipmentData = null;
        }
        public bool IsEmpty()
        {
            return equipmentData == null;
        }
    }
}
