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

    public class CHeroSelectNormalSystem : Singleton<CHeroSelectNormalSystem>
    {
        private const float c_countDownCheckTime = 6.1f;
        private const int c_countDownTotalTime = 0x3b;
        private const int c_maxSelectedHeroIDsBeforeGC = 3;
        private ListView<IHeroData> m_canUseHeroListByJob = new ListView<IHeroData>();
        public string m_heroGameObjName = string.Empty;
        private enHeroJobType m_heroSelectJobType;
        public uint m_nowShowHeroID;
        private List<uint> m_selectedHeroModelIDs = new List<uint>();
        public uint m_showHeroID;
        public static string s_defaultHeroListName = "ListHostHeroInfo";
        public static string s_heroSelectFormPath = "UGUI/Form/System/HeroSelect/Form_HeroSelectNormal.prefab";
        public static string s_symbolPropPanelPath = "Other/Panel_SymbolProp";

        private void BuyHeroCount(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            msg.stPkgData.stShopBuyReq.iBuyType = 13;
            msg.stPkgData.stShopBuyReq.iBuySubType = Singleton<CHeroSelectBaseSystem>.instance.m_UseRandSelCount + 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroSelectFormPath);
        }

        private void HeroSelect_ClickCloseBtn(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer)
            {
                CHeroSelectBaseSystem.SendQuitSingleGameReq();
            }
            else
            {
                this.CloseForm();
            }
        }

        private void HeroSelect_CloseFullHeroList(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfo");
                Transform transform2 = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfoFull");
                Transform transform3 = form.gameObject.transform.Find("PanelLeft/MenuList");
                if ((transform != null) && (transform2 != null))
                {
                    transform.gameObject.CustomSetActive(true);
                    transform2.gameObject.CustomSetActive(false);
                    transform3.gameObject.CustomSetActive(false);
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    Singleton<CChatController>.instance.Set_Show_Bottom(true);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(true);
                }
            }
        }

        private void HeroSelect_ConfirmHeroSelect(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
            {
                CHeroSelectBaseSystem.SendMuliPrepareToBattleMsg();
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enArenaDefTeamConfig)
            {
                stUIEventParams par = new stUIEventParams {
                    heroIdList = new List<uint>()
                };
                for (int i = 0; i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount; i++)
                {
                    par.heroIdList.Add(Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[i]);
                }
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_ReciveDefTeamInfo, par);
                this.CloseForm();
            }
            else
            {
                Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
                if (Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    CFakePvPHelper.OnSelfHeroConfirmed();
                    this.SwitchSkinMenuSelect();
                }
                else
                {
                    CHeroSelectBaseSystem.SendSinglePrepareToBattleMsg(Singleton<CHeroSelectBaseSystem>.instance.roomInfo, Singleton<CHeroSelectBaseSystem>.instance.m_battleListID, Singleton<CHeroSelectBaseSystem>.instance.gameType, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount, Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList);
                }
                this.RefreshHeroPanel(false, true);
            }
            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    Transform transform = form.transform.FindChild("Other/RandomHero");
                    if (transform != null)
                    {
                        CUICommonSystem.SetButtonEnableWithShader(transform.GetComponent<Button>(), false, true);
                    }
                }
            }
        }

        private void HeroSelect_Del_Hero(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count) && (srcWidgetIndexInBelongedList >= 0))
            {
                if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[srcWidgetIndexInBelongedList] != 0)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.RemoveAt(srcWidgetIndexInBelongedList);
                    Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Add(0);
                    CHeroSelectBaseSystem instance = Singleton<CHeroSelectBaseSystem>.instance;
                    instance.m_selectHeroCount = (byte) (instance.m_selectHeroCount - 1);
                }
                this.m_showHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                this.RefreshHeroPanel(false, true);
                this.RefreshSkinPanel(null);
            }
        }

        private void HeroSelect_OnClose(CUIEvent uiEvent)
        {
            this.m_selectedHeroModelIDs.Clear();
            this.m_showHeroID = 0;
            this.m_nowShowHeroID = 0;
            this.m_heroSelectJobType = enHeroJobType.All;
            this.m_canUseHeroListByJob.Clear();
            if (uiEvent.m_srcWidget != null)
            {
                DynamicShadow.EnableDynamicShow(uiEvent.m_srcWidget, false);
            }
            OutlineFilter.EnableSurfaceShaderOutline(false);
            Singleton<CHeroSelectBaseSystem>.instance.Clear();
        }

        private void HeroSelect_OnMenuSelect(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                if ((Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom) && (selectedIndex == 0))
                {
                    form.gameObject.transform.Find("TabList").GetComponent<CUIListScript>().SelectElement(1, true);
                    Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("Luandou_heroabandon_Tips_1"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    Transform transform = form.gameObject.transform.Find("PanelLeft");
                    Transform transform2 = form.gameObject.transform.Find("PanelLeftSkin");
                    if (selectedIndex == 0)
                    {
                        transform.gameObject.CustomSetActive(true);
                        transform2.gameObject.CustomSetActive(false);
                        this.HeroSelect_CloseFullHeroList(null);
                        this.RefreshHeroPanel(false, true);
                    }
                    else
                    {
                        transform.gameObject.CustomSetActive(false);
                        transform2.gameObject.CustomSetActive(true);
                        this.RefreshSkinPanel(null);
                    }
                }
            }
        }

        private void HeroSelect_OnSkinSelect(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint showHeroID = this.m_showHeroID;
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            bool commonBool = uiEvent.m_eventParams.commonBool;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("PanelLeftSkin/ListHostSkinInfo");
                Transform transform2 = form.gameObject.transform.Find("PanelLeftSkin/ListHostSkinInfo/panelEffect/List");
                if ((transform != null) && (transform2 != null))
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (masterRoleInfo.IsCanUseSkin(showHeroID, tagUInt))
                    {
                        this.InitSkinEffect(transform2.gameObject, showHeroID, tagUInt);
                    }
                    else
                    {
                        component.SelectElement(component.GetLastSelectedIndex(), true);
                    }
                    if (masterRoleInfo.IsCanUseSkin(showHeroID, tagUInt))
                    {
                        if (masterRoleInfo.GetHeroWearSkinId(showHeroID) != tagUInt)
                        {
                            CHeroInfoSystem2.ReqWearHeroSkin(showHeroID, tagUInt, true);
                        }
                    }
                    else
                    {
                        CHeroSkinBuyManager.OpenBuyHeroSkinForm3D(showHeroID, tagUInt, false);
                    }
                }
            }
        }

        private void HeroSelect_OpenFullHeroList(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfo");
                Transform transform2 = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfoFull");
                Transform transform3 = form.gameObject.transform.Find("PanelLeft/MenuList");
                if ((transform != null) && (transform2 != null))
                {
                    transform.gameObject.CustomSetActive(false);
                    transform2.gameObject.CustomSetActive(true);
                    transform3.gameObject.CustomSetActive(true);
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    Singleton<CChatController>.instance.Hide_SelectChat_MidNode();
                    Singleton<CChatController>.instance.Set_Show_Bottom(false);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(false);
                }
            }
        }

        public void HeroSelect_SelectHero(IHeroData selectHeroData)
        {
            if ((Singleton<CHeroSelectBaseSystem>.instance.selectType != enSelectType.enRandom) && (((selectHeroData != null) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)) && (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count != 0)))
            {
                if (selectHeroData.cfgID > 0)
                {
                    if (this.m_selectedHeroModelIDs.Count >= 3)
                    {
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
                        Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
                        Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
                        this.m_selectedHeroModelIDs.Clear();
                    }
                    if (!this.m_selectedHeroModelIDs.Contains(selectHeroData.cfgID))
                    {
                        this.m_selectedHeroModelIDs.Add(selectHeroData.cfgID);
                    }
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
                {
                    CHeroSelectBaseSystem.SendHeroSelectMsg(0, 0, selectHeroData.cfgID);
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer)
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
                    {
                        return;
                    }
                    MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                    if (masterMemberInfo == null)
                    {
                        return;
                    }
                    masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = selectHeroData.cfgID;
                    this.m_showHeroID = selectHeroData.cfgID;
                    Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(this.m_showHeroID);
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count == 1)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(selectHeroData.cfgID);
                    this.m_showHeroID = selectHeroData.cfgID;
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount] = selectHeroData.cfgID;
                    CHeroSelectBaseSystem instance = Singleton<CHeroSelectBaseSystem>.instance;
                    instance.m_selectHeroCount = (byte) (instance.m_selectHeroCount + 1);
                    this.m_showHeroID = selectHeroData.cfgID;
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("hero is select over", false, 1.5f, null, new object[0]);
                }
                this.RefreshHeroPanel(false, true);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode() || (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count == 1))
                {
                    this.HeroSelect_CloseFullHeroList(null);
                }
                this.HeroSelect_Skill_Up(null);
                if (CFakePvPHelper.bInFakeSelect)
                {
                    CFakePvPHelper.OnSelfSelectHero(Singleton<CHeroSelectBaseSystem>.instance.roomInfo.selfInfo.ullUid, selectHeroData.cfgID);
                }
                uint[] param = new uint[] { Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.heroSelectedForBattle, param);
            }
        }

        private void HeroSelect_SelectHero(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_BanPick_Swicth", null);
            CUIListScript srcWidgetBelongedListScript = uiEvent.m_srcWidgetBelongedListScript;
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (srcWidgetBelongedListScript != null)
            {
                int count = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count;
                if (srcWidgetBelongedListScript.gameObject.name != s_defaultHeroListName)
                {
                    count = this.m_canUseHeroListByJob.Count;
                }
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < count))
                {
                    IHeroData selectHeroData = null;
                    if (srcWidgetBelongedListScript.gameObject.name != s_defaultHeroListName)
                    {
                        selectHeroData = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
                    }
                    else
                    {
                        selectHeroData = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList[srcWidgetIndexInBelongedList];
                    }
                    if (CHeroDataFactory.IsHeroCanUse(selectHeroData.cfgID) || Singleton<CHeroSelectBaseSystem>.instance.IsSpecTraingMode())
                    {
                        this.HeroSelect_SelectHero(selectHeroData);
                    }
                    else
                    {
                        string[] args = new string[] { selectHeroData.heroName };
                        string text = Singleton<CTextManager>.instance.GetText("ExpCard_Use", args);
                        stUIEventParams par = new stUIEventParams {
                            tagUInt = selectHeroData.cfgID
                        };
                        Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(text, enUIEventID.HeroSelect_UseHeroExpCard, enUIEventID.HeroSelect_UseHeroExpCardChanel, par, false);
                    }
                }
            }
        }

        private void HeroSelect_SelectTeamHero(CUIEvent uiEvent)
        {
            if (!Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if ((srcWidgetIndexInBelongedList < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList.Count) && (srcWidgetIndexInBelongedList >= 0))
                {
                    this.m_showHeroID = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[srcWidgetIndexInBelongedList];
                    this.RefreshHeroPanel(false, true);
                    this.RefreshSkinPanel(null);
                }
            }
        }

        private void HeroSelect_Skill_Down(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("Other/PanelSkillInfo");
                if (transform != null)
                {
                    Text component = transform.Find("lblName").gameObject.GetComponent<Text>();
                    Text text2 = transform.Find("lblDesc").gameObject.GetComponent<Text>();
                    Text text3 = transform.Find("SkillCDText").gameObject.GetComponent<Text>();
                    Text text4 = transform.Find("SkillEnergyCostText").gameObject.GetComponent<Text>();
                    component.text = uiEvent.m_eventParams.skillTipParam.strTipTitle;
                    text2.text = uiEvent.m_eventParams.skillTipParam.strTipText;
                    text3.text = uiEvent.m_eventParams.skillTipParam.skillCoolDown;
                    text4.text = uiEvent.m_eventParams.skillTipParam.skillEnergyCost;
                    uint[] skillEffect = uiEvent.m_eventParams.skillTipParam.skillEffect;
                    if (skillEffect != null)
                    {
                        GameObject gameObject = null;
                        for (int i = 1; i <= 2; i++)
                        {
                            gameObject = transform.transform.Find(string.Format("EffectNode{0}", i)).gameObject;
                            if ((i <= skillEffect.Length) && (skillEffect[i - 1] != 0))
                            {
                                gameObject.CustomSetActive(true);
                                gameObject.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffect[i - 1]), uiEvent.m_srcFormScript, true, false, false);
                                gameObject.transform.Find("Text").GetComponent<Text>().text = CSkillData.GetEffectDesc((SkillEffectType) skillEffect[i - 1]);
                            }
                            else
                            {
                                gameObject.CustomSetActive(false);
                            }
                        }
                        transform.gameObject.CustomSetActive(true);
                    }
                }
            }
        }

        private void HeroSelect_Skill_Up(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if ((form != null) && (form.gameObject != null))
            {
                Transform transform = form.gameObject.transform.Find("Other/PanelSkillInfo");
                GameObject obj2 = (transform == null) ? null : transform.gameObject;
                if (obj2 != null)
                {
                    obj2.gameObject.CustomSetActive(false);
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_ReqCloseForm, new CUIEventManager.OnUIEventHandler(this.HeroSelect_ClickCloseBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_FormClose, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_SelectHero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SelectHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_LeftHeroItemEnable, new CUIEventManager.OnUIEventHandler(this.LeftHeroItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_SelectTeamHero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SelectTeamHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_ConfirmHeroSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_ConfirmHeroSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_Del_Hero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Del_Hero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_Skill_Down, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_Skill_Up, new CUIEventManager.OnUIEventHandler(this.HeroSelect_Skill_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_OpenFullHeroList, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OpenFullHeroList));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_CloseFullHeroList, new CUIEventManager.OnUIEventHandler(this.HeroSelect_CloseFullHeroList));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_SymbolPageSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSymbolPageSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ViewProp_Down, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ViewProp_Up, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_MenuSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_RefreshSkinPanel, new CUIEventManager.OnUIEventHandler(this.RefreshSkinPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_SkinSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnSkinSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_HeroJobMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroJobMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_AddedSkillSelected, new CUIEventManager.OnUIEventHandler(this.OnSelectedAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_AddedSkillOpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_AddedSkillCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_AddedSkillConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_RandomHero, new CUIEventManager.OnUIEventHandler(this.OnReqRandHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroCount_Buy, new CUIEventManager.OnUIEventHandler(this.BuyHeroCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroCount_CancelBuy, new CUIEventManager.OnUIEventHandler(this.OnCancelBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_OnTimerCountDown, new CUIEventManager.OnUIEventHandler(this.OnTimerCountDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_UseHeroExpCard, new CUIEventManager.OnUIEventHandler(this.OnUseHeroExpCard));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_UseHeroExpCardChanel, new CUIEventManager.OnUIEventHandler(this.OnUseHeroExpCardChanel));
        }

        public void InitAddedSkillPanel(CUIFormScript form)
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null)
            {
                if (CAddSkillSys.IsSelSkillAvailable())
                {
                    CUIToggleListScript component = form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>();
                    CUIListElementScript elemenet = null;
                    CUIEventScript script3 = null;
                    ResSkillUnlock dataByIndex = null;
                    ResSkillCfgInfo dataByKey = null;
                    uint key = 0;
                    ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                    component.SetElementAmount(selSkillAvailable.Count);
                    int index = 0;
                    for (int i = 0; i < selSkillAvailable.Count; i++)
                    {
                        elemenet = component.GetElemenet(i);
                        script3 = elemenet.GetComponent<CUIEventScript>();
                        dataByIndex = selSkillAvailable[i];
                        key = dataByIndex.dwUnlockSkillID;
                        dataByKey = GameDataMgr.skillDatabin.GetDataByKey(key);
                        if (dataByKey != null)
                        {
                            script3.m_onClickEventID = enUIEventID.HeroSelect_AddedSkillSelected;
                            script3.m_onClickEventParams.tag = (int) dataByIndex.dwUnlockSkillID;
                            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                            elemenet.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, form.GetComponent<CUIFormScript>(), true, false, false);
                            elemenet.transform.Find("SkillNameTxt").GetComponent<Text>().text = Utility.UTF8Convert(dataByKey.szSkillName);
                        }
                        else
                        {
                            DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", key));
                        }
                    }
                    component.SelectElement(index, true);
                    dataByIndex = GameDataMgr.addedSkiilDatabin.GetDataByIndex(index);
                    form.transform.Find("Other/SkillList/AddedSkillItem").gameObject.CustomSetActive(selSkillAvailable.Count > 0);
                }
                else
                {
                    form.transform.Find("Other/SkillList/AddedSkillItem").gameObject.CustomSetActive(false);
                }
                form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
            }
        }

        private void InitFullHeroListData()
        {
            this.m_canUseHeroListByJob.Clear();
            ListView<IHeroData> canUseHeroList = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList;
            for (int i = 0; i < canUseHeroList.Count; i++)
            {
                if (((this.m_heroSelectJobType == enHeroJobType.All) || (canUseHeroList[i].heroCfgInfo.bMainJob == ((byte) this.m_heroSelectJobType))) || (canUseHeroList[i].heroCfgInfo.bMinorJob == ((byte) this.m_heroSelectJobType)))
                {
                    this.m_canUseHeroListByJob.Add(canUseHeroList[i]);
                }
            }
        }

        private void InitHeroJobMenu(CUIFormScript form)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
            string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
            string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
            string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
            string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
            string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
            string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
            string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
            CUICommonSystem.InitMenuPanel(form.transform.Find("PanelLeft/MenuList").gameObject, titleList, 0);
        }

        private void InitSkinEffect(GameObject objList, uint heroID, uint skinID)
        {
            CSkinInfo.GetHeroSkinProp(heroID, skinID, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
            CUICommonSystem.SetListProp(objList, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
        }

        private void LeftHeroItemEnable(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript srcWidgetBelongedListScript = uiEvent.m_srcWidgetBelongedListScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            int count = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count;
            if (srcWidgetBelongedListScript.gameObject.name != s_defaultHeroListName)
            {
                count = this.m_canUseHeroListByJob.Count;
            }
            if ((((srcFormScript != null) && (srcWidgetBelongedListScript != null)) && ((srcWidget != null) && (masterRoleInfo != null))) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < count)))
            {
                IHeroData data = null;
                if (srcWidgetBelongedListScript.gameObject.name != s_defaultHeroListName)
                {
                    data = this.m_canUseHeroListByJob[srcWidgetIndexInBelongedList];
                }
                else
                {
                    data = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList[srcWidgetIndexInBelongedList];
                }
                if (data != null)
                {
                    CUIListElementScript component = srcWidget.GetComponent<CUIListElementScript>();
                    if (component != null)
                    {
                        Image image = srcWidget.transform.Find("imgHP").gameObject.GetComponent<Image>();
                        Image image2 = srcWidget.transform.Find("imgDead").gameObject.GetComponent<Image>();
                        GameObject gameObject = srcWidget.transform.Find("heroItemCell").gameObject;
                        GameObject obj4 = gameObject.transform.Find("TxtFree").gameObject;
                        GameObject obj5 = gameObject.transform.Find("TxtCreditFree").gameObject;
                        GameObject obj6 = gameObject.transform.Find("imgExperienceMark").gameObject;
                        Transform targetTrans = gameObject.transform.Find("expCardPanel");
                        CUIEventScript script4 = gameObject.GetComponent<CUIEventScript>();
                        CUIEventScript script5 = srcWidget.GetComponent<CUIEventScript>();
                        bool flag = masterRoleInfo.IsFreeHero(data.cfgID);
                        bool bActive = masterRoleInfo.IsCreditFreeHero(data.cfgID);
                        obj4.CustomSetActive(flag && !bActive);
                        obj5.CustomSetActive(bActive);
                        if (masterRoleInfo.IsValidExperienceHero(data.cfgID) && !Singleton<CHeroSelectBaseSystem>.instance.IsSpecTraingMode())
                        {
                            obj6.CustomSetActive(true);
                        }
                        else
                        {
                            obj6.CustomSetActive(false);
                        }
                        if (!CHeroDataFactory.IsHeroCanUse(data.cfgID) && !Singleton<CHeroSelectBaseSystem>.instance.IsSpecTraingMode())
                        {
                            CUICommonSystem.SetObjActive(targetTrans, true);
                        }
                        else
                        {
                            CUICommonSystem.SetObjActive(targetTrans, false);
                        }
                        image.gameObject.CustomSetActive(false);
                        image2.gameObject.CustomSetActive(false);
                        script4.enabled = false;
                        script5.enabled = false;
                        if (!Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                        {
                            bool flag3 = Singleton<CHeroSelectBaseSystem>.instance.IsHeroExist(data.cfgID);
                            if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enBurning)
                            {
                                if (Singleton<BurnExpeditionController>.instance.model.IsHeroInRecord(data.cfgID))
                                {
                                    int num3 = Singleton<BurnExpeditionController>.instance.model.Get_HeroHP(data.cfgID);
                                    int num4 = Singleton<BurnExpeditionController>.instance.model.Get_HeroMaxHP(data.cfgID);
                                    if (num3 <= 0)
                                    {
                                        flag3 = true;
                                        image2.gameObject.CustomSetActive(true);
                                    }
                                    else
                                    {
                                        image.CustomFillAmount(((float) num3) / (num4 * 1f));
                                        image.gameObject.CustomSetActive(true);
                                    }
                                }
                                else
                                {
                                    image.CustomFillAmount(1f);
                                    image.gameObject.CustomSetActive(true);
                                }
                            }
                            if (!flag3)
                            {
                                script4.enabled = true;
                                script5.enabled = true;
                                CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, false);
                            }
                            else
                            {
                                CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, true);
                            }
                            if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount > 0)
                            {
                                if (data.cfgID == Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount - 1])
                                {
                                    component.ChangeDisplay(true);
                                }
                                else
                                {
                                    component.ChangeDisplay(false);
                                }
                            }
                            else
                            {
                                component.ChangeDisplay(false);
                            }
                        }
                        else
                        {
                            CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, true);
                        }
                    }
                }
            }
        }

        private void OnCancelBuy(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcFormScript.transform != null) && !Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
            {
                CUICommonSystem.SetButtonEnableWithShader(uiEvent.m_srcFormScript.transform.FindChild("Other/RandomHero").GetComponent<Button>(), true, true);
            }
        }

        public void OnCloseAddedSkillPanel(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    Singleton<CChatController>.instance.Set_Show_Bottom(true);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(true);
                }
            }
        }

        private void OnCloseSymbolProp(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.gameObject.transform.Find(s_symbolPropPanelPath).gameObject.gameObject.CustomSetActive(false);
            }
        }

        public void OnConfirmAddedSkill(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            uint tag = (uint) uiEvent.m_eventParams.tag;
            if (((form == null) || (Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill == null)) || !CAddSkillSys.IsSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, tag))
            {
                DebugHelper.Assert(false, string.Format("CHeroSelectNormalSystem addedSkillID[{0}]", tag));
            }
            else
            {
                this.RefreshAddedSkillItem(form, tag, true);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x48e);
                msg.stPkgData.stUnlockSkillSelReq.dwHeroID = this.m_showHeroID;
                msg.stPkgData.stUnlockSkillSelReq.dwSkillID = tag;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            this.OnCloseAddedSkillPanel(null);
        }

        public void OnHeroCountBought()
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
            {
                if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer)
                {
                    this.RadomHeroBySelfWithSingleMode();
                    this.RefreshHeroPanel(false, true);
                    this.RefreshSkinPanel(null);
                }
                CHeroSelectBaseSystem instance = Singleton<CHeroSelectBaseSystem>.instance;
                instance.m_UseRandSelCount++;
                this.RefreshLeftRandCountText();
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        public void OnHeroJobMenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            if (this.m_heroSelectJobType != selectedIndex)
            {
                this.m_heroSelectJobType = (enHeroJobType) selectedIndex;
                this.RefreshHeroInfo_LeftPanel(uiEvent.m_srcFormScript, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo());
            }
        }

        public void OnHeroSkinWearSuc(uint heroId, uint skinId)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath) != null)
            {
                this.m_nowShowHeroID = 0;
                this.RefreshHeroPanel(false, true);
                this.RefreshSkinPanel(null);
            }
        }

        public void OnHeroSymbolPageSelect(CUIEvent uiEvent)
        {
            CHeroInfo info2;
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uiEvent.m_srcFormScript.gameObject.transform.Find("Other").Find("Panel_SymbolChange/DropList/List").gameObject.CustomSetActive(false);
            bool flag = masterRoleInfo.GetHeroInfo(this.m_showHeroID, out info2, true);
            if (flag && (selectedIndex != info2.m_selectPageIndex))
            {
                CHeroSelectBaseSystem.SendHeroSelectSymbolPage(this.m_showHeroID, selectedIndex, false);
            }
            else if ((!flag && masterRoleInfo.IsFreeHero(this.m_showHeroID)) && (selectedIndex != masterRoleInfo.GetFreeHeroSymbolId(this.m_showHeroID)))
            {
                CHeroSelectBaseSystem.SendHeroSelectSymbolPage(this.m_showHeroID, selectedIndex, false);
            }
        }

        public void OnOpenAddedSkillPanel(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(true);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
                {
                    Singleton<CChatController>.instance.Hide_SelectChat_MidNode();
                    Singleton<CChatController>.instance.Set_Show_Bottom(false);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(false);
                }
            }
        }

        private void OnOpenSymbolProp(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CHeroInfo info2;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo.GetHeroInfo(this.m_showHeroID, out info2, true))
                {
                    this.OpenSymbolPropPanel(form, info2.m_selectPageIndex);
                }
                else if (masterRoleInfo.IsFreeHero(this.m_showHeroID))
                {
                    int freeHeroSymbolId = masterRoleInfo.GetFreeHeroSymbolId(this.m_showHeroID);
                    this.OpenSymbolPropPanel(form, freeHeroSymbolId);
                }
            }
        }

        public void OnReqRandHero(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
            {
                ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_ENTERTAINMENTRANDHERO, Singleton<CHeroSelectBaseSystem>.instance.m_UseRandSelCount + 1);
                if (cfgShopInfo != null)
                {
                    Transform transform = uiEvent.m_srcFormScript.transform.FindChild("Other/RandomHero");
                    CUICommonSystem.SetButtonEnableWithShader(transform.GetComponent<Button>(), false, true);
                    transform.transform.Find("Timer").GetComponent<CUITimerScript>().ReStartTimer();
                    uint dwCoinPrice = cfgShopInfo.dwCoinPrice;
                    int dwValue = (int) cfgShopInfo.dwValue;
                    enPayType payType = CMallSystem.ResBuyTypeToPayType(cfgShopInfo.bCoinType);
                    stUIEventParams confirmEventParams = new stUIEventParams();
                    CMallSystem.TryToPay(enPayPurpose.Buy, string.Empty, payType, dwCoinPrice, enUIEventID.HeroCount_Buy, ref confirmEventParams, enUIEventID.None, false, false, false);
                }
            }
        }

        public void OnSelectedAddedSkill(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                uint tag = (uint) uiEvent.m_eventParams.tag;
                form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) tag;
                ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(tag);
                if (dataByKey != null)
                {
                    string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, this.m_showHeroID);
                    form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().text = skillDescLobby;
                }
            }
        }

        public void OnSymbolPageChange()
        {
            this.RefreshHeroInfo_DropList();
        }

        public void OnSymbolPageDownBtnClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("Other/Panel_SymbolChange/DropList/List");
                transform.gameObject.CustomSetActive(!transform.gameObject.activeSelf);
            }
        }

        private void OnTimerCountDown(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcFormScript != null) && (uiEvent.m_srcWidget != null))
            {
                Transform transform = uiEvent.m_srcFormScript.transform.Find("CountDownMovie");
                CUITimerScript component = uiEvent.m_srcWidget.GetComponent<CUITimerScript>();
                if (((component.GetCurrentTime() <= 6.1f) && !transform.gameObject.activeSelf) && ((Singleton<CHeroSelectBaseSystem>.instance.selectType != enSelectType.enClone) || (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)))
                {
                    transform.gameObject.CustomSetActive(true);
                    component.gameObject.CustomSetActive(false);
                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_daojishi", null);
                }
            }
        }

        private void OnUseHeroExpCard(CUIEvent uiEvent)
        {
            CBagSystem.UseHeroExpCard(uiEvent.m_eventParams.tagUInt);
        }

        private void OnUseHeroExpCardChanel(CUIEvent uiEvent)
        {
            this.RefreshHeroPanel(false, true);
        }

        public void OpenForm()
        {
            OutlineFilter.EnableSurfaceShaderOutline(true);
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_heroSelectFormPath, false, true);
            Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSelectBgPath, formScript);
            CUICommonSystem.SetObjActive(formScript.transform.Find("CountDownMovie"), false);
            CUITimerScript component = formScript.transform.Find("CountDown/Timer").gameObject.GetComponent<CUITimerScript>();
            Button button = formScript.transform.Find("btnClose").gameObject.GetComponent<Button>();
            button.gameObject.CustomSetActive(false);
            component.gameObject.CustomSetActive(false);
            if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() || Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
            {
                if (!component.gameObject.activeInHierarchy)
                {
                    this.StartEndTimer(0x3b);
                }
            }
            else
            {
                button.gameObject.CustomSetActive(true);
            }
            if (formScript != null)
            {
                DynamicShadow.EnableDynamicShow(formScript.gameObject, true);
            }
            string[] titleList = new string[] { Singleton<CTextManager>.instance.GetText("Choose_Hero"), Singleton<CTextManager>.instance.GetText("Choose_Skin") };
            GameObject gameObject = formScript.gameObject.transform.Find("TabList").gameObject;
            int selectIndex = 0;
            this.InitHeroJobMenu(formScript);
            if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
            {
                selectIndex = 1;
                if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer)
                {
                    this.RadomHeroBySelfWithSingleMode();
                    this.RefreshHeroPanel(false, true);
                }
            }
            CUICommonSystem.InitMenuPanel(gameObject, titleList, selectIndex);
            this.RefreshLeftRandCountText();
            this.InitAddedSkillPanel(formScript);
            if (Singleton<CHeroSelectBaseSystem>.instance.IsSingleWarmBattle())
            {
                CFakePvPHelper.BeginFakeSelectHero();
            }
            Singleton<CReplayKitSys>.GetInstance().InitReplayKit(formScript.transform.Find("Other/ReplayKit"), true, true);
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterBattleHeroSel, new uint[0]);
        }

        private void OpenSymbolPropPanel(CUIFormScript form, int pageIndex)
        {
            GameObject gameObject = form.transform.Find(s_symbolPropPanelPath).gameObject;
            GameObject obj3 = gameObject.gameObject.transform.Find("basePropPanel").gameObject;
            GameObject obj4 = gameObject.gameObject.transform.Find("enhancePropPanel").gameObject;
            CSymbolSystem.RefreshSymbolPageProp(obj3.transform.Find("List").gameObject, pageIndex, true);
            obj4.CustomSetActive(!Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode());
            if (!Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
            {
                CSymbolSystem.RefreshSymbolPagePveEnhanceProp(obj4.transform.Find("List").gameObject, pageIndex);
            }
            gameObject.gameObject.CustomSetActive(true);
        }

        public void RadomHeroBySelfWithSingleMode()
        {
            bool flag = false;
            List<uint> list = new List<uint>();
            ResBanHeroConf dataByKey = GameDataMgr.banHeroBin.GetDataByKey(GameDataMgr.GetDoubleKey(4, Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.dwMapId));
            if (dataByKey != null)
            {
                for (int i = 0; i < dataByKey.BanHero.Length; i++)
                {
                    if (dataByKey.BanHero[i] != 0)
                    {
                        list.Add(dataByKey.BanHero[i]);
                    }
                }
            }
            while (!flag)
            {
                IHeroData data = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList[UnityEngine.Random.Range(0, Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count)];
                if (((Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0] != data.cfgID) && !list.Contains(data.cfgID)) || (Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count == 1))
                {
                    Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(data.cfgID);
                    this.m_showHeroID = data.cfgID;
                    MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                    if (masterMemberInfo == null)
                    {
                        return;
                    }
                    masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = data.cfgID;
                    flag = true;
                }
            }
        }

        public void RefreshAddedSkillItem(CUIFormScript form, uint addedSkillID, bool bForceRefresh = false)
        {
            if (CAddSkillSys.IsSelSkillAvailable())
            {
                GameObject gameObject = form.transform.Find("Other/SkillList/AddedSkillItem").gameObject;
                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(addedSkillID);
                if (dataByKey != null)
                {
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                    gameObject.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, form, true, false, false);
                    gameObject.transform.Find("SkillNameTxt").GetComponent<Text>().text = Utility.UTF8Convert(dataByKey.szSkillName);
                    string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, this.m_showHeroID);
                    stUIEventParams eventParams = new stUIEventParams {
                        skillTipParam = new stSkillTipParams()
                    };
                    eventParams.skillTipParam.strTipText = skillDescLobby;
                    eventParams.skillTipParam.strTipTitle = StringHelper.UTF8BytesToString(ref dataByKey.szSkillName);
                    string[] args = new string[] { ((int) (dataByKey.iCoolDown / 0x3e8)).ToString() };
                    eventParams.skillTipParam.skillCoolDown = Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", args);
                    eventParams.skillTipParam.skillEffect = dataByKey.SkillEffectType;
                    eventParams.skillTipParam.skillEnergyCost = (dataByKey.iEnergyCost != 0) ? Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText(dataByKey.dwEnergyCostType, EnergyShowType.CostValue), new string[] { ((int) dataByKey.iEnergyCost).ToString() }) : string.Empty;
                    component.SetUIEvent(enUIEventType.Down, enUIEventID.HeroSelect_Skill_Down, eventParams);
                    if (bForceRefresh)
                    {
                        form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().text = skillDescLobby;
                        form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) addedSkillID;
                        ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                        for (int i = 0; i < selSkillAvailable.Count; i++)
                        {
                            if (selSkillAvailable[i].dwUnlockSkillID == addedSkillID)
                            {
                                form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>().SelectElement(i, true);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", addedSkillID));
                }
            }
        }

        private void RefreshHeroInfo_CenterPanel(CUIFormScript form, CRoleInfo roleInfo, CUIListScript skillList)
        {
            CUI3DImageScript component = form.transform.Find("PanelCenter/3DImage").gameObject.GetComponent<CUI3DImageScript>();
            Image image = form.transform.Find("Other/HeroInfo/imgJob").gameObject.GetComponent<Image>();
            Text text = form.transform.Find("Other/HeroInfo/HeroName/lblName").gameObject.GetComponent<Text>();
            Text text2 = form.transform.Find("Other/HeroInfo/HeroName/jobTitleText").gameObject.GetComponent<Text>();
            Text text3 = form.transform.Find("Other/HeroInfo/HeroJob/jobFeatureText").gameObject.GetComponent<Text>();
            if (this.m_showHeroID == 0)
            {
                text.gameObject.CustomSetActive(false);
                image.gameObject.CustomSetActive(false);
                text2.gameObject.CustomSetActive(false);
                text3.gameObject.CustomSetActive(false);
            }
            if (this.m_nowShowHeroID != this.m_showHeroID)
            {
                component.RemoveGameObject(this.m_heroGameObjName);
                this.m_nowShowHeroID = this.m_showHeroID;
                if (this.m_nowShowHeroID != 0)
                {
                    int heroWearSkinId = (int) roleInfo.GetHeroWearSkinId(this.m_nowShowHeroID);
                    ObjNameData data = CUICommonSystem.GetHeroPrefabPath(this.m_nowShowHeroID, heroWearSkinId, true);
                    this.m_heroGameObjName = data.ObjectName;
                    GameObject model = component.AddGameObject(this.m_heroGameObjName, false, false);
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
                        instance.InitAnimatSoundList(this.m_nowShowHeroID, (uint) heroWearSkinId);
                        instance.OnModePlayAnima("Come");
                    }
                    IHeroData data2 = CHeroDataFactory.CreateHeroData(this.m_nowShowHeroID);
                    if (data2 != null)
                    {
                        ResDT_SkillInfo[] skillArr = data2.skillArr;
                        skillList.SetElementAmount(skillArr.Length - 1);
                        for (int i = 0; i < (skillArr.Length - 1); i++)
                        {
                            GameObject gameObject = skillList.GetElemenet(i).gameObject.transform.Find("heroSkillItemCell").gameObject;
                            ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(skillArr[i].iSkillID);
                            CUIEventScript script2 = gameObject.GetComponent<CUIEventScript>();
                            if (skillCfgInfo == null)
                            {
                                return;
                            }
                            if (i == 0)
                            {
                                gameObject.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
                            }
                            else
                            {
                                gameObject.transform.localScale = Vector3.one;
                            }
                            GameObject obj4 = Utility.FindChild(gameObject, "skillMask/skillIcon");
                            if (obj4 == null)
                            {
                                return;
                            }
                            Image image2 = obj4.GetComponent<Image>();
                            if (image2 == null)
                            {
                                return;
                            }
                            CUIUtility.SetImageSprite(image2, CUIUtility.s_Sprite_Dynamic_Skill_Dir + StringHelper.UTF8BytesToString(ref skillCfgInfo.szIconPath), form, true, false, false);
                            obj4.CustomSetActive(true);
                            stUIEventParams eventParams = new stUIEventParams();
                            stSkillTipParams params2 = new stSkillTipParams();
                            eventParams.skillTipParam = params2;
                            if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
                            {
                                eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szSkillDesc, this.m_nowShowHeroID);
                                eventParams.skillTipParam.strTipTitle = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillName);
                            }
                            else if (data2 is CHeroInfoData)
                            {
                                eventParams.skillTipParam.strTipText = ((CHeroInfoData) data2).m_info.skillInfo.GetSkillDesc(i);
                                eventParams.skillTipParam.strTipTitle = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillName);
                            }
                            else
                            {
                                eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szSkillDesc, this.m_nowShowHeroID);
                                eventParams.skillTipParam.strTipTitle = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillName);
                            }
                            eventParams.skillTipParam.skillCoolDown = (i != 0) ? Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[1]) : Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5");
                            eventParams.skillTipParam.skillEnergyCost = (i != 0) ? Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText(skillCfgInfo.dwEnergyCostType, EnergyShowType.CostValue), new string[] { ((int) skillCfgInfo.iEnergyCost).ToString() }) : string.Empty;
                            eventParams.skillTipParam.skillEffect = skillCfgInfo.SkillEffectType;
                            script2.SetUIEvent(enUIEventType.Down, enUIEventID.HeroSelect_Skill_Down, eventParams);
                        }
                        text.text = data2.heroName;
                        CUICommonSystem.SetHeroJob(form, image.gameObject, (enHeroJobType) data2.heroType);
                        text.gameObject.CustomSetActive(true);
                        image.gameObject.CustomSetActive(true);
                        text2.gameObject.CustomSetActive(true);
                        text3.gameObject.CustomSetActive(true);
                        text2.text = CHeroInfo.GetHeroJob(this.m_nowShowHeroID);
                        text3.text = CHeroInfo.GetJobFeature(this.m_nowShowHeroID);
                    }
                }
            }
        }

        private void RefreshHeroInfo_ConfirmButtonPanel(CUIFormScript form, CRoleInfo roleInfo)
        {
            Button component = form.transform.Find("PanelRight/btnConfirm").gameObject.GetComponent<Button>();
            bool isSelectConfirm = true;
            if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
            {
                isSelectConfirm = Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm;
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo == null)
                {
                    return;
                }
                if (masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID == 0)
                {
                    isSelectConfirm = true;
                }
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount == 0)
            {
                isSelectConfirm = true;
            }
            else if (!Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
            {
                isSelectConfirm = false;
            }
            if (isSelectConfirm)
            {
                CUICommonSystem.SetButtonEnableWithShader(component.GetComponent<Button>(), false, true);
            }
            else
            {
                CUICommonSystem.SetButtonEnableWithShader(component.GetComponent<Button>(), true, true);
            }
        }

        public void RefreshHeroInfo_DropList()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("Other/Panel_SymbolChange");
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL) && (GameDataMgr.heroDatabin.GetDataByKey(this.m_showHeroID) != null))
                {
                    CHeroInfo info2;
                    int selectIndex = 0;
                    if (masterRoleInfo.GetHeroInfo(this.m_showHeroID, out info2, true))
                    {
                        selectIndex = info2.m_selectPageIndex;
                    }
                    else if (masterRoleInfo.IsFreeHero(this.m_showHeroID))
                    {
                        selectIndex = masterRoleInfo.GetFreeHeroSymbolId(this.m_showHeroID);
                    }
                    transform.gameObject.CustomSetActive(true);
                    SetPageDropListDataByHeroSelect(transform.gameObject, selectIndex);
                }
                else
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        private void RefreshHeroInfo_ExperiencePanel(CUIFormScript form)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            GameObject widget = form.GetWidget(0);
            GameObject obj3 = form.GetWidget(1);
            GameObject obj4 = form.GetWidget(2);
            GameObject obj5 = form.GetWidget(3);
            widget.CustomSetActive(false);
            obj3.CustomSetActive(false);
            obj4.CustomSetActive(false);
            obj5.CustomSetActive(false);
            if (masterRoleInfo.IsValidExperienceHero(this.m_showHeroID))
            {
                CUICommonSystem.RefreshExperienceHeroLeftTime(widget, obj4, this.m_showHeroID);
            }
            uint heroWearSkinId = masterRoleInfo.GetHeroWearSkinId(this.m_showHeroID);
            if (masterRoleInfo.IsValidExperienceSkin(this.m_showHeroID, heroWearSkinId))
            {
                CUICommonSystem.RefreshExperienceSkinLeftTime(obj3, obj5, this.m_showHeroID, heroWearSkinId, null);
            }
        }

        private void RefreshHeroInfo_LeftPanel(CUIFormScript form, CRoleInfo roleInfo)
        {
            CUIListScript component = form.transform.Find("PanelLeft/ListHostHeroInfo").gameObject.GetComponent<CUIListScript>();
            CUIListScript script2 = form.transform.Find("PanelLeft/ListHostHeroInfoFull").gameObject.GetComponent<CUIListScript>();
            component.m_alwaysDispatchSelectedChangeEvent = true;
            script2.m_alwaysDispatchSelectedChangeEvent = true;
            component.SetElementAmount(Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count);
            this.InitFullHeroListData();
            script2.SetElementAmount(this.m_canUseHeroListByJob.Count);
        }

        private void RefreshHeroInfo_RightPanel(CUIFormScript form, CRoleInfo roleInfo)
        {
            <RefreshHeroInfo_RightPanel>c__AnonStorey78 storey = new <RefreshHeroInfo_RightPanel>c__AnonStorey78 {
                form = form
            };
            CUIListScript component = storey.form.transform.Find("PanelRight/ListTeamHeroInfo").gameObject.GetComponent<CUIListScript>();
            component.m_alwaysDispatchSelectedChangeEvent = true;
            List<uint> teamHeroList = null;
            if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
            {
                if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
                {
                    return;
                }
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo == null)
                {
                    return;
                }
                teamHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetTeamHeroList(masterMemberInfo.camp);
            }
            else
            {
                teamHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetTeamHeroList(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
            component.SetElementAmount(teamHeroList.Count);
            for (int i = 0; i < teamHeroList.Count; i++)
            {
                <RefreshHeroInfo_RightPanel>c__AnonStorey77 storey2 = new <RefreshHeroInfo_RightPanel>c__AnonStorey77();
                GameObject gameObject = component.GetElemenet(i).gameObject;
                GameObject item = gameObject.transform.Find("heroItemCell").gameObject;
                GameObject obj4 = item.transform.Find("ItemBg1").gameObject;
                GameObject obj5 = item.transform.Find("ItemBg2").gameObject;
                GameObject obj6 = item.transform.Find("redReadyIcon").gameObject;
                GameObject obj7 = item.transform.Find("redReadyIcon").gameObject;
                GameObject obj8 = item.transform.Find("selfIcon").gameObject;
                GameObject obj9 = item.transform.Find("delBtn").gameObject;
                storey2.selSkillCell = gameObject.transform.Find("selSkillItemCell").gameObject;
                Image image = item.transform.Find("imageIcon").gameObject.GetComponent<Image>();
                Text text = item.transform.Find("lblName").gameObject.GetComponent<Text>();
                CUIEventScript script2 = item.GetComponent<CUIEventScript>();
                uint id = teamHeroList[i];
                text.text = string.Empty;
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
                obj6.CustomSetActive(false);
                obj7.CustomSetActive(false);
                obj8.CustomSetActive(false);
                obj9.CustomSetActive(false);
                storey2.selSkillCell.CustomSetActive(false);
                script2.enabled = false;
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
                {
                    <RefreshHeroInfo_RightPanel>c__AnonStorey76 storey3 = new <RefreshHeroInfo_RightPanel>c__AnonStorey76 {
                        <>f__ref$120 = storey,
                        <>f__ref$119 = storey2
                    };
                    MemberInfo info3 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                    ListView<MemberInfo> view = Singleton<CHeroSelectBaseSystem>.instance.roomInfo[info3.camp];
                    MemberInfo info4 = view[i];
                    text.text = info4.MemberName;
                    if ((Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer) && (info3.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID != 0))
                    {
                        info3.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID = CAddSkillSys.GetSelfSelSkill(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, info3.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                    }
                    storey3.selSkillID = info4.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
                    if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode() && (storey3.selSkillID != 0))
                    {
                        GameDataMgr.addedSkiilDatabin.Accept(new Action<ResSkillUnlock>(storey3.<>m__5F));
                    }
                    if ((info4.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.roomInfo.selfObjID) && (info4.RoomMemberType != 2))
                    {
                        obj4.CustomSetActive(true);
                        if (info4.isPrepare)
                        {
                            obj6.CustomSetActive(true);
                        }
                        string[] args = new string[] { info4.MemberName };
                        text.text = Singleton<CTextManager>.instance.GetText("Pvp_PlayerName", args);
                    }
                    else
                    {
                        obj5.CustomSetActive(true);
                        if (info4.isPrepare)
                        {
                            obj7.CustomSetActive(true);
                        }
                    }
                }
                else
                {
                    if (id != 0)
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData(id);
                        if (data == null)
                        {
                            return;
                        }
                        text.text = data.heroName;
                    }
                    obj4.CustomSetActive(true);
                    if (i == 0)
                    {
                        obj8.CustomSetActive(true);
                    }
                    if (i < Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount)
                    {
                        obj9.CustomSetActive(true);
                    }
                }
                if (id != 0)
                {
                    IHeroData data2 = CHeroDataFactory.CreateHeroData(id);
                    if (data2 == null)
                    {
                        return;
                    }
                    CUICommonSystem.SetHeroItemData(storey.form, item, data2, enHeroHeadType.enIcon, false);
                }
                else if (!Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
                {
                    image.SetSprite(CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroChoose_unknownIcon", storey.form, true, false, false);
                }
                else
                {
                    MemberInfo info5 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                    ListView<MemberInfo> view2 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo[info5.camp];
                    MemberInfo info6 = view2[i];
                    if ((info6.RoomMemberType == 2) && !Singleton<CHeroSelectBaseSystem>.instance.roomInfo.roomAttrib.bWarmBattle)
                    {
                        image.SetSprite(CUIUtility.s_Sprite_System_HeroSelect_Dir + "Img_ComputerHead", storey.form, true, false, false);
                    }
                    else
                    {
                        image.SetSprite(CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroChoose_unknownIcon", storey.form, true, false, false);
                    }
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMobaMode())
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm && (id != 0))
                    {
                        script2.enabled = true;
                    }
                }
                else if (id != 0)
                {
                    script2.enabled = true;
                }
            }
        }

        private void RefreshHeroInfo_SpecSkillPanel(CUIListScript skillList, CRoleInfo roleInfo, bool bForceRefreshAddSkillPanel, CUIFormScript form)
        {
            skillList.gameObject.CustomSetActive(false);
            if (this.m_nowShowHeroID != 0)
            {
                skillList.gameObject.CustomSetActive(true);
                if (CAddSkillSys.IsSelSkillAvailable())
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
                    {
                        MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                        if (masterMemberInfo != null)
                        {
                            this.RefreshAddedSkillItem(form, masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID, bForceRefreshAddSkillPanel);
                        }
                    }
                    else if (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enPVE_Computer)
                    {
                        uint selfSelSkill = CAddSkillSys.GetSelfSelSkill(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, this.m_showHeroID);
                        this.RefreshAddedSkillItem(form, selfSelSkill, bForceRefreshAddSkillPanel);
                    }
                }
            }
        }

        public void RefreshHeroPanel(bool bForceRefreshAddSkillPanel = false, bool bRefreshSymbol = true)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((form != null) && (masterRoleInfo != null))
            {
                CUIListScript component = form.transform.Find("Other/SkillList").gameObject.GetComponent<CUIListScript>();
                if (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroCount <= 0)
                {
                    CUICommonSystem.PlayAnimator(form.gameObject, "show");
                }
                else
                {
                    CUICommonSystem.PlayAnimator(form.gameObject, "hide");
                }
                this.RefreshHeroInfo_LeftPanel(form, masterRoleInfo);
                this.RefreshHeroInfo_RightPanel(form, masterRoleInfo);
                this.RefreshHeroInfo_CenterPanel(form, masterRoleInfo, component);
                this.RefreshHeroInfo_SpecSkillPanel(component, masterRoleInfo, bForceRefreshAddSkillPanel, form);
                if (bRefreshSymbol)
                {
                    this.RefreshHeroInfo_DropList();
                }
                this.RefreshHeroInfo_ExperiencePanel(form);
                this.RefreshHeroInfo_ConfirmButtonPanel(form, masterRoleInfo);
            }
        }

        private void RefreshLeftRandCountText()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.transform.FindChild("Other/RandomHero");
                if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
                {
                    transform.gameObject.CustomSetActive(true);
                    ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_ENTERTAINMENTRANDHERO, Singleton<CHeroSelectBaseSystem>.instance.m_UseRandSelCount + 1);
                    if (cfgShopInfo != null)
                    {
                        stPayInfo info2 = new stPayInfo {
                            m_payType = CMallSystem.ResBuyTypeToPayType(cfgShopInfo.bCoinType),
                            m_payValue = cfgShopInfo.dwCoinPrice
                        };
                        stUIEventParams eventParams = new stUIEventParams();
                        CMallSystem.SetPayButton(form, transform.transform as RectTransform, info2.m_payType, info2.m_payValue, enUIEventID.HeroSelect_RandomHero, ref eventParams);
                    }
                    else
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                }
                else
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        public void RefreshSkinPanel(CUIEvent uiEvent = null)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint showHeroID = this.m_showHeroID;
            ListView<ResHeroSkin> view = new ListView<ResHeroSkin>();
            ListView<ResHeroSkin> collection = new ListView<ResHeroSkin>();
            int index = -1;
            ResHeroSkin item = null;
            if (showHeroID != 0)
            {
                ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(showHeroID);
                for (int i = 0; i < availableSkinByHeroId.Count; i++)
                {
                    item = availableSkinByHeroId[i];
                    if (masterRoleInfo.IsCanUseSkin(showHeroID, item.dwSkinID) || CBagSystem.IsHaveSkinExpCard(item.dwID))
                    {
                        view.Add(item);
                    }
                    else
                    {
                        collection.Add(item);
                    }
                    if (masterRoleInfo.GetHeroWearSkinId(showHeroID) == item.dwSkinID)
                    {
                        index = view.Count - 1;
                    }
                }
                view.AddRange(collection);
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("PanelLeftSkin/ListHostSkinInfo");
                Transform transform2 = form.gameObject.transform.Find("PanelLeftSkin/ListHostSkinInfo/panelEffect");
                if (transform != null)
                {
                    CUIListScript[] scriptArray1 = new CUIListScript[] { transform.GetComponent<CUIListScript>() };
                    foreach (CUIListScript script2 in scriptArray1)
                    {
                        script2.SetElementAmount(view.Count);
                        for (int j = 0; j < view.Count; j++)
                        {
                            CUIListElementScript elemenet = script2.GetElemenet(j);
                            Transform transform3 = script2.GetElemenet(j).transform;
                            Image component = transform3.Find("imageIcon").GetComponent<Image>();
                            Image image = transform3.Find("imageIconGray").GetComponent<Image>();
                            Text text = transform3.Find("lblName").GetComponent<Text>();
                            GameObject gameObject = transform3.Find("imgExperienceMark").gameObject;
                            Transform transform4 = transform3.Find("expCardPanel");
                            item = view[j];
                            bool bActive = masterRoleInfo.IsValidExperienceSkin(showHeroID, item.dwSkinID);
                            gameObject.CustomSetActive(bActive);
                            bool flag2 = !masterRoleInfo.IsCanUseSkin(showHeroID, item.dwSkinID) && CBagSystem.IsHaveSkinExpCard(item.dwID);
                            RectTransform transform5 = (RectTransform) text.transform;
                            RectTransform transform6 = (RectTransform) transform4;
                            if (flag2)
                            {
                                transform4.gameObject.CustomSetActive(true);
                                transform5.anchoredPosition = new Vector2(transform5.anchoredPosition.x, transform6.anchoredPosition.y + transform5.rect.height);
                            }
                            else
                            {
                                transform4.gameObject.CustomSetActive(false);
                                transform5.anchoredPosition = new Vector2(transform5.anchoredPosition.x, transform6.anchoredPosition.y);
                            }
                            if (masterRoleInfo.IsCanUseSkin(showHeroID, item.dwSkinID) || CBagSystem.IsHaveSkinExpCard(item.dwID))
                            {
                                component.gameObject.CustomSetActive(true);
                                image.gameObject.CustomSetActive(false);
                                elemenet.enabled = true;
                            }
                            else
                            {
                                component.gameObject.CustomSetActive(false);
                                image.gameObject.CustomSetActive(true);
                                elemenet.enabled = false;
                            }
                            GameObject prefab = CUIUtility.GetSpritePrefeb(CUIUtility.s_Sprite_Dynamic_Icon_Dir + StringHelper.UTF8BytesToString(ref item.szSkinPicID), true, true);
                            component.SetSprite(prefab);
                            image.SetSprite(prefab);
                            text.text = StringHelper.UTF8BytesToString(ref item.szSkinName);
                            CUIEventScript script4 = transform3.GetComponent<CUIEventScript>();
                            stUIEventParams eventParams = new stUIEventParams {
                                tagUInt = item.dwSkinID,
                                commonBool = bActive
                            };
                            script4.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSelect_SkinSelect, eventParams);
                            if (j == index)
                            {
                                this.InitSkinEffect(script2.transform.Find("panelEffect/List").gameObject, showHeroID, item.dwSkinID);
                            }
                        }
                        script2.SelectElement(index, true);
                    }
                    if (index == -1)
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        transform2.gameObject.CustomSetActive(true);
                    }
                }
            }
        }

        public void ResetHero3DObj()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (((form != null) && (form.gameObject != null)) && (this.m_nowShowHeroID != 0))
            {
                GameObject gameObject = form.gameObject.transform.Find("PanelCenter/3DImage").gameObject.GetComponent<CUI3DImageScript>().GetGameObject(this.m_heroGameObjName);
                if (gameObject != null)
                {
                    CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                    instance.Set3DModel(gameObject);
                    instance.InitAnimatList();
                    uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_nowShowHeroID);
                    instance.InitAnimatSoundList(this.m_nowShowHeroID, heroWearSkinId);
                }
            }
        }

        public static void SetPageDropListDataByHeroSelect(GameObject panelObj, int selectIndex)
        {
            if (panelObj != null)
            {
                CUIListScript component = panelObj.transform.Find("DropList/List").GetComponent<CUIListScript>();
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                component.SetElementAmount(masterRoleInfo.m_symbolInfo.m_pageCount);
                for (int i = 0; i < masterRoleInfo.m_symbolInfo.m_pageCount; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    elemenet.gameObject.transform.Find("Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageName(i);
                    elemenet.gameObject.transform.Find("SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(i).ToString();
                }
                component.SelectElement(selectIndex, true);
                panelObj.transform.Find("DropList/Button_Down/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageName(selectIndex);
                panelObj.transform.Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString();
                panelObj.transform.Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString();
            }
        }

        public void StartEndTimer(int totlaTimes)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find("CountDown/Timer");
                if (transform != null)
                {
                    CUITimerScript component = transform.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.SetTotalTime((float) totlaTimes);
                        component.m_timerType = Assets.Scripts.UI.enTimerType.CountDown;
                        component.ReStartTimer();
                        component.gameObject.CustomSetActive(true);
                    }
                }
            }
        }

        public void SwitchSkinMenuSelect()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUIListScript component = form.gameObject.transform.Find("TabList").GetComponent<CUIListScript>();
                component.m_alwaysDispatchSelectedChangeEvent = true;
                component.SelectElement(1, true);
                component.m_alwaysDispatchSelectedChangeEvent = false;
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshHeroInfo_RightPanel>c__AnonStorey76
        {
            internal CHeroSelectNormalSystem.<RefreshHeroInfo_RightPanel>c__AnonStorey77 <>f__ref$119;
            internal CHeroSelectNormalSystem.<RefreshHeroInfo_RightPanel>c__AnonStorey78 <>f__ref$120;
            internal uint selSkillID;

            internal void <>m__5F(ResSkillUnlock rule)
            {
                if ((rule != null) && (rule.dwUnlockSkillID == this.selSkillID))
                {
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(this.selSkillID);
                    if (dataByKey != null)
                    {
                        this.<>f__ref$119.selSkillCell.CustomSetActive(true);
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                        this.<>f__ref$119.selSkillCell.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, this.<>f__ref$120.form, true, false, false);
                    }
                    else
                    {
                        DebugHelper.Assert(false, string.Format("SelSkill ResSkillCfgInfo[{0}] can not be find!!", this.selSkillID));
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshHeroInfo_RightPanel>c__AnonStorey77
        {
            internal GameObject selSkillCell;
        }

        [CompilerGenerated]
        private sealed class <RefreshHeroInfo_RightPanel>c__AnonStorey78
        {
            internal CUIFormScript form;
        }
    }
}

