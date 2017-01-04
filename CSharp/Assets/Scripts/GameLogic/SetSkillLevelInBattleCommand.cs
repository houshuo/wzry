namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SET_SKILL_LEVEL)]
    public struct SetSkillLevelInBattleCommand : ICommandImplement
    {
        public byte SkillSlot;
        public byte SkillLevel;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<SetSkillLevelInBattleCommand> command = FrameCommandFactory.CreateFrameCommand<SetSkillLevelInBattleCommand>();
            command.cmdData.SkillSlot = msg.stCmdInfo.stCmdSetSkillLevel.bSkillSlot;
            command.cmdData.SkillLevel = msg.stCmdInfo.stCmdSetSkillLevel.bSkillLevel;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdSetSkillLevel.bSkillSlot = this.SkillSlot;
            msg.stCmdInfo.stCmdSetSkillLevel.bSkillLevel = this.SkillLevel;
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
            if ((((player != null) && player.isGM) && ((player.Captain != 0) && (player.Captain.handle.SkillControl != null))) && (this.SkillSlot < 8))
            {
                player.Captain.handle.SkillControl.SkillSlotArray[this.SkillSlot].SetSkillLevel(this.SkillLevel);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

