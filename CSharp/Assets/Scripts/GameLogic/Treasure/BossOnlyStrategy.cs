namespace Assets.Scripts.GameLogic.Treasure
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal abstract class BossOnlyStrategy : BaseStrategy
    {
        protected BossOnlyStrategy()
        {
        }

        public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert((bool) actor);
            if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                ResMonsterCfgInfo info = base.FindMonsterConfig(actor.handle.TheActorMeta.ConfigId);
                if ((info != null) && (info.bMonsterGrade == 3))
                {
                    this.FinishDrop();
                }
            }
        }

        public override bool isSupportDrop
        {
            get
            {
                return true;
            }
        }
    }
}

