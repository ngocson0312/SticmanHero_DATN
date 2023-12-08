using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(ChunkMap))]
    public class ChunkMapEditor : Editor
    {
        private bool isPreview;
        private void OnEnable()
        {
            isPreview = false;
            enemies = new List<Enemy>();
            ChunkMap chunkMap = target as ChunkMap;
        }
        private List<Enemy> enemies;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Setup"))
            {
                ChunkMap chunkMap = target as ChunkMap;
                chunkMap.enemySpawners = chunkMap.GetComponentsInChildren<EnemySpawner>();
                chunkMap.checkPoints = chunkMap.GetComponentsInChildren<CheckPoint>();
                chunkMap.bossFightArena = chunkMap.GetComponentsInChildren<BossFightArena>();
                for (int i = 0; i < chunkMap.checkPoints.Length; i++)
                {
                    Vector3 pos = chunkMap.checkPoints[i].transform.position;
                    pos.z = 5;
                    chunkMap.checkPoints[i].transform.position = pos;
                }
                EditorUtility.SetDirty(chunkMap);
            }
            if (GUILayout.Button("Preview"))
            {
                foreach (var item in enemies)
                {
                    GameObject.DestroyImmediate(item.gameObject);
                }
                ChunkMap chunkMap = target as ChunkMap;
                for (int i = 0; i < chunkMap.enemySpawners.Length; i++)
                {
                    Enemy enemy = Resources.Load<Enemy>("Enemy/" + chunkMap.enemySpawners[i].enemyName);
                    enemy = PrefabUtility.InstantiatePrefab(enemy) as Enemy;
                    enemy.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    enemy.transform.parent = chunkMap.enemySpawners[i].transform;
                    enemy.transform.localPosition = Vector3.zero;
                    enemies.Add(enemy);
                }
            }
        }
    }
#endif
    public class ChunkMap : MonoBehaviour
    {
        [Header("Info")]
        public int chunkID;
        public string areaName;
        public Vector3 center;
        public Vector3 size;
        public CheckPoint[] checkPoints;
        public EnemySpawner[] enemySpawners;
        public BossFightArena[] bossFightArena;
        public void Initialize(WorldMap worldMap)
        {
            for (int i = 0; i < enemySpawners.Length; i++)
            {
                // enemySpawners[i].Spawn();
            }
            ChunkData chunkData = worldMap.GetChunkData(chunkID);
            for (int i = 0; i < checkPoints.Length; i++)
            {
                bool isUnlocked = false;
                if (chunkData != null)
                {
                    isUnlocked = chunkData.checkPoints.Contains(i);
                }
                checkPoints[i].Initialize(chunkID, i, isUnlocked);
            }
            for (int i = 0; i < bossFightArena.Length; i++)
            {
                // bossFightArena[i].Initialize();
            }
        }
        public void SetActive(bool status)
        {
            gameObject.SetActive(status);
        }
        public void RemoveChunk()
        {
            for (int i = 0; i < bossFightArena.Length; i++)
            {
                bossFightArena[i].OnClear();
            }
            Destroy(gameObject);
        }
        public Vector3 GetChunkSpawnPosition()
        {
            return checkPoints[0].position;
        }
        private void Update()
        {
            if (GameManager.GameState == GameState.PAUSE) return;
            for (int i = 0; i < enemySpawners.Length; i++)
            {
                enemySpawners[i].UpdateLogic();
            }
        }
        public CheckPoint GetCheckPoint(int id)
        {
            return checkPoints[id];
        }
        public Bounds GetBounds()
        {
            return new Bounds(center, size);
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawCube(center, size);
        }
    }
}
