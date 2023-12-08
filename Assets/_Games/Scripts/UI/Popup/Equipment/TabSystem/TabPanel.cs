using UnityEngine;
namespace SuperFight
{
    public abstract class TabPanel : MonoBehaviour
    {
        public virtual void Active()
        {
            gameObject.SetActive(true);
        }
        public virtual void Deactive()
        {
            gameObject.SetActive(false);
        }
    }
}
