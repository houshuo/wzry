namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class MoveCityDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            MoveCityDuration duration = ClassObjPool<MoveCityDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveCityDuration duration = src as MoveCityDuration;
            this.targetId = duration.targetId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            this.MoveCity();
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actorObj != 0)
            {
                this.actorObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveCity);
            }
            base.Leave(_action, _track);
        }

        private void MoveCity()
        {
            if (this.actorObj != 0)
            {
                VInt3 zero = VInt3.zero;
                VInt3 outPosWorld = VInt3.zero;
                if (Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.actorObj.handle.TheActorMeta, true, out outPosWorld, out zero))
                {
                    VInt num3;
                    if (PathfindingUtility.GetGroundY(outPosWorld, out num3))
                    {
                        this.actorObj.handle.groundY = num3;
                        outPosWorld.y = num3.i;
                    }
                    this.actorObj.handle.forward = zero;
                    this.actorObj.handle.location = outPosWorld;
                    this.actorObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveCity);
                    DefaultGameEventParam prm = new DefaultGameEventParam(this.actorObj, this.actorObj);
                    Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, ref prm);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.actorObj.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

