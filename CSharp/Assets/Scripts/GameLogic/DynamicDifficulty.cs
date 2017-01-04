namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class DynamicDifficulty
    {
        public void FightStart()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && (curLvelContext.m_warmHeroAiDiffInfo != null))
            {
                ResBattleDynamicDifficulty warmHeroAiDiffInfo = curLvelContext.m_warmHeroAiDiffInfo;
                List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
                for (int i = 0; i < campActors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = campActors[i];
                    ActorRoot root = handle.handle;
                    if (root.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        PropertyHelper mActorValue = root.ValueComponent.mActorValue;
                        ValueDataInfo info1 = mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTOUTPUTRATE];
                        info1.baseValue += warmHeroAiDiffInfo.iSelfAttackAdd;
                        ValueDataInfo info2 = mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE];
                        info2.baseValue += warmHeroAiDiffInfo.iSelfDefenceAdd;
                    }
                }
            }
        }
    }
}

