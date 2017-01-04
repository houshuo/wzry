namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_Signal_MiniMap_Position)]
    public struct SignalMiniMapPosition : ICommandImplement
    {
        public byte m_signalID;
        public VInt3 m_worldPos;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SignalMiniMapPosition> command = FrameCommandFactory.CreateFrameCommand<SignalMiniMapPosition>();
            command.cmdData.m_signalID = msg.stCmdInfo.stCmdSignalMiniMapPosition.bSignalID;
            command.cmdData.m_worldPos = CommonTools.ToVector3(msg.stCmdInfo.stCmdSignalMiniMapPosition.stWorldPos);
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdSignalMiniMapPosition.bSignalID = this.m_signalID;
            msg.stCmdInfo.stCmdSignalMiniMapPosition.stWorldPos = CommonTools.FromVector3(this.m_worldPos);
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
            SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
            if (panel != null)
            {
                panel.ExecCommand_SignalMiniMap_Position(cmd.playerID, this.m_signalID, ref this.m_worldPos);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

