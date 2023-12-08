using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public enum TypeGift
    {
        Equipment,
        Coin,
        Diamond
    }
    public class GiftBox : MonoBehaviour
    {
        public Text txtGift;
        public Image imgGift;
        public EquipmentData itemData;
        public List<Sprite> icons;
        public TypeGift type;
        public int amount;
        public int id;
        public void DisplayUI()
        {
            switch (type)
            {
                case TypeGift.Equipment:
                    SetEquipmentClaim();
                    break;
                case TypeGift.Coin:
                    txtGift.text = "+" + amount;
                    imgGift.sprite = icons[0];
                    break;
                case TypeGift.Diamond:
                    txtGift.text = "+" + amount;
                    imgGift.sprite = icons[1];
                    break;
                default:
                    break;
            }
        }
        public void ClaimGift()
        {
            switch (type)
            {
                case TypeGift.Coin:
                    ClaimCoin();
                    break;
                case TypeGift.Diamond:
                    ClaimDiamond();
                    break;
                case TypeGift.Equipment:
                    ClaimEquipment();
                    break;
                default:
                    break;
            }
            mygame.sdk.FIRhelper.logEvent("claim_reward_lucky_wheel");
        }

        public void SetEquipmentClaim()
        {
            EquipmentObjectSO item = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            imgGift.sprite = item.icon;
            txtGift.text = item.itemName;
        }
        void ClaimCoin()
        {
            DataManager.Instance.AddCoin(amount, 0, "Spin");
        }
        void ClaimDiamond()
        {
            DataManager.Instance.AddDiamond(amount, 0, "Spin");
        }
        void ClaimEquipment()
        {
            Inventory.Instance.AddItem(itemData);
        }
    }
}


