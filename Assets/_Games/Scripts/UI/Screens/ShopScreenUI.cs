using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePlugins;
using UnityEngine.UI;
using mygame.sdk;
using System;

namespace SuperFight
{
    public class ShopScreenUI : PopupUI
    {
        public List<ChestPack> chestPacks;
        public List<InappButton> inappButtons;
        public ScrollRect scrollRect;
        public ChestOpenningScreen chestOpenning;
        public Button backBtn;
        [Header("Tutorial")]
        public Button openChestKeyBtn;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            chestOpenning.Initialize();
            for (int i = 0; i < chestPacks.Count; i++)
            {
                chestPacks[i].type = i;
                chestPacks[i].OnPreview += Preview;
                chestPacks[i].Initialize(this);
            }
            for (int i = 0; i < inappButtons.Count; i++)
            {
                inappButtons[i].Initialize(i);
            }
            backBtn.onClick.AddListener(Hide);
        }
        public void Preview(ChestPack chestPack)
        {
            for (int i = 0; i < chestPacks.Count; i++)
            {
                if (chestPacks[i] != chestPack)
                {
                    chestPacks[i].SetActiveInfoRatio(false);
                }
                else
                {
                    chestPack.SetActiveInfoRatio(!chestPack.InfoRatio.gameObject.activeSelf);
                }
            }
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            scrollRect.horizontalNormalizedPosition = 0;
            for (int i = 0; i < chestPacks.Count; i++)
            {
                chestPacks[i].CheckTimeFreeChest();
            }
            UpdateKey();
            if (Tutorial.TutorialStep == 2)
            {
                Tutorial.Instance.TutorialClick(openChestKeyBtn, 1f, () =>
                {
                    Tutorial.TutorialStep = 3;
                });
            }
        }
        public void UpdateKey()
        {
            for (int i = 0; i < chestPacks.Count; i++)
            {
                chestPacks[i].SetKeyTxt();
            }
        }
    }
}

