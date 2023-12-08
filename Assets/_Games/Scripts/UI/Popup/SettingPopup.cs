using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mygame.sdk;
using System;

namespace SuperFight
{
    public class SettingPopup : PopupUI
    {
        [SerializeField] Button closeBtn;
        [SerializeField] Button soundBtn;
        [SerializeField] Button musicBtn;
        [SerializeField] Button vibrateBtn;
        [SerializeField] Button rateBtn;
        [SerializeField] Button backBtn;
        [SerializeField] GameObject[] OnObjects;
        [SerializeField] GameObject[] OffObjects;
        private int vibrateOption
        {
            get { return PlayerPrefs.GetInt("key_config_vibrate", 1); }
            set { PlayerPrefs.SetInt("key_config_vibrate", value); }
        }
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            closeBtn.onClick.AddListener(Hide);
            soundBtn.onClick.AddListener(Sound);
            musicBtn.onClick.AddListener(Music);
            vibrateBtn.onClick.AddListener(Vibrate);
            rateBtn.onClick.AddListener(Rate);
            backBtn.onClick.AddListener(BackToMenu);

            OnObjects[0].SetActive(AudioManager.SoundSetting == 1);
            OffObjects[0].SetActive(AudioManager.SoundSetting != 1);

            OnObjects[1].SetActive(AudioManager.MusicSetting == 1);
            OffObjects[1].SetActive(AudioManager.MusicSetting != 1);

            OnObjects[2].SetActive(vibrateOption == 1);
            OffObjects[2].SetActive(vibrateOption != 1);
        }

        public void ShowType(bool isIngame)
        {
            backBtn.gameObject.SetActive(isIngame);
        }

        private void BackToMenu()
        {
            Hide();
           // AdsHelper.Instance.showFull(false, GameManager.LevelSelected, false, false, "end", false);
            uiManager.transition.Transition(1, () =>
            {
                GameManager.Instance.BackToMenu();
            });
        }

        private void Sound()
        {
            if (AudioManager.SoundSetting != 1)
            {
                AudioManager.Instance.EnableSound(true);
            }
            else
            {
                AudioManager.Instance.EnableSound(false);
            }
            OnObjects[0].SetActive(AudioManager.SoundSetting == 1);
            OffObjects[0].SetActive(AudioManager.SoundSetting != 1);
        }
        private void Music()
        {
            if (AudioManager.MusicSetting != 1)
            {
                AudioManager.Instance.EnableMusic(true);
            }
            else
            {
                AudioManager.Instance.EnableMusic(false);
            }
            OnObjects[1].SetActive(AudioManager.MusicSetting == 1);
            OffObjects[1].SetActive(AudioManager.MusicSetting != 1);
        }
        private void Vibrate()
        {
            if (vibrateOption != 0)
            {
                vibrateOption = 0;
            }
            else
            {
                vibrateOption = 1;
            }
            OnObjects[2].SetActive(vibrateOption == 1);
            OffObjects[2].SetActive(vibrateOption != 1);
        }

        private void Rate()
        {
            GameHelper.Instance.rate();
        }
    }
}
