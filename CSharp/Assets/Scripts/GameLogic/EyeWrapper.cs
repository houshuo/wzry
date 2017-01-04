namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class EyeWrapper : ObjWrapper
    {
        private int lifeTime;

        public override void Born(ActorRoot owner)
        {
            base.actor = owner;
            base.actorPtr = new PoolObjHandle<ActorRoot>(base.actor);
            base.m_isNeedToHelpOther = false;
            base.m_curWaypointsHolder = null;
            base.m_curWaypointTarget = null;
            base.m_isCurWaypointEndPoint = false;
            base.m_isStartPoint = false;
            base.m_isControledByMan = true;
            base.m_isAutoAI = false;
            base.m_offline = false;
            base.m_followOther = false;
            base.m_leaderID = 0;
            base.m_isAttackedByEnemyHero = false;
            base.m_isAttacked = false;
            base.bForceNotRevive = false;
            base.actor.SkillControl = base.actor.CreateLogicComponent<SkillComponent>(base.actor);
            base.actor.ValueComponent = base.actor.CreateLogicComponent<ValueProperty>(base.actor);
            base.actor.HurtControl = base.actor.CreateLogicComponent<HurtComponent>(base.actor);
            base.actor.BuffHolderComp = base.actor.CreateLogicComponent<BuffHolderComponent>(base.actor);
            base.actor.AnimControl = base.actor.CreateLogicComponent<AnimPlayComponent>(base.actor);
            base.actor.HudControl = base.actor.CreateLogicComponent<HudComponent3D>(base.actor);
            base.actor.HorizonMarker = base.actor.CreateLogicComponent<HorizonMarker>(base.actor);
            base.actor.MatHurtEffect = base.actor.CreateActorComponent<MaterialHurtEffect>(base.actor);
        }

        public override void Deactive()
        {
            base.Deactive();
        }

        public override void Fight()
        {
            base.Fight();
        }

        public override void FightOver()
        {
            base.FightOver();
        }

        public override string GetTypeName()
        {
            return "EyeWrapper";
        }

        public override void Init()
        {
            base.Init();
        }

        public override void LogActorState()
        {
        }

        protected override void OnDead()
        {
            base.OnDead();
            if (true)
            {
                Singleton<GameObjMgr>.instance.RecycleActor(base.actorPtr, 0);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.lifeTime = 0;
        }

        public override void Prepare()
        {
            base.Prepare();
        }

        public override void Reactive()
        {
            base.Reactive();
            this.LifeTime = 0;
        }

        public override void Uninit()
        {
            base.Uninit();
        }

        public override void UpdateLogic(int delta)
        {
            base.updateAffectActors();
            if (this.lifeTime > 0)
            {
                this.lifeTime -= delta;
                if (this.lifeTime <= 0)
                {
                    base.SetObjBehaviMode(ObjBehaviMode.State_Dead);
                }
            }
        }

        public int LifeTime
        {
            get
            {
                return this.lifeTime;
            }
            set
            {
                this.lifeTime = value;
            }
        }
    }
}

