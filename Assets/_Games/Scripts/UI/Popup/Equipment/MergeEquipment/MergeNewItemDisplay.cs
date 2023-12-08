using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class MergeNewItemDisplay : MonoBehaviour
    {
        public Image gradeImg;
        public Image iconImg;
        public Text levelText;
        private bool inited;
        public void Display(Sprite grade, Sprite icon, int level)
        {
            if (!inited)
            {
                inited = true;
                GetComponent<Button>().onClick.AddListener(Close);
            }
            gradeImg.sprite = grade;
            iconImg.sprite = icon;
            levelText.text = "Lv." + level;
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
