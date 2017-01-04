namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_USEPOSITIONSKILL)]
    public struct UsePositionSkillCommand : ICommandImplement
    {
        public VInt3 Position;
        public SkillSlotType SlotType;
        public int iSkillID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UsePositionSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UsePositionSkillCommand>();
            command.cmdData.SlotType = (SkillSlotType) msg.stCSSyncDt.stPositionSkill.chSlotType;
            command.cmdData.Position = CommonTools.ToVector3(msg.stCSSyncDt.stPositionSkill.stPosition);
            command.cmdData.iSkillID = msg.stCSSyncDt.stPositionSkill.iSkillID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stPositionSkill.chSlotType = (sbyte) this.SlotType;
            msg.stCSSyncDt.stPositionSkill.stPosition = CommonTools.CSDTFromVector3(this.Position);
            msg.stCSSyncDt.stPositionSkill.iSkillID = this.iSkillID;
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
                param.Init(this.SlotType, this.Position);
                player.Captain.handle.ActorControl.CmdUseSkill(cmd, ref param);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

