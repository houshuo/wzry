namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    public class CheckInPhase : ActivityPhase
    {
        private ResDT_WealCheckInDay _config;
        private uint _id;

        public CheckInPhase(Activity owner, uint id, ResDT_WealCheckInDay config) : base(owner)
        {
            this._id = id;
            this._config = config;
        }

        public uint GetGameVipDoubleLv()
        {
            return this._config.dwMultipleVipLvl;
        }

        public override uint GetVipAddition(int vipFlagBit)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return base.GetVipAddition(vipFlagBit);
            }
            int[] numArray = new int[] { 1, 0x10 };
            uint num = 0;
            for (int i = 0; i < numArray.Length; i++)
            {
                int num3 = numArray[i];
                if (((vipFlagBit == 0) || (vipFlagBit == num3)) && (this.HasVipAddition(num3) && masterRoleInfo.HasVip(num3)))
                {
                    num += base.Owner.GetVipAddition(num3);
                    if (vipFlagBit != 0)
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        public bool HasVipAddition(int vipFlagBit)
        {
            return ((this._config.dwMultipleMask & vipFlagBit) > 0);
        }

        public override int CloseTime
        {
            get
            {
                return 0;
            }
        }

        public override uint ID
        {
            get
            {
                return this._id;
            }
        }

        public override bool InMultipleTime
        {
            get
            {
                return false;
            }
        }

        public bool ReadyForFill
        {
            get
            {
                CheckInActivity owner = (CheckInActivity) base.Owner;
                if ((owner.CanFillIn && base.ReadyForGet) && (owner.Current == this.ID))
                {
                    DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                    DateTime time2 = Utility.ToUtcTime2Local((long) owner.LastCheckTime);
                    if (((this.ID < time.Day) && (owner.LastCheckTime != 0)) && (time.DayOfYear == time2.DayOfYear))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override bool ReadyForGet
        {
            get
            {
                if (base.ReadyForGet)
                {
                    CheckInActivity owner = (CheckInActivity) base.Owner;
                    if (owner.Current == this.ID)
                    {
                        DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                        DateTime time2 = Utility.ToUtcTime2Local((long) owner.LastCheckTime);
                        if ((owner.LastCheckTime == 0) || (time.DayOfYear != time2.DayOfYear))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public override uint RewardID
        {
            get
            {
                return this._config.dwRewardID;
            }
        }

        public override int StartTime
        {
            get
            {
                return 0;
            }
        }

        public override string Tips
        {
            get
            {
                return Singleton<CTextManager>.GetInstance().GetText("CheckInTips").Replace("{0}", this.Target.ToString());
            }
        }
    }
}

