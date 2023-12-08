using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperFight
{
    public class MissionTab : TabPanel
    {
        public override void Active()
        {
            base.Active();
            UIManager.Instance.ShowPopup<QuestUI>(null);
        }
    }
}
