using UnityEngine;
using System.Collections.Generic;
using System;

namespace SuperFight
{
    public class LevelManager : Singleton<LevelManager>
    {
        private LevelObject currentLevel;
        public List<Enemy> enemies = new List<Enemy>();
        private IngameScreenUI ingameScreen;
        private int enemyCount;
        private int totalEnemy;
        private void Start()
        {
            Controller.OnDead += OnEnemyDead;
        }
        public void LoadLevel(int level)
        {
            ClearLevel();
            enemies.Clear();
            currentLevel = Resources.Load<LevelObject>("Level/Level " + level);
            currentLevel = Instantiate(currentLevel, transform);
            currentLevel.Initialize(this, GameManager.LevelSelected);
            PlayerManager.Instance.SetPosition(currentLevel.spawnPoint.position);
            PlayerManager.Instance.ResetCharacter();
            if (ingameScreen == null)
            {
                ingameScreen = UIManager.Instance.GetScreen<IngameScreenUI>();
            }
            enemyCount = 0;
            totalEnemy = enemies.Count;
            if (currentLevel.bossFightArena != null && currentLevel.bossFightArena.Length > 0)
            {
                ingameScreen.SetEnemyCount("");
            }
            else
            {
                ingameScreen.SetEnemyCount(enemyCount + "/" + totalEnemy);
            }
            currentLevel.OnEnemyDead(enemyCount, totalEnemy);
        }

        private void OnEnemyDead(Controller obj)
        {
            if (obj.characterType == CharacterType.Character) return;
            enemyCount++;
            ingameScreen.SetEnemyCount(enemyCount + "/" + totalEnemy);
            currentLevel.OnEnemyDead(enemyCount, totalEnemy);
        }

        public void ClearLevel()
        {
            if (currentLevel)
            {
                currentLevel.OnClearLevel();
                Destroy(currentLevel.gameObject);
            }
        }
    }
}

