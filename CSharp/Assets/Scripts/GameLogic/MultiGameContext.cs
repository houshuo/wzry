namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using UnityEngine;

    public class MultiGameContext : GameContextBase
    {
        public SCPKG_MULTGAME_BEGINLOAD MessageRef;

        public MultiGameContext(SCPKG_MULTGAME_BEGINLOAD InMessage)
        {
            this.MessageRef = InMessage;
            if (CheatCommandReplayEntry.heroPerformanceTest)
            {
                this.InitPerformanceTest();
            }
            uint dwMapId = this.MessageRef.stDeskInfo.dwMapId;
            byte bMapType = this.MessageRef.stDeskInfo.bMapType;
            base.LevelContext = CLevelCfgLogicManager.CreatePvpLevelContext(bMapType, dwMapId, (COM_GAME_TYPE) InMessage.bGameType, 1);
            base.LevelContext.m_isWarmBattle = Convert.ToBoolean(InMessage.stDeskInfo.bIsWarmBattle);
            base.LevelContext.SetWarmHeroAiDiff(InMessage.stDeskInfo.bAILevel);
            Singleton<GameLogic>.GetInstance().HashCheckFreq = InMessage.dwHaskChkFreq;
            FrameTracer.Initial(InMessage.dwCltLogMask, InMessage.dwCltLogSize);
        }

        public override GameInfoBase CreateGameInfo()
        {
            PVPMobaGame game = new PVPMobaGame();
            game.Initialize(this);
            return game;
        }

        private void InitPerformanceTest()
        {
            for (int i = 0; i < this.MessageRef.astCampInfo.Length; i++)
            {
                CSDT_CAMPINFO csdt_campinfo = this.MessageRef.astCampInfo[i];
                COMDT_HERO_COMMON_INFO stCommonInfo = csdt_campinfo.astCampPlayerInfo[0].stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo;
                uint dwHeroID = stCommonInfo.dwHeroID;
                ushort wSkinID = stCommonInfo.wSkinID;
                for (int j = 0; j < csdt_campinfo.dwPlayerNum; j++)
                {
                    CSDT_CAMPPLAYERINFO csdt_campplayerinfo = csdt_campinfo.astCampPlayerInfo[j];
                    if (csdt_campplayerinfo != null)
                    {
                        for (int k = 0; k < csdt_campplayerinfo.stPlayerInfo.astChoiceHero.Length; k++)
                        {
                            COMDT_CHOICEHERO comdt_choicehero = csdt_campplayerinfo.stPlayerInfo.astChoiceHero[k];
                            if (comdt_choicehero != null)
                            {
                                comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID = dwHeroID;
                                comdt_choicehero.stBaseInfo.stCommonInfo.wSkinID = wSkinID;
                            }
                        }
                    }
                }
            }
        }

        public void SaveServerData()
        {
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            if (this.MessageRef != null)
            {
                for (int i = 0; i < this.MessageRef.astCampInfo.Length; i++)
                {
                    CSDT_CAMPINFO csdt_campinfo = this.MessageRef.astCampInfo[i];
                    int num2 = Mathf.Min(csdt_campinfo.astCampPlayerInfo.Length, (int) csdt_campinfo.dwPlayerNum);
                    for (int j = 0; j < num2; j++)
                    {
                        COMDT_PLAYERINFO stPlayerInfo = csdt_campinfo.astCampPlayerInfo[j].stPlayerInfo;
                        Singleton<ActorDataCenter>.instance.AddHeroesServerData(stPlayerInfo.dwObjId, stPlayerInfo.astChoiceHero);
                    }
                }
            }
        }
    }
}

