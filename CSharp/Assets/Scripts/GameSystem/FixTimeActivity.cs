namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    public class FixTimeActivity : Activity
    {
        private ResWealFixedTime _config;
        private bool _inMultipleTime;

        public FixTimeActivity(ActivitySys mgr, ResWealFixedTime config) : base(mgr, config.stCommon)
        {
            this._config = config;
            this._inMultipleTime = false;
            for (ushort i = 0; (i < this._config.wPeriodNum) && (i < this._config.astPeriod.Length); i = (ushort) (i + 1))
            {
                FixTimePhase ap = new FixTimePhase(this, i, this._config.astPeriod[i]);
                base.AddPhase(ap);
            }
        }

        public override bool CheckTimeState()
        {
            DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            bool flag = false;
            for (int i = 0; i < this._config.astMultipleTime.Length; i++)
            {
                ResDT_DateTime time2 = this._config.astMultipleTime[i];
                if (((time2.ullStartTime > 0L) && (time2.ullEndTime > 0L)) && ((time >= Utility.ToUtcTime2Local((long) time2.ullStartTime)) && (time < Utility.ToUtcTime2Local((long) time2.ullEndTime))))
                {
                    flag = true;
                    break;
                }
            }
            bool flag2 = flag != this._inMultipleTime;
            this._inMultipleTime = flag;
            bool flag3 = base.CheckTimeState();
            if (!flag3 && flag2)
            {
                base.NotifyTimeStateChanged();
            }
            return (flag3 || flag2);
        }

        public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
        {
            base.SetPhaseMarks(actvInfo.stFixedTime.ullRewardMask);
        }

        public override uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public override bool InMultipleTime
        {
            get
            {
                return this._inMultipleTime;
            }
        }

        public override uint MultipleTimes
        {
            get
            {
                return (this._config.dwMultipleRatio / 0x2710);
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return COM_WEAL_TYPE.COM_WEAL_FIXEDTIME;
            }
        }
    }
}

