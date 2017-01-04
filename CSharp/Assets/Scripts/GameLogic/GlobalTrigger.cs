namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/Global Trigger")]
    public class GlobalTrigger : MonoBehaviour, ITrigger
    {
        [SerializeField]
        public TriggerActionWrapper[] ActionList = new TriggerActionWrapper[0];
        public int CurGlobalVariable;
        public GameObject[] DeactiveObjList = new GameObject[0];
        private DictionaryView<int, CDelayMatch> DelayTimeSeqMap = new DictionaryView<int, CDelayMatch>();
        private MultiValueListDictionary<uint, TriggerActionBase> TriggerActionMultiMap = new MultiValueListDictionary<uint, TriggerActionBase>();
        public CTriggerMatch[] TriggerMatches = new CTriggerMatch[0];

        private void Awake()
        {
            if (Singleton<GameEventSys>.instance != null)
            {
                Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
                Singleton<GameEventSys>.instance.AddEventHandler<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, new RefAction<SGroupDeadEventParam>(this.onSpawnGroupDone));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onFightPrepare));
                Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
                Singleton<GameEventSys>.instance.AddEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.onActorInit));
                Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.onEnterCombat));
                Singleton<GameEventSys>.instance.AddEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
            }
            if (Singleton<GameSkillEventSys>.instance != null)
            {
                Singleton<GameSkillEventSys>.instance.AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
            }
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorBattleCoinChanged));
            foreach (TriggerActionWrapper wrapper in this.ActionList)
            {
                if (wrapper != null)
                {
                    EGlobalTriggerAct triggerType = wrapper.TriggerType;
                    wrapper.Init();
                    TriggerActionBase actionInternal = wrapper.GetActionInternal();
                    DebugHelper.Assert(actionInternal != null);
                    this.TriggerActionMultiMap.Add((uint) triggerType, actionInternal);
                }
            }
        }

        public void BindSkillCancelListener()
        {
            Singleton<GameSkillEventSys>.instance.AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
        }

        private void ClearDelayTimers()
        {
            DictionaryView<int, CDelayMatch>.Enumerator enumerator = this.DelayTimeSeqMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, CDelayMatch> current = enumerator.Current;
                int key = current.Key;
                Singleton<CTimerManager>.instance.RemoveTimer(key);
            }
            this.DelayTimeSeqMap.Clear();
        }

        public void CloseTalentTip()
        {
            int length = this.TriggerMatches.Length;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.CloseTalentTip, match, src, atker, null, i))
                    {
                        this.DoTriggering(match, src, atker, null);
                    }
                }
            }
        }

        private void DoTriggering(CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, object param)
        {
            if (((match.TriggerCountMax <= 0) || (++match.m_triggeredCounter <= match.TriggerCountMax)) && !match.bCoolingDown)
            {
                match.IntoCoolingDown();
                if (match.DelayTime > 0)
                {
                    int key = Singleton<CTimerManager>.instance.AddTimer(match.DelayTime, 1, new CTimer.OnTimeUpHandler(this.OnDelayTriggerTimer), true);
                    if (key >= 0)
                    {
                        DebugHelper.Assert(!this.DelayTimeSeqMap.ContainsKey(key));
                        CDelayMatch match2 = new CDelayMatch {
                            AtkerHandle = atker,
                            SrcHandle = src,
                            ParamObj = param,
                            TriggerMatch = match
                        };
                        this.DelayTimeSeqMap.Add(key, match2);
                    }
                }
                else
                {
                    this.DoTriggeringImpl(match, src, atker, param);
                }
            }
        }

        private void DoTriggeringImpl(CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, object param)
        {
            ListView<TriggerActionBase> values = this.TriggerActionMultiMap.GetValues((uint) match.ActType, true);
            for (int i = 0; i < values.Count; i++)
            {
                TriggerActionBase base2 = values[i];
                if (base2 != null)
                {
                    base2.AppendRefObj(match.Listeners);
                    base2.TriggerEnter(src, atker, this, param);
                }
            }
            if ((match.ActionList != null) && (match.ActionList.Length > 0))
            {
                int length = match.ActionList.Length;
                for (int j = 0; j < length; j++)
                {
                    TriggerActionWrapper wrapper = match.ActionList[j];
                    if (wrapper != null)
                    {
                        TriggerActionBase actionInternal = wrapper.GetActionInternal();
                        if (actionInternal == null)
                        {
                            wrapper.Init();
                            actionInternal = wrapper.GetActionInternal();
                            DebugHelper.Assert(actionInternal != null);
                        }
                        actionInternal.AppendRefObj(match.Listeners);
                        actionInternal.TriggerEnter(src, atker, this, param);
                    }
                }
            }
        }

        private bool FilterMatch(EGlobalGameEvent inEventType, CTriggerMatch match, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, object param, int inMatchIndex)
        {
            if (match.EventType != inEventType)
            {
                return false;
            }
            if (!match.Condition.FilterMatch(inEventType, src, atker, param, match, inMatchIndex))
            {
                return false;
            }
            return true;
        }

        public GameObject GetTriggerObj()
        {
            return base.gameObject;
        }

        private void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            int length = this.TriggerMatches.Length;
            object param = currentValue;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.BattleGoldChange, match, actor, atker, param, i))
                    {
                        this.DoTriggering(match, actor, atker, param);
                    }
                }
            }
        }

        private void onActorDamage(ref HurtEventResultInfo prm)
        {
            int length = this.TriggerMatches.Length;
            object param = (HurtEventResultInfo) prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if ((match != null) && this.FilterMatch(EGlobalGameEvent.ActorDamage, match, prm.src, prm.atker, param, i))
                {
                    this.DoTriggering(match, prm.src, prm.atker, param);
                }
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            int length = this.TriggerMatches.Length;
            GameDeadEventParam param = prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if ((match != null) && this.FilterMatch(EGlobalGameEvent.ActorDead, match, prm.src, prm.orignalAtker, param, i))
                {
                    this.DoTriggering(match, prm.src, prm.orignalAtker, param);
                }
            }
        }

        private void onActorInit(ref PoolObjHandle<ActorRoot> inActor)
        {
            int length = this.TriggerMatches.Length;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.ActorInit, match, inActor, atker, null, i))
                    {
                        this.DoTriggering(match, inActor, atker, null);
                    }
                }
            }
        }

        private void OnDelayTriggerTimer(int inTimeSeq)
        {
            if (this.DelayTimeSeqMap.ContainsKey(inTimeSeq))
            {
                CDelayMatch match = this.DelayTimeSeqMap[inTimeSeq];
                if (match != null)
                {
                    this.DoTriggeringImpl(match.TriggerMatch, match.SrcHandle, match.AtkerHandle, match.ParamObj);
                }
                this.DelayTimeSeqMap.Remove(inTimeSeq);
            }
            Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
        }

        private void OnDestroy()
        {
            if (Singleton<GameEventSys>.instance != null)
            {
                Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
                Singleton<GameEventSys>.instance.RmvEventHandler<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, new RefAction<SGroupDeadEventParam>(this.onSpawnGroupDone));
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onFightPrepare));
                Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onFightStart));
                Singleton<GameEventSys>.instance.RmvEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.onActorInit));
                Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.onEnterCombat));
                Singleton<GameEventSys>.instance.RmvEventHandler<TalentLevelChangeParam>(GameEventDef.Event_TalentLevelChange, new RefAction<TalentLevelChangeParam>(this.onTalentLevelChange));
            }
            if (Singleton<GameSkillEventSys>.instance != null)
            {
                Singleton<GameSkillEventSys>.instance.RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
                Singleton<GameSkillEventSys>.instance.RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
            }
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorBattleCoinChanged));
            this.ClearDelayTimers();
            foreach (TriggerActionWrapper wrapper in this.ActionList)
            {
                if (wrapper != null)
                {
                    wrapper.Destroy();
                }
            }
            foreach (CTriggerMatch match in this.TriggerMatches)
            {
                if (match != null)
                {
                    foreach (TriggerActionWrapper wrapper2 in match.ActionList)
                    {
                        if (wrapper2 != null)
                        {
                            wrapper2.Destroy();
                        }
                    }
                }
            }
        }

        private void onEnterCombat(ref DefaultGameEventParam prm)
        {
            int length = this.TriggerMatches.Length;
            object param = (DefaultGameEventParam) prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if ((match != null) && this.FilterMatch(EGlobalGameEvent.EnterCombat, match, prm.src, prm.atker, param, i))
                {
                    this.DoTriggering(match, prm.src, prm.atker, param);
                }
            }
        }

        private void onFightPrepare(ref DefaultGameEventParam prm)
        {
            object param = (DefaultGameEventParam) prm;
            int length = this.TriggerMatches.Length;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if ((match != null) && this.FilterMatch(EGlobalGameEvent.FightPrepare, match, prm.src, prm.atker, param, i))
                {
                    this.DoTriggering(match, prm.src, prm.atker, param);
                }
            }
        }

        private void onFightStart(ref DefaultGameEventParam prm)
        {
            int length = this.TriggerMatches.Length;
            object param = (DefaultGameEventParam) prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if ((match != null) && this.FilterMatch(EGlobalGameEvent.FightStart, match, prm.src, prm.atker, param, i))
                {
                    this.DoTriggering(match, prm.src, prm.atker, param);
                }
            }
        }

        private void onSpawnGroupDone(ref SGroupDeadEventParam inParam)
        {
            int length = this.TriggerMatches.Length;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    object param = null;
                    if (inParam.csg != null)
                    {
                        param = inParam.csg;
                    }
                    else if (inParam.sg != null)
                    {
                        param = inParam.sg;
                    }
                    if (this.FilterMatch(EGlobalGameEvent.SpawnGroupDead, match, src, atker, param, i))
                    {
                        this.DoTriggering(match, src, atker, param);
                    }
                }
            }
        }

        private void onTalentLevelChange(ref TalentLevelChangeParam prm)
        {
            int length = this.TriggerMatches.Length;
            object param = (TalentLevelChangeParam) prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.TalentLevelChange, match, prm.src, atker, param, i))
                    {
                        this.DoTriggering(match, prm.src, atker, param);
                    }
                }
            }
        }

        private void onUseSkill(ref ActorSkillEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                int length = this.TriggerMatches.Length;
                object param = (ActorSkillEventParam) prm;
                for (int i = 0; i < length; i++)
                {
                    CTriggerMatch match = this.TriggerMatches[i];
                    if (match != null)
                    {
                        PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
                        PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                        if (this.FilterMatch(EGlobalGameEvent.UseSkill, match, src, atker, param, i))
                        {
                            this.DoTriggering(match, src, atker, param);
                        }
                    }
                }
            }
        }

        private void onUseSkillCanceled(ref DefaultSkillEventParam prm)
        {
            int length = this.TriggerMatches.Length;
            object param = (DefaultSkillEventParam) prm;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.SkillUseCanceled, match, src, atker, param, i))
                    {
                        this.DoTriggering(match, src, atker, param);
                    }
                }
            }
        }

        public void OpenTalentTip()
        {
            int length = this.TriggerMatches.Length;
            for (int i = 0; i < length; i++)
            {
                CTriggerMatch match = this.TriggerMatches[i];
                if (match != null)
                {
                    PoolObjHandle<ActorRoot> src = new PoolObjHandle<ActorRoot>(null);
                    PoolObjHandle<ActorRoot> atker = new PoolObjHandle<ActorRoot>(null);
                    if (this.FilterMatch(EGlobalGameEvent.OpenTalentTip, match, src, atker, null, i))
                    {
                        this.DoTriggering(match, src, atker, null);
                    }
                }
            }
        }

        public void PrepareFight()
        {
            foreach (GameObject obj2 in this.DeactiveObjList)
            {
                if (obj2 != null)
                {
                    obj2.SetActive(false);
                }
            }
        }

        public void UnbindSkillCancelListener()
        {
            Singleton<GameSkillEventSys>.instance.RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, new GameSkillEvent<DefaultSkillEventParam>(this.onUseSkillCanceled));
        }

        public void UpdateLogic(int inDelta)
        {
            foreach (CTriggerMatch match in this.TriggerMatches)
            {
                if ((match != null) && match.bCoolingDown)
                {
                    match.m_cooldownTimer -= inDelta;
                    if (match.m_cooldownTimer <= 0)
                    {
                        match.m_cooldownTimer = 0;
                    }
                }
            }
        }

        private class CDelayMatch
        {
            public PoolObjHandle<ActorRoot> AtkerHandle;
            public object ParamObj;
            public PoolObjHandle<ActorRoot> SrcHandle;
            public CTriggerMatch TriggerMatch;
        }
    }
}

