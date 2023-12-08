using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SuperFight
{
    public class ItemCoin : MonoBehaviour
    {
        private bool isActive;
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private TextMeshPro coinText;
        [SerializeField] private Animation anim;
        [SerializeField] private GameObject display;
        [SerializeField] private float force;
        private int amount;
        private float timer;
        public void Initialize(int amount)
        {
            isActive = true;
            this.amount = amount;
            coinText.text = "+" + amount;
            coinText.gameObject.SetActive(false);
            display.SetActive(true);
            rb2D.AddForce(new Vector2(Random.Range(-force / 2, force / 2), force * 1.5f));
            timer = 0;
        }
        private void Update()
        {
            if (!isActive) return;
            timer += Time.deltaTime;
            if (timer > 10)
            {
                FactoryObject.Despawn("Item", transform);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive || timer < 0.3f || !other.GetComponent<PlayerManager>()) return;
            isActive = false;
            anim.Play("CoinOn");
            AudioManager.Instance.PlayOneShot("eff_collect_coin", 0.8f);
            DataManager.Instance.AddCoin(amount, 0, "pickup", false);
            display.SetActive(false);
            coinText.gameObject.SetActive(true);
            FactoryObject.Despawn("Item", transform, 1f);
        }
    }
}
