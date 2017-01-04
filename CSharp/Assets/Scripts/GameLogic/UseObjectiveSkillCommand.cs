namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_USEOBJECTIVESKILL)]
    public struct UseObjectiveSkillCommand : ICommandImplement
    {
        public uint ObjectID;
        public SkillSlotType SlotType;
        public int iSkillID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UseObjectiveSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
            command.cmdData.ObjectID = (uint) msg.stCSSyncDt.stObjectiveSkill.iObjectID;
            command.cmdData.SlotType = (SkillSlotType) msg.stCSSyncDt.stObjectiveSkill.chSlotType;
            command.cmdData.iSkillID = msg.stCSSyncDt.stObjectiveSkill.iSkillID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stObjectiveSkill.iObjectID = (int) this.ObjectID;
            msg.stCSSyncDt.stObjectiveSkill.chSlotType = (sbyte) this.SlotType;
            msg.stCSSyncDt.stObjectiveSkill.iSkillID = this.iSkillID;
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
                SkillUseParam param = new SkillUseParam();
                param.Init(this.SlotType, this.ObjectID);
                player.Captain.handle.ActorControl.CmdUseSkill(cmd, ref param);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

