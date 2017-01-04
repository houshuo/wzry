namespace Assets.Scripts.GameLogic.Treasure
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal abstract class OrganRandomStrategy : BaseStrategy
    {
        protected OrganRandomStrategy()
        {
        }

        public override void NotifyDropEvent(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert((bool) actor);
            if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(actor.handle.TheActorMeta.ConfigId);
                object[] inParameters = new object[] { actor.handle.TheActorMeta.ConfigId };
                DebugHelper.Assert(dataCfgInfoByCurLevelDiff != null, "can't find organ config, id={0}", inParameters);
                if (dataCfgInfoByCurLevelDiff != null)
                {
                    if (dataCfgInfoByCurLevelDiff.bOrganType == 2)
                    {
                        this.FinishDrop();
                    }
                    else if (this.hasRemain && (FrameRandom.Random(100) <= MonoSingleton<GlobalConfig>.instance.OrganDropItemProbability))
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

