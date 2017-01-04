namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StarConditionAttrContext(4)]
    internal class StarConditionRestrictHero : StarCondition
    {
        private bool bCheckResults;
        private ListView<Info> TeamStat = new ListView<Info>();

        private bool CheckResults()
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer == null)
            {
                return false;
            }
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = hostPlayer.GetAllHeroes().GetEnumerator();
            bool flag = false;
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                int configId = current.handle.TheActorMeta.ConfigId;
                flag = false;
                for (int j = 0; j < this.TeamStat.Count; j++)
                {
                    Info info2 = this.TeamStat[j];
                    if (info2.Key == configId)
                    {
                        flag = true;
                        Info info = this.TeamStat[j];
                        info.Value++;
                        break;
                    }
                }
                if (!flag)
                {
                    Info item = new Info {
                        Key = configId,
                        Value = 1
                    };
                    this.TeamStat.Add(item);
                }
            }
            int inFirst = 0;
            flag = false;
            for (int i = 0; i < this.TeamStat.Count; i++)
            {
                Info info4 = this.TeamStat[i];
                if (info4.Key == this.targetID)
                {
                    Info info5 = this.TeamStat[i];
                    inFirst = info5.Value;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
            return SmartCompare.Compare<int>(inFirst, this.targetCount, this.operation);
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
        }

        public override void Start()
        {
            base.Start();
            bool flag = this.CheckResults();
            if (this.bCheckResults != flag)
            {
                this.bCheckResults = flag;
                this.TriggerChangedEvent();
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return (!this.bCheckResults ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success);
            }
        }

        private int targetCount
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        private int targetID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[1];
            }
        }

        public override int[] values
        {
            get
            {
                return new int[] { (!this.bCheckResults ? 0 : 1) };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Info
        {
            public int Key;
            public int Value;
        }
    }
}

