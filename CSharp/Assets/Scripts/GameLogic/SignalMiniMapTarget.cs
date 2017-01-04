namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_Signal_MiniMap_Target)]
    public struct SignalMiniMapTarget : ICommandImplement
    {
        public byte m_type;
        public byte m_signalID;
        public uint m_targetObjID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SignalMiniMapTarget> command = FrameCommandFactory.CreateFrameCommand<SignalMiniMapTarget>();
            command.cmdData.m_type = msg.stCmdInfo.stCmdSignalMiniMapTarget.bType;
            command.cmdData.m_signalID = msg.stCmdInfo.stCmdSignalMiniMapTarget.bSignalID;
            command.cmdData.m_targetObjID = msg.stCmdInfo.stCmdSignalMiniMapTarget.dwTargetObjID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdSignalMiniMapTarget.bType = this.m_type;
            msg.stCmdInfo.stCmdSignalMiniMapTarget.bSignalID = this.m_signalID;
            msg.stCmdInfo.stCmdSignalMiniMapTarget.dwTargetObjID = this.m_targetObjID;
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
                panel.ExecCommand_SignalMiniMap_Target(cmd.playerID, this.m_signalID, this.m_type, this.m_targetObjID);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

