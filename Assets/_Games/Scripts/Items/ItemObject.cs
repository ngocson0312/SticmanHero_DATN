using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public abstract class ItemObject : MonoBehaviour
    {
        public Controller controller;
        public void Start()
        {
            Initialize();
        }
        public abstract void Initialize();
        public abstract void ResetObject();
    }
}
