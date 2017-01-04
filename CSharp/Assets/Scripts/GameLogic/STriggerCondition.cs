namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct STriggerCondition
    {
        [FriendlyName("要求受害者是玩家队长")]
        public bool bCaptainSrc;
        [FriendlyName("要求肇事者是玩家队长")]
        public bool bCaptainAtker;
        public STriggerCondActor[] SrcActorCond;
        public STriggerCondActor[] AtkerActorCond;
        [FriendlyName("使用百分比")]
        public bool bPercent;
        [FriendlyName("百分比数")]
        public int Percent;
        [FriendlyName("技能槽位")]
        public int skillSlot;
        [FriendlyName("天赋等级")]
        public int TalentLevel;
        [FriendlyName("难度筛选")]
        public int Difficulty;
        [FriendlyName("金币数目")]
        public int GoldNum;
        [FriendlyName("全局变量")]
        public int GlobalVariable;
        private bool CheckDifficulty()
        {
            return ((this.Difficulty == 0) || (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_levelDifficulty >= this.Difficulty));
        }

        public bool FilterMatch(EGlobalGameEvent inEventType, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, object param, CTriggerMatch match, int inMatchIndex)
        {
            if (!this.CheckDifficulty())
            {
                return false;
            }
            if (((this.GlobalVariable != 0) && (Singleton<BattleLogic>.instance.m_globalTrigger != null)) && (this.GlobalVariable != Singleton<BattleLogic>.instance.m_globalTrigger.CurGlobalVariable))
            {
                return false;
            }
            if (this.bCaptainSrc && ((src == 0) || (src != Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain)))
            {
                return false;
            }
            if (this.bCaptainAtker && ((atker == 0) || (atker != Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain)))
            {
                return false;
            }
            if (this.SrcActorCond != null)
            {
                foreach (STriggerCondActor actor in this.SrcActorCond)
                {
                    if (!actor.FilterMatch(ref src))
                    {
                        return false;
                    }
                }
            }
            if (this.AtkerActorCond != null)
            {
                foreach (STriggerCondActor actor2 in this.AtkerActorCond)
                {
                    if (!actor2.FilterMatch(ref atker))
                    {
                        return false;
                    }
                }
            }
            switch (inEventType)
            {
                case EGlobalGameEvent.SpawnGroupDead:
                {
                    if (match.Originator == null)
                    {
                        break;
                    }
                    CommonSpawnGroup component = match.Originator.GetComponent<CommonSpawnGroup>();
                    SpawnGroup group2 = match.Originator.GetComponent<SpawnGroup>();
                    if ((component == null) || (component == (param as CommonSpawnGroup)))
                    {
                        if ((group2 == null) || (group2 == (param as SpawnGroup)))
                        {
                            break;
                        }
                        return false;
                    }
                    return false;
                }
                case EGlobalGameEvent.ActorDead:
                {
                    PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(match.Originator);
                    if ((actorRoot == 0) || !(actorRoot != src))
                    {
                        break;
                    }
                    return false;
                }
                case EGlobalGameEvent.ActorDamage:
                {
                    HurtEventResultInfo inHurtInfo = (HurtEventResultInfo) param;
                    if (this.FilterMatchDamage(ref inHurtInfo))
                    {
                        break;
                    }
                    return false;
                }
                case EGlobalGameEvent.UseSkill:
                {
                    ActorSkillEventParam param2 = (ActorSkillEventParam) param;
                    if (param2.slot == this.skillSlot)
                    {
                        break;
                    }
                    return false;
                }
                case EGlobalGameEvent.TalentLevelChange:
                {
                    TalentLevelChangeParam param4 = (TalentLevelChangeParam) param;
                    if (match.Condition.TalentLevel == param4.TalentLevel)
                    {
                        break;
                    }
                    return false;
                }
                case EGlobalGameEvent.BattleGoldChange:
                    if ((src != 0) && ActorHelper.IsHostCtrlActor(ref src))
                    {
                        int currentGold = (int) param;
                        if (!this.FilterBattleGoldNum(currentGold))
                        {
                            return false;
                        }
                        break;
                    }
                    return false;

                case EGlobalGameEvent.SkillUseCanceled:
                {
                    DefaultSkillEventParam param3 = (DefaultSkillEventParam) param;
                    if (param3.slot == this.skillSlot)
                    {
                        break;
                    }
                    return false;
                }
            }
            return true;
        }

        private bool FilterMatchDamage(ref HurtEventResultInfo inHurtInfo)
        {
            if (!this.bPercent)
            {
                return true;
            }
            int actorHp = inHurtInfo.src.handle.ValueComponent.actorHp;
            int hpChanged = inHurtInfo.hpChanged;
            int num3 = actorHp - hpChanged;
            int percent = this.Percent;
            if (percent < 0)
            {
                percent = 0;
            }
            else if (percent > 100)
            {
                percent = 100;
            }
            int num5 = (inHurtInfo.src.handle.ValueComponent.actorHpTotal * percent) / 100;
            return (((num3 > actorHp) && (num3 >= num5)) && (actorHp <= num5));
        }

        public bool FilterBattleGoldNum(int currentGold)
        {
            return (currentGold >= this.GoldNum);
        }
    }
}

