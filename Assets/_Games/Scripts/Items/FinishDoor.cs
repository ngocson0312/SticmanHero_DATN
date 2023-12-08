using UnityEngine;
using TMPro;
namespace SuperFight
{
    public class FinishDoor : MonoBehaviour
    {
        [SerializeField] private GameObject locker;
        [SerializeField] private TextMeshPro requireText;
        private bool isTriggered;
        private bool isOpen;
        public void Initialize()
        {
            isTriggered = false;
            isOpen = false;
        }
        public void UpdateDoor(int enemyCount, int totalEnemy)
        {
            requireText.text = enemyCount + "/" + totalEnemy;
            locker.SetActive(enemyCount < totalEnemy);
            isOpen = enemyCount >= totalEnemy;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isTriggered || !isOpen) return;
            if (other.GetComponent<PlayerController>() != null)
            {
                isTriggered = true;
                GameManager.Instance.CompleteLevel();
            }
        }
    }
}
