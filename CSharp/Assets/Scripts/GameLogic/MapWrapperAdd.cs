namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class MapWrapperAdd : MonoBehaviour, IUpdateLogic
    {
        public ActorConfig CareObjActorConfig;
        public SoldierRegion CareSoldierRegion;

        private void Awake()
        {
            if (this.CareSoldierRegion != null)
            {
                this.CareSoldierRegion.bTriggerEvent = true;
            }
        }

        public void UpdateLogic(int delta)
        {
        }
    }
}

