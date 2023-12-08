using System;
using System.Collections;
using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class IngameScreenUI : ScreenUI
    {
        public FillBar playerHealthBar;
        public FillBar bossHealthBar;
        [SerializeField] Text enemyCountText;
        [SerializeField] Text levelText;
        [SerializeField] RevivePanel revivePanel;
        [SerializeField] GameObject healthWarning;
        [SerializeField] Button nextBtn;
        [SerializeField] Button settingBtn;
        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);
            revivePanel.Initialize();
            settingBtn.onClick.AddListener(OpenSetting);
            nextBtn.onClick.AddListener(Next);
        }
        private void Next()
        {
            int lv = GameManager.LevelSelected + 1;
            GameManager.Instance.PlayGame(lv);
        }
        public override void Active()
        {
            base.Active();
            AudioManager.Instance.StopAllMusic();
            AudioManager.Instance.PlayMusic("music_gameplay", 0.6f, true);
            bossHealthBar.Deactive();
            ActiveWarning(false);
            levelText.text = DataManager.PlayerLevel.ToString();
            if (GameManager.LevelSelected == 1)
            {
                GameManager.Instance.DelayCallBack(1.5f, () =>
                {
                    GameManager.Instance.PauseGame();
                    uiManager.ShowPopup<TipPopup>(() =>
                    {
                        GameManager.Instance.ResumeGame();
                    });
                });
            }
        }
        public void SetEnemyCount(string content)
        {
            enemyCountText.text = content;
        }
        public void ActiveWarning(bool status)
        {
            healthWarning.SetActive(status);
        }
        public void ActiveSecondChange()
        {
            ActiveWarning(false);
            revivePanel.Active();
        }
        private void OpenSetting()
        {
            GameManager.Instance.PauseGame();
            var x = uiManager.ShowPopup<SettingPopup>(() =>
            {
                GameManager.Instance.ResumeGame();
            });
            x.ShowType(true);
        }
    }
}
