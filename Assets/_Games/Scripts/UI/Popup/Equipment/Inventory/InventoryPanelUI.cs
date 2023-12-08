using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class InventoryPanelUI : MonoBehaviour
    {
        private EquipmentScreen equipmentScreen;
        private List<InventorySlotItem> listSlot;
        [SerializeField] Transform itemHolder;
        [SerializeField] InventorySlotItem inventorySlotPrefab;
        [SerializeField] Sprite[] statusSprites;
        [SerializeField] Button allBtn;
        [SerializeField] Button weaponBtn;
        [SerializeField] Button armorBtn;
        [SerializeField] Button bootsBtn;
        [SerializeField] Button necklaceBtn;
        [SerializeField] Button ringBtn;
        public void Initialize(EquipmentScreen equipmentScreen)
        {
            this.equipmentScreen = equipmentScreen;
            listSlot = new List<InventorySlotItem>();
            allBtn.onClick.AddListener(SelectAll);
            weaponBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.WEAPON, weaponBtn.image); });
            armorBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.ARMOR, armorBtn.image); });
            bootsBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.BOOTS, bootsBtn.image); });
            necklaceBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.NECKLACE, necklaceBtn.image); });
            ringBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.RING, ringBtn.image); });
            Inventory.OnAddItem += Refresh;
        }
        public void Show()
        {
            gameObject.SetActive(true);
            Refresh();
            if (Tutorial.TutorialStep == 4)
            {
                Tutorial.TutorialStep = 5;
                Tutorial.Instance.TutorialClick(listSlot[0].button, 0.5f, null);
            }
        }
        public void Refresh()
        {
            List<InventorySlot> inventory = new List<InventorySlot>(DataManager.Instance.data.inventoryData.inventory);
            List<ItemData> listItems = new List<ItemData>();
            for (int i = 0; i < inventory.Count; i++)
            {
                var sl = inventory[i];
                if (sl.itemInventoryID > 0)
                {
                    EquipmentData itemInfo = Inventory.Instance.GetEquipmentDataById(sl.itemInventoryID);
                    EquipmentObjectSO item = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemInfo.itemID);
                    listItems.Add(new ItemData(sl.itemInventoryID, item.id, itemInfo.level, itemInfo.grade, item.equipmentType));
                }
                else
                {
                    listItems.Add(new ItemData(-1, -1, -1, 0, EquipmentType.WEAPON));
                }
            }

            listItems = listItems.OrderByDescending(i => i.level).ThenByDescending(i => i.grade).ThenBy(i => i.type).ToList();

            for (int i = 0; i < listItems.Count; i++)
            {
                inventory[i].index = i;
                inventory[i].itemInventoryID = listItems[i].idInventory;
            }

            if (listSlot.Count > inventory.Count)
            {
                for (int i = 0; i < listSlot.Count; i++)
                {
                    if (i < inventory.Count)
                    {
                        EquipmentData data = Inventory.Instance.GetEquipmentDataById(inventory[i].itemInventoryID);
                        listSlot[i].gameObject.SetActive(true);
                        listSlot[i].SetSlot(data);
                    }
                    else
                    {
                        listSlot[i].SetSlot(null);
                        listSlot[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    EquipmentData data = Inventory.Instance.GetEquipmentDataById(inventory[i].itemInventoryID);
                    if (i < listSlot.Count)
                    {
                        listSlot[i].SetSlot(data);
                        listSlot[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        InventorySlotItem slotItem = Instantiate(inventorySlotPrefab, itemHolder);
                        slotItem.Initialize();
                        slotItem.gameObject.SetActive(true);
                        slotItem.OnClick += ShowItem;
                        slotItem.SetSlot(data);
                        listSlot.Add(slotItem);
                    }
                }
            }
            SelectAll();
        }
        private void ShowItem(InventorySlotItem slotItem)
        {
            equipmentScreen.equipmentInfoUI.SetInfo(slotItem.equipmentData, false);
        }

        public void SelectTypeEquipment(EquipmentType equipmentType, Image image)
        {
            SetStatusBtn();
            image.sprite = statusSprites[1];
            for (int i = 0; i < listSlot.Count; i++)
            {
                if (listSlot[i].IsEmpty() || listSlot[i].GetEquipmentType() != equipmentType)
                {
                    listSlot[i].gameObject.SetActive(false);
                }
                else
                {
                    listSlot[i].gameObject.SetActive(true);
                }
            }
        }

        public void SelectAll()
        {
            SetStatusBtn();
            allBtn.image.sprite = statusSprites[1];
            for (int i = 0; i < listSlot.Count; i++)
            {
                if (!listSlot[i].IsEmpty())
                {
                    listSlot[i].gameObject.SetActive(true);
                }
                else
                {
                    listSlot[i].gameObject.SetActive(false);
                }
            }
        }

        private void SetStatusBtn()
        {
            allBtn.image.sprite = statusSprites[0];
            weaponBtn.image.sprite = statusSprites[0];
            armorBtn.image.sprite = statusSprites[0];
            bootsBtn.image.sprite = statusSprites[0];
            necklaceBtn.image.sprite = statusSprites[0];
            ringBtn.image.sprite = statusSprites[0];
        }
    }
    public struct ItemData
    {
        public ItemData(int id, int itemID, int level, int grade, EquipmentType type)
        {
            this.idInventory = id;
            this.idItem = itemID;
            this.level = level;
            this.grade = grade;
            this.type = type;
        }
        public int idInventory;
        public int idItem;
        public int level;
        public int grade;
        public EquipmentType type;
    }
}
