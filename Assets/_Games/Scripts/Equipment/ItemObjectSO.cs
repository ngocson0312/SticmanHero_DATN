using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public abstract class ItemObjectSO : ScriptableObject
    {
        public int id;
        public Sprite icon;
        public ItemType itemType;
        public string itemName;
        [TextArea(5, 10)]
        public string description;
    }
}
