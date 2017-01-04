namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CPaySystem : Singleton<CPaySystem>
    {
        private ListView<ResCouponsBuyInfo> dianQuanBuyInfoList = new ListView<ResCouponsBuyInfo>();
        private int m_dianQuanCnt;
        private int m_dianQuanGiftId;
        private ulong m_lastDianQuan;
        private int m_pandoraDianQuanReqSeq = -1;
        private ListView<CUseable> rewardItems = new ListView<CUseable>();
        public static string s_buyDianQuanFormPath = "UGUI/Form/System/Pay/Form_BuyDiamond.prefab";
        public static string s_firstPayFormPath = "UGUI/Form/System/Pay/Form_FirstPayDiamond.prefab";
        public static string s_renewalDianQuanFormPath = "UGUI/Form/System/Pay/Form_RenewalDiamond.prefab";

        public void AutoOpenRewardPanel(bool checkLobby)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (!checkLobby || LobbyLogic.IsLobbyFormPure())
            {
                if (masterRoleInfo.IsGuidedStateSet(0x16) && !masterRoleInfo.IsGuidedStateSet(0x17))
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenFirstPayPanel);
                }
                else if (masterRoleInfo.IsGuidedStateSet(0x18) && !masterRoleInfo.IsGuidedStateSet(0x19))
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenRenewalPanel);
                }
            }
        }

        private uint GetDianQuanGiftCnt(int giftId)
        {
            uint num = 0;
            giftId--;
            if ((giftId < 0) || (giftId >= this.dianQuanBuyInfoList.Count))
            {
                return num;
            }
            ResCouponsBuyInfo info = this.dianQuanBuyInfoList[giftId];
            if (info == null)
            {
                return 0;
            }
            if ((info.bFirstGift > 0) && !this.IsDianQuanHaveFirstPay(info.dwID))
            {
                return info.dwBuyCount;
            }
            return info.dwExtraGiftCnt;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenFirstPayPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenFirstPayPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenRenewalPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenRenewalPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GetFirstPayReward, new CUIEventManager.OnUIEventHandler(this.OnGetFirstPayReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GetRenewalReward, new CUIEventManager.OnUIEventHandler(this.OnGetRenewalReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_FirstPayDianQuan, new CUIEventManager.OnUIEventHandler(this.OnFirstPayDianQuan));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_RenewalDianQuan, new CUIEventManager.OnUIEventHandler(this.OnRenewalDianQuan));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_ClickDianQuanGift, new CUIEventManager.OnUIEventHandler(this.OnClickDianQuanGift));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_ClickGetNewHeroPanel, new CUIEventManager.OnUIEventHandler(this.OnClickGetNewHeroPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_PlayHeroVideo, new CUIEventManager.OnUIEventHandler(this.OnPlayHeroVideo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_TehuiShop, new CUIEventManager.OnUIEventHandler(this.OnDisplayChaoZhiGift));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GotoTehuiShop, new CUIEventManager.OnUIEventHandler(this.OnGotoChaoZhiGift));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_ClosePayDianQuanForm, new CUIEventManager.OnUIEventHandler(this.OnPayDianQuanFormClose));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Pay_Success, new System.Action(this.OnPaySuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler<int>(EventID.ApolloHelper_Pay_Risk_Hit, new Action<int>(this.OnPayRiskHit));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Pay_Failed, new System.Action(this.OnPayFailed));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Need_Login, new System.Action(this.OnNeedLogin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanelinLobby, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanelInLobby));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, new Action<CMallFactoryShopController.ShopProduct>(this.NeedUpdateChaoZhieGift));
        }

        private bool IsDianQuanHaveFirstPay(uint id)
        {
            enNewbieAchieve achieve = enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_SOLDIER;
            id--;
            achieve = (enNewbieAchieve) (20 + id);
            return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsNewbieAchieveSet((int) achieve);
        }

        private void NeedUpdateChaoZhieGift(CMallFactoryShopController.ShopProduct product)
        {
            int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xac).dwConfValue;
            int num2 = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xad).dwConfValue;
            CMallFactoryShopController.ShopProduct product2 = Singleton<CMallFactoryShopController>.GetInstance().GetProduct((uint) dwConfValue);
            CMallFactoryShopController.ShopProduct product3 = Singleton<CMallFactoryShopController>.GetInstance().GetProduct((uint) num2);
            if (((product2 == null) || (product2.IsOnSale != 1)) && ((product3 == null) || (product3.IsOnSale != 1)))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
                if ((form != null) && (form.transform.Find("DiamondPayBtn") != null))
                {
                    GameObject gameObject = form.transform.Find("DiamondPayBtn").gameObject;
                    if ((gameObject != null) && masterRoleInfo.IsClientBitsSet(0))
                    {
                        gameObject.CustomSetActive(false);
                        masterRoleInfo.SetClientBits(0, false, true);
                    }
                }
            }
        }

        private void OnBuyDianQuanPanelClose(CUIEvent uiEvent)
        {
            Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
            if (!CSysDynamicBlock.bLobbyEntryBlocked)
            {
                this.AutoOpenRewardPanel(false);
            }
        }

        private void OnClickDianQuanGift(CUIEvent uiEvent)
        {
            this.m_dianQuanGiftId = uiEvent.m_eventParams.dianQuanBuyPar.giftId;
            this.m_dianQuanCnt = uiEvent.m_eventParams.dianQuanBuyPar.dianQuanCnt;
            uint dianQuanGiftCnt = this.GetDianQuanGiftCnt(this.m_dianQuanGiftId);
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_id = this.m_dianQuanCnt.ToString();
            DateTime now = DateTime.Now;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                now = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            }
            Singleton<BeaconHelper>.GetInstance().ReportOpenBuyDianEvent(now);
            DebugHelper.Assert(masterRoleInfo != null, "pay master role = null");
            if (masterRoleInfo != null)
            {
                if ((masterRoleInfo.DianQuan + dianQuanGiftCnt) > 0x7fffffffL)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("充值超过钻石上限", false, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                    Singleton<ApolloHelper>.GetInstance().Pay(this.m_dianQuanCnt.ToString(), string.Empty);
                }
            }
        }

        private void OnClickGetNewHeroPanel(CUIEvent uiEvent)
        {
            if ((this.rewardItems != null) && (this.rewardItems.Count > 0))
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(this.rewardItems), null, false, enUIEventID.None, false, false, "Form_Award");
            }
        }

        private void OnDisplayChaoZhiGift(CUIEvent uiEvent)
        {
            string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/DisplayTeHuiShop.prefab");
            Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true).gameObject.transform.FindChild("Panel/Title/Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("GotoTehuiShopTitle");
        }

        private void OnFirstPayDianQuan(CUIEvent uiEvent)
        {
            this.OpenBuyDianQuanPanel();
        }

        private void OnGetFirstPayReward(CUIEvent uiEvent)
        {
            SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_FIRST);
        }

        private void OnGetRenewalReward(CUIEvent uiEvent)
        {
            SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_RENEW);
        }

        private void OnGotoChaoZhiGift(CUIEvent uiEvent)
        {
            string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/DisplayTeHuiShop.prefab");
            Singleton<CUIManager>.GetInstance().CloseForm(formPath);
            CUICommonSystem.JumpForm(RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DISCOUNT);
        }

        private void OnNeedLogin()
        {
            Singleton<CUIManager>.GetInstance().OpenTips("Common_Need_Login", true, 1.5f, null, new object[0]);
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(LobbyMsgHandler.SendMidasToken));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(LobbyMsgHandler.SendMidasToken));
            Singleton<CTimerManager>.GetInstance().AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.SwitchToLogin));
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
        }

        private void OnOpenBuyDianQuanPanel(CUIEvent uiEvent)
        {
            this.OpenBuyDianQuanPanel();
        }

        private void OnOpenBuyDianQuanPanelInLobby(CUIEvent uiEvent)
        {
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "formal";
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
            this.OnOpenBuyDianQuanPanel(uiEvent);
        }

        private void OnOpenFirstPayPanel(CUIEvent uiEvent)
        {
            Singleton<CChatController>.GetInstance().ShowPanel(true, false);
            if (Singleton<CUIManager>.GetInstance().OpenForm(s_firstPayFormPath, false, true) != null)
            {
                Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "first";
                Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
                this.RefreshFirstPayPanel();
            }
        }

        private void OnOpenRenewalPanel(CUIEvent uiEvent)
        {
            Singleton<CChatController>.GetInstance().ShowPanel(true, false);
            if (Singleton<CUIManager>.GetInstance().OpenForm(s_renewalDianQuanFormPath, false, true) != null)
            {
                this.RefreshRenewalPanel();
            }
        }

        public void OnPandroiaPaySuccss()
        {
            if (this.m_pandoraDianQuanReqSeq == -1)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    this.m_lastDianQuan = masterRoleInfo.DianQuan;
                }
                this.m_pandoraDianQuanReqSeq = Singleton<CTimerManager>.GetInstance().AddTimer(0x1388, 3, new CTimer.OnTimeUpHandler(this.ReqAcntDianQuanHanlder));
            }
        }

        private void OnPayDianQuanFormClose(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_buyDianQuanFormPath);
            if (form != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(form);
            }
        }

        private void OnPayFailed()
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CUIManager>.GetInstance().OpenTips("Err_Common_Pay_Failed", true, 1.5f, null, new object[0]);
        }

        private void OnPayRiskHit(int errCode)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText(string.Format("Err_Common_Pay_Risk_Hit_{0}", errCode)), false);
        }

        private void OnPaySuccess()
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            CS_COUPONS_PAYTYPE payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_NULL;
            if (!masterRoleInfo.IsGuidedStateSet(0x16))
            {
                payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_FIRST;
            }
            else if (!masterRoleInfo.IsGuidedStateSet(0x18))
            {
                payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_RENEW;
            }
            SendReqQueryAcntDianQuan(payType, true);
            this.SetDianQuanFirstPay(this.m_dianQuanGiftId);
            this.RefreshBuyDianQuanPanel();
            this.RefreshFirstPayPanel();
            this.RefreshRenewalPanel();
        }

        private void OnPlayHeroVideo(CUIEvent uiEvent)
        {
            Handheld.PlayFullScreenMovie("Video/HeroVideo.mp4", Color.black, 2, 1);
        }

        [MessageHandler(0x489)]
        public static void OnReceiveDianQuanReward(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_COUPONS_REWARDINFO stCouponsRewardRsp = msg.stPkgData.stCouponsRewardRsp;
            bool flag = true;
            CPaySystem instance = Singleton<CPaySystem>.GetInstance();
            instance.rewardItems.Clear();
            int num = Mathf.Min(stCouponsRewardRsp.stRewardInfo.bNum, stCouponsRewardRsp.stRewardInfo.astRewardDetail.Length);
            for (int i = 0; i < num; i++)
            {
                if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 5)
                {
                    CUICommonSystem.ShowNewHeroOrSkin(stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stRewardInfo.stHero.dwHeroID, 0, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
                    flag = false;
                    break;
                }
                if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 10)
                {
                    uint num3;
                    uint num4;
                    CSkinInfo.ResolveHeroSkin(stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stRewardInfo.stSkin.dwSkinID, out num3, out num4);
                    CUICommonSystem.ShowNewHeroOrSkin(num3, num4, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                    flag = false;
                    break;
                }
                if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 1)
                {
                    if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bFromType == 1)
                    {
                        uint dwHeroID = stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stFromInfo.stHeroInfo.dwHeroID;
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(dwHeroID);
                        if (dataByKey != null)
                        {
                            ResHeroShop shop = null;
                            GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                            CUICommonSystem.ShowNewHeroOrSkin(dwHeroID, 0, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (shop == null) ? 1 : shop.dwChgItemCnt, 0);
                        }
                    }
                    else if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bFromType == 2)
                    {
                        uint dwSkinID = stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stFromInfo.stSkinInfo.dwSkinID;
                        uint heroId = 0;
                        uint skinId = 0;
                        CSkinInfo.ResolveHeroSkin(dwSkinID, out heroId, out skinId);
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                        if (heroSkin != null)
                        {
                            ResHeroSkinShop shop2 = null;
                            GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out shop2);
                            CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (shop2 == null) ? 1 : shop2.dwChgItemCnt, 0);
                        }
                    }
                }
            }
            instance.rewardItems = CUseableManager.GetUseableListFromReward(stCouponsRewardRsp.stRewardInfo);
            if (flag)
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(instance.rewardItems), null, false, enUIEventID.None, false, false, "Form_Award");
            }
        }

        [MessageHandler(0x4a7)]
        public static void OnReceiveNewbitSyn(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_NTF_NEWIEBITSYN stNewieBitSyn = msg.stPkgData.stNewieBitSyn;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SetGuidedStateSet((int) stNewieBitSyn.dwBitType, true);
            if (((stNewieBitSyn.dwBitType == 0x16) || (stNewieBitSyn.dwBitType == 0x17)) || ((stNewieBitSyn.dwBitType == 0x18) || (stNewieBitSyn.dwBitType == 0x19)))
            {
                CLobbySystem.RefreshDianQuanPayButton(true);
                Singleton<CPaySystem>.GetInstance().AutoOpenRewardPanel(true);
            }
        }

        private void OnRenewalDianQuan(CUIEvent uiEvent)
        {
            this.OpenBuyDianQuanPanel();
        }

        public void OpenBuyDianQuanPanel()
        {
            if (ApolloConfig.payEnabled)
            {
                Singleton<CChatController>.GetInstance().ShowPanel(true, false);
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_buyDianQuanFormPath, false, true);
                if (script != null)
                {
                    GameObject obj2 = script.m_formWidgets[0];
                    if (obj2 != null)
                    {
                        if ((GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xec).dwConfValue == 1) || MonoSingleton<TdirMgr>.instance.ShowBuyTongCaiBtn())
                        {
                            obj2.CustomSetActive(true);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                    Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
                    this.RefreshBuyDianQuanPanel();
                    MonoSingleton<NobeSys>.GetInstance().ShowNobeTipsInDiamond();
                }
            }
        }

        public void RefreshBuyDianQuanPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_buyDianQuanFormPath);
            if (form != null)
            {
                DatabinTable<ResCouponsBuyInfo, uint> androidDianQuanBuyInfo = null;
                androidDianQuanBuyInfo = GameDataMgr.androidDianQuanBuyInfo;
                int index = 0;
                this.dianQuanBuyInfoList.Clear();
                androidDianQuanBuyInfo.Accept(x => this.dianQuanBuyInfoList.Add(x));
                this.SortDianQuanInfoList();
                CUIListScript component = form.transform.Find("pnlBg/pnlBody/List").GetComponent<CUIListScript>();
                component.SetElementAmount(this.dianQuanBuyInfoList.Count);
                for (index = 0; index < this.dianQuanBuyInfoList.Count; index++)
                {
                    ResCouponsBuyInfo info = this.dianQuanBuyInfoList[index];
                    CUIListElementScript elemenet = component.GetElemenet(index);
                    Image image = elemenet.transform.Find("imgIcon").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Purchase_Dir, StringHelper.UTF8BytesToString(ref info.szImgPath));
                    image.SetSprite(prefabPath, form, true, false, false);
                    elemenet.transform.Find("diamondCntText").GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_DianQuan_Cnt"), info.dwBuyCount);
                    GameObject gameObject = elemenet.transform.Find("buyPanel/buyBtn").gameObject;
                    gameObject.transform.Find("Text").GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_DianQuan_Price"), info.dwBuyPrice);
                    GameObject obj3 = elemenet.transform.Find("additionPanel").gameObject;
                    obj3.CustomSetActive(false);
                    GameObject obj4 = elemenet.transform.Find("pnlRecommend").gameObject;
                    if ((info.bFirstGift > 0) && !this.IsDianQuanHaveFirstPay(info.dwID))
                    {
                        obj4.CustomSetActive(true);
                        obj4.transform.Find("txtDiscount").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Pay_First_Pay_Double");
                    }
                    else
                    {
                        if (info.dwExtraGiftCnt > 0)
                        {
                            obj3.CustomSetActive(true);
                            obj3.transform.Find("Text").GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_Gift_Diamond_Cnt"), info.dwExtraGiftCnt);
                        }
                        obj4.CustomSetActive(false);
                    }
                    CUIEventScript script4 = gameObject.GetComponent<CUIEventScript>();
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.dianQuanBuyPar.giftId = (int) info.dwID;
                    eventParams.dianQuanBuyPar.dianQuanCnt = (int) info.dwBuyCount;
                    script4.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_ClickDianQuanGift, eventParams);
                }
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    Transform transform = form.transform.FindChild("Button_OpenNobe");
                    Transform transform2 = form.transform.FindChild("Button_HelpMe");
                    Transform transform3 = form.transform.FindChild("Button_TongCai");
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                    if (transform2 != null)
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                    if (transform3 != null)
                    {
                        transform3.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        private void RefreshFirstPayPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_firstPayFormPath);
            if (form != null)
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 60).dwConfValue;
                ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(dwConfValue);
                Transform transform = form.transform.Find("Panel_FirstPay/bodyPanel");
                CUIListScript component = transform.Find("rewardList").GetComponent<CUIListScript>();
                this.RefreshRewardList(form, component, ref dataByKey.astRewardDetail, 0);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                bool flag = masterRoleInfo.IsGuidedStateSet(0x16);
                bool flag2 = masterRoleInfo.IsGuidedStateSet(0x17);
                GameObject gameObject = transform.Find("payButton").gameObject;
                GameObject btn = transform.Find("getButton").gameObject;
                CUICommonSystem.SetButtonName(gameObject, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Top_Up"));
                gameObject.CustomSetActive(!flag);
                CUICommonSystem.SetButtonName(btn, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Get_Reward"));
                btn.CustomSetActive(flag && !flag2);
            }
        }

        private void RefreshRenewalPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_renewalDianQuanFormPath);
            if (form != null)
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x3d).dwConfValue;
                ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(dwConfValue);
                Transform transform = form.transform.Find("Panel_Renewal/bodyPanel");
                transform.Find("rewardDescText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref dataByKey.szRewardDesc);
                GameObject gameObject = transform.Find("itemCellFirst").gameObject;
                CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) dataByKey.astRewardDetail[0].bItemType, (int) dataByKey.astRewardDetail[0].dwLowCnt, dataByKey.astRewardDetail[0].dwItemID);
                CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false);
                CUIListScript component = transform.Find("List").GetComponent<CUIListScript>();
                this.RefreshRewardList(form, component, ref dataByKey.astRewardDetail, 1);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                bool flag = masterRoleInfo.IsGuidedStateSet(0x18);
                bool flag2 = masterRoleInfo.IsGuidedStateSet(0x19);
                GameObject btn = transform.Find("payButton").gameObject;
                GameObject obj4 = transform.Find("getButton").gameObject;
                CUICommonSystem.SetButtonName(btn, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Top_Up"));
                btn.CustomSetActive(!flag);
                CUICommonSystem.SetButtonName(obj4, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Get_Reward"));
                obj4.CustomSetActive(flag && !flag2);
            }
        }

        private void RefreshRewardList(CUIFormScript form, CUIListScript listScript, ref ResDT_RandomRewardInfo[] rewardInfoArr, int index = 0)
        {
            if ((form != null) && (listScript != null))
            {
                int amount = 0;
                for (int i = index; i < rewardInfoArr.Length; i++)
                {
                    if (rewardInfoArr[i].bItemType == 0)
                    {
                        break;
                    }
                    amount++;
                }
                listScript.SetElementAmount(amount);
                for (int j = 0; j < amount; j++)
                {
                    CUIListElementScript elemenet = listScript.GetElemenet(j);
                    CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) rewardInfoArr[j + index].bItemType, (int) rewardInfoArr[j + index].dwLowCnt, rewardInfoArr[j + index].dwItemID);
                    GameObject gameObject = elemenet.transform.Find("itemCell").gameObject;
                    CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false);
                }
            }
        }

        private void ReqAcntDianQuanHanlder(int seq)
        {
            CS_COUPONS_PAYTYPE payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_QUERY;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                SendReqQueryAcntDianQuan(payType, false);
            }
            else
            {
                ulong dianQuan = masterRoleInfo.DianQuan;
                if (this.m_lastDianQuan != dianQuan)
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref seq);
                    this.m_pandoraDianQuanReqSeq = -1;
                    this.m_lastDianQuan = dianQuan;
                }
                else
                {
                    SendReqQueryAcntDianQuan(payType, false);
                }
            }
        }

        public static void SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE payType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x488);
            msg.stPkgData.stCouponsRewardReq.bPayType = (byte) payType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqQueryAcntDianQuan(CS_COUPONS_PAYTYPE payType, bool isAlert = true)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x47e);
            msg.stPkgData.stAcntCouponsReq.bPayType = (byte) payType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, isAlert);
        }

        private void SetDianQuanFirstPay(int id)
        {
            enNewbieAchieve achieve = enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_SOLDIER;
            id--;
            achieve = (enNewbieAchieve) (20 + id);
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SetNewbieAchieve((int) achieve, true, true);
        }

        private void SortDianQuanInfoList()
        {
            int count = this.dianQuanBuyInfoList.Count;
            ResCouponsBuyInfo info = null;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < (count - 1); i++)
            {
                for (int j = 0; j < ((count - 1) - i); j++)
                {
                    flag = false;
                    flag2 = (this.dianQuanBuyInfoList[j].bFirstGift > 0) && !this.IsDianQuanHaveFirstPay(this.dianQuanBuyInfoList[j].dwID);
                    flag3 = (this.dianQuanBuyInfoList[j + 1].bFirstGift > 0) && !this.IsDianQuanHaveFirstPay(this.dianQuanBuyInfoList[j + 1].dwID);
                    if (flag2 == flag3)
                    {
                        if (this.dianQuanBuyInfoList[j].dwBuyCount > this.dianQuanBuyInfoList[j + 1].dwBuyCount)
                        {
                            flag = true;
                        }
                    }
                    else if (!flag2 && flag3)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        info = this.dianQuanBuyInfoList[j];
                        this.dianQuanBuyInfoList[j] = this.dianQuanBuyInfoList[j + 1];
                        this.dianQuanBuyInfoList[j + 1] = info;
                    }
                }
            }
        }

        private void SwitchToLogin(int seq)
        {
            Singleton<ApolloHelper>.GetInstance().Login(Singleton<ApolloHelper>.GetInstance().CurPlatform, 0L, null);
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenFirstPayPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenFirstPayPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenRenewalPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenRenewalPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GetFirstPayReward, new CUIEventManager.OnUIEventHandler(this.OnGetFirstPayReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GetRenewalReward, new CUIEventManager.OnUIEventHandler(this.OnGetRenewalReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_FirstPayDianQuan, new CUIEventManager.OnUIEventHandler(this.OnFirstPayDianQuan));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_RenewalDianQuan, new CUIEventManager.OnUIEventHandler(this.OnRenewalDianQuan));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_ClickDianQuanGift, new CUIEventManager.OnUIEventHandler(this.OnClickDianQuanGift));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_ClickGetNewHeroPanel, new CUIEventManager.OnUIEventHandler(this.OnClickGetNewHeroPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_PlayHeroVideo, new CUIEventManager.OnUIEventHandler(this.OnPlayHeroVideo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_TehuiShop, new CUIEventManager.OnUIEventHandler(this.OnDisplayChaoZhiGift));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GotoTehuiShop, new CUIEventManager.OnUIEventHandler(this.OnGotoChaoZhiGift));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_ClosePayDianQuanForm, new CUIEventManager.OnUIEventHandler(this.OnPayDianQuanFormClose));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Pay_Success, new System.Action(this.OnPaySuccess));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<int>(EventID.ApolloHelper_Pay_Risk_Hit, new Action<int>(this.OnPayRiskHit));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Pay_Failed, new System.Action(this.OnPayFailed));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Need_Login, new System.Action(this.OnNeedLogin));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanelinLobby, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanelInLobby));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, new Action<CMallFactoryShopController.ShopProduct>(this.NeedUpdateChaoZhieGift));
        }
    }
}

