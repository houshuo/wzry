namespace Assets.Scripts.Framework
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GameTask
    {
        private static CompareOperatorFunc[] _compareFuncs;
        private ResGameTask _config;
        private int _current;
        private ListView<GameTaskGroup> _ownerGroups;
        private int _readyTimer;
        private GameTaskSys _rootSys;
        private State _state;
        private int _timer;
        [CompilerGenerated]
        private static CompareOperatorFunc <>f__am$cache8;
        [CompilerGenerated]
        private static CompareOperatorFunc <>f__am$cache9;
        [CompilerGenerated]
        private static CompareOperatorFunc <>f__am$cacheA;
        [CompilerGenerated]
        private static CompareOperatorFunc <>f__am$cacheB;
        [CompilerGenerated]
        private static CompareOperatorFunc <>f__am$cacheC;

        static GameTask()
        {
            CompareOperatorFunc[] funcArray1 = new CompareOperatorFunc[5];
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new CompareOperatorFunc(GameTask.<_compareFuncs>m__2A);
            }
            funcArray1[0] = <>f__am$cache8;
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = new CompareOperatorFunc(GameTask.<_compareFuncs>m__2B);
            }
            funcArray1[1] = <>f__am$cache9;
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = new CompareOperatorFunc(GameTask.<_compareFuncs>m__2C);
            }
            funcArray1[2] = <>f__am$cacheA;
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = new CompareOperatorFunc(GameTask.<_compareFuncs>m__2D);
            }
            funcArray1[3] = <>f__am$cacheB;
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = new CompareOperatorFunc(GameTask.<_compareFuncs>m__2E);
            }
            funcArray1[4] = <>f__am$cacheC;
            _compareFuncs = funcArray1;
        }

        internal void _AddOwnerGroup(GameTaskGroup etg)
        {
            if (this._ownerGroups == null)
            {
                this._ownerGroups = new ListView<GameTaskGroup>();
            }
            this._ownerGroups.Add(etg);
        }

        [CompilerGenerated]
        private static bool <_compareFuncs>m__2A(int l, int r)
        {
            return (l < r);
        }

        [CompilerGenerated]
        private static bool <_compareFuncs>m__2B(int l, int r)
        {
            return (l <= r);
        }

        [CompilerGenerated]
        private static bool <_compareFuncs>m__2C(int l, int r)
        {
            return (l == r);
        }

        [CompilerGenerated]
        private static bool <_compareFuncs>m__2D(int l, int r)
        {
            return (l > r);
        }

        [CompilerGenerated]
        private static bool <_compareFuncs>m__2E(int l, int r)
        {
            return (l >= r);
        }

        public void Close()
        {
            if (this._state == State.START)
            {
                Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
                this.OnClose();
                this._state = State.CLOSE;
                this.RootSys._OnTaskClose(this);
                if (this._ownerGroups != null)
                {
                    for (int i = 0; i < this._ownerGroups.Count; i++)
                    {
                        this._ownerGroups[i]._OnChildClosed(this);
                    }
                }
            }
        }

        public void Destroy()
        {
            this.OnDestroy();
            Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
            Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
        }

        private void DoStart()
        {
            this._current = this.StartValue;
            Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._timer);
            if (this.TimeLimit > 0)
            {
                this._timer = Singleton<CTimerManager>.instance.AddTimer(this.TimeLimit, 1, new CTimer.OnTimeUpHandler(this.OnTimeOut), true);
            }
            this._state = State.START;
            this.OnStart();
            this.RootSys._OnTaskStart(this);
        }

        public void Initial(ResGameTask config, GameTaskSys rootSys)
        {
            this._rootSys = rootSys;
            this._config = config;
            this._current = 0;
            this._timer = 0;
            this._readyTimer = 0;
            this._ownerGroups = null;
            this._state = State.AWAKE;
            DebugHelper.Assert(null != this._config, "GameTask.config must not be null!");
            this.OnInitial();
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnInitial()
        {
        }

        private void OnReadyOver(int timer)
        {
            Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
            this.DoStart();
        }

        protected virtual void OnStart()
        {
        }

        private void OnTimeOut(int timer)
        {
            this.OnTimeOver();
            this.Close();
        }

        protected virtual void OnTimeOver()
        {
        }

        public void Start()
        {
            if (this._state == State.AWAKE)
            {
                if (this.TimeReady > 0)
                {
                    Singleton<CTimerManager>.instance.RemoveTimerSafely(ref this._readyTimer);
                    this._readyTimer = Singleton<CTimerManager>.instance.AddTimer(this.TimeReady, 1, new CTimer.OnTimeUpHandler(this.OnReadyOver), true);
                    this._state = State.READY;
                    this._rootSys._OnTaskReady(this);
                }
                else
                {
                    this.DoStart();
                }
            }
        }

        public bool Achieving
        {
            get
            {
                return _compareFuncs[((int) this.CompareOperator) - 1](this.Current, this.Target);
            }
        }

        public bool Active
        {
            get
            {
                return (this._state == State.START);
            }
        }

        public string CloseAction
        {
            get
            {
                return Utility.UTF8Convert(this._config.szCloseAction);
            }
        }

        public bool Closed
        {
            get
            {
                return (this._state == State.CLOSE);
            }
        }

        protected RES_COMPARE_OPERATOR_TYPE CompareOperator
        {
            get
            {
                if ((this._config.bCompare > 0) && (this._config.bCompare < 6))
                {
                    return (RES_COMPARE_OPERATOR_TYPE) this._config.bCompare;
                }
                return RES_COMPARE_OPERATOR_TYPE.RES_COMPARE_OPERATOR_TYPE_BIGGER_EQUAL;
            }
        }

        protected ResGameTask Config
        {
            get
            {
                return this._config;
            }
        }

        public int Current
        {
            get
            {
                return this._current;
            }
            set
            {
                if ((value != this._current) && (this._state == State.START))
                {
                    bool achieving = this.Achieving;
                    this._current = value;
                    this.RootSys._OnTaskGoing(this);
                    if ((this.Achieving && !achieving) && !this.StrictTime)
                    {
                        this.Close();
                    }
                }
            }
        }

        public uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public virtual bool IsGroup
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return Utility.UTF8Convert(this._config.szName);
            }
        }

        public virtual float Progress
        {
            get
            {
                return (((float) this.Current) / ((float) this.Target));
            }
        }

        public GameTaskSys RootSys
        {
            get
            {
                return this._rootSys;
            }
        }

        public string StartAction
        {
            get
            {
                return Utility.UTF8Convert(this._config.szStartAction);
            }
        }

        protected virtual int StartValue
        {
            get
            {
                return 0;
            }
        }

        public bool StrictTime
        {
            get
            {
                return (this._config.iTimeLimit < 0);
            }
        }

        public virtual int Target
        {
            get
            {
                return this._config.iTarget;
            }
        }

        public int TimeLimit
        {
            get
            {
                return Mathf.Abs(this._config.iTimeLimit);
            }
        }

        public int TimeReady
        {
            get
            {
                return this._config.iStartDelay;
            }
        }

        public int TimeRemain
        {
            get
            {
                if (this._timer > 0)
                {
                    int timerCurrent = Singleton<CTimerManager>.instance.GetTimerCurrent(this._timer);
                    if (timerCurrent > -1)
                    {
                        return (this.TimeLimit - timerCurrent);
                    }
                }
                return 0;
            }
        }

        public string Type
        {
            get
            {
                return Utility.UTF8Convert(this._config.szType);
            }
        }

        private delegate bool CompareOperatorFunc(int l, int r);

        protected enum State
        {
            AWAKE,
            READY,
            START,
            CLOSE
        }
    }
}

