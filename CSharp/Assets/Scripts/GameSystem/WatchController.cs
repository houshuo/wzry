namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    [MessageHandlerClass]
    public class WatchController : Singleton<WatchController>
    {
        private uint _buffRecvSize;
        private bool _isRunning;
        private bool _isStreamEnd;
        private uint _lastRequestTick;
        private uint _maxFrameNo;
        private uint _observeDelayFrames = 900;
        private float _overTime;
        private Queue<object> _pkgQueue;
        private byte[] _recvBuff;
        private uint _requestTickSpan = 80;
        private byte _speedRate = 1;
        private COMDT_TGWINFO _tgw;
        private uint _thisTick;
        private uint _totalRecvSize;
        private WorkMode _workMode;
        public const int MAX_PUSH_FRAME_LEN = 60;
        public const float OVER_WAIT_TIME = 10f;
        public const int RECV_BUFF_SIZE = 0x64000;
        public const int STOP_SPEED_BUFFERLIMIT = 120;
        public ulong TargetUID;

        private void CloseConnect()
        {
            if (this._tgw != null)
            {
                Singleton<NetworkModule>.GetInstance().CloseGameServerConnect(false);
                this._tgw = null;
            }
        }

        private void HandlePackage()
        {
            uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
            while (!MonoSingleton<GameLoader>.GetInstance().isLoadStart && (this._pkgQueue.Count > 0))
            {
                object obj2 = this._pkgQueue.Peek();
                CSDT_FRAPBOOT_INFO fbid = obj2 as CSDT_FRAPBOOT_INFO;
                if (fbid != null)
                {
                    if (((fbid.dwKFrapsNo > this.EndFrameNo) || (fbid.dwKFrapsNo <= curFrameNum)) || (fbid.dwKFrapsNo > (curFrameNum + 60)))
                    {
                        break;
                    }
                    this._pkgQueue.Dequeue();
                    FrameWindow.HandleFraqBootInfo(fbid);
                }
                else
                {
                    this._pkgQueue.Dequeue();
                    CSPkg msg = obj2 as CSPkg;
                    NetMsgDelegate msgHandler = Singleton<NetworkModule>.GetInstance().GetMsgHandler(msg.stPkgHead.dwMsgID);
                    if (msgHandler != null)
                    {
                        try
                        {
                            msgHandler(msg);
                            continue;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            if ((this.SpeedRate > 1) && (this.EndFrameNo < (curFrameNum + 120)))
            {
                this.SpeedRate = 1;
            }
        }

        private void HandleQuitResponse(SCPKG_QUITOBSERVE_RSP pkg)
        {
            if (this.IsWatching)
            {
                this.Stop();
            }
        }

        private void HandleVideoPiece(SCPKG_GET_VIDEOFRAPS_RSP pkg)
        {
            if (this._workMode != WorkMode.Observe)
            {
            }
            if (pkg.dwOffset == this._totalRecvSize)
            {
                if (pkg.bAllOver == 1)
                {
                    this._isStreamEnd = true;
                    this.CloseConnect();
                }
                if (pkg.dwTotalNum > 0)
                {
                    uint dwBufLen = pkg.dwBufLen;
                    if (dwBufLen > 0)
                    {
                        int newSize = (int) (this._buffRecvSize + dwBufLen);
                        if (newSize > this._recvBuff.Length)
                        {
                            Array.Resize<byte>(ref this._recvBuff, newSize);
                        }
                        Buffer.BlockCopy(pkg.szBuf, 0, this._recvBuff, (int) this._buffRecvSize, (int) dwBufLen);
                        this._buffRecvSize = (uint) newSize;
                        this._totalRecvSize += dwBufLen;
                        if ((pkg.dwThisPos + 1) < pkg.dwTotalNum)
                        {
                            this.RequestVideoPiece(false);
                        }
                        else
                        {
                            this.ParseVideoPackage();
                        }
                    }
                }
            }
        }

        public void MarkOver()
        {
            if (this.IsWatching)
            {
                this._overTime = Time.time;
            }
        }

        [MessageHandler(0x146d)]
        public static void OnGetVideoFraqRsp(CSPkg msg)
        {
            Singleton<WatchController>.GetInstance().HandleVideoPiece(msg.stPkgData.stGetVideoFrapsRsp);
        }

        [MessageHandler(0x146f)]
        public static void OnQuitObserveRsp(CSPkg msg)
        {
            Singleton<WatchController>.GetInstance().HandleQuitResponse(msg.stPkgData.stQuitObserveRsp);
        }

        private void ParseVideoPackage()
        {
            while (this._buffRecvSize > 0)
            {
                int usedSize = 0;
                CSPkg item = CSPkg.New();
                if (((item.unpack(ref this._recvBuff, (int) this._buffRecvSize, ref usedSize, 0) != TdrError.ErrorType.TDR_NO_ERROR) || (usedSize <= 0)) || (usedSize > this._buffRecvSize))
                {
                    break;
                }
                if (item.stPkgHead.dwMsgID == 0x40b)
                {
                    SCPKG_FRAPBOOTINFO stFrapBootInfo = item.stPkgData.stFrapBootInfo;
                    for (int i = 0; i < stFrapBootInfo.bSpareNum; i++)
                    {
                        SCPKG_FRAPBOOT_SINGLE scpkg_frapboot_single = stFrapBootInfo.astSpareFrap[i];
                        CSDT_FRAPBOOT_INFO csdt_frapboot_info = CSDT_FRAPBOOT_INFO.New();
                        int num3 = 0;
                        if ((csdt_frapboot_info.unpack(ref scpkg_frapboot_single.szInfoBuff, scpkg_frapboot_single.wLen, ref num3, 0) == TdrError.ErrorType.TDR_NO_ERROR) && (num3 > 0))
                        {
                            this._maxFrameNo = csdt_frapboot_info.dwKFrapsNo;
                            this._pkgQueue.Enqueue(csdt_frapboot_info);
                        }
                    }
                }
                else
                {
                    this._pkgQueue.Enqueue(item);
                }
                this._buffRecvSize -= (uint) usedSize;
                Buffer.BlockCopy(this._recvBuff, usedSize, this._recvBuff, 0, (int) this._buffRecvSize);
            }
        }

        public void Quit()
        {
            if (this._workMode == WorkMode.Observe)
            {
                if (this._isStreamEnd)
                {
                    this.Stop();
                }
                else
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x146e);
                    msg.stPkgData.stQuitObserveReq.bReserve = 0;
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                }
            }
            else if (this._workMode == WorkMode.Judge)
            {
                CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x146e);
                pkg2.stPkgData.stQuitObserveReq.bReserve = 0;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, false);
            }
            else
            {
                this.Stop();
            }
        }

        private void RequestVideoPiece(bool isNewPiece)
        {
            if ((this._workMode == WorkMode.Observe) && !this._isStreamEnd)
            {
                CSPkg pkg = NetworkModule.CreateDefaultCSPKG(0x146c);
                if (isNewPiece)
                {
                    pkg.stPkgData.stGetVideoFrapsReq.dwOffset = this._totalRecvSize;
                    pkg.stPkgData.stGetVideoFrapsReq.bNew = 1;
                }
                else
                {
                    pkg.stPkgData.stGetVideoFrapsReq.bNew = 0;
                }
                this.SendPackage(pkg);
                this._lastRequestTick = this._thisTick;
            }
        }

        private void SendPackage(CSPkg pkg)
        {
            if (this._tgw != null)
            {
                Singleton<NetworkModule>.GetInstance().SendGameMsg(ref pkg, 0);
            }
            else
            {
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg, false);
            }
        }

        public void StartJudge(COMDT_TGWINFO inTGW)
        {
            if (!this.IsWatching && !Singleton<BattleLogic>.instance.isRuning)
            {
                this._workMode = WorkMode.Judge;
                this._tgw = inTGW;
                if (this._tgw != null)
                {
                    NetworkModule.InitRelayConnnecting(this._tgw);
                }
            }
        }

        public bool StartObserve(COMDT_TGWINFO inTGW)
        {
            if (this.IsWatching || Singleton<BattleLogic>.instance.isRuning)
            {
                return false;
            }
            this._workMode = WorkMode.Observe;
            this._overTime = 0f;
            this.IsRunning = true;
            this.SpeedRate = 1;
            this._recvBuff = new byte[0x64000];
            this._buffRecvSize = 0;
            this._totalRecvSize = 0;
            this._pkgQueue = new Queue<object>(0x3e8);
            this._maxFrameNo = 0;
            this._isStreamEnd = false;
            this._thisTick = 0;
            this._lastRequestTick = 0;
            ResGlobalInfo info = new ResGlobalInfo();
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(0xd7, out info))
            {
                this._observeDelayFrames = (uint) Mathf.Clamp((int) ((info.dwConfValue * 0x3e8) / Singleton<FrameSynchr>.GetInstance().SvrFrameDelta), 0, 0x1194);
            }
            else
            {
                this._observeDelayFrames = 900;
            }
            this._tgw = inTGW;
            if (this._tgw != null)
            {
                NetworkModule.InitRelayConnnecting(this._tgw);
            }
            this.RequestVideoPiece(true);
            return true;
        }

        public void StartReplay(string replayPath)
        {
            if (!this.IsWatching && !Singleton<BattleLogic>.instance.isRuning)
            {
                if (Singleton<GameReplayModule>.GetInstance().BeginReplay(replayPath, false))
                {
                    this._workMode = WorkMode.Replay;
                    this._overTime = 0f;
                    this.IsRunning = true;
                    this.SpeedRate = 1;
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Can not play file: " + replayPath, false, 2f, null, new object[0]);
                }
                this._tgw = null;
            }
        }

        public void Stop()
        {
            this.SpeedRate = 1;
            this._workMode = WorkMode.None;
            this._isRunning = false;
            this._recvBuff = null;
            this._pkgQueue = null;
            this._overTime = 0f;
            this.TargetUID = 0L;
            if (this._workMode == WorkMode.Replay)
            {
                Singleton<GameReplayModule>.GetInstance().StopReplay();
            }
            this.CloseConnect();
        }

        public void UpdateFrame()
        {
            if (this.IsWatching)
            {
                this._thisTick++;
                if (this._overTime > 0f)
                {
                    if (Time.time > (this._overTime + 10f))
                    {
                        this.Stop();
                        return;
                    }
                }
                else
                {
                    bool isStreamEnd = false;
                    if (this._workMode == WorkMode.Observe)
                    {
                        isStreamEnd = this._isStreamEnd && (this._pkgQueue.Count == 0);
                    }
                    else if (this._workMode == WorkMode.Replay)
                    {
                        isStreamEnd = Singleton<GameReplayModule>.GetInstance().IsStreamEnd;
                    }
                    if (isStreamEnd && (Singleton<FrameSynchr>.GetInstance().CurFrameNum >= Singleton<FrameSynchr>.GetInstance().EndFrameNum))
                    {
                        this.MarkOver();
                        return;
                    }
                }
                if (this._workMode == WorkMode.Observe)
                {
                    if (!this._isStreamEnd && (this._thisTick > (this._lastRequestTick + this._requestTickSpan)))
                    {
                        this.RequestVideoPiece(true);
                    }
                    if (this._isRunning && (this._pkgQueue.Count > 0))
                    {
                        this.HandlePackage();
                    }
                }
                else if (((this._workMode == WorkMode.Replay) && (this.SpeedRate > 1)) && ((Singleton<FrameSynchr>.GetInstance().CurFrameNum >= Singleton<FrameSynchr>.GetInstance().EndFrameNum) && Singleton<GameReplayModule>.GetInstance().IsStreamEnd))
                {
                    this.SpeedRate = 1;
                }
            }
        }

        public uint CurFrameNo
        {
            get
            {
                uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
                uint endFrameNo = this.EndFrameNo;
                return ((curFrameNum >= endFrameNo) ? endFrameNo : curFrameNum);
            }
        }

        public uint EndFrameNo
        {
            get
            {
                if (this._workMode == WorkMode.Observe)
                {
                    return (!this._isStreamEnd ? ((this._maxFrameNo <= this._observeDelayFrames) ? 0 : (this._maxFrameNo - this._observeDelayFrames)) : this._maxFrameNo);
                }
                return Singleton<FrameSynchr>.GetInstance().EndFrameNum;
            }
        }

        public uint FrameDelta
        {
            get
            {
                return Singleton<FrameSynchr>.GetInstance().FrameDelta;
            }
        }

        public bool IsLiveCast
        {
            get
            {
                return (this._workMode == WorkMode.Judge);
            }
        }

        public bool IsReplaying
        {
            get
            {
                return (this._workMode == WorkMode.Replay);
            }
        }

        public bool IsRunning
        {
            get
            {
                return this._isRunning;
            }
            set
            {
                if (this.IsWatching)
                {
                    this._isRunning = value;
                    Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(this._isRunning);
                    Singleton<FrameSynchr>.GetInstance().FrameSpeed = this._speedRate;
                    Time.timeScale = !this._isRunning ? 0f : ((float) this._speedRate);
                }
            }
        }

        public bool IsWatching
        {
            get
            {
                return (this._workMode != WorkMode.None);
            }
        }

        public byte SpeedRate
        {
            get
            {
                return this._speedRate;
            }
            set
            {
                this._speedRate = (byte) Mathf.Clamp((int) value, (int) this.SpeedRateMin, (int) this.SpeedRateMax);
                if (this.IsWatching && this._isRunning)
                {
                    Singleton<FrameSynchr>.GetInstance().FrameSpeed = this._speedRate;
                    Time.timeScale = this._speedRate;
                }
            }
        }

        public byte SpeedRateMax
        {
            get
            {
                return 8;
            }
        }

        public byte SpeedRateMin
        {
            get
            {
                return 1;
            }
        }

        public enum WorkMode
        {
            None,
            Observe,
            Replay,
            Judge
        }
    }
}

