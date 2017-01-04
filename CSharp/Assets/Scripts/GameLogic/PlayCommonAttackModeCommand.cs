namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYCOMMONATTACKTMODE)]
    public struct PlayCommonAttackModeCommand : ICommandImplement
    {
        public byte CommonAttackMode;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayCommonAttackModeCommand> command = FrameCommandFactory.CreateFrameCommand<PlayCommonAttackModeCommand>();
            command.cmdData.CommonAttackMode = msg.stCmdInfo.stCmdPlayCommonAttackMode.bCommonAttackMode;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayCommonAttackMode.bCommonAttackMode = this.CommonAttackMode;
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
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if (player != null)
            {
                player.SetOperateMode((OperateMode) this.CommonAttackMode);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

