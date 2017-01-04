namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERRECONNECT)]
    public struct SvrReconnectCommand : ICommandImplement
    {
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            return FrameCommandFactory.CreateFrameCommand<SvrReconnectCommand>();
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
        }

        public void Preprocess(IFrameCommand cmd)
        {
        }

        public void ExecCommand(IFrameCommand cmd)
        {
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if (((player != null) && (player.Captain != 0)) && ActorHelper.IsHostCampActor(ref player.Captain))
            {
                KillDetailInfo info = new KillDetailInfo {
                    Killer = player.Captain,
                    bSelfCamp = true,
                    Type = KillDetailInfoType.Info_Type_Reconnect
                };
                Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
                Singleton<EventRouter>.instance.BroadCastEvent<bool, uint>(EventID.DisConnectNtf, false, cmd.playerID);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

