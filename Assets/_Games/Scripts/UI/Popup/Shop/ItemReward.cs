using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SuperFight
{
    public class ItemReward : MonoBehaviour
    {
        public Image rank;
        public Image type;
        public ParticleSystem bumFx;
        public void SetInfos(EquipmentData itemData, Sprite grade)
        {
            EquipmentObjectSO equipmentObject = DataManager.Instance.equipmentContainer.GetEquipmentObject(itemData.itemID);
            rank.sprite = grade;
            type.sprite = equipmentObject.icon;
            transform.localScale = Vector3.zero;

        }

        public void SetSize(float dur)
        {
            bumFx.Play();
            transform.DOScale(Vector3.one, dur + 0.2f).OnComplete ( () => {
                transform.DOKill();
            });
        }
    }
}
