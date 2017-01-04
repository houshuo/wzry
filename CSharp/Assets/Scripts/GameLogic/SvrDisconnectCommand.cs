namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERDISCONNECT)]
    public struct SvrDisconnectCommand : ICommandImplement
    {
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            return FrameCommandFactory.CreateFrameCommand<SvrDisconnectCommand>();
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
            Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(cmd.playerID);
            if ((player != null) && (player.Captain != 0))
            {
                if (ActorHelper.IsHostCampActor(ref player.Captain))
                {
                    KillDetailInfo info = new KillDetailInfo {
                        Killer = player.Captain,
                        bSelfCamp = true,
                        Type = KillDetailInfoType.Info_Type_Disconnect
                    };
                    Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, info);
                    Singleton<EventRouter>.instance.BroadCastEvent<bool, uint>(EventID.DisConnectNtf, true, cmd.playerID);
                }
                player.Captain.handle.ActorControl.SetOffline(true);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

