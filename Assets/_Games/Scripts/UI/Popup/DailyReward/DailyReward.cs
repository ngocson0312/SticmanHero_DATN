using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;
using System;

namespace SuperFight
{
    public class DailyReward : PopupUI
    {
        public Button close;
        public DailyDay[] gifts;
        public int currentGift
        {
            get { return PlayerPrefs.GetInt("daily_current_day", 0); }
            set { PlayerPrefs.SetInt("daily_current_day", value); }
        }
        public int canGetGift
        {
            get { return PlayerPrefs.GetInt("can_get_daily", 0); }
            set { PlayerPrefs.SetInt("can_get_daily", value); }
        }
        public override void Initialize(UIManager uIManager)
        {
            base.Initialize(uIManager);
            for (int i = 0; i < gifts.Length; i++)
            {
                gifts[i].day = i + 1;
                gifts[i].Initialize(this);
            }
            close.onClick.AddListener(Hide);
        }
        public void OnClaim(DailyRewardItem item)
        {
            if (canGetGift <= 0) return;
            if (item.coin > 0)
            {
                DataManager.Instance.AddCoin(item.coin, 0.5f, "Daily");
            }
            else if (item.diamond > 0)
            {
                DataManager.Instance.AddDiamond(item.diamond, 0.5f, "Daily");
            }
            else if (item.equipmentData != null)
            {
                Inventory.Instance.AddItem(item.equipmentData);
            }
            canGetGift = 0;
            Hide();
        }
        public void ResetDay()
        {
            GameManager.resetDay += SetupDay;
        }
        void SetupDay()
        {
            if (canGetGift == 0)
            {
                currentGift++;
                canGetGift = 1;
            }
        }
    }
    [System.Serializable]
    public class DailyRewardItem
    {
        public int coin;
        public int diamond;
        public EquipmentData equipmentData;
    }
}

