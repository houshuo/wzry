namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_BUY_HORIZON_EQUIP)]
    public struct PlayerBuyHorizonEquipCommand : ICommandImplement
    {
        public ushort m_equipID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayerBuyHorizonEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerBuyHorizonEquipCommand>();
            command.cmdData.m_equipID = msg.stCmdInfo.stCmdPlayerBuyHorizonEquip.wEquipID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerBuyHorizonEquip.wEquipID = this.m_equipID;
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
                Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteBuyHorizonEquipFrameCommand(this.m_equipID, ref player.Captain);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

