namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;

    public interface ICommandImplement
    {
        void AwakeCommand(IFrameCommand cmd);
        void ExecCommand(IFrameCommand cmd);
        void OnReceive(IFrameCommand cmd);
        void Preprocess(IFrameCommand cmd);
        bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg);
        bool TransProtocol(ref FRAME_CMD_PKG msg);
    }
}

