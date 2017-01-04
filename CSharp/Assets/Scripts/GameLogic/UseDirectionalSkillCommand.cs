namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_USEDIRECTIONALSKILL)]
    public struct UseDirectionalSkillCommand : ICommandImplement
    {
        public VInt3 Direction;
        public SkillSlotType SlotType;
        public int iSkillID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            FrameCommand<UseDirectionalSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseDirectionalSkillCommand>();
            command.cmdData.Direction = CommonTools.ToVector3(msg.stCSSyncDt.stDirectionSkill.stDirection);
            command.cmdData.SlotType = (SkillSlotType) msg.stCSSyncDt.stDirectionSkill.chSlotType;
            command.cmdData.iSkillID = msg.stCSSyncDt.stDirectionSkill.iSkillID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            return true;
        }

        public bool TransProtocol(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            msg.stCSSyncDt.stDirectionSkill.stDirection = CommonTools.CSDTFromVector3(this.Direction);
            msg.stCSSyncDt.stDirectionSkill.chSlotType = (sbyte) this.SlotType;
            msg.stCSSyncDt.stDirectionSkill.iSkillID = this.iSkillID;
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
                param.Init(this.SlotType, this.Direction, false);
                player.Captain.handle.ActorControl.CmdUseSkill(cmd, ref param);
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

