using UnityEngine;
namespace SuperFight
{
    public class CheckPoint : MonoBehaviour
    {
        public int idChunk;
        public int idPoint;
        private bool isUnlocked;
        [SerializeField] GameObject fireObj;
        [SerializeField] ParticleSystem unlockFX;
        [SerializeField] Animation anim;
        public Vector3 position
        {
            get => transform.position;
        }
        public void Initialize(int idChunk, int idPoint, bool isUnlocked)
        {
            this.idChunk = idChunk;
            this.idPoint = idPoint;
            this.isUnlocked = isUnlocked;
            fireObj.SetActive(isUnlocked);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (WorldMap.Instance.IsCurrentCheckPoint(idChunk, idPoint)) return;
            if (other.GetComponent<PlayerManager>())
            {
                Unlock();
                AudioManager.Instance.PlayOneShot("CheckPoint", 1f);
                WorldMap.Instance.OnUnlockCheckPoint(idChunk, idPoint);
            }
        }
        private void Unlock()
        {
            isUnlocked = true;
            fireObj.SetActive(true);
            unlockFX.Play();
            anim.Play("CheckPointOn");
        }
    }
}
