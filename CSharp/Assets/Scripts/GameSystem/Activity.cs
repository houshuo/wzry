namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class Activity
    {
        private ResDT_WealCommon _config;
        private ListView<ActivityPhase> _phaseList;
        private int _secondSpan;
        private ActivitySys _sys;
        private TimeState _timeState;
        private bool _visited;

        public event ActivityEvent OnMaskStateChange;

        public event ActivityEvent OnTimeStateChange;

        public Activity(ActivitySys mgr, ResDT_WealCommon config)
        {
            this._sys = mgr;
            this._config = config;
            this._phaseList = new ListView<ActivityPhase>();
            this._timeState = TimeState.InHiding;
            this._secondSpan = 0;
        }

        protected void AddPhase(ActivityPhase ap)
        {
            this._phaseList.Add(ap);
        }

        public virtual bool CheckTimeState()
        {
            TimeState close;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            bool flag = false;
            if (((this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_LIMIT) || (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT)) || (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS))
            {
                if ((this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT) && ((masterRoleInfo.AccountRegisterTime < this.StartTime) || (masterRoleInfo.AccountRegisterTime > this.CloseTime)))
                {
                    close = TimeState.Close;
                }
                else
                {
                    long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                    this._secondSpan = (int) (currentUTCTime - this.ShowTime);
                    if (this._secondSpan < 0)
                    {
                        this._secondSpan = -this._secondSpan;
                        close = TimeState.InHiding;
                    }
                    else
                    {
                        this._secondSpan = (int) (currentUTCTime - this.StartTime);
                        if (this._secondSpan < 0)
                        {
                            close = TimeState.ForeShow;
                            this._secondSpan = -this._secondSpan;
                        }
                        else
                        {
                            this._secondSpan = (int) (currentUTCTime - this.CloseTime);
                            if (this._secondSpan <= 0)
                            {
                                close = TimeState.Going;
                                this._secondSpan = -this._secondSpan;
                            }
                            else
                            {
                                close = TimeState.Close;
                            }
                        }
                    }
                }
            }
            else if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER)
            {
                close = TimeState.Going;
            }
            else
            {
                return false;
            }
            TimeState state2 = this._timeState;
            if (close != this._timeState)
            {
                this._timeState = close;
                flag = true;
            }
            if ((this._timeState == TimeState.Going) || (state2 == TimeState.Going))
            {
                for (int i = 0; i < this.PhaseList.Count; i++)
                {
                    if (this.PhaseList[i].CheckTimeState())
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                this.NotifyTimeStateChanged();
            }
            return flag;
        }

        public virtual void Clear()
        {
            for (int i = 0; i < this._phaseList.Count; i++)
            {
                this._phaseList[i].Clear();
            }
            this._phaseList.Clear();
        }

        public static uint GenKey(COM_WEAL_TYPE type, uint id)
        {
            return ((((uint) type) << 0x1c) | (0xfffffff & id));
        }

        public virtual uint GetVipAddition(int vipFlagBit)
        {
            return 0;
        }

        protected void NotifyMaskStateChanged()
        {
            if (this.OnMaskStateChange != null)
            {
                this.OnMaskStateChange(this);
            }
            this.Sys._NotifyStateChanged();
        }

        protected void NotifyTimeStateChanged()
        {
            if (this.OnTimeStateChange != null)
            {
                this.OnTimeStateChange(this);
            }
            this.Sys._NotifyStateChanged();
        }

        public virtual void SetPhaseMarked(uint phaseId)
        {
            if (phaseId < this.PhaseList.Count)
            {
                this.PhaseList[(int) phaseId].Marked = true;
                this.NotifyMaskStateChanged();
            }
        }

        public virtual void SetPhaseMarks(ulong mask)
        {
            for (int i = 0; i < this.PhaseList.Count; i++)
            {
                this.PhaseList[i].Marked = (mask & (((ulong) 1L) << i)) > 0L;
            }
            this.NotifyMaskStateChanged();
        }

        public virtual void Start()
        {
            this._visited = PlayerPrefs.GetInt(this.UID, 0) > 0;
        }

        public virtual void UpdateInfo(ref COMDT_WEAL_UNION actvInfo)
        {
        }

        public virtual string Brief
        {
            get
            {
                return Utility.UTF8Convert(this._config.szBrief);
            }
        }

        public virtual long CloseTime
        {
            get
            {
                if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        if (masterRoleInfo.AccountRegisterTime >= this._config.ullStartTime)
                        {
                            return ((masterRoleInfo.AccountRegisterTime_ZeroDay + ((long) (((ulong) 0x1517fL) * this._config.ullEndTime))) - 1L);
                        }
                        return 0xf45c2700L;
                    }
                }
                return (long) this._config.ullEndTime;
            }
        }

        public virtual bool Completed
        {
            get
            {
                for (int i = 0; i < this.PhaseList.Count; i++)
                {
                    if (!this.PhaseList[i].Marked)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public virtual string Content
        {
            get
            {
                return Utility.UTF8Convert(this._config.szDescContent);
            }
        }

        public virtual ActivityPhase CurPhase
        {
            get
            {
                for (int i = 0; i < this.PhaseList.Count; i++)
                {
                    if (!this.PhaseList[i].Marked)
                    {
                        return this.PhaseList[i];
                    }
                }
                if (this.PhaseList.Count > 0)
                {
                    return this.PhaseList[this.PhaseList.Count - 1];
                }
                return null;
            }
        }

        public virtual int Current
        {
            get
            {
                for (int i = 0; i < this.PhaseList.Count; i++)
                {
                    if (!this.PhaseList[i].Marked)
                    {
                        return i;
                    }
                }
                return this.PhaseList.Count;
            }
        }

        public virtual RES_WEAL_ENTRANCE_TYPE Entrance
        {
            get
            {
                return (RES_WEAL_ENTRANCE_TYPE) this._config.bEntrance;
            }
        }

        public virtual RES_WEAL_COLORBAR_TYPE FlagType
        {
            get
            {
                if (this.timeState < TimeState.Going)
                {
                    return RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NOTICE;
                }
                RES_WEAL_COLORBAR_TYPE bColorBar = (RES_WEAL_COLORBAR_TYPE) this._config.bColorBar;
                if ((bColorBar == RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NEW) && (this.StartTime > 0L))
                {
                    TimeSpan span = (TimeSpan) (Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime()) - Utility.ToUtcTime2Local(this.StartTime));
                    if (span.TotalDays > 2.0)
                    {
                        bColorBar = RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_HOT;
                    }
                }
                return bColorBar;
            }
        }

        public virtual string Icon
        {
            get
            {
                return Utility.UTF8Convert(this._config.szIcon);
            }
        }

        public abstract uint ID { get; }

        public virtual bool InMultipleTime
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsImageTitle
        {
            get
            {
                string str = Utility.UTF8Convert(this._config.szTitle);
                return ((str.Length > 2) && (str.IndexOf("i:", 0, 2) == 0));
            }
        }

        public uint Key
        {
            get
            {
                return GenKey(this.Type, this.ID);
            }
        }

        public virtual uint MultipleTimes
        {
            get
            {
                return 0;
            }
        }

        public virtual string Name
        {
            get
            {
                return Utility.UTF8Convert(this._config.szName);
            }
        }

        public string PeriodText
        {
            get
            {
                if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("activityForever");
                }
                string str = Utility.DateTimeFormatString(Utility.ToUtcTime2Local(this.StartTime), Utility.enDTFormate.DATE);
                string str2 = Utility.DateTimeFormatString(Utility.ToUtcTime2Local(this.CloseTime), Utility.enDTFormate.DATE);
                return (!(str != str2) ? str : (str + " ~ " + str2));
            }
        }

        public ListView<ActivityPhase> PhaseList
        {
            get
            {
                return this._phaseList;
            }
        }

        public virtual bool ReadyForDot
        {
            get
            {
                return (this.ReadyForGet || !this._visited);
            }
        }

        public virtual bool ReadyForGet
        {
            get
            {
                if (this.timeState == TimeState.Going)
                {
                    for (int i = 0; i < this.PhaseList.Count; i++)
                    {
                        if (this.PhaseList[i].ReadyForGet)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public int secondSpan
        {
            get
            {
                return this._secondSpan;
            }
        }

        public virtual uint Sequence
        {
            get
            {
                return this._config.dwSortID;
            }
        }

        public virtual long ShowTime
        {
            get
            {
                if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_LIMIT)
                {
                    return (long) this._config.ullShowTime;
                }
                return this.StartTime;
            }
        }

        public virtual long StartTime
        {
            get
            {
                if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_DAYS)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        if (masterRoleInfo.AccountRegisterTime >= this._config.ullStartTime)
                        {
                            return masterRoleInfo.AccountRegisterTime_ZeroDay;
                        }
                        return 0xf45c2700L;
                    }
                }
                return (long) this._config.ullStartTime;
            }
        }

        public ActivitySys Sys
        {
            get
            {
                return this._sys;
            }
        }

        public virtual int Target
        {
            get
            {
                return this.PhaseList.Count;
            }
        }

        public string timeRemainText
        {
            get
            {
                if (this.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("activityForever");
                }
                if (this._timeState < TimeState.Going)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("activityNotStart");
                }
                if (this._timeState > TimeState.Going)
                {
                    return Singleton<CTextManager>.GetInstance().GetText("activityTimeOver");
                }
                int num = this._secondSpan;
                int num2 = num / 0x15180;
                num -= num2 * 0x15180;
                int num3 = num / 0xe10;
                num -= num3 * 0xe10;
                int num4 = num / 60;
                num -= num4 * 60;
                return Singleton<CTextManager>.GetInstance().GetText("TIME_SPAN_FORMAT").Replace("{0}", num2.ToString()).Replace("{1}", num3.ToString()).Replace("{2}", num4.ToString()).Replace("{3}", num.ToString());
            }
        }

        public TimeState timeState
        {
            get
            {
                return this._timeState;
            }
        }

        public virtual RES_WEAL_TIME_TYPE TimeType
        {
            get
            {
                return (RES_WEAL_TIME_TYPE) this._config.dwTimeType;
            }
        }

        public virtual string Tips
        {
            get
            {
                return Utility.UTF8Convert(this._config.szTips).Replace("{C}", this.Current.ToString()).Replace("{T}", this.Target.ToString());
            }
        }

        public virtual string Title
        {
            get
            {
                string str = Utility.UTF8Convert(this._config.szTitle);
                if (this.IsImageTitle)
                {
                    return str.Substring(2);
                }
                return str;
            }
        }

        public abstract COM_WEAL_TYPE Type { get; }

        public string UID
        {
            get
            {
                ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                object[] objArray1 = new object[] { "sgame_", (accountInfo == null) ? "0" : accountInfo.OpenId, "_activity_", this.Key };
                return string.Concat(objArray1);
            }
        }

        public bool Visited
        {
            get
            {
                return this._visited;
            }
            set
            {
                if (this._visited != value)
                {
                    this._visited = value;
                    PlayerPrefs.SetInt(this.UID, !this._visited ? 0 : 1);
                    PlayerPrefs.Save();
                    this.NotifyMaskStateChanged();
                }
            }
        }

        public virtual string Wigets
        {
            get
            {
                return Utility.UTF8Convert(this._config.szWidgets);
            }
        }

        public delegate void ActivityEvent(Activity acty);

        public enum TimeState
        {
            InHiding,
            ForeShow,
            Going,
            Close
        }
    }
}

