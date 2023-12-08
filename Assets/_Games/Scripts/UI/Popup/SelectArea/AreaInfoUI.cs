using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class AreaInfoUI : MonoBehaviour
    {
        public Text areaName;
        public Image areaImg;
        public int id;
        public void Initialize(int id, Sprite icon, string nameArea)
        {
            gameObject.SetActive(true);
            this.id = id;
            areaName.text = nameArea;
            areaImg.sprite = icon;
        }
    }
}
