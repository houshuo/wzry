namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class PointsExchangeActivity : Activity
    {
        private List<int> _delTempList;
        private readonly Dictionary<int, uint> _exchangeCount;
        private uint _occConsumeValue;
        private uint _occPointValue;
        private uint _pointPerConsume;
        public readonly ResWealPointExchange PointsConfig;

        public PointsExchangeActivity(ActivitySys mgr, ResWealPointExchange config) : base(mgr, config.stCommon)
        {
            this._exchangeCount = new Dictionary<int, uint>();
            this._delTempList = new List<int>();
            this.PointsConfig = config;
            for (uint i = 0; i < config.bExchangeCnt; i++)
            {
                PointsExchangePhase ap = new PointsExchangePhase(this, i, config.astExchangeInfo[i]);
                base.AddPhase(ap);
            }
        }

        public uint GetExchangeCount(int index)
        {
            uint num = 0;
            return (!this._exchangeCount.TryGetValue(index, out num) ? 0 : num);
        }

        public uint GetMaxExchangeCount(int index)
        {
            if (index < this.PointsConfig.bExchangeCnt)
            {
                return this.PointsConfig.astExchangeInfo[index].dwDupCnt;
            }
            return 0;
        }

        public void IncreaseExchangeCount(int index)
        {
            uint num = 0;
            if (this._exchangeCount.TryGetValue(index, out num))
            {
                ResDT_PointExchange exchange = this.PointsConfig.astExchangeInfo[index];
                if (num < exchange.dwDupCnt)
                {
                    this._exchangeCount.Remove(index);
                    this._exchangeCount.Add(index, ++num);
                }
            }
            else
            {
                this._exchangeCount.Add(index, 1);
            }
        }

        public void ResetExchangeCount()
        {
            this._delTempList.Clear();
            Dictionary<int, uint>.Enumerator enumerator = this._exchangeCount.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, uint> current = enumerator.Current;
                int key = current.Key;
                if ((key >= 0) && (key < this.PointsConfig.astExchangeInfo.Length))
                {
                    ResDT_PointExchange exchange = this.PointsConfig.astExchangeInfo[key];
                    if (exchange.bIsDupClr > 0)
                    {
                        this._delTempList.Add(key);
                    }
                }
            }
            List<int>.Enumerator enumerator2 = this._delTempList.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                int num2 = enumerator2.Current;
                this._exchangeCount.Remove(num2);
            }
            this._delTempList.Clear();
        }

        public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
        {
            this._exchangeCount.Clear();
            byte bWealCnt = actvInfo.stPtExchange.bWealCnt;
            COMDT_WEAL_EXCHANGE_OBJ[] astWealList = actvInfo.stPtExchange.astWealList;
            for (int i = 0; i < bWealCnt; i++)
            {
                this._exchangeCount.Add(astWealList[i].bWealIdx, astWealList[i].dwExchangeCnt);
            }
        }

        public void UpdatePointsInfo(uint occJiFen, uint occConsumeValue)
        {
            this._occPointValue = occJiFen;
            this._occConsumeValue = occConsumeValue;
        }

        public void UpdateView()
        {
            base.NotifyTimeStateChanged();
        }

        public override string Brief
        {
            get
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                object[] args = new object[] { this.PerConsume, this.PerPoint, this.OccuConsume, this.OccuPoint, masterRoleInfo.JiFen };
                return string.Format(base.Brief, args);
            }
        }

        public override bool Completed
        {
            get
            {
                return false;
            }
        }

        public override uint ID
        {
            get
            {
                return this.PointsConfig.dwID;
            }
        }

        public uint OccuConsume
        {
            get
            {
                return this._occConsumeValue;
            }
        }

        public uint OccuPoint
        {
            get
            {
                return this._occPointValue;
            }
        }

        public uint PerConsume
        {
            get
            {
                return ((this.PointsConfig != null) ? this.PointsConfig.dwPointGetParam : 1);
            }
        }

        public uint PerPoint
        {
            get
            {
                return ((this.PointsConfig != null) ? this.PointsConfig.dwPointGetCnt : 1);
            }
        }

        public override bool ReadyForDot
        {
            get
            {
                for (int i = 0; i < base.PhaseList.Count; i++)
                {
                    ActivityPhase phase = base.PhaseList[i];
                    if ((phase != null) && phase.ReadyForGet)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE;
            }
        }
    }
}

