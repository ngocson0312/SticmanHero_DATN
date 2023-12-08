using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;
using System;
using Random = UnityEngine.Random;
namespace SuperFight
{
    public class ChestPack : MonoBehaviour
    {
        public event Action<ChestPack> OnPreview;
        public int adsCountChest
        {
            get { return PlayerPrefs.GetInt("shop_ads_count_openchest" + type, adsCount); }
            set { PlayerPrefs.SetInt("shop_ads_count_openchest" + type, value); }
        }
        public string timeFree
        {
            get { return PlayerPrefs.GetString("open_free_Chest_time" + type, "0"); }
            set { PlayerPrefs.SetString("open_free_Chest_time" + type, value); }
        }
        private int firstOpen
        {
            get { return PlayerPrefs.GetInt("first_chest_" + type, 0); }
            set { PlayerPrefs.SetInt("first_chest_" + type, value); }
        }
        private int openCount
        {
            get { return PlayerPrefs.GetInt("open_count_chest" + type, 0); }
            set { PlayerPrefs.SetInt("open_count_chest" + type, value); }
        }
        private int countChest
        {
            get { return PlayerPrefs.GetInt("count_chest_" + type, 0); }
            set { PlayerPrefs.SetInt("count_chest_" + type, value); }
        }
        [SerializeField] EquipmentObjectSO defaultEquipment;
        [SerializeField] private int adsCount;
        public int type;
        public RarityRate[] rarityRates;
        public int rarityAbsoluteDrop = 1;
        public Button open1;
        public Button open10;
        public Button keyBtn;
        public Button infoRatioBtn;
        public List<GameObject> iconButton;
        public int price;
        [Header("key")]
        public Text keyTxt;
        [SerializeField] private Text open1Txt;
        [SerializeField] private Text AdsCountTxt;
        [SerializeField] private Text open10Txt;
        [Header("InfoRatio")]
        public GameObject InfoRatio;
        [SerializeField] Text timeFreeTxt;
        private ShopScreenUI shopScreenUI;
        private float timeUpdate;
        private EquipmentContainerSO equipmentContainer;
        public void Initialize(ShopScreenUI shopScreenUI)
        {
            this.shopScreenUI = shopScreenUI;
            open1.onClick.AddListener(() => OpenChest(1));
            open10.onClick.AddListener(() => OpenChest(10));
            infoRatioBtn.onClick.AddListener(OnClickPreviewRatio);
            open1Txt.text = $"{price}";
            open10Txt.text = $"{(int)(price * 10 * 0.96f) + 1}";
            equipmentContainer = DataManager.Instance.equipmentContainer;
            CheckButton();
            SetKeyTxt();
            keyBtn.onClick.AddListener(OpenKey);
        }

        public void Update()
        {
            if (timeUpdate <= 0)
            {
                CheckTimeFreeChest();
                timeUpdate = 1;
            }
            else
            {
                timeUpdate -= Time.deltaTime;
            }
        }
        public void OpenChest(int numItem)
        {
            int totalPrice = numItem * price;
            if (numItem == 10)
            {
                totalPrice = (int)(totalPrice * 0.96f);
            }
            if (DataManager.Diamond < totalPrice)
            {
                return;
            }
            GameManager.Instance.UpdateTask(QuestType.OPEN_CHEST, numItem);
            DataManager.Instance.AddDiamond(-totalPrice, 0, "OpenChest");
            CreateEquipment(numItem);
        }
        public void OpenKey()
        {
            int keys = DataManager.Instance.GetKey(type);
            if (keys >= 10)
            {
                CreateEquipment(10);
                keys -= 10;
                DataManager.Instance.AddKey(-10, type);
            }
            else if (keys > 0)
            {
                CreateEquipment(1);
                keys -= 1;
                DataManager.Instance.AddKey(-1, type);
            }
            else
            {
                if (adsCountChest > 0)
                {

                    int ss = AdsHelper.Instance.showGift(GameRes.LevelCommon(), "buy_ads_open_chest", (value) =>
                    {
                        if (value == AD_State.AD_REWARD_OK)
                        {
                            GameManager.Instance.UpdateTask(QuestType.OPEN_CHEST, 1);
                            adsCountChest--;
                            AdsCountTxt.text = $"{adsCountChest}/{adsCount}";
                            if (adsCountChest <= 0)
                            {
                                timeFree = $"{SdkUtil.CurrentTimeMilis()}";
                                CheckTimeFreeChest();
                            }
                            CreateEquipment(1);
                        }
                    });
                }
            }
            SetKeyTxt();
        }

