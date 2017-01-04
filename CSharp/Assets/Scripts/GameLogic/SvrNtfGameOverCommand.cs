namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SVRNTF_GAMEOVER)]
    public struct SvrNtfGameOverCommand : ICommandImplement
    {
        public byte m_bWinCamp;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SvrNtfGameOverCommand> command = FrameCommandFactory.CreateFrameCommand<SvrNtfGameOverCommand>();
            command.cmdData.m_bWinCamp = msg.stCmdInfo.stCmdSvrNtfGameover.bWinCamp;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdSvrNtfGameover.bWinCamp = this.m_bWinCamp;
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
            Singleton<BattleLogic>.instance.DealGameSurrender(this.m_bWinCamp);
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

