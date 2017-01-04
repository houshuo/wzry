namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class BattleStatistic : Singleton<BattleStatistic>
    {
        public COMDT_ACNT_INFO acntInfo;
        public bool bSelfCampHaveWinningFlag;
        private DictionaryView<uint, CampInfo> campStat = new DictionaryView<uint, CampInfo>();
        private DictionaryView<uint, Dictionary<int, DestroyStat>> destroyStats = new DictionaryView<uint, Dictionary<int, DestroyStat>>();
        public COMDT_SETTLE_HERO_RESULT_DETAIL heroSettleInfo;
        public CBattleBuffStat m_battleBuffStat = new CBattleBuffStat();
        public CBattleDeadStat m_battleDeadStat = new CBattleDeadStat();
        public CHeroSkillStat m_heroSkillStat = new CHeroSkillStat();
        private int m_iBattleResult;
        private uint m_lastBestPlayer;
        public CPlayerLocationStat m_locStat = new CPlayerLocationStat();
        private uint m_loseMvpId;
        public DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>> m_NonHeroInfo = new DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>>();
        private AchievementRecorder m_optAchievementRecorder = new AchievementRecorder();
        public CPlayerBehaviorStat m_playerBehaviorStat = new CPlayerBehaviorStat();
        public CPlayerKDAStat m_playerKDAStat = new CPlayerKDAStat();
        public CPlayerSoulLevelStat m_playerSoulLevelStat = new CPlayerSoulLevelStat();
        public CShenFuStat m_shenFuStat = new CShenFuStat();
        public GET_SOUL_EXP_STATISTIC_INFO m_stSoulStatisticInfo = new GET_SOUL_EXP_STATISTIC_INFO();
        public VDStat m_vdStat = new VDStat();
        private uint m_winMvpId;
        public COMDT_REWARD_MULTIPLE_DETAIL multiDetail;
        public COMDT_RANK_SETTLE_INFO rankInfo;
        public COMDT_REWARD_DETAIL Rewards;
        public COMDT_PVPSPECITEM_OUTPUT SpecialItemInfo;

        private void CondfoToDestroyStat(ResDT_ConditionInfo[] astCond)
        {
            for (int i = 0; i < astCond.Length; i++)
            {
                if ((astCond[i].dwType == 1) && (astCond[i].KeyDetail[1] != 0))
                {
                    DestroyStat stat;
                    Dictionary<int, DestroyStat> dictionary;
                    if (!this.destroyStats.TryGetValue((uint) astCond[i].KeyDetail[0], out dictionary))
                    {
                        dictionary = new Dictionary<int, DestroyStat>();
                        this.destroyStats.Add((uint) astCond[i].KeyDetail[0], dictionary);
                    }
                    if (!dictionary.TryGetValue(astCond[i].KeyDetail[1], out stat))
                    {
                        stat = new DestroyStat();
                        dictionary.Add(astCond[i].KeyDetail[1], stat);
                    }
                }
            }
        }

        private void doDestroyStat(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
        {
            DestroyStat stat;
            Dictionary<int, DestroyStat> dictionary;
            if (!this.destroyStats.TryGetValue((uint) src.handle.TheActorMeta.ActorType, out dictionary))
            {
                dictionary = new Dictionary<int, DestroyStat>();
                this.destroyStats.Add((uint) src.handle.TheActorMeta.ActorType, dictionary);
            }
            if (!dictionary.TryGetValue(src.handle.TheActorMeta.ConfigId, out stat))
            {
                stat = new DestroyStat();
                dictionary.Add(src.handle.TheActorMeta.ConfigId, stat);
            }
            COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp;
            COM_PLAYERCAMP actorCamp = src.handle.TheActorMeta.ActorCamp;
            int num = (playerCamp == actorCamp) ? 0 : 1;
            if (num == 1)
            {
                if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                {
                    if ((atker != 0) && (atker.handle.TheActorMeta.ActorCamp == playerCamp))
                    {
                        stat.CampEnemyNum++;
                    }
                }
                else
                {
                    stat.CampEnemyNum++;
                }
            }
            else if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
            {
                if ((atker != 0) && (atker.handle.TheActorMeta.ActorCamp != playerCamp))
                {
                    stat.CampSelfNum++;
                }
            }
            else
            {
                stat.CampSelfNum++;
            }
            this.destroyStats[(uint) src.handle.TheActorMeta.ActorType][src.handle.TheActorMeta.ConfigId] = stat;
        }

        public uint GetBestPlayer()
        {
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetEnumerator();
            float num = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_KILL_FACTOR)) / 10000f;
            float num2 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_ASSIST_FACTOR)) / 10000f;
            float num3 = ((float) GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_DEAD_FACTOR)) / 10000f;
            uint lastBestPlayer = this.m_lastBestPlayer;
            float b = 0f;
            int numKill = 0;
            PlayerKDA rkda = null;
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                rkda = current.Value;
                float a = ((rkda.numKill * num) + (rkda.numAssist * num2)) - (rkda.numDead * num3);
                if (Mathf.Approximately(a, b))
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    if (pair2.Value.numKill >= numKill)
                    {
                        if (rkda.PlayerId != this.m_lastBestPlayer)
                        {
                            lastBestPlayer = rkda.PlayerId;
                        }
                        b = a;
                        KeyValuePair<uint, PlayerKDA> pair3 = enumerator.Current;
                        numKill = pair3.Value.numKill;
                    }
                }
                else if (a >= b)
                {
                    lastBestPlayer = rkda.PlayerId;
                    b = a;
                    KeyValuePair<uint, PlayerKDA> pair4 = enumerator.Current;
                    numKill = pair4.Value.numKill;
                }
            }
            if (Mathf.Approximately(b, 0f))
            {
                this.m_lastBestPlayer = 0;
                return 0;
            }
            this.m_lastBestPlayer = lastBestPlayer;
            return lastBestPlayer;
        }

        public CampInfo GetCampInfoByCamp(COM_PLAYERCAMP campType)
        {
            CampInfo info;
            if (this.campStat.TryGetValue((uint) campType, out info))
            {
                return info;
            }
            return null;
        }

        public void GetCampsByScoreRank(RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType, out List<COM_PLAYERCAMP> result, out List<int> resultScore)
        {
            result = new List<COM_PLAYERCAMP>();
            resultScore = new List<int>();
            Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
            DictionaryView<uint, CampInfo>.Enumerator enumerator = this.campStat.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, CampInfo> current = enumerator.Current;
                CampInfo info = current.Value;
                if (info != null)
                {
                    KeyValuePair<uint, CampInfo> pair2 = enumerator.Current;
                    uint key = pair2.Key;
                    int score = info.GetScore(inDataSubType);
                    if (score >= 0)
                    {
                        dictionary.Add(key, score);
                    }
                }
            }
            Dictionary<uint, int>.Enumerator enumerator2 = dictionary.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<uint, int> pair3 = enumerator2.Current;
                COM_PLAYERCAMP item = pair3.Key;
                KeyValuePair<uint, int> pair4 = enumerator2.Current;
                int num3 = pair4.Value;
                bool flag = false;
                int count = result.Count;
                for (int i = 0; i < count; i++)
                {
                    if (resultScore[i] < num3)
                    {
                        result.Insert(i, item);
                        resultScore.Insert(i, num3);
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(item);
                    resultScore.Add(num3);
                }
            }
            DebugHelper.Assert(resultScore.Count == result.Count);
        }

        public int GetCampScore(COM_PLAYERCAMP camp)
        {
            int num = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_playerKDAStat.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                if (camp == current.Value.PlayerCamp)
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                    num += pair2.Value.numKill;
                }
            }
            return num;
        }

        public DictionaryView<uint, CampInfo> GetCampStat()
        {
            return this.campStat;
        }

        public DictionaryView<uint, Dictionary<int, DestroyStat>> GetDestroyStat()
        {
            return this.destroyStats;
        }

        public uint GetLastBestPlayer()
        {
            return this.m_lastBestPlayer;
        }

        public uint GetLoseMvp()
        {
            return this.m_loseMvpId;
        }

        public uint GetMvpPlayer(COM_PLAYERCAMP camp, bool bWin)
        {
            if ((this.m_winMvpId != 0) || (this.m_loseMvpId != 0))
            {
                if (bWin)
                {
                    return this.m_winMvpId;
                }
                return this.m_loseMvpId;
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            int pvpPlayerNum = 0;
            if ((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide())
            {
                pvpPlayerNum = curLvelContext.m_pvpPlayerNum;
            }
            if (pvpPlayerNum <= 2)
            {
                return 0;
            }
            uint playerId = 0;
            float num3 = 0f;
            int numKill = 0;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetEnumerator();
            PlayerKDA rkda = null;
            float num5 = ((float) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xb1).dwConfValue) / 10000f;
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                rkda = current.Value;
                if ((!rkda.bHangup && !rkda.bRunaway) && !rkda.bDisconnect)
                {
                    float scoreValue = rkda.GetScoreValue();
                    if ((rkda.PlayerCamp == camp) && (scoreValue >= num3))
                    {
                        if (scoreValue == num3)
                        {
                            KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                            if (pair2.Value.numKill < numKill)
                            {
                                continue;
                            }
                        }
                        if (bWin || (scoreValue >= num5))
                        {
                            KeyValuePair<uint, PlayerKDA> pair3 = enumerator.Current;
                            playerId = pair3.Value.PlayerId;
                            num3 = scoreValue;
                            KeyValuePair<uint, PlayerKDA> pair4 = enumerator.Current;
                            numKill = pair4.Value.numKill;
                        }
                    }
                }
            }
            return playerId;
        }

        public uint GetScoreRank(COM_PLAYERCAMP inCampType, RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType)
        {
            List<COM_PLAYERCAMP> result = null;
            List<int> resultScore = null;
            this.GetCampsByScoreRank(inDataSubType, out result, out resultScore);
            HashSet<int> set = new HashSet<int>();
            if ((result != null) && (result.Count > 0))
            {
                int index = result.IndexOf(inCampType);
                if (index >= 0)
                {
                    for (int i = 0; i <= index; i++)
                    {
                        set.Add(resultScore[i]);
                    }
                }
            }
            return (uint) set.Count;
        }

        public uint GetWinMvp()
        {
            return this.m_winMvpId;
        }

        private void initEvent()
        {
            this.unInitEvent();
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
            Singleton<GameEventSys>.instance.AddEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.OnActorInit));
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.AddEventHandler<AddSoulExpEventParam>(GameEventDef.Event_AddExpValue, new RefAction<AddSoulExpEventParam>(this.OnAddExpValue));
        }

        private void initNotifyDestroyStat()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (!curLvelContext.IsMobaMode())
            {
                if ((curLvelContext != null) && (curLvelContext.m_starDetail != null))
                {
                    for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
                    {
                        int iParam = curLvelContext.m_starDetail[i].iParam;
                        if (iParam != 0)
                        {
                            ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint) iParam);
                            this.CondfoToDestroyStat(dataByKey.astConditions);
                        }
                    }
                }
                else
                {
                    DebugHelper.Assert(false, string.Format("LevelConfig is null -- levelID: {0}", curLvelContext.m_mapID));
                }
            }
        }

        private void OnActorDamage(ref HurtEventResultInfo prm)
        {
            DebugHelper.Assert(this.m_NonHeroInfo != null, "invalid m_NonHeroInfo");
            this.OnActorDamageAtker(ref prm);
            if (((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)) && (prm.hurtInfo.hurtType != HurtTypeDef.Therapic))
            {
                DictionaryView<uint, NONHERO_STATISTIC_INFO> view;
                NONHERO_STATISTIC_INFO nonhero_statistic_info;
                if (!this.m_NonHeroInfo.TryGetValue((uint) prm.src.handle.TheActorMeta.ActorType, out view))
                {
                    view = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
                    this.m_NonHeroInfo.Add((uint) prm.src.handle.TheActorMeta.ActorType, view);
                }
                if (!view.TryGetValue((uint) prm.src.handle.TheActorMeta.ActorCamp, out nonhero_statistic_info))
                {
                    nonhero_statistic_info = new NONHERO_STATISTIC_INFO();
                    view.Add((uint) prm.src.handle.TheActorMeta.ActorCamp, nonhero_statistic_info);
                }
                nonhero_statistic_info.uiTotalBeAttackedNum++;
                nonhero_statistic_info.uiTotalBeHurtCount += (uint) prm.hurtTotal;
                nonhero_statistic_info.uiBeHurtMax = (nonhero_statistic_info.uiBeHurtMax <= prm.hurtTotal) ? ((uint) prm.hurtTotal) : nonhero_statistic_info.uiBeHurtMax;
                nonhero_statistic_info.uiBeHurtMin = (nonhero_statistic_info.uiBeHurtMin >= prm.hurtTotal) ? ((uint) prm.hurtTotal) : nonhero_statistic_info.uiBeHurtMin;
                int num = (prm.src.handle.ValueComponent == null) ? 0 : prm.src.handle.ValueComponent.actorHp;
                int num2 = num - prm.hurtTotal;
                if (num2 < 0)
                {
                    num2 = 0;
                }
                nonhero_statistic_info.uiHpMax = (nonhero_statistic_info.uiHpMax <= num) ? ((uint) num) : nonhero_statistic_info.uiHpMax;
                nonhero_statistic_info.uiHpMin = (nonhero_statistic_info.uiHpMin >= num2) ? ((uint) num2) : nonhero_statistic_info.uiHpMin;
                this.m_NonHeroInfo[(uint) prm.src.handle.TheActorMeta.ActorType][(uint) prm.src.handle.TheActorMeta.ActorCamp] = nonhero_statistic_info;
            }
        }

        private void OnActorDamageAtker(ref HurtEventResultInfo prm)
        {
            if (((prm.atker != 0) && (prm.atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)) && (prm.hurtInfo.hurtType != HurtTypeDef.Therapic))
            {
                DictionaryView<uint, NONHERO_STATISTIC_INFO> view;
                NONHERO_STATISTIC_INFO nonhero_statistic_info;
                if (!this.m_NonHeroInfo.TryGetValue((uint) prm.atker.handle.TheActorMeta.ActorType, out view))
                {
                    view = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
                    this.m_NonHeroInfo.Add((uint) prm.atker.handle.TheActorMeta.ActorType, view);
                }
                if (!view.TryGetValue((uint) prm.atker.handle.TheActorMeta.ActorCamp, out nonhero_statistic_info))
                {
                    nonhero_statistic_info = new NONHERO_STATISTIC_INFO();
                    view.Add((uint) prm.atker.handle.TheActorMeta.ActorCamp, nonhero_statistic_info);
                }
                nonhero_statistic_info.uiTotalAttackNum++;
                nonhero_statistic_info.uiTotalHurtCount += (uint) prm.hurtTotal;
                nonhero_statistic_info.uiHurtMax = (nonhero_statistic_info.uiHurtMax <= prm.hurtTotal) ? ((uint) prm.hurtTotal) : nonhero_statistic_info.uiHurtMax;
                nonhero_statistic_info.uiHurtMin = (nonhero_statistic_info.uiHurtMin >= prm.hurtTotal) ? ((uint) prm.hurtTotal) : nonhero_statistic_info.uiHurtMin;
                DebugHelper.Assert(prm.atker.handle.SkillControl != null, "empty skill control");
                if (((prm.atker.handle.SkillControl != null) && (prm.atker.handle.SkillControl.stSkillStat != null)) && (prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo != null))
                {
                    int atkSlot = (int) prm.hurtInfo.atkSlot;
                    int length = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo.Length;
                    if ((atkSlot >= 0) && (atkSlot < length))
                    {
                        SKILLSTATISTICTINFO skillstatistictinfo = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int) prm.hurtInfo.atkSlot];
                        nonhero_statistic_info.uiAttackDistanceMax = (uint) skillstatistictinfo.iAttackDistanceMax;
                        if (((prm.atker.handle.SkillControl.CurUseSkillSlot != null) && (prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj != null)) && (prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj.cfgData != null))
                        {
                            uint iMaxAttackDistance = (uint) prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj.cfgData.iMaxAttackDistance;
                            nonhero_statistic_info.uiAttackDistanceMin = (nonhero_statistic_info.uiAttackDistanceMin >= iMaxAttackDistance) ? iMaxAttackDistance : nonhero_statistic_info.uiAttackDistanceMin;
                        }
                    }
                }
                if (nonhero_statistic_info.uiFirstBeAttackTime == 0)
                {
                    nonhero_statistic_info.uiFirstBeAttackTime = (uint) Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
                this.m_NonHeroInfo[(uint) prm.atker.handle.TheActorMeta.ActorType][(uint) prm.atker.handle.TheActorMeta.ActorCamp] = nonhero_statistic_info;
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (!prm.bImmediateRevive)
            {
                NONHERO_STATISTIC_INFO nonhero_statistic_info;
                DictionaryView<uint, NONHERO_STATISTIC_INFO> view;
                this.doDestroyStat(prm.src, prm.orignalAtker);
                if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    HeroWrapper actorControl = prm.src.handle.ActorControl as HeroWrapper;
                    CampInfo campInfoByCamp = null;
                    PoolObjHandle<ActorRoot> inAtker = new PoolObjHandle<ActorRoot>(null);
                    if ((prm.orignalAtker != 0) && (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        campInfoByCamp = this.GetCampInfoByCamp(prm.orignalAtker.handle.TheActorMeta.ActorCamp);
                        inAtker = prm.orignalAtker;
                    }
                    else if (actorControl.IsKilledByHero() && (actorControl.LastHeroAtker != 0))
                    {
                        campInfoByCamp = this.GetCampInfoByCamp(actorControl.LastHeroAtker.handle.TheActorMeta.ActorCamp);
                        inAtker = actorControl.LastHeroAtker;
                    }
                    if ((campInfoByCamp != null) && (inAtker != 0))
                    {
                        campInfoByCamp.IncCampScore(prm.src, inAtker);
                        uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x6d).dwConfValue;
                        campInfoByCamp.IncHeadPoints((int) dwConfValue, prm.src, inAtker);
                    }
                }
                else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    MonsterWrapper wrapper2 = prm.src.handle.ActorControl as MonsterWrapper;
                    if (wrapper2.IsKilledByHero())
                    {
                        CampInfo info2 = this.GetCampInfoByCamp(wrapper2.LastHeroAtker.handle.TheActorMeta.ActorCamp);
                        DebugHelper.Assert(info2 != null);
                        if (info2 != null)
                        {
                            info2.IncHeadPoints(wrapper2.cfgInfo.iHeadPoints, prm.src, prm.orignalAtker);
                        }
                    }
                    else if ((prm.orignalAtker != 0) && (prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        CampInfo info3 = this.GetCampInfoByCamp(prm.orignalAtker.handle.TheActorMeta.ActorCamp);
                        DebugHelper.Assert(info3 != null);
                        if (info3 != null)
                        {
                            info3.IncHeadPoints(wrapper2.cfgInfo.iHeadPoints, prm.src, prm.orignalAtker);
                        }
                    }
                }
                if (((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)) && (this.m_NonHeroInfo.TryGetValue((uint) prm.src.handle.TheActorMeta.ActorType, out view) && view.TryGetValue((uint) prm.src.handle.TheActorMeta.ActorCamp, out nonhero_statistic_info)))
                {
                    nonhero_statistic_info.uiTotalDeadNum++;
                }
                if (((prm.atker != 0) && (prm.src != 0)) && (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
                {
                    OrganWrapper wrapper3 = prm.src.handle.ActorControl as OrganWrapper;
                    if ((wrapper3 != null) && (wrapper3.cfgInfo.bOrganType == 1))
                    {
                        CampInfo info4 = this.GetCampInfoByCamp(prm.atker.handle.TheActorMeta.ActorCamp);
                        if (info4 != null)
                        {
                            info4.destoryTowers++;
                            Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_TOWER_DESTROY_CHANGED);
                        }
                    }
                }
            }
        }

        private void OnActorInit(ref PoolObjHandle<ActorRoot> inActor)
        {
            if (inActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                DictionaryView<uint, NONHERO_STATISTIC_INFO> view;
                NONHERO_STATISTIC_INFO nonhero_statistic_info;
                if (!this.m_NonHeroInfo.TryGetValue((uint) inActor.handle.TheActorMeta.ActorType, out view))
                {
                    view = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
                    this.m_NonHeroInfo.Add((uint) inActor.handle.TheActorMeta.ActorType, view);
                }
                if (!view.TryGetValue((uint) inActor.handle.TheActorMeta.ActorCamp, out nonhero_statistic_info))
                {
                    nonhero_statistic_info = new NONHERO_STATISTIC_INFO();
                    view.Add((uint) inActor.handle.TheActorMeta.ActorCamp, nonhero_statistic_info);
                }
                nonhero_statistic_info.ActorType = inActor.handle.TheActorMeta.ActorType;
                nonhero_statistic_info.ActorCamp = inActor.handle.TheActorMeta.ActorCamp;
                nonhero_statistic_info.uiTotalSpawnNum++;
                this.m_NonHeroInfo[(uint) inActor.handle.TheActorMeta.ActorType][(uint) inActor.handle.TheActorMeta.ActorCamp] = nonhero_statistic_info;
            }
        }

        private void OnAddExpValue(ref AddSoulExpEventParam prm)
        {
            this.m_stSoulStatisticInfo.iAddExpTotal += prm.iAddExpValue;
            if (prm.src != 0)
            {
                if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (prm.src.handle.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
                {
                    this.m_stSoulStatisticInfo.iKillSoldierExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillSoldierExpMax, prm.iAddExpValue);
                }
                else if ((prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) && (prm.src.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
                {
                    this.m_stSoulStatisticInfo.iKillMonsterExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillMonsterExpMax, prm.iAddExpValue);
                }
                else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    this.m_stSoulStatisticInfo.iKillHeroExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillHeroExpMax, prm.iAddExpValue);
                }
                else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    this.m_stSoulStatisticInfo.iKillOrganExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillOrganExpMax, prm.iAddExpValue);
                }
            }
            if ((prm.atker != 0) && (prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                ValueProperty valueComponent = prm.atker.handle.ValueComponent;
                if ((valueComponent != null) && (valueComponent.ObjValueStatistic != null))
                {
                    uint num2 = (uint) (Singleton<FrameSynchr>.instance.LogicFrameTick - valueComponent.ObjValueStatistic.ulLastAddSoulExpTime);
                    valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax = (valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax <= num2) ? num2 : valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax;
                }
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                for (int i = 0; i < heroActors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = heroActors[i];
                    if (handle.handle.TheActorMeta.ActorCamp == prm.atker.handle.TheActorMeta.ActorCamp)
                    {
                        PoolObjHandle<ActorRoot> handle2 = heroActors[i];
                        if (handle2.handle.ValueComponent != null)
                        {
                            PoolObjHandle<ActorRoot> handle3 = heroActors[i];
                            if (handle3.handle.ValueComponent.ObjValueStatistic != null)
                            {
                                PoolObjHandle<ActorRoot> handle4 = heroActors[i];
                                handle4.handle.ValueComponent.ObjValueStatistic.uiTeamSoulExpTotal += (uint) prm.iAddExpValue;
                            }
                        }
                    }
                }
            }
        }

        public void onFightOver(ref DefaultGameEventParam prm)
        {
            this.iBattleResult = Singleton<BattleLogic>.instance.JudgeBattleResult(prm.src, prm.atker);
        }

        public void PostEndGame()
        {
            this.unInitEvent();
            this.m_battleBuffStat.RemoveTimerEvent();
            this.m_locStat.Clear();
            this.m_vdStat.Clear();
        }

        public void RecordMvp(COMDT_GAME_INFO gameInfo)
        {
            this.m_winMvpId = gameInfo.dwWinMvpObjID;
            this.m_loseMvpId = gameInfo.dwLoseMvpObjID;
        }

        public void StartStatistic()
        {
            this.campStat.Clear();
            this.destroyStats.Clear();
            CPlayerBehaviorStat.Clear();
            this.destroyStats.Add(0, new Dictionary<int, DestroyStat>());
            this.destroyStats.Add(1, new Dictionary<int, DestroyStat>());
            this.destroyStats.Add(2, new Dictionary<int, DestroyStat>());
            this.m_playerKDAStat.StartKDARecord();
            this.m_battleDeadStat.StartRecord();
            this.m_playerSoulLevelStat.StartRecord();
            this.m_shenFuStat.StartRecord();
            this.m_battleBuffStat.StartRecord();
            this.m_optAchievementRecorder.StartRecord();
            this.m_locStat.StartRecord();
            this.m_vdStat.StartRecord();
            this.m_NonHeroInfo.Clear();
            this.campStat.Add(0, new CampInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_MID));
            this.campStat.Add(1, new CampInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_1));
            this.campStat.Add(2, new CampInfo(COM_PLAYERCAMP.COM_PLAYERCAMP_2));
            this.initEvent();
            this.initNotifyDestroyStat();
            this.m_winMvpId = 0;
            this.m_loseMvpId = 0;
            this.m_lastBestPlayer = 0;
            this.m_heroSkillStat.StartRecord();
        }

        public void unInitEvent()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
            Singleton<GameEventSys>.instance.RmvEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.OnActorInit));
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.RmvEventHandler<AddSoulExpEventParam>(GameEventDef.Event_AddExpValue, new RefAction<AddSoulExpEventParam>(this.OnAddExpValue));
        }

        public void UpdateLogic(int DeltaTime)
        {
            if (this.m_locStat != null)
            {
                this.m_locStat.UpdateLogic(DeltaTime);
            }
        }

        public int iBattleResult
        {
            get
            {
                return this.m_iBattleResult;
            }
            set
            {
                this.m_iBattleResult = value;
            }
        }
    }
}

