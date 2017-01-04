namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class SoldierControl : MonoBehaviour, IUpdateLogic
    {
        private void Awake()
        {
            Singleton<BattleLogic>.GetInstance().SetupSoldier(this);
        }

        public void Startup()
        {
        }

        public void UpdateLogic(int delta)
        {
        }
    }
}

