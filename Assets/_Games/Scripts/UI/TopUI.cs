using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace SuperFight
{
    public class TopUI : MonoBehaviour
    {
        public Button addCoinButton;
        public Text coinText;
        public ParticleSystem coinFx;
        public CoinFx coinFxFly;

        public Button addDiamondButton;
        public Text diamondText;
        public ParticleSystem diamondFx;
        public CoinFx diamondFxFly;
        public void Initialize()
        {
            DataManager.OnAddCoin += SetCoinText;
            DataManager.OnAddDiamond += SetDiamondText;
            coinText.text = ConvertMoneyToString(DataManager.Coin);
            diamondText.text = ConvertMoneyToString(DataManager.Diamond);
            addCoinButton.onClick.AddListener(OpenShop);
            addDiamondButton.onClick.AddListener(OpenShop);

        }
        public void SetActive(bool status)
        {
            gameObject.SetActive(status);
        }
        private void SetDiamondText(int startValue, int endValue, float delayTime)
        {
            int num = startValue;
            DOTween.To(() => num, x => num = x, endValue, 1).SetDelay(0.2f).OnUpdate(() =>
            {
                diamondText.text = ConvertMoneyToString(num);
            });
            if (endValue > startValue)
            {
                diamondFxFly.PlayFx(() => diamondFx.Play(), 1);
            }
        }

        public void SetCoinText(int startValue, int endValue, float delayTime, bool showAnim)
        {
            int num = startValue;
            DOTween.To(() => num, x => num = x, endValue, 1).SetDelay(0.2f).OnUpdate(() =>
            {
                coinText.text = ConvertMoneyToString(num);
            });
            if (endValue > startValue && showAnim)
            {
                coinFxFly.PlayFx(() => coinFx.Play(), 0);
            }
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                DataManager.Instance.AddCoin(10000, 0, "test", true);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                DataManager.Instance.AddDiamond(2000, 0, "test");
            }

        }
#endif
        public void OpenShop()
        {
            UIManager.Instance.ShowPopup<ShopScreenUI>(null);
        }

        #region ConvertMoney
        public static string ConvertMoneyToString(long money)
        {
            if (money >= 1000000000)
            {
                return (money / 1000000000).ToString() + "B";
            }
            if (money >= 1000000)
            {
                return (money / 1000000).ToString() + "M";
            }
            if (money >= 100000)
            {
                return (money / 1000).ToString() + "K";
            }
            return money.ToString();
        }
        #endregion
    }
}
