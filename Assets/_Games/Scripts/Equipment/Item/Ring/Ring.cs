using UnityEngine;
namespace SuperFight
{
    public abstract class Ring : Equipment
    {
        protected Controller controller;
        public override void Initialize(Controller controller)
        {
            this.controller = controller;
            equipmentType = EquipmentType.RING;
        }
        public override void SetEquipmentData(EquipmentData data)
        {
            base.SetEquipmentData(data);
            int level = data.level - 1;
            switch (data.grade)
            {
                case 1:
                    equipmentStats.health = 50 + level * 10;
                    break;
                case 2:
                    equipmentStats.health = 70 + level * 20;
                    break;
                case 3:
                    equipmentStats.health = 100 + level * 30;
                    break;
                case 4:
                    equipmentStats.health = 150 + level * 40;
                    break;
                case 5:
                    equipmentStats.health = 200 + level * 50;
                    break;
                case 6:
                    equipmentStats.health = 300 + level * 50;
                    break;
            }
        }
        public virtual void OnEquip()
        {

        }
        public virtual void OnUnEquip()
        {
        }

    }
}
