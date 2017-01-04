namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_Spawn")]
    public class AreaEventTriggerSpawn : AreaEventTrigger
    {
        public SpawnGroup[] SpawnGroups = new SpawnGroup[0];

        protected override void BuildTriggerWrapper()
        {
            base.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerSpawn);
            GameObject[] objArray = new GameObject[this.SpawnGroups.Length];
            for (int i = 0; i < this.SpawnGroups.Length; i++)
            {
                objArray[i] = null;
                if (this.SpawnGroups[i] != null)
                {
                    objArray[i] = this.SpawnGroups[i].gameObject;
                }
            }
            base.PresetActWrapper.RefObjList = objArray;
            base.PresetActWrapper.Init();
        }
    }
}

