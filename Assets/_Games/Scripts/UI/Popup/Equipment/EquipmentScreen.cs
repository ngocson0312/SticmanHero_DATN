using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SuperFight
{
    public class EquipmentScreen : PopupUI
    {
        public CharacterStatsUI characterStatsUI;
        public InventoryPanelUI inventoryPanelUI;
        public EquipmentPanelUI equipmentPanelUI;
        public EquipmentInfoUI equipmentInfoUI;
        public MergeEquipmentPanelUI mergeEquipmentPanelUI;
        [SerializeField] Button mergeBtn;
        [SerializeField] Button backBtn;
        [SerializeField] Button shopBtn;
        [SerializeField] Button[] tabButtons;
        [SerializeField] Sprite[] tabButtonStatus;
        [SerializeField] Transform tabPanelHolder;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            inventoryPanelUI.Initialize(this);
            equipmentPanelUI.Initialize(this);
            mergeEquipmentPanelUI.Initialize(this);
            equipmentInfoUI.Initialize(this);
            mergeBtn.onClick.AddListener(OpenMerge);
            backBtn.onClick.AddListener(Hide);
            shopBtn.onClick.AddListener(GoShop);
            for (int i = 0; i < tabButtons.Length; i++)
            {
                Button btn = tabButtons[i];
                btn.onClick.AddListener(() => EnablePanel(btn.transform.GetSiblingIndex()));
            }
        }

        private void GoShop()
        {
            uiManager.ShowPopup<ShopScreenUI>(() =>
            {
                EnablePanel(0);
            });
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            equipmentInfoUI.gameObject.SetActive(false);
            EnablePanel(0);
        }
        public void EnablePanel(int index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].image.sprite = tabButtonStatus[0];
            }
            tabButtons[index].image.sprite = tabButtonStatus[1];
            for (int i = 0; i < tabPanelHolder.childCount; i++)
            {
                tabPanelHolder.GetChild(i).gameObject.SetActive(false);
            }
            switch (index)
            {
                case 0:
                    inventoryPanelUI.Show();
                    break;
                case 1:
                    characterStatsUI.Show();
                    break;
            }
        }
        public void OpenMerge()
        {
            mergeEquipmentPanelUI.ShowUpgrade();
        }
        public void Refresh()
        {
            inventoryPanelUI.Refresh();
            characterStatsUI.Refresh();
        }

    }
}
