namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class TriggerActionJungle : TriggerActionBase
    {
        public TriggerActionJungle(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override void Destroy()
        {
        }

        private void ModifyHorizonMarks(PoolObjHandle<ActorRoot> src, ITrigger inTrigger, bool enterOrLeave)
        {
            <ModifyHorizonMarks>c__AnonStorey64 storey = new <ModifyHorizonMarks>c__AnonStorey64 {
                src = src
            };
            if (storey.src != 0)
            {
                int num = !enterOrLeave ? -1 : 1;
                List<PoolObjHandle<ActorRoot>> actors = (inTrigger as AreaEventTrigger).GetActors(new Func<PoolObjHandle<ActorRoot>, bool>(storey.<>m__3F));
                for (int i = 0; i < actors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = actors[i];
                    handle.handle.HorizonMarker.AddShowMark(storey.src.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
                    storey.src.handle.HorizonMarker.AddShowMark(handle.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Jungle, num * 1);
                }
                COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(storey.src.handle.TheActorMeta.ActorCamp);
                for (int j = 0; j < othersCmp.Length; j++)
                {
                    storey.src.handle.HorizonMarker.AddHideMark(othersCmp[j], HorizonConfig.HideMark.Jungle, num * 1);
                }
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            this.ModifyHorizonMarks(src, inTrigger, true);
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            this.ModifyHorizonMarks(src, inTrigger, false);
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }

        [CompilerGenerated]
        private sealed class <ModifyHorizonMarks>c__AnonStorey64
        {
            internal PoolObjHandle<ActorRoot> src;

            internal bool <>m__3F(PoolObjHandle<ActorRoot> enr)
            {
                return (enr.handle.TheActorMeta.ActorCamp != this.src.handle.TheActorMeta.ActorCamp);
            }
        }
    }
}

