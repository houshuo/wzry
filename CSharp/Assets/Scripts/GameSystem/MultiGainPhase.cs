namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;

    public class MultiGainPhase : ActivityPhase
    {
        private ResDT_WealMultiplePeriod _config;
        private uint _id;
        internal ushort _usedTimes;

        public MultiGainPhase(Activity owner, uint id, ResDT_WealMultiplePeriod config) : base(owner)
        {
            this._id = id;
            this._config = config;
            this._usedTimes = 0;
        }

        public override bool AchieveJump()
        {
            switch (this.GameType)
            {
                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ADVENTURE:
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Adv_OpenChapterForm);
                    return true;

                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ACTIVITY:
                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_ARENA:
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Explore_OpenForm);
                    return true;

                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_PVP_MATCH:
                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_PVP_ROOM:
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
                    return true;

                case RES_WEAL_GAME_TYPE.RES_WEAL_GAME_BURNING:
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
                    return true;
            }
            return false;
        }

        public bool HasSubGameType(int subType)
        {
            return ((this._config.dwGameSubTypeMask & (((int) 1) << subType)) > 0);
        }

        public override int CloseTime
        {
            get
            {
                return (int) this._config.dwEndTime;
            }
        }

        public string Desc
        {
            get
            {
                return Utility.UTF8Convert(this._config.szDesc);
            }
        }

        public RES_WEAL_GAME_TYPE GameType
        {
            get
            {
                return (RES_WEAL_GAME_TYPE) this._config.dwGameType;
            }
        }

        public override uint ID
        {
            get
            {
                return this._id;
            }
        }

        public ushort LimitTimes
        {
            get
            {
                return this._config.wLimitTimes;
            }
        }

        public override uint MultipleTimes
        {
            get
            {
                return (this._config.dwMultipleRatio / 0x2710);
            }
        }

        public override bool ReadyForGet
        {
            get
            {
                return false;
            }
        }

        public bool ReadyForGo
        {
            get
            {
                return (base.ReadyForGet && (((this.LimitTimes > 0) && (this.RemainTimes > 0)) || (this.LimitTimes == 0)));
            }
        }

        public ushort RemainTimes
        {
            get
            {
                return (ushort) (this.LimitTimes - this._usedTimes);
            }
        }

        public override uint RewardID
        {
            get
            {
                return 0;
            }
        }

        public override int StartTime
        {
            get
            {
                return (int) this._config.dwStartTime;
            }
        }
    }
}

