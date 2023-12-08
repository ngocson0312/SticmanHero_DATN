using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame.sdk;
using System;
using Random = UnityEngine.Random;
namespace SuperFight
{
    public class GameManager : Singleton<GameManager>
    {
        public static GAMEMODE GameMode { get; private set; }
        public static GameState GameState { get; private set; }
        public static event Action OnPause;
        public static event Action OnResume;
        public static event Action resetDay;
        [Header("Component")]
        public DataManager dataManager;
        public UIManager uiManager;
        public QuestManager questManager;
        // public ChallengeManager challengeManager;
        public PlayerManager playerManager;
        public LevelManager levelManager;
        public GameObject standingTheme;
        [Header("Level")]
        public int maxLevel;
        public static int LevelSelected
        {
            get { return PlayerPrefs.GetInt("sf_lv_selected", 1); }
            private set { PlayerPrefs.SetInt("sf_lv_selected", value); }
        }
        private bool hasSecondChange;
        //========================================================
        private void Start()
        {
            mygame.sdk.SDKManager.CBFinishloadDing += () =>
            {
                AdsHelper.Instance.showBanner(AD_BANNER_POS.TOP, App_Open_ad_Orien.Orien_Landscape, 320);
                BackToMenu();
            };
            Initialize();
        }
        public void Initialize()
        {
            Application.targetFrameRate = 60;
            DataManager.Instance.LoadData();
            uiManager.GetPopup<DailyReward>().ResetDay();
            // challengeManager.ResetDay();
            if (PlayerPrefsUtil.Yesterday.Length <= 0)
            {
                resetDay?.Invoke();
                PlayerPrefsUtil.Yesterday = DateTime.Today.ToString();
            }
            else
            {
                DateTime yesterday = DateTime.Parse(PlayerPrefsUtil.Yesterday);
                if (yesterday < DateTime.Today)
                {
                    resetDay?.Invoke();
                    PlayerPrefsUtil.Yesterday = DateTime.Today.ToString();
                    uiManager.ShowPopup<DailyReward>(null);
                }
            }
            // challengeManager.Initialize();
            uiManager.Initialize(this);
            questManager.Initialize();
            playerManager.Initialize();
            SwitchGameState(GameState.NONE);
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                DataManager.Level++;
                PlayGame(DataManager.Level);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                DataManager.Level--;
                PlayGame(DataManager.Level);
            }
        }
#endif

        public void SwitchGameMode(GAMEMODE mode)
        {
            GameMode = mode;
        }
        public void SwitchGameState(GameState newState)
        {
            GameState = newState;
        }
        public void PlayGame(int level)
        {
            FIRhelper.logEvent($"Level_{level:000}_play");
            uiManager.transition.Transition(1, () =>
            {
                LevelSelected = level;

                if (level > maxLevel)
                {
                    if (level % 4 == 0)
                    {
                        level = Random.Range(1, maxLevel / 4) * 4;
                    }
                    else
                    {
                        level = Random.Range(15, maxLevel);
                    }
                }

                hasSecondChange = true;
                SwitchGameState(GameState.PLAYING);
                uiManager.ActiveScreen<IngameScreenUI>(false);
                standingTheme.SetActive(false);
                levelManager.LoadLevel(level);
                CameraController.Instance.CamOnStart();
            });
        }
        public void BackToMenu()
        {
            SwitchGameState(GameState.NONE);
            PlayerManager.Instance.ResetCharacter();
            levelManager.ClearLevel();
            standingTheme.SetActive(true);
            uiManager.ActiveScreen<MenuScreenUI>();
            PlayerManager.Instance.SetPosition(Vector3.zero);
            hasSecondChange = true;
            CameraController.Instance.CameraOnMain(Vector3.zero);
        }
        public void Respawn()
        {
            uiManager.transition.Transition(1f, () =>
            {
                SwitchGameState(GameState.PLAYING);
                hasSecondChange = true;
                CameraController.Instance.CamOnStart();
                playerManager.ResetCharacter();
            });
        }
        public void CompleteLevel()
        {
            if (GameState == GameState.FINISH) return;
            FIRhelper.logEvent($"Level_{LevelSelected:000}_win");
            SwitchGameState(GameState.FINISH);
            uiManager.DeactiveAllScreen(false);
            playerManager.playerController.OnWin();
            LevelSelected++;
            if (LevelSelected >= DataManager.Level)
            {
                DataManager.Level = LevelSelected;
            }
            DelayCallBack(2f, () =>
            {
                uiManager.ActiveScreen<WinScreenUI>();
            });
        }
        public void OnLose()
        {
            if (GameState == GameState.FINISH) return;
            FIRhelper.logEvent($"Level_{LevelSelected:000}_lose");
            if (hasSecondChange)
            {
                PauseGame();
                hasSecondChange = false;
                CameraController.Instance.DeathSaturation();
                uiManager.GetScreen<IngameScreenUI>().ActiveSecondChange();
            }
            else
            {
                ResumeGame();
                SwitchGameState(GameState.FINISH);
                DelayCallBack(1f, () =>
                {
                    AudioManager.Instance.PlayOneShot("DeathPiano", 1f);
                    uiManager.ActiveScreen<LoseScreenUI>();
                });
            }
        }
        public void Revive()
        {
            CameraController.Instance.ReviveSaturation();
            AudioManager.Instance.PlayOneShot("ReverseTime", 1f);
            playerManager.playerController.Revive();
            DelayCallBack(2f, () =>
            {
                ResumeGame();
                SwitchGameState(GameState.PLAYING);
                playerManager.ResetCharacter();
            });
        }
        public void PauseGame()
        {
            OnPause?.Invoke();
        }
        public void ResumeGame()
        {
            OnResume?.Invoke();
        }
        public void DelayCallBack(float duration, Action onComplete)
        {
            StartCoroutine(IEDelay());
            IEnumerator IEDelay()
            {
                float timer = duration;
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
                onComplete?.Invoke();
            }
        }
        public void UpdateTask(QuestType questType, int amount)
        {
            questManager.UpdateTask(questType, amount);
            // challengeManager.UpdateTask(questType, amount);
        }
    }
    public enum GAMEMODE
    {
        CAMPAIGN, BOSS
    }
    public enum GameState
    {
        NONE, PLAYING, PAUSE, FINISH
    }
}

