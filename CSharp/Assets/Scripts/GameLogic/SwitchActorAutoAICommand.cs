namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SWITCHAOUTAI)]
    public struct SwitchActorAutoAICommand : ICommandImplement
    {
        public sbyte IsAutoAI;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SwitchActorAutoAICommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
            command.cmdData.IsAutoAI = msg.stCmdInfo.stCmdPlayerSwithAutoAI.chIsAutoAI;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerSwithAutoAI.chIsAutoAI = this.IsAutoAI;
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
                player.Captain.handle.ActorControl.SetAutoAI(this.IsAutoAI != 0);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

