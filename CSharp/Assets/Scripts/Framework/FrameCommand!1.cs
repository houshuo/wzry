namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameCommand<T> : IFrameCommand where T: struct, ICommandImplement
    {
        private uint _playerID;
        private uint _frameNum;
        private uint _cmdId;
        private byte _cmdType;
        public T cmdData;
        public uint cmdId
        {
            get
            {
                return this._cmdId;
            }
            set
            {
                this._cmdId = value;
            }
        }
        public byte cmdType
        {
            get
            {
                return this._cmdType;
            }
            set
            {
                this._cmdType = value;
            }
        }
        public uint frameNum
        {
            get
            {
                return this._frameNum;
            }
            set
            {
                this._frameNum = value;
            }
        }
        public uint playerID
        {
            get
            {
                return this._playerID;
            }
            set
            {
                this._playerID = value;
            }
        }
        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.bCmdType = this.cmdType;
            return this.cmdData.TransProtocol(ref msg);
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.bSyncType = this.cmdType;
            return this.cmdData.TransProtocol(ref msg);
        }

        public void OnReceive()
        {
            this.cmdData.OnReceive((FrameCommand<T>) this);
        }

        public void Preprocess()
        {
            this.cmdData.Preprocess((FrameCommand<T>) this);
        }

        public void ExecCommand()
        {
            this.cmdData.ExecCommand((FrameCommand<T>) this);
        }

        public void AwakeCommand()
        {
            this.cmdData.AwakeCommand((FrameCommand<T>) this);
        }

        public void Send()
        {
            if (Singleton<BattleLogic>.instance.isFighting)
            {
                this.playerID = Singleton<GamePlayerCenter>.instance.HostPlayerId;
                Singleton<FrameWindow>.GetInstance().SendGameCmd((FrameCommand<T>) this, Singleton<LobbyLogic>.instance.inMultiGame, false);
            }
        }

        public void Send(bool bUseCSSync)
        {
            if (Singleton<BattleLogic>.instance.isFighting)
            {
                this.playerID = Singleton<GamePlayerCenter>.instance.HostPlayerId;
                Singleton<FrameWindow>.GetInstance().SendGameCmd((FrameCommand<T>) this, Singleton<LobbyLogic>.instance.inMultiGame, bUseCSSync);
            }
        }
    }
}

