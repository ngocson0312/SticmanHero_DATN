using System.Collections.Generic;
using UnityEngine;
using mygame.sdk;
using SuperFight;
using System;
namespace SuperFight
{
    public class DataManager : Singleton<DataManager>
    {
        public static event Action<int, int, float, bool> OnAddCoin;
        public static event Action<int, int, float> OnAddDiamond;
        public static event Action<int, int, bool> OnAddExp;
        public static event Action OnUpgrade;
        public static event Action OnDataSaved;
        public static event Action OnAddKey;
        public UserData data;
        public EquipmentContainerSO equipmentContainer;
        private string userPrefData
        {
            get { return PlayerPrefs.GetString("sf_data", ""); }
            set { PlayerPrefs.SetString("sf_data", value); }
        }
        public static int Coin
        {
            get { return GameRes.getRes(RES_type.GOLD); }
        }
        public static int Diamond
        {
            get { return GameRes.getRes(RES_type.CRYSTAL); }
        }
        public static int Level
        {
            get { return GameRes.GetLevel(Level_type.Normal); }
            set { GameRes.SetLevel(Level_type.Normal, value); }
        }
        public static int PlayerLevel
        {
            get { return PlayerPrefs.GetInt("player_level", 1); }
            set { PlayerPrefs.SetInt("player_level", value); }
        }
        public int silverKey
        {
            get { return PlayerPrefs.GetInt("silver_key_count", 0); }
            set { PlayerPrefs.SetInt("silver_key_count", value); }
        }

        public int goldKey
        {
            get { return PlayerPrefs.GetInt("gold_key_count", 0); }
            set { PlayerPrefs.SetInt("gold_key_count", value); }
        }

        public void AddCoin(int value, float delayTime, string where, bool showAnim = true)
        {
            OnAddCoin(Coin, Coin + value, delayTime, showAnim);
            GameRes.AddRes(RES_type.GOLD, value, where, true, 0);
            if (value < 0)
            {
                QuestManager.Instance.UpdateTask(QuestType.SPEND_COIN, -value);
                // ChallengeManager.Instance.UpdateTask(QuestType.SPEND_COIN, -value);
            }
            else
            {
                QuestManager.Instance.UpdateTask(QuestType.EARN_COIN, value);
                // ChallengeManager.Instance.UpdateTask(QuestType.EARN_COIN, value);
            }
        }
        public void AddDiamond(int value, float delayTime, string where)
        {
            OnAddDiamond(Diamond, Diamond + value, delayTime);
            GameRes.AddRes(RES_type.CRYSTAL, value, where, true, 0);
            if (value < 0)
            {
                QuestManager.Instance.UpdateTask(QuestType.SPEND_DIAMOND, -value);
                // ChallengeManager.Instance.UpdateTask(QuestType.SPEND_DIAMOND, -value);
            }
            else
            {
                QuestManager.Instance.UpdateTask(QuestType.EARN_DIAMOND, value);
                // ChallengeManager.Instance.UpdateTask(QuestType.EARN_DIAMOND, value);
            }
        }

        public void AddKey(int amount, int type)
        {
            if (type == 0)
            {
                silverKey += amount;
            }
            else if (type == 1)
            {
                goldKey += amount;
            }
            OnAddKey?.Invoke();
        }

        public int GetKey(int type)
        {
            if (type == 0)
            {
                return silverKey;
            }
            else if (type == 1)
            {
                return goldKey;
            }
            return 0;
        }
        public void AddExp(int amount)
        {
            data.experience += amount;
            int require = GetExpRequire(data.level);
            bool levelUp = false;
            if (data.experience >= require)
            {
                data.level++;
                int expResidual = data.experience - require;
                data.experience = expResidual;
                levelUp = true;
            }
            OnAddExp?.Invoke(data.level, data.experience, levelUp);
            SaveData();
        }
        public int GetCurrentExp()
        {
            return data.experience;
        }
        public int GetExpRequire(int level)
        {
            return (int)Mathf.Pow((level / 0.1f), 2);
        }
        public int GetCurrentExpRequire()
        {
            return (int)Mathf.Pow((data.level / 0.1f), 2);
        }
        public void UpgradeLevel()
        {
            PlayerLevel++;
            OnUpgrade?.Invoke();
        }
        public void LoadData()
        {
            string pref = userPrefData;
            if (!string.IsNullOrEmpty(pref))
            {
                data = JsonUtility.FromJson<UserData>(pref);
            }
            else
            {
                data = new UserData();
            }
            Inventory.Instance.LoadData(this, data);
            Debug.Log("Load:" + pref);
        }
        public void SaveData()
        {
            string dataJson = JsonUtility.ToJson(data);
            userPrefData = dataJson;
            Debug.Log("Save:" + dataJson);
            OnDataSaved?.Invoke();
        }
    }
    [System.Serializable]
    public class UserData
    {
        public UserData()
        {
            inventoryData = new InventoryData();
            worldMapData = new WorldMapData();
            experience = 0;
            level = 1;
        }
        public int experience;
        public int level;
        public InventoryData inventoryData;
        public WorldMapData worldMapData;
    }
}
