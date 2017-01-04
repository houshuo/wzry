namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_LEARNTALENT)]
    public struct LearnTalentCommand : ICommandImplement
    {
        public uint HeroObjID;
        public sbyte TalentLevelIndex;
        public uint TalentID;
        [FrameCommandCreator]
        public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
        {
            FrameCommand<LearnTalentCommand> command = FrameCommandFactory.CreateFrameCommand<LearnTalentCommand>();
            command.cmdData.HeroObjID = msg.stCmdInfo.stCmdPlayerLearnTalent.dwHeroObjID;
            command.cmdData.TalentLevelIndex = msg.stCmdInfo.stCmdPlayerLearnTalent.chTalentLevel;
            command.cmdData.TalentID = msg.stCmdInfo.stCmdPlayerLearnTalent.dwTalentID;
            return command;
        }

        public bool TransProtocol(ref FRAME_CMD_PKG msg)
        {
            msg.stCmdInfo.stCmdPlayerLearnTalent.dwHeroObjID = this.HeroObjID;
            msg.stCmdInfo.stCmdPlayerLearnTalent.chTalentLevel = this.TalentLevelIndex;
            msg.stCmdInfo.stCmdPlayerLearnTalent.dwTalentID = this.TalentID;
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
                    if (handle.handle.ObjID == this.HeroObjID)
                    {
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

