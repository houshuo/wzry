namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    public class PassiveCondition
    {
        protected int[] localParams = new int[6];
        protected PassiveEvent rootEvent;
        protected PoolObjHandle<ActorRoot> sourceActor;

        protected bool CheckTargetSubType(PoolObjHandle<ActorRoot> _actor, int typeMask, int typeSubMask)
        {
            if (typeMask == 0)
            {
                return true;
            }
            if (_actor != 0)
            {
                int actorType = (int) _actor.handle.TheActorMeta.ActorType;
                if ((typeMask & (((int) 1) << actorType)) > 0)
                {
                    if (actorType != 1)
                    {
                        return true;
                    }
                    if (typeSubMask == 0)
                    {
                        return true;
                    }
                    if (_actor.handle.ActorControl.GetActorSubType() == typeSubMask)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool Fit()
        {
            return false;
        }

        public virtual void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.rootEvent = _event;
            this.sourceActor = _source;
            this.SetConditionParam(ref _config);
        }

        public virtual void Reset()
        {
        }

        private void SetConditionParam(ref ResDT_SkillPassiveCondition _config)
        {
            for (int i = 0; i < 6; i++)
            {
                this.localParams[i] = _config.astConditionParam[i].iParam;
            }
        }

        public virtual void UnInit()
        {
        }
    }
}

