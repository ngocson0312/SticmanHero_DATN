using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class CharacterEquipment : MonoBehaviour
    {
        [Header("Weapon")]
        public Weapon primaryWeapon;
        public Weapon secondaryWeapon;
        [Header("Items")]
        public Armor currentArmor;
        public Boots currentBoots;
        public Necklace currentNecklace;
        public Ring currentRing;
        private Controller controller;
        private Character character;
        public void Initialize(Controller controller, Character character)
        {
            this.controller = controller;
            this.character = character;
        }
        public void RefreshEquipment()
        {
            for (int i = 0; i < character.cloths.Length; i++)
            {
                character.cloths[i].SetActive(false);
            }
            List<InventorySlot> listEquipment = DataManager.Instance.data.inventoryData.equipment;
            bool noWeapon = true;
            for (int i = 0; i < listEquipment.Count; i++)
            {
                EquipmentData equipmentData = null;
                EquipmentObjectSO equipmentObject = null;
                if (listEquipment[i].itemInventoryID > 0)
                {
                    equipmentData = Inventory.Instance.GetEquipmentDataById(listEquipment[i].itemInventoryID);
                    equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(equipmentData.itemID);
                    EnableCloth(equipmentObject.itemName);
                }
                if (listEquipment[i].index == 0)
                {
                    if (equipmentObject)
                    {
                        EquipWeapon(equipmentObject.prefab as Weapon, equipmentData, true);
                        noWeapon = false;
                    }
                    else
                    {
                        EquipWeapon(null, null, true);
                    }
                }
                if (listEquipment[i].index == 1)
                {
                    if (equipmentObject)
                    {
                        EquipWeapon(equipmentObject.prefab as Weapon, equipmentData, false);
                        noWeapon = false;
                    }
                    else
                    {
                        EquipWeapon(null, null, false);
                    }
                }
                if (listEquipment[i].index == 2)
                {
                    if (equipmentObject)
                    {
                        EquipArmor(equipmentObject.prefab as Armor, equipmentData);
                    }
                    else
                    {
                        EquipArmor(null, equipmentData);
                    }
                }
                if (listEquipment[i].index == 3)
                {
                    if (equipmentObject)
                    {
                        EquipBoots(equipmentObject.prefab as Boots, equipmentData);
                    }
                    else
                    {
                        EquipBoots(null, equipmentData);
                    }
                }
                if (listEquipment[i].index == 4)
                {
                    if (equipmentObject)
                    {
                        EquipRing(equipmentObject.prefab as Ring, equipmentData);
                    }
                    else
                    {
                        EquipRing(null, equipmentData);
                    }
                }
                if (listEquipment[i].index == 5)
                {
                    if (equipmentObject)
                    {
                        EquipNecklace(equipmentObject.prefab as Necklace, equipmentData);
                    }
                    else
                    {
                        EquipNecklace(null, equipmentData);
                    }
                }
            }
            if (noWeapon)
            {
                EquipWeapon(DataManager.Instance.equipmentContainer.defaultWeapon.prefab as Weapon, new EquipmentData(0, 1), true);
            }
            if (primaryWeapon == null && secondaryWeapon != null)
            {
                primaryWeapon = secondaryWeapon;
                primaryWeapon.SetActive(true);
                secondaryWeapon = null;
            }
            if (primaryWeapon && secondaryWeapon)
            {
                secondaryWeapon.SetActive(false);
            }

        }
        private void EnableCloth(string clothName)
        {
            for (int i = 0; i < character.cloths.Length; i++)
            {
                if (character.cloths[i].name == clothName)
                {
                    character.cloths[i].SetActive(true);
                }
            }
        }
        public void EquipArmor(Armor armor, EquipmentData data)
        {
            if (currentArmor)
            {
                currentArmor.OnUnEquip();
                Destroy(currentArmor.gameObject);
                currentArmor = null;
            }
            if (armor)
            {
                currentArmor = Instantiate(armor, transform);
                currentArmor.Initialize(controller);
                currentArmor.SetEquipmentData(data);
                currentArmor.OnEquip();
            }
        }
        public void EquipBoots(Boots boots, EquipmentData data)
        {
            if (currentBoots)
            {
                currentBoots.OnUnEquip();
                Destroy(currentBoots.gameObject);
                currentBoots = null;
            }
            if (boots)
            {
                currentBoots = Instantiate(boots, transform);
                currentBoots.Initialize(controller);
                currentBoots.SetEquipmentData(data);
                currentBoots.OnEquip();
            }
        }
        public void EquipNecklace(Necklace necklace, EquipmentData data)
        {
            if (currentNecklace)
            {
                currentNecklace.OnUnEquip();
                Destroy(currentNecklace.gameObject);
                currentNecklace = null;
            }
            if (necklace)
            {
                currentNecklace = Instantiate(necklace, transform);
                currentNecklace.Initialize(controller);
                currentNecklace.SetEquipmentData(data);
                currentNecklace.OnEquip();
            }
        }
        public void EquipRing(Ring ring, EquipmentData data)
        {
            if (currentRing)
            {
                currentRing.OnUnEquip();
                Destroy(currentRing.gameObject);
                currentRing = null;
            }
            if (ring)
            {
                currentRing = Instantiate(ring, transform);
                currentRing.Initialize(controller);
                currentRing.SetEquipmentData(data);
                currentRing.OnEquip();
            }
        }
        public void EquipWeapon(Weapon weapon, EquipmentData equipmentData, bool isPrimaryWeapon)
        {
            if (isPrimaryWeapon)
            {
                if (primaryWeapon)
                {
                    primaryWeapon.OnUnEquip();
                    Destroy(primaryWeapon.gameObject);
                    primaryWeapon = null;
                }
                if (weapon)
                {
                    primaryWeapon = Instantiate(weapon, character.weaponHolderRight);
                    primaryWeapon.Initialize(controller);
                    primaryWeapon.SetEquipmentData(equipmentData);
                    primaryWeapon.OnEquip();
                }
            }
            else
            {
                if (secondaryWeapon)
                {
                    secondaryWeapon.OnUnEquip();
                    Destroy(secondaryWeapon.gameObject);
                    secondaryWeapon = null;
                }
                if (weapon)
                {
                    secondaryWeapon = Instantiate(weapon, character.weaponHolderRight);
                    secondaryWeapon.gameObject.SetActive(false);
                    secondaryWeapon.Initialize(controller);
                    secondaryWeapon.SetEquipmentData(equipmentData);
                    secondaryWeapon.OnEquip();
                }
            }
        }
    }
}
