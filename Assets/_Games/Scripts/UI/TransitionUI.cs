using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
namespace SuperFight
{
    public class TransitionUI : MonoBehaviour
    {
        // [SerializeField] Image img;
        [SerializeField] Transform rect;
        // [SerializeField] Transform icon;
        public void Transition(float duration, System.Action onComplete)
        {
            gameObject.SetActive(true);
            rect.gameObject.SetActive(true);
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one * 15f, duration / 2f).OnComplete(() =>
            {
                onComplete?.Invoke();
                rect.DOScale(Vector3.zero, duration / 2f).OnComplete(() =>
                {
                    rect.gameObject.SetActive(false);
                    gameObject.SetActive(false);
                });
            });

        }

    }
}
