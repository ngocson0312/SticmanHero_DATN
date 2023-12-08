using UnityEngine;
namespace SuperFight
{

#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(EnemyContainerSO))]
    public class EnemyContainerSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                EnemyContainerSO enemyContainer = target as EnemyContainerSO;
                enemyContainer.enemiesName = new string[enemyContainer.container.Length];
                for (int i = 0; i < enemyContainer.enemiesName.Length; i++)
                {
                    enemyContainer.enemiesName[i] = enemyContainer.container[i].name;
                }
                EditorUtility.SetDirty(enemyContainer);
            }
        }
    }
#endif
    [CreateAssetMenu(fileName = "EnemyContainer", menuName = "Data/EnemyContainer")]
    public class EnemyContainerSO : ScriptableObject
    {
        public string[] enemiesName;
#if UNITY_EDITOR
        public Enemy[] container;
#endif
    }
}
