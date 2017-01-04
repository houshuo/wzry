namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERSTOPMOVE)]
    public struct StopMoveCommand : ICommandImplement
    {
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            return FrameCommandFactory.CreateFrameCommand<StopMoveCommand>();
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
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
                Singleton<GameInput>.instance.OnHostActorRecvMove(0x7fffffff);
            }
        }

        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if ((player != null) && (player.Captain != 0))
            {
                player.Captain.handle.ActorControl.CmdStopMove();
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

