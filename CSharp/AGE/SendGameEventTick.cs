namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Drama")]
    public class SendGameEventTick : TickEvent
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int eventAtkerId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int eventSrcId;
        public GameEventDef eventType;

        public override BaseEvent Clone()
        {
            SendGameEventTick tick = ClassObjPool<SendGameEventTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SendGameEventTick tick = src as SendGameEventTick;
            this.eventType = tick.eventType;
            this.eventSrcId = tick.eventSrcId;
            this.eventAtkerId = tick.eventAtkerId;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.eventSrcId);
            PoolObjHandle<ActorRoot> myLastAtker = _action.GetActorHandle(this.eventAtkerId);
            if ((myLastAtker == 0) && (actorHandle != 0))
            {
                myLastAtker = actorHandle.handle.ActorControl.myLastAtker;
            }
            DefaultGameEventParam prm = new DefaultGameEventParam(actorHandle, myLastAtker);
            if (this.eventType == GameEventDef.Event_GameEnd)
            {
                Singleton<GameEventSys>.instance.PostEvent<DefaultGameEventParam>(this.eventType, ref prm);
            }
            else
            {
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(this.eventType, ref prm);
            }
        }
    }
}

