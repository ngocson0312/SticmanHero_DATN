using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace SuperFight
{
    public class RewardSpin : MonoBehaviour
    {
        public Button claim;
        public Image bgItem;
        public Image iconItem;
        public Text description;
        public GameObject item;

        public void Initialize()
        {
            claim.onClick.AddListener(Hide);
        }
        public void Show()
        {
            gameObject.SetActive(true);
            claim.gameObject.SetActive(false);
            item.transform.localScale = Vector3.zero;
            item.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
            {
                claim.gameObject.SetActive(true);
                transform.DOKill();
            });

        }

        public void SetInfo(Sprite bgItem, Sprite iconItem, string description)
        {
            this.bgItem.sprite = bgItem;
            this.iconItem.sprite = iconItem;
            this.description.text = description;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
