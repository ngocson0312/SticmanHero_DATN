using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace SuperFight
{
    public class MergeEquipmentPanelUI : MonoBehaviour
    {
        [SerializeField] Transform itemHolder;
        [SerializeField] InventorySlotItem inventorySlotPrefab;
        [SerializeField] ItemUtilitiesSO utilities;
        private EquipmentScreen equipmentScreen;
        public List<InventorySlotItem> listSlot;
        [SerializeField] Sprite[] statusSprites;
        [Header("Merge")]
        private int mergeStep;
        private EquipmentData itemData;
        public MergeSlotUI itemMainSlot;
        public MergeSlotUI itemMaterialSlot1;
        public MergeSlotUI itemMaterialSlot2;
        [Header("New Item")]
        public Button previewItemBtn;
        public Image gradeImg;
        public Image iconImg;
        public Text levelText;
        public MergeNewItemDisplay itemDisplay;
        [Header("Button")]
        public Button closeBtn;
        public Button mergeBtn;
        [SerializeField] Button allBtn;
        [SerializeField] Button weaponBtn;
        [SerializeField] Button armorBtn;
        [SerializeField] Button bootsBtn;
        [SerializeField] Button necklaceBtn;
        [SerializeField] Button ringBtn;
        public void Initialize(EquipmentScreen equipmentScreen)
        {
            this.equipmentScreen = equipmentScreen;
            closeBtn.onClick.AddListener(Hide);
            mergeBtn.onClick.AddListener(MergeItem);
            previewItemBtn.onClick.AddListener(PreviewItem);
            allBtn.onClick.AddListener(SelectAll);
            weaponBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.WEAPON, weaponBtn.image); });
            armorBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.ARMOR, armorBtn.image); });
            bootsBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.BOOTS, bootsBtn.image); });
            necklaceBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.NECKLACE, necklaceBtn.image); });
            ringBtn.onClick.AddListener(() => { SelectTypeEquipment(EquipmentType.RING, ringBtn.image); });
            SelectAll();
            itemMainSlot.Initialize(this);
            itemMaterialSlot1.Initialize(this);
            itemMaterialSlot2.Initialize(this);
            listSlot = new List<InventorySlotItem>();

        }

        private void PreviewItem()
        {
            if (itemData == null) return;
            EquipmentData equipmentData = new EquipmentData(itemData);
            equipmentData.grade++;
            equipmentScreen.equipmentInfoUI.SetInfo(equipmentData, false, false);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
        public void ShowUpgrade()
        {
            gameObject.SetActive(true);
            itemDisplay.gameObject.SetActive(false);
            ResetMerge();
            SelectAll();
        }
        private void ShowItems()
        {
            List<EquipmentData> inventory = new List<EquipmentData>(DataManager.Instance.data.inventoryData.itemsOwned);
            List<ItemData> listItems = new List<ItemData>();
            for (int i = 0; i < inventory.Count; i++)
            {
                var sl = inventory[i];
                if (sl.idInventory > 0 && sl.grade < 6)
                {
                    EquipmentObjectSO item = DataManager.Instance.equipmentContainer.GetEquipmentObject(sl.itemID);
                    listItems.Add(new ItemData(sl.idInventory, item.id, sl.level, sl.grade, item.equipmentType));
                }
                else
                {
                    listItems.Add(new ItemData(-1, -1, -1, 0, EquipmentType.WEAPON));
                }
            }
            listItems = listItems.OrderByDescending(i => i.grade).ThenBy(i => i.idItem).ToList();
            if (listSlot.Count > inventory.Count)
            {
                for (int i = 0; i < listSlot.Count; i++)
                {
                    if (i < inventory.Count)
                    {
                        EquipmentData data = Inventory.Instance.GetEquipmentDataById(listItems[i].idInventory);
                        listSlot[i].SetSlot(data);
                        listSlot[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        listSlot[i].gameObject.SetActive(false);
                        listSlot[i].SetSlot(null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    EquipmentData data = Inventory.Instance.GetEquipmentDataById(listItems[i].idInventory);
                    if (i < listSlot.Count)
                    {
                        listSlot[i].SetSlot(data);
                        listSlot[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        InventorySlotItem slotItem = Instantiate(inventorySlotPrefab, itemHolder);
                        slotItem.Initialize();
                        slotItem.OnClick += OnSelectItem;
                        slotItem.SetSlot(data);
                        slotItem.gameObject.SetActive(true);
                        listSlot.Add(slotItem);
                    }
                }
            }
        }
        private void OnSelectItem(InventorySlotItem slotItem)
        {
            if (mergeStep == 0)
            {
                mergeStep = 1;
                itemData = slotItem.equipmentData;
                itemMainSlot.SetSlot(slotItem, true);
                SortFindItems();
                ShowNewItem();
            }
            else if (mergeStep == 1)
            {
                if (itemMaterialSlot1.IsEmpty())
                {
                    itemMaterialSlot1.SetSlot(slotItem, false);
                }
                else if (itemMaterialSlot2.IsEmpty())
                {
                    itemMaterialSlot2.SetSlot(slotItem, false);

                }
                if (!itemMaterialSlot1.IsEmpty() && !itemMaterialSlot2.IsEmpty())
                {
                    mergeStep = 2;
                    mergeBtn.gameObject.SetActive(true);
                }
            }
        }
        private void ShowNewItem()
        {
            gradeImg.enabled = true;
            iconImg.enabled = true;
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            EquipmentData equipmentData = new EquipmentData(itemData);
            equipmentData.grade++;
            gradeImg.sprite = GetGradeBackGround(equipmentData.grade);
            iconImg.sprite = equipmentObject.icon;
            levelText.text = "Lv.1";
            itemDisplay.Display(GetGradeBackGround(equipmentData.grade), equipmentObject.icon, 1);
        }
        public Sprite GetGradeBackGround(int grade)
        {
            return utilities.GetGradeBackGround(grade);
        }
        public void UndoStep(InventorySlotItem inventorySlot)
        {
            if (!inventorySlot) return;
            mergeStep = 1;
            inventorySlot.gameObject.SetActive(true);
            mergeBtn.gameObject.SetActive(false);
            SelectAll();
        }
        public void ResetMerge()
        {
            mergeStep = 0;
            itemMainSlot.ResetSlot();
            itemMaterialSlot1.ResetSlot();
            itemMaterialSlot2.ResetSlot();
            mergeBtn.gameObject.SetActive(false);
            itemData = null;

            ShowItems();
            SelectAll();
            gradeImg.enabled = false;
            iconImg.enabled = false;
            levelText.text = "";
        }

        private void MergeItem()
        {
            EquipmentData xData = new EquipmentData(itemData);
            itemData.grade++;
            itemData.level = 1;

            int level = xData.level - 1;
            int totalCoin = (xData.grade * 50) * level;
            totalCoin += (level * (100 + level * 100)) / 2;

            Inventory.Instance.ModifyItem(itemData);

            xData = itemMaterialSlot1.equipmentData;
            level = xData.level - 1;
            totalCoin += (xData.grade * 50) * level;
            totalCoin += (level * (100 + level * 100)) / 2;

            xData = itemMaterialSlot2.equipmentData;
            level = xData.level - 1;
            totalCoin += (xData.grade * 50) * level;
            totalCoin += (level * (100 + level * 100)) / 2;


            Inventory.Instance.DeleteItem(itemMaterialSlot1.equipmentData);
            Inventory.Instance.DeleteItem(itemMaterialSlot2.equipmentData);

            DataManager.Instance.AddCoin(totalCoin, 0, "revertmerge");

            ResetMerge();
            equipmentScreen.Refresh();
            itemDisplay.gameObject.SetActive(true);
            GameManager.Instance.UpdateTask(QuestType.MERGE_EQUIPMENT, 1);
            AudioManager.Instance.PlayOneShot("eff_happy", 1f);
        }

        private void SortFindItems()
        {
            for (int i = 0; i < listSlot.Count; i++)
            {
                if (!listSlot[i].gameObject.activeSelf) continue;
                EquipmentData item = listSlot[i].equipmentData;
                if (item == null || item.grade != itemData.grade || item.idInventory == itemData.idInventory || item.itemID != itemData.itemID || !ExistEquipment(item))
                {
                    listSlot[i].gameObject.SetActive(false);
                    continue;
                }
                listSlot[i].gameObject.SetActive(true);
            }
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
            if (itemData != null)
            {
                SortFindItems();
            }
        }

        public bool ExistEquipment(EquipmentData equipmentData)
        {
            if (!itemMainSlot.IsEmpty() && equipmentData.idInventory == itemMainSlot.equipmentData.idInventory)
            {
                return false;
            }
            if (!itemMaterialSlot1.IsEmpty() && equipmentData.idInventory == itemMaterialSlot1.equipmentData.idInventory)
            {
                return false;
            }
            if (!itemMaterialSlot2.IsEmpty() && equipmentData.idInventory == itemMaterialSlot2.equipmentData.idInventory)
            {
                return false;
            }
            return true;
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
            if (itemData != null)
            {
                SortFindItems();
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
}
