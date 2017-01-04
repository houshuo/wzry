namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    [EventCategory("MMGame/SkillFunc")]
    public class SkillFuncDuration : DurationCondition
    {
        protected bool bInit;
        private bool m_bSucceeded;
        protected SSkillFuncContext m_context = new SSkillFuncContext();
        public RES_SKILLFUNC_TYPE SkillFuncType;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.m_bSucceeded;
        }

        public override BaseEvent Clone()
        {
            SkillFuncDuration duration = ClassObjPool<SkillFuncDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillFuncDuration duration = src as SkillFuncDuration;
            this.SkillFuncType = duration.SkillFuncType;
            this.m_context = duration.m_context;
            this.m_bSucceeded = duration.m_bSucceeded;
            this.bInit = duration.bInit;
        }

        private void DeinitContext()
        {
        }

        protected bool DoSkillFuncShared(ESkillFuncStage inStage)
        {
            if ((!this.bInit || !Singleton<BattleLogic>.GetInstance().isRuning) || Singleton<BattleLogic>.GetInstance().isGameOver)
            {
                return false;
            }
            this.m_context.inStage = inStage;
            this.m_context.inDoCount++;
            this.m_bSucceeded = Singleton<SkillFuncDelegator>.GetInstance().DoSkillFunc((int) this.SkillFuncType, ref this.m_context);
            return this.m_bSucceeded;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.InitContext(_action);
            this.DoSkillFuncShared(ESkillFuncStage.Enter);
            base.Enter(_action, _track);
        }

        private void InitContext(AGE.Action _action)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(1);
            PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(0);
            if ((actorHandle != 0) && (handle2 != 0))
            {
                SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                BuffSkill skill = _action.refParams.GetRefParamObject<BuffSkill>("SkillObj");
                ResDT_SkillFunc outSkillFunc = null;
                if ((skill != null) && skill.FindSkillFunc((int) this.SkillFuncType, out outSkillFunc))
                {
                    this.m_context.inTargetObj = actorHandle;
                    this.m_context.inOriginator = handle2;
                    this.m_context.inUseContext = refParamObject;
                    this.m_context.inSkillFunc = outSkillFunc;
                    this.m_context.LocalParams = new SSkillFuncIntParam[6];
                    for (int i = 0; i < 6; i++)
                    {
                        this.m_context.LocalParams[i] = new SSkillFuncIntParam();
                        this.m_context.LocalParams[i].iParam = 0;
                    }
                    this.m_context.inAction = new PoolObjHandle<AGE.Action>(_action);
                    this.m_context.inBuffSkill = new PoolObjHandle<BuffSkill>(skill);
                    this.m_context.inDoCount = 0;
                    this.m_context.inOverlayCount = skill.GetOverlayCount();
                    this.m_context.inLastEffect = true;
                    if (refParamObject != null)
                    {
                        this.m_context.inEffectCount = refParamObject.EffectCount;
                        this.m_context.inMarkCount = refParamObject.MarkCount;
                    }
                    this.m_context.InitSkillFuncContext();
                    skill.SetBuffLevel(this.m_context.iSkillLevel);
                    this.bInit = true;
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            this.DoSkillFuncShared(ESkillFuncStage.Leave);
            this.DeinitContext();
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.SkillFuncType = RES_SKILLFUNC_TYPE.RES_SKILLFUNC_TYPE_PHYSHURT;
            this.m_context = new SSkillFuncContext();
            this.m_bSucceeded = false;
            this.bInit = false;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

