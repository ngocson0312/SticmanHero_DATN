using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class Chest : MonoBehaviour
    {
        [SerializeField] Transform lid;
        [SerializeField] ParticleSystem glow;
        private Rigidbody2D rb2d;
        private RewardInfo[] rewardInfos;
        private bool isOpen;
        public void Initialize(RewardInfo[] rewards)
        {
            this.rewardInfos = rewards;
            lid.localRotation = Quaternion.Euler(90, 0, 0);
            isOpen = false;
            rb2d = GetComponent<Rigidbody2D>();
        }
        private void Open()
        {
            lid.DOLocalRotate(new Vector3(90, 0, -45), 1f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                RewardPopup rewardPopup = UIManager.Instance.ShowPopup<RewardPopup>(Hide);
                rewardPopup.ShowReward(rewardInfos);
            });
            glow.Play();
            isOpen = true;
        }
        private void Hide()
        {
            glow.Stop();
        }
        public void SetActive(bool status)
        {
            gameObject.SetActive(status);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isOpen) return;
            if (!other.GetComponent<PlayerManager>()) return;
            Open();
        }
    }
}
