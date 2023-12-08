using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using mygame.sdk;
namespace SuperFight
{
    public class RevivePanel : MonoBehaviour
    {
        [SerializeField] Button reviveButton;
        [SerializeField] Button skipButton;
        [SerializeField] Image fillTimeImg;
        [SerializeField] GameObject holder;
        [SerializeField] GameObject adIcon;
        private bool watchADs;
        private RectTransform rectTransform;
        private int firstRevive
        {
            get { return PlayerPrefs.GetInt("first_revive", 0); }
            set { PlayerPrefs.SetInt("first_revive", value); }
        }
        public void Initialize()
        {
            reviveButton.onClick.AddListener(Revive);
            skipButton.onClick.AddListener(Skip);
            rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }
        public void Active()
        {
            gameObject.SetActive(true);
            holder.SetActive(false);
            watchADs = false;
            skipButton.transform.localScale = Vector3.zero;
            if (firstRevive == 0)
            {
                adIcon.SetActive(false);
            }
            else
            {
                adIcon.SetActive(true);
                skipButton.transform.DOScale(Vector3.one, 1f).SetDelay(2f);
            }
            StartCoroutine(IECountDown());
            fillTimeImg.fillAmount = 1f;
            IEnumerator IECountDown()
            {
                float timer = 1f;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
                holder.SetActive(true);
                timer = 5f;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    fillTimeImg.fillAmount = timer / 5f;
                    if (watchADs) yield break;
                    yield return null;
                }
                if (!watchADs)
                {
                    Skip();
                }
            }
        }
        private void Skip()
        {
            gameObject.SetActive(false);
            GameManager.Instance.OnLose();
        }

        private void Revive()
        {
            if (firstRevive == 0)
            {
                firstRevive = 1;
                gameObject.SetActive(false);
                GameManager.Instance.Revive();
                GameManager.Instance.UpdateTask(QuestType.REVIVE_INGAME, 1);
                return;
            }
            AdsHelper.Instance.showGift(DataManager.Level, "revive", (x) =>
            {
                if (x == AD_State.AD_SHOW)
                {
                    watchADs = true;
                }
                if (x == AD_State.AD_REWARD_OK)
                {
                    gameObject.SetActive(false);
                    GameManager.Instance.Revive();
                    GameManager.Instance.UpdateTask(QuestType.REVIVE_INGAME, 1);
                }
            });
        }
    }
}
