namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [Serializable]
    public class TriggerActionWrapper
    {
        [FriendlyName("起效时间")]
        public int ActiveTime;
        [FriendlyName("作用于事件肇事者")]
        public bool bAtker;
        [FriendlyName("打开")]
        public bool bEnable;
        [FriendlyName("作用于事件受害者")]
        public bool bSrc;
        [FriendlyName("离开时拔除")]
        public bool bStopWhenLeaving;
        [FriendlyName("进入时配置ID")]
        public int EnterUniqueId;
        [FriendlyName("离开时配置ID")]
        public int LeaveUniqueId;
        private TriggerActionBase m_internalAct;
        [FriendlyName("偏移距离_x")]
        public int Offset_x;
        [FriendlyName("偏移距离_y")]
        public int Offset_y;
        public GameObject[] RefObjList;
        [SerializeField]
        public AreaEventTrigger.STimingAction[] TimingActionsInter;
        [FriendlyName("持续时间")]
        public int TotalTime;
        public EGlobalTriggerAct TriggerType;
        [FriendlyName("轮询探测时配置ID")]
        public int UpdateUniqueId;

        public TriggerActionWrapper()
        {
            this.RefObjList = new GameObject[0];
            this.TimingActionsInter = new AreaEventTrigger.STimingAction[0];
            this.bEnable = true;
            this.bEnable = true;
        }

        public TriggerActionWrapper(EGlobalTriggerAct inTriggerType)
        {
            this.RefObjList = new GameObject[0];
            this.TimingActionsInter = new AreaEventTrigger.STimingAction[0];
            this.bEnable = true;
            this.TriggerType = inTriggerType;
            this.bEnable = true;
        }

        public void Destroy()
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.Destroy();
                this.m_internalAct = null;
            }
        }

        public TriggerActionBase GetActionInternal()
        {
            return this.m_internalAct;
        }

        public void Init()
        {
            if (this.m_internalAct == null)
            {
                switch (this.TriggerType)
                {
                    case EGlobalTriggerAct.Activate:
                        this.m_internalAct = new TriggerActionActivator(this);
                        this.m_internalAct.bEnable = true;
                        break;

                    case EGlobalTriggerAct.Deactivate:
                        this.m_internalAct = new TriggerActionActivator(this);
                        this.m_internalAct.bEnable = false;
                        break;

                    case EGlobalTriggerAct.TriggerBuff:
                        this.m_internalAct = new TriggerActionBuff(this);
                        break;

                    case EGlobalTriggerAct.TriggerDialogue:
                        this.m_internalAct = new TriggerActionDialogue(this);
                        break;

                    case EGlobalTriggerAct.TriggerSpawn:
                        this.m_internalAct = new TriggerActionSpawn(this);
                        break;

                    case EGlobalTriggerAct.TriggerGuideTip:
                        this.m_internalAct = new TriggerActionGuideTip(this);
                        break;

                    case EGlobalTriggerAct.TriggerDynamicBlock:
                        this.m_internalAct = new TriggerActionBlockSwitcher(this);
                        break;

                    case EGlobalTriggerAct.TriggerAge:
                        this.m_internalAct = new TriggerActionAge(this);
                        break;

                    case EGlobalTriggerAct.TriggerJungle:
                        this.m_internalAct = new TriggerActionJungle(this);
                        break;

                    case EGlobalTriggerAct.TriggerBubbleText:
                        this.m_internalAct = new TriggerActionTextBubble(this);
                        break;

                    case EGlobalTriggerAct.TriggerSkillHud:
                        this.m_internalAct = new TriggerActionSkillHud(this);
                        break;

                    case EGlobalTriggerAct.TriggerBattleUi:
                        this.m_internalAct = new TriggerActionShowToggleAuto(this);
                        break;

                    case EGlobalTriggerAct.TriggerNewbieForm:
                        this.m_internalAct = new TriggerActionNewbieForm(this);
                        break;

                    case EGlobalTriggerAct.TriggerSoldierLine:
                        this.m_internalAct = new TriggerActionSoldierLine(this);
                        break;

                    case EGlobalTriggerAct.TriggerPauseGame:
                        this.m_internalAct = new TriggerActionPauseGame(this);
                        break;

                    case EGlobalTriggerAct.TriggerShenFu:
                        this.m_internalAct = new TriggerActionShenFu(this);
                        break;

                    case EGlobalTriggerAct.TriggerBattleEquipLimit:
                        this.m_internalAct = new TriggerActionBattleEquipLimit(this);
                        break;

                    case EGlobalTriggerAct.TriggerSetGlobalVariable:
                        this.m_internalAct = new TriggerActionSetGlobalVariable(this);
                        break;

                    default:
                        DebugHelper.Assert(false);
                        break;
                }
            }
        }

        public virtual void OnCoolDown(ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.OnCoolDown(inTrigger);
            }
        }

        public virtual void OnTriggerStart(ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.OnTriggerStart(inTrigger);
            }
        }

        public RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if (this.m_internalAct == null)
            {
                return null;
            }
            return this.m_internalAct.TriggerEnter(src, atker, inTrigger, prm);
        }

        public void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.TriggerLeave(src, inTrigger);
            }
        }

        public void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
            if (this.m_internalAct != null)
            {
                this.m_internalAct.TriggerUpdate(src, atker, inTrigger);
            }
        }
    }
}

