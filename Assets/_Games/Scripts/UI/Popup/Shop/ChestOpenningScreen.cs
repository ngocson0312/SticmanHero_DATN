using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using DG.Tweening;

namespace SuperFight
{
    public class ChestOpenningScreen : MonoBehaviour
    {
        [SerializeField] ItemUtilitiesSO utilities;

        List<ItemReward> tempObj = new List<ItemReward>();
        private bool isActive;
        public ItemReward itemRewardPrefab;
        public Transform holder;
        public GridLayoutGroup gridLayout;
        public SkeletonGraphic chestAnimation;
        private List<EquipmentData> equipmentDatas;
        public RectTransform chestHolder;
        private ChestPack chestPack;
        public Text priceTxt;
        [Header("Button")]
        public Button openX10;
        public Button claimBtn;
        public Button keyBtn;
        [Header("Key")]
        public Text keyTxt;
        public Image iconKey;
        public List<Sprite> iconKeys;

        [Header("ParticleSystem")]
        public FxOpenItem starFx;
        public ParticleSystem fireFX;
        //private InappData total;
        public void Initialize()
        {
            claimBtn.onClick.AddListener(OnClickClaim);
            openX10.onClick.AddListener(OnOpenX10);
            keyBtn.onClick.AddListener(OnOpenKey);
            isActive = false;
            // starFx.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void ShowReward(List<EquipmentData> equipmentDatas, ChestPack chestPack)
        {
            gameObject.SetActive(true);
            this.chestPack = chestPack;
            this.equipmentDatas = equipmentDatas;
            for (int i = 0; i < gridLayout.transform.childCount; i++)
            {
                Destroy(gridLayout.transform.GetChild(i).gameObject);
            }
            tempObj = new List<ItemReward>();
            if (this.equipmentDatas.Count <= 1)
            {
                gridLayout.childAlignment = TextAnchor.UpperCenter;
            }
            else
            {
                gridLayout.childAlignment = TextAnchor.UpperLeft;
            }
            for (int i = 0; i < equipmentDatas.Count; i++)
            {
                itemRewardPrefab.SetInfos(this.equipmentDatas[i], GetGradeBackGround(this.equipmentDatas[i].grade));
                ItemReward reward = Instantiate(itemRewardPrefab, holder);
                tempObj.Add(reward);
            }
            SetActiveBtn(false);
            StartCoroutine(ChestAnim());
        }

        public void OnClickClaim()
        {
            if (tempObj.Count > 0 || tempObj != null)
            {
                for (int i = 0; i < tempObj.Count; i++)
                {
                    Destroy(tempObj[i].gameObject);
                }
            }
            isActive = false;
            gameObject.SetActive(false);
        }

        IEnumerator OpenItemReward()
        {
            int i = 0;
            while (i < tempObj.Count)
            {
                AudioManager.Instance.PlayOneShot("receive", 1);
                chestAnimation.AnimationState.SetAnimation(0, "Open2", false);
                chestAnimation.AnimationState.TimeScale = 1.5f;
                starFx.Fly(tempObj[i].GetComponent<RectTransform>().position);
                yield return new WaitForSeconds(0.1f);
                tempObj[i].SetSize(0.16f);
                yield return new WaitForSeconds(0.2f);
                i++;
            }
            int price = (int)(chestPack.price * 10 * 0.96f + 1);
            priceTxt.text = $"{(price)}";
            isActive = false;
            chestAnimation.timeScale = 1f;
            chestAnimation.AnimationState.SetAnimation(0, "Open2", false);
            AudioManager.Instance.PlayOneShot("firework", 1);
            fireFX.Play();
            AudioManager.Instance.PlayOneShot("eff_happy", 1);
            yield return new WaitForSeconds(1f);
            SetActiveBtn(true);
        }

        IEnumerator ChestAnim()
        {
            AudioManager.Instance.PlayOneShot("idleChest", 1);
            chestHolder.localPosition = new Vector2(0, -100);
            chestAnimation.AnimationState.SetAnimation(0, "Idle", true);
            chestAnimation.timeScale = 2f;
            yield return new WaitForSeconds(1.2f);
            chestHolder.DOAnchorPos(new Vector2(0, -500), 0.2f);
            chestAnimation.timeScale = 1.8f;
            chestAnimation.AnimationState.SetAnimation(0, "Open", false);
            yield return new WaitForSeconds(0.5f);
            if (!isActive)
            {
                isActive = true;
                StartCoroutine(OpenItemReward());
            }

        }
        public void OnOpenX10()
        {
            if (DataManager.Diamond < (int)(chestPack.price * 10 * 0.96f) + 1)
            {
                UIManager.Instance.ShowPopup<NotificePopup>(null).NotEnoughDiamond(this.gameObject);
            }
            else
            {
                chestPack.OpenChest(10);
            }

        }

        public void OnOpenKey()
        {
            chestPack.OpenKey();
        }
        public Sprite GetGradeBackGround(int grade)
        {
            return utilities.GetGradeBackGround(grade);
        }

        public void SetActiveBtn(bool status)
        {
            claimBtn.gameObject.SetActive(status);
            openX10.gameObject.SetActive(status);
            int keys = DataManager.Instance.GetKey(chestPack.type);
            keyBtn.gameObject.SetActive(keys > 0 ? status : false);

            keyTxt.text = (keys >= 10) ? $"{keys}/10" : $"{keys}/1";
            iconKey.sprite = iconKeys[chestPack.type];
        }

    }
}
