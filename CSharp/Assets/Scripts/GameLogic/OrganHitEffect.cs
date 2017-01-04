namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct OrganHitEffect
    {
        private int InitHp;
        private int CurHp;
        private int AccHp;
        public void Reset(OrganWrapper InWrapper)
        {
            DebugHelper.Assert(InWrapper.actor != null, "Wrapper上的actor怎么是空的呢");
            if ((InWrapper.actor != null) && (InWrapper.actor.ValueComponent != null))
            {
                this.InitHp = this.CurHp = InWrapper.actor.ValueComponent.actorHp;
                this.AccHp = 0;
            }
        }

        public void OnHpChanged(OrganWrapper InWrapper)
        {
            DebugHelper.Assert(InWrapper.actor != null, "Wrapper上的actor怎么是空的呢");
            if ((InWrapper.actor != null) && (InWrapper.actor.ValueComponent != null))
            {
                int actorHp = InWrapper.actor.ValueComponent.actorHp;
                int num2 = this.CurHp - actorHp;
                this.CurHp = actorHp;
                if ((num2 > 0) && (actorHp > 0))
                {
                    this.AccHp += num2;
                    int num3 = (int) ((this.AccHp * 100f) / ((float) this.InitHp));
                    if (num3 >= 0x19)
                    {
                        this.AccHp -= (this.InitHp * 0x19) / 100;
                        this.OnHitEffect(InWrapper);
                    }
                }
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddAction("Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Red");
            preloadTab.AddAction("Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Blue");
        }

        public void OnHitEffect(OrganWrapper InWrapper)
        {
            COM_PLAYERCAMP actorCamp = InWrapper.actor.TheActorMeta.ActorCamp;
            GameObject[] objArray1 = new GameObject[] { InWrapper.actor.gameObject };
            MonoSingleton<ActionManager>.GetInstance().PlayAction((actorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Blue" : "Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Red", true, false, objArray1);
        }
        private enum EConfig
        {
            StepRate = 0x19
        }
    }
}

