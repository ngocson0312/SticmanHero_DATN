using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace SuperFight
{
    public class RewardItemUI : MonoBehaviour
    {
        public Image bgImg;
        public Image iconImg;
        public Text amountText;
        public void SetInfo(Sprite rarity, Sprite icon, string amount, int i)
        {
            GameManager.Instance.DelayCallBack(0.15f * i, () =>
            {
                AudioManager.Instance.PlayOneShot("ItemCollect", 1f);
                gameObject.SetActive(true);
                transform.localScale = Vector3.one * 5;
                transform.DOScale(Vector3.one, 0.15f);
            });
            bgImg.sprite = rarity;
            iconImg.sprite = icon;
            amountText.text = amount;
        }
    }
}
