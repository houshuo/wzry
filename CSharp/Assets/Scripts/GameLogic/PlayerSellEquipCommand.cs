namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_SELL_EQUIP)]
    public struct PlayerSellEquipCommand : ICommandImplement
    {
        public int m_equipIndex;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayerSellEquipCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerSellEquipCommand>();
            command.cmdData.m_equipIndex = msg.stCmdInfo.stCmdPlayerSellEquip.bEquipIndex;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerSellEquip.bEquipIndex = (byte) this.m_equipIndex;
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
                Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteSellEquipFrameCommand(this.m_equipIndex, ref player.Captain);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