        public void CreateEquipment(int numItem)
        {
            List<EquipmentData> listItems = new List<EquipmentData>();
            for (int i = 0; i < numItem; i++)
            {
                if (firstOpen == 0)
                {
                    firstOpen = 1;
                    EquipmentData equipmentData = new EquipmentData(defaultEquipment.id, 2);
                    listItems.Add(equipmentData);
                    Inventory.Instance.AddItem(equipmentData);
                }
                else
                {
                    int id = equipmentContainer.container[Random.Range(0, equipmentContainer.container.Length)].id;
                    EquipmentData equipmentData = new EquipmentData(id, GetRarity());
                    Inventory.Instance.AddItem(equipmentData);
                    listItems.Add(equipmentData);
                }
            }
            shopScreenUI.chestOpenning.ShowReward(listItems, this);
        }
        public void CheckButton()
        {
            int keys = DataManager.Instance.GetKey(type);
            if (keys > 0)
            {
                SetActiveButton(false);
            }
            else
            {
                SetActiveButton(true);
                AdsCountTxt.text = $"{adsCountChest}/{adsCount}";
            }

        }

        public void SetActiveButton(bool status)
        {
            iconButton[0].gameObject.SetActive(status);
            iconButton[1].gameObject.SetActive(!status);
        }

        public void OnClickPreviewRatio()
        {
            this.OnPreview?.Invoke(this);
        }
        public int GetRarity()
        {
            float chance = Random.Range(0f, 1f);
            float currentRate = 0;
            openCount++;
            countChest++;
            if (openCount == 10)
            {
                openCount = 0;
                return rarityAbsoluteDrop;
            }
            int rarity = 1;
            for (int i = 0; i < rarityRates.Length; i++)
            {
                if (chance >= currentRate && chance < rarityRates[i].rate + currentRate)
                {
                    rarity = rarityRates[i].rarity;
                    break;
                }
                currentRate += rarityRates[i].rate;
            }
            if (rarity == rarityRates[rarityRates.Length - 1].rarity && countChest < 10)
            {
                rarity = Random.Range(1, 4);
            }
            if (rarity == rarityRates[rarityRates.Length - 1].rarity && countChest >= 10)
            {
                countChest = 0;
            }
            return rarity;
        }

        public void SetActiveInfoRatio(bool status)
        {
            InfoRatio.gameObject.SetActive(status);
        }

        public void SetTxtTimeFree(long time)
        {
            timeFreeTxt.text = $"Free in: {((int)(24 - ((float)time / 3600000))):00}h {((int)(60 - ((float)time % 3600000) / 60000)):00}m {((int)(60 - ((((float)time % 3600000) % 60000) / 1000))):00}s";
        }

        public void CheckTimeFreeChest()
        {
            long current = SdkUtil.CurrentTimeMilis();
            if (long.Parse(timeFree) <= 0) timeFree = $"{SdkUtil.CurrentTimeMilis()}";
            long delta = Math.Abs(current - long.Parse(timeFree));
            if (delta >= 86400000 || adsCountChest > 0)
            {
                timeFreeTxt.gameObject.SetActive(false);
                if (adsCountChest <= 0)
                {
                    adsCountChest = adsCount;
                    AdsCountTxt.text = $"{adsCountChest}/{adsCount}";
                }
            }
            else
            {
                timeFreeTxt.gameObject.SetActive(true);
                SetTxtTimeFree(delta);
            }
        }

        public void SetKeyTxt()
        {
            int keys = DataManager.Instance.GetKey(type);
            if (keys >= 10)
            {
                keyTxt.text = $"{keys}/10";
            }
            else if (keys >= 0)
            {
                keyTxt.text = $"{keys}/1";
            }
            CheckButton();
        }
        [System.Serializable]
        public struct RarityRate
        {
            public int rarity;
            [Range(0f, 1f)]
            public float rate;
        }
    }
}
