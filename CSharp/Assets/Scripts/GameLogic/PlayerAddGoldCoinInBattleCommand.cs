namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE)]
    public struct PlayerAddGoldCoinInBattleCommand : ICommandImplement
    {
        public uint m_addValue;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<PlayerAddGoldCoinInBattleCommand> command = FrameCommandFactory.CreateFrameCommand<PlayerAddGoldCoinInBattleCommand>();
            command.cmdData.m_addValue = msg.stCmdInfo.stCmdPlayerAddGoldCoinInBattle.dwAddValue;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerAddGoldCoinInBattle.dwAddValue = this.m_addValue;
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
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            bool flag = (curLvelContext != null) && curLvelContext.IsMobaMode();
            Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
            if (((player != null) && ((flag && player.isGM) || (!flag && LobbyMsgHandler.isHostGMAcnt))) && ((player.Captain != 0) && (player.Captain.handle.ValueComponent != null)))
            {
                player.Captain.handle.ValueComponent.ChangeGoldCoinInBattle((int) this.m_addValue, true, true, new Vector3(), false);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

