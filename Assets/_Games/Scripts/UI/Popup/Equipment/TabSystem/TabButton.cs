using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SuperFight
{
    [RequireComponent(typeof(Button))]
    public class TabButton : MonoBehaviour
    {
        public event UnityAction<TabButton> OnSelect;
        public event UnityAction<TabButton> OnDeSelect;
        public int id;
        private Button button;
        public void Initialize(int id)
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(Select);
            this.id = id;
        }
        public void Select()
        {
            OnSelect?.Invoke(this);
        }
        public void Deselect()
        {
            OnDeSelect?.Invoke(this);
        }
    }
}
