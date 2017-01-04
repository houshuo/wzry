namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYATTACKTARGETMODE)]
    public struct PlayAttackTargetModeCommand : ICommandImplement
    {
        public sbyte AttackTargetMode;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayAttackTargetModeCommand> command = FrameCommandFactory.CreateFrameCommand<PlayAttackTargetModeCommand>();
            command.cmdData.AttackTargetMode = msg.stCmdInfo.stCmdPlayerAttackTargetMode.chAttackTargetMode;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerAttackTargetMode.chAttackTargetMode = this.AttackTargetMode;
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
                player.AttackTargetMode = (SelectEnemyType) this.AttackTargetMode;
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

