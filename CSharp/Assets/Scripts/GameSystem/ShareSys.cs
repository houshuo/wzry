namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class ShareSys : MonoSingleton<ShareSys>
    {
        private float g_fDownloadOutTime = 10f;
        private const string HeroShowImgDir = "UGUI/Sprite/Dynamic/HeroShow/";
        private bool isRegisterd;
        private bool m_bAdreo306;
        private bool m_bClickShareFriendBtn;
        private bool m_bClickTimeLineBtn;
        public bool m_bHide;
        private bool m_bShareHero;
        private bool m_bSharePvpForm;
        public bool m_bShowTimeline;
        public bool m_bWinPVPResult;
        private List<CLoadReq> m_DownLoadSkinList = new List<CLoadReq>();
        private Image m_FriendBtnImage;
        public ShareActivityParam m_ShareActivityParam = new ShareActivityParam(false);
        public SHARE_INFO m_ShareInfo;
        private string m_sharePic = CFileManager.GetCachePath("share.jpg");
        public string m_SharePicCDNCachePath = string.Empty;
        private ListView<CSDT_SHARE_TLOG_INFO> m_ShareReportInfoList = new ListView<CSDT_SHARE_TLOG_INFO>();
        private CSPKG_JOINMULTIGAMEREQ m_ShareRoom;
        private Transform m_ShareSkinPicImage;
        private string m_ShareSkinPicLoading = string.Empty;
        private string m_ShareSkinPicNotFound = string.Empty;
        private string m_ShareSkinPicOutofTime = string.Empty;
        private string m_ShareStr = string.Empty;
        private COMDT_INVITE_JOIN_INFO m_ShareTeam;
        private Transform m_TimelineBtn;
        private Image m_TimeLineBtnImage;
        public static string s_formShareMysteryDiscountPath = "UGUI/Form/System/ShareUI/Form_ShareMystery_Discount.prefab";
        public static string s_formShareNewAchievementPath = "UGUI/Form/System/Achieve/Form_Achievement_ShareNewAchievement.prefab";
        public static string s_formShareNewHeroPath = "UGUI/Form/System/ShareUI/Form_ShareNewHero.prefab";
        public static string s_formSharePVPPath = "UGUI/Form/System/ShareUI/Form_SharePVPResult.prefab";
        public static string s_imageSharePVPAchievement = (CUIUtility.s_Sprite_Dynamic_PvpAchievementShare_Dir + "Img_PVP_ShareAchievement_");
        public static readonly string SNS_SHARE_COMMON = "SNS_SHARE_SEND_COMMON";
        public static readonly string SNS_SHARE_RECALL_FRIEND = "SNS_SHARE_RECALL_FRIEND";
        public static readonly string SNS_SHARE_SEND_HEART = "SNS_SHARE_SEND_HEART";

        public void AddshareReportInfo(uint dwType, uint dwSubType)
        {
            bool flag = false;
            for (int i = 0; i < this.m_ShareReportInfoList.Count; i++)
            {
                CSDT_SHARE_TLOG_INFO csdt_share_tlog_info = this.m_ShareReportInfoList[i];
                if ((csdt_share_tlog_info.dwType == dwType) && (csdt_share_tlog_info.dwSubType == dwSubType))
                {
                    csdt_share_tlog_info.dwCnt++;
                    flag = true;
                }
            }
            if (!flag)
            {
                CSDT_SHARE_TLOG_INFO item = new CSDT_SHARE_TLOG_INFO {
                    dwCnt = 1,
                    dwType = dwType,
                    dwSubType = dwSubType
                };
                this.m_ShareReportInfoList.Add(item);
            }
        }

        private void BtnGray(Image imgeBtn, bool bShow)
        {
            if (imgeBtn != null)
            {
                if (bShow)
                {
                    imgeBtn.color = new Color(1f, 1f, 1f, 1f);
                    imgeBtn.GetComponent<CUIEventScript>().enabled = true;
                }
                else
                {
                    imgeBtn.color = new Color(0f, 1f, 1f, 1f);
                    imgeBtn.GetComponent<CUIEventScript>().enabled = false;
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator Capture(Rect screenShotRect, Action<string> callback)
        {
            return new <Capture>c__Iterator22 { screenShotRect = screenShotRect, callback = callback, <$>screenShotRect = screenShotRect, <$>callback = callback, <>f__this = this };
        }

        private void ClearRoomData()
        {
            this.m_ShareRoom = null;
            this.m_ShareStr = string.Empty;
        }

        public void ClearShareDataMsg()
        {
            this.ClearTeamDataMsg();
            this.ClearRoomData();
        }

        private void ClearTeamDataMsg()
        {
            this.m_ShareTeam = null;
            this.m_ShareStr = string.Empty;
        }

        public void CloseNewHeroForm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            if (form != null)
            {
                DynamicShadow.EnableDynamicShow(form.gameObject, false);
            }
            this.RemoveDownLoading(this.m_ShareInfo.shareSkinUrl);
            this.m_ShareInfo.clear();
            this.m_bShowTimeline = false;
            Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newHeroOrSkinPath);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Get_Product_OK);
        }

        public void CloseShareHeroForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formShareNewHeroPath);
            this.m_bShareHero = false;
            this.m_bClickShareFriendBtn = false;
            this.m_ShareSkinPicImage = null;
            this.m_FriendBtnImage = null;
            this.m_TimeLineBtnImage = null;
        }

        [DebuggerHidden]
        public IEnumerator DownloadImageByTag2(string preUrl, CLoadReq req, LoadRCallBack3 callBack, string tagPath)
        {
            return new <DownloadImageByTag2>c__Iterator21 { preUrl = preUrl, tagPath = tagPath, req = req, callBack = callBack, <$>preUrl = preUrl, <$>tagPath = tagPath, <$>req = req, <$>callBack = callBack, <>f__this = this };
        }

        private string GetCDNSharePicUrl(string url, int type)
        {
            string str = string.Empty;
            if (type == 1)
            {
                return string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
            }
            if (type == 2)
            {
                str = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, url);
            }
            return str;
        }

        private GameObject GetCloseBtn(CUIFormScript form)
        {
            if (form != null)
            {
                if (form.m_formPath == s_formShareNewHeroPath)
                {
                    return form.GetWidget(2);
                }
                if (form.m_formPath == s_formSharePVPPath)
                {
                    return form.GetWidget(1);
                }
                if (form.m_formPath == s_formShareNewAchievementPath)
                {
                    return form.GetWidget(3);
                }
                if (form.m_formPath == s_formShareMysteryDiscountPath)
                {
                    return form.GetWidget(1);
                }
                if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == Singleton<SettlementSystem>.GetInstance().SettlementFormName)
                {
                    return form.gameObject.transform.FindChild("Panel/Btn_Share_PVP_DATA_CLOSE").gameObject;
                }
                if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
                {
                    return form.gameObject.transform.FindChild("Button_Close").gameObject;
                }
                object[] inParameters = new object[] { form.m_formPath };
                DebugHelper.Assert(false, "ShareSys.GetCloseBtn(): error form path = {0}", inParameters);
            }
            return null;
        }

        private GameObject GetDisplayPanel(CUIFormScript form)
        {
            if (form != null)
            {
                if (form.m_formPath == s_formShareNewHeroPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == s_formSharePVPPath)
                {
                    return form.GetWidget(0);
                }
                if (form.m_formPath == s_formShareNewAchievementPath)
                {
                    return form.GetWidget(4);
                }
                if (form.m_formPath == PvpAchievementForm.s_formSharePVPDefeatPath)
                {
                    return form.GetWidget(2);
                }
                if (form.m_formPath == Singleton<SettlementSystem>.GetInstance().SettlementFormName)
                {
                    return form.gameObject.transform.FindChild("Panel").gameObject;
                }
                if (form.m_formPath == "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab")
                {
                    return form.gameObject.transform.FindChild("ShareFrame").gameObject;
                }
                if (form.m_formPath == s_formShareMysteryDiscountPath)
                {
                    return form.gameObject.transform.FindChild("DiscountShow").gameObject;
                }
                object[] inParameters = new object[] { form.m_formPath };
                DebugHelper.Assert(false, "ShareSys.GetDisplayPanel(): error form path = {0}", inParameters);
            }
            return null;
        }

        private ELoadError GetLoadReq(CLoadReq url)
        {
            for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (url.m_Url == req.m_Url)
                {
                    CLoadReq req2 = this.m_DownLoadSkinList[i];
                    return req2.m_LoadError;
                }
            }
            return ELoadError.None;
        }

        private Rect GetScreenShotRect(GameObject displayeRect)
        {
            RectTransform transform = (displayeRect == null) ? new RectTransform() : displayeRect.GetComponent<RectTransform>();
            float x = transform.rect.width * 0.5f;
            float y = transform.rect.height * 0.5f;
            Vector3 position = displayeRect.transform.TransformPoint(new Vector3(-x, -y, 0f));
            position = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(position);
            Vector3 vector2 = displayeRect.transform.TransformPoint(new Vector3(x, y, 0f));
            vector2 = Singleton<CUIManager>.instance.FormCamera.WorldToScreenPoint(vector2);
            float width = Math.Abs((float) (position.x - vector2.x));
            return new Rect(position.x, position.y, width, Math.Abs((float) (position.y - vector2.y)));
        }

        public void GShare(string buttonType, string sharePathPic)
        {
            this.m_bClickTimeLineBtn = true;
            this.Share(buttonType, sharePathPic);
        }

        protected override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroorSkin, new CUIEventManager.OnUIEventHandler(this.CloseNewHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_NewHero, new CUIEventManager.OnUIEventHandler(this.OpenShareNewHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_CloseNewHeroShareForm, new CUIEventManager.OnUIEventHandler(this.CloseShareHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareFriend, new CUIEventManager.OnUIEventHandler(this.ShareFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareTimeLine, new CUIEventManager.OnUIEventHandler(this.ShareTimeLine));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShareSavePic, new CUIEventManager.OnUIEventHandler(this.SavePic));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPScore, new CUIEventManager.OnUIEventHandler(this.SettlementShareBtnHandle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_SharePVPSCcoreClose, new CUIEventManager.OnUIEventHandler(this.OnCloseShowPVPSCore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_MysteryDiscount, new CUIEventManager.OnUIEventHandler(this.ShareMysteryDiscount));
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x88).dwConfValue;
            Singleton<CTimerManager>.GetInstance().AddTimer((int) (dwConfValue * 0x3e8), -1, new CTimer.OnTimeUpHandler(this.OnReportShareInfo));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SHARE_PVP_SETTLEDATA_CLOSE, new System.Action(this.On_SHARE_PVP_SETTLEDATA_CLOSE));
            this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
            string cachePath = CFileManager.GetCachePath("SkinSharePic");
            try
            {
                if (!Directory.Exists(cachePath))
                {
                    Directory.CreateDirectory(cachePath);
                }
                this.m_SharePicCDNCachePath = cachePath;
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.Log("sharesys cannot create dictionary " + exception);
                this.m_SharePicCDNCachePath = CFileManager.GetCachePath();
            }
            this.m_ShareSkinPicNotFound = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_NotFound");
            this.m_ShareSkinPicOutofTime = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_OutofTime");
            this.m_ShareSkinPicLoading = Singleton<CTextManager>.GetInstance().GetText("Share_Skin_Pic_Error_Loading");
            this.m_bAdreo306 = this.IsAdreo306();
        }

        private bool IsAdreo306()
        {
            string str = SystemInfo.graphicsDeviceName.ToLower();
            char[] separator = new char[] { ' ', '\t', '\r', '\n', '+', '-', ':', '\0' };
            string[] strArray = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (((strArray != null) && (strArray.Length != 0)) && (strArray[0] == "adreno"))
            {
                int result = 0;
                for (int i = 1; i < strArray.Length; i++)
                {
                    bool flag = int.TryParse(strArray[i], out result);
                    if (result == 0x132)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isDownLoading(CLoadReq url)
        {
            for (int i = 0; i < this.m_DownLoadSkinList.Count; i++)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (url.m_Url == req.m_Url)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInstallPlatform()
        {
            if (Singleton<ApolloHelper>.GetInstance().IsPlatformInstalled(Singleton<ApolloHelper>.GetInstance().CurPlatform))
            {
                return true;
            }
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装微信，无法使用该功能", false);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox("未安装手机QQ，无法使用该功能", false);
            }
            return false;
        }

        private void LoadShareSkinImage(CLoadReq loadReq, Image imageObj)
        {
            string url = loadReq.m_Url;
            string cachePath = loadReq.m_CachePath;
            string cDNSharePicUrl = this.GetCDNSharePicUrl(url, loadReq.m_Type);
            string str4 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
            string path = CFileManager.CombinePath(cachePath, str4);
            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                if ((texture.LoadImage(data) && this.m_bShareHero) && (imageObj != null))
                {
                    imageObj.gameObject.CustomSetActive(true);
                    if (this.m_bShareHero && (imageObj != null))
                    {
                        imageObj.SetSprite(Sprite.Create(texture, new Rect(0f, 0f, (float) texture.width, (float) texture.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                    }
                    if (this.m_FriendBtnImage != null)
                    {
                        this.BtnGray(this.m_FriendBtnImage, true);
                    }
                    if ((this.m_TimeLineBtnImage != null) && !this.m_bShowTimeline)
                    {
                        this.BtnGray(this.m_TimeLineBtnImage, true);
                    }
                }
            }
            else
            {
                switch (this.GetLoadReq(loadReq))
                {
                    case ELoadError.NotFound:
                        Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
                        return;

                    case ELoadError.OutOfTime:
                        Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
                        return;
                }
                Singleton<CUIManager>.GetInstance().OpenTips(this.m_ShareSkinPicLoading, false, 1.5f, null, new object[0]);
            }
        }

        private void On_SHARE_PVP_SETTLEDATA_CLOSE()
        {
            this.m_bSharePvpForm = false;
        }

        public void OnCloseShowPVPSCore(CUIEvent ievent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formSharePVPPath);
            this.m_bSharePvpForm = false;
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShareFin, new uint[0]);
            Singleton<CChatController>.instance.ShowPanel(true, false);
        }

        private static void OnLoadNewHeroOrSkin3DModel(GameObject rawImage, uint heroId, uint skinId, bool bInitAnima)
        {
            CUI3DImageScript script = (rawImage == null) ? null : rawImage.GetComponent<CUI3DImageScript>();
            string objectName = CUICommonSystem.GetHeroPrefabPath(heroId, (int) skinId, true).ObjectName;
            GameObject model = (script == null) ? null : script.AddGameObject(objectName, false, false);
            CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
            instance.Set3DModel(model);
            if (model == null)
            {
                objectName = null;
            }
            else if (bInitAnima)
            {
                instance.InitAnimatList();
                instance.InitAnimatSoundList(heroId, skinId);
                instance.OnModePlayAnima("Come");
            }
        }

        private void OnReportShareInfo(int timerSequence)
        {
            if (Singleton<CBattleSystem>.instance.FormScript == null)
            {
                this.ReportShareInfo();
            }
        }

        public void OnShareCallBack()
        {
            IApolloSnsService service = IApollo.Instance.GetService(1) as IApolloSnsService;
            if ((service != null) && !this.isRegisterd)
            {
                service.onShareEvent += delegate (ApolloShareResult shareResponseInfo) {
                    object[] args = new object[] { shareResponseInfo.result, shareResponseInfo.platform, shareResponseInfo.drescription, shareResponseInfo.extInfo };
                    UnityEngine.Debug.Log("sns += " + string.Format("share result:{0} \n share platform:{1} \n share description:{2}\n share extInfo:{3}", args));
                    if (shareResponseInfo.result == ApolloResult.Success)
                    {
                        if ((shareResponseInfo.extInfo != SNS_SHARE_SEND_HEART) && (shareResponseInfo.extInfo != SNS_SHARE_RECALL_FRIEND))
                        {
                            if (this.m_bClickTimeLineBtn)
                            {
                                this.m_bShowTimeline = true;
                                Singleton<EventRouter>.instance.BroadCastEvent<Transform>(EventID.SHARE_TIMELINE_SUCC, this.m_TimelineBtn);
                                this.UpdateTimelineBtn();
                            }
                            uint dwType = 0;
                            if (this.m_bShareHero)
                            {
                                dwType = 0;
                            }
                            else if (this.m_bSharePvpForm)
                            {
                                dwType = 1;
                            }
                            if (this.m_bClickShareFriendBtn)
                            {
                                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                                {
                                    this.AddshareReportInfo(dwType, 3);
                                }
                                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                                {
                                    this.AddshareReportInfo(dwType, 2);
                                }
                            }
                            else if (this.m_bClickTimeLineBtn)
                            {
                                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                                {
                                    this.AddshareReportInfo(dwType, 5);
                                }
                                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                                {
                                    this.AddshareReportInfo(dwType, 4);
                                }
                            }
                            CTaskSys.Send_Share_Task();
                            if (this.m_bClickTimeLineBtn)
                            {
                                this.SendShareActivityDoneMsg();
                            }
                            this.m_bClickTimeLineBtn = false;
                            this.m_bClickShareFriendBtn = false;
                        }
                    }
                    else
                    {
                        this.m_bClickTimeLineBtn = false;
                        this.m_bClickShareFriendBtn = false;
                    }
                };
                this.isRegisterd = true;
            }
        }

        [MessageHandler(0x10d4)]
        public static void OnShareReport(CSPkg msg)
        {
            UnityEngine.Debug.Log("share report " + msg.stPkgData.stShareTLogRsp.iErrCode);
        }

        public void OpenShareNewHeroForm(CUIEvent uiEvent)
        {
            this.m_ShareActivityParam.clear();
            this.AddshareReportInfo(0, 0);
            this.m_bShareHero = true;
            this.m_bClickShareFriendBtn = false;
            if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)
            {
                uint[] kShareParam = new uint[] { this.m_ShareInfo.heroId };
                this.m_ShareActivityParam.set(3, 1, kShareParam);
                this.ShowNewHeroShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
            }
            else if (this.m_ShareInfo.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
            {
                uint[] numArray2 = new uint[] { this.m_ShareInfo.skinId };
                this.m_ShareActivityParam.set(3, 1, numArray2);
                this.ShowNewSkinShare(this.m_ShareInfo.heroId, this.m_ShareInfo.skinId, false, this.m_ShareInfo.rewardType, false);
            }
        }

        public void OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE type)
        {
            this.m_ShareActivityParam.clear();
            this.m_bSharePvpForm = true;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_formSharePVPPath, false, true);
            this.UpdateSharePVPForm(form, form.GetWidget(0));
            CUIUtility.SetImageSprite(form.GetWidget(2).GetComponent<Image>(), s_imageSharePVPAchievement + ((int) type) + ".prefab", form, true, false, false);
        }

        public string PackRoomData(int iRoomEntity, uint dwRoomID, uint dwRoomSeq, byte bMapType, uint dwMapId, ulong ullFeature)
        {
            object[] args = new object[] { iRoomEntity, dwRoomID, dwRoomSeq, bMapType, dwMapId, ullFeature, (int) Application.platform, (int) ApolloConfig.platform };
            string message = string.Format("ShareRoom_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", args);
            UnityEngine.Debug.Log(message);
            return message;
        }

        public string PackTeamData(ulong uuid, uint dwTeamId, uint dwTeamSeq, int iTeamEntity, ulong ullTeamFeature, byte bInviterGradeOfRank, byte bGameMode, byte bPkAI, byte bMapType, uint dwMapId, byte bAILevel)
        {
            object[] args = new object[] { uuid, dwTeamId, dwTeamSeq, iTeamEntity, ullTeamFeature, bInviterGradeOfRank, bGameMode, bPkAI, bMapType, dwMapId, bAILevel, (int) Application.platform, (int) ApolloConfig.platform };
            string message = string.Format("ShareTeam_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}_{12}", args);
            UnityEngine.Debug.Log(message);
            return message;
        }

        public void PreLoadShareSkinImage(CLoadReq loadReq)
        {
            <PreLoadShareSkinImage>c__AnonStorey85 storey = new <PreLoadShareSkinImage>c__AnonStorey85 {
                loadReq = loadReq,
                <>f__this = this
            };
            if (!this.SharePicIsExist(storey.loadReq.m_Url, this.m_SharePicCDNCachePath, storey.loadReq.m_Type) && !this.isDownLoading(storey.loadReq))
            {
                this.m_DownLoadSkinList.Add(storey.loadReq);
                string str = string.Empty;
                if (storey.loadReq.m_Type == 1)
                {
                    str = string.Format("{0}HeroSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, storey.loadReq.m_Url);
                }
                else if (storey.loadReq.m_Type == 2)
                {
                    str = string.Format("{0}SkinSharePic/{1}.jpg", BannerImageSys.GlobalLoadPath, storey.loadReq.m_Url);
                }
                if (!string.IsNullOrEmpty(str))
                {
                    base.StartCoroutine(this.DownloadImageByTag2(str, storey.loadReq, new LoadRCallBack3(storey.<>m__8A), this.m_SharePicCDNCachePath));
                }
            }
        }

        private void RefeshPhoto(string filename)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (class2 != null)
            {
                AndroidJavaObject @static = class2.GetStatic<AndroidJavaObject>("currentActivity");
                if (@static != null)
                {
                    UnityEngine.Debug.Log("RefeshPhoto in unity");
                    object[] args = new object[] { filename };
                    @static.Call("RefeshPhoto", args);
                }
            }
        }

        private void RemoveDownLoading(string url)
        {
            for (int i = this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
            {
                CLoadReq req = this.m_DownLoadSkinList[i];
                if (req.m_Url == url)
                {
                    this.m_DownLoadSkinList.Remove(this.m_DownLoadSkinList[i]);
                }
            }
        }

        private void ReportShareInfo()
        {
            CSDT_TRANK_TLOG_INFO[] uiTlog = Singleton<RankingSystem>.instance.GetUiTlog();
            if ((uiTlog.Length != 0) || (this.m_ShareReportInfoList.Count != 0))
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d3);
                int count = this.m_ShareReportInfoList.Count;
                msg.stPkgData.stShareTLogReq.bNum = (byte) count;
                for (int i = 0; i < count; i++)
                {
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwType = this.m_ShareReportInfoList[i].dwType;
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwSubType = this.m_ShareReportInfoList[i].dwSubType;
                    msg.stPkgData.stShareTLogReq.astShareDetail[i].dwCnt = this.m_ShareReportInfoList[i].dwCnt;
                }
                count = uiTlog.Length;
                msg.stPkgData.stShareTLogReq.dwTrankNum = (uint) count;
                for (int j = 0; j < count; j++)
                {
                    msg.stPkgData.stShareTLogReq.astTrankDetail[j].dwType = uiTlog[j].dwType;
                    msg.stPkgData.stShareTLogReq.astTrankDetail[j].dwCnt = uiTlog[j].dwCnt;
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this.m_ShareReportInfoList.Clear();
                Singleton<RankingSystem>.instance.ClearUiTlog();
            }
        }

        public void SavePic(CUIEvent uiEvent)
        {
            <SavePic>c__AnonStorey88 storey = new <SavePic>c__AnonStorey88 {
                <>f__this = this,
                btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript)
            };
            if (storey.btnClose != null)
            {
                if (storey.btnClose != null)
                {
                    storey.btnClose.CustomSetActive(false);
                }
                Singleton<CUIManager>.instance.CloseTips();
                storey.bSettltment = false;
                if (uiEvent.m_srcFormScript.m_formPath == Singleton<SettlementSystem>.GetInstance().SettlementFormName)
                {
                    storey.bSettltment = true;
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                }
                GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                if (displayPanel != null)
                {
                    Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                    base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storey.<>m__8D)));
                    uint dwType = 0;
                    if (this.m_bShareHero)
                    {
                        dwType = 0;
                    }
                    else if (this.m_bSharePvpForm)
                    {
                        dwType = 1;
                    }
                    this.AddshareReportInfo(dwType, 1);
                }
            }
        }

        private void SendRoomDataMsg()
        {
            if (this.m_ShareRoom != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1458);
                msg.stPkgData.stJoinMultiGameReq = this.m_ShareRoom;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                UnityEngine.Debug.Log("share roomdata msg");
            }
            this.ClearRoomData();
        }

        public void SendShareActivityDoneMsg()
        {
            if (Singleton<ActivitySys>.GetInstance().IsShareTask && this.m_ShareActivityParam.bUsed)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1472);
                msg.stPkgData.stWealContentShareDone.bShareType = this.m_ShareActivityParam.bShareType;
                msg.stPkgData.stWealContentShareDone.bParamCnt = this.m_ShareActivityParam.bParamCnt;
                for (int i = 0; i < this.m_ShareActivityParam.bParamCnt; i++)
                {
                    msg.stPkgData.stWealContentShareDone.ShareParam[i] = this.m_ShareActivityParam.ShareParam[i];
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                UnityEngine.Debug.Log(string.Format("SendShareActivityDoneMsg{0}/{1}/{2} ", this.m_ShareActivityParam.bShareType, this.m_ShareActivityParam.bParamCnt, this.m_ShareActivityParam.ShareParam));
            }
            this.m_ShareActivityParam.clear();
        }

        public void SendShareDataMsg()
        {
            if (!string.IsNullOrEmpty(this.m_ShareStr))
            {
                this.UnPackSNSDataStr(this.m_ShareStr);
                this.m_ShareStr = string.Empty;
            }
            else
            {
                if (this.m_ShareRoom != null)
                {
                    this.SendRoomDataMsg();
                }
                if (this.m_ShareTeam != null)
                {
                    this.SendTeamDataMsg();
                }
                this.m_ShareStr = string.Empty;
            }
        }

        private void SendTeamDataMsg()
        {
            if (this.m_ShareTeam != null)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x145a);
                msg.stPkgData.stJoinTeamReq.stInviteJoinInfo = this.m_ShareTeam;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                UnityEngine.Debug.Log("share teamdata msg");
            }
            this.ClearTeamDataMsg();
        }

        public static void SetSharePlatfText(Text platText)
        {
            if (null != platText)
            {
                if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    platText.text = "分享空间";
                }
                else
                {
                    platText.text = "分享朋友圈";
                }
            }
        }

        private void SettlementShareBtnHandle(CUIEvent ievent)
        {
            if (!MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
            {
                Singleton<CChatController>.instance.ShowPanel(false, false);
                this.AddshareReportInfo(1, 0);
                this.OpenShowSharePVPFrom(RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT);
            }
        }

        private void Share(string buttonType, string sharePathPic)
        {
            IApolloSnsService service = IApollo.Instance.GetService(1) as IApolloSnsService;
            if (service != null)
            {
                FileStream stream = new FileStream(sharePathPic, FileMode.Open, FileAccess.Read);
                byte[] array = new byte[stream.Length];
                int count = Convert.ToInt32(stream.Length);
                stream.Read(array, 0, count);
                stream.Close();
                this.OnShareCallBack();
                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    if (buttonType == "TimeLine/Qzone")
                    {
                        service.SendToWeixinWithPhoto(ApolloShareScene.TimeLine, "MSG_INVITE", array, count, string.Empty, "WECHAT_SNS_JUMP_APP");
                    }
                    else if (buttonType == "Session")
                    {
                        service.SendToWeixinWithPhoto(ApolloShareScene.Session, "apollo test", array, count);
                    }
                }
                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    if (buttonType == "TimeLine/Qzone")
                    {
                        service.SendToQQWithPhoto(ApolloShareScene.TimeLine, sharePathPic);
                    }
                    else if (buttonType == "Session")
                    {
                        service.SendToQQWithPhoto(ApolloShareScene.QSession, sharePathPic);
                    }
                }
            }
        }

        public void ShareFriend(CUIEvent uiEvent)
        {
            <ShareFriend>c__AnonStorey86 storey = new <ShareFriend>c__AnonStorey86 {
                <>f__this = this
            };
            Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
            this.m_bClickTimeLineBtn = false;
            if (this.IsInstallPlatform())
            {
                storey.btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
                if (storey.btnClose != null)
                {
                    Singleton<CUIManager>.instance.CloseTips();
                    storey.btnClose.CustomSetActive(false);
                    storey.bSettltment = false;
                    if (uiEvent.m_srcFormScript.m_formPath == Singleton<SettlementSystem>.GetInstance().SettlementFormName)
                    {
                        storey.bSettltment = true;
                        Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                    }
                    GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                    if (displayPanel != null)
                    {
                        Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                        this.m_bClickShareFriendBtn = true;
                        base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storey.<>m__8B)));
                    }
                }
            }
        }

        private void ShareMysteryDiscount(CUIEvent uiEvent)
        {
            CMallMysteryShop instance = Singleton<CMallMysteryShop>.GetInstance();
            if (instance.HasGotDiscount)
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareMysteryDiscountPath, false, true);
                DebugHelper.Assert(formScript != null, "神秘商店分享form打开失败");
                if (formScript != null)
                {
                    GameObject widget = formScript.GetWidget(0);
                    if (widget != null)
                    {
                        Image component = widget.GetComponent<Image>();
                        if (component != null)
                        {
                            component.SetSprite(instance.GetDiscountNumIconPath(instance.Discount), formScript, true, false, false);
                        }
                    }
                    GameObject obj3 = formScript.GetWidget(2);
                    if (obj3 != null)
                    {
                        SetSharePlatfText(obj3.GetComponent<Text>());
                    }
                }
            }
        }

        private bool SharePicIsExist(string url, string tagPath, int type)
        {
            string cDNSharePicUrl = this.GetCDNSharePicUrl(url, type);
            string str2 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(cDNSharePicUrl);
            return File.Exists(CFileManager.CombinePath(tagPath, str2));
        }

        public void ShareTimeLine(CUIEvent uiEvent)
        {
            <ShareTimeLine>c__AnonStorey87 storey = new <ShareTimeLine>c__AnonStorey87 {
                <>f__this = this
            };
            Singleton<ApolloHelper>.GetInstance().m_bShareQQBox = false;
            if (this.IsInstallPlatform())
            {
                storey.btnClose = this.GetCloseBtn(uiEvent.m_srcFormScript);
                if (storey.btnClose != null)
                {
                    UnityEngine.Debug.Log(" m_bClickTimeLineBtn " + this.m_bClickTimeLineBtn);
                    this.m_TimelineBtn = uiEvent.m_srcWidget.transform;
                    this.m_bClickTimeLineBtn = true;
                    this.m_bClickShareFriendBtn = false;
                    storey.btnClose.CustomSetActive(false);
                    Singleton<CUIManager>.instance.CloseTips();
                    storey.bSettltment = false;
                    if (uiEvent.m_srcFormScript.m_formPath == Singleton<SettlementSystem>.GetInstance().SettlementFormName)
                    {
                        storey.bSettltment = true;
                        Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(false);
                    }
                    GameObject displayPanel = this.GetDisplayPanel(uiEvent.m_srcFormScript);
                    if (displayPanel != null)
                    {
                        Rect screenShotRect = this.GetScreenShotRect(displayPanel);
                        base.StartCoroutine(this.Capture(screenShotRect, new Action<string>(storey.<>m__8C)));
                    }
                }
            }
        }

        public void ShowNewHeroShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = 5, bool interactableTransition = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareNewHeroPath, false, true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            enFormPriority priority = enFormPriority.Priority1;
            if (form != null)
            {
                priority = form.m_priority;
            }
            script.SetPriority(priority);
            script.GetWidget(2).CustomSetActive(true);
            this.m_FriendBtnImage = script.GetWidget(4).GetComponent<Image>();
            this.m_TimeLineBtnImage = script.GetWidget(5).GetComponent<Image>();
            if (this.m_FriendBtnImage != null)
            {
                this.BtnGray(this.m_FriendBtnImage, false);
            }
            if (this.m_TimeLineBtnImage != null)
            {
                this.BtnGray(this.m_TimeLineBtnImage, false);
            }
            Image component = script.GetWidget(0).GetComponent<Image>();
            component.gameObject.CustomSetActive(false);
            this.m_ShareSkinPicImage = component.transform;
            if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
            {
                CLoadReq loadReq = new CLoadReq {
                    m_Url = this.m_ShareInfo.shareSkinUrl,
                    m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath,
                    m_LoadError = ELoadError.None,
                    m_Type = 1
                };
                this.LoadShareSkinImage(loadReq, component);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
            }
            SetSharePlatfText(script.GetWidget(3).GetComponent<Text>());
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                Text text = script.GetWidget(1).GetComponent<Text>();
                text.text = masterRoleInfo.GetHaveHeroCount(false).ToString();
                text.gameObject.CustomSetActive(true);
            }
            script.GetWidget(6).GetComponent<Text>().gameObject.CustomSetActive(false);
        }

        public void ShowNewSkinShare(uint heroId, uint skinId, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = 5, bool interactableTransition = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_formShareNewHeroPath, false, true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newHeroOrSkinPath);
            enFormPriority priority = enFormPriority.Priority1;
            if (form != null)
            {
                priority = form.m_priority;
            }
            script.SetPriority(priority);
            script.GetWidget(2).CustomSetActive(true);
            Image component = script.GetWidget(0).GetComponent<Image>();
            component.gameObject.CustomSetActive(false);
            this.m_ShareSkinPicImage = component.transform;
            Text text = script.GetWidget(1).GetComponent<Text>();
            if (text != null)
            {
                text.gameObject.CustomSetActive(false);
            }
            this.m_FriendBtnImage = script.GetWidget(4).GetComponent<Image>();
            this.m_TimeLineBtnImage = script.GetWidget(5).GetComponent<Image>();
            if (this.m_FriendBtnImage != null)
            {
                this.BtnGray(this.m_FriendBtnImage, false);
            }
            if (this.m_TimeLineBtnImage != null)
            {
                this.BtnGray(this.m_TimeLineBtnImage, false);
            }
            if (!string.IsNullOrEmpty(this.m_ShareInfo.shareSkinUrl))
            {
                CLoadReq loadReq = new CLoadReq {
                    m_Url = this.m_ShareInfo.shareSkinUrl,
                    m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath,
                    m_LoadError = ELoadError.None,
                    m_Type = 2
                };
                this.LoadShareSkinImage(loadReq, component);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("图片还没有上传", false, 1.5f, null, new object[0]);
            }
            SetSharePlatfText(script.GetWidget(3).GetComponent<Text>());
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                Text text2 = script.GetWidget(6).GetComponent<Text>();
                text2.gameObject.CustomSetActive(true);
                text2.text = masterRoleInfo.GetHeroSkinCount(false).ToString();
            }
        }

        public bool UnpackInviteSNSData(string data)
        {
            UnityEngine.Debug.Log(data);
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }
            if (MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
            {
                UnityEngine.Debug.Log("正在新手引导中");
                return false;
            }
            if (Singleton<LobbyLogic>.instance.isLogin)
            {
                return this.UnPackSNSDataStr(data);
            }
            this.m_ShareStr = data;
            return true;
        }

        public bool UnpackRoomData(string data)
        {
            UnityEngine.Debug.Log("UnpackRoomData");
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if (((strArray == null) || (strArray.Length != 9)) || (strArray[0] != "ShareRoom"))
            {
                return false;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xbf).dwConfValue;
            if ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel < dwConfValue))
            {
                object[] replaceArr = new object[] { dwConfValue };
                Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Level_Limit", true, 1f, null, replaceArr);
                return false;
            }
            int result = 0;
            if (!int.TryParse(strArray[7], out result) || (Application.platform != result))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Device", true, 1.5f, null, new object[0]);
                return false;
            }
            int num3 = -1;
            if (!int.TryParse(strArray[8], out num3) || (num3 != ApolloConfig.platform))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Platform", true, 1.5f, null, new object[0]);
                return false;
            }
            if (Singleton<GameStateCtrl>.GetInstance().isBattleState || Singleton<GameStateCtrl>.GetInstance().isLoadingState)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_InBattle", true, 1.5f, null, new object[0]);
                return false;
            }
            this.m_ShareRoom = new CSPKG_JOINMULTIGAMEREQ();
            if (((!int.TryParse(strArray[1], out this.m_ShareRoom.iRoomEntity) || !uint.TryParse(strArray[2], out this.m_ShareRoom.dwRoomID)) || (!uint.TryParse(strArray[3], out this.m_ShareRoom.dwRoomSeq) || !byte.TryParse(strArray[4], out this.m_ShareRoom.bMapType))) || (!uint.TryParse(strArray[5], out this.m_ShareRoom.dwMapId) || !ulong.TryParse(strArray[6], out this.m_ShareRoom.ullFeature)))
            {
                return false;
            }
            if (Singleton<LobbyLogic>.instance.isLogin)
            {
                this.SendRoomDataMsg();
            }
            return true;
        }

        private bool UnPackSNSDataStr(string data)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if ((strArray != null) && (strArray[0] == "ShareRoom"))
            {
                return this.UnpackRoomData(data);
            }
            return (((strArray != null) && (strArray[0] == "ShareTeam")) && this.UnpackTeamData(data));
        }

        public bool UnpackTeamData(string data)
        {
            UnityEngine.Debug.Log("UnpackTeamData");
            char[] separator = new char[] { '_' };
            string[] strArray = data.Split(separator);
            if (((strArray != null) && (strArray.Length == 14)) && (strArray[0] == "ShareTeam"))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xbf).dwConfValue;
                if ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel < dwConfValue))
                {
                    object[] replaceArr = new object[] { dwConfValue };
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Level_Limit", true, 1f, null, replaceArr);
                    return false;
                }
                int result = 0;
                if (!int.TryParse(strArray[12], out result) || (Application.platform != result))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Device", true, 1.5f, null, new object[0]);
                    return false;
                }
                int num3 = -1;
                if (!int.TryParse(strArray[13], out num3) || (num3 != ApolloConfig.platform))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_Different_Platform", true, 1.5f, null, new object[0]);
                    return false;
                }
                if (Singleton<GameStateCtrl>.GetInstance().isBattleState || Singleton<GameStateCtrl>.GetInstance().isLoadingState)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Enter_Room_InBattle", true, 1.5f, null, new object[0]);
                    return false;
                }
                this.m_ShareTeam = new COMDT_INVITE_JOIN_INFO();
                this.m_ShareTeam.iInviteType = 2;
                this.m_ShareTeam.stInviteDetail.stInviteJoinTeam = new COMDT_INVITE_TEAM_DETAIL();
                try
                {
                    if ((((ulong.TryParse(strArray[1], out this.m_ShareTeam.stInviterInfo.ullUid) && uint.TryParse(strArray[2], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamId)) && (uint.TryParse(strArray[3], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.dwTeamSeq) && int.TryParse(strArray[4], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.iTeamEntity))) && ((ulong.TryParse(strArray[5], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.ullTeamFeature) && byte.TryParse(strArray[6], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.bInviterGradeOfRank)) && (byte.TryParse(strArray[7], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bGameMode) && byte.TryParse(strArray[8], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bPkAI)))) && ((byte.TryParse(strArray[9], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bMapType) && uint.TryParse(strArray[10], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.dwMapId)) && (byte.TryParse(strArray[11], out this.m_ShareTeam.stInviteDetail.stInviteJoinTeam.stTeamInfo.bAILevel) && Singleton<LobbyLogic>.instance.isLogin)))
                    {
                        this.SendTeamDataMsg();
                    }
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.Log(exception.ToString());
                }
            }
            return false;
        }

        public void UpdateShareGradeForm(CUIFormScript form)
        {
            if (form != null)
            {
                SetSharePlatfText(Utility.GetComponetInChild<Text>(form.gameObject, "ShareGroup/Button_TimeLine/ClickText"));
                if (this.m_bShowTimeline)
                {
                    Transform transform = null;
                    foreach (Text text in form.transform.GetComponentsInChildren<Text>())
                    {
                        if (((text != null) && (text.text == "分享空间")) || (text.text == "分享朋友圈"))
                        {
                            Transform parent = text.transform.parent;
                            if (parent.GetComponent<Button>() != null)
                            {
                                transform = parent;
                                break;
                            }
                        }
                    }
                    if (transform != null)
                    {
                        GameObject gameObject = transform.gameObject;
                        if ((gameObject != null) || this.m_bShowTimeline)
                        {
                            gameObject.GetComponent<CUIEventScript>().enabled = false;
                            gameObject.GetComponent<Button>().interactable = false;
                            gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
                            Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                            componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
                        }
                    }
                }
            }
        }

        public void UpdateSharePVPForm(CUIFormScript form, GameObject shareRootGO)
        {
            if (form != null)
            {
                SetSharePlatfText(Utility.GetComponetInChild<Text>(form.gameObject, "ShareGroup/Button_TimeLine/ClickText"));
                if (this.m_bShowTimeline)
                {
                    Transform transform = null;
                    foreach (Text text in form.transform.GetComponentsInChildren<Text>())
                    {
                        if (((text != null) && (text.text == "分享空间")) || (text.text == "分享朋友圈"))
                        {
                            Transform parent = text.transform.parent;
                            if (parent.GetComponent<Button>() != null)
                            {
                                transform = parent;
                                break;
                            }
                        }
                    }
                    if (transform != null)
                    {
                        GameObject gameObject = transform.gameObject;
                        if ((gameObject != null) || this.m_bShowTimeline)
                        {
                            gameObject.GetComponent<CUIEventScript>().enabled = false;
                            gameObject.GetComponent<Button>().interactable = false;
                            gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
                            Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                            componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
                        }
                    }
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(shareRootGO, "PlayerHead");
                    componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
                    Utility.GetComponetInChild<Text>(shareRootGO, "PlayerName").text = masterRoleInfo.Name;
                    DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                    PlayerKDA rkda = null;
                    int[] numArray = new int[3];
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                        PlayerKDA rkda2 = current.Value;
                        if (rkda2.IsHost)
                        {
                            rkda = rkda2;
                        }
                        numArray[(int) rkda2.PlayerCamp] += rkda2.numKill;
                    }
                    Utility.FindChild(componetInChild.gameObject, "Mvp").CustomSetActive(Singleton<BattleStatistic>.instance.GetMvpPlayer(rkda.PlayerCamp, this.m_bWinPVPResult) == rkda.PlayerId);
                    if (rkda != null)
                    {
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostKillNum").text = rkda.numKill.ToString();
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostDeadNum").text = rkda.numDead.ToString();
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostAssistNum").text = rkda.numAssist.ToString();
                        Utility.GetComponetInChild<Text>(shareRootGO, "HostKillTotalNum").text = numArray[(int) rkda.PlayerCamp].ToString();
                        Utility.GetComponetInChild<Text>(shareRootGO, "OppoKillTotalNum").text = numArray[(int) BattleLogic.GetOppositeCmp(rkda.PlayerCamp)].ToString();
                        ListView<HeroKDA>.Enumerator enumerator2 = rkda.GetEnumerator();
                        if (enumerator2.MoveNext())
                        {
                            HeroKDA okda = enumerator2.Current;
                            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) okda.HeroId);
                            Utility.GetComponetInChild<Image>(shareRootGO, "HeroHead").SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, true, false, false);
                            int num2 = 1;
                            for (int i = 1; i < 13; i++)
                            {
                                switch (((PvpAchievement) i))
                                {
                                    case PvpAchievement.Legendary:
                                        if (okda.LegendaryNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.Legendary, num2++);
                                        }
                                        break;

                                    case PvpAchievement.PentaKill:
                                        if (okda.PentaKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.PentaKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.QuataryKill:
                                        if (okda.QuataryKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.QuataryKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.TripleKill:
                                        if (okda.TripleKillNum > 0)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.TripleKill, num2++);
                                        }
                                        break;

                                    case PvpAchievement.DoubleKill:
                                        if (okda.DoubleKillNum <= 0)
                                        {
                                        }
                                        break;

                                    case PvpAchievement.KillMost:
                                        if (okda.bKillMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.KillMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.HurtMost:
                                        if (okda.bHurtMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.HurtTakenMost:
                                        if (okda.bHurtTakenMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.HurtTakenMost, num2++);
                                        }
                                        break;

                                    case PvpAchievement.AsssistMost:
                                        if (okda.bAsssistMost)
                                        {
                                            CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.AsssistMost, num2++);
                                        }
                                        break;
                                }
                            }
                            for (int j = num2; j <= 6; j++)
                            {
                                CSettlementView.SetAchievementIcon(form, shareRootGO, PvpAchievement.NULL, j);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTimelineBtn()
        {
            if (this.m_TimelineBtn != null)
            {
                GameObject gameObject = this.m_TimelineBtn.gameObject;
                if (this.m_bShowTimeline && (gameObject != null))
                {
                    gameObject.GetComponent<CUIEventScript>().enabled = false;
                    gameObject.GetComponent<Button>().interactable = false;
                    gameObject.GetComponent<Image>().color = new Color(gameObject.GetComponent<Image>().color.r, gameObject.GetComponent<Image>().color.g, gameObject.GetComponent<Image>().color.b, 0.37f);
                    Text componentInChildren = gameObject.GetComponentInChildren<Text>();
                    componentInChildren.color = new Color(componentInChildren.color.r, componentInChildren.color.g, componentInChildren.color.b, 0.37f);
                }
                this.m_TimelineBtn = null;
            }
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Share_ClosePVPAchievement);
        }

        [CompilerGenerated]
        private sealed class <Capture>c__Iterator22 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>callback;
            internal Rect <$>screenShotRect;
            internal ShareSys <>f__this;
            internal Color <c>__6;
            internal byte[] <data>__7;
            internal Exception <e>__8;
            internal string <filename>__0;
            internal int <i>__5;
            internal Color[] <noAlphaColors>__4;
            internal Texture2D <result>__1;
            internal Texture2D <src>__2;
            internal Color[] <srcColors>__3;
            internal Action<string> callback;
            internal Rect screenShotRect;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        try
                        {
                            this.<filename>__0 = this.<>f__this.m_sharePic;
                            this.<result>__1 = null;
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                if (this.<>f__this.m_bAdreo306)
                                {
                                    this.<src>__2 = new Texture2D((int) this.screenShotRect.width, (int) this.screenShotRect.height, TextureFormat.ARGB32, false);
                                    this.<src>__2.ReadPixels(this.screenShotRect, 0, 0);
                                    this.<src>__2.Apply();
                                    this.<srcColors>__3 = this.<src>__2.GetPixels();
                                    this.<noAlphaColors>__4 = new Color[this.<srcColors>__3.Length];
                                    this.<i>__5 = 0;
                                    while (this.<i>__5 < this.<srcColors>__3.Length)
                                    {
                                        this.<c>__6 = this.<srcColors>__3[this.<i>__5];
                                        this.<noAlphaColors>__4[this.<i>__5] = new Color(this.<c>__6.r, this.<c>__6.g, this.<c>__6.b);
                                        this.<i>__5++;
                                    }
                                    this.<result>__1 = new Texture2D((int) this.screenShotRect.width, (int) this.screenShotRect.height, TextureFormat.RGB24, false);
                                    this.<result>__1.SetPixels(this.<noAlphaColors>__4);
                                    this.<result>__1.Apply();
                                    UnityEngine.Object.Destroy(this.<src>__2);
                                }
                                else
                                {
                                    this.<result>__1 = new Texture2D((int) this.screenShotRect.width, (int) this.screenShotRect.height, TextureFormat.RGB24, false);
                                    this.<result>__1.ReadPixels(this.screenShotRect, 0, 0);
                                    this.<result>__1.Apply();
                                }
                            }
                            else
                            {
                                this.<result>__1 = new Texture2D((int) this.screenShotRect.width, (int) this.screenShotRect.height, TextureFormat.RGB24, false);
                                this.<result>__1.ReadPixels(this.screenShotRect, 0, 0);
                                this.<result>__1.Apply();
                            }
                            this.<data>__7 = null;
                            if (this.<result>__1 != null)
                            {
                                this.<data>__7 = this.<result>__1.EncodeToJPG();
                                UnityEngine.Object.Destroy(this.<result>__1);
                            }
                            if (this.<data>__7 != null)
                            {
                                CFileManager.WriteFile(this.<filename>__0, this.<data>__7);
                            }
                            if (this.callback != null)
                            {
                                this.callback(this.<filename>__0);
                            }
                        }
                        catch (Exception exception)
                        {
                            this.<e>__8 = exception;
                            object[] inParameters = new object[] { this.<e>__8.Message };
                            DebugHelper.Assert(false, "Exception in ShareSys.Capture, {0}", inParameters);
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadImageByTag2>c__Iterator21 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ShareSys.LoadRCallBack3 <$>callBack;
            internal string <$>preUrl;
            internal ShareSys.CLoadReq <$>req;
            internal string <$>tagPath;
            internal ShareSys <>f__this;
            internal float <beginTime>__3;
            internal string <key>__0;
            internal string <localCachePath>__1;
            internal bool <outOfTime>__4;
            internal Texture2D <tex>__5;
            internal WWW <www>__2;
            internal ShareSys.LoadRCallBack3 callBack;
            internal string preUrl;
            internal ShareSys.CLoadReq req;
            internal string tagPath;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<key>__0 = MonoSingleton<IDIPSys>.GetInstance().ToMD5(this.preUrl);
                        this.<localCachePath>__1 = CFileManager.CombinePath(this.tagPath, this.<key>__0);
                        this.<www>__2 = null;
                        this.<www>__2 = new WWW(this.preUrl);
                        this.<beginTime>__3 = Time.time;
                        this.<outOfTime>__4 = false;
                        break;

                    case 1:
                        if ((Time.time - this.<beginTime>__3) <= this.<>f__this.g_fDownloadOutTime)
                        {
                            break;
                        }
                        this.<outOfTime>__4 = true;
                        goto Label_00DD;

                    default:
                        goto Label_01D2;
                }
                if (!this.<www>__2.isDone && string.IsNullOrEmpty(this.<www>__2.error))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
            Label_00DD:
                if (this.<outOfTime>__4)
                {
                    this.req.m_LoadError = ShareSys.ELoadError.OutOfTime;
                    this.<www>__2.Dispose();
                    this.callBack(null, this.req);
                }
                else if (!string.IsNullOrEmpty(this.<www>__2.error))
                {
                    this.req.m_LoadError = ShareSys.ELoadError.NotFound;
                    this.<www>__2.Dispose();
                    this.callBack(null, this.req);
                }
                else
                {
                    this.req.m_LoadError = ShareSys.ELoadError.SUCC;
                    this.<tex>__5 = this.<www>__2.texture;
                    if ((this.<tex>__5 != null) && (this.<localCachePath>__1 != null))
                    {
                        CFileManager.WriteFile(this.<localCachePath>__1, this.<www>__2.bytes);
                    }
                    if (this.callBack != null)
                    {
                        this.callBack(this.<tex>__5, this.req);
                    }
                }
                this.$PC = -1;
            Label_01D2:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PreLoadShareSkinImage>c__AnonStorey85
        {
            internal ShareSys <>f__this;
            internal ShareSys.CLoadReq loadReq;

            internal void <>m__8A(Texture2D text2, ShareSys.CLoadReq tempLoadReq)
            {
                if (this.<>f__this.m_DownLoadSkinList.Count > 0)
                {
                    for (int i = this.<>f__this.m_DownLoadSkinList.Count - 1; i >= 0; i--)
                    {
                        ShareSys.CLoadReq item = this.<>f__this.m_DownLoadSkinList[i];
                        if (item.m_Url == tempLoadReq.m_Url)
                        {
                            this.<>f__this.m_DownLoadSkinList.Remove(item);
                            if (tempLoadReq.m_LoadError != ShareSys.ELoadError.SUCC)
                            {
                                this.<>f__this.m_DownLoadSkinList.Add(tempLoadReq);
                            }
                        }
                    }
                }
                UnityEngine.Debug.Log("skic share pic tempLoadReq " + tempLoadReq.m_LoadError);
                if ((this.<>f__this.m_bShareHero && (this.<>f__this.m_ShareSkinPicImage != null)) && (this.loadReq.m_Url == this.<>f__this.m_ShareInfo.shareSkinUrl))
                {
                    if (tempLoadReq.m_LoadError == ShareSys.ELoadError.SUCC)
                    {
                        this.<>f__this.m_ShareSkinPicImage.gameObject.CustomSetActive(true);
                        Image component = this.<>f__this.m_ShareSkinPicImage.GetComponent<Image>();
                        if (component != null)
                        {
                            component.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float) text2.width, (float) text2.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                        }
                        if (this.<>f__this.m_FriendBtnImage != null)
                        {
                            this.<>f__this.BtnGray(this.<>f__this.m_FriendBtnImage, true);
                        }
                        if ((this.<>f__this.m_TimeLineBtnImage != null) && !this.<>f__this.m_bShowTimeline)
                        {
                            this.<>f__this.BtnGray(this.<>f__this.m_TimeLineBtnImage, true);
                        }
                    }
                    else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.OutOfTime)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(this.<>f__this.m_ShareSkinPicOutofTime, false, 1.5f, null, new object[0]);
                    }
                    else if (tempLoadReq.m_LoadError == ShareSys.ELoadError.NotFound)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(this.<>f__this.m_ShareSkinPicNotFound, false, 1.5f, null, new object[0]);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SavePic>c__AnonStorey88
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;

            internal void <>m__8D(string filename)
            {
                if (this.btnClose != null)
                {
                    this.btnClose.CustomSetActive(true);
                }
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                }
                if (Application.platform == RuntimePlatform.Android)
                {
                    try
                    {
                        string path = "/mnt/sdcard/DCIM/Sgame";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        path = string.Format("{0}/share_{1}.png", path, DateTime.Now.ToFileTimeUtc());
                        UnityEngine.Debug.Log("sns += SavePic " + path);
                        FileStream stream = new FileStream(this.<>f__this.m_sharePic, FileMode.Open, FileAccess.Read);
                        byte[] array = new byte[stream.Length];
                        int count = Convert.ToInt32(stream.Length);
                        stream.Read(array, 0, count);
                        stream.Close();
                        File.WriteAllBytes(path, array);
                        this.<>f__this.RefeshPhoto(path);
                        Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
                    }
                    catch (Exception exception)
                    {
                        object[] inParameters = new object[] { exception.Message };
                        DebugHelper.Assert(false, "Error In Save Pic, {0}", inParameters);
                        Singleton<CUIManager>.instance.OpenTips("保存到相册出错", false, 1.5f, null, new object[0]);
                    }
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    this.<>f__this.RefeshPhoto(this.<>f__this.m_sharePic);
                    Singleton<CUIManager>.instance.OpenTips("成功保存到相册", false, 1.5f, null, new object[0]);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShareFriend>c__AnonStorey86
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;

            internal void <>m__8B(string filename)
            {
                UnityEngine.Debug.Log("sns += capture showfriend filename" + filename);
                this.<>f__this.Share("Session", this.<>f__this.m_sharePic);
                this.btnClose.CustomSetActive(true);
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                    this.bSettltment = false;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ShareTimeLine>c__AnonStorey87
        {
            internal ShareSys <>f__this;
            internal bool bSettltment;
            internal GameObject btnClose;

            internal void <>m__8C(string filename)
            {
                UnityEngine.Debug.Log("sns += capture showfriend filename" + filename);
                this.<>f__this.Share("TimeLine/Qzone", this.<>f__this.m_sharePic);
                this.btnClose.CustomSetActive(true);
                if (this.bSettltment)
                {
                    Singleton<SettlementSystem>.GetInstance().SnapScreenShotShowBtn(true);
                    this.bSettltment = false;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CLoadReq
        {
            public string m_Url;
            public ShareSys.ELoadError m_LoadError;
            public string m_CachePath;
            public int m_Type;
        }

        public enum ELoadError
        {
            None,
            SUCC,
            NotFound,
            OutOfTime
        }

        private enum HeroShareFormWidgets
        {
            DisplayRect,
            HeroAmount,
            ButtonClose,
            ShareClickText,
            ShareFriendBtn,
            TimeLineBtn,
            SkinAmount
        }

        public delegate void LoadRCallBack3(Texture2D image, ShareSys.CLoadReq req);

        private enum MysteryDiscountFOrmWigets
        {
            DiscountNum,
            ButtonClose,
            ShareClickText
        }

        public enum PVPShareFormWidgets
        {
            DisplayRect,
            ButtonClose,
            ShareImg
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHARE_INFO
        {
            public uint heroId;
            public uint skinId;
            public COM_REWARDS_TYPE rewardType;
            public float beginDownloadTime;
            public string shareSkinUrl;
            public void clear()
            {
                this.shareSkinUrl = string.Empty;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShareActivityParam
        {
            public byte bShareType;
            public byte bParamCnt;
            public uint[] ShareParam;
            public bool bUsed;
            public ShareActivityParam(bool buse)
            {
                this.bUsed = buse;
                this.bShareType = 0;
                this.bParamCnt = 0;
                this.ShareParam = null;
            }

            public void clear()
            {
                this.bUsed = false;
                this.ShareParam = null;
                this.bParamCnt = 0;
                this.bShareType = 0;
            }

            public void set(byte kShareType, byte kParamCnt, uint[] kShareParam)
            {
                this.clear();
                this.bUsed = true;
                this.bShareType = kShareType;
                this.bParamCnt = kParamCnt;
                this.ShareParam = new uint[kParamCnt];
                for (int i = 0; i < kParamCnt; i++)
                {
                    this.ShareParam[i] = kShareParam[i];
                }
            }
        }
    }
}

