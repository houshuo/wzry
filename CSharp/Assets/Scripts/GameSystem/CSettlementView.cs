namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CSettlementView
    {
        private static float _coinFrom;
        private static LTDescr _coinLTD;
        private static float _coinTo;
        private static Text _coinTweenText;
        private static GameObject _continueBtn;
        private static float _expFrom;
        private static LTDescr _expLTD;
        private static float _expTo;
        private static RectTransform _expTweenRect;
        private static uint _lvUpGrade;
        private const float expBarWidth = 327.6f;
        public const int MAX_ACHIEVEMENT = 6;
        private const float proficientBarWidth = 205f;
        private const float TweenTime = 2f;

        public static void DoCoinTweenEnd()
        {
            if ((_coinLTD != null) && (_coinTweenText != null))
            {
                _coinTweenText.text = string.Format("+{0}", _coinTo.ToString("N0"));
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(_coinTweenText, CUseable.GetMultiple(ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
                }
                _coinLTD.cancel();
                _coinLTD = null;
                _coinTweenText = null;
            }
            if (_continueBtn != null)
            {
                _continueBtn.CustomSetActive(true);
                _continueBtn = null;
            }
        }

        private static void DoExpTweenEnd()
        {
            if ((_expTweenRect != null) && (_expLTD != null))
            {
                _expTweenRect.sizeDelta = new Vector2(_expTo * 327.6f, _expTweenRect.sizeDelta.y);
                _expLTD.cancel();
                _expLTD = null;
                _expTweenRect = null;
            }
            if (_continueBtn != null)
            {
                _continueBtn.CustomSetActive(true);
                _continueBtn = null;
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

        public static void HideData(CUIFormScript form)
        {
            form.gameObject.transform.Find("PanelB/StatCon").gameObject.CustomSetActive(false);
        }

        public static void SetAchievementIcon(CUIFormScript formScript, GameObject item, PvpAchievement type, int count)
        {
            if (count <= 6)
            {
                Image component = Utility.FindChild(item, string.Format("Achievement/Image{0}", count)).GetComponent<Image>();
                if (type == PvpAchievement.NULL)
                {
                    component.gameObject.CustomSetActive(false);
                }
                else
                {
                    string prefabPath = CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + type.ToString();
                    component.gameObject.CustomSetActive(true);
                    component.SetSprite(prefabPath, formScript, true, false, false);
                }
            }
        }

        private static void SetExpInfo(GameObject root, CUIFormScript formScript)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "can't find roleinfo");
            if (masterRoleInfo != null)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) masterRoleInfo.PvpLevel));
                object[] inParameters = new object[] { masterRoleInfo.PvpLevel };
                DebugHelper.Assert(dataByKey != null, "can't find resexp id={0}", inParameters);
                if (dataByKey != null)
                {
                    root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpLevelTxt").GetComponent<Text>().text = string.Format("Lv.{0}", dataByKey.bLevel.ToString());
                    Text component = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpTxt").GetComponent<Text>();
                    Text text3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/ExpMax").GetComponent<Text>();
                    Text text4 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PlayerName").GetComponent<Text>();
                    CUIHttpImageScript script = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadImage").GetComponent<CUIHttpImageScript>();
                    Image image = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/NobeIcon").GetComponent<Image>();
                    Image image2 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/HeadFrame").GetComponent<Image>();
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
                        script.SetImageUrl(headUrl);
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                    }
                    else
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, 0, false);
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    DebugHelper.Assert(curLvelContext != null, "Battle Level Context is NULL!!");
                    GameObject gameObject = root.transform.Find("PanelA/Award/RankCon").gameObject;
                    gameObject.CustomSetActive(false);
                    if (curLvelContext.IsGameTypeLadder())
                    {
                        COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                        if (rankInfo != null)
                        {
                            gameObject.CustomSetActive(true);
                            Text text5 = gameObject.transform.FindChild(string.Format("txtRankName", new object[0])).gameObject.GetComponent<Text>();
                            Text text6 = gameObject.transform.FindChild(string.Format("WangZheXingTxt", new object[0])).gameObject.GetComponent<Text>();
                            text5.text = StringHelper.UTF8BytesToString(ref GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankInfo.bNowGrade).szGradeDesc);
                            if (rankInfo.bNowGrade == GameDataMgr.rankGradeDatabin.count)
                            {
                                Transform transform = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", 1));
                                if (transform != null)
                                {
                                    transform.gameObject.CustomSetActive(true);
                                }
                                text6.gameObject.CustomSetActive(true);
                                text6.text = string.Format("X{0}", rankInfo.dwNowScore);
                            }
                            else
                            {
                                text6.gameObject.CustomSetActive(false);
                                for (int i = 1; i <= rankInfo.dwNowScore; i++)
                                {
                                    Transform transform2 = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", i));
                                    if (transform2 != null)
                                    {
                                        transform2.gameObject.CustomSetActive(true);
                                    }
                                }
                            }
                            root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpLevelNode").gameObject.CustomSetActive(false);
                        }
                    }
                    Image image3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/QQVIPIcon").GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(image3);
                    COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                    if (multiDetail != null)
                    {
                        string[] strArray = new string[8];
                        StringBuilder builder = new StringBuilder();
                        int num2 = CUseable.GetMultiple(ref multiDetail, 15, -1);
                        if (num2 > 0)
                        {
                            COMDT_MULTIPLE_INFO comdt_multiple_info = CUseable.GetMultipleInfo(ref multiDetail, 15, -1);
                            string[] args = new string[] { num2.ToString() };
                            strArray[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", args);
                            if (comdt_multiple_info.dwPvpDailyRatio > 0)
                            {
                                string[] textArray2 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), (comdt_multiple_info.dwPvpDailyRatio / 100).ToString() };
                                strArray[1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray2);
                            }
                            if (comdt_multiple_info.dwQQVIPRatio > 0)
                            {
                                if (masterRoleInfo.HasVip(0x10))
                                {
                                    string[] textArray3 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString() };
                                    strArray[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray3);
                                }
                                else
                                {
                                    string[] textArray4 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString() };
                                    strArray[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray4);
                                }
                            }
                            if (comdt_multiple_info.dwPropRatio > 0)
                            {
                                strArray[3] = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pvp_settle_Common_Tips_4"), comdt_multiple_info.dwPropRatio / 100, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetExpExpireHours()) / 24f)));
                            }
                            if (comdt_multiple_info.dwWealRatio > 0)
                            {
                                string[] textArray5 = new string[] { (comdt_multiple_info.dwWealRatio / 100).ToString() };
                                strArray[4] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray5);
                            }
                            if (comdt_multiple_info.dwWXGameCenterLoginRatio > 0)
                            {
                                string[] textArray6 = new string[] { (comdt_multiple_info.dwWXGameCenterLoginRatio / 100).ToString() };
                                strArray[5] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray6);
                            }
                            if (comdt_multiple_info.dwQQGameCenterLoginRatio > 0)
                            {
                                string[] textArray7 = new string[] { (comdt_multiple_info.dwQQGameCenterLoginRatio / 100).ToString() };
                                strArray[6] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray7);
                            }
                            if (comdt_multiple_info.dwIOSVisitorLoginRatio > 0)
                            {
                                string[] textArray8 = new string[] { (comdt_multiple_info.dwIOSVisitorLoginRatio / 100).ToString() };
                                strArray[7] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray8);
                            }
                            builder.Append(strArray[0]);
                            for (int j = 1; j < strArray.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(strArray[j]))
                                {
                                    builder.Append("\n");
                                    builder.Append(strArray[j]);
                                }
                            }
                            GameObject obj3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").gameObject;
                            obj3.CustomSetActive(true);
                            obj3.GetComponentInChildren<Text>().text = string.Format("+{0}%", num2);
                            CUICommonSystem.SetCommonTipsEvent(formScript, obj3, builder.ToString(), enUseableTipsPos.enLeft);
                        }
                        else
                        {
                            root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/DoubleExp").gameObject.CustomSetActive(false);
                        }
                        GameObject obj5 = root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/GoldMax").gameObject;
                        if (Singleton<BattleStatistic>.GetInstance().acntInfo.bReachDailyLimit > 0)
                        {
                            obj5.CustomSetActive(true);
                        }
                        else
                        {
                            obj5.CustomSetActive(false);
                        }
                        int num4 = CUseable.GetMultiple(ref multiDetail, 11, -1);
                        if (num4 > 0)
                        {
                            COMDT_MULTIPLE_INFO comdt_multiple_info2 = CUseable.GetMultipleInfo(ref multiDetail, 11, -1);
                            strArray = new string[8];
                            string[] textArray9 = new string[] { num4.ToString() };
                            strArray[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", textArray9);
                            if (comdt_multiple_info2.dwPvpDailyRatio > 0)
                            {
                                string[] textArray10 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), (comdt_multiple_info2.dwPvpDailyRatio / 100).ToString() };
                                strArray[1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_8", textArray10);
                            }
                            if (comdt_multiple_info2.dwQQVIPRatio > 0)
                            {
                                if (masterRoleInfo.HasVip(0x10))
                                {
                                    string[] textArray11 = new string[] { (comdt_multiple_info2.dwQQVIPRatio / 100).ToString() };
                                    strArray[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray11);
                                }
                                else
                                {
                                    string[] textArray12 = new string[] { (comdt_multiple_info2.dwQQVIPRatio / 100).ToString() };
                                    strArray[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray12);
                                }
                            }
                            if (comdt_multiple_info2.dwPropRatio > 0)
                            {
                                strArray[3] = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pvp_settle_Common_Tips_10"), comdt_multiple_info2.dwPropRatio / 100, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetCoinExpireHours()) / 24f)));
                            }
                            if (comdt_multiple_info2.dwWealRatio > 0)
                            {
                                string[] textArray13 = new string[] { (comdt_multiple_info2.dwWealRatio / 100).ToString() };
                                strArray[4] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_12", textArray13);
                            }
                            if (comdt_multiple_info2.dwWXGameCenterLoginRatio > 0)
                            {
                                string[] textArray14 = new string[] { (comdt_multiple_info2.dwWXGameCenterLoginRatio / 100).ToString() };
                                strArray[5] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray14);
                            }
                            if (comdt_multiple_info2.dwQQGameCenterLoginRatio > 0)
                            {
                                string[] textArray15 = new string[] { (comdt_multiple_info2.dwQQGameCenterLoginRatio / 100).ToString() };
                                strArray[6] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray15);
                            }
                            if (comdt_multiple_info2.dwIOSVisitorLoginRatio > 0)
                            {
                                string[] textArray16 = new string[] { (comdt_multiple_info2.dwIOSVisitorLoginRatio / 100).ToString() };
                                strArray[7] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray16);
                            }
                            builder.Remove(0, builder.Length);
                            builder.Append(strArray[0]);
                            for (int k = 1; k < strArray.Length; k++)
                            {
                                if (!string.IsNullOrEmpty(strArray[k]))
                                {
                                    builder.Append("\n");
                                    builder.Append(strArray[k]);
                                }
                            }
                            GameObject obj6 = root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").gameObject;
                            obj6.CustomSetActive(true);
                            obj6.GetComponentInChildren<Text>().text = string.Format("+{0}%", num4);
                            CUICommonSystem.SetCommonTipsEvent(formScript, obj6, builder.ToString(), enUseableTipsPos.enLeft);
                        }
                        else
                        {
                            root.transform.Find("PanelA/Award/ItemAndCoin/Panel_Gold/DoubleCoin").gameObject.CustomSetActive(false);
                        }
                    }
                    text4.text = masterRoleInfo.Name;
                    RectTransform transform3 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
                    RectTransform transform4 = root.transform.Find("PanelA/Award/Panel_PlayerExp/PvpExpNode/PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    if (acntInfo != null)
                    {
                        if (acntInfo.dwPvpSettleExp > 0)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                        }
                        int num6 = (int) (acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
                        if (num6 < 0)
                        {
                            _lvUpGrade = acntInfo.dwPvpLv;
                        }
                        else
                        {
                            _lvUpGrade = 0;
                        }
                        float num7 = Mathf.Max((float) 0f, (float) (((float) num6) / ((float) dataByKey.dwNeedExp)));
                        float num8 = Mathf.Max((float) 0f, (float) (((num6 >= 0) ? ((float) acntInfo.dwPvpSettleExp) : ((float) acntInfo.dwPvpExp)) / ((float) dataByKey.dwNeedExp)));
                        root.transform.FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/AddPvpExpTxt").GetComponent<Text>().text = (acntInfo.dwPvpSettleExp <= 0) ? string.Empty : string.Format("+{0}", acntInfo.dwPvpSettleExp).ToString();
                        if (acntInfo.dwPvpSettleExp == 0)
                        {
                            root.transform.FindChild("PanelA/Award/Panel_PlayerExp/PvpExpNode/Bar2").gameObject.CustomSetActive(false);
                        }
                        transform3.sizeDelta = new Vector2(num7 * 327.6f, transform3.sizeDelta.y);
                        transform4.sizeDelta = new Vector2(num7 * 327.6f, transform4.sizeDelta.y);
                        _expFrom = num7;
                        _expTo = num7 + num8;
                        _expTweenRect = transform4;
                        transform3.gameObject.CustomSetActive(num6 >= 0);
                        text3.text = (acntInfo.bExpDailyLimit <= 0) ? string.Empty : Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit");
                        component.text = string.Format("{0}/{1}", acntInfo.dwPvpExp.ToString(), dataByKey.dwNeedExp.ToString());
                    }
                }
            }
        }

        private static void SetHeroStat_Share(CUIFormScript formScript, GameObject item, HeroKDA kda, bool bSelf, bool bMvp, bool bWin)
        {
            Utility.GetComponetInChild<Text>(item, "Txt_PlayerLevel").text = string.Format("Lv.{0}", kda.SoulLevel.ToString());
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) kda.HeroId);
            DebugHelper.Assert(dataByKey != null);
            item.transform.Find("Txt_HeroName").gameObject.GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
            string str = (kda.numKill >= 10) ? kda.numKill.ToString() : string.Format(" {0} ", kda.numKill.ToString());
            string str2 = (kda.numDead >= 10) ? kda.numDead.ToString() : string.Format(" {0} ", kda.numDead.ToString());
            string str3 = (kda.numAssist >= 10) ? kda.numAssist.ToString() : string.Format(" {0}", kda.numAssist.ToString());
            item.transform.Find("Txt_KDA").gameObject.GetComponent<Text>().text = string.Format("{0} / {1} / {2}", str, str2, str3);
            item.transform.Find("KillerImg").gameObject.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) kda.HeroId, 0)), formScript, true, false, false);
            GameObject gameObject = item.transform.Find("Mvp").gameObject;
            gameObject.CustomSetActive(bMvp);
            if (bMvp)
            {
                Image component = gameObject.GetComponent<Image>();
                if (bWin)
                {
                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Red_Mvp", formScript, true, false, false);
                    component.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                }
                else
                {
                    component.SetSprite(CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir + "Img_Icon_Blue_Mvp", formScript, true, false, false);
                    component.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                uint dwTalentID = kda.TalentArr[i].dwTalentID;
                int num12 = i + 1;
                Image image = item.transform.FindChild(string.Format("TianFu/TianFuIcon{0}", num12.ToString())).GetComponent<Image>();
                if (dwTalentID == 0)
                {
                    image.gameObject.CustomSetActive(false);
                }
                else
                {
                    image.gameObject.CustomSetActive(true);
                    ResTalentLib lib = GameDataMgr.talentLib.GetDataByKey(dwTalentID);
                    image.SetSprite(CUIUtility.s_Sprite_Dynamic_Talent_Dir + lib.dwIcon, formScript, true, false, false);
                }
            }
            int count = 1;
            for (int j = 1; j < 13; j++)
            {
                switch (((PvpAchievement) j))
                {
                    case PvpAchievement.Legendary:
                        if (kda.LegendaryNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.Legendary, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.PentaKill:
                        if (kda.PentaKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.PentaKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.QuataryKill:
                        if (kda.QuataryKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.QuataryKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.TripleKill:
                        if (kda.TripleKillNum > 0)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.TripleKill, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.DoubleKill:
                        if (kda.DoubleKillNum <= 0)
                        {
                        }
                        break;

                    case PvpAchievement.KillMost:
                        if (kda.bKillMost)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.KillMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.HurtMost:
                        if (kda.bHurtMost && (kda.hurtToEnemy > 0))
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.HurtMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.HurtTakenMost:
                        if (kda.bHurtTakenMost && (kda.hurtTakenByEnemy > 0))
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.HurtTakenMost, count);
                            count++;
                        }
                        break;

                    case PvpAchievement.AsssistMost:
                        if (kda.bAsssistMost)
                        {
                            SetAchievementIcon(formScript, item, PvpAchievement.AsssistMost, count);
                            count++;
                        }
                        break;
                }
            }
            for (int k = count; k <= 6; k++)
            {
                SetAchievementIcon(formScript, item, PvpAchievement.NULL, k);
            }
        }

        public static void SetTab(int index, GameObject root)
        {
            if (index == 0)
            {
                Utility.FindChild(root, "PanelA").CustomSetActive(true);
                Utility.FindChild(root, "PanelB").CustomSetActive(false);
            }
            else if (index == 1)
            {
                DoCoinTweenEnd();
                DoExpTweenEnd();
                Utility.FindChild(root, "PanelA").CustomSetActive(false);
                Utility.FindChild(root, "PanelB").CustomSetActive(true);
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, new uint[0]);
            }
        }

        private static void SetWin(GameObject root, bool bWin)
        {
            Utility.FindChild(root, "PanelA/WinOrLoseTitle/win").CustomSetActive(bWin);
            Utility.FindChild(root, "PanelA/WinOrLoseTitle/lose").CustomSetActive(!bWin);
            Utility.FindChild(root, "PanelB/WinOrLoseTitle/win").CustomSetActive(bWin);
            Utility.FindChild(root, "PanelB/WinOrLoseTitle/lose").CustomSetActive(!bWin);
        }

        public static void ShowData(CUIFormScript form)
        {
            form.gameObject.transform.Find("PanelB/StatCon").gameObject.CustomSetActive(true);
        }
    }
}

