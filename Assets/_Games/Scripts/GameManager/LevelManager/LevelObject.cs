using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(LevelObject))]
    [CanEditMultipleObjects]
    public class LevelObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SetUp();
        }
        [CanEditMultipleObjects]
        public void SetUp()
        {
            if (GUILayout.Button("SetUp"))
            {
                LevelObject level = (LevelObject)target;
                for (int i = 0; i < targets.Length; i++)
                {
                    LevelObject levelObject = targets[i] as LevelObject;
                    Transform[] trs = levelObject.GetComponentsInChildren<Transform>();
                    List<Transform> xs = new List<Transform>();
                    for (int z = 0; z < trs.Length; z++)
                    {
                        if (trs[z].name == "Door")
                        {
                            levelObject.doorAnchor = trs[z];
                        }
                        if (trs[z].name == "PlayerAnchor")
                        {
                            levelObject.spawnPoint = trs[z];
                        }
                        if (trs[z].name.Contains("BillBoard"))
                        {
                            xs.Add(trs[z]);
                        }
                    }
                    levelObject.admixAnchors = xs.ToArray();
                    levelObject.enemySpawners = levelObject.GetComponentsInChildren<EnemySpawner>();
                    levelObject.bossFightArena = levelObject.GetComponentsInChildren<BossFightArena>();
                    EditorUtility.SetDirty(levelObject);
                }
            }
        }
    }
#endif

    public class LevelObject : MonoBehaviour
    {
        public Transform spawnPoint;
        public float minX = -50;
        public float maxX = 50;
        public float minY = -50;
        public float maxY = 50;
        public EnemySpawner[] enemySpawners;
        public BossFightArena[] bossFightArena;
        public Transform[] admixAnchors;
        public Transform doorAnchor;
        private FinishDoor finishDoor;
        public void Initialize(LevelManager levelManager, int level)
        {
            finishDoor = FactoryObject.Spawn<FinishDoor>("Item", "Door");
            Vector3 doorPos = doorAnchor.position;
            doorPos.z = 5;
            finishDoor.transform.position = doorPos;
            finishDoor.Initialize();
            for (int i = 0; i < enemySpawners.Length; i++)
            {
                levelManager.enemies.Add(enemySpawners[i].Spawn(level));
            }

            for (int i = 0; i < bossFightArena.Length; i++)
            {
                bossFightArena[i].Initialize(finishDoor.gameObject);
            }
            if (admixAnchors != null && admixAnchors.Length > 0)
            {
                for (int i = 0; i < admixAnchors.Length; i++)
                {
                    BaseAdCanvasObject ad = AdCanvasHelper.GenAd(AdCanvasSize.Size6x5, admixAnchors[i].position, Vector3.zero);
                    if (ad != null)
                    {
                        ad.transform.position = ad.transform.position;
                        ad.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                        ad.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
            }
        }
        public void OnEnemyDead(int enemyCount, int totalEnemy)
        {
            finishDoor.UpdateDoor(enemyCount, totalEnemy);
        }
        public void OnClearLevel()
        {
            for (int i = 0; i < bossFightArena.Length; i++)
            {
                bossFightArena[i].OnClear();
            }
            FactoryObject.Despawn("Item", finishDoor.transform);
            AdCanvasHelper.FreeAll();
        }
        private void OnDrawGizmos()
        {
            Vector3 center = new Vector3((maxX + minX) / 2, (maxY + minY) / 2);
            Vector3 size = new Vector3(Mathf.Abs(maxX) + Mathf.Abs(minX), Mathf.Abs(maxY) + Mathf.Abs(minY));
            Gizmos.DrawWireCube(center, size);
        }

    }
}
