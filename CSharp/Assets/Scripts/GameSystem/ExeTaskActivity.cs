namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    public class ExeTaskActivity : Activity
    {
        private ResWealCondition _config;

        public ExeTaskActivity(ActivitySys mgr, ResWealCondition config) : base(mgr, config.stCommon)
        {
            this._config = config;
            for (ushort i = 0; (i < this._config.wConNum) && (i < this._config.astConInfo.Length); i = (ushort) (i + 1))
            {
                ExeTaskPhase ap = new ExeTaskPhase(this, i, this._config.astConInfo[i]);
                base.AddPhase(ap);
                if (this._config.astConInfo[i].dwConType == 14)
                {
                    mgr.IsShareTask = true;
                }
            }
        }

        public void LoadInfo(COMDT_WEAL_CON_DATA_DETAIL conData)
        {
            for (int i = 0; i < base.PhaseList.Count; i++)
            {
                ExeTaskPhase phase = (ExeTaskPhase) base.PhaseList[i];
                phase.SetAchiveve((conData.dwReachMask & (((int) 1) << i)) > 0, (conData.dwLimitReachMask & (((int) 1) << i)) > 0);
                if (i < conData.wConNum)
                {
                    phase.SetCurrent((int) conData.astConData[i].dwValue);
                }
            }
            base.SetPhaseMarks((ulong) conData.dwRewardMask);
        }

        public override int Current
        {
            get
            {
                ExeTaskPhase phase = null;
                for (int i = 0; i < base.PhaseList.Count; i++)
                {
                    ExeTaskPhase phase2 = base.PhaseList[i] as ExeTaskPhase;
                    if (!phase2.Achieved)
                    {
                        phase = phase2;
                        break;
                    }
                }
                if ((phase == null) && (base.PhaseList.Count > 0))
                {
                    phase = base.PhaseList[base.PhaseList.Count - 1] as ExeTaskPhase;
                }
                return ((phase == null) ? 0 : phase.Current);
            }
        }

        public override uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public override int Target
        {
            get
            {
                return base.PhaseList[base.PhaseList.Count - 1].Target;
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return COM_WEAL_TYPE.COM_WEAL_CONDITION;
            }
        }
    }
}

