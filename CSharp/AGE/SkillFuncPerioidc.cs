namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/SkillFunc")]
    public class SkillFuncPerioidc : SkillFuncDuration
    {
        private int intervalTimer;
        private int lastTime;
        public int PeriodicInterval = 0x3e8;

        public override BaseEvent Clone()
        {
            SkillFuncPerioidc perioidc = ClassObjPool<SkillFuncPerioidc>.Get();
            perioidc.CopyData(this);
            return perioidc;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillFuncPerioidc perioidc = src as SkillFuncPerioidc;
            this.PeriodicInterval = perioidc.PeriodicInterval;
            this.intervalTimer = perioidc.intervalTimer;
            this.lastTime = perioidc.lastTime;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            if (base.bInit)
            {
                this.PeriodicInterval = (int) this.m_context.inSkillFunc.dwSkillFuncFreq;
                if (this.PeriodicInterval > 0)
                {
                    this.lastTime = 0;
                    this.intervalTimer = 0;
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.PeriodicInterval = 0x3e8;
            this.intervalTimer = 0;
            this.lastTime = 0;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if ((this.PeriodicInterval > 0) && base.bInit)
            {
                int num = _localTime - this.lastTime;
                this.lastTime = _localTime;
                this.intervalTimer += num;
                if (this.intervalTimer >= this.PeriodicInterval)
                {
                    this.intervalTimer = 0;
                    base.DoSkillFuncShared(ESkillFuncStage.Update);
                    bool flag = this.Check(_action, _track);
                    _action.SetCondition(_track, flag);
                }
            }
        }
    }
}

