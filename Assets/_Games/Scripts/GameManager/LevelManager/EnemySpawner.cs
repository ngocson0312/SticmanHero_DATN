using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(EnemySpawner))]
    [CanEditMultipleObjects]
    public class EnemySpawnerEditor : Editor
    {
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return a;
        }
        public EnemyContainerSO enemyContainer;
        public string[] enemies;
        private int index;
        private void OnEnable()
        {
            EnemySpawner spawner = target as EnemySpawner;
            if (enemyContainer == null)
            {
                enemyContainer = GetAllInstances<EnemyContainerSO>()[0];
            }
            if (enemies == null)
            {
                enemies = new string[enemyContainer.enemiesName.Length];
                for (int i = 0; i < enemies.Length; i++)
                {
                    enemies[i] = enemyContainer.enemiesName[i];
                }
            }
            if (spawner.enemyName == null || spawner.enemyName.Length < 1)
            {
                string namen = "";
                for (int i = 0; i < enemyContainer.enemiesName.Length; i++)
                {
                    if (spawner.gameObject.name == enemyContainer.enemiesName[i])
                    {
                        namen = spawner.gameObject.name;
                        index = i;
                        break;
                    }
                }
                if (namen == "")
                {
                    namen = enemyContainer.enemiesName[0];
                }
                spawner.enemyName = namen;
                spawner.gameObject.name = namen;
                EditorUtility.SetDirty(spawner);
            }
            else
            {
                for (int i = 0; i < enemyContainer.enemiesName.Length; i++)
                {
                    if (enemyContainer.enemiesName[i] == spawner.enemyName)
                    {
                        index = i;
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EnemyGenerate();
        }

        private void EnemyGenerate()
        {
            GUILayout.Label("\n-----------GenerateEnemy-----------\n", new GUIStyle { alignment = TextAnchor.MiddleCenter });
            GUIContent content = new GUIContent("Enemy");
            int idx = EditorGUILayout.Popup(content, index, enemies);
            Texture2D t = AssetPreview.GetAssetPreview(enemyContainer.container[idx].gameObject);
            GUILayout.Label(t, new GUIStyle { alignment = TextAnchor.MiddleCenter });
            if (idx != index)
            {
                EnemySpawner spawner = target as EnemySpawner;
                spawner.enemyName = enemyContainer.enemiesName[idx];
                index = idx;
                spawner.gameObject.name = spawner.enemyName;
                EditorUtility.SetDirty(spawner);
            }
        }
    }
#endif
    public class EnemySpawner : MonoBehaviour
    {
        public CharacterStats characterStats;
        public string enemyName;
        public int coinReward;
        private float timer;
        private Enemy enemy;
        public void UpdateLogic()
        {

        }
        public Enemy Spawn(int level)
        {
            timer = 30f;
            if (enemy == null)
            {
                enemy = Resources.Load<Enemy>("Enemy/" + enemyName);
                enemy = Instantiate(enemy, transform);
                enemy.Initialize();
            }
            float maxHeath = enemy.originalStats.health;
            float damage = enemy.originalStats.damage;
            if (level < 10)
            {
                maxHeath = maxHeath + ((float)level / 4f) * maxHeath * 0.7f;
                damage = damage + ((float)level / 4f) * damage * 0.7f;
            }
            else if (level >= 10 && level <= 24)
            {
                maxHeath = maxHeath + ((float)level / 4f) * maxHeath;
                damage = damage + ((float)level / 4f) * damage;
            }
            else if (level > 24 && level <= 40)
            {
                maxHeath = maxHeath + ((float)level / 4f) * maxHeath * 2f;
                damage = damage + ((float)level / 4f) * damage * 1.5f;
            }
            else
            {
                maxHeath = maxHeath + ((float)level / 4f) * maxHeath * 2.5f;
                damage = damage + ((float)level / 4f) * damage * 2f;
            }
            CharacterStats enemyStats = new CharacterStats();
            enemyStats.health = (int)maxHeath;
            enemyStats.damage = (int)damage;
            enemy.transform.position = transform.position;
            enemy.ConfigStats(enemyStats, coinReward);
            enemy.Active();
            return enemy;
        }
    }
}
