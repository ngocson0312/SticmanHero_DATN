using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace SuperFight
{
    public class EquipmentInfoUI : MonoBehaviour
    {
        private EquipmentScreen equipmentScreen;
        private bool isEquipped;
        private EquipmentData itemData;
        [SerializeField] private InventoryStone inventoryStone;
        [SerializeField] Character character;
        [Header("Item")]
        [SerializeField] Image itemRankImg;
        [SerializeField] Image itemIconImg;
        [SerializeField] Text itemNameText;
        [SerializeField] Text itemLevelText;
        [SerializeField] Text priceText;
        [SerializeField] Text descText;
        [SerializeField] Text statBonusText;
        [SerializeField] Image statIconImg;
        [SerializeField] Sprite[] statIcons;
        [SerializeField] GradeSkillUI[] gradeSkills;
        [SerializeField] ItemUtilitiesSO itemUtilities;
        [Header("Button")]
        [SerializeField] GameObject buttonHolder;
        [SerializeField] Button closeBtn;
        [SerializeField] Button equipBtn;
        [SerializeField] Button unEquipBtn;
        [SerializeField] Button levelUpBtn;
        [SerializeField] Button levelAllBtn;
        [SerializeField] Button revertBtn;
        [Header("Stone")]
        [SerializeField] Button stoneBtn;
        [SerializeField] GameObject lockIcon;
        [SerializeField] Image iconStone;
        [SerializeField] Sprite[] stoneIcons;
        public void Initialize(EquipmentScreen equipmentScreen)
        {
            this.equipmentScreen = equipmentScreen;
            closeBtn.onClick.AddListener(Hide);
            equipBtn.onClick.AddListener(Equip);
            unEquipBtn.onClick.AddListener(UnEquip);
            levelUpBtn.onClick.AddListener(Upgrade);
            levelAllBtn.onClick.AddListener(LevelAll);
            stoneBtn.onClick.AddListener(OnStone);
            revertBtn.onClick.AddListener(Revert);
            inventoryStone.Initialize();
        }

        private void Revert()
        {
            int level = itemData.level - 1;
            int totalCoin = (itemData.grade * 50) * level;
            totalCoin += (level * (100 + level * 100)) / 2;
            itemData.level = 1;
            Inventory.Instance.ModifyItem(itemData);
            DataManager.Instance.AddCoin(totalCoin, 0, "revert");
            SetInfo(itemData, this.isEquipped);
            equipmentScreen.Refresh();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Equip()
        {
            equipmentScreen.equipmentPanelUI.OnEquip(itemData);
            gameObject.SetActive(false);
            equipmentScreen.Refresh();
            AudioManager.Instance.PlayOneShot("EquipItem", 1f);
        }

        public void UnEquip()
        {
            equipmentScreen.equipmentPanelUI.UnEquip(itemData);
            gameObject.SetActive(false);
            equipmentScreen.Refresh();
            AudioManager.Instance.PlayOneShot("EquipItem", 1f);
        }
        public void Upgrade()
        {
            int price = GetPrice();
            if (itemData.level >= itemData.grade * 10) return;
            if (price > DataManager.Coin) return;
            itemData.level++;
            Inventory.Instance.ModifyItem(itemData);
            DataManager.Instance.AddCoin(-price, 0, "upgrade");
            SetInfo(itemData, this.isEquipped);
            UpdateTask(price, itemData);
            transform.DOScale(Vector3.one * 1.05f, 0.2f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart);
            });
            AudioManager.Instance.PlayOneShot("eff_upgrade", 1f);
        }

        public void LevelAll()
        {
            int currentCoin = DataManager.Coin;
            int level = itemData.level;
            while (currentCoin > 0)
            {
                int price = itemData.grade * 50 + level * 100;
                if (currentCoin < price || level >= itemData.grade * 10)
                {
                    break;
                }
                level++;
                currentCoin -= price;
                UpdateTask(price, itemData);
            }
            if (itemData.level == level) return;
            itemData.level = level;
            Inventory.Instance.ModifyItem(itemData);
            DataManager.Instance.AddCoin(-(DataManager.Coin - currentCoin), 0, "upgrade");
            SetInfo(itemData, this.isEquipped);
            transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuart);
            });
            AudioManager.Instance.PlayOneShot("eff_upgrade", 1f);
        }
        private int GetPrice()
        {
            return itemData.grade * 50 + itemData.level * 100;
        }
        public void OnStone()
        {
            if (itemData.grade < 5) return;
            inventoryStone.Show(itemData, () =>
            {
                SetInfo(itemData, this.isEquipped);
            });
        }
        public void SetInfo(EquipmentData itemData, bool isEquipped, bool showButtons = true)
        {
            if (Tutorial.TutorialStep == 5)
            {
                Tutorial.TutorialStep = 6;
                Tutorial.Instance.TutorialClick(equipBtn, 0.5f, null);
            }
            this.isEquipped = isEquipped;
            gameObject.SetActive(true);
            buttonHolder.SetActive(showButtons);
            equipBtn.gameObject.SetActive(!isEquipped);
            unEquipBtn.gameObject.SetActive(isEquipped);
            this.itemData = itemData;
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            equipmentObject.prefab.SetEquipmentData(itemData);
            itemRankImg.sprite = itemUtilities.GetGradeBackGround(itemData.grade);
            itemIconImg.sprite = equipmentObject.icon;
            itemNameText.text = equipmentObject.itemName;
            descText.text = equipmentObject.description;
            int price = GetPrice();
            itemLevelText.text = $"Level : {itemData.level}/{itemData.grade * 10}";
            priceText.color = price <= DataManager.Coin ? Color.white : Color.red;
            priceText.text = DataManager.Coin + "/" + price;
            stoneBtn.gameObject.SetActive(false);
            inventoryStone.Hide();
            if (equipmentObject.equipmentType == EquipmentType.WEAPON)
            {
                stoneBtn.gameObject.SetActive(true);
                statBonusText.text = equipmentObject.prefab.equipmentStats.damage.ToString();
                statIconImg.sprite = statIcons[0];
                if (itemData.grade < 5)
                {
                    lockIcon.gameObject.SetActive(true);
                }
                else
                {
                    iconStone.sprite = stoneIcons[itemData.embedStone];
                    lockIcon.gameObject.SetActive(false);
                }
            }
            if (equipmentObject.equipmentType == EquipmentType.ARMOR || equipmentObject.equipmentType == EquipmentType.BOOTS)
            {
                statBonusText.text = equipmentObject.prefab.equipmentStats.armor.ToString();
                statIconImg.sprite = statIcons[1];
            }
            if (equipmentObject.equipmentType == EquipmentType.RING || equipmentObject.equipmentType == EquipmentType.NECKLACE)
            {
                statBonusText.text = equipmentObject.prefab.equipmentStats.health.ToString();
                statIconImg.sprite = statIcons[2];
            }
            string[] desc = equipmentObject.gradeSkillDesc;
            for (int i = 0; i < gradeSkills.Length; i++)
            {
                if (i < gradeSkills.Length - desc.Length)
                {
                    gradeSkills[i].SetActive(false);
                }
                else
                {
                    gradeSkills[i].SetActive(true);
                    int index = i - (gradeSkills.Length - desc.Length);
                    gradeSkills[i].SetInfo(desc[index], itemData.grade >= i + 1);
                }
            }
        }

        public void UpdateTask(int price, EquipmentData itemData)
        {
            GameManager.Instance.UpdateTask(QuestType.UPGRADE_EQUIPMENT, 1);
            EquipmentObjectSO equipmentObjectSO = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            switch (equipmentObjectSO.equipmentType)
            {
                case EquipmentType.WEAPON:
                    GameManager.Instance.UpdateTask(QuestType.UPGRADE_WEAPON, 1);
                    break;
                case EquipmentType.ARMOR:
                    GameManager.Instance.UpdateTask(QuestType.UPGRADE_ARMOR, 1);
                    break;
                case EquipmentType.NECKLACE:
                    GameManager.Instance.UpdateTask(QuestType.UPGRADE_NECKLACE, 1);
                    break;
                case EquipmentType.RING:
                    GameManager.Instance.UpdateTask(QuestType.UPGRADE_RING, 1);
                    break;
                case EquipmentType.BOOTS:
                    GameManager.Instance.UpdateTask(QuestType.UPGRADE_BOOTS, 1);
                    break;
            }
        }
    }
}
