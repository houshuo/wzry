namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_ATTACKPOSITION)]
    public struct AttackPositionCommand : ICommandImplement
    {
        public VInt3 WorldPos;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<AttackPositionCommand> command = FrameCommandFactory.CreateFrameCommand<AttackPositionCommand>();
            command.cmdData.WorldPos = CommonTools.ToVector3(msg.stCmdInfo.stCmdPlayerAttackPosition.stWorldPos);
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerAttackPosition.stWorldPos = CommonTools.FromVector3(this.WorldPos);
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
            if ((player != null) && (player.Captain != 0))
            {
                player.Captain.handle.ActorControl.CmdAttackMoveToDest(cmd, this.WorldPos);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

