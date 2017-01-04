namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.UI;
    using com.tencent.pandora;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PandroaSys : MonoSingleton<PandroaSys>
    {
        private bool m_bInit;
        public bool m_bShowBoxBtn;
        private bool m_bShowPopNew;
        public bool m_bShowRedPoint;

        protected override void Init()
        {
            base.Init();
        }

        private void InitEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
        }

        private void InitPara()
        {
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            string str = "Smoba";
            string openId = accountInfo.OpenId;
            string str3 = "qq";
            string str4 = string.Empty;
            string str5 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID.ToString();
            string str6 = string.Empty;
            string str7 = string.Empty;
            foreach (ApolloToken token in accountInfo.TokenList)
            {
                if (ApolloConfig.platform == ApolloPlatform.Wechat)
                {
                    if (token.Type == ApolloTokenType.Access)
                    {
                        str6 = token.Value;
                    }
                }
                else if (ApolloConfig.platform == ApolloPlatform.QQ)
                {
                    if (token.Type == ApolloTokenType.Pay)
                    {
                        str7 = token.Value;
                    }
                    if (token.Type == ApolloTokenType.Access)
                    {
                        str6 = token.Value;
                    }
                }
            }
            if (ApolloConfig.platform == ApolloPlatform.QQ)
            {
                str3 = "qq";
                if (Application.platform == RuntimePlatform.Android)
                {
                    str4 = "1";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    str4 = "2";
                }
            }
            else if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                str3 = "wx";
                if (Application.platform == RuntimePlatform.Android)
                {
                    str4 = "3";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    str4 = "4";
                }
            }
            string str8 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString();
            string appID = ApolloConfig.appID;
            if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                appID = ApolloConfig.WXAppID;
            }
            string appVersion = CVersion.GetAppVersion();
            string str11 = "1";
            Pandora.setParentGameObject(base.gameObject);
            Pandora.setDepth(0x3e8);
            NotificationCenter.DefaultCenter().AddObserver(this, "OnPandoraEvent");
            Dictionary<string, string> dicPara = new Dictionary<string, string>();
            dicPara.Add("sOpenId", openId);
            dicPara.Add("sServiceType", str);
            dicPara.Add("sAcountType", str3);
            dicPara.Add("sArea", str4);
            dicPara.Add("sPartition", str8);
            dicPara.Add("sAppId", appID);
            dicPara.Add("sRoleId", str5);
            dicPara.Add("sAccessToken", str6);
            dicPara.Add("sPayToken", str7);
            dicPara.Add("sGameVer", appVersion);
            dicPara.Add("sPlatID", str11);
            Pandora.GetInstance().SetUserInfo(dicPara);
        }

        public void InitSys()
        {
            this.InitEvent();
            Debug.Log("Pandora InitSys");
            this.m_bInit = true;
            this.m_bShowPopNew = false;
            this.m_bShowBoxBtn = false;
            this.m_bShowRedPoint = false;
            this.InitPara();
            Pandora.Init();
        }

        public void OnPandoraEvent(object data)
        {
            com.tencent.pandora.Logger.d("OnPandoraEvent enter");
            if (this.m_bInit)
            {
                Notification notification = (Notification) data;
                string json = (string) notification.data;
                Dictionary<string, object> dictionary = null;
                try
                {
                    dictionary = Json.Deserialize(json) as Dictionary<string, object>;
                }
                catch (Exception exception)
                {
                    com.tencent.pandora.Logger.d("协议无法解析:" + json + "," + exception.Message);
                    return;
                }
                if ((dictionary != null) && dictionary.ContainsKey("type"))
                {
                    string str2 = dictionary["type"].ToString();
                    string s = string.Empty;
                    if (dictionary.ContainsKey("content"))
                    {
                        s = dictionary["content"].ToString();
                    }
                    if (str2 == "redpoint")
                    {
                        int result = 0;
                        int.TryParse(s, out result);
                        if (result <= 0)
                        {
                            this.m_bShowRedPoint = false;
                        }
                        else
                        {
                            this.m_bShowRedPoint = true;
                        }
                        Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
                        Singleton<EventRouter>.instance.BroadCastEvent("IDIPNOTICE_UNREAD_NUM_UPDATE");
                    }
                    else if (str2.Equals("showIcon"))
                    {
                        if (s.Equals("1"))
                        {
                            this.m_bShowBoxBtn = true;
                        }
                        else
                        {
                            this.m_bShowBoxBtn = false;
                        }
                        Singleton<ActivitySys>.GetInstance().PandroaUpdateBtn();
                    }
                    else if (str2.Equals("closePopNews"))
                    {
                        this.m_bShowPopNew = true;
                    }
                    else if (str2.Equals("ShowGameWindow"))
                    {
                        int num2 = 0;
                        int.TryParse(s, out num2);
                        com.tencent.pandora.Logger.d("ShowGameWindow:" + num2);
                        if (num2 > 0)
                        {
                            CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) num2);
                        }
                    }
                    else if (str2.Equals("refreshDiamond"))
                    {
                        Debug.Log("pandorasys ShowGameWindow");
                        Singleton<CPaySystem>.GetInstance().OnPandroiaPaySuccss();
                    }
                }
                com.tencent.pandora.Logger.d("strCmd:" + json);
            }
        }

        private void OnShowActBox(CUIEvent uiEvent)
        {
            if (this.m_bInit)
            {
                this.ShowActBox();
            }
        }

        public void PausePandoraSys(bool bPause)
        {
            if (bPause)
            {
                string strJson = "{\"type\":\"inMainSence\",\"content\":\"0\"}";
                Pandora.Do(strJson);
            }
            else
            {
                string str2 = "{\"type\":\"inMainSence\",\"content\":\"1\"}";
                Pandora.Do(str2);
            }
        }

        public void ShowActBox()
        {
            Debug.Log("Pandora ShowActBox1");
            if (this.m_bInit)
            {
                Debug.Log("Pandora ShowActBox2");
                string strJson = "{\"type\":\"open\",\"content\":\"Lucky\"}";
                Pandora.Do(strJson);
            }
        }

        public void ShowActiveActBoxBtn(CUIFormScript uiForm)
        {
            if (uiForm != null)
            {
                if (this.m_bInit)
                {
                    Transform transform = uiForm.gameObject.transform.Find("Panel/PandroaBtn");
                    if (transform != null)
                    {
                        if (this.m_bShowBoxBtn)
                        {
                            transform.gameObject.CustomSetActive(true);
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                        Transform transform2 = transform.Find("Hotspot");
                        if (transform2 != null)
                        {
                            if (this.m_bShowRedPoint)
                            {
                                transform2.gameObject.CustomSetActive(true);
                            }
                            else
                            {
                                transform2.gameObject.CustomSetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    Transform transform3 = uiForm.gameObject.transform.Find("Panel/PandroaBtn");
                    if (transform3 != null)
                    {
                        transform3.gameObject.CustomSetActive(false);
                        Transform transform4 = transform3.Find("Hotspot");
                        if (transform4 != null)
                        {
                            transform4.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void ShowPopNews()
        {
            if (this.m_bInit && !this.m_bShowPopNew)
            {
                this.m_bShowPopNew = true;
                string strJson = "{\"type\":\"open\",\"content\":\"LuckyPop\"}";
                Pandora.Do(strJson);
            }
        }

        public void UninitSys()
        {
            this.m_bInit = false;
            this.m_bShowPopNew = false;
            this.m_bShowBoxBtn = false;
            this.m_bShowRedPoint = false;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pandroa_ShowActBox, new CUIEventManager.OnUIEventHandler(this.OnShowActBox));
            Pandora.LogOutAccount();
        }

        public bool ShowRedPoint
        {
            get
            {
                return this.m_bShowRedPoint;
            }
        }
    }
}

