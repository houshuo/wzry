namespace Assets.Scripts.GameLogic.Treasure
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal abstract class NormalStrategy : BaseStrategy
    {
        protected NormalStrategy()
        {
        }

        public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert((bool) actor);
            if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                ResMonsterCfgInfo info = base.FindMonsterConfig(actor.handle.TheActorMeta.ConfigId);
                object[] inParameters = new object[] { actor.handle.TheActorMeta.ConfigId };
                DebugHelper.Assert(info != null, "怪物数据档里面找不到id:{0}", inParameters);
                if (info != null)
                {
                    RES_DROP_PROBABILITY_TYPE iDropProbability = (RES_DROP_PROBABILITY_TYPE) info.iDropProbability;
                    if (iDropProbability == RES_DROP_PROBABILITY_TYPE.RES_PROBABILITY_SETTLE)
                    {
                        this.FinishDrop();
                    }
                    else if ((this.hasRemain && (iDropProbability != ((RES_DROP_PROBABILITY_TYPE) 0))) && (FrameRandom.Random(100) <= ((ushort) iDropProbability)))
                    {
                        this.PlayDrop();
                    }
                }
            }
        }

        protected override bool hasRemain
        {
            get
            {
                return (base.DropedCount < (this.maxCount - 1));
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

