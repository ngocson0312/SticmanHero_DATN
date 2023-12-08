using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(ItemContainerSO))]
    public class ItemContainerSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("GenerateID"))
            {
                ItemContainerSO itemContainer = target as ItemContainerSO;

                for (int i = 0; i < itemContainer.container.Length; i++)
                {
                    itemContainer.container[i].id = i + 1;
                    EditorUtility.SetDirty(itemContainer.container[i]);
                }
            }
        }
    }
#endif
    [CreateAssetMenu(fileName = "ItemContainer", menuName = "Data/ItemContainer")]
    public class ItemContainerSO : ScriptableObject
    {
        public ItemObjectSO[] container;
        public ItemObjectSO GetItemObject(int id)
        {
            for (int i = 0; i < container.Length; i++)
            {
                if (container[i].id == id)
                {
                    return container[i];
                }
            }
            return null;
        }
    }
}
