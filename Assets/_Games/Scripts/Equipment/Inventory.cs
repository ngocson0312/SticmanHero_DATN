using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace SuperFight
{
    public class Inventory : Singleton<Inventory>
    {
        private DataManager dataManager;
        private UserData userData;
        public static event Action OnEquipmentChange;
        public static event Action OnAddItem;
        public static event Action<EquipmentData> OnModifyEquipment;
        public static event Action<EquipmentData> OnDeleteItem;
        public void LoadData(DataManager dataManager, UserData userData)
        {
            this.dataManager = dataManager;
            this.userData = userData;
        }
        public void ModifyItem(EquipmentData itemInfo)
        {
            for (int i = 0; i < userData.inventoryData.itemsOwned.Count; i++)
            {
                if (userData.inventoryData.itemsOwned[i].idInventory == itemInfo.idInventory)
                {
                    userData.inventoryData.itemsOwned[i] = itemInfo;
                }
            }
            OnModifyEquipment?.Invoke(itemInfo);
            dataManager.SaveData();
        }
        public void AddItem(EquipmentData itemInfo)
        {
            int index = userData.inventoryData.inventory.Count;
            int idDefine = userData.inventoryData.idItemDefine;
            itemInfo.idInventory = idDefine;
            int indexFound = -1;
            for (int i = 0; i < userData.inventoryData.inventory.Count; i++)
            {
                if (userData.inventoryData.inventory[i].itemInventoryID < 0)
                {
                    indexFound = i;
                    break;
                }
            }
            if (indexFound >= 0)
            {
                userData.inventoryData.inventory[indexFound].itemInventoryID = itemInfo.idInventory;
            }
            else
            {
                InventorySlot newSlot = new InventorySlot(index, idDefine);
                userData.inventoryData.inventory.Add(newSlot);
            }
            userData.inventoryData.itemsOwned.Add(itemInfo);
            userData.inventoryData.idItemDefine++;
            OnAddItem?.Invoke();
            dataManager.SaveData();
        }
        public void OnEquipItem(InventorySlot equipmentSlot, InventorySlot oldSlot)
        {
            if (userData.inventoryData.equipment.Count > 0)
            {
                int indexFound = -1;
                for (int i = 0; i < userData.inventoryData.equipment.Count; i++)
                {
                    if (userData.inventoryData.equipment[i].index == equipmentSlot.index)
                    {
                        indexFound = i;
                        break;
                    }
                }
                if (indexFound != -1)
                {
                    userData.inventoryData.equipment[indexFound].itemInventoryID = equipmentSlot.itemInventoryID;
                }
                else
                {
                    userData.inventoryData.equipment.Add(equipmentSlot);
                }
            }
            else
            {
                userData.inventoryData.equipment.Add(equipmentSlot);
            }

            for (int i = 0; i < userData.inventoryData.inventory.Count; i++)
            {
                if (userData.inventoryData.inventory[i].itemInventoryID == equipmentSlot.itemInventoryID)
                {
                    userData.inventoryData.inventory[i].itemInventoryID = oldSlot.itemInventoryID;
                    break;
                }
            }
            OnEquipmentChange?.Invoke();
            dataManager.SaveData();
        }
        public void OnUnEquipItem(EquipmentData equipmentData)
        {
            int indexFound = -1;
            for (int i = 0; i < userData.inventoryData.inventory.Count; i++)
            {
                if (userData.inventoryData.inventory[i].itemInventoryID < 0)
                {
                    indexFound = i;
                    break;
                }
            }
            if (indexFound != -1)
            {
                userData.inventoryData.inventory[indexFound].itemInventoryID = equipmentData.idInventory;
            }
            else
            {
                int index = userData.inventoryData.inventory.Count;
                userData.inventoryData.inventory.Add(new InventorySlot(index, equipmentData.idInventory));
            }

            indexFound = -1;

            for (int i = 0; i < userData.inventoryData.equipment.Count; i++)
            {
                if (userData.inventoryData.equipment[i].itemInventoryID == equipmentData.idInventory)
                {
                    indexFound = i;
                    break;
                }
            }
            if (indexFound != -1)
            {
                userData.inventoryData.equipment[indexFound].itemInventoryID = -1;
            }
            OnEquipmentChange?.Invoke();
            dataManager.SaveData();
        }
        public EquipmentData GetEquipmentDataById(int id)
        {
            return userData.inventoryData.itemsOwned.Find(x => x.idInventory == id);
        }
        public InventorySlot GetInventorySlotById(int id)
        {
            return userData.inventoryData.inventory.Find(x => x.itemInventoryID == id);
        }
        public void DeleteItem(EquipmentData data)
        {
            for (int i = 0; i < userData.inventoryData.itemsOwned.Count; i++)
            {
                if (userData.inventoryData.itemsOwned[i].idInventory == data.idInventory)
                {
                    userData.inventoryData.itemsOwned.RemoveAt(i);
                break;
                }
            }
            for (int i = 0; i < userData.inventoryData.inventory.Count; i++)
            {
                if (userData.inventoryData.inventory[i].itemInventoryID == data.idInventory)
                {
                    userData.inventoryData.inventory[i].itemInventoryID = -1;
                    break;
                }
            }
            for (int i = 0; i < userData.inventoryData.equipment.Count; i++)
            {
                if (userData.inventoryData.equipment[i].itemInventoryID == data.idInventory)
                {
                    userData.inventoryData.equipment[i].itemInventoryID = -1;
                    break;
                }
            }
            OnEquipmentChange?.Invoke();
            OnDeleteItem?.Invoke(data);
            dataManager.SaveData();
        }
    }
    [System.Serializable]
    public class InventoryData
    {
        public InventoryData()
        {
            inventory = new List<InventorySlot>();
            equipment = new List<InventorySlot>();
            itemsOwned = new List<EquipmentData>();
            idItemDefine = 1;
        }
        public int idItemDefine;
        public List<InventorySlot> inventory;
        public List<InventorySlot> equipment;
        public List<EquipmentData> itemsOwned;
    }
    [System.Serializable]
    public class EquipmentData
    {
        public EquipmentData(int itemID, int grade)
        {
            this.itemID = itemID;
            this.idInventory = -1;
            this.level = 1;
            this.grade = grade;
            this.embedStone = 0;
        }
        public EquipmentData(EquipmentData equipmentData)
        {
            this.itemID = equipmentData.itemID;
            this.idInventory = equipmentData.idInventory;
            this.level = equipmentData.level;
            this.grade = equipmentData.grade;
            this.embedStone = equipmentData.embedStone;
        }
        public int idInventory;
        public int itemID;
        public int level;
        public int grade;
        public int embedStone;
    }
    [System.Serializable]
    public class InventorySlot
    {
        public InventorySlot(int index, int itemInventoryID)
        {
            this.index = index;
            this.itemInventoryID = itemInventoryID;
        }
        public InventorySlot(int index)
        {
            this.index = index;
            this.itemInventoryID = -1;
        }
        public int index;
        public int itemInventoryID;// idInventory
    }
    // public enum EquipmentSlotType
    // {
    //     PRIMARY, SECONDARY, ARMOR, BOOTS, RING, NECKLACE, SKILL
    // }
}
