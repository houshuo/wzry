namespace Assets.Scripts.GameLogic
{
    using System;

    public class TreasureChestMgr : Singleton<TreasureChestMgr>
    {
        private ITreasureChestStrategy DropStrategy;
        private StarSystemFactory Factory = new StarSystemFactory(typeof(TreasureChestStrategyAttribute), typeof(ITreasureChestStrategy));
        private int maxDropCount;

        public override void Init()
        {
            base.Init();
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.NotifyDropEvent));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
        }

        public void NotifyDropEvent(ref GameDeadEventParam prm)
        {
            DebugHelper.Assert((bool) prm.src);
            if ((ActorHelper.IsHostEnemyActor(ref prm.src) && (this.DropStrategy != null)) && this.DropStrategy.isSupportDrop)
            {
                this.DropStrategy.NotifyDropEvent(prm.src);
            }
        }

        private void OnDrop()
        {
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.DropTreasure);
        }

        private void OnGameEnd(ref DefaultGameEventParam prm)
        {
            if (this.DropStrategy != null)
            {
                this.DropStrategy.NotifyMatchEnd();
            }
            this.maxDropCount = 0;
        }

        public void Reset(SLevelContext InLevelContext, int InMaxCount)
        {
            this.StopDrop();
            this.maxDropCount = InMaxCount;
            this.DropStrategy = this.Factory.Create((int) InLevelContext.m_pveLevelType) as ITreasureChestStrategy;
            DebugHelper.Assert(this.DropStrategy != null, "no game type support you say a j8!");
            if (this.DropStrategy != null)
            {
                this.DropStrategy.Initialize(this.maxDropCount);
                this.DropStrategy.OnDropTreasure += new OnDropTreasureChestDelegate(this.OnDrop);
            }
        }

        public void StopDrop()
        {
            if (this.DropStrategy != null)
            {
                this.DropStrategy.OnDropTreasure -= new OnDropTreasureChestDelegate(this.OnDrop);
                this.DropStrategy.Stop();
                this.DropStrategy = null;
            }
        }

        public int droppedCount
        {
            get
            {
                return ((this.DropStrategy == null) ? 0 : this.DropStrategy.droppedCount);
            }
        }
    }
}

