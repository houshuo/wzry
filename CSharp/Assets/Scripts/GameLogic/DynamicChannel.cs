namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    public class DynamicChannel : FuncRegion
    {
        public GameObject enablePassEffect;
        public GameObject unablePassEffect;

        public override void Startup()
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            bool flag = (hostPlayer != null) && (hostPlayer.PlayerCamp == base.CampType);
            if (this.enablePassEffect != null)
            {
                this.enablePassEffect.SetActive(flag);
            }
            if (this.unablePassEffect != null)
            {
                this.unablePassEffect.SetActive(!flag);
            }
            base.Startup();
        }
    }
}

