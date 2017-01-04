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
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CMallRecommendController : Singleton<CMallRecommendController>
    {
        [CompilerGenerated]
        private static Comparison<ResSaleRecommend> <>f__am$cache14;
        private ListTab m_CurListTab;
        public ResRandDrawInfo[] m_CurrentLottryCtrl;
        private ListView<ResRewardPoolInfo>[] m_CurrentPoolList;
        private ListView<ResHeroCfgInfo> m_CurrentRecommendHeros;
        private ListView<ResSaleRecommend> m_CurrentRecommendProductList;
        private Dictionary<long, uint> m_CurrentRecommendProductMap;
        private ListView<ResHeroSkin> m_CurrentRecommendSkins;
        private Tab m_CurTab;
        private ResRandDrawInfo[] m_DefaultLotteryCtrl;
        private ResHeroCfgInfo m_FirstHero;
        private ResHeroSkin m_FirstSkin;
        private int m_HasHeroAndSkin;
        private string m_heroModelPath;
        private int[] m_MostRecentCtrlRefreshTime;
        private int m_MostRecentRecommendRefreshTime;
        private int[] m_RefreshLotteryCtrlTimerSeq;
        private int m_RefreshRecommendProductTimerSeq;
        public static string[] s_exchangePurposeNameKeys = new string[] { "ExchangePurpose_Exchange" };
        public static string[] s_exchangeTypeNameKeys = new string[] { "ExchangeType_NotSupport", "ExchangeType_Hero_Exchange_Coupons", "ExchangeType_Skin_Exchange_Coupons" };
        public static string sMallHeroExchangeListPath = "UGUI/Form/System/Mall/Form_Recommend_Exchange_List.prefab";

        public void Clear()
        {
            for (byte i = 0; i < 3; i = (byte) (i + 1))
            {
                if (this.m_CurrentPoolList[i] != null)
                {
                    this.m_CurrentPoolList[i].Clear();
                }
            }
            this.m_CurrentRecommendProductList.Clear();
            this.m_CurrentRecommendProductMap.Clear();
            this.m_CurrentRecommendHeros.Clear();
            this.m_CurrentRecommendSkins.Clear();
            this.m_HasHeroAndSkin = 0;
            this.m_FirstHero = null;
            this.m_FirstSkin = null;
        }

        public void Draw(CUIFormScript form)
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Exchange_More, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeMore));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendListTabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListHeroEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Skin_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListSkinEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Mall_Recommend_Recommend_Data_Refresh, new System.Action(this.RefreshRecommendView));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BAG_ITEMS_UPDATE, new System.Action(this.OnItemAdd));
            this.InitElements();
            if (!this.RefreshPoolData(0))
            {
                Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            }
            if (!this.RefreshRecommendData())
            {
                Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
            }
        }

        public static uint GetCurrencyValueFromRoleInfo(CRoleInfo roleInfo, enExchangeType exchangeType)
        {
            enExchangeType type = exchangeType;
            if (type != enExchangeType.HeroExchangeCoupons)
            {
                if (type != enExchangeType.SkinExchangeCoupons)
                {
                    return 0;
                }
            }
            else
            {
                ResGlobalInfo info = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x86);
                DebugHelper.Assert(info != null, "global cfg databin err: hero exchange id doesnt exist");
                if (info == null)
                {
                    return 0;
                }
                uint baseID = info.dwConfValue;
                return (uint) roleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, baseID);
            }
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x87);
            DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange id doesnt exist");
            if (dataByKey == null)
            {
                return 0;
            }
            uint dwConfValue = dataByKey.dwConfValue;
            return (uint) roleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, dwConfValue);
        }

        public string GetExchangeTypeIconPath(enExchangeType type)
        {
            ResGlobalInfo dataByKey = null;
            uint baseID = 0;
            switch (type)
            {
                case enExchangeType.HeroExchangeCoupons:
                {
                    dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x86);
                    DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange id doesnt exist");
                    if (dataByKey == null)
                    {
                        return null;
                    }
                    baseID = dataByKey.dwConfValue;
                    CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, baseID, 0);
                    if (useable != null)
                    {
                        return useable.GetIconPath();
                    }
                    break;
                }
                case enExchangeType.SkinExchangeCoupons:
                {
                    dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x87);
                    DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange id doesnt exist");
                    if (dataByKey == null)
                    {
                        return null;
                    }
                    baseID = dataByKey.dwConfValue;
                    CUseable useable2 = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, baseID, 0);
                    if (useable2 != null)
                    {
                        return useable2.GetIconPath();
                    }
                    break;
                }
            }
            return null;
        }

        private void HeroSelect_Skill_Down(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject p = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/PanelSkillInfo");
                if (p != null)
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(p, "skillNameText");
                    Text text2 = Utility.GetComponetInChild<Text>(p, "SkillDescribeText");
                    Text text3 = Utility.GetComponetInChild<Text>(p, "SkillCDText");
                    Text text4 = Utility.GetComponetInChild<Text>(p, "SkillCDText/SkillEnergyCostText");
                    componetInChild.text = uiEvent.m_eventParams.skillTipParam.skillName;
                    text2.text = uiEvent.m_eventParams.skillTipParam.strTipText;
                    text3.text = uiEvent.m_eventParams.skillTipParam.skillCoolDown;
                    text4.text = uiEvent.m_eventParams.skillTipParam.skillEnergyCost;
                    uint[] skillEffect = uiEvent.m_eventParams.skillTipParam.skillEffect;
                    if (skillEffect != null)
                    {
                        for (int i = 1; i <= 2; i++)
                        {
                            GameObject obj3 = Utility.FindChild(p, string.Format("skillNameText/EffectNode{0}", i));
                            if (obj3 != null)
                            {
                                if ((i <= skillEffect.Length) && (skillEffect[i - 1] != 0))
                                {
                                    obj3.CustomSetActive(true);
                                    obj3.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffect[i - 1]), uiEvent.m_srcFormScript, true, false, false);
                                    obj3.transform.Find("Text").GetComponent<Text>().text = CSkillData.GetEffectDesc((SkillEffectType) skillEffect[i - 1]);
                                }
                                else
                                {
                                    obj3.CustomSetActive(false);
                                }
                            }
                        }
                        p.CustomSetActive(true);
                    }
                }
            }
        }

        private void HeroSelect_Skill_Up(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/PanelSkillInfo").CustomSetActive(false);
            }
        }

        public override void Init()
        {
            base.Init();
            this.m_DefaultLotteryCtrl = new ResRandDrawInfo[3];
            this.m_CurrentLottryCtrl = new ResRandDrawInfo[3];
            this.m_CurrentPoolList = new ListView<ResRewardPoolInfo>[3];
            this.m_CurrentRecommendProductList = new ListView<ResSaleRecommend>();
            this.m_CurrentRecommendHeros = new ListView<ResHeroCfgInfo>();
            this.m_CurrentRecommendSkins = new ListView<ResHeroSkin>();
            this.m_CurrentRecommendProductMap = new Dictionary<long, uint>();
            this.m_MostRecentCtrlRefreshTime = new int[3];
            this.m_MostRecentRecommendRefreshTime = 0;
            this.m_RefreshLotteryCtrlTimerSeq = new int[3];
            this.m_RefreshRecommendProductTimerSeq = -1;
            this.m_FirstHero = null;
            this.m_FirstSkin = null;
            this.CurTab = Tab.None;
        }

        public void InitElements()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend").CustomSetActive(true);
                Utility.FindChild(mallForm.gameObject, "UIScene_Recommend_HeroInfo").CustomSetActive(true);
            }
        }

        private void InitListTab()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallHeroExchangeListPath);
            if (form != null)
            {
                ListTab[] values = (ListTab[]) Enum.GetValues(typeof(ListTab));
                List<ListTab> list = new List<ListTab>();
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    ListTab tab = values[i];
                    switch ((tab + 1))
                    {
                        case ListTab.Skin:
                            if ((this.m_HasHeroAndSkin & 1) > 0)
                            {
                                list.Add(values[i]);
                            }
                            break;

                        case ((ListTab) 2):
                            if ((this.m_HasHeroAndSkin & 2) > 0)
                            {
                                list.Add(values[i]);
                            }
                            break;
                    }
                }
                string[] strArray = new string[list.Count];
                for (byte j = 0; j < strArray.Length; j = (byte) (j + 1))
                {
                    switch (list[j])
                    {
                        case ListTab.Hero:
                            strArray[j] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewHero");
                            break;

                        case ListTab.Skin:
                            strArray[j] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewSkin");
                            break;
                    }
                }
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "Panel/Tab");
                if (componetInChild == null)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("list err", false, 1.5f, null, new object[0]);
                    Singleton<CUIManager>.GetInstance().CloseForm(sMallHeroExchangeListPath);
                }
                else
                {
                    componetInChild.SetElementAmount(strArray.Length);
                    CUIListElementScript elemenet = null;
                    for (int k = 0; k < strArray.Length; k++)
                    {
                        elemenet = componetInChild.GetElemenet(k);
                        if (elemenet != null)
                        {
                            elemenet.gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[k];
                        }
                    }
                    Tab curTab = this.CurTab;
                    if (curTab == Tab.Hero)
                    {
                        this.CurListTab = ListTab.Hero;
                    }
                    else if (curTab == Tab.Skin)
                    {
                        this.CurListTab = ListTab.Skin;
                    }
                    else
                    {
                        this.CurListTab = ListTab.Hero;
                    }
                    componetInChild.SelectElement((int) this.CurListTab, true);
                }
            }
        }

        private void InitRecommendTab()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                List<Tab> list = new List<Tab>();
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    Tab tab = values[i];
                    switch ((tab + 1))
                    {
                        case Tab.Skin:
                            if ((this.m_HasHeroAndSkin & 1) > 0)
                            {
                                list.Add(values[i]);
                            }
                            break;

                        case ((Tab) 2):
                            if ((this.m_HasHeroAndSkin & 2) > 0)
                            {
                                list.Add(values[i]);
                            }
                            break;
                    }
                }
                string[] strArray = new string[list.Count];
                for (byte j = 0; j < strArray.Length; j = (byte) (j + 1))
                {
                    switch (list[j])
                    {
                        case Tab.Hero:
                            strArray[j] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewHero");
                            break;

                        case Tab.Skin:
                            strArray[j] = Singleton<CTextManager>.GetInstance().GetText("Mall_NewSkin");
                            break;
                    }
                }
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlRecommend/Tab");
                if (componetInChild == null)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("list err", false, 1.5f, null, new object[0]);
                    Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
                }
                else
                {
                    componetInChild.SetElementAmount(strArray.Length);
                    CUIListElementScript elemenet = null;
                    for (int k = 0; k < strArray.Length; k++)
                    {
                        elemenet = componetInChild.GetElemenet(k);
                        if (elemenet != null)
                        {
                            elemenet.gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[k];
                        }
                    }
                    componetInChild.m_alwaysDispatchSelectedChangeEvent = true;
                    if (((this.CurTab == Tab.None) || (this.CurTab < Tab.Hero)) || (this.CurTab >= componetInChild.GetElementAmount()))
                    {
                        componetInChild.SelectElement(0, true);
                    }
                    else
                    {
                        componetInChild.SelectElement((int) this.CurTab, true);
                    }
                }
            }
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Recommend", "pnlRecommend", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlRecommend") == null)
            {
                return false;
            }
            return true;
        }

        private void OnItemAdd()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                switch (this.CurTab)
                {
                    case Tab.Hero:
                        this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.HeroExchangeCoupons);
                        break;

                    case Tab.Skin:
                        this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.SkinExchangeCoupons);
                        break;
                }
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallHeroExchangeListPath);
            if (form != null)
            {
                switch (this.CurListTab)
                {
                    case ListTab.Hero:
                        this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.HeroExchangeCoupons);
                        break;

                    case ListTab.Skin:
                        this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.SkinExchangeCoupons);
                        break;
                }
            }
        }

        private void OnLotteryBuy(CUIEvent uiEvent)
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                switch (this.CurTab)
                {
                    case Tab.Hero:
                        CMallSystem.TryToPay(enPayPurpose.RecommendLottery, string.Empty, CMallSystem.ResBuyTypeToPayType(this.m_CurrentLottryCtrl[1].bCostType), this.m_CurrentLottryCtrl[1].dwCostPrice, enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, false, true, false);
                        break;

                    case Tab.Skin:
                        CMallSystem.TryToPay(enPayPurpose.RecommendLottery, string.Empty, CMallSystem.ResBuyTypeToPayType(this.m_CurrentLottryCtrl[2].bCostType), this.m_CurrentLottryCtrl[2].dwCostPrice, enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, false, true, false);
                        break;
                }
            }
        }

        private void OnLotteryBuyConfirm(CUIEvent uiEvent)
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CMallSystem>.GetInstance().ToggleActionMask(true, 10f);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x499);
                CSPKG_CMD_RANDDRAW_REQ cspkg_cmd_randdraw_req = new CSPKG_CMD_RANDDRAW_REQ();
                switch (this.CurTab)
                {
                    case Tab.Hero:
                        cspkg_cmd_randdraw_req.dwDrawID = this.m_CurrentLottryCtrl[1].dwDrawID;
                        break;

                    case Tab.Skin:
                        cspkg_cmd_randdraw_req.dwDrawID = this.m_CurrentLottryCtrl[2].dwDrawID;
                        break;
                }
                msg.stPkgData.stRandDrawReq = cspkg_cmd_randdraw_req;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnMallAppear(CUIEvent uiEvent)
        {
            if (((uiEvent.m_srcFormScript != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Recommend))
            {
                switch (this.CurTab)
                {
                    case Tab.Hero:
                        this.UpdateHeroView();
                        break;

                    case Tab.Skin:
                        this.UpdateSkinView();
                        break;
                }
            }
        }

        private void OnMallTabChange()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Hero_Skill_Up);
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchange));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeConfirm));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Exchange_More, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeMore));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendListTabChange));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListHeroEnable));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Skin_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListSkinEnable));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
                Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
                Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Recommend_Recommend_Data_Refresh, new System.Action(this.RefreshRecommendView));
                Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
                Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
                Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new System.Action(this.OnItemAdd));
                this.Clear();
                Utility.FindChild(mallForm.gameObject, "UIScene_Recommend_HeroInfo").CustomSetActive(false);
            }
        }

        private void OnNtyAddHero(uint heroID)
        {
            this.RefreshRecommendListPnl();
        }

        private void OnNtyAddSkin(uint heroID, uint skinID, uint addReason)
        {
            if (addReason == 4)
            {
                CUICommonSystem.ShowNewHeroOrSkin(heroID, skinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
            }
            this.RefreshRecommendListPnl();
        }

        private void OnRecommendBuy(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            switch (this.CurTab)
            {
                case Tab.Hero:
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                    DebugHelper.Assert(dataByKey != null);
                    CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, payValue, enUIEventID.Mall_Recommend_On_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
                    break;
                }
                case Tab.Skin:
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    DebugHelper.Assert(heroSkin != null);
                    CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref heroSkin.szSkinName), tag, payValue, enUIEventID.Mall_Recommend_On_Buy_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
                    break;
                }
            }
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHero;
            uIEvent.m_eventParams.heroId = heroId;
        }

        private void OnRecommendBuyConfirm(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            CS_SALERECMD_BUYTYPE buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COUPONS;
            switch (tag)
            {
                case enPayType.GoldCoin:
                    buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COIN;
                    break;

                case enPayType.DianQuan:
                    buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_COUPONS;
                    break;

                case enPayType.Diamond:
                case enPayType.DiamondAndDianQuan:
                    buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_DIAMOND;
                    break;
            }
            uint num = 0;
            long key = 0L;
            Tab curTab = this.CurTab;
            if (curTab == Tab.Hero)
            {
                key = GameDataMgr.GetDoubleKey(4, uiEvent.m_eventParams.heroSkinParam.heroId);
                if (!this.m_CurrentRecommendProductMap.TryGetValue(key, out num))
                {
                    return;
                }
            }
            else if (curTab == Tab.Skin)
            {
                uint skinCfgId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
                key = GameDataMgr.GetDoubleKey(7, skinCfgId);
                if (!this.m_CurrentRecommendProductMap.TryGetValue(key, out num))
                {
                    return;
                }
            }
            this.ReqBuy(num, buyType);
        }

        private void OnRecommendExchange(CUIEvent uiEvent)
        {
            enExchangeType tag = (enExchangeType) uiEvent.m_eventParams.tag;
            uint exchangeValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            switch (tag)
            {
                case enExchangeType.HeroExchangeCoupons:
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                    DebugHelper.Assert(dataByKey != null);
                    TryToExchange(enExchangePurpose.Exchange, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, exchangeValue, enUIEventID.Mall_Recommend_On_Exchange_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, false);
                    break;
                }
                case enExchangeType.SkinExchangeCoupons:
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    DebugHelper.Assert(heroSkin != null);
                    TryToExchange(enExchangePurpose.Exchange, StringHelper.UTF8BytesToString(ref heroSkin.szSkinName), tag, exchangeValue, enUIEventID.Mall_Recommend_On_Exchange_Confirm, ref uiEvent.m_eventParams, enUIEventID.None, true, false);
                    break;
                }
            }
        }

        private void OnRecommendExchangeConfirm(CUIEvent uiEvent)
        {
            enExchangeType tag = (enExchangeType) uiEvent.m_eventParams.tag;
            uint num = 0;
            long key = 0L;
            CS_SALERECMD_BUYTYPE buyType = CS_SALERECMD_BUYTYPE.CS_SALERECMD_BUY_EXCHANGE;
            switch (tag)
            {
                case enExchangeType.HeroExchangeCoupons:
                    key = GameDataMgr.GetDoubleKey(4, uiEvent.m_eventParams.heroSkinParam.heroId);
                    if (!this.m_CurrentRecommendProductMap.TryGetValue(key, out num))
                    {
                        return;
                    }
                    break;

                case enExchangeType.SkinExchangeCoupons:
                {
                    uint skinCfgId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
                    key = GameDataMgr.GetDoubleKey(7, skinCfgId);
                    if (!this.m_CurrentRecommendProductMap.TryGetValue(key, out num))
                    {
                        return;
                    }
                    break;
                }
            }
            this.ReqBuy(num, buyType);
        }

        private void OnRecommendExchangeMore(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenForm(sMallHeroExchangeListPath, false, true);
            this.InitListTab();
        }

        private void OnRecommendListHeroEnable(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcFormScript != null) && (srcWidget != null)) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_CurrentRecommendHeros.Count)))
            {
                ResHeroCfgInfo info = this.m_CurrentRecommendHeros[srcWidgetIndexInBelongedList];
                ResHeroShop shop = null;
                GameDataMgr.heroShopInfoDict.TryGetValue(info.dwCfgID, out shop);
                if ((shop != null) && ((shop.bIsBuyItem == 0) || (shop.dwBuyItemCnt == 0)))
                {
                    Debug.LogError(string.Format("hero {0} exchange not supported", info.dwCfgID));
                }
                else
                {
                    uint num2 = 0;
                    long doubleKey = GameDataMgr.GetDoubleKey(4, info.dwCfgID);
                    if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out num2))
                    {
                        Debug.LogError("recommend product not found");
                    }
                    else
                    {
                        ResSaleRecommend recommend = new ResSaleRecommend();
                        if (!GameDataMgr.recommendProductDict.TryGetValue(num2, out recommend))
                        {
                            Debug.LogError("recommend product not found");
                        }
                        else
                        {
                            Transform transform = srcWidget.transform;
                            Transform transform2 = transform.Find("heroItem");
                            GameObject gameObject = transform2.Find("profession").gameObject;
                            CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.gameObject, StringHelper.UTF8BytesToString(ref this.m_CurrentRecommendHeros[srcWidgetIndexInBelongedList].szImagePath), enHeroHeadType.enBust, false);
                            CUICommonSystem.SetHeroJob(srcFormScript, gameObject, (enHeroJobType) info.bMainJob);
                            transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref info.szName);
                            CUIEventScript component = transform2.GetComponent<CUIEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.openHeroFormPar.heroId = info.dwCfgID;
                            eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                            component.m_closeFormWhenClicked = true;
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (masterRoleInfo == null)
                            {
                                DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
                            }
                            else
                            {
                                GameObject obj4 = transform.Find("imgExperienceMark").gameObject;
                                GameObject obj5 = Utility.FindChild(transform2.gameObject, "heroDataPanel/heroPricePanel");
                                if (obj5 == null)
                                {
                                    DebugHelper.Assert(obj5 != null, "price panel is null");
                                }
                                else
                                {
                                    obj5.CustomSetActive(false);
                                    Transform transform3 = transform.Find("ButtonGroup/BuyBtn");
                                    Button button = transform3.GetComponent<Button>();
                                    Text text2 = transform3.Find("Text").GetComponent<Text>();
                                    CUIEventScript script3 = transform3.GetComponent<CUIEventScript>();
                                    transform3.gameObject.CustomSetActive(false);
                                    script3.enabled = false;
                                    button.enabled = false;
                                    if (masterRoleInfo.IsHaveHero(info.dwCfgID, false))
                                    {
                                        transform3.gameObject.CustomSetActive(true);
                                        text2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
                                        obj4.CustomSetActive(false);
                                        CUICommonSystem.PlayAnimator(srcWidget, "OnlyName");
                                    }
                                    else
                                    {
                                        obj4.CustomSetActive(masterRoleInfo.IsValidExperienceHero(info.dwCfgID));
                                        obj5.CustomSetActive(true);
                                        Image componetInChild = Utility.GetComponetInChild<Image>(obj5, "pnlExchange/costImage");
                                        Text text3 = Utility.GetComponetInChild<Text>(obj5, "pnlExchange/costText");
                                        if ((componetInChild != null) && (text3 != null))
                                        {
                                            componetInChild.SetSprite(this.GetExchangeTypeIconPath(enExchangeType.HeroExchangeCoupons), srcFormScript, true, false, false);
                                            if (shop != null)
                                            {
                                                text3.text = shop.dwBuyItemCnt.ToString();
                                            }
                                            transform3.gameObject.CustomSetActive(true);
                                            text2.text = Singleton<CTextManager>.GetInstance().GetText("Exchange_Btn");
                                            script3.enabled = true;
                                            button.enabled = true;
                                            stUIEventParams params2 = new stUIEventParams {
                                                tag = 1
                                            };
                                            if (shop != null)
                                            {
                                                params2.commonUInt32Param1 = shop.dwBuyItemCnt;
                                            }
                                            params2.heroSkinParam.heroId = info.dwCfgID;
                                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_On_Exchange, params2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnRecommendListSkinEnable(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcFormScript != null) && (srcWidget != null)) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_CurrentRecommendSkins.Count)))
            {
                ResHeroSkin skin = this.m_CurrentRecommendSkins[srcWidgetIndexInBelongedList];
                ResHeroSkinShop shop = null;
                GameDataMgr.skinShopInfoDict.TryGetValue(skin.dwID, out shop);
                if ((shop != null) && ((shop.bIsBuyItem == 0) || (shop.dwBuyItemCnt == 0)))
                {
                    Debug.LogError(string.Format("skin {0} exchange not supported", skin.dwID));
                }
                else
                {
                    uint num2 = 0;
                    long doubleKey = GameDataMgr.GetDoubleKey(7, skin.dwID);
                    if (!this.m_CurrentRecommendProductMap.TryGetValue(doubleKey, out num2))
                    {
                        Debug.LogError("recommend product not found");
                    }
                    else
                    {
                        ResSaleRecommend recommend = new ResSaleRecommend();
                        if (!GameDataMgr.recommendProductDict.TryGetValue(num2, out recommend))
                        {
                            Debug.LogError("recommend product not found");
                        }
                        else
                        {
                            Transform transform = srcWidget.transform;
                            Transform transform2 = transform.Find("heroItem");
                            Text component = transform2.Find("heroDataPanel/heroNamePanel/heroNameText").GetComponent<Text>();
                            Text text2 = transform2.Find("heroDataPanel/heroNamePanel/heroSkinText").GetComponent<Text>();
                            CUICommonSystem.SetHeroItemImage(srcFormScript, transform2.gameObject, skin.szSkinPicID, enHeroHeadType.enBust, false);
                            Transform transform3 = transform2.Find("skinLabelImage");
                            CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, transform3.gameObject, skin.dwHeroID, skin.dwSkinID);
                            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(skin.dwHeroID);
                            if (dataByKey != null)
                            {
                                component.text = dataByKey.szName;
                            }
                            text2.text = skin.szSkinName;
                            CUIEventScript script2 = transform2.GetComponent<CUIEventScript>();
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.openHeroFormPar.heroId = skin.dwHeroID;
                            eventParams.openHeroFormPar.skinId = skin.dwSkinID;
                            eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                            script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                            script2.m_closeFormWhenClicked = true;
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (masterRoleInfo == null)
                            {
                                DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is Null");
                            }
                            else
                            {
                                Transform transform4 = transform.Find("ButtonGroup/BuyBtn");
                                Button button = transform4.GetComponent<Button>();
                                Text text3 = transform4.Find("Text").GetComponent<Text>();
                                CUIEventScript script3 = transform4.GetComponent<CUIEventScript>();
                                transform4.gameObject.CustomSetActive(false);
                                script3.enabled = false;
                                button.enabled = false;
                                GameObject obj3 = Utility.FindChild(transform2.gameObject, "heroDataPanel/heroPricePanel");
                                if (obj3 == null)
                                {
                                    DebugHelper.Assert(obj3 != null, "price panel is null");
                                }
                                else
                                {
                                    obj3.CustomSetActive(false);
                                    GameObject gameObject = transform.Find("imgExperienceMark").gameObject;
                                    CTextManager instance = Singleton<CTextManager>.GetInstance();
                                    if (masterRoleInfo.IsHaveHeroSkin(skin.dwHeroID, skin.dwSkinID, false))
                                    {
                                        transform4.gameObject.CustomSetActive(true);
                                        text3.text = instance.GetText("Mall_Skin_State_Own");
                                        gameObject.CustomSetActive(false);
                                    }
                                    else
                                    {
                                        gameObject.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(skin.dwHeroID, skin.dwSkinID));
                                        if (masterRoleInfo.IsCanBuySkinButNotHaveHero(skin.dwHeroID, skin.dwSkinID))
                                        {
                                            obj3.CustomSetActive(false);
                                            script3.enabled = true;
                                            transform4.gameObject.CustomSetActive(true);
                                            text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
                                            button.enabled = true;
                                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                                            script3.m_closeFormWhenClicked = true;
                                        }
                                        else
                                        {
                                            obj3.CustomSetActive(true);
                                            Image componetInChild = Utility.GetComponetInChild<Image>(obj3, "pnlExchange/costImage");
                                            Text text4 = Utility.GetComponetInChild<Text>(obj3, "pnlExchange/costText");
                                            if ((componetInChild != null) && (text4 != null))
                                            {
                                                componetInChild.SetSprite(this.GetExchangeTypeIconPath(enExchangeType.SkinExchangeCoupons), srcFormScript, true, false, false);
                                                if (shop != null)
                                                {
                                                    text4.text = shop.dwBuyItemCnt.ToString();
                                                }
                                                script3.enabled = true;
                                                transform4.gameObject.CustomSetActive(true);
                                                text3.text = Singleton<CTextManager>.GetInstance().GetText("Exchange_Btn");
                                                button.enabled = true;
                                                stUIEventParams params2 = new stUIEventParams {
                                                    tag = 2
                                                };
                                                if (shop != null)
                                                {
                                                    params2.commonUInt32Param1 = shop.dwBuyItemCnt;
                                                }
                                                params2.heroSkinParam.heroId = skin.dwHeroID;
                                                params2.heroSkinParam.skinId = skin.dwSkinID;
                                                script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_On_Exchange, params2);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnRecommendListTabChange(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseTips();
            if (Singleton<CUIManager>.GetInstance().GetForm(sMallHeroExchangeListPath) != null)
            {
                int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                this.CurListTab = (ListTab) selectedIndex;
                this.RefreshRecommendListPnl();
            }
        }

        private void OnRecommendTabChange(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseTips();
            CUICommonSystem.CloseCommonTips();
            CUICommonSystem.CloseUseableTips();
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Recommend_Hero_Skill_Up);
                int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                this.CurTab = (Tab) selectedIndex;
                switch (this.m_CurTab)
                {
                    case Tab.Hero:
                        this.UpdateHeroView();
                        this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.HeroExchangeCoupons);
                        this.RefreshPoolView();
                        break;

                    case Tab.Skin:
                        this.UpdateSkinView();
                        this.UpdateExchangeCouponsInfo(mallForm, mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/Status/GameObject"), enExchangeType.SkinExchangeCoupons);
                        this.RefreshPoolView();
                        break;
                }
            }
        }

        private void OnRecommendTimerEnd(CUIEvent uiEvent)
        {
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                Transform parent = srcWidget.transform.parent;
                if (parent != null)
                {
                    parent.gameObject.CustomSetActive(false);
                }
            }
        }

        [MessageHandler(0x49a)]
        public static void ReceiveLotteryRsp(CSPkg msg)
        {
            Singleton<CMallSystem>.GetInstance().ToggleActionMask(false, 30f);
            if (msg.stPkgData.stRandDrawRsp.iResult != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x49a, msg.stPkgData.stRandDrawRsp.iResult), false, 1.5f, null, new object[0]);
            }
            else
            {
                CMallRecommendController instance = Singleton<CMallRecommendController>.GetInstance();
                ResRandDrawInfo info = new ResRandDrawInfo();
                if (GameDataMgr.recommendLotteryCtrlDict.TryGetValue(msg.stPkgData.stRandDrawRsp.dwDrawID, out info))
                {
                    long doubleKey = GameDataMgr.GetDoubleKey(info.dwRewardPoolID, msg.stPkgData.stRandDrawRsp.bPoolIdx);
                    ResRewardPoolInfo info2 = new ResRewardPoolInfo();
                    if (GameDataMgr.recommendRewardDict.TryGetValue(doubleKey, out info2))
                    {
                        ListView<CUseable> items = new ListView<CUseable>();
                        CUseable item = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) info2.stRewardInfo.wItemType, info2.stRewardInfo.wItemCnt, info2.stRewardInfo.dwItemID);
                        if (item != null)
                        {
                            items.Add(item);
                            CUseable[] useableArray = new CUseable[items.Count];
                            for (int i = 0; i < useableArray.Length; i++)
                            {
                                useableArray[i] = items[i];
                            }
                            Singleton<CUIManager>.GetInstance().OpenAwardTip(useableArray, Singleton<CTextManager>.GetInstance().GetText("gotAward"), false, enUIEventID.None, false, false, "Form_Award");
                            Singleton<CMallRouletteController>.GetInstance().ShowHeroSkin(items);
                        }
                    }
                    else
                    {
                        object[] replaceArr = new object[] { "pool", doubleKey };
                        Singleton<CUIManager>.GetInstance().OpenTips("Mall_Recommend_Config_Err_Tips", true, 1f, null, replaceArr);
                    }
                }
                else
                {
                    object[] objArray2 = new object[] { "ctr", msg.stPkgData.stRandDrawRsp.dwDrawID };
                    Singleton<CUIManager>.GetInstance().OpenTips("Mall_Recommend_Config_Err_Tips", true, 1f, null, objArray2);
                }
            }
        }

        [MessageHandler(0x498)]
        public static void ReceiveRecommendBuyRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stSaleRecmdBuyRsp.iResult != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x498, msg.stPkgData.stSaleRecmdBuyRsp.iResult), false, 1.5f, null, new object[0]);
            }
            else
            {
                uint dwRcmdID = msg.stPkgData.stSaleRecmdBuyRsp.dwRcmdID;
                ResSaleRecommend recommend = new ResSaleRecommend();
                if (GameDataMgr.recommendProductDict.TryGetValue(dwRcmdID, out recommend) && (recommend.wSaleType == 4))
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(recommend.dwSaleID);
                    DebugHelper.Assert(dataByKey != null, string.Format("heroCfg databin error, Hero ID{0}", recommend.dwSaleID));
                    if (dataByKey != null)
                    {
                        CUICommonSystem.ShowNewHeroOrSkin(dataByKey.dwCfgID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
                    }
                    else
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(string.Format("heroCfg databin error, Hero ID:{0} not found", recommend.dwSaleID), false, 1.5f, null, new object[0]);
                    }
                }
            }
        }

        [MessageHandler(0x49b)]
        public static void ReceiveRecommendSyncIDNTF(CSPkg msg)
        {
            Singleton<CMallRecommendController>.GetInstance().RefreshPoolData(msg.stPkgData.stSyncRandDraw.dwDrawID);
        }

        private void Refresh3DModel(uint heroID, int skinId)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/3DImage");
                DebugHelper.Assert(obj2 != null);
                if (obj2 != null)
                {
                    CUI3DImageScript component = obj2.GetComponent<CUI3DImageScript>();
                    ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroID, skinId, true);
                    string objectName = data.ObjectName;
                    if (!string.IsNullOrEmpty(this.m_heroModelPath))
                    {
                        component.RemoveGameObject(this.m_heroModelPath);
                    }
                    this.m_heroModelPath = objectName;
                    GameObject model = component.AddGameObject(this.m_heroModelPath, false, false);
                    if (model != null)
                    {
                        model.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        if (data.ActorInfo != null)
                        {
                            model.transform.localScale = new Vector3(data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale);
                        }
                        CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                        instance.Set3DModel(model);
                        instance.InitAnimatList();
                        instance.InitAnimatSoundList(heroID, (uint) skinId);
                        instance.OnModePlayAnima("Come");
                    }
                }
            }
        }

        private void RefreshHeroBaseInfo(ResHeroCfgInfo heroCfg)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject p = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/heroInfoPanel");
                if (p != null)
                {
                    Text component = p.transform.Find("heroNameText").GetComponent<Text>();
                    Utility.FindChild(p, "heroTitleText").CustomSetActive(false);
                    GameObject gameObject = p.transform.Find("jobImage").gameObject;
                    IHeroData data = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
                    if (gameObject != null)
                    {
                        CUICommonSystem.SetHeroJob(mallForm, gameObject, (enHeroJobType) data.heroType);
                    }
                    if (component != null)
                    {
                        component.text = data.heroName;
                    }
                }
            }
        }

        private void RefreshHeroPricePnl(ResHeroCfgInfo heroCfg)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/coinBuy");
                GameObject obj3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/diamondBuy");
                GameObject obj4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/couponsBuy");
                GameObject obj5 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/exchangeBuy");
                GameObject obj6 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/linkBtnContainer");
                GameObject obj7 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/owned");
                obj2.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
                obj6.CustomSetActive(false);
                obj7.CustomSetActive(false);
                IHeroData data = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
                if (data.bPlayerOwn)
                {
                    obj7.CustomSetActive(true);
                }
                else
                {
                    ResHeroPromotion resPromotion = data.promotion();
                    stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroCfg, resPromotion);
                    for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
                    {
                        GameObject obj8 = null;
                        stPayInfo info = payInfoSetOfGood.m_payInfos[i];
                        switch (info.m_payType)
                        {
                            case enPayType.GoldCoin:
                                obj2.CustomSetActive(true);
                                obj8 = obj2;
                                break;

                            case enPayType.DianQuan:
                                obj4.CustomSetActive(true);
                                obj8 = obj4;
                                break;

                            case enPayType.Diamond:
                            case enPayType.DiamondAndDianQuan:
                                obj3.CustomSetActive(true);
                                obj8 = obj3;
                                break;
                        }
                        if (obj8 != null)
                        {
                            Transform transform = obj8.transform.Find("BuyButton");
                            if (transform != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams {
                                    tag = (int) info.m_payType,
                                    commonUInt32Param1 = info.m_payValue
                                };
                                eventParams.heroSkinParam.heroId = heroCfg.dwCfgID;
                                CMallSystem.SetPayButton(mallForm, transform as RectTransform, info.m_payType, info.m_payValue, enUIEventID.Mall_Recommend_On_Buy, ref eventParams);
                            }
                        }
                    }
                    if (obj5 != null)
                    {
                        ResHeroShop shop = null;
                        GameDataMgr.heroShopInfoDict.TryGetValue(heroCfg.dwCfgID, out shop);
                        if (shop != null)
                        {
                            uint exchangeValue = (shop.bIsBuyItem <= 0) ? 0 : shop.dwBuyItemCnt;
                            Transform transform2 = obj5.transform.Find("BuyButton");
                            stUIEventParams params2 = new stUIEventParams {
                                tag = 1,
                                commonUInt32Param1 = exchangeValue
                            };
                            params2.heroSkinParam.heroId = heroCfg.dwCfgID;
                            if (exchangeValue > 0)
                            {
                                obj5.CustomSetActive(true);
                                this.SetExchangeButton(mallForm, transform2 as RectTransform, enExchangeType.HeroExchangeCoupons, exchangeValue, enUIEventID.Mall_Recommend_On_Exchange, ref params2);
                            }
                        }
                    }
                }
            }
        }

        public bool RefreshPoolData(uint drawID = 0)
        {
            if (!this.SetCurrentLotteryCtrl(drawID))
            {
                return false;
            }
            if (!this.SetCurrentPoolData())
            {
                return false;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Recommend_Pool_Data_Refresh);
            return true;
        }

        private void RefreshPoolDataTimerHandler(int seq)
        {
            this.RefreshPoolData(0);
        }

        private void RefreshPoolView()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/rewardBody/List");
                if (obj2 != null)
                {
                    CUIListScript component = obj2.GetComponent<CUIListScript>();
                    ListView<ResRewardPoolInfo> view = new ListView<ResRewardPoolInfo>();
                    ResRandDrawInfo info = new ResRandDrawInfo();
                    switch (this.CurTab)
                    {
                        case Tab.Hero:
                            view = this.m_CurrentPoolList[1];
                            info = this.m_CurrentLottryCtrl[1];
                            break;

                        case Tab.Skin:
                            view = this.m_CurrentPoolList[2];
                            info = this.m_CurrentLottryCtrl[2];
                            break;
                    }
                    int count = view.Count;
                    component.SetElementAmount(count);
                    for (int i = 0; i < count; i++)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(i);
                        CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) view[i].stRewardInfo.wItemType, view[i].stRewardInfo.wItemCnt, view[i].stRewardInfo.dwItemID);
                        if ((elemenet != null) && (itemUseable != null))
                        {
                            GameObject itemCell = Utility.FindChild(elemenet.gameObject, "itemCell");
                            if (itemCell != null)
                            {
                                CUICommonSystem.SetItemCell(mallForm, itemCell, itemUseable, false, false);
                                if (itemUseable.m_stackCount == 1)
                                {
                                    Utility.FindChild(itemCell, "cntBg").CustomSetActive(false);
                                    Utility.FindChild(itemCell, "lblIconCount").CustomSetActive(false);
                                }
                            }
                        }
                    }
                    stPayInfo info2 = new stPayInfo {
                        m_payType = CMallSystem.ResBuyTypeToPayType(info.bCostType),
                        m_payValue = info.dwCostPrice
                    };
                    Transform transform = mallForm.transform.Find("pnlBodyBg/pnlRecommend/rewardBody/BuyButton");
                    stUIEventParams eventParams = new stUIEventParams();
                    CMallSystem.SetPayButton(mallForm, transform as RectTransform, info2.m_payType, info2.m_payValue, enUIEventID.Mall_Recommend_On_Lottery_Buy, ref eventParams);
                }
            }
        }

        public bool RefreshRecommendData()
        {
            if (!this.SetCurrentRecommendData())
            {
                return false;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Mall_Recommend_Recommend_Data_Refresh);
            return true;
        }

        private void RefreshRecommendDataTimerHandler(int seq)
        {
            this.RefreshRecommendData();
        }

        private void RefreshRecommendList(enExchangeType type)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallHeroExchangeListPath);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.gameObject, "Panel/HeroList");
                GameObject obj3 = Utility.FindChild(form.gameObject, "Panel/SkinList");
                if ((obj2 != null) && (obj3 != null))
                {
                    switch (type)
                    {
                        case enExchangeType.HeroExchangeCoupons:
                        {
                            obj2.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            CUIListScript component = obj2.GetComponent<CUIListScript>();
                            if (component == null)
                            {
                                return;
                            }
                            component.SetElementAmount(this.m_CurrentRecommendHeros.Count);
                            break;
                        }
                        case enExchangeType.SkinExchangeCoupons:
                        {
                            obj2.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            CUIListScript script3 = obj3.GetComponent<CUIListScript>();
                            if (script3 == null)
                            {
                                return;
                            }
                            script3.SetElementAmount(this.m_CurrentRecommendSkins.Count);
                            break;
                        }
                    }
                }
            }
        }

        public void RefreshRecommendListPnl()
        {
            <RefreshRecommendListPnl>c__AnonStorey7D storeyd = new <RefreshRecommendListPnl>c__AnonStorey7D();
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(sMallHeroExchangeListPath);
            if (form != null)
            {
                storeyd.role = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(storeyd.role != null, "master roleInfo is null");
                if (storeyd.role != null)
                {
                    switch (this.CurListTab)
                    {
                        case ListTab.Hero:
                            this.m_CurrentRecommendHeros.Sort(new Comparison<ResHeroCfgInfo>(storeyd.<>m__70));
                            this.RefreshRecommendList(enExchangeType.HeroExchangeCoupons);
                            this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.HeroExchangeCoupons);
                            break;

                        case ListTab.Skin:
                            this.m_CurrentRecommendSkins.Sort(new Comparison<ResHeroSkin>(storeyd.<>m__71));
                            this.RefreshRecommendList(enExchangeType.SkinExchangeCoupons);
                            this.UpdateExchangeCouponsInfo(form, form.transform.Find("Panel/pnlTotalMoney"), enExchangeType.SkinExchangeCoupons);
                            break;
                    }
                }
            }
        }

        private void RefreshRecommendView()
        {
            if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
                }
                else
                {
                    for (int i = 0; i < this.m_CurrentRecommendProductList.Count; i++)
                    {
                        ResSaleRecommend recommend = this.m_CurrentRecommendProductList[i];
                        switch (((COM_ITEM_TYPE) recommend.wSaleType))
                        {
                            case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                            {
                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(recommend.dwSaleID);
                                DebugHelper.Assert(dataByKey != null, string.Format("heroCfg databin error, Hero ID{0}", recommend.dwSaleID));
                                if (dataByKey != null)
                                {
                                    if ((this.m_HasHeroAndSkin & 1) == 0)
                                    {
                                        this.m_FirstHero = dataByKey;
                                        this.m_HasHeroAndSkin |= 1;
                                    }
                                    IHeroData data = CHeroDataFactory.CreateHeroData(this.m_FirstHero.dwCfgID);
                                    IHeroData data2 = CHeroDataFactory.CreateHeroData(dataByKey.dwCfgID);
                                    if (data.bPlayerOwn && !data2.bPlayerOwn)
                                    {
                                        this.m_FirstHero = dataByKey;
                                    }
                                    this.m_CurrentRecommendHeros.Add(dataByKey);
                                    long doubleKey = GameDataMgr.GetDoubleKey(recommend.wSaleType, recommend.dwSaleID);
                                    if (!this.m_CurrentRecommendProductMap.ContainsKey(doubleKey))
                                    {
                                        this.m_CurrentRecommendProductMap.Add(doubleKey, recommend.dwID);
                                    }
                                }
                                break;
                            }
                            case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                            {
                                uint heroId = 0;
                                uint skinId = 0;
                                CSkinInfo.ResolveHeroSkin(recommend.dwSaleID, out heroId, out skinId);
                                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                                DebugHelper.Assert(heroSkin != null, string.Format("skinCfg databin error, Skin ID{0}", recommend.dwSaleID));
                                if (heroSkin != null)
                                {
                                    if ((this.m_HasHeroAndSkin & 2) == 0)
                                    {
                                        this.m_FirstSkin = heroSkin;
                                        this.m_HasHeroAndSkin |= 2;
                                    }
                                    if (masterRoleInfo.IsHaveHeroSkin(this.m_FirstSkin.dwHeroID, this.m_FirstSkin.dwSkinID, false) && !masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                                    {
                                        this.m_FirstSkin = heroSkin;
                                    }
                                    this.m_CurrentRecommendSkins.Add(heroSkin);
                                    long key = GameDataMgr.GetDoubleKey(recommend.wSaleType, recommend.dwSaleID);
                                    if (!this.m_CurrentRecommendProductMap.ContainsKey(key))
                                    {
                                        this.m_CurrentRecommendProductMap.Add(key, recommend.dwID);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    this.InitRecommendTab();
                }
            }
        }

        private void RefreshSkillPanel(ResHeroCfgInfo heroCfg)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/SkillList");
                obj2.CustomSetActive(true);
                CUIListScript component = null;
                if (obj2 != null)
                {
                    component = obj2.GetComponent<CUIListScript>();
                }
                DebugHelper.Assert(component != null, "skill list is null");
                if (component == null)
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    IHeroData data = CHeroDataFactory.CreateHeroData(heroCfg.dwCfgID);
                    if (data != null)
                    {
                        ResDT_SkillInfo[] skillArr = data.skillArr;
                        component.SetElementAmount(skillArr.Length - 1);
                        for (int i = 0; i < (skillArr.Length - 1); i++)
                        {
                            CUIListElementScript elemenet = component.GetElemenet(i);
                            if (elemenet == null)
                            {
                                DebugHelper.Assert(elemenet != null, "list element is null");
                                break;
                            }
                            GameObject gameObject = component.GetElemenet(i).transform.Find("heroSkillItemCell").gameObject;
                            if (gameObject != null)
                            {
                                ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(skillArr[i].iSkillID);
                                CUIEventScript script4 = gameObject.GetComponent<CUIEventScript>();
                                if (skillCfgInfo == null)
                                {
                                    return;
                                }
                                GameObject obj4 = Utility.FindChild(gameObject, "skillMask/skillIcon");
                                if (obj4 == null)
                                {
                                    return;
                                }
                                Image image = obj4.GetComponent<Image>();
                                if (image == null)
                                {
                                    return;
                                }
                                string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, StringHelper.UTF8BytesToString(ref skillCfgInfo.szIconPath));
                                CUIUtility.SetImageSprite(image, prefabPath, elemenet.m_belongedFormScript, false, true, true);
                                obj4.CustomSetActive(true);
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.skillTipParam.skillName = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillName);
                                eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szSkillDesc, heroCfg.dwCfgID);
                                eventParams.skillTipParam.skillCoolDown = (i != 0) ? Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[1]) : Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5");
                                eventParams.skillTipParam.skillEnergyCost = (i != 0) ? Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText(heroCfg.dwEnergyType, EnergyShowType.CostValue), new string[] { ((int) skillCfgInfo.iEnergyCost).ToString() }) : string.Empty;
                                eventParams.skillTipParam.skillEffect = skillCfgInfo.SkillEffectType;
                                if (script4 != null)
                                {
                                    script4.SetUIEvent(enUIEventType.Down, enUIEventID.Mall_Recommend_Hero_Skill_Down, eventParams);
                                    script4.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
                                    script4.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
                                    script4.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Mall_Recommend_Hero_Skill_Up, eventParams);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshSkinBaseInfo(ResHeroSkin skinCfg)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject p = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/heroInfoPanel");
                if (p != null)
                {
                    Text component = p.transform.Find("heroNameText").GetComponent<Text>();
                    Utility.FindChild(p, "heroTitleText").CustomSetActive(false);
                    GameObject gameObject = p.transform.Find("jobImage").gameObject;
                    IHeroData data = CHeroDataFactory.CreateHeroData(skinCfg.dwHeroID);
                    if (gameObject != null)
                    {
                        CUICommonSystem.SetHeroJob(mallForm, gameObject, (enHeroJobType) data.heroType);
                    }
                    if (component != null)
                    {
                        component.text = string.Format("{0} {1}", data.heroName, StringHelper.UTF8BytesToString(ref skinCfg.szSkinName));
                    }
                }
            }
        }

        private void RefreshSkinPricePnl(ResHeroSkin skinCfg)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
                }
                else
                {
                    GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/coinBuy");
                    GameObject obj3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/diamondBuy");
                    GameObject obj4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/couponsBuy");
                    GameObject obj5 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/exchangeBuy");
                    GameObject obj6 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/owned");
                    GameObject obj7 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/pricePanel/linkBtnContainer");
                    obj2.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    if (masterRoleInfo.IsHaveHeroSkin(skinCfg.dwHeroID, skinCfg.dwSkinID, false))
                    {
                        obj6.CustomSetActive(true);
                    }
                    else if (masterRoleInfo.IsCanBuySkinButNotHaveHero(skinCfg.dwHeroID, skinCfg.dwSkinID))
                    {
                        obj7.CustomSetActive(true);
                        if (obj7 != null)
                        {
                            Text componetInChild = Utility.GetComponetInChild<Text>(obj7, "linkBtn/Text");
                            CUIEventScript script2 = Utility.GetComponetInChild<CUIEventScript>(obj7, "linkBtn");
                            if ((componetInChild != null) && (script2 != null))
                            {
                                componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
                                stUIEventParams eventParams = new stUIEventParams();
                                eventParams.openHeroFormPar.heroId = skinCfg.dwHeroID;
                                eventParams.openHeroFormPar.skinId = skinCfg.dwSkinID;
                                eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                            }
                        }
                    }
                    else
                    {
                        ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(skinCfg.dwID);
                        stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(skinCfg, skinPromotion);
                        for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
                        {
                            GameObject obj8 = null;
                            stPayInfo info2 = payInfoSetOfGood.m_payInfos[i];
                            switch (info2.m_payType)
                            {
                                case enPayType.GoldCoin:
                                    obj2.CustomSetActive(true);
                                    obj8 = obj2;
                                    break;

                                case enPayType.DianQuan:
                                    obj4.CustomSetActive(true);
                                    obj8 = obj4;
                                    break;

                                case enPayType.Diamond:
                                case enPayType.DiamondAndDianQuan:
                                    obj3.CustomSetActive(true);
                                    obj8 = obj3;
                                    break;
                            }
                            if (obj8 != null)
                            {
                                Transform transform = obj8.transform.Find("BuyButton");
                                if (transform != null)
                                {
                                    stUIEventParams params2 = new stUIEventParams {
                                        tag = (int) info2.m_payType,
                                        commonUInt32Param1 = info2.m_payValue
                                    };
                                    params2.heroSkinParam.heroId = skinCfg.dwHeroID;
                                    params2.heroSkinParam.skinId = skinCfg.dwSkinID;
                                    CMallSystem.SetPayButton(mallForm, transform as RectTransform, info2.m_payType, info2.m_payValue, enUIEventID.Mall_Recommend_On_Buy, ref params2);
                                }
                            }
                        }
                        if (obj5 != null)
                        {
                            ResHeroSkinShop shop = null;
                            GameDataMgr.skinShopInfoDict.TryGetValue(skinCfg.dwID, out shop);
                            if (shop != null)
                            {
                                uint exchangeValue = (shop.bIsBuyItem <= 0) ? 0 : shop.dwBuyItemCnt;
                                Transform transform2 = obj5.transform.Find("BuyButton");
                                stUIEventParams params3 = new stUIEventParams {
                                    tag = 2,
                                    commonUInt32Param1 = exchangeValue
                                };
                                params3.heroSkinParam.heroId = skinCfg.dwHeroID;
                                params3.heroSkinParam.skinId = skinCfg.dwSkinID;
                                if (exchangeValue > 0)
                                {
                                    obj5.CustomSetActive(true);
                                    this.SetExchangeButton(mallForm, transform2 as RectTransform, enExchangeType.SkinExchangeCoupons, exchangeValue, enUIEventID.Mall_Recommend_On_Exchange, ref params3);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshTimerView(Transform pnlRefreshTrans)
        {
            if (pnlRefreshTrans != null)
            {
                uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(pnlRefreshTrans.gameObject, "refreshTimer");
                int num2 = this.m_MostRecentRecommendRefreshTime - ((int) currentUTCTime);
                if ((num2 > 0) && (componetInChild != null))
                {
                    pnlRefreshTrans.gameObject.CustomSetActive(true);
                    componetInChild.SetTotalTime((float) num2);
                    componetInChild.ReStartTimer();
                }
                else
                {
                    pnlRefreshTrans.gameObject.CustomSetActive(false);
                }
            }
        }

        private void ReqBuy(uint recommendProductID, CS_SALERECMD_BUYTYPE buyType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x497);
            msg.stPkgData.stSaleRecmdBuyReq.bBuyType = (byte) buyType;
            msg.stPkgData.stSaleRecmdBuyReq.dwRcmdID = recommendProductID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private bool SetCurrentLotteryCtrl(uint drawID = 0)
        {
            byte num = 3;
            for (byte i = 0; i < num; i = (byte) (i + 1))
            {
                this.m_CurrentLottryCtrl[i] = null;
                this.m_MostRecentCtrlRefreshTime[i] = 0x7fffffff;
            }
            ResRandDrawInfo info = new ResRandDrawInfo();
            if (((drawID != 0) && GameDataMgr.recommendLotteryCtrlDict.TryGetValue(drawID, out info)) && ((info.bDrawType >= 1) && (info.bDrawType < 3)))
            {
                this.m_CurrentLottryCtrl[info.bDrawType] = info;
            }
            DictionaryView<uint, ResRandDrawInfo>.Enumerator enumerator = GameDataMgr.recommendLotteryCtrlDict.GetEnumerator();
            int num3 = 1;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResRandDrawInfo> current = enumerator.Current;
                ResRandDrawInfo info2 = current.Value;
                byte bDrawType = info2.bDrawType;
                if ((bDrawType < 1) || (info2.bDrawType > 3))
                {
                    DebugHelper.Assert(false, string.Format("recommend random draw config err, draw type{0} invalid!", bDrawType));
                    return false;
                }
                if ((info2.dwEndTimeGen == 0x7fffffff) && (info2.dwStartTimeGen == 0))
                {
                    this.m_DefaultLotteryCtrl[bDrawType] = info2;
                    num3++;
                }
                else
                {
                    uint dwStartTimeGen = info2.dwStartTimeGen;
                    uint dwEndTimeGen = info2.dwEndTimeGen;
                    if (dwStartTimeGen > dwEndTimeGen)
                    {
                        dwStartTimeGen ^= dwEndTimeGen;
                        dwEndTimeGen ^= dwStartTimeGen;
                        dwStartTimeGen = dwEndTimeGen ^ dwStartTimeGen;
                    }
                    if ((dwStartTimeGen < currentUTCTime) && (currentUTCTime < dwEndTimeGen))
                    {
                        if (this.m_CurrentLottryCtrl[bDrawType] == null)
                        {
                            info2.dwStartTimeGen = dwStartTimeGen;
                            info2.dwEndTimeGen = dwEndTimeGen;
                            this.m_CurrentLottryCtrl[bDrawType] = info2;
                        }
                    }
                    else if ((dwStartTimeGen > currentUTCTime) && (dwStartTimeGen < this.m_MostRecentCtrlRefreshTime[bDrawType]))
                    {
                        this.m_MostRecentCtrlRefreshTime[bDrawType] = (int) dwStartTimeGen;
                    }
                    num3++;
                }
            }
            for (int j = 1; j < num; j++)
            {
                if ((this.m_DefaultLotteryCtrl[j] == null) || (this.m_DefaultLotteryCtrl[j].dwDrawID == 0))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Recommend Lottery Ctrl Cfg Err", false, 1.5f, null, new object[0]);
                    return false;
                }
                if ((this.m_CurrentLottryCtrl[j] == null) || (this.m_CurrentLottryCtrl[j].dwDrawID == 0))
                {
                    this.m_CurrentLottryCtrl[j] = this.m_DefaultLotteryCtrl[j];
                }
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshLotteryCtrlTimerSeq[j]);
                if ((this.m_MostRecentCtrlRefreshTime[j] == 0x7fffffff) && (this.m_CurrentLottryCtrl[j].dwDrawID != this.m_DefaultLotteryCtrl[j].dwDrawID))
                {
                    this.m_MostRecentCtrlRefreshTime[j] = (int) this.m_CurrentLottryCtrl[j].dwEndTimeGen;
                }
                long num9 = (long) ((this.m_MostRecentCtrlRefreshTime[j] - currentUTCTime) * ((ulong) 0x3e8L));
                if ((num9 > 0L) && (num9 < 0x7fffffffL))
                {
                    this.m_RefreshLotteryCtrlTimerSeq[j] = Singleton<CTimerManager>.GetInstance().AddTimer((int) num9, 1, new CTimer.OnTimeUpHandler(this.RefreshPoolDataTimerHandler));
                }
            }
            return true;
        }

        private bool SetCurrentPoolData()
        {
            byte num = 3;
            for (byte i = 1; i < num; i = (byte) (i + 1))
            {
                if (this.m_CurrentPoolList[i] == null)
                {
                    this.m_CurrentPoolList[i] = new ListView<ResRewardPoolInfo>();
                }
                else
                {
                    this.m_CurrentPoolList[i].Clear();
                }
                DictionaryView<long, ResRewardPoolInfo>.Enumerator enumerator = GameDataMgr.recommendRewardDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<long, ResRewardPoolInfo> current = enumerator.Current;
                    ResRewardPoolInfo item = current.Value;
                    if ((item.dwPoolID == this.m_CurrentLottryCtrl[i].dwRewardPoolID) && (item.stRewardInfo.bIsShow > 0))
                    {
                        this.m_CurrentPoolList[i].Add(item);
                    }
                }
                if (this.m_CurrentPoolList[i].Count == 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Recommend Lottery Reward Cfg Err", false, 1.5f, null, new object[0]);
                    return false;
                }
            }
            return true;
        }

        private bool SetCurrentRecommendData()
        {
            this.m_MostRecentRecommendRefreshTime = 0x7fffffff;
            DictionaryView<uint, ResSaleRecommend>.Enumerator enumerator = GameDataMgr.recommendProductDict.GetEnumerator();
            int num = 1;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResSaleRecommend> current = enumerator.Current;
                ResSaleRecommend item = current.Value;
                if (item.dwOffTimeGen == 0x7fffffff)
                {
                    item.dwOnTimeGen = 0;
                    item.dwOffTimeGen = 0x7fffffff;
                }
                uint dwOnTimeGen = item.dwOnTimeGen;
                uint dwOffTimeGen = item.dwOffTimeGen;
                if (dwOnTimeGen > dwOffTimeGen)
                {
                    dwOnTimeGen ^= dwOffTimeGen;
                    dwOffTimeGen ^= dwOnTimeGen;
                    dwOnTimeGen = dwOffTimeGen ^ dwOnTimeGen;
                }
                if ((dwOnTimeGen < currentUTCTime) && (currentUTCTime < dwOffTimeGen))
                {
                    item.dwOnTimeGen = dwOnTimeGen;
                    item.dwOffTimeGen = dwOffTimeGen;
                    if (this.m_MostRecentRecommendRefreshTime > item.dwOffTimeGen)
                    {
                        this.m_MostRecentRecommendRefreshTime = (int) item.dwOffTimeGen;
                    }
                    this.m_CurrentRecommendProductList.Add(item);
                }
                else if ((dwOnTimeGen > currentUTCTime) && (dwOnTimeGen < this.m_MostRecentRecommendRefreshTime))
                {
                    this.m_MostRecentRecommendRefreshTime = (int) dwOnTimeGen;
                }
                num++;
            }
            if (this.m_CurrentRecommendProductList.Count == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Recommend Products Ctrl Cfg Err", false, 1.5f, null, new object[0]);
                return false;
            }
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = delegate (ResSaleRecommend a, ResSaleRecommend b) {
                    if ((a == null) && (b == null))
                    {
                        return 0;
                    }
                    if ((a == null) && (b != null))
                    {
                        return 1;
                    }
                    if ((b != null) || (a == null))
                    {
                        if (a.iRecmdStar < b.iRecmdStar)
                        {
                            return 1;
                        }
                        if (a.iRecmdStar == b.iRecmdStar)
                        {
                            return 0;
                        }
                        if (a.iRecmdStar > b.iRecmdStar)
                        {
                            return -1;
                        }
                    }
                    return -1;
                };
            }
            this.m_CurrentRecommendProductList.Sort(<>f__am$cache14);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshRecommendProductTimerSeq);
            long num5 = (long) ((this.m_MostRecentRecommendRefreshTime - currentUTCTime) * ((ulong) 0x3e8L));
            if ((num5 > 0L) && (num5 < 0x7fffffffL))
            {
                this.m_RefreshRecommendProductTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int) num5, 1, new CTimer.OnTimeUpHandler(this.RefreshRecommendDataTimerHandler));
            }
            return true;
        }

        public void SetExchangeButton(CUIFormScript formScript, RectTransform buttonTransform, enExchangeType exchangeType, uint exchangeValue, enUIEventID eventID, ref stUIEventParams eventParams)
        {
            if (((formScript != null) && (buttonTransform != null)) && (exchangeValue != 0))
            {
                Transform transform = buttonTransform.FindChild("Image");
                if (transform != null)
                {
                    Image image = transform.gameObject.GetComponent<Image>();
                    if (image != null)
                    {
                        image.SetSprite(this.GetExchangeTypeIconPath(exchangeType), formScript, true, false, false);
                    }
                }
                Transform transform2 = buttonTransform.FindChild("Text");
                if (transform2 != null)
                {
                    Text text = transform2.gameObject.GetComponent<Text>();
                    if (text != null)
                    {
                        text.text = exchangeValue.ToString();
                    }
                }
                CUIEventScript component = buttonTransform.gameObject.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.SetUIEvent(enUIEventType.Click, eventID, eventParams);
                }
            }
        }

        public static void TryToExchange(enExchangePurpose exchangePurpose, string goodName, enExchangeType exchangeType, uint exchangeValue, enUIEventID confirmEventID, ref stUIEventParams confirmEventParams, enUIEventID cancelEventID, bool needConfirm, bool guideToAchieveDianQuan)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (GetCurrencyValueFromRoleInfo(masterRoleInfo, exchangeType) >= exchangeValue)
                {
                    string str = string.Empty;
                    if (needConfirm)
                    {
                        string[] args = new string[] { exchangeValue.ToString(), Singleton<CTextManager>.GetInstance().GetText(s_exchangeTypeNameKeys[(int) exchangeType]), Singleton<CTextManager>.GetInstance().GetText(s_exchangePurposeNameKeys[(int) exchangePurpose]), goodName, str };
                        string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPay", args), new object[0]);
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, confirmEventID, cancelEventID, confirmEventParams, false);
                    }
                    else
                    {
                        CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                        uIEvent.m_eventID = confirmEventID;
                        uIEvent.m_eventParams = confirmEventParams;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                    }
                }
                else
                {
                    string str3 = string.Format(Singleton<CTextManager>.GetInstance().GetText("CurrencyNotEnough"), Singleton<CTextManager>.GetInstance().GetText(s_exchangeTypeNameKeys[(int) exchangeType]));
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(str3, cancelEventID, false);
                }
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_DefaultLotteryCtrl = null;
            this.m_CurrentLottryCtrl = null;
            this.m_CurrentPoolList = null;
            this.m_CurrentRecommendProductList = null;
            this.m_CurrentRecommendProductMap = null;
            this.m_CurrentRecommendHeros = null;
            this.m_CurrentRecommendSkins = null;
            this.m_MostRecentRecommendRefreshTime = 0;
            for (byte i = 0; i < 3; i = (byte) (i + 1))
            {
                this.m_MostRecentCtrlRefreshTime[i] = 0;
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_RefreshLotteryCtrlTimerSeq[i]);
                this.m_RefreshLotteryCtrlTimerSeq[i] = -1;
            }
            this.m_RefreshRecommendProductTimerSeq = -1;
            this.m_FirstHero = null;
            this.m_FirstSkin = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Lottery_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnLotteryBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendTabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Exchange_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_On_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnRecommendBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Exchange_More, new CUIEventManager.OnUIEventHandler(this.OnRecommendExchangeMore));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_List_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnRecommendListTabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListHeroEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Skin_List_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnRecommendListSkinEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Timer_End, new CUIEventManager.OnUIEventHandler(this.OnRecommendTimerEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Recommend_Hero_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Recommend_Recommend_Data_Refresh, new System.Action(this.RefreshRecommendView));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new System.Action(this.OnItemAdd));
        }

        private void UpdateExchangeCouponsInfo(CUIFormScript form, Transform container, enExchangeType exchangeType)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(masterRoleInfo != null, "Master RoleInfo Is NUll");
            }
            else
            {
                Image componetInChild = Utility.GetComponetInChild<Image>(container.gameObject, "Image");
                Text text = Utility.GetComponetInChild<Text>(container.gameObject, "Cnt");
                if ((componetInChild != null) && (text != null))
                {
                    componetInChild.SetSprite(this.GetExchangeTypeIconPath(exchangeType), form, true, false, false);
                    text.text = GetCurrencyValueFromRoleInfo(masterRoleInfo, exchangeType).ToString();
                }
            }
        }

        private void UpdateHeroView()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_recommendHeroInfoBgPath, mallForm);
                if (this.m_FirstHero != null)
                {
                    this.Refresh3DModel(this.m_FirstHero.dwCfgID, 0);
                    this.RefreshHeroBaseInfo(this.m_FirstHero);
                    this.RefreshHeroPricePnl(this.m_FirstHero);
                    this.RefreshSkillPanel(this.m_FirstHero);
                }
            }
        }

        private void UpdateSkinView()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_recommendHeroInfoBgPath, mallForm);
                this.Refresh3DModel(this.m_FirstSkin.dwHeroID, (int) this.m_FirstSkin.dwSkinID);
                this.RefreshSkinBaseInfo(this.m_FirstSkin);
                this.RefreshSkinPricePnl(this.m_FirstSkin);
                Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlRecommend/heroInfo/SkillList").CustomSetActive(false);
            }
        }

        public ListTab CurListTab
        {
            get
            {
                return this.m_CurListTab;
            }
            set
            {
                this.m_CurListTab = value;
            }
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshRecommendListPnl>c__AnonStorey7D
        {
            internal CRoleInfo role;

            internal int <>m__70(ResHeroCfgInfo l, ResHeroCfgInfo r)
            {
                bool flag = false;
                bool flag2 = false;
                if (this.role.IsHaveHero(l.dwCfgID, false))
                {
                    flag = true;
                }
                if (this.role.IsHaveHero(r.dwCfgID, false))
                {
                    flag2 = true;
                }
                if (!flag || !flag2)
                {
                    if (flag)
                    {
                        return 1;
                    }
                    if (flag2)
                    {
                        return -1;
                    }
                }
                return -1;
            }

            internal int <>m__71(ResHeroSkin l, ResHeroSkin r)
            {
                bool flag = false;
                bool flag2 = false;
                if (this.role.IsHaveHeroSkin(l.dwHeroID, l.dwSkinID, false))
                {
                    flag = true;
                }
                if (this.role.IsHaveHeroSkin(r.dwHeroID, r.dwSkinID, false))
                {
                    flag2 = true;
                }
                if (!flag || !flag2)
                {
                    if (flag)
                    {
                        return 1;
                    }
                    if (flag2)
                    {
                        return -1;
                    }
                }
                return -1;
            }
        }

        public enum ListTab
        {
            Hero = 0,
            None = -1,
            Skin = 1
        }

        public enum Tab
        {
            Hero = 0,
            None = -1,
            Skin = 1
        }
    }
}

