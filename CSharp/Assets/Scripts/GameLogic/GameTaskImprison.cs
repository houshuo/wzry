namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class GameTaskImprison : GameTask
    {
        private bool FilterWaitActor(ref PoolObjHandle<ActorRoot> actor)
        {
            return ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (actor.handle.TheActorMeta.ActorCamp == this.WaitCamp));
        }

        protected override void OnClose()
        {
            List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterWaitActor));
            for (int i = 0; i < list.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = list[i];
                handle.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
            }
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnInitial()
        {
        }

        protected override void OnStart()
        {
            List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(this.FilterWaitActor));
            for (int i = 0; i < list.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = list[i];
                handle.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
            }
        }

        protected override void OnTimeOver()
        {
            base.Current = this.Target;
        }

        public override float Progress
        {
            get
            {
                return (1f - (((float) base.TimeRemain) / ((float) base.TimeLimit)));
            }
        }

        protected COM_PLAYERCAMP WaitCamp
        {
            get
            {
                return (COM_PLAYERCAMP) base.Config.iParam1;
            }
        }
    }
}

