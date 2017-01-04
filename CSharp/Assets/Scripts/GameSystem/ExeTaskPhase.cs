namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;
    using UnityEngine;

    public class ExeTaskPhase : ActivityPhase
    {
        private bool _achieved;
        private bool _achieveInLimit;
        private ResDT_WealConInfo _config;
        private int _current;
        private uint _id;

        public ExeTaskPhase(Activity owner, uint id, ResDT_WealConInfo config) : base(owner)
        {
            this._id = id;
            this._config = config;
            this._achieved = false;
            this._achieveInLimit = false;
            this._current = 0;
        }

        public override bool AchieveJump()
        {
            if (((this._config.dwConType != 14) || (this._config.ReachConParam.Length < 2)) || (this._config.ReachConParam[0] != 2))
            {
                return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) this._config.dwJumpEntry);
            }
            uint id = this._config.ReachConParam[1];
            uint[] kShareParam = new uint[] { id };
            MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.set(2, 1, kShareParam);
            string cDNUrl = MonoSingleton<BannerImageSys>.GetInstance().GetCDNUrl(id);
            Debug.Log("share jump " + cDNUrl);
            if (!string.IsNullOrEmpty(cDNUrl))
            {
                MonoSingleton<IDIPSys>.GetInstance().ShareActivityTask(cDNUrl);
            }
            return true;
        }

        internal void SetAchiveve(bool achieved, bool achieveInLimit)
        {
            if ((this._achieved != achieved) || (this._achieveInLimit != achieveInLimit))
            {
                this._achieved = achieved;
                this._achieveInLimit = achieveInLimit;
                base._NotifyStateChanged();
            }
        }

        internal void SetCurrent(int val)
        {
            if (val != this._current)
            {
                this._current = val;
                base._NotifyStateChanged();
            }
        }

        public override bool Achieved
        {
            get
            {
                return this._achieved;
            }
        }

        public override int AchieveInDays
        {
            get
            {
                if (this.LimitDays == 0)
                {
                    return 0x63;
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    return -1;
                }
                if (this._achieved)
                {
                    if (this._achieveInLimit)
                    {
                        return 0;
                    }
                    return -1;
                }
                DateTime time = Utility.ToUtcTime2Local((long) ((ulong) CRoleInfo.GetCurrentUTCTime()));
                DateTime time2 = Utility.ToUtcTime2Local((long) masterRoleInfo.AccountRegisterTime);
                time2 = new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0);
                TimeSpan span = (TimeSpan) (time - time2);
                double totalDays = span.TotalDays;
                if (totalDays > this.LimitDays)
                {
                    return -1;
                }
                return (int) Math.Ceiling(this.LimitDays - totalDays);
            }
        }

        public override bool AchieveStateValid
        {
            get
            {
                return (base.timeState == ActivityPhase.TimeState.Started);
            }
        }

        public override int CloseTime
        {
            get
            {
                return 0;
            }
        }

        public RES_WEAL_CON_TYPE ConditionType
        {
            get
            {
                return (RES_WEAL_CON_TYPE) this._config.dwConType;
            }
        }

        public override int Current
        {
            get
            {
                return this._current;
            }
        }

        public override uint ExtraRewardID
        {
            get
            {
                return this._config.dwLimitRewardID;
            }
        }

        public override uint ID
        {
            get
            {
                return this._id;
            }
        }

        public uint LimitDays
        {
            get
            {
                return this._config.dwLimitDays;
            }
        }

        public override bool ReadyForGet
        {
            get
            {
                return (this.Achieved && base.ReadyForGet);
            }
        }

        public override uint RewardID
        {
            get
            {
                return this._config.dwFixedRewardID;
            }
        }

        public override int StartTime
        {
            get
            {
                return 0;
            }
        }

        public override int Target
        {
            get
            {
                return (int) this._config.dwGoalValue;
            }
        }

        public override string Tips
        {
            get
            {
                int current = this.Current;
                int target = this.Target;
                if (current > target)
                {
                    current = target;
                }
                int num3 = target - current;
                return Utility.UTF8Convert(this._config.szDesc).Replace("{C}", current.ToString()).Replace("{T}", target.ToString()).Replace("{R}", num3.ToString());
            }
        }
    }
}

