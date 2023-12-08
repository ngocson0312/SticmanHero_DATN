using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Crystal
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaPanel : MonoBehaviour
    {
        public RectTransform rectTransform
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                }
                return _rect;
            }
        }
        private RectTransform _rect;
        private SafeArea s;
        private void Start()
        {
            s = FindObjectOfType<SafeArea>();
            if (s)
            {
                s.AddPanel(this);
            }
        }
        private void OnDestroy()
        {
            if (s)
            {
                s.RemovePanel(this);
            }
        }
    }
}

