using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class CharacterStatsUI : MonoBehaviour
    {
        public Text healthText;
        public Text atkText;
        public Text defText;
        public Text critText;
        public Text critDmgText;
        public Text priceText;
        public Button upgradeBtn;
        private void Start()
        {
            upgradeBtn.onClick.AddListener(Upgrade);
        }
        private void Upgrade()
        {
            int price = Price();
            if (DataManager.Coin < price) return;
            DataManager.Instance.UpgradeLevel();
            DataManager.Instance.AddCoin(-price, 0, "upgrade");
            Refresh();
        }
        private int Price()
        {
            return DataManager.PlayerLevel * 100;
        }
        public void Show()
        {
            gameObject.SetActive(true);
            Refresh();
        }
        public void Refresh()
        {
            int price = Price();
            priceText.text = price.ToString();
            priceText.color = DataManager.Coin < price ? Color.red : Color.white;
            CharacterStats characterStats = PlayerManager.Instance.playerController.originalStats;
            healthText.text = characterStats.health.ToString();
            atkText.text = characterStats.damage.ToString();
            defText.text = characterStats.armor.ToString();
            critText.text = characterStats.critRate.ToString();
            critDmgText.text = characterStats.critDamage.ToString();

            
        }
    }
}
