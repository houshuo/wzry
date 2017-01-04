namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_LEARNSKILL)]
    public struct LearnSkillCommand : ICommandImplement
    {
        public uint dwHeroID;
        public byte bSlotType;
        public byte bSkillLevel;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<LearnSkillCommand> command = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
            command.cmdData.dwHeroID = msg.stCmdInfo.stCmdPlayerLearnSkill.dwHeroObjID;
            command.cmdData.bSlotType = msg.stCmdInfo.stCmdPlayerLearnSkill.bSlotType;
            command.cmdData.bSkillLevel = msg.stCmdInfo.stCmdPlayerLearnSkill.bSkillLevel;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerLearnSkill.dwHeroObjID = this.dwHeroID;
            msg.stCmdInfo.stCmdPlayerLearnSkill.bSlotType = this.bSlotType;
            msg.stCmdInfo.stCmdPlayerLearnSkill.bSkillLevel = this.bSkillLevel;
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
            if (player != null)
            {
                ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = player.GetAllHeroes();
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = allHeroes[i];
                    if (handle.handle.ObjID == this.dwHeroID)
                    {
                        PoolObjHandle<ActorRoot> handle2 = allHeroes[i];
                        handle2.handle.ActorControl.CmdCommonLearnSkill(cmd);
                        break;
                    }
                }
            }
        }

        public void AwakeCommand(IFrameCommand cmd)
        {
        }
    }
}

