using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace SuperFight
{
    public class ChestMissionButton : MonoBehaviour
    {
        private bool claimed;
        private bool complete;
        [SerializeField] Text pointText;
        [SerializeField] Image statusImg;
        [SerializeField] Sprite[] statusSprs;
        public PreviewReward previewReward;
        public event Action<ChestMissionButton> OnPreview;
        public event Action<ChestMissionButton> OnClaim;
        public void Initialize()
        {
            GetComponent<Button>().onClick.AddListener(OnClickButton);
        }
        public void Refresh(bool claimed, int pointRequire, int currentPoint, List<ItemRewardMission> itemRewards)
        {
            this.claimed = claimed;
            this.complete = currentPoint >= pointRequire;
            pointText.text = pointRequire.ToString();
            statusImg.sprite = statusSprs[claimed ? 1 : 0];
            previewReward.itemRewardMissions = itemRewards;
            previewReward.SetInfo();
            GetComponent<Animator>().SetBool("IsComplete", complete);
        }
        private void OnClickButton()
        {
            if (claimed) return;
            if (complete)
            {
                GetComponent<Animator>().SetBool("IsComplete", false);
                previewReward.Claim();
                statusImg.sprite = statusSprs[1];
                OnClaim?.Invoke(this);
            }
            else
            {
                Preview();
            }
        }
        private void Preview()
        {
            OnPreview?.Invoke(this);
        }

        public void SetActivepreview(bool status)
        {
            previewReward.gameObject.SetActive(status);
        }
    }
}
