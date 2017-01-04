using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NoticeSys : MonoSingleton<NoticeSys>
{
    [CompilerGenerated]
    private static IDIPSys.LoadRCallBack <>f__am$cache13;
    [CompilerGenerated]
    private static IDIPSys.LoadRCallBack <>f__am$cache14;
    private bool m_bGoto;
    private bool m_bLoadImage;
    private bool m_bShow;
    public static bool m_bShowLoginBefore = false;
    private BTN_DOSOMTHING m_btnDoSth;
    private string m_btnUrl = string.Empty;
    private NOTICE_STATE m_CurState;
    private CUIFormScript m_Form;
    private Image m_ImageContent;
    private Image m_ImageDefault;
    private string m_ImageModleTitle = string.Empty;
    private GameObject m_ImageTop;
    private ListView<ApolloNoticeData> m_NoticeDataList = new ListView<ApolloNoticeData>();
    private Image m_PanelImage;
    private Text m_TextContent;
    private Text m_Title;
    private GameObject m_TitleBoard;
    private UrlAction m_urlAction;
    public static string s_formNoticeLoginPath = (CUIUtility.s_IDIP_Form_Dir + "Form_NoticeLoginBefore.prefab");

    private bool CheckIsBtnUrl(string msgUrl, ref bool bTitle, ref string sTitle, ref BTN_DOSOMTHING btnDoSth, ref string url)
    {
        if (msgUrl == null)
        {
            url = string.Empty;
            btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
            bTitle = false;
            return false;
        }
        string str = "#";
        string str2 = "&end";
        string str3 = "&";
        string str4 = msgUrl;
        int index = str4.IndexOf(str);
        if (index >= 0)
        {
            int num2 = str4.IndexOf(str2);
            if (num2 < 0)
            {
                url = string.Empty;
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
                bTitle = false;
                return false;
            }
            int num3 = str4.IndexOf(str3);
            if (num3 < 0)
            {
                url = string.Empty;
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
                bTitle = false;
                return false;
            }
            string str5 = str4.Substring(index + str.Length, (num3 - index) - str.Length);
            if (str5.Contains("title="))
            {
                string stringToUnescape = str5.Substring("title=".Length);
                sTitle = Uri.UnescapeDataString(stringToUnescape);
                bTitle = true;
            }
            else if (str5.Contains("button=0"))
            {
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_URL;
            }
            else if (str5.Contains("button=1"))
            {
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_GAME;
            }
            else if (str5.Contains("button=2"))
            {
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NOTSHOW;
            }
            string str7 = string.Empty;
            if (((num2 - num3) - str3.Length) > 0)
            {
                str7 = str4.Substring(num3 + str3.Length, (num2 - num3) - str3.Length);
            }
            if (bTitle)
            {
                url = str7;
                btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
                return true;
            }
            if (str7.Contains("url="))
            {
                url = str7.Substring("url=".Length);
                return true;
            }
            url = string.Empty;
            btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
            bTitle = false;
            return false;
        }
        url = string.Empty;
        btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
        bTitle = false;
        return false;
    }

    public void DelayShowNoticeWindow()
    {
        this.ShowNoticeWindow(0);
    }

    protected override void Init()
    {
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MSDK_NOTICE_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseIDIPForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MSDK_NOTICE_Btn_Complete, new CUIEventManager.OnUIEventHandler(this.OnBtnComplete));
        Singleton<CTimerManager>.GetInstance().AddTimer(0xbb8, 1, new CTimer.OnTimeUpHandler(this.OnTimeEnd));
    }

    private void OnBtnComplete(CUIEvent ciEvent)
    {
        if ((this.m_urlAction != null) && this.m_urlAction.Execute())
        {
            this.OnCloseIDIPForm(null);
        }
        else if ((this.m_bGoto && this.m_bShow) && (this.m_btnUrl != string.Empty))
        {
            BTN_DOSOMTHING btnDoSth = this.m_btnDoSth;
            string btnUrl = this.m_btnUrl;
            this.OnCloseIDIPForm(null);
            switch (btnDoSth)
            {
                case BTN_DOSOMTHING.BTN_DOSOMTHING_URL:
                    CUICommonSystem.OpenUrl(btnUrl, true);
                    break;

                case BTN_DOSOMTHING.BTN_DOSOMTHING_GAME:
                {
                    int result = 0;
                    int.TryParse(btnUrl, out result);
                    if (result > 0)
                    {
                        CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) result);
                    }
                    break;
                }
            }
            btnUrl = string.Empty;
            btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
        }
    }

    private void OnCloseIDIPForm(CUIEvent uiEvent)
    {
        if (this.m_Form != null)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(this.m_Form);
            this.m_Form = null;
        }
        this.m_bShow = false;
        if (((this.m_ImageContent != null) && (this.m_ImageContent.sprite != null)) && this.m_bLoadImage)
        {
            UnityEngine.Object.Destroy(this.m_ImageContent.sprite);
        }
        if (this.m_NoticeDataList.Count > 0)
        {
            this.ShowNoticeWindow(0);
        }
        else
        {
            this.ShowOtherTips();
        }
    }

    public void OnOpenForm(ApolloNoticeInfo noticeInfo, NOTICE_STATE noticeState)
    {
        this.m_CurState = noticeState;
        this.m_NoticeDataList = new ListView<ApolloNoticeData>();
        int count = noticeInfo.DataList.Count;
        for (int i = 0; i < count; i++)
        {
            ApolloNoticeData item = noticeInfo.DataList[i];
            if (item.MsgType == APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT)
            {
                this.m_NoticeDataList.Add(item);
            }
        }
        if (count > 0)
        {
            this.ShowNoticeWindow(0);
        }
        else if (this.m_CurState == NOTICE_STATE.LOGIN_After)
        {
            this.ShowOtherTips();
        }
    }

    private void OnTimeEnd(int timersequence)
    {
        this.PreLoadImage();
    }

    public void PreLoadImage()
    {
        List<string> noticeUrl = Singleton<ApolloHelper>.GetInstance().GetNoticeUrl(0, "1");
        for (int i = 0; i < noticeUrl.Count; i++)
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = text2 => BugLocateLogSys.Log("preloadimage ok");
            }
            base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(noticeUrl[i], <>f__am$cache13));
        }
        List<string> list2 = Singleton<ApolloHelper>.GetInstance().GetNoticeUrl(0, "2");
        for (int j = 0; j < list2.Count; j++)
        {
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = text2 => BugLocateLogSys.Log("preloadimage ok");
            }
            base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(list2[j], <>f__am$cache14));
        }
    }

    private void ProcessButton(uint nLevel, bool isGoto)
    {
        if (this.m_Form != null)
        {
            Transform transform = this.m_Form.gameObject.transform.Find("Panel/Image/Button_Complte");
            if (nLevel <= 5)
            {
                transform.gameObject.CustomSetActive(false);
            }
            else if (isGoto)
            {
                transform.gameObject.CustomSetActive(true);
            }
            else
            {
                transform.gameObject.CustomSetActive(false);
            }
        }
    }

    private void ProcessShowNoticeWindown(ApolloNoticeData noticeData)
    {
        this.m_bGoto = false;
        this.m_bLoadImage = false;
        string msgID = noticeData.MsgID;
        string openID = noticeData.OpenID;
        string msgUrl = noticeData.MsgUrl;
        ListView<UrlAction> view = UrlAction.ParseFromText(noticeData.ContentUrl, null);
        if (view.Count > 0)
        {
            this.m_urlAction = view[0];
        }
        else
        {
            this.m_urlAction = null;
        }
        if (msgUrl == null)
        {
            msgUrl = string.Empty;
        }
        APOLLO_NOTICETYPE msgType = noticeData.MsgType;
        string startTime = noticeData.StartTime;
        APOLLO_NOTICE_CONTENTTYPE contentType = noticeData.ContentType;
        string msgTitle = noticeData.MsgTitle;
        string msgContent = noticeData.MsgContent;
        Debug.Log(string.Concat(new object[] { "noticesysy onopenform MsgUrl", msgUrl, "msgtitle = ", msgTitle, " content ", msgContent, " openid= ", openID, " MsgType  = ", msgType, "Msg Scene", noticeData.MsgScene }));
        uint pvpLevel = 0;
        if (this.m_CurState == NOTICE_STATE.LOGIN_After)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                pvpLevel = masterRoleInfo.PvpLevel;
            }
        }
        else
        {
            pvpLevel = 0;
        }
        this.m_btnUrl = string.Empty;
        this.m_btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
        if (msgType == APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT)
        {
            bool bTitle = false;
            if (this.CheckIsBtnUrl(msgUrl, ref bTitle, ref this.m_ImageModleTitle, ref this.m_btnDoSth, ref this.m_btnUrl))
            {
                if (bTitle)
                {
                    bool flag3 = false;
                    bool flag2 = this.CheckIsBtnUrl("#" + this.m_btnUrl + "&end", ref flag3, ref this.m_ImageModleTitle, ref this.m_btnDoSth, ref this.m_btnUrl);
                }
                if ((this.m_btnDoSth == BTN_DOSOMTHING.BTN_DOSOMTHING_NOTSHOW) && (this.m_btnUrl != MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()))
                {
                    Debug.Log("noticesys not show " + this.m_btnUrl);
                    return;
                }
                Debug.Log("find url " + this.m_btnUrl + " ori = " + msgUrl);
            }
            else
            {
                Debug.Log("find url   ori = " + msgUrl);
            }
            if ((this.m_CurState == NOTICE_STATE.LOGIN_After) && (pvpLevel <= 5))
            {
                this.m_btnDoSth = BTN_DOSOMTHING.BTN_DOSOMTHING_NONE;
                this.m_btnUrl = string.Empty;
            }
            if (this.m_Form == null)
            {
                this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(s_formNoticeLoginPath, false, true);
            }
            Transform transform = this.m_Form.gameObject.transform.Find("Panel/BtnGroup/Button_Complte");
            if ((this.m_btnUrl != string.Empty) && (this.m_btnDoSth != BTN_DOSOMTHING.BTN_DOSOMTHING_NONE))
            {
                this.m_bGoto = true;
            }
            else
            {
                this.m_bGoto = false;
            }
            this.m_bShow = true;
            this.m_TextContent = Utility.GetComponetInChild<Text>(this.m_Form.gameObject, "Panel/ScrollRect/Content/Text");
            this.m_ImageContent = Utility.GetComponetInChild<Image>(this.m_Form.gameObject, "Panel/Image");
            this.m_ImageDefault = Utility.GetComponetInChild<Image>(this.m_Form.gameObject, "Panel/ImageDefalut");
            this.m_Title = Utility.GetComponetInChild<Text>(this.m_Form.gameObject, "Panel/GameObject/Title/ContentTitle");
            this.m_TitleBoard = this.m_Form.gameObject.transform.Find("Panel/GameObject/Title").gameObject;
            this.m_TextContent.gameObject.CustomSetActive(false);
            this.m_ImageContent.gameObject.CustomSetActive(false);
            if (this.m_ImageDefault != null)
            {
                this.m_ImageDefault.gameObject.CustomSetActive(false);
            }
            this.m_Title.text = msgTitle;
            switch (contentType)
            {
                case APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_IMAGE:
                    return;

                case APOLLO_NOTICE_CONTENTTYPE.APO_SCONTENTTYPE_TEXT:
                {
                    this.m_TextContent.gameObject.CustomSetActive(true);
                    this.m_TextContent.text = msgContent;
                    this.m_TitleBoard.CustomSetActive(true);
                    RectTransform component = this.m_TextContent.transform.parent.gameObject.GetComponent<RectTransform>();
                    if (component != null)
                    {
                        component.sizeDelta = new Vector2(0f, this.m_TextContent.preferredHeight + 50f);
                    }
                    return;
                }
            }
            if ((contentType == APOLLO_NOTICE_CONTENTTYPE.APO_CONTENTTYPE_WEB) && (this.m_urlAction != null))
            {
                this.m_bShow = true;
                this.m_TitleBoard.CustomSetActive(true);
                this.m_Title.text = this.m_ImageModleTitle;
                if (this.m_ImageDefault != null)
                {
                    this.m_ImageDefault.gameObject.CustomSetActive(true);
                }
                this.m_ImageContent.gameObject.CustomSetActive(false);
                base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImage(this.m_urlAction.target, delegate (Texture2D text2) {
                    if (this.m_bShow && (this.m_ImageContent != null))
                    {
                        this.m_ImageContent.gameObject.CustomSetActive(true);
                        if (this.m_ImageDefault != null)
                        {
                            this.m_ImageDefault.gameObject.CustomSetActive(false);
                        }
                        this.m_bLoadImage = true;
                        this.m_ImageContent.SetSprite(Sprite.Create(text2, new Rect(0f, 0f, (float) text2.width, (float) text2.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                        BugLocateLogSys.Log("noticesysy contenturl " + this.m_urlAction.target);
                    }
                }));
            }
        }
    }

    private void ShowNoticeWindow(int idx)
    {
        if (((this.m_NoticeDataList != null) && (idx >= 0)) && (idx < this.m_NoticeDataList.Count))
        {
            ApolloNoticeData item = this.m_NoticeDataList[idx];
            this.m_NoticeDataList.Remove(item);
            this.ProcessShowNoticeWindown(item);
        }
    }

    private void ShowOtherTips()
    {
        if (Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState)
        {
            if (((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null) && ActivitySys.NeedShowWhenLogin()) && !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding)
            {
                ActivitySys.UpdateLoginShowCnt();
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_OpenForm);
            }
            else
            {
                MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeLoseTipsInfo();
                MonoSingleton<PandroaSys>.GetInstance().ShowPopNews();
            }
        }
    }

    public enum BTN_DOSOMTHING
    {
        BTN_DOSOMTHING_NONE,
        BTN_DOSOMTHING_URL,
        BTN_DOSOMTHING_GAME,
        BTN_DOSOMTHING_NOTSHOW
    }

    public enum NOTICE_STATE
    {
        LOGIN_Before,
        LOGIN_After
    }

    public static class UrlX
    {
        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char) (n + 0x30);
            }
            return (char) ((n - 10) + 0x61);
        }

        private static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '!':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;
            }
            return false;
        }

        public static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes));
        }

        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        public static byte[] UrlEncodeBytesToBytes(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char) bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char) num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte) IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte) IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlEncodeBytesToBytes(bytes, 0, bytes.Length, false);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncodeToBytes(e.GetBytes(str));
        }
    }
}

