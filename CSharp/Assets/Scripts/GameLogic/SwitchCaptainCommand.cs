namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SWITCHCAPTAIN)]
    public struct SwitchCaptainCommand : ICommandImplement
    {
        public uint ObjectID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SwitchCaptainCommand> command = FrameCommandFactory.CreateFrameCommand<SwitchCaptainCommand>();
            command.cmdData.ObjectID = (uint) msg.stCmdInfo.stCmdPlayerSwitchCaptain.iObjectID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerSwitchCaptain.iObjectID = (int) this.ObjectID;
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
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(this.ObjectID);
                if (actor != 0)
                {
                    player.Captain.handle.HudControl.SetSelected(false);
                    player.Captain.handle.ActorControl.SetSelected(false);
                    bool isAutoAI = player.Captain.handle.ActorControl.m_isAutoAI;
                    player.Captain.handle.ActorControl.SetAutoAI(true);
                    player.SetCaptain((uint) actor.handle.TheActorMeta.ConfigId);
                    player.Captain.handle.HudControl.SetSelected(true);
                    player.Captain.handle.ActorControl.SetSelected(true);
                    player.Captain.handle.ActorControl.SetAutoAI(isAutoAI);
                    DefaultGameEventParam prm = new DefaultGameEventParam(actor, actor);
                    Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, ref prm);
                }
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

