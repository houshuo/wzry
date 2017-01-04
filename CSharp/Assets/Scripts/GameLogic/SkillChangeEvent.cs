namespace Assets.Scripts.GameLogic
{
    using System;

    public class SkillChangeEvent
    {
        private bool bAbortChange;
        private bool bActive;
        private bool bOvertimeCD;
        private bool bSendEvent;
        private bool bUseStop;
        private int changeSkillID;
        private SkillSlot curSkillSlot;
        private int effectMaxTime;
        private int effectTime;
        private int recoverSkillID;

        public SkillChangeEvent(SkillSlot slot)
        {
            this.curSkillSlot = slot;
        }

        public void Abort()
        {
            if (this.bAbortChange)
            {
                this.Leave();
            }
        }

        private void Enter()
        {
            Skill skill = new Skill(this.changeSkillID);
            if (this.curSkillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
            {
                this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
                this.curSkillSlot.skillIndicator.CreateIndicatePrefab(skill);
            }
            this.curSkillSlot.NextSkillObj = skill;
            this.bActive = true;
            ChangeSkillEventParam param = new ChangeSkillEventParam(this.curSkillSlot.SlotType, this.changeSkillID, this.effectTime);
            if (this.bSendEvent)
            {
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, this.curSkillSlot.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public int GetEffectMaxTime()
        {
            return this.effectMaxTime;
        }

        public int GetEffectTIme()
        {
            return this.effectTime;
        }

        public bool IsActive()
        {
            return this.bActive;
        }

        public bool IsDisplayUI()
        {
            return this.bSendEvent;
        }

        public void Leave()
        {
            this.bActive = false;
            if (this.recoverSkillID != 0)
            {
                Skill skill = new Skill(this.recoverSkillID);
                if (this.curSkillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
                {
                    this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
                    this.curSkillSlot.skillIndicator.CreateIndicatePrefab(skill);
                }
                this.curSkillSlot.NextSkillObj = skill;
                if (this.bOvertimeCD)
                {
                    this.curSkillSlot.StartSkillCD();
                }
                if (this.bSendEvent)
                {
                    DefaultSkillEventParam param = new DefaultSkillEventParam(this.curSkillSlot.SlotType, this.recoverSkillID, this.curSkillSlot.Actor);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, this.curSkillSlot.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
            }
        }

        public void Reset()
        {
            this.bActive = false;
            this.bAbortChange = false;
            if (this.recoverSkillID != 0)
            {
                this.curSkillSlot.NextSkillObj = null;
                this.curSkillSlot.SkillObj = this.curSkillSlot.InitSkillObj;
                this.curSkillSlot.skillIndicator.UnInitIndicatePrefab(true);
                this.curSkillSlot.skillIndicator.CreateIndicatePrefab(this.curSkillSlot.SkillObj);
            }
        }

        public void Start(int _time, int _changeId, int _recoverId, bool _bOvertimeCD, bool _bSendEvent, bool _bAbort, bool _bUseStop)
        {
            this.effectTime = _time;
            this.effectMaxTime = _time;
            this.changeSkillID = _changeId;
            this.recoverSkillID = _recoverId;
            this.bOvertimeCD = _bOvertimeCD;
            this.bAbortChange = _bAbort;
            this.bSendEvent = _bSendEvent;
            this.bUseStop = _bUseStop;
            this.Enter();
        }

        public void Stop()
        {
            if (this.bUseStop)
            {
                this.bActive = false;
            }
        }

        public void UpdateSkillCD(int nDelta)
        {
            if (this.bActive)
            {
                this.effectTime -= nDelta;
                if (this.effectTime < 0)
                {
                    this.Leave();
                }
            }
        }
    }
}

