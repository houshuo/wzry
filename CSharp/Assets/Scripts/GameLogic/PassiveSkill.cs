namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class PassiveSkill : BaseSkill
    {
        public ResSkillPassiveCfgInfo cfgData;
        public PassiveEvent passiveEvent;
        public SkillSlotType SlotType;
        private PoolObjHandle<ActorRoot> sourceActor;

        public PassiveSkill(int id, PoolObjHandle<ActorRoot> root)
        {
            this.sourceActor = root;
            base.SkillID = id;
            this.cfgData = GameDataMgr.skillPassiveDatabin.GetDataByKey((long) id);
            if (this.cfgData != null)
            {
                this.Init();
            }
        }

        public void ChangeEventParam(int index, int value)
        {
            if (this.passiveEvent != null)
            {
                this.passiveEvent.ChangeEventParam(index, value);
            }
        }

        public int GetCDTime()
        {
            if (this.passiveEvent != null)
            {
                return this.passiveEvent.GetCDTime();
            }
            return 0;
        }

        public void Init()
        {
            this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
            base.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szActionName);
            base.bAgeImmeExcute = this.cfgData.bAgeImmeExcute == 1;
            this.passiveEvent = Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.GetInstance().Create((int) this.cfgData.dwPassiveEventType);
            if (this.passiveEvent != null)
            {
                PassiveCondition condition = null;
                for (int i = 0; i < 2; i++)
                {
                    int dwConditionType = (int) this.cfgData.astPassiveConditon[i].dwConditionType;
                    condition = Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.GetInstance().Create(dwConditionType);
                    if (condition != null)
                    {
                        this.passiveEvent.AddCondition(condition);
                    }
                }
                this.passiveEvent.Init(this.sourceActor, this);
            }
        }

        public void InitCDTime(int _cdTime)
        {
            if (this.passiveEvent != null)
            {
                this.passiveEvent.InitCDTime(_cdTime);
            }
        }

        public void Reset()
        {
            this.sourceActor.Validate();
            this.passiveEvent.UnInit();
            this.passiveEvent.Init(this.sourceActor, this);
        }

        public void UnInit()
        {
            if (this.passiveEvent != null)
            {
                this.passiveEvent.UnInit();
            }
        }

        public void UpdateLogic(int nDelta)
        {
            if (this.passiveEvent != null)
            {
                this.passiveEvent.UpdateLogic(nDelta);
            }
        }

        public override bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
        {
            param.Instigator = this;
            DebugHelper.Assert((bool) param.Originator);
            if (!base.Use(user, ref param))
            {
                return false;
            }
            return true;
        }

        public bool bShowAsElite
        {
            get
            {
                return (this.cfgData.bShowAsElite != 0);
            }
        }

        public string PassiveSkillDesc
        {
            get
            {
                return Utility.UTF8Convert(this.cfgData.szPassiveDesc);
            }
        }

        public string PassiveSkillName
        {
            get
            {
                return Utility.UTF8Convert(this.cfgData.szPassiveName);
            }
        }
    }
}

