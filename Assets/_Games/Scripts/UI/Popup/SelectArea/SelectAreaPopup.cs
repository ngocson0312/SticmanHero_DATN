using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using System;

namespace SuperFight
{
    public class SelectAreaPopup : PopupUI
    {
        public Transform content;
        public List<AreaInfoUI> listButton;
        public AreaInfoUI areaUiPrf;
        public SimpleScrollSnap simpleScrollSnap;
        public Button backButton;
        public Button selectButton;
        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            listButton = new List<AreaInfoUI>();
            backButton.onClick.AddListener(Hide);
            selectButton.onClick.AddListener(Select);
        }
        public override void Show(Action onClose)
        {
            base.Show(onClose);
            ChunkInfo[] chunkDatas = WorldMap.Instance.chunkDatas;
            List<ChunkInfo> chunkInfos = new List<ChunkInfo>();
            for (int i = 0; i < chunkDatas.Length; i++)
            {
                if (WorldMap.Instance.GetChunkData(chunkDatas[i].id) == null) continue;
                chunkInfos.Add(chunkDatas[i]);
            }
            if (listButton.Count <= chunkInfos.Count)
            {
                for (int i = 0; i < chunkInfos.Count; i++)
                {
                    Sprite spr = Resources.Load<Sprite>("Area/" + chunkDatas[i].id);
                    if (i < listButton.Count)
                    {
                        listButton[i].Initialize(chunkInfos[i].id, spr, chunkInfos[i].path);
                    }
                    else
                    {
                        AreaInfoUI area = Instantiate(areaUiPrf, content);
                        area.Initialize(chunkDatas[i].id, spr, chunkDatas[i].path);
                        listButton.Add(area);
                    }
                }
            }
            else
            {
                for (int i = 0; i < listButton.Count; i++)
                {
                    if (i < chunkInfos.Count)
                    {
                        Sprite spr = Resources.Load<Sprite>("Area/" + chunkDatas[i].id);
                        listButton[i].Initialize(chunkInfos[i].id, spr, chunkInfos[i].path);
                    }
                    else
                    {
                        listButton[i].gameObject.SetActive(false);
                    }
                }
            }
            simpleScrollSnap.Initialize();
        }
        private void Select()
        {
            int index = simpleScrollSnap.SelectedPanel;
            Hide();
            uiManager.transition.Transition(2f, () =>
            {
                WorldMap.Instance.LoadChunk(listButton[index].id);
            });
        }
    }
}
