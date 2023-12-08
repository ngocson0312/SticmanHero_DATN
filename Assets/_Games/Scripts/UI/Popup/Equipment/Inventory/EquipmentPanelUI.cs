using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class EquipmentPanelUI : MonoBehaviour
    {
        [Header("Refs")]
        private EquipmentScreen equipmentScreen;
        private List<InventorySlotItem> listEquipmentSlot;
        public InventorySlotItem itemEquipPrefab;
        [SerializeField] ItemUtilitiesSO itemUtilities;
        [Header("Anchor")]
        [SerializeField] private Transform primary;
        [SerializeField] private Transform secondary;
        [SerializeField] private Transform armor;
        [SerializeField] private Transform boots;
        [SerializeField] private Transform ring;
        [SerializeField] private Transform necklace;
        [Header("Slot")]
        [SerializeField] private InventorySlotItem primaryEquip;
        [SerializeField] private InventorySlotItem secondaryEquip;
        [SerializeField] private InventorySlotItem armorEquip;
        [SerializeField] private InventorySlotItem bootsEquip;
        [SerializeField] private InventorySlotItem ringEquip;
        [SerializeField] private InventorySlotItem necklaceEquip;
        [SerializeField] private Button warningSecondary;
        public void Initialize(EquipmentScreen equipmentScreen)
        {
            this.equipmentScreen = equipmentScreen;
            listEquipmentSlot = new List<InventorySlotItem>();
            for (int i = 0; i < DataManager.Instance.data.inventoryData.equipment.Count; i++)
            {
                InventorySlot inventorySlot = DataManager.Instance.data.inventoryData.equipment[i];
                if (inventorySlot.itemInventoryID > 0)
                {
                    OnEquip(Inventory.Instance.GetEquipmentDataById(inventorySlot.itemInventoryID));
                }
            }
            warningSecondary.onClick.AddListener(() =>
            {
                warningSecondary.gameObject.SetActive(false);
            });
        }
        public void OnEquip(EquipmentData itemData)
        {
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            Sprite rank = itemUtilities.GetGradeBackGround(itemData.grade);
            int indexSlot = 0;
            int currentItemID = -1;
            if (equipmentObject.equipmentType == EquipmentType.WEAPON)
            {
                if (primaryEquip == null)
                {
                    primaryEquip = Instantiate(itemEquipPrefab, primary);
                    primaryEquip.Initialize();
                    primaryEquip.OnClick += ShowInfo;
                    primaryEquip.gameObject.SetActive(true);
                    primaryEquip.SetSlot(itemData);
                    indexSlot = 0;
                    listEquipmentSlot.Add(primaryEquip);
                }
                else
                {
                    if (primaryEquip.IsEmpty())
                    {
                        currentItemID = primaryEquip.GetItemInventoryID();
                        primaryEquip.gameObject.SetActive(true);
                        primaryEquip.SetSlot(itemData);
                        indexSlot = 0;
                    }
                    else
                    {
                        if (secondaryEquip == null)
                        {
                            secondaryEquip = Instantiate(itemEquipPrefab, secondary);
                            secondaryEquip.Initialize();
                            secondaryEquip.OnClick += ShowInfo;
                            listEquipmentSlot.Add(secondaryEquip);
                        }
                        warningSecondary.gameObject.SetActive(true);
                        currentItemID = secondaryEquip.GetItemInventoryID();
                        secondaryEquip.gameObject.SetActive(true);
                        secondaryEquip.SetSlot(itemData);
                        indexSlot = 1;
                    }
                }
            }
            else if (equipmentObject.equipmentType == EquipmentType.ARMOR)
            {
                if (armorEquip == null)
                {
                    armorEquip = Instantiate(itemEquipPrefab, armor);
                    armorEquip.Initialize();
                    armorEquip.OnClick += ShowInfo;
                    listEquipmentSlot.Add(armorEquip);
                }
                currentItemID = armorEquip.GetItemInventoryID();
                armorEquip.gameObject.SetActive(true);
                armorEquip.SetSlot(itemData);
                indexSlot = 2;
            }
            else if (equipmentObject.equipmentType == EquipmentType.BOOTS)
            {
                if (bootsEquip == null)
                {
                    bootsEquip = Instantiate(itemEquipPrefab, boots);
                    bootsEquip.Initialize();
                    bootsEquip.OnClick += ShowInfo;
                    listEquipmentSlot.Add(bootsEquip);
                }
                currentItemID = bootsEquip.GetItemInventoryID();
                bootsEquip.gameObject.SetActive(true);
                bootsEquip.SetSlot(itemData);
                indexSlot = 3;
            }
            else if (equipmentObject.equipmentType == EquipmentType.RING)
            {
                if (ringEquip == null)
                {
                    ringEquip = Instantiate(itemEquipPrefab, ring);
                    ringEquip.Initialize();
                    ringEquip.OnClick += ShowInfo;
                    listEquipmentSlot.Add(ringEquip);
                }
                currentItemID = ringEquip.GetItemInventoryID();
                ringEquip.gameObject.SetActive(true);
                ringEquip.SetSlot(itemData);
                indexSlot = 4;
            }
            else if (equipmentObject.equipmentType == EquipmentType.NECKLACE)
            {
                if (necklaceEquip == null)
                {
                    necklaceEquip = Instantiate(itemEquipPrefab, necklace);
                    necklaceEquip.Initialize();
                    necklaceEquip.OnClick += ShowInfo;
                    listEquipmentSlot.Add(necklaceEquip);
                }
                currentItemID = necklaceEquip.GetItemInventoryID();
                necklaceEquip.gameObject.SetActive(true);
                necklaceEquip.SetSlot(itemData);
                indexSlot = 5;
            }
            Inventory.Instance.OnEquipItem(new InventorySlot(indexSlot, itemData.idInventory), new InventorySlot(indexSlot, currentItemID));
        }
        private void ShowInfo(InventorySlotItem inventorySlotItem)
        {
            equipmentScreen.equipmentInfoUI.SetInfo(inventorySlotItem.equipmentData, true);
        }
        public void UnEquip(EquipmentData itemData)
        {
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            for (int i = 0; i < listEquipmentSlot.Count; i++)
            {
                if (listEquipmentSlot[i].GetItemInventoryID() == itemData.idInventory)
                {
                    listEquipmentSlot[i].gameObject.SetActive(false);
                    listEquipmentSlot[i].SetSlot(null);
                    break;
                }
            }
            if (primaryEquip && secondaryEquip)
            {
                if (primaryEquip.IsEmpty() && !secondaryEquip.IsEmpty())
                {
                    EquipmentData dt = new EquipmentData(secondaryEquip.equipmentData);
                    secondaryEquip.gameObject.SetActive(false);
                    secondaryEquip.SetSlot(null);
                    Inventory.Instance.OnUnEquipItem(dt);
                    OnEquip(dt);
                }
            }
            Inventory.Instance.OnUnEquipItem(itemData);
        }
    }
}
