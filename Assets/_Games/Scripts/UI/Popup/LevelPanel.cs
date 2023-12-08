using System;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class LevelPanel : MonoBehaviour
    {
        private int index;
        private LevelSelectButton[] levelSelectButtons;
        public Button nextBtn;
        public Button backBtn;
        public Button closeBtn;
        public void Initialize()
        {
            index = 0;
            nextBtn.onClick.AddListener(Next);
            backBtn.onClick.AddListener(Back);
            closeBtn.onClick.AddListener(Close);
            levelSelectButtons = GetComponentsInChildren<LevelSelectButton>();
            for (int i = 0; i < levelSelectButtons.Length; i++)
            {
                levelSelectButtons[i].Initialize(this);
            }
            gameObject.SetActive(false);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Refresh();
        }
        private void Next()
        {
            index++;
            Refresh();
        }
        private void Back()
        {
            if (index == 0) return;
            index--;
            Refresh();
        }
        private void Refresh()
        {
            backBtn.gameObject.SetActive(index > 0);
            nextBtn.gameObject.SetActive(index * levelSelectButtons.Length < DataManager.Level);
            for (int i = 0; i < levelSelectButtons.Length; i++)
            {
                levelSelectButtons[i].SetTextLevel((i + 1) + (levelSelectButtons.Length * index));
            }
        }
        public void SelectLevel(int level)
        {
            Close();
            GameManager.Instance.PlayGame(level);
        }
    }
}
