namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    internal class StarEvaluation : IStarEvaluation
    {
        private static StarSystemFactory ConditionFactory = new StarSystemFactory(typeof(StarConditionAttribute), typeof(IStarCondition));
        public ListView<IStarCondition> Conditions = new ListView<IStarCondition>(3);
        private string Description;
        public int Index;
        public ResEvaluateStarInfo StarInfo;

        public event OnEvaluationChangedDelegate OnChanged;

        protected void AddCondition(ResDT_ConditionInfo InCondConfig)
        {
            StarCondition item = ConditionFactory.Create((int) InCondConfig.dwType) as StarCondition;
            DebugHelper.Assert(item != null);
            if (item != null)
            {
                item.OnStarConditionChanged += new OnStarConditionChangedDelegate(this.OnConditionChanged);
                item.Initialize(InCondConfig);
                this.Conditions.Add(item);
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < this.Conditions.Count; i++)
            {
                if (this.Conditions[i] != null)
                {
                    this.Conditions[i].Dispose();
                }
            }
            this.Conditions.Clear();
        }

        public IStarCondition GetConditionAt(int Index)
        {
            return (((Index < 0) || (Index >= this.Conditions.Count)) ? null : this.Conditions[Index]);
        }

        public ListView<IStarCondition>.Enumerator GetEnumerator()
        {
            return this.Conditions.GetEnumerator();
        }

        public void Initialize(ResEvaluateStarInfo InStarInfo)
        {
            this.StarInfo = InStarInfo;
            this.Description = Utility.UTF8Convert(InStarInfo.szCondDesc);
            for (int i = 0; i < InStarInfo.astConditions.Length; i++)
            {
                ResDT_ConditionInfo inCondConfig = InStarInfo.astConditions[i];
                if (inCondConfig.dwType == 0)
                {
                    break;
                }
                this.AddCondition(inCondConfig);
            }
        }

        public virtual void OnActorDeath(ref GameDeadEventParam prm)
        {
            for (int i = this.Conditions.Count - 1; (i >= 0) && (i < this.Conditions.Count); i--)
            {
                IStarCondition condition = this.Conditions[i];
                if (condition != null)
                {
                    condition.OnActorDeath(ref prm);
                }
            }
        }

        public virtual void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
        {
            for (int i = this.Conditions.Count - 1; (i >= 0) && (i < this.Conditions.Count); i--)
            {
                IStarCondition condition = this.Conditions[i];
                if (condition != null)
                {
                    condition.OnCampScoreUpdated(ref prm);
                }
            }
        }

        protected void OnConditionChanged(IStarCondition InCondition)
        {
            DebugHelper.Assert(InCondition != null);
            if (this.OnChanged != null)
            {
                this.OnChanged(this, InCondition);
            }
        }

        public virtual void Start()
        {
            for (int i = 0; i < this.Conditions.Count; i++)
            {
                this.Conditions[i].Start();
            }
        }

        public ResEvaluateStarInfo configInfo
        {
            get
            {
                return this.StarInfo;
            }
        }

        public virtual string description
        {
            get
            {
                if (this.StarInfo.bHideDetail > 0)
                {
                    return this.Description;
                }
                return (this.Description + (((this.Conditions.Count <= 0) || (this.Conditions[0] == null)) ? string.Empty : this.Conditions[0].description));
            }
        }

        public int index
        {
            get
            {
                return this.Index;
            }
        }

        public bool isFailure
        {
            get
            {
                return (this.status == StarEvaluationStatus.Failure);
            }
        }

        public bool isInProgressing
        {
            get
            {
                return (this.status == StarEvaluationStatus.InProgressing);
            }
        }

        public bool isSuccess
        {
            get
            {
                return (this.status == StarEvaluationStatus.Success);
            }
        }

        public RES_LOGIC_OPERATION_TYPE logicType
        {
            get
            {
                return (RES_LOGIC_OPERATION_TYPE) this.StarInfo.bLogicType;
            }
        }

        public string rawDescription
        {
            get
            {
                return this.Description;
            }
        }

        public StarEvaluationStatus status
        {
            get
            {
                if (this.logicType == RES_LOGIC_OPERATION_TYPE.RES_LOGIC_OPERATION_AND)
                {
                    bool flag = true;
                    for (int i = 0; i < this.Conditions.Count; i++)
                    {
                        DebugHelper.Assert(this.Conditions[i] != null);
                        if (this.Conditions[i].status == StarEvaluationStatus.Failure)
                        {
                            return StarEvaluationStatus.Failure;
                        }
                        if (this.Conditions[i].status == StarEvaluationStatus.InProgressing)
                        {
                            flag = false;
                        }
                    }
                    return (!flag ? StarEvaluationStatus.InProgressing : StarEvaluationStatus.Success);
                }
                if (this.logicType == RES_LOGIC_OPERATION_TYPE.RES_LOGIC_OPERATION_OR)
                {
                    for (int j = 0; j < this.Conditions.Count; j++)
                    {
                        DebugHelper.Assert(this.Conditions[j] != null);
                        if (this.Conditions[j].status == StarEvaluationStatus.Success)
                        {
                            return StarEvaluationStatus.Success;
                        }
                    }
                    return StarEvaluationStatus.Failure;
                }
                DebugHelper.Assert(false, "未识别的逻辑关系");
                return StarEvaluationStatus.Failure;
            }
        }
    }
}

