using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(WorldMap))]
    public class WorldMapEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Setup"))
            {
                ChunkMap[] chunks = Resources.LoadAll<ChunkMap>("Chunks");
                WorldMap worldMap = target as WorldMap;
                worldMap.chunkDatas = new ChunkInfo[chunks.Length];
                for (int i = 0; i < chunks.Length; i++)
                {
                    ChunkInfo chunkData = new ChunkInfo();
                    chunkData.id = chunks[i].chunkID;
                    chunkData.path = "Chunks/" + chunks[i].name;
                    chunkData.chunkName = chunks[i].areaName;
                    chunkData.chunkBound = chunks[i].GetComponent<ChunkMap>().GetBounds();
                    worldMap.chunkDatas[i] = chunkData;
                }
                worldMap.chunkDatas = worldMap.chunkDatas.OrderBy(t => t.id).ToArray();
                EditorUtility.SetDirty(worldMap);
            }
            if (GUILayout.Button("SpawnAllChunk"))
            {
                ChunkMap[] chunks = Resources.LoadAll<ChunkMap>("Chunks");
                WorldMap worldMap = target as WorldMap;
                for (int i = 0; i < chunks.Length; i++)
                {
                    PrefabUtility.InstantiatePrefab(chunks[i], worldMap.transform);
                }
                EditorUtility.SetDirty(worldMap);
            }
            if (GUILayout.Button("ClearAll"))
            {
                WorldMap worldMap = target as WorldMap;
                for (int i = 0; i < worldMap.transform.childCount; i++)
                {
                    DestroyImmediate(worldMap.transform.GetChild(i).gameObject);
                }
                EditorUtility.SetDirty(worldMap);
            }
        }
    }
