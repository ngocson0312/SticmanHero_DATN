using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class InventoryStone : MonoBehaviour
    {
        public IndexStone[] stoneButtons;
        public Button close;
        private EquipmentData itemData;
        private Action complete;
        public void Initialize()
        {
            for (int i = 0; i < stoneButtons.Length; i++)
            {
                stoneButtons[i].Initialize(this, i + 1);
            }
            close.onClick.AddListener(Hide);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Show(EquipmentData equipmentData, Action complete)
        {
            this.itemData = equipmentData;
            gameObject.SetActive(true);
            this.complete = complete;
            for (int i = 0; i < stoneButtons.Length; i++)
            {
                if (stoneButtons[i].index == equipmentData.embedStone)
                {
                    stoneButtons[i].SetActive(false);
                }
                else
                {
                    stoneButtons[i].SetActive(true);
                }
            }
        }
        public void EmbedStone(int index)
        {
            itemData.embedStone = index;
            Inventory.Instance.ModifyItem(itemData);
            complete?.Invoke();
            complete = null;
            Hide();
        }
    }
}
