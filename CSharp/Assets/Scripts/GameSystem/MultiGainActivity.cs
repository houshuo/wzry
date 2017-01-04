namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    public class MultiGainActivity : Activity
    {
        private ResWealMultiple _config;

        public MultiGainActivity(ActivitySys mgr, ResWealMultiple config) : base(mgr, config.stCommon)
        {
            this._config = config;
            for (ushort i = 0; i < this._config.wPeriodNum; i = (ushort) (i + 1))
            {
                MultiGainPhase ap = new MultiGainPhase(this, i, this._config.astPeriod[i]);
                base.AddPhase(ap);
            }
        }

        public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
        {
            for (int i = 0; i < base.PhaseList.Count; i++)
            {
                if (i >= actvInfo.stMultiple.UsedCnt.Length)
                {
                    break;
                }
                MultiGainPhase phase = (MultiGainPhase) base.PhaseList[i];
                phase._usedTimes = actvInfo.stMultiple.UsedCnt[i];
                phase._NotifyStateChanged();
            }
        }

        public override uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return COM_WEAL_TYPE.COM_WEAL_MULTIPLE;
            }
        }
    }
}