#endif
    public class WorldMap : Singleton<WorldMap>
    {
        public ChunkInfo[] chunkDatas;
        public Vector2 activeBounds = new Vector2(30, 30);
        private Dictionary<int, ChunkMap> listSpawned;
        private Transform target;
        // public int mapID;
        // [ContextMenu("XXX")]
        // public void LoadMapX()
        // {
        //     ChunkMap chunkMap = Resources.Load<ChunkMap>(chunkDatas[mapID].path);
        //     chunkMap = Instantiate(chunkMap, transform);
        //     chunkMap.Initialize(this);
        // }
        private int currentCheckPoint
        {
            get { return PlayerPrefs.GetInt("check_point", 0); }
            set { PlayerPrefs.SetInt("check_point", value); }
        }
        private int currentChunkPoint
        {
            get { return PlayerPrefs.GetInt("chunk_point", 1); }
            set { PlayerPrefs.SetInt("chunk_point", value); }
        }
        private UserData userData;
        private DataManager dataManager;
        public void Initialize(DataManager dataManager, Transform target)
        {
            listSpawned = new Dictionary<int, ChunkMap>();
            this.dataManager = dataManager;
            userData = dataManager.data;
            this.target = target;
            int index = GetChunkIndex(currentChunkPoint);
            ChunkMap chunkMap = SpawnChunk(index);
            PlayerManager.Instance.SetPosition(chunkMap.GetCheckPoint(currentCheckPoint).position);
        }
        public void ResetMap()
        {
            UIManager.Instance.ActiveScreen<IngameScreenUI>();
            foreach (var item in listSpawned)
            {
                Destroy(item.Value.gameObject);
            }
            for (int i = 0; i < chunkDatas.Length; i++)
            {
                chunkDatas[i].isSpawned = false;
            }
            listSpawned.Clear();
            int index = GetChunkIndex(currentChunkPoint);
            ChunkMap chunkMap = SpawnChunk(index);
            PlayerManager.Instance.SetPosition(chunkMap.GetCheckPoint(currentCheckPoint).position);
        }
        public void BackToBase()
        {
            currentChunkPoint = 1;
            currentCheckPoint = 2;
            ResetMap();
        }
        public void LoadChunk(int id)
        {
            currentChunkPoint = id;
            currentCheckPoint = 0;
            ResetMap();
        }
        private void Update()
        {
            Bounds activeBound = new Bounds(target.position, activeBounds);
            for (int i = 0; i < chunkDatas.Length; i++)
            {
                if (!chunkDatas[i].chunkBound.Intersects(activeBound))
                {
                    DeactiveChunk(i);
                    chunkDatas[i].deactiveTime += Time.deltaTime;
                    if (chunkDatas[i].deactiveTime > 30)
                    {
                        RemoveChunk(i);
                    }
                }
                else
                {
                    SpawnChunk(i);
                }
            }
        }
        private ChunkMap SpawnChunk(int i)
        {
            if (chunkDatas[i].isSpawned)
            {
                if (!chunkDatas[i].isActive)
                {
                    listSpawned[chunkDatas[i].id].SetActive(true);
                    chunkDatas[i].isActive = true;
                    chunkDatas[i].deactiveTime = 0;
                    UpdateMapBound();
                }
                return listSpawned[chunkDatas[i].id];
            }
            chunkDatas[i].isSpawned = true;
            chunkDatas[i].isActive = true;
            ChunkMap chunkMap = Resources.Load<ChunkMap>(chunkDatas[i].path);
            chunkMap = Instantiate(chunkMap, transform);
            chunkMap.Initialize(this);
            chunkMap.SetActive(true);
            chunkDatas[i].deactiveTime = 0;
            listSpawned.Add(chunkDatas[i].id, chunkMap);
            UpdateMapBound();
            return chunkMap;
        }
        private void DeactiveChunk(int index)
        {
            if (!chunkDatas[index].isSpawned) return;
            if (!chunkDatas[index].isActive) return;
            listSpawned[chunkDatas[index].id].SetActive(false);
            chunkDatas[index].isActive = false;
            // Destroy(listSpawned[chunkDatas[index].id].gameObject);
            // listSpawned.Remove(chunkDatas[index].id);
            UpdateMapBound();
        }
        private void RemoveChunk(int index)
        {
            if (!chunkDatas[index].isSpawned) return;
            chunkDatas[index].isSpawned = false;
            chunkDatas[index].isActive = false;
            listSpawned[chunkDatas[index].id].RemoveChunk();
            listSpawned.Remove(chunkDatas[index].id);
            UpdateMapBound();
        }
        private int GetChunkIndex(int id)
        {
            for (int i = 0; i < chunkDatas.Length; i++)
            {
                if (chunkDatas[i].id == id)
                {
                    return i;
                }
            }
            return 0;
        }
        private void UpdateMapBound()
        {
            Bounds bound = new Bounds();
            foreach (var item in listSpawned)
            {
                if (item.Value.gameObject.activeInHierarchy)
                {
                    bound.Encapsulate(item.Value.GetBounds());
                }
            }
            CameraController.Instance.SetLimitCamera(bound.min.x, bound.min.y, bound.max.x, bound.max.y);
        }
        //==================================================
        public void OnUnlockCheckPoint(int chunkID, int checkPoint)
        {
            ChunkData chunkData = GetChunkData(chunkID);
            currentCheckPoint = checkPoint;
            currentChunkPoint = chunkID;
            if (chunkData != null)
            {
                if (chunkData.checkPoints.Contains(checkPoint)) return;
                chunkData.checkPoints.Add(checkPoint);
            }
            else
            {
                chunkData = new ChunkData(chunkID);
                chunkData.checkPoints.Add(checkPoint);
                userData.worldMapData.chunkDatas.Add(chunkData);
            }
            dataManager.SaveData();
        }
        public bool IsCurrentCheckPoint(int chunkID, int checkPoint)
        {
            if (chunkID != currentChunkPoint) return false;
            if (checkPoint != currentCheckPoint) return false;
            return true;
        }
        public ChunkData GetChunkData(int id)
        {
            for (int i = 0; i < userData.worldMapData.chunkDatas.Count; i++)
            {
                if (userData.worldMapData.chunkDatas[i].id == id) return userData.worldMapData.chunkDatas[i];
            }
            return null;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan * 0.3f;
            for (int i = 0; i < chunkDatas.Length; i++)
            {
                Gizmos.DrawCube(chunkDatas[i].chunkBound.center, chunkDatas[i].chunkBound.size);
            }
            Gizmos.color = Color.green;
            Transform tt = transform;
            if (target)
            {
                tt = target;
            }
            Gizmos.DrawWireCube(tt.position, activeBounds);
        }
    }
    [System.Serializable]
    public struct ChunkInfo
    {
        public int id;
        public float deactiveTime;
        public string path;
        public string chunkName;
        public Bounds chunkBound;
        public bool isSpawned;
        public bool isActive;
    }
    [System.Serializable]
    public class WorldMapData
    {
        public WorldMapData()
        {
            chunkDatas = new List<ChunkData>();
        }
        public List<ChunkData> chunkDatas;
    }
    [System.Serializable]
    public class ChunkData
    {
        public ChunkData(int id)
        {
            this.id = id;
            checkPoints = new List<int>();
        }
        public int id;
        public List<int> checkPoints;
    }
}
