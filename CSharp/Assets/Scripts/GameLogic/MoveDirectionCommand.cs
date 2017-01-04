namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERMOVEDIRECTION)]
    public struct MoveDirectionCommand : ICommandImplement
    {
        public short Degree;
        public byte nSeq;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<MoveDirectionCommand> command = FrameCommandFactory.CreateFrameCommand<MoveDirectionCommand>();
            command.cmdData.Degree = msg.stCmdInfo.stCmdPlayerMoveDirection.nDegree;
            command.cmdData.nSeq = msg.stCmdInfo.stCmdPlayerMoveDirection.bMoveSeq;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerMoveDirection.nDegree = this.Degree;
            msg.stCmdInfo.stCmdPlayerMoveDirection.bMoveSeq = this.nSeq;
            return true;
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            return true;
        }

        public void OnReceive(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if ((player != null) && ActorHelper.IsHostCtrlActor(ref player.Captain))
            {
                Singleton<GameInput>.instance.OnHostActorRecvMove(this.Degree);
            }
        }

        public void Preprocess(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if ((player != null) && (player.Captain != 0))
            {
                player.Captain.handle.ActorControl.PreMoveDirection(cmd, this.Degree, this.nSeq);
            }
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if ((player != null) && (player.Captain != 0))
            {
                player.Captain.handle.ActorControl.CmdMoveDirection(cmd, this.Degree);
                if (!player.m_bMoved)
                {
                    player.m_bMoved = true;
                    Singleton<EventRouter>.instance.BroadCastEvent<Player>(EventID.FirstMoved, player);
                }
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

