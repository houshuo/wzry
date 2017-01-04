namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;

    [EventCategory("MMGame/Newbie")]
    public class NewbieSelectActor : TickEvent
    {
        public enActorType ActorType;
        public bool bPauseGame;
        public int configId;
        public int index;

        public override BaseEvent Clone()
        {
            NewbieSelectActor actor = ClassObjPool<NewbieSelectActor>.Get();
            actor.CopyData(this);
            return actor;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            NewbieSelectActor actor = src as NewbieSelectActor;
            this.ActorType = actor.ActorType;
            this.bPauseGame = actor.bPauseGame;
            this.configId = actor.configId;
            this.index = actor.index;
        }

        private PoolObjHandle<ActorRoot> GetActor(List<PoolObjHandle<ActorRoot>> actorList, int configId, int index)
        {
            PoolObjHandle<ActorRoot> handle = new PoolObjHandle<ActorRoot>();
            if (actorList != null)
            {
                int num = 0;
                int count = actorList.Count;
                int num3 = 0;
                while (num3 < count)
                {
                    PoolObjHandle<ActorRoot> handle2 = actorList[num3];
                    if ((handle2.handle.TheActorMeta.ConfigId == configId) && (num == index))
                    {
                        return handle2;
                    }
                    num3++;
                    num++;
                }
            }
            return handle;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> handle;
            base.Process(_action, _track);
            switch (this.ActorType)
            {
                case enActorType.All:
                    handle = this.GetActor(Singleton<GameObjMgr>.GetInstance().GameActors, this.configId, this.index);
                    break;

                case enActorType.Hero:
                    handle = this.GetActor(Singleton<GameObjMgr>.GetInstance().HeroActors, this.configId, this.index);
                    break;

                case enActorType.Soldier:
                    handle = this.GetActor(Singleton<GameObjMgr>.GetInstance().SoldierActors, this.configId, this.index);
                    break;

                default:
                    handle = new PoolObjHandle<ActorRoot>();
                    break;
            }
            Singleton<BattleSkillHudControl>.GetInstance().AddHighlightForActor(handle, this.bPauseGame);
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        public enum enActorType
        {
            All,
            Hero,
            Soldier
        }
    }
}

