namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SVRNTFCHGKFRAMELATER)]
    public struct SvrNtfChgFrameLaterCommand : ICommandImplement
    {
        public byte LaterNum;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SvrNtfChgFrameLaterCommand> command = FrameCommandFactory.CreateFrameCommand<SvrNtfChgFrameLaterCommand>();
            command.cmdData.LaterNum = msg.stCmdInfo.stCmdSvrNtfChgFrameLater.bKFrameLaterNum;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdSvrNtfChgFrameLater.bKFrameLaterNum = this.LaterNum;
            return true;
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            return true;
        }

        public void OnReceive(IFrameCommand cmd)
        {
        }

        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void ExecCommand(IFrameCommand cmd)
        {
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

