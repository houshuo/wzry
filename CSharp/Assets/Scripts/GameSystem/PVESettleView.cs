namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class PVESettleView
    {
        private static float _coinFrom;
        private static LTDescr _coinLTD;
        private static COMDT_REWARD_MULTIPLE_DETAIL _coinMulti;
        private static float _coinTo;
        private static Text _coinTweenText;
        private static GameObject _continueBtn1;
        private static GameObject _continueBtn2;
        private static float _expFrom;
        private static LTDescr _expLTD;
        private static float _expTo;
        private static RectTransform _expTweenRect;
        private static uint _lvUpGrade;
        [CompilerGenerated]
        private static Action<float> <>f__am$cacheC;
        [CompilerGenerated]
        private static Action<float> <>f__am$cacheD;
        private const float expBarWidth = 260f;
        public const string REWARD_ANIM_1_NAME = "Box_Show_2";
        public const string REWARD_ANIM_2_NAME = "AppearThePrizes_2";
        public const string STAR_WIN_ANIM_NAME = "Win_Show";
        private const float TweenTime = 2f;

        private static void DoCoinAndExpTween()
        {
            if ((_expTweenRect != null) && (_expTweenRect.gameObject != null))
            {
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = delegate (float value) {
                        if ((_expTweenRect != null) && (_expTweenRect.gameObject != null))
                        {
                            _expTweenRect.sizeDelta = new Vector2(value * 260f, _expTweenRect.sizeDelta.y);
                            if (value >= _expTo)
                            {
                                DoExpTweenEnd();
                            }
                        }
                    };
                }
                _expLTD = LeanTween.value(_expTweenRect.gameObject, <>f__am$cacheC, _expFrom, _expTo, 2f);
            }
            if ((_coinTweenText != null) && (_coinTweenText.gameObject != null))
            {
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = delegate (float value) {
                        if ((_coinTweenText != null) && (_coinTweenText.gameObject != null))
                        {
                            _coinTweenText.text = string.Format("+{0}", value.ToString("N0"));
                            if (value >= _coinTo)
                            {
                                DoCoinTweenEnd();
                            }
                        }
                    };
                }
                _coinLTD = LeanTween.value(_coinTweenText.gameObject, <>f__am$cacheD, _coinFrom, _coinTo, 2f);
            }
        }

        public static void DoCoinTweenEnd()
        {
            if ((_coinLTD != null) && (_coinTweenText != null))
            {
                _coinTweenText.text = string.Format("+{0}", _coinTo.ToString("N0"));
                if (_coinMulti != null)
                {
                    CUICommonSystem.AppendMultipleText(_coinTweenText, CUseable.GetMultiple(ref _coinMulti, 0, -1));
                }
                _coinLTD.cancel();
                _coinLTD = null;
                _coinTweenText = null;
                _coinMulti = null;
            }
        }

        private static void DoExpTweenEnd()
        {
            if ((_expTweenRect != null) && (_expLTD != null))
            {
                _expTweenRect.sizeDelta = new Vector2(_expTo * 260f, _expTweenRect.sizeDelta.y);
                _expLTD.cancel();
                _expLTD = null;
                _expTweenRect = null;
            }
            if (_continueBtn1 != null)
            {
                _continueBtn1.CustomSetActive(true);
                _continueBtn1 = null;
            }
            if (_continueBtn2 != null)
            {
                _continueBtn2.CustomSetActive(true);
                _continueBtn2 = null;
            }
            if (_lvUpGrade > 1)
            {
                CUIEvent event3 = new CUIEvent {
                    m_eventID = enUIEventID.Settle_OpenLvlUp
                };
                event3.m_eventParams.tag = ((int) _lvUpGrade) - 1;
                event3.m_eventParams.tag2 = (int) _lvUpGrade;
                CUIEvent uiEvent = event3;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            _lvUpGrade = 0;
        }

        private static void GoToStarAnimState(CUIFormScript starForm, ref Assets.Scripts.GameSystem.StarCondition[] starArr, string animSuffix = "")
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < 3; i++)
            {
                if (starArr[i].bCompelete)
                {
                    if (i == 1)
                    {
                        flag2 = true;
                    }
                    if (i == 2)
                    {
                        flag3 = true;
                    }
                }
                if (!starArr[i].bCompelete && (i == 0))
                {
                    flag = true;
                }
            }
            GameObject gameObject = starForm.transform.Find("Root").gameObject;
            if (flag)
            {
                CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_3{0}", animSuffix));
            }
            else if (flag2)
            {
                if (flag3)
                {
                    CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_3{0}", animSuffix));
                }
                else
                {
                    CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_2{0}", animSuffix));
                }
            }
            else if (flag3)
            {
                CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_1_3{0}", animSuffix));
            }
            else
            {
                CUICommonSystem.PlayAnimator(gameObject, string.Format("Star_1{0}", animSuffix));
            }
        }

        public static void OnStarWinAnimEnd(CUIFormScript starForm, ref Assets.Scripts.GameSystem.StarCondition[] starArr)
        {
            GoToStarAnimState(starForm, ref starArr, string.Empty);
        }

        public static void SetExpFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                new PVEPlayerItem(form.transform.Find("Root/Panel_Exp/Exp_Player").gameObject).addExp(settleData.stAcntInfo.dwSettleExp);
                CUI3DImageScript component = form.transform.Find("Root/3DImage").gameObject.GetComponent<CUI3DImageScript>();
                DebugHelper.Assert(component != null);
                int num = 1;
                for (int i = 0; i < settleData.stHeroList.bNum; i++)
                {
                    CHeroInfo info2;
                    uint dwHeroConfID = settleData.stHeroList.astHeroList[i].dwHeroConfID;
                    GameObject gameObject = form.transform.Find(string.Format("Root/Panel_Exp/Exp_Hero{0}", num)).gameObject;
                    if (masterRoleInfo.GetHeroInfoDic().TryGetValue(dwHeroConfID, out info2))
                    {
                        ResHeroCfgInfo cfgInfo = info2.cfgInfo;
                        PVEHeroItem item2 = new PVEHeroItem(gameObject, cfgInfo.dwCfgID);
                        if (num <= settleData.stHeroList.bNum)
                        {
                            gameObject.CustomSetActive(true);
                            item2.addExp(settleData.stHeroList.astHeroList[num - 1].dwSettleExp);
                            int heroWearSkinId = (int) masterRoleInfo.GetHeroWearSkinId(cfgInfo.dwCfgID);
                            string objectName = CUICommonSystem.GetHeroPrefabPath(cfgInfo.dwCfgID, heroWearSkinId, true).ObjectName;
                            GameObject model = component.AddGameObjectToPath(objectName, false, string.Format("_root/Hero{0}", num));
                            CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                            instance.Set3DModel(model);
                            instance.InitAnimatList();
                            instance.InitAnimatSoundList(cfgInfo.dwCfgID, (uint) heroWearSkinId);
                            instance.OnModePlayAnima("idleshow2");
                        }
                    }
                    num++;
                }
            }
        }

        public static void SetItemEtcCell(CUIFormScript form, GameObject item, Text name, COMDT_REWARD_INFO rewardInfo, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            CUseable itemUseable = null;
            switch (rewardInfo.bType)
            {
                case 1:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, 0L, rewardInfo.stRewardInfo.stItem.dwItemID, (int) rewardInfo.stRewardInfo.stItem.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(rewardInfo.stRewardInfo.stItem.dwItemID);
                    if (dataByKey != null)
                    {
                        name.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                    }
                    break;
                }
                case 3:
                    itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, (int) rewardInfo.stRewardInfo.dwCoupons);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    name.text = itemUseable.m_name;
                    break;

                case 4:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0L, rewardInfo.stRewardInfo.stEquip.dwEquipID, (int) rewardInfo.stRewardInfo.stEquip.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    ResEquipInfo info2 = GameDataMgr.equipInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stEquip.dwEquipID);
                    if (info2 != null)
                    {
                        name.text = StringHelper.UTF8BytesToString(ref info2.szName);
                    }
                    break;
                }
                case 5:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, 0L, rewardInfo.stRewardInfo.stHero.dwHeroID, (int) rewardInfo.stRewardInfo.stHero.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(rewardInfo.stRewardInfo.stHero.dwHeroID);
                    if (info3 != null)
                    {
                        name.text = StringHelper.UTF8BytesToString(ref info3.szName);
                    }
                    break;
                }
                case 6:
                {
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0L, rewardInfo.stRewardInfo.stSymbol.dwSymbolID, (int) rewardInfo.stRewardInfo.stSymbol.dwCnt, 0);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    ResSymbolInfo info4 = GameDataMgr.symbolInfoDatabin.GetDataByKey(rewardInfo.stRewardInfo.stSymbol.dwSymbolID);
                    if (info4 != null)
                    {
                        name.text = StringHelper.UTF8BytesToString(ref info4.szName);
                    }
                    break;
                }
                case 0x10:
                    itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int) rewardInfo.stRewardInfo.dwDiamond);
                    itemUseable.SetMultiple(ref settleData.stMultipleDetail, true);
                    CUICommonSystem.SetItemCell(form, item, itemUseable, true, false);
                    name.text = itemUseable.m_name;
                    break;
            }
        }

        public static void SetRewardFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSceneBgPath, form);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                GameObject gameObject = form.transform.Find("Root/Panel_Interactable/Button_Next").gameObject;
                if (curLvelContext.IsGameTypeActivity())
                {
                    gameObject.CustomSetActive(false);
                }
                else
                {
                    int levelId = CAdventureSys.GetNextLevelId(curLvelContext.m_chapterNo, curLvelContext.m_levelNo, curLvelContext.m_levelDifficulty);
                    if (levelId != 0)
                    {
                        if (Singleton<CAdventureSys>.GetInstance().IsLevelOpen(levelId))
                        {
                            gameObject.CustomSetActive(true);
                        }
                        else
                        {
                            gameObject.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        gameObject.CustomSetActive(false);
                    }
                }
                gameObject.CustomSetActive(false);
                Show3DModel(form);
                GameObject obj3 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty1").gameObject;
                GameObject obj4 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty2").gameObject;
                GameObject obj5 = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaDifficulty3").gameObject;
                Text component = form.transform.Find("Root/Panel_Award/Award/Panel_GuanKa/GuanKaName").gameObject.GetComponent<Text>();
                if (curLvelContext.m_levelDifficulty == 1)
                {
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                }
                else if (curLvelContext.m_levelDifficulty == 2)
                {
                    obj3.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                }
                else if (curLvelContext.m_levelDifficulty == 3)
                {
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
                component.text = string.Format(curLvelContext.m_levelName, new object[0]);
                _continueBtn1 = form.transform.Find("Root/Panel_Interactable/Button_Once").gameObject;
                _continueBtn2 = form.transform.Find("Root/Panel_Interactable/Button_ReturnLobby").gameObject;
                _continueBtn1.CustomSetActive(true);
                _continueBtn2.CustomSetActive(true);
                ShowReward(form, settleData);
                CUICommonSystem.PlayAnimator(form.gameObject, "Box_Show_2");
            }
        }

        public static void SetStarFormData(CUIFormScript form, COMDT_SETTLE_RESULT_DETAIL settleData, ref Assets.Scripts.GameSystem.StarCondition[] starArr)
        {
            GameObject gameObject = form.transform.Find("Root").gameObject;
            int num = 0;
            for (int i = 1; i < 4; i++)
            {
                GameObject obj3 = form.transform.Find(string.Format("Root/Condition{0}", i)).gameObject;
                Text component = obj3.transform.Find("Condition_text").gameObject.GetComponent<Text>();
                component.text = starArr[i - 1].ConditionName;
                if (!starArr[i - 1].bCompelete)
                {
                    string name = string.Empty;
                    if (i == 2)
                    {
                        name = "Condition_Star1";
                    }
                    else
                    {
                        name = "Condition_Star";
                    }
                    obj3.transform.Find(name).gameObject.CustomSetActive(false);
                    component.color = CUIUtility.s_Color_Grey;
                }
                else
                {
                    num++;
                }
            }
            for (int j = 1; j < 4; j++)
            {
                if (num < j)
                {
                    form.transform.Find(string.Format("Root/Panel_Star/Star{0}", j)).gameObject.CustomSetActive(false);
                }
            }
            CUICommonSystem.PlayAnimator(gameObject, "Win_Show");
        }

        private static void Show3DModel(CUIFormScript belongForm)
        {
            CUI3DImageScript component = null;
            Transform transform = belongForm.transform.Find("Root/Panel_Award/3DImage");
            if (transform != null)
            {
                component = transform.GetComponent<CUI3DImageScript>();
            }
            if (component != null)
            {
                PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                if (hostKDA != null)
                {
                    ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                    uint heroId = 0;
                    while (enumerator.MoveNext())
                    {
                        HeroKDA current = enumerator.Current;
                        if (current != null)
                        {
                            heroId = (uint) current.HeroId;
                            break;
                        }
                    }
                    int heroWearSkinId = (int) Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(heroId);
                    ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroId, heroWearSkinId, true);
                    GameObject model = component.AddGameObject(data.ObjectName, false, false);
                    CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                    instance.Set3DModel(model);
                    if (model != null)
                    {
                        instance.InitAnimatList();
                        instance.InitAnimatSoundList(heroId, (uint) heroWearSkinId);
                    }
                }
            }
        }

        public static void ShowPlayerLevelUp(CUIFormScript form, int oldLvl, int newLvl)
        {
            <ShowPlayerLevelUp>c__AnonStorey81 storey = new <ShowPlayerLevelUp>c__AnonStorey81 {
                newLvl = newLvl
            };
            if (form != null)
            {
                <ShowPlayerLevelUp>c__AnonStorey82 storey2 = new <ShowPlayerLevelUp>c__AnonStorey82 {
                    <>f__ref$129 = storey
                };
                GameObject gameObject = form.transform.Find("PlayerLvlUp").gameObject;
                gameObject.transform.Find("bg/TxtPlayerLvl").gameObject.GetComponent<Text>().text = storey.newLvl.ToString();
                gameObject.transform.Find("bg/TxtPlayerBeforeLvl").gameObject.GetComponent<Text>().text = oldLvl.ToString();
                object[] inParameters = new object[] { oldLvl };
                DebugHelper.Assert(GameDataMgr.acntExpDatabin.GetDataByKey((uint) oldLvl) != null, "Can't find acnt exp config -- level {0}", inParameters);
                object[] objArray2 = new object[] { storey.newLvl };
                DebugHelper.Assert(GameDataMgr.acntExpDatabin.GetDataByKey((uint) storey.newLvl) != null, "Can't find acnt exp config -- level {0}", objArray2);
                Transform transform = gameObject.transform.Find("Panel/groupPanel/symbolPosPanel");
                int symbolPosOpenCnt = CSymbolInfo.GetSymbolPosOpenCnt(oldLvl);
                int num2 = CSymbolInfo.GetSymbolPosOpenCnt(storey.newLvl);
                storey2.hasBuy = false;
                storey2.master = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if ((storey2.master != null) && (symbolPosOpenCnt < num2))
                {
                    GameDataMgr.symbolPosDatabin.Accept(new Action<ResSymbolPos>(storey2.<>m__7F));
                }
                transform.gameObject.CustomSetActive(!storey2.hasBuy && (num2 > symbolPosOpenCnt));
                if (!storey2.hasBuy && (num2 > symbolPosOpenCnt))
                {
                    transform.Find("curCntText").gameObject.GetComponent<Text>().text = symbolPosOpenCnt.ToString();
                    transform.Find("levelUpCntText").gameObject.GetComponent<Text>().text = num2.ToString();
                }
                Transform transform2 = gameObject.transform.Find("Panel/groupPanel/symbolLevelPanel");
                int symbolLvlLimit = CSymbolInfo.GetSymbolLvlLimit(oldLvl);
                int num4 = CSymbolInfo.GetSymbolLvlLimit(storey.newLvl);
                transform2.gameObject.CustomSetActive(num4 > symbolLvlLimit);
                if (num4 > symbolLvlLimit)
                {
                    Text component = transform2.Find("curCntText").gameObject.GetComponent<Text>();
                    Text text6 = transform2.Find("levelUpCntText").gameObject.GetComponent<Text>();
                    component.text = symbolLvlLimit.ToString();
                    text6.text = num4.ToString();
                }
                Transform transform3 = gameObject.transform.Find("Panel/groupPanel/symbolPageCntPanel");
                ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((long) storey.newLvl);
                if (dataByKey != null)
                {
                    transform3.gameObject.CustomSetActive(dataByKey.bPresentSymbolPage > 0);
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && ((dataByKey.bPresentSymbolPage > 0) && (masterRoleInfo != null)))
                    {
                        Text text7 = transform3.Find("curCntText").gameObject.GetComponent<Text>();
                        Text text8 = transform3.Find("levelUpCntText").gameObject.GetComponent<Text>();
                        text7.text = (masterRoleInfo.m_symbolInfo.m_pageCount - 1).ToString();
                        text8.text = masterRoleInfo.m_symbolInfo.m_pageCount.ToString();
                    }
                }
            }
        }

        private static void ShowReward(CUIFormScript belongForm, COMDT_SETTLE_RESULT_DETAIL settleData)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                COMDT_REWARD_INFO comdt_reward_info;
                GameObject gameObject = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/Panel_Gold").gameObject;
                Text component = gameObject.transform.Find("GoldNum").gameObject.GetComponent<Text>();
                GameObject obj3 = gameObject.transform.Find("GoldMax").gameObject;
                if (settleData.stAcntInfo.bReachDailyLimit > 0)
                {
                    obj3.CustomSetActive(true);
                }
                else
                {
                    obj3.CustomSetActive(false);
                }
                component.text = "0";
                COMDT_REWARD_DETAIL stReward = settleData.stReward;
                COMDT_ACNT_INFO stAcntInfo = settleData.stAcntInfo;
                if (stAcntInfo != null)
                {
                    GameObject obj4 = belongForm.transform.FindChild("Root/Panel_Award/Award/Panel_PlayerExp/PvpExpNode").gameObject;
                    Text text2 = obj4.transform.FindChild("PvpExpTxt").gameObject.GetComponent<Text>();
                    Text target = obj4.transform.FindChild("AddPvpExpTxt").gameObject.GetComponent<Text>();
                    RectTransform transform = obj4.transform.FindChild("PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
                    RectTransform transform2 = obj4.transform.FindChild("PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
                    Text text4 = obj4.transform.FindChild("PlayerName").gameObject.GetComponent<Text>();
                    CUIHttpImageScript script = obj4.transform.FindChild("HeadImage").gameObject.GetComponent<CUIHttpImageScript>();
                    Text text5 = obj4.transform.FindChild("PvpLevelTxt").gameObject.GetComponent<Text>();
                    Image image = obj4.transform.FindChild("NobeIcon").gameObject.GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                    Image image2 = obj4.transform.FindChild("HeadFrame").gameObject.GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                    text5.text = string.Format("Lv.{0}", stAcntInfo.dwPvpLv.ToString());
                    ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) stAcntInfo.dwPvpLv));
                    GameObject obj5 = obj4.transform.FindChild("ExpMax").gameObject;
                    if (stAcntInfo.bExpDailyLimit == 0)
                    {
                        obj5.CustomSetActive(false);
                    }
                    text2.text = string.Format("{0}/{1}", stAcntInfo.dwPvpExp, dataByKey.dwNeedExp);
                    target.text = string.Format("+{0}", stAcntInfo.dwPvpSettleExp);
                    CUICommonSystem.AppendMultipleText(target, CUseable.GetMultiple(ref settleData.stMultipleDetail, 15, -1));
                    text4.text = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name;
                    string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
                    if (!CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        script.SetImageUrl(headUrl);
                    }
                    if (stAcntInfo.dwPvpSettleExp > 0)
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                    }
                    float num = 0f;
                    if (stAcntInfo.dwPvpExp < stAcntInfo.dwPvpSettleExp)
                    {
                        transform.sizeDelta = new Vector2(num * 260f, transform.sizeDelta.y);
                        _lvUpGrade = stAcntInfo.dwPvpLv;
                    }
                    else
                    {
                        num = ((float) (stAcntInfo.dwPvpExp - stAcntInfo.dwPvpSettleExp)) / ((float) dataByKey.dwNeedExp);
                        transform.sizeDelta = new Vector2(num * 260f, transform.sizeDelta.y);
                        _lvUpGrade = 0;
                    }
                    float num2 = ((float) stAcntInfo.dwPvpExp) / ((float) dataByKey.dwNeedExp);
                    _expFrom = num;
                    _expTo = num2;
                    transform2.sizeDelta = new Vector2(num * 260f, transform2.sizeDelta.y);
                    _expTweenRect = transform2;
                    _coinFrom = 0f;
                    _coinTo = 0f;
                    for (int j = 0; j < stReward.bNum; j++)
                    {
                        comdt_reward_info = stReward.astRewardDetail[j];
                        if (comdt_reward_info.bType == 11)
                        {
                            _coinTo = comdt_reward_info.stRewardInfo.dwPvpCoin;
                            _coinMulti = settleData.stMultipleDetail;
                        }
                    }
                    _coinTweenText = component;
                    DoCoinAndExpTween();
                }
                ListView<COMDT_REWARD_INFO> view = new ListView<COMDT_REWARD_INFO>();
                GameObject obj6 = belongForm.transform.Find("Root/Panel_Award/Award/Panel_QQVIPGold").gameObject;
                if (obj6 != null)
                {
                    obj6.CustomSetActive(false);
                }
                GameObject obj7 = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/FirstGain").gameObject;
                if (obj7 != null)
                {
                    obj7.CustomSetActive(false);
                }
                for (int i = 0; i < stReward.bNum; i++)
                {
                    comdt_reward_info = stReward.astRewardDetail[i];
                    switch (comdt_reward_info.bType)
                    {
                        case 6:
                            view.Add(stReward.astRewardDetail[i]);
                            if (obj7 != null)
                            {
                                obj7.CustomSetActive(false);
                            }
                            break;

                        case 11:
                            CUICommonSystem.AppendMultipleText(component, CUseable.GetMultiple(ref settleData.stMultipleDetail, 0, -1));
                            if (obj6 != null)
                            {
                                obj6.CustomSetActive(false);
                                Text text6 = obj6.transform.FindChild("Text_Value").gameObject.GetComponent<Text>();
                                GameObject obj8 = obj6.transform.FindChild("Icon_QQVIP").gameObject;
                                GameObject obj9 = obj6.transform.FindChild("Icon_QQSVIP").gameObject;
                                obj8.CustomSetActive(false);
                                obj9.CustomSetActive(false);
                                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                uint num5 = CUseable.GetQqVipExtraCoin(comdt_reward_info.stRewardInfo.dwPvpCoin, ref settleData.stMultipleDetail, 0);
                                if ((masterRoleInfo != null) && (num5 > 0))
                                {
                                    text6.text = string.Format("+{0}", num5);
                                    if (masterRoleInfo.HasVip(0x10))
                                    {
                                        obj6.CustomSetActive(true);
                                        obj9.CustomSetActive(true);
                                    }
                                    else if (masterRoleInfo.HasVip(1))
                                    {
                                        obj6.CustomSetActive(true);
                                        obj8.CustomSetActive(true);
                                    }
                                }
                                obj6.CustomSetActive(false);
                            }
                            break;
                    }
                }
                GameObject obj10 = belongForm.transform.Find("Root/Panel_Award/Award/ItemAndCoin/itemCell").gameObject;
                obj10.CustomSetActive(false);
                if (view.Count > 0)
                {
                    Text name = obj10.transform.FindChild("ItemName").gameObject.GetComponent<Text>();
                    obj10.CustomSetActive(true);
                    comdt_reward_info = view[0];
                    SetItemEtcCell(belongForm, obj10, name, comdt_reward_info, settleData);
                }
            }
        }

        public static void StopExpAnim(CUIFormScript expForm)
        {
            if (expForm.transform.Find("Root/EscapeAnim").gameObject.activeSelf)
            {
                expForm.transform.Find("Root/EscapeAnim").gameObject.CustomSetActive(false);
                expForm.transform.Find("Root/Panel_Interactable").gameObject.CustomSetActive(true);
            }
        }

        public static void StopRewardAnim(CUIFormScript rewardForm)
        {
            if ((rewardForm != null) && rewardForm.transform.Find("Root/EscapeAnim").gameObject.activeSelf)
            {
                rewardForm.transform.Find("Root/EscapeAnim").gameObject.CustomSetActive(false);
                rewardForm.transform.Find("Root/Panel_Interactable").gameObject.CustomSetActive(true);
                rewardForm.gameObject.GetComponent<Animator>().Play("AppearThePrizes_2_Done");
                Singleton<PVESettleSys>.instance.OnAwardDisplayEnd();
            }
        }

        public static void StopStarAnim(CUIFormScript starForm)
        {
            if (starForm.transform.Find("EscapeAnim").gameObject.activeSelf)
            {
                starForm.transform.Find("EscapeAnim").gameObject.CustomSetActive(false);
                starForm.transform.Find("Panel_Interactable").gameObject.CustomSetActive(true);
                Assets.Scripts.GameSystem.StarCondition[] condition = Singleton<PVESettleSys>.GetInstance().GetCondition();
                GoToStarAnimState(starForm, ref condition, "_Done");
            }
        }

        [CompilerGenerated]
        private sealed class <ShowPlayerLevelUp>c__AnonStorey81
        {
            internal int newLvl;
        }

        [CompilerGenerated]
        private sealed class <ShowPlayerLevelUp>c__AnonStorey82
        {
            internal PVESettleView.<ShowPlayerLevelUp>c__AnonStorey81 <>f__ref$129;
            internal bool hasBuy;
            internal CRoleInfo master;

            internal void <>m__7F(ResSymbolPos rule)
            {
                if ((rule != null) && (rule.wOpenLevel == this.<>f__ref$129.newLvl))
                {
                    this.hasBuy = this.master.m_symbolInfo.IsGridPosHasBuy(rule.bSymbolPos);
                }
            }
        }

        public enum AwardWidgets
        {
            ItemDetailPanel = 1,
            None = -1,
            Reserve = 0
        }
    }
}

