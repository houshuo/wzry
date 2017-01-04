namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    internal class CUILoadingSystem : Singleton<CUILoadingSystem>
    {
        private static int _pvpLoadingIndex = 1;
        private static CUIFormScript _singlePlayerLoading;
        private const uint MAX_PLAYER_NUM = 5;
        public static string PVE_PATH_LOADING = "UGUI/Form/System/PvE/Adv/Form_Adv_Loading.prefab";
        public static string PVP_PATH_LOADING = "UGUI/Form/System/PvP/Loading/Form_Loading.prefab";
        private static int SoloRandomNum = 1;

        private static int GenerateMultiRandomNum()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x5e).dwConfValue;
            int num2 = _pvpLoadingIndex;
            _pvpLoadingIndex++;
            if (_pvpLoadingIndex > dwConfValue)
            {
                _pvpLoadingIndex = 1;
            }
            return num2;
        }

        private static string GenerateRandomPveLoadingTips(int randNum)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", randNum));
            if (string.IsNullOrEmpty(text))
            {
                text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVE_Tips_{0}", 0));
            }
            return text;
        }

        private static string GenerateRandomPvpLoadingTips(int randNum)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", randNum));
            if (string.IsNullOrEmpty(text))
            {
                text = Singleton<CTextManager>.GetInstance().GetText(string.Format("Loading_PVP_Tips_{0}", 0));
            }
            return text;
        }

        private static int GenerateSoloRandomNum()
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x5d).dwConfValue;
            if ((SoloRandomNum > dwConfValue) || (SoloRandomNum < 4))
            {
                SoloRandomNum = 4;
            }
            return SoloRandomNum++;
        }

        private static GameObject GetMemberItem(uint ObjId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVP_PATH_LOADING);
            if (form != null)
            {
                List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.PlayerId == ObjId)
                    {
                        int num = enumerator.Current.CampPos + 1;
                        return ((enumerator.Current.PlayerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? form.gameObject.transform.FindChild("DownPanel").FindChild(string.Format("Down_Player{0}", num)).gameObject : form.gameObject.transform.FindChild("UpPanel").FindChild(string.Format("Up_Player{0}", num)).gameObject);
                    }
                }
            }
            return null;
        }

        private static Player GetPlayer(COM_PLAYERCAMP Camp, int Pos)
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(Camp).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (Pos == (enumerator.Current.CampPos + 1))
                {
                    return enumerator.Current;
                }
            }
            return null;
        }

        private static uint GetPlayerId(COM_PLAYERCAMP Camp, int Pos)
        {
            List<Player>.Enumerator enumerator = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if ((enumerator.Current.PlayerCamp == Camp) && (Pos == (enumerator.Current.CampPos + 1)))
                {
                    return enumerator.Current.PlayerId;
                }
            }
            return 0;
        }

        public void HideLoading()
        {
            if (Singleton<LobbyLogic>.instance.inMultiGame)
            {
                HideMultiLoading();
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    if (curLvelContext.IsMobaModeWithOutGuide())
                    {
                        HideMultiLoading();
                    }
                    else
                    {
                        this.HidePveLoading();
                    }
                }
            }
        }

        private static void HideMultiLoading()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PVP_PATH_LOADING);
        }

        private void HidePveLoading()
        {
            _singlePlayerLoading = null;
            Singleton<CUIManager>.GetInstance().CloseForm(PVE_PATH_LOADING);
        }

        public override void Init()
        {
            base.Init();
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.onGamePrepareFight));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.onGameStartFight));
            Singleton<GameEventSys>.instance.AddEventHandler(GameEventDef.Event_MultiRecoverFin, new System.Action(this.onGameRecoverFin));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ADVANCE_STOP_LOADING, new System.Action(this.HideLoading));
            Singleton<GameEventSys>.instance.AddEventHandler<PreDialogStartedEventParam>(GameEventDef.Event_PreDialogStarted, new RefAction<PreDialogStartedEventParam>(this.OnPreDialogStarted));
        }

        private void onGamePrepareFight(ref DefaultGameEventParam prm)
        {
            if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
            }
        }

        private void onGameRecoverFin()
        {
            this.HideLoading();
        }

        private void onGameStartFight(ref DefaultGameEventParam prm)
        {
            if (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
            {
                this.HideLoading();
            }
        }

        [MessageHandler(0x43c)]
        public static void OnMultiGameLoadProcess(CSPkg msg)
        {
            GameObject memberItem = GetMemberItem(msg.stPkgData.stMultGameLoadProcessRsp.dwObjId);
            if (memberItem != null)
            {
                GameObject gameObject = memberItem.transform.Find("Txt_LoadingPct").gameObject;
                if (gameObject != null)
                {
                    gameObject.GetComponent<Text>().text = string.Format("{0}%", msg.stPkgData.stMultGameLoadProcessRsp.wProcess);
                }
            }
        }

        private void OnPreDialogStarted(ref PreDialogStartedEventParam eventParam)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (((curLvelContext != null) && (curLvelContext.m_preDialogId > 0)) && (curLvelContext.m_preDialogId == eventParam.PreDialogId))
            {
                this.HideLoading();
            }
        }

        public static void OnSelfLoadProcess(float progress)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                if (curLvelContext.IsMobaModeWithOutGuide())
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        GameObject memberItem = GetMemberItem(hostPlayer.PlayerId);
                        if (memberItem != null)
                        {
                            Transform transform = memberItem.transform.Find("Txt_LoadingPct");
                            if (transform != null)
                            {
                                transform.GetComponent<Text>().text = string.Format("{0}%", Convert.ToUInt16((float) (progress * 100f)));
                            }
                        }
                    }
                    if (curLvelContext.m_isWarmBattle)
                    {
                        CFakePvPHelper.FakeLoadProcess(progress);
                    }
                }
                else if (_singlePlayerLoading != null)
                {
                    _singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().text = string.Format("{0}%", (int) (Mathf.Clamp(progress, 0f, 1f) * 100f));
                    _singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(Mathf.Clamp(progress, 0f, 1f));
                }
            }
        }

        public void ShowLoading()
        {
            Singleton<BurnExpeditionController>.instance.Clear();
            MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
            Singleton<CUIManager>.GetInstance().CloseAllForm(null, true, true);
            if (Singleton<LobbyLogic>.instance.inMultiGame)
            {
                ShowMultiLoading();
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext != null)
                {
                    if (curLvelContext.IsMobaModeWithOutGuide())
                    {
                        ShowMultiLoading();
                    }
                    else
                    {
                        this.ShowPveLoading();
                    }
                }
            }
        }

        private static void ShowMultiLoading()
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(PVP_PATH_LOADING, false, false);
            if (formScript != null)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                GameObject widget = formScript.GetWidget(0);
                if (widget != null)
                {
                    if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
                    {
                        widget.CustomSetActive(true);
                    }
                    else
                    {
                        widget.CustomSetActive(false);
                    }
                }
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                IGameActorDataProvider provider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                ActorStaticData actorData = new ActorStaticData();
                ActorMeta actorMeta = new ActorMeta();
                ActorMeta meta2 = new ActorMeta();
                ActorServerData data2 = new ActorServerData();
                actorMeta.ActorType = ActorTypeDef.Actor_Type_Hero;
                string name = null;
                for (int i = 1; i <= 2; i++)
                {
                    List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers((COM_PLAYERCAMP) i);
                    if (allCampPlayers != null)
                    {
                        Transform transform = (i != 1) ? formScript.transform.FindChild("DownPanel") : formScript.transform.FindChild("UpPanel");
                        for (int j = 1; j <= 5L; j++)
                        {
                            name = (i != 1) ? string.Format("Down_Player{0}", j) : string.Format("Up_Player{0}", j);
                            transform.FindChild(name).gameObject.CustomSetActive(false);
                        }
                        List<Player>.Enumerator enumerator = allCampPlayers.GetEnumerator();
                        COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                        while (enumerator.MoveNext())
                        {
                            Player current = enumerator.Current;
                            if (current != null)
                            {
                                name = (i != 1) ? string.Format("Down_Player{0}", current.CampPos + 1) : string.Format("Up_Player{0}", current.CampPos + 1);
                                GameObject gameObject = transform.FindChild(name).gameObject;
                                gameObject.CustomSetActive(true);
                                GameObject obj5 = gameObject.transform.Find("RankFrame").gameObject;
                                bool flag = ((current.PlayerCamp == playerCamp) || Singleton<WatchController>.GetInstance().IsWatching) || Singleton<WatchController>.GetInstance().IsReplaying;
                                if (obj5 != null)
                                {
                                    if (flag)
                                    {
                                        string rankFrameIconPath = CLadderView.GetRankFrameIconPath((byte) current.GradeOfRank, current.ClassOfRank);
                                        if (string.IsNullOrEmpty(rankFrameIconPath))
                                        {
                                            obj5.CustomSetActive(false);
                                        }
                                        else
                                        {
                                            Image image = obj5.GetComponent<Image>();
                                            if (image != null)
                                            {
                                                image.SetSprite(rankFrameIconPath, formScript, true, false, false);
                                            }
                                            obj5.CustomSetActive(true);
                                        }
                                    }
                                    else
                                    {
                                        obj5.CustomSetActive(false);
                                    }
                                }
                                Transform transform2 = gameObject.transform.Find("RankClassText");
                                if (transform2 != null)
                                {
                                    GameObject obj6 = transform2.gameObject;
                                    if (flag && CLadderView.IsSuperKing((byte) current.GradeOfRank, current.ClassOfRank))
                                    {
                                        obj6.CustomSetActive(true);
                                        obj6.GetComponent<Text>().text = current.ClassOfRank.ToString();
                                    }
                                    else
                                    {
                                        obj6.CustomSetActive(false);
                                    }
                                }
                                Text component = gameObject.transform.Find("Txt_PlayerName/Name").gameObject.GetComponent<Text>();
                                component.text = current.Name;
                                Image image2 = gameObject.transform.Find("Txt_PlayerName/NobeIcon").gameObject.GetComponent<Image>();
                                if (image2 != null)
                                {
                                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image2, (int) current.VipLv, true);
                                }
                                Text text2 = gameObject.transform.Find("Txt_HeroName").gameObject.GetComponent<Text>();
                                actorMeta.ConfigId = (int) current.CaptainId;
                                text2.text = !actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData) ? null : actorData.TheResInfo.Name;
                                if ((Singleton<WatchController>.GetInstance().IsWatching && (current.PlayerUId == Singleton<WatchController>.GetInstance().TargetUID)) || (!Singleton<WatchController>.GetInstance().IsWatching && (current.PlayerId == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId)))
                                {
                                    component.color = CUIUtility.s_Text_Color_Self;
                                }
                                else
                                {
                                    component.color = CUIUtility.s_Text_Color_CommonGray;
                                }
                                GameObject obj7 = gameObject.transform.Find("Txt_LoadingPct").gameObject;
                                if (obj7 != null)
                                {
                                    Text text3 = obj7.GetComponent<Text>();
                                    if (current.Computer)
                                    {
                                        text3.text = !curLvelContext.m_isWarmBattle ? "100%" : "1%";
                                    }
                                    else
                                    {
                                        text3.text = (!MonoSingleton<Reconnection>.instance.isProcessingRelayRecover && !Singleton<WatchController>.GetInstance().IsWatching) ? "1%" : "100%";
                                    }
                                }
                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(current.CaptainId);
                                if (dataByKey != null)
                                {
                                    meta2.PlayerId = current.PlayerId;
                                    meta2.ActorCamp = (COM_PLAYERCAMP) i;
                                    meta2.ConfigId = (int) dataByKey.dwCfgID;
                                    meta2.ActorType = ActorTypeDef.Actor_Type_Hero;
                                    Image image3 = gameObject.transform.Find("Hero").gameObject.GetComponent<Image>();
                                    if (provider2.GetActorServerData(ref meta2, ref data2))
                                    {
                                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin((uint) data2.TheActorMeta.ConfigId, data2.SkinId);
                                        if (heroSkin != null)
                                        {
                                            image3.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID), formScript, true, false, false);
                                            if (heroSkin.dwSkinID > 0)
                                            {
                                                text2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("LoadingSkinNameTxt"), heroSkin.szSkinName, heroSkin.szHeroName);
                                            }
                                        }
                                        bool bActive = (((current.PlayerCamp == playerCamp) || Singleton<WatchController>.GetInstance().IsWatching) || Singleton<WatchController>.GetInstance().IsReplaying) && (curLvelContext.m_isWarmBattle || !current.Computer);
                                        Transform transform3 = gameObject.transform.Find("heroProficiencyBgImg");
                                        if (transform3 != null)
                                        {
                                            transform3.gameObject.CustomSetActive(bActive);
                                            if (bActive)
                                            {
                                                CUICommonSystem.SetHeroProficiencyBgImage(formScript, transform3.gameObject, (int) data2.TheProficiencyInfo.Level, true);
                                            }
                                        }
                                        Transform transform4 = gameObject.transform.Find("heroProficiencyImg");
                                        if (transform4 != null)
                                        {
                                            transform4.gameObject.CustomSetActive(bActive);
                                            if (bActive)
                                            {
                                                CUICommonSystem.SetHeroProficiencyIconImage(formScript, transform4.gameObject, (int) data2.TheProficiencyInfo.Level);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        image3.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), formScript, true, false, false);
                                    }
                                    uint skillID = 0;
                                    if (provider2.GetActorServerCommonSkillData(ref meta2, out skillID) && (skillID != 0))
                                    {
                                        ResSkillCfgInfo info2 = GameDataMgr.skillDatabin.GetDataByKey(skillID);
                                        if (info2 != null)
                                        {
                                            gameObject.transform.Find("SelSkill").gameObject.CustomSetActive(true);
                                            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(info2.szIconPath));
                                            gameObject.transform.Find("SelSkill/Icon").GetComponent<Image>().SetSprite(prefabPath, formScript.GetComponent<CUIFormScript>(), true, false, false);
                                        }
                                        else
                                        {
                                            gameObject.transform.Find("SelSkill").gameObject.CustomSetActive(false);
                                        }
                                    }
                                    else
                                    {
                                        gameObject.transform.Find("SelSkill").gameObject.CustomSetActive(false);
                                    }
                                    Transform transform5 = gameObject.transform.Find("skinLabelImage");
                                    if (transform5 != null)
                                    {
                                        CUICommonSystem.SetHeroSkinLabelPic(formScript, transform5.gameObject, dataByKey.dwCfgID, data2.SkinId);
                                    }
                                }
                            }
                        }
                    }
                }
                formScript.gameObject.transform.FindChild("Img_Tiao").FindChild("Tips").gameObject.GetComponent<Text>().text = GenerateRandomPvpLoadingTips(GenerateMultiRandomNum());
            }
        }

        private void ShowPveLoading()
        {
            _singlePlayerLoading = Singleton<CUIManager>.GetInstance().OpenForm(PVE_PATH_LOADING, false, true);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            int randNum = 0;
            if (curLvelContext != null)
            {
                if (curLvelContext.IsGameTypeGuide())
                {
                    if (curLvelContext.m_mapID == 1)
                    {
                        randNum = 1;
                    }
                    else if (curLvelContext.m_mapID == 3)
                    {
                        randNum = 2;
                    }
                    else if (curLvelContext.m_mapID == 8)
                    {
                        randNum = 3;
                    }
                    else
                    {
                        randNum = GenerateSoloRandomNum();
                    }
                }
                else
                {
                    randNum = GenerateSoloRandomNum();
                }
            }
            _singlePlayerLoading.m_formWidgets[1].GetComponent<Text>().text = GenerateRandomPveLoadingTips(randNum);
            _singlePlayerLoading.m_formWidgets[2].GetComponent<Text>().text = string.Format("{0}%", 0);
            _singlePlayerLoading.m_formWidgets[3].GetComponent<Image>().CustomFillAmount(0f);
            Image component = _singlePlayerLoading.m_formWidgets[4].GetComponent<Image>();
            if (randNum >= 4)
            {
                MonoSingleton<BannerImageSys>.GetInstance().TrySetLoadingImage((uint) randNum, component);
            }
            else
            {
                component.SetSprite(string.Format("{0}{1}{2}", "UGUI/Sprite/Dynamic/", "Loading/LoadingNotice", randNum), _singlePlayerLoading, true, false, false);
            }
        }

        public enum SinglePlayerLoadingFormWidget
        {
            LoadingNotice = 4,
            LoadingProgress = 2,
            LoadingProgressBar = 3,
            None = -1,
            Reserve = 0,
            Tips = 1
        }
    }
}

