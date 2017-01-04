namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;

    public class CheckInActivity : Activity
    {
        private ResWealCheckIn _config;
        private byte _curFillPriceIndex;
        private uint _lastCheckTime;

        public CheckInActivity(ActivitySys mgr, ResWealCheckIn config) : base(mgr, config.stCommon)
        {
            this._config = config;
            for (ushort i = 0; i < this._config.wDays; i = (ushort) (i + 1))
            {
                CheckInPhase ap = new CheckInPhase(this, i, this._config.astReward[i]);
                base.AddPhase(ap);
            }
            this._lastCheckTime = 0;
            this._curFillPriceIndex = 0;
        }

        public override uint GetVipAddition(int vipFlagBit)
        {
            switch (vipFlagBit)
            {
                case 1:
                    return this._config.wQQVIPExtraRatio;

                case 0x10:
                    return this._config.wQQSuperVIPExtraRatio;
            }
            return base.GetVipAddition(vipFlagBit);
        }

        public override void SetPhaseMarked(uint phaseId)
        {
            base.SetPhaseMarked(phaseId);
            if ((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null) && ((phaseId + 1) < base.PhaseList.Count))
            {
                base.PhaseList[((int) phaseId) + 1]._NotifyStateChanged();
            }
        }

        public override void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
        {
            this._lastCheckTime = actvInfo.stCheckIn.dwLastCheckTime;
            this._curFillPriceIndex = actvInfo.stCheckIn.bFillPriceIndex;
            base.SetPhaseMarks(actvInfo.stCheckIn.ullRewardMask);
        }

        public bool CanFillIn
        {
            get
            {
                return (this._config.bCanFillIn == 1);
            }
        }

        public RES_WEAL_CHEKIN_TYPE CheckInType
        {
            get
            {
                return (RES_WEAL_CHEKIN_TYPE) this._config.bCheckInType;
            }
        }

        public uint FillInPriceID
        {
            get
            {
                return this._config.dwFillInPriceID;
            }
        }

        public uint FillPrice
        {
            get
            {
                ResFillInPrice price = null;
                if (GameDataMgr.wealCheckFillDict.TryGetValue(this.FillInPriceID, out price) && (this._curFillPriceIndex < price.Price.Length))
                {
                    return price.Price[this._curFillPriceIndex];
                }
                return 0;
            }
        }

        public RES_SHOPBUY_COINTYPE FillPriceCoin
        {
            get
            {
                return RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
            }
        }

        public override uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public uint LastCheckTime
        {
            get
            {
                return this._lastCheckTime;
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return COM_WEAL_TYPE.COM_WEAL_CHECKIN;
            }
        }
    }
}

