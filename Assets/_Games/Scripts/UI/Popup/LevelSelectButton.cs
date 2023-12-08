using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class LevelSelectButton : MonoBehaviour
    {
        [SerializeField] Text levelText;
        [SerializeField] GameObject iconBoss;
        [SerializeField] Sprite[] bgs;
        public Button button { get; private set; }
        public int level;
        private LevelPanel levelPanel;
        public void Initialize(LevelPanel levelPanel)
        {
            this.levelPanel = levelPanel;
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectLevel);
        }

        private void SelectLevel()
        {
            levelPanel.SelectLevel(level);
        }

        public void SetTextLevel(int level)
        {
            this.level = level;
            button.image.sprite = bgs[level <= DataManager.Level ? 0 : 1];
            button.interactable = level <= DataManager.Level;
            iconBoss.SetActive(level % 4 == 0);
            levelText.text = level.ToString();
        }
    }
}
