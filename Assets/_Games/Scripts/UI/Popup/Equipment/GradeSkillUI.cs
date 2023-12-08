using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperFight
{
    public class GradeSkillUI : MonoBehaviour
    {
        public Text desText;
        public Image iconImg;
        public Sprite[] sprs;
        public void SetActive(bool status)
        {
            gameObject.SetActive(status);
        }
        public void SetInfo(string des, bool isUnlock)
        {
            desText.text = des;
            desText.color = isUnlock ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
            iconImg.sprite = sprs[isUnlock ? 1 : 0];
        }
    }
}
