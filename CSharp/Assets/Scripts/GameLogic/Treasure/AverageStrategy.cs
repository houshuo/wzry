namespace Assets.Scripts.GameLogic.Treasure
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;

    internal abstract class AverageStrategy : BaseStrategy
    {
        protected AverageStrategy()
        {
        }

        public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert((bool) actor);
            if (((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && this.hasRemain) && (FrameRandom.Random(100) <= MonoSingleton<GlobalConfig>.instance.NormalMonsterDropItemProbability))
            {
                this.PlayDrop();
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

