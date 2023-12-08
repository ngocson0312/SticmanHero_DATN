using System.Collections;
using UnityEngine;
namespace SuperFight
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(BossFightArena))]
    public class BossFightArenaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Setup"))
            {
                BossFightArena arena = target as BossFightArena;
                arena.bosses = arena.GetComponentsInChildren<Boss>();
                EditorUtility.SetDirty(arena);
            }
        }
    }
#endif
    public class BossFightArena : MonoBehaviour
    {
        public float arenaRangeX = 10f;
        public float arenaRangeY = 10f;
        public bool lockCamera;
        public Vector2 offsetCamera;
        public Transform center;
        public Boss[] bosses;
        public RewardInfo[] rewards;
        private Chest chest;
        private FillBar healthBar;
        private int bossCount;
        private float totalHealth = 0;
        private float currentHealth = 0;
        private bool battleActive;
        public float timeDelayBattleActive;
        private GameObject door;
        public void Initialize(GameObject door)
        {
            this.door = door;
            bossCount = bosses.Length;
            battleActive = false;
            currentHealth = 0;
            totalHealth = 0;
            for (int i = 0; i < bosses.Length; i++)
            {
                bosses[i].Initialize(this);
                bosses[i].OnDisappear += OnDisappear;
                totalHealth += bosses[i].runtimeStats.health;
            }
            currentHealth = totalHealth;
            IngameScreenUI ingameScreen = UIManager.Instance.GetScreen<IngameScreenUI>();
            healthBar = ingameScreen.bossHealthBar;
            chest = FactoryObject.Spawn<Chest>("Item", "Chest");
            chest.SetActive(false);
            door.SetActive(false);
            GameManager.OnPause += OnPauseGame;
            GameManager.OnResume += OnResumeGame;
        }
        private void Update()
        {
            if (PlayerManager.Instance == null || battleActive) return;
            Vector3 centerPos = center.position;
            centerPos.z = 0;
            Bounds bound = new Bounds(centerPos, new Vector3(arenaRangeX, arenaRangeY, 0));
            Vector2 playerPos = PlayerManager.Instance.transform.position;
            if (bound.Contains(playerPos))
            {
                OnTriggerBattle();
            }
        }
        public void OnTriggerBattle()
        {
            // lockObj.SetActive(true);
            healthBar.InitializeBar(true);
            Bounds bound = new Bounds(center.position, new Vector3(arenaRangeX, arenaRangeY, 0));
            if (lockCamera)
            {
                CameraController.Instance.SetTargetFollow(center, true);
            }
            CameraController.Instance.SetOrthoSize(bound.size, 1.5f);
            CameraController.Instance.SetOffset(offsetCamera);
            StartCoroutine(IEDelayBattleActive());
            IEnumerator IEDelayBattleActive()
            {
                yield return new WaitForSeconds(timeDelayBattleActive);
                for (int i = 0; i < bosses.Length; i++)
                {
                    bosses[i].Active();
                }
                battleActive = true;
            }
        }
        private void OnDisappear(Boss boss)
        {
            if (currentHealth <= 0)
            {
                chest.transform.position = boss.position + Vector3.up;
                if (DataManager.Level > GameManager.LevelSelected)
                {
                    for (int i = 0; i < rewards.Length; i++)
                    {
                        rewards[i].amount /= 3;
                        if (rewards[i].amount == 0)
                        {
                            rewards[i].amount = 1;
                        }
                    }
                }
                chest.Initialize(rewards);
                chest.SetActive(true);
            }
        }
        public void OnPauseGame()
        {
            for (int i = 0; i < bosses.Length; i++)
            {
                bosses[i].Pause();
            }
        }
        public void OnResumeGame()
        {
            for (int i = 0; i < bosses.Length; i++)
            {
                bosses[i].Resume();
            }
        }
        public void OnBossTakeDamage(int damage, Boss boss)
        {
            currentHealth -= damage;
            healthBar.UpdateFillBar(currentHealth / totalHealth);
            if (currentHealth <= 0)
            {
                CameraController.Instance.SetOrthoSize(8, 0.5f, true);
                // lockObj.SetActive(false);
                StartCoroutine(IEOnFinishFight());
                IEnumerator IEOnFinishFight()
                {
                    yield return new WaitForSeconds(2f);
                    healthBar.Deactive();
                    yield return new WaitForSeconds(1.5f);
                    CameraController.Instance.SetTargetFollow(PlayerManager.Instance.transform, false);
                    CameraController.Instance.SetOrthoSize(CameraController.CAMERA_ORTHOSIZE, 2f, false);
                    CameraController.Instance.SetOffset(new Vector3(0, 1.5f));
                    door.SetActive(true);
                }
            }

        }
        public void OnClear()
        {
            FactoryObject.Despawn("Item", chest.transform);
            GameManager.OnPause -= OnPauseGame;
            GameManager.OnResume -= OnResumeGame;
        }
        private void OnDrawGizmos()
        {
            if (center == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center.position, new Vector3(arenaRangeX, arenaRangeY, 0));
        }
    }
}