using System;
using UnityEngine;
using UnityEngine.UI;
namespace SuperFight
{
    public class IndexStone : MonoBehaviour
    {
        public int index;
        private InventoryStone inventoryStone;
        public void Initialize(InventoryStone inventoryStone, int index)
        {
            this.inventoryStone = inventoryStone;
            this.index = index;
            GetComponent<Button>().onClick.AddListener(EmbedStone);
        }
        public void SetActive(bool status)
        {
            gameObject.SetActive(status);
        }
        private void EmbedStone()
        {
            inventoryStone.EmbedStone(index);
        }
    }
    public enum StoneName
    {
        NONE, SHARP, FIRE, ICE, POISON, BLEED, LIGHTNING
    }
}
