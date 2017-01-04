namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;

    public interface IFrameCommand
    {
        void AwakeCommand();
        void ExecCommand();
        void OnReceive();
        void Preprocess();
        void Send();
        bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg);
        bool TransProtocol(ref FRAME_CMD_PKG msg);

        uint cmdId { get; set; }

        byte cmdType { get; set; }

        uint frameNum { get; set; }

        uint playerID { get; set; }
    }
}

