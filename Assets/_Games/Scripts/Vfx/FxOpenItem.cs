using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SuperFight
{
    public class FxOpenItem : MonoBehaviour
    {
        public float duration;
        public Transform chestPos;
        public void Fly(Vector3 pos)
        {
            gameObject.SetActive(true);
            transform.position = chestPos.position;
            transform.DOMove(pos, duration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.DOKill();
                gameObject.SetActive(false);
            });
        }
    }
}
