namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CLadderView
    {
        public static string LADDER_IMG_PATH = "UGUI/Sprite/System/Ladder/";
        public static string LADDER_IMG_STAR = (LADDER_IMG_PATH + "Img_CompetitiveRace_Staropen.prefab");
        public static string LADDER_IMG_STAR_EMPTY = (LADDER_IMG_PATH + "Img_CompetitiveRace_Staroff.prefab");
        public const int MAX_MOST_USED_HERO_NUM = 4;
        public const int MAX_RECENT_GAME_SHOW_NUM = 10;
        public static string RANK_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder/";
        public static string RANK_SMALL_ICON_PATH = "UGUI/Sprite/Dynamic/Ladder_Small/";
        public static Color s_Bg_Color_Expand = new Color(0.3176471f, 0.3803922f, 0.5607843f, 0.5f);
        public static Color s_Bg_Color_Shrink = new Color(0.1372549f, 0.1803922f, 0.282353f, 0.5f);

        private static int ComparisonHeroData(COMDT_RANK_COMMON_USED_HERO a, COMDT_RANK_COMMON_USED_HERO b)
        {
            if (a.dwFightCnt > b.dwFightCnt)
            {
                return -1;
            }
            if (a.dwFightCnt < b.dwFightCnt)
            {
                return 1;
            }
            return 0;
        }

        private static string GetGameTime(ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
        {
            DateTime time = Utility.ToUtcTime2Local((long) data.dwFightTime);
            object[] args = new object[] { time.Month.ToString("00"), time.Day.ToString("00"), time.Hour.ToString("00"), time.Minute.ToString("00") };
            return string.Format(Singleton<CTextManager>.GetInstance().GetText("GameTime_Template"), args);
        }

        public static string GetRankBigGradeName(byte rankBigGrade)
        {
            <GetRankBigGradeName>c__AnonStorey84 storey = new <GetRankBigGradeName>c__AnonStorey84 {
                rankBigGrade = rankBigGrade
            };
            ResRankGradeConf conf = GameDataMgr.rankGradeDatabin.FindIf(new Func<ResRankGradeConf, bool>(storey.<>m__82));
            if ((conf != null) && !string.IsNullOrEmpty(conf.szBigGradeName))
            {
                return conf.szBigGradeName;
            }
            return string.Empty;
        }

        public static string GetRankFrameIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if ((dataByKey == null) || string.IsNullOrEmpty(dataByKey.szGradeFramePicPath))
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + dataByKey.szGradeFramePicPathSuperMaster);
            }
            return (RANK_ICON_PATH + dataByKey.szGradeFramePicPath);
        }

        public static string GetRankIconPath()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                return GetRankIconPath(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
            }
            return string.Empty;
        }

        public static string GetRankIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePathSuperMaster));
            }
            return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePath));
        }

        public static string GetRankSmallIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePathSuperMaster));
            }
            return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradePicturePath));
        }

        private static string GetRankTitle(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
        {
            return GetRankTitle(data.bGrade, data.dwClassOfRank);
        }

        public static string GetRankTitle(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return Singleton<CTextManager>.GetInstance().GetText("Ladder_Super_Master");
            }
            return StringHelper.UTF8BytesToString(ref dataByKey.szGradeDesc);
        }

        private static string GetSeasonNameWithBracket(ulong time)
        {
            string ladderSeasonName = Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(time);
            return (!string.IsNullOrEmpty(ladderSeasonName) ? ("(" + ladderSeasonName + ")") : string.Empty);
        }

        private static string GetSeasonText(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data)
        {
            DateTime time = Utility.ToUtcTime2Local((long) data.dwSeaStartTime);
            DateTime time2 = Utility.ToUtcTime2Local((long) data.dwSeaEndTime);
            object[] args = new object[] { time.Year, time.Month, time2.Year, time2.Month };
            return string.Format(Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration"), args);
        }

        private static string GetSeasonText(ref COMDT_RANKDETAIL data)
        {
            if (data.bState == 1)
            {
                DateTime time = Utility.ToUtcTime2Local((long) data.dwSeasonStartTime);
                DateTime time2 = Utility.ToUtcTime2Local((long) data.dwSeasonEndTime);
                object[] args = new object[] { time.Year, time.Month, time2.Year, time2.Month };
                return string.Format(Singleton<CTextManager>.GetInstance().GetText("ladder_season_duration"), args);
            }
            return "赛季还未开始";
        }

        public static string GetSubRankIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPathSuperMaster));
            }
            return (RANK_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPath));
        }

        public static string GetSubRankSmallIconPath(byte rankGrade, uint rankClass)
        {
            ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (IsSuperMaster(rankGrade, rankClass))
            {
                return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPathSuperMaster));
            }
            return (RANK_SMALL_ICON_PATH + StringHelper.UTF8BytesToString(ref dataByKey.szGradeSmallPicPath));
        }

        private static string GetTopUseHeroNames(ref COMDT_RANK_PASTSEASON_FIGHT_RECORD data, out List<COMDT_RANK_COMMON_USED_HERO> heroList)
        {
            heroList = new List<COMDT_RANK_COMMON_USED_HERO>();
            for (int i = 0; i < data.dwCommonUsedHeroNum; i++)
            {
                if (data.astCommonUsedHeroInfo[i].dwHeroId != 0)
                {
                    heroList.Add(data.astCommonUsedHeroInfo[i]);
                }
            }
            heroList.Sort(new Comparison<COMDT_RANK_COMMON_USED_HERO>(CLadderView.ComparisonHeroData));
            StringBuilder builder = new StringBuilder();
            int num2 = (heroList.Count <= 4) ? heroList.Count : 4;
            for (int j = 0; j < num2; j++)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroList[j].dwHeroId);
                if (dataByKey != null)
                {
                    builder.Append(StringHelper.UTF8BytesToString(ref dataByKey.szName));
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }

        public static void InitKingForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
        {
            ShowRankDetail(form.transform.Find("RankCon").gameObject, ref data, false);
        }

        public static void InitLadderEntry(CUIFormScript form, ref COMDT_RANKDETAIL data, bool isQualified)
        {
            Transform transform = form.transform.Find("MainPanel/SingleStart");
            Transform transform2 = form.transform.Find("MainPanel/DoubleStart");
            Button btn = null;
            Button component = null;
            if (transform != null)
            {
                btn = transform.GetComponent<Button>();
            }
            if (transform2 != null)
            {
                component = transform2.GetComponent<Button>();
            }
            GameObject widget = form.GetWidget(7);
            if (isQualified)
            {
                form.transform.Find("StatPanel").gameObject.CustomSetActive(true);
                form.transform.Find("ReqPanel").gameObject.CustomSetActive(false);
                form.transform.Find("MainPanel/ImgLock").gameObject.CustomSetActive(false);
                form.transform.Find("MainPanel/RankCon").gameObject.CustomSetActive(true);
                GameObject gameObject = form.transform.Find("StatPanel/SeasonCon").gameObject;
                GameObject obj4 = form.transform.Find("StatPanel/lblNoRecord").gameObject;
                GameObject go = form.transform.Find("MainPanel/RankCon").gameObject;
                Text text = form.transform.Find("StatPanel/SeasonCon/txtGameNum").GetComponent<Text>();
                Text text2 = form.transform.Find("StatPanel/SeasonCon/txtWinNum").GetComponent<Text>();
                Text text3 = form.transform.Find("StatPanel/SeasonCon/txtContiWinNum").GetComponent<Text>();
                Text text4 = form.transform.Find("StatPanel/txtLeagueTime").GetComponent<Text>();
                if (data != null)
                {
                    if (gameObject != null)
                    {
                        gameObject.CustomSetActive(data.dwTotalFightCnt != 0);
                    }
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(data.dwTotalFightCnt == 0);
                    }
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(btn, data.bState == 1, true);
                    }
                    if (component != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(component, data.bState == 1, true);
                    }
                    if (text != null)
                    {
                        text.text = data.dwTotalFightCnt.ToString();
                    }
                    if (text2 != null)
                    {
                        text2.text = data.dwTotalWinCnt.ToString();
                    }
                    if (text3 != null)
                    {
                        text3.text = data.dwMaxContinuousWinCnt.ToString();
                    }
                    if (text4 != null)
                    {
                        text4.text = GetSeasonText(ref data);
                    }
                    if (widget != null)
                    {
                        widget.gameObject.CustomSetActive(true);
                        Text local1 = widget.GetComponent<Text>();
                        local1.text = local1.text + " " + GetSeasonNameWithBracket((ulong) data.dwSeasonStartTime);
                    }
                    ShowRankDetail(go, ref data, false);
                    int num = (data.bMaxSeasonGrade - 1) / 3;
                    string strContent = Singleton<CTextManager>.instance.GetText("Ladder_Award_Tips_" + num);
                    Image image = form.transform.Find("AwardGroup/award1").GetComponent<Image>();
                    image.SetSprite(CUIUtility.s_Sprite_System_Ladder_Dir + "award" + num, form, true, false, false);
                    CUICommonSystem.SetCommonTipsEvent(form, image.gameObject, strContent, enUseableTipsPos.enLeft);
                    form.transform.Find("AwardGroup").gameObject.CustomSetActive(true);
                }
                else
                {
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                    }
                    if (component != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(component, false, true);
                    }
                    form.transform.Find("AwardGroup").gameObject.CustomSetActive(false);
                }
            }
            else
            {
                if (btn != null)
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                }
                if (component != null)
                {
                    CUICommonSystem.SetButtonEnableWithShader(component, false, true);
                }
                form.transform.Find("AwardGroup").gameObject.CustomSetActive(false);
                form.transform.Find("StatPanel").gameObject.CustomSetActive(false);
                form.transform.Find("ReqPanel").gameObject.CustomSetActive(true);
                form.transform.Find("MainPanel/ImgLock").gameObject.CustomSetActive(true);
                form.transform.Find("MainPanel/RankCon").gameObject.CustomSetActive(false);
                Text text6 = form.transform.Find("ReqPanel/txtHeroNum").GetComponent<Text>();
                Text text7 = form.transform.Find("ReqPanel/txtReqHeroNum").GetComponent<Text>();
                int haveHeroCount = 0;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    haveHeroCount = masterRoleInfo.GetHaveHeroCount(false);
                }
                text6.text = string.Format("{0}/{1}", haveHeroCount, CLadderSystem.REQ_HERO_NUM);
                text7.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Ladder_Req_Hero_Num"), CLadderSystem.REQ_HERO_NUM.ToString());
                if (widget != null)
                {
                    widget.gameObject.CustomSetActive(false);
                }
            }
            ShowBpModePanel(form);
            ShowContinuousWinPanel(form, data, isQualified);
            ShowSuperKingRankPanel(form);
        }

        public static void InitLadderHistory(CUIFormScript form, List<COMDT_RANK_PASTSEASON_FIGHT_RECORD> dataList)
        {
            CUIListScript component = form.transform.Find("ExpandList").GetComponent<CUIListScript>();
            if (dataList == null)
            {
                component.SetElementAmount(0);
            }
            else
            {
                component.SetElementAmount(dataList.Count);
                CUIListElementScript elemenet = null;
                for (int i = 0; i < dataList.Count; i++)
                {
                    elemenet = component.GetElemenet(i);
                    COMDT_RANK_PASTSEASON_FIGHT_RECORD data = dataList[i];
                    Text text = elemenet.transform.Find("Title/txtLeagueTime").GetComponent<Text>();
                    Text text2 = elemenet.transform.Find("Title/txtRankTitle").GetComponent<Text>();
                    Text text3 = elemenet.transform.Find("Title/txtHeroes").GetComponent<Text>();
                    Text text4 = elemenet.transform.Find("Expand/txtGameNum").GetComponent<Text>();
                    Text text5 = elemenet.transform.Find("Expand/txtWinNum").GetComponent<Text>();
                    Text text6 = elemenet.transform.Find("Expand/txtWinRate").GetComponent<Text>();
                    Text text7 = elemenet.transform.Find("Expand/txtContiWinNum").GetComponent<Text>();
                    text.text = GetSeasonNameWithBracket((ulong) data.dwSeaStartTime) + " " + GetSeasonText(ref data);
                    text2.text = GetRankTitle(ref data);
                    List<COMDT_RANK_COMMON_USED_HERO> heroList = new List<COMDT_RANK_COMMON_USED_HERO>();
                    text3.text = GetTopUseHeroNames(ref data, out heroList);
                    text4.text = data.dwTotalFightCnt.ToString();
                    text5.text = data.dwTotalWinCnt.ToString();
                    text6.text = (data.dwTotalFightCnt <= 0) ? "0.00%" : string.Format("{0}%", ((data.dwTotalWinCnt * 100f) / ((float) data.dwTotalFightCnt)).ToString("0.00"));
                    text7.text = data.dwMaxContinuousWinCnt.ToString();
                    int num2 = (heroList.Count <= 4) ? heroList.Count : 4;
                    for (int j = 0; j < num2; j++)
                    {
                        Transform item = elemenet.transform.Find(string.Format("Expand/Hero{0}", j + 1));
                        item.gameObject.CustomSetActive(true);
                        COMDT_RANK_COMMON_USED_HERO comdt_rank_common_used_hero = heroList[j];
                        SetMostUsedHero(item, ref comdt_rank_common_used_hero, form);
                    }
                    for (int k = num2; k < 4; k++)
                    {
                        elemenet.transform.Find(string.Format("Expand/Hero{0}", k + 1)).gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public static void InitLadderRecent(CUIFormScript form, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
        {
            CUIListScript component = form.transform.Find("Root/List").GetComponent<CUIListScript>();
            if (dataList == null)
            {
                component.SetElementAmount(0);
            }
            else
            {
                int amount = (dataList.Count >= 10) ? 10 : dataList.Count;
                component.SetElementAmount(amount);
                CUIListElementScript elemenet = null;
                for (int i = 0; i < amount; i++)
                {
                    elemenet = component.GetElemenet(i);
                    COMDT_RANK_CURSEASON_FIGHT_RECORD data = dataList[i];
                    DebugHelper.Assert(data != null);
                    Image image = elemenet.transform.Find("imageIcon").GetComponent<Image>();
                    Text result = elemenet.transform.Find("txtResult").GetComponent<Text>();
                    Text text2 = elemenet.transform.Find("txtTime").GetComponent<Text>();
                    Text text3 = elemenet.transform.Find("txtKDA").GetComponent<Text>();
                    SetWinLose(result, ref data);
                    text2.text = GetGameTime(ref data);
                    text3.text = string.Format("{0}  /  {1}  /  {2}", data.dwKillNum, data.dwDeadNum, data.dwAssistNum);
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
                    if (dataByKey != null)
                    {
                        image.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false);
                        Utility.FindChild(image.gameObject, "Friend").CustomSetActive(data.bTeamerNum > 1);
                        Utility.FindChild(image.gameObject, "Bp").CustomSetActive(Convert.ToBoolean(data.bIsBanPick));
                    }
                    for (int j = 0; j < 6; j++)
                    {
                        COMDT_INGAME_EQUIP_INFO comdt_ingame_equip_info = null;
                        if (j < data.bEquipNum)
                        {
                            comdt_ingame_equip_info = data.astEquipDetail[j];
                        }
                        int num4 = j + 1;
                        Image image2 = elemenet.transform.FindChild(string.Format("TianFu/TianFuIcon{0}", num4.ToString())).GetComponent<Image>();
                        if ((comdt_ingame_equip_info == null) || (comdt_ingame_equip_info.dwEquipID == 0))
                        {
                            image2.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            image2.gameObject.CustomSetActive(true);
                            CUICommonSystem.SetEquipIcon((ushort) comdt_ingame_equip_info.dwEquipID, image2.gameObject, form);
                        }
                    }
                }
            }
        }

        public static void InitRewardForm(CUIFormScript form, ref COMDT_RANKDETAIL data)
        {
            ShowRankDetail(form.transform.Find("RankCon").gameObject, ref data, true);
        }

        public static bool IsSuperKing(byte rankGrade, uint rankClass)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcd);
            return ((dataByKey != null) && ((rankGrade == CLadderSystem.MAX_RANK_LEVEL) && ((0 < rankClass) && (rankClass <= dataByKey.dwConfValue))));
        }

        public static bool IsSuperMaster(byte rankGrade, uint rankClass)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcd);
            return ((dataByKey != null) && ((rankGrade == CLadderSystem.MAX_RANK_LEVEL) && ((rankClass == 0) || (rankClass > dataByKey.dwConfValue))));
        }

        public static void OnHistoryItemChange(GameObject go, bool bExpand)
        {
            Image component = go.transform.Find("Bg").GetComponent<Image>();
            if (component != null)
            {
                component.color = !bExpand ? s_Bg_Color_Shrink : s_Bg_Color_Expand;
            }
            Transform transform = go.transform.Find("Title/Button");
            if (transform != null)
            {
                if (bExpand)
                {
                    (transform as RectTransform).rotation = Quaternion.Euler(0f, 0f, 180f);
                }
                else
                {
                    (transform as RectTransform).rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }

        public static void SetMostRecentGameData(CUIFormScript form, ref COMDT_RANKDETAIL rankDetail, List<COMDT_RANK_CURSEASON_FIGHT_RECORD> dataList)
        {
            GameObject gameObject = form.transform.Find("StatPanel/RecentCon").gameObject;
            if (((rankDetail != null) && (dataList != null)) && ((dataList.Count > 0) && (rankDetail.dwSeasonIdx == dataList[0].dwSeasonId)))
            {
                Text component = gameObject.transform.Find("txtGameResult").GetComponent<Text>();
                Text text2 = gameObject.transform.Find("txtGameTime").GetComponent<Text>();
                COMDT_RANK_CURSEASON_FIGHT_RECORD data = dataList[0];
                SetWinLose(component, ref data);
                text2.text = GetGameTime(ref data);
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
                if (dataByKey != null)
                {
                    Image image = gameObject.transform.Find("heroItemCell/imageIcon").GetComponent<Image>();
                    image.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false);
                    image.transform.Find("Friend").gameObject.CustomSetActive(data.bTeamerNum > 1);
                    image.transform.Find("Bp").gameObject.CustomSetActive(Convert.ToBoolean(data.bIsBanPick));
                }
                gameObject.CustomSetActive(true);
            }
            else
            {
                gameObject.CustomSetActive(false);
            }
        }

        private static void SetMostUsedHero(Transform item, ref COMDT_RANK_COMMON_USED_HERO data, CUIFormScript formScript)
        {
            Text component = item.Find("txtGameNum").GetComponent<Text>();
            Text text2 = item.Find("txtWinNum").GetComponent<Text>();
            component.text = data.dwFightCnt.ToString();
            text2.text = data.dwWinCnt.ToString();
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.dwHeroId);
            if (dataByKey != null)
            {
                item.Find("heroItemCell/imageIcon").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), formScript, true, false, false);
            }
        }

        private static void SetWinLose(Text Result, ref COMDT_RANK_CURSEASON_FIGHT_RECORD data)
        {
            if (data.dwGameResult == 1)
            {
                Result.text = Singleton<CTextManager>.GetInstance().GetText("GameResult_Win");
            }
            else if (data.dwGameResult == 2)
            {
                Result.text = Singleton<CTextManager>.GetInstance().GetText("GameResult_Lose");
            }
            else if (data.dwGameResult == 3)
            {
                Result.text = Singleton<CTextManager>.GetInstance().GetText("GameResult_Draw");
            }
            else
            {
                Result.text = Singleton<CTextManager>.GetInstance().GetText("GameResult_Null");
            }
        }

        private static void ShowBpModePanel(CUIFormScript form)
        {
            GameObject widget = form.GetWidget(4);
            GameObject obj3 = form.GetWidget(5);
            GameObject obj4 = form.GetWidget(6);
            if (Singleton<CLadderSystem>.GetInstance().IsUseBpMode())
            {
                widget.CustomSetActive(true);
                obj3.CustomSetActive(true);
                obj4.CustomSetActive(true);
            }
            else
            {
                widget.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
            }
        }

        private static void ShowContinuousWinPanel(CUIFormScript form, COMDT_RANKDETAIL data, bool isQualified)
        {
            GameObject widget = form.GetWidget(1);
            uint continuousWinCountForExtraStar = Singleton<CLadderSystem>.GetInstance().GetContinuousWinCountForExtraStar();
            if (isQualified && (continuousWinCountForExtraStar != 0))
            {
                widget.CustomSetActive(true);
                string[] args = new string[] { (continuousWinCountForExtraStar - data.dwAddScoreOfConWinCnt).ToString() };
                form.GetWidget(0).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Continuous_Win_Desc", args);
                Transform transform = form.GetWidget(2).transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child != null)
                    {
                        child.gameObject.CustomSetActive(i < continuousWinCountForExtraStar);
                        Transform transform3 = child.GetChild(0);
                        if (transform3 != null)
                        {
                            transform3.gameObject.CustomSetActive(i < data.dwAddScoreOfConWinCnt);
                        }
                    }
                }
            }
            else
            {
                widget.CustomSetActive(false);
            }
        }

        public static void ShowRankDetail(GameObject go, ref COMDT_RANKDETAIL rankDetail, bool isShowSeasonHighestGrade = false)
        {
            DebugHelper.Assert(rankDetail != null, "Rank Data must not be null!!!");
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (rankDetail != null))
            {
                if (isShowSeasonHighestGrade)
                {
                    ShowRankDetail(go, masterRoleInfo.m_rankSeasonHighestGrade, masterRoleInfo.m_rankSeasonHighestClass, rankDetail.dwScore, true, false, true, false);
                }
                else
                {
                    ShowRankDetail(go, masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass, rankDetail.dwScore, true, false, false, false);
                }
            }
        }

        public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, bool isGrey = false, bool isUseSmall = false)
        {
            DebugHelper.Assert(go != null, "GameObject is NULL!!!");
            DebugHelper.Assert(rankGrade > 0, "grade must be above 0!!!");
            Image image = (go.transform.Find("ImgRank") == null) ? null : go.transform.Find("ImgRank").GetComponent<Image>();
            Image image2 = (go.transform.Find("ImgSubRank") == null) ? null : go.transform.Find("ImgSubRank").GetComponent<Image>();
            Text graphic = (go.transform.Find("txtRankName") == null) ? null : go.transform.Find("txtRankName").GetComponent<Text>();
            if (image != null)
            {
                string rankIconPath = GetRankIconPath(rankGrade, rankClass);
                image.SetSprite(isUseSmall ? string.Format("{0}_small", rankIconPath) : rankIconPath, null, true, false, false);
                image.gameObject.CustomSetActive(true);
                CUIUtility.SetImageGrey(image, isGrey);
            }
            if (image2 != null)
            {
                image2.SetSprite(GetSubRankIconPath(rankGrade, rankClass), null, true, false, false);
                image2.gameObject.CustomSetActive(true);
                CUIUtility.SetImageGrey(image2, isGrey);
            }
            if (graphic != null)
            {
                graphic.text = GetRankTitle(rankGrade, rankClass);
                CUIUtility.SetImageGrey(graphic, isGrey);
            }
        }

        public static void ShowRankDetail(GameObject go, byte rankGrade, uint rankClass, uint score, bool bShowScore = true, bool useSmall = false, bool isLadderRewardForm = false, bool isUseSpecialColorWhenSuperKing = false)
        {
            DebugHelper.Assert(go != null, "GameObject is NULL!!!");
            DebugHelper.Assert(rankGrade > 0, "grade must be above 0!!!");
            Image image = (go.transform.Find("ImgRank") == null) ? null : go.transform.Find("ImgRank").GetComponent<Image>();
            Image image2 = (go.transform.Find("ImgSubRank") == null) ? null : go.transform.Find("ImgSubRank").GetComponent<Image>();
            Text text = (go.transform.Find("txtRankName") == null) ? null : go.transform.Find("txtRankName").GetComponent<Text>();
            Text text2 = (go.transform.Find("txtTopGroupScore") == null) ? null : go.transform.Find("txtTopGroupScore").GetComponent<Text>();
            if (image != null)
            {
                string rankIconPath = GetRankIconPath(rankGrade, rankClass);
                image.SetSprite(useSmall ? string.Format("{0}_small", rankIconPath) : rankIconPath, null, true, false, false);
                image.gameObject.CustomSetActive(true);
            }
            if (image2 != null)
            {
                if (isLadderRewardForm && (rankGrade >= CLadderSystem.MAX_RANK_LEVEL))
                {
                    image2.gameObject.CustomSetActive(false);
                }
                else
                {
                    image2.SetSprite(GetSubRankIconPath(rankGrade, rankClass), null, true, false, false);
                    image2.gameObject.CustomSetActive(true);
                }
            }
            if (text != null)
            {
                text.text = GetRankTitle(rankGrade, rankClass);
                if (isUseSpecialColorWhenSuperKing && IsSuperKing(rankGrade, rankClass))
                {
                    text.text = "<color=#feba29>" + text.text + "</color>";
                }
            }
            if (text2 != null)
            {
                if (rankGrade >= CLadderSystem.MAX_RANK_LEVEL)
                {
                    text2.text = string.Format("x{0}", score);
                }
                text2.gameObject.CustomSetActive(rankGrade >= CLadderSystem.MAX_RANK_LEVEL);
            }
            Transform transform = go.transform.Find("ScoreCon");
            if (transform != null)
            {
                if ((rankGrade >= CLadderSystem.MAX_RANK_LEVEL) || !bShowScore)
                {
                    transform.gameObject.CustomSetActive(false);
                }
                else
                {
                    transform.Find("Con3Star").gameObject.CustomSetActive(false);
                    transform.Find("Con4Star").gameObject.CustomSetActive(false);
                    transform.Find("Con5Star").gameObject.CustomSetActive(false);
                    ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankGrade);
                    if (dataByKey != null)
                    {
                        Transform transform2 = transform.Find(string.Format("Con{0}Star", dataByKey.dwGradeUpNeedScore));
                        if (transform2 != null)
                        {
                            transform2.gameObject.CustomSetActive(true);
                            for (int i = 1; i <= dataByKey.dwGradeUpNeedScore; i++)
                            {
                                Image component = transform2.Find(string.Format("ImgScore{0}", i)).GetComponent<Image>();
                                string prefabPath = (score < i) ? LADDER_IMG_STAR_EMPTY : LADDER_IMG_STAR;
                                if (component != null)
                                {
                                    component.SetSprite(prefabPath, null, true, false, false);
                                }
                            }
                        }
                    }
                    transform.gameObject.CustomSetActive(true);
                }
            }
        }

        public static void ShowRankReward(byte grade)
        {
            ResRankRewardConf dataByKey = GameDataMgr.rankRewardDatabin.GetDataByKey((uint) grade);
            if (dataByKey != null)
            {
                ListView<CUseable> inList = new ListView<CUseable>();
                for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                {
                    ResDT_ChapterRewardInfo info = dataByKey.astRewardDetail[i];
                    if (info.bType != 0)
                    {
                        CUseable item = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) info.bType, (int) info.dwNum, info.dwID);
                        if (item != null)
                        {
                            inList.Add(item);
                        }
                    }
                }
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(inList), Singleton<CTextManager>.GetInstance().GetText("Ladder_Season_Reward"), false, enUIEventID.Ladder_ReqGetSeasonReward, false, false, "Form_Award");
            }
        }

        private static void ShowSuperKingRankPanel(CUIFormScript form)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            bool bActive = IsSuperKing(masterRoleInfo.m_rankGrade, masterRoleInfo.m_rankClass);
            GameObject widget = form.GetWidget(3);
            widget.CustomSetActive(bActive);
            if (bActive)
            {
                CUICommonSystem.SetRankDisplay(masterRoleInfo.m_rankClass, widget.transform);
            }
        }

        [CompilerGenerated]
        private sealed class <GetRankBigGradeName>c__AnonStorey84
        {
            internal byte rankBigGrade;

            internal bool <>m__82(ResRankGradeConf x)
            {
                return (x.bBelongBigGrade == this.rankBigGrade);
            }
        }

        public enum enLadderEntryFormWidget
        {
            ContinuousWinDescText,
            ContinuousWinPanel,
            ContinuousWinStarPanel,
            SuperKingRankPanel,
            SingleStartBtnBpTag,
            DoubleStartBtnBpTag,
            BpModePanel,
            LeagueTimeLabel
        }
    }
}

