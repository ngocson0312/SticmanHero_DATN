using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class NotificePopup : PopupUI
    {

        [SerializeField] private Button yesBtn;
        [SerializeField] private Button noBtn;
        [SerializeField] private Text notiTxt;
        [SerializeField] private Image iconNoti;
        [SerializeField] private GameObject obj;
        [SerializeField] private List<Sprite> icons;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            noBtn.onClick.AddListener(OnNo);
        }

        public void OnYesShop()
        {
            UIManager.Instance.ShowPopup<ShopScreenUI>(null).scrollRect.horizontalNormalizedPosition = 0.837f;
            gameObject.SetActive(false);
            obj.SetActive(false);
        }
        public void OnYesMerge()
        {
            UIManager.Instance.ShowPopup<EquipmentScreen>(null).OpenMerge();
            gameObject.SetActive(false);
        }

        public void OnNo()
        {
            gameObject.SetActive(false);
        }

        public void NotEnoughCoin(GameObject obj)
        {
            yesBtn.onClick.RemoveAllListeners();
            yesBtn.onClick.AddListener(OnYesShop);
            notiTxt.text = "Not enough coins" + "\n" + "Do you want to go to the shop?";
            this.obj = obj;
            iconNoti.sprite = icons[0];
            Show(null);
        }

        public void NotEnoughDiamond(GameObject obj)
        {
            yesBtn.onClick.RemoveAllListeners();
            yesBtn.onClick.AddListener(OnYesShop);
            notiTxt.text = "Not enough diamonds" + "\n" + "Do you want to go to the shop?";
            this.obj = obj;
            iconNoti.sprite = icons[0];
            Show(null);
        }

        public void NotGrade()
        {
            yesBtn.onClick.RemoveAllListeners();
            yesBtn.onClick.AddListener(OnYesMerge);
            notiTxt.text = "Max level" + "\n" + "Do you want to go to merge?";
            iconNoti.sprite = icons[1];
            Show(null);
        }
    }
}
