namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CHeroSelectBanPickSystem : Singleton<CHeroSelectBanPickSystem>
    {
        public const int c_banHeroCountMax = 3;
        private const float c_countDownCheckTime = 6.1f;
        private ListView<IHeroData> m_banHeroList;
        private IHeroData m_selectBanHeroData;
        public static string s_heroSelectFormPath = "UGUI/Form/System/HeroSelect/Form_HeroSelectBanPick.prefab";
        public static string s_symbolPropPanelPath = "Bottom/Panel_SymbolProp";

        private void CenterHeroItemEnable(CUIEvent uiEvent)
        {
            IHeroData data;
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript srcWidgetBelongedListScript = uiEvent.m_srcWidgetBelongedListScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if (((srcFormScript != null) && (srcWidgetBelongedListScript != null)) && (((srcWidget != null) && (masterRoleInfo != null)) && (srcWidgetIndexInBelongedList >= 0)))
            {
                data = null;
                if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan) && (srcWidgetIndexInBelongedList < this.m_banHeroList.Count))
                {
                    data = this.m_banHeroList[srcWidgetIndexInBelongedList];
                    goto Label_00D2;
                }
                if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && (srcWidgetIndexInBelongedList < Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count))
                {
                    data = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList[srcWidgetIndexInBelongedList];
                    goto Label_00D2;
                }
            }
            return;
        Label_00D2:
            if (data == null)
            {
                return;
            }
            if (srcWidget.GetComponent<CUIListElementScript>() != null)
            {
                GameObject gameObject = srcWidget.transform.Find("heroItemCell").gameObject;
                GameObject obj4 = gameObject.transform.Find("TxtFree").gameObject;
                GameObject obj5 = gameObject.transform.Find("TxtCreditFree").gameObject;
                GameObject obj6 = gameObject.transform.Find("imgExperienceMark").gameObject;
                Transform targetTrans = gameObject.transform.Find("expCardPanel");
                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                CUIEventScript script5 = srcWidget.GetComponent<CUIEventScript>();
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
                obj6.CustomSetActive(false);
                targetTrans.gameObject.CustomSetActive(false);
                component.enabled = false;
                script5.enabled = false;
                if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && (Singleton<CHeroSelectBaseSystem>.instance.gameType != enSelectGameType.enLadder))
                {
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
                }
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                if ((masterMemberInfo != null) && ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan) || (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)))
                {
                    if (masterMemberInfo == memberInfo)
                    {
                        if (Singleton<CHeroSelectBaseSystem>.instance.IsBanByHeroID(data.cfgID) || Singleton<CHeroSelectBaseSystem>.instance.IsHeroExist(data.cfgID))
                        {
                            CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, true);
                        }
                        else
                        {
                            component.enabled = true;
                            script5.enabled = true;
                            CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, false);
                        }
                    }
                    else
                    {
                        CUICommonSystem.SetHeroItemData(srcFormScript, gameObject, data, enHeroHeadType.enIcon, true);
                    }
                }
            }
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroSelectFormPath);
        }

        private void HeroSelect_ConfirmHeroSelect(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
            {
                if (this.m_selectBanHeroData != null)
                {
                    CHeroSelectBaseSystem.SendBanHeroMsg(this.m_selectBanHeroData.cfgID);
                }
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
            {
                CHeroSelectBaseSystem.SendMuliPrepareToBattleMsg();
            }
        }

        private void HeroSelect_OnClose(CUIEvent uiEvent)
        {
            this.m_banHeroList = null;
            this.m_selectBanHeroData = null;
            Singleton<CHeroSelectBaseSystem>.instance.Clear();
            Singleton<CSoundManager>.GetInstance().UnLoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
        }

        private void HeroSelect_OnMenuSelect(CUIEvent uiEvent)
        {
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
            if (masterMemberInfo != null)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                    int selectedIndex = component.GetSelectedIndex();
                    if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan) && (selectedIndex == 1))
                    {
                        component.SelectElement(0, true);
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("BP_Tip_Dis_Skin"), false, 1.5f, null, new object[0]);
                    }
                    else if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap) && (selectedIndex == 0))
                    {
                        component.SelectElement(1, true);
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("BP_Tip_Dis_Hero"), false, 1.5f, null, new object[0]);
                    }
                    else if (((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && (selectedIndex == 1)) && (!Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm && (masterMemberInfo != memberInfo)))
                    {
                        component.SelectElement(0, true);
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("BP_Tip_Dis_Skin"), false, 1.5f, null, new object[0]);
                    }
                    else if (((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && (selectedIndex == 0)) && Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                    {
                        component.SelectElement(1, true);
                        Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("BP_Tip_Dis_Hero"), false, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        Transform transform = form.gameObject.transform.Find("PanelCenter/ListHostHeroInfo");
                        Transform transform2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
                        if (selectedIndex == 0)
                        {
                            transform.gameObject.CustomSetActive(true);
                            transform2.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(false);
                            transform2.gameObject.CustomSetActive(true);
                        }
                    }
                }
            }
        }

        private void HeroSelect_OnSkinSelect(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            bool commonBool = uiEvent.m_eventParams.commonBool;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
                Transform transform2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo/panelEffect/List");
                if ((transform != null) && (transform2 != null))
                {
                    MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                    if (masterMemberInfo != null)
                    {
                        CUIListScript component = transform.GetComponent<CUIListScript>();
                        if (masterRoleInfo.IsCanUseSkin(heroId, tagUInt))
                        {
                            this.InitSkinEffect(transform2.gameObject, heroId, tagUInt);
                        }
                        else
                        {
                            component.SelectElement(component.GetLastSelectedIndex(), true);
                        }
                        if (masterRoleInfo.IsCanUseSkin(heroId, tagUInt))
                        {
                            if (masterRoleInfo.GetHeroWearSkinId(heroId) != tagUInt)
                            {
                                CHeroInfoSystem2.ReqWearHeroSkin(heroId, tagUInt, true);
                            }
                        }
                        else
                        {
                            CHeroSkinBuyManager.OpenBuyHeroSkinForm3D(heroId, tagUInt, false);
                        }
                    }
                }
            }
        }

        private void HeroSelect_SelectHero(CUIEvent uiEvent)
        {
            Singleton<CSoundManager>.GetInstance().PostEvent("UI_BanPick_Swicth", null);
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
            if (masterMemberInfo != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                    {
                        if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_banHeroList.Count)) && (masterMemberInfo == memberInfo))
                        {
                            this.m_selectBanHeroData = this.m_banHeroList[srcWidgetIndexInBelongedList];
                            this.RefreshCenter();
                        }
                    }
                    else if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count)))
                    {
                        IHeroData data = Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList[srcWidgetIndexInBelongedList];
                        if (data != null)
                        {
                            if (CHeroDataFactory.IsHeroCanUse(data.cfgID) || (Singleton<CHeroSelectBaseSystem>.instance.gameType == enSelectGameType.enLadder))
                            {
                                CHeroSelectBaseSystem.SendHeroSelectMsg(0, 0, data.cfgID);
                            }
                            else
                            {
                                string[] args = new string[] { data.heroName };
                                string text = Singleton<CTextManager>.instance.GetText("ExpCard_Use", args);
                                stUIEventParams par = new stUIEventParams {
                                    tagUInt = data.cfgID
                                };
                                Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(text, enUIEventID.HeroSelect_BanPick_UseHeroExpCard, enUIEventID.HeroSelect_BanPick_UseHeroExpCardChanel, par, false);
                            }
                        }
                    }
                }
            }
        }

        private void HeroSelect_SwapHeroAllow(CUIEvent uiEvent)
        {
            CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(1);
        }

        private void HeroSelect_SwapHeroCanel(CUIEvent uiEvent)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
            {
                CHeroSelectBaseSystem.SendSwapAcceptHeroMsg(0);
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enReqing)
            {
                CHeroSelectBaseSystem.SendCanelSwapHeroMsg();
            }
        }

        private void HeroSelect_SwapHeroReq(CUIEvent uiEvent)
        {
            CHeroSelectBaseSystem.SendSwapHeroMsg(uiEvent.m_eventParams.tagUInt);
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_FormClose, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_MenuSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SkinSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_OnSkinSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SelectHero, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SelectHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_CenterHeroItemEnable, new CUIEventManager.OnUIEventHandler(this.CenterHeroItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_ConfirmHeroSelect, new CUIEventManager.OnUIEventHandler(this.HeroSelect_ConfirmHeroSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroReq, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroReq));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroAllow, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroAllow));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SwapHeroCanel, new CUIEventManager.OnUIEventHandler(this.HeroSelect_SwapHeroCanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_PageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_SymbolPageSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSymbolPageSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_ViewProp_Down, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_Symbol_ViewProp_Up, new CUIEventManager.OnUIEventHandler(this.OnCloseSymbolProp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillOpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillSelected, new CUIEventManager.OnUIEventHandler(this.OnSelectedAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmAddedSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_AddedSkillCloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseAddedSkillPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_OnTimerCountDown, new CUIEventManager.OnUIEventHandler(this.OnTimerCountDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_UseHeroExpCard, new CUIEventManager.OnUIEventHandler(this.OnUseHeroExpCard));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_BanPick_UseHeroExpCardChanel, new CUIEventManager.OnUIEventHandler(this.OnUseHeroExpCardChanel));
        }

        public void InitAddedSkillPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if ((form != null) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null))
            {
                if (CAddSkillSys.IsSelSkillAvailable())
                {
                    CUIToggleListScript component = form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>();
                    CUIListElementScript elemenet = null;
                    CUIEventScript script4 = null;
                    ResSkillUnlock dataByIndex = null;
                    ResSkillCfgInfo dataByKey = null;
                    uint key = 0;
                    ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                    component.SetElementAmount(selSkillAvailable.Count);
                    int index = 0;
                    for (int i = 0; i < selSkillAvailable.Count; i++)
                    {
                        elemenet = component.GetElemenet(i);
                        script4 = elemenet.GetComponent<CUIEventScript>();
                        dataByIndex = selSkillAvailable[i];
                        key = dataByIndex.dwUnlockSkillID;
                        dataByKey = GameDataMgr.skillDatabin.GetDataByKey(key);
                        if (dataByKey != null)
                        {
                            script4.m_onClickEventID = enUIEventID.HeroSelect_BanPick_AddedSkillSelected;
                            script4.m_onClickEventParams.tag = (int) dataByIndex.dwUnlockSkillID;
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
                }
                form.transform.Find("Bottom/AddedSkillItem").gameObject.CustomSetActive(false);
                form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
            }
        }

        public void InitBanHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
        {
            List<uint> banHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetBanHeroList(camp);
            listScript.SetElementAmount(Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount);
            IHeroData data = null;
            for (int i = 0; i < banHeroList.Count; i++)
            {
                Transform transform = listScript.GetElemenet(i).transform;
                data = CHeroDataFactory.CreateHeroData(banHeroList[i]);
                if (data != null)
                {
                    CUICommonSystem.SetObjActive(transform.transform.Find("imageIcon"), true);
                    CUICommonSystem.SetHeroItemData(listScript.m_belongedFormScript, transform.gameObject, data, enHeroHeadType.enBustCircle, false);
                }
            }
        }

        public void InitHeroList(CUIFormScript form, bool isResetSelect = false)
        {
            CUIListScript component = form.transform.Find("PanelCenter/ListHostHeroInfo").GetComponent<CUIListScript>();
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
            {
                component.SetElementAmount(this.m_banHeroList.Count);
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
            {
                component.SetElementAmount(Singleton<CHeroSelectBaseSystem>.instance.m_canUseHeroList.Count);
            }
            else
            {
                component.gameObject.CustomSetActive(false);
            }
            Button btn = form.transform.Find("PanelCenter/ListHostHeroInfo/btnConfirmSelectHero").GetComponent<Button>();
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap) && (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID))
            {
                MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                if (masterMemberInfo == null)
                {
                    return;
                }
                if (((masterMemberInfo == memberInfo) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)) && (this.m_selectBanHeroData != null))
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                }
                else if (((masterMemberInfo == memberInfo) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)) && (Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0] != 0))
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                }
                else
                {
                    CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                }
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    CUICommonSystem.SetButtonName(btn.gameObject, Singleton<CTextManager>.instance.GetText("BP_SureButton_1"));
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    CUICommonSystem.SetButtonName(btn.gameObject, Singleton<CTextManager>.instance.GetText("BP_SureButton_2"));
                }
            }
            else
            {
                CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
            }
            if (isResetSelect)
            {
                component.SelectElement(-1, true);
            }
        }

        public void InitMenu(bool isResetListSelect = false)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                GameObject gameObject = form.gameObject.transform.Find("PanelCenter/TabList").gameObject;
                string[] titleList = new string[] { Singleton<CTextManager>.instance.GetText("Choose_Hero"), Singleton<CTextManager>.instance.GetText("Choose_Skin") };
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    CUICommonSystem.InitMenuPanel(gameObject, titleList, 0);
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                    {
                        CUICommonSystem.InitMenuPanel(gameObject, titleList, 1);
                    }
                    else
                    {
                        CUICommonSystem.InitMenuPanel(gameObject, titleList, 0);
                    }
                }
                else if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap) || Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm)
                {
                    CUICommonSystem.InitMenuPanel(gameObject, titleList, 1);
                }
                if (isResetListSelect)
                {
                    this.InitHeroList(form, true);
                }
            }
        }

        private void InitSkinEffect(GameObject objList, uint heroID, uint skinID)
        {
            CSkinInfo.GetHeroSkinProp(heroID, skinID, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
            CUICommonSystem.SetListProp(objList, ref CHeroSelectBaseSystem.s_propArr, ref CHeroSelectBaseSystem.s_propPctArr);
        }

        public void InitSkinList(CUIFormScript form, uint customHeroID = 0)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                if (masterMemberInfo != null)
                {
                    uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                    if (customHeroID != 0)
                    {
                        heroId = customHeroID;
                    }
                    ListView<ResHeroSkin> view = new ListView<ResHeroSkin>();
                    ListView<ResHeroSkin> collection = new ListView<ResHeroSkin>();
                    int index = -1;
                    ResHeroSkin item = null;
                    if (heroId != 0)
                    {
                        ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(heroId);
                        for (int i = 0; i < availableSkinByHeroId.Count; i++)
                        {
                            item = availableSkinByHeroId[i];
                            if (masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) || CBagSystem.IsHaveSkinExpCard(item.dwID))
                            {
                                view.Add(item);
                            }
                            else
                            {
                                collection.Add(item);
                            }
                            if (masterRoleInfo.GetHeroWearSkinId(heroId) == item.dwSkinID)
                            {
                                index = view.Count - 1;
                            }
                        }
                        view.AddRange(collection);
                    }
                    Transform transform = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo");
                    Transform transform2 = form.gameObject.transform.Find("PanelCenter/ListHostSkinInfo/panelEffect");
                    if (transform != null)
                    {
                        CUIListScript[] scriptArray1 = new CUIListScript[] { transform.GetComponent<CUIListScript>() };
                        foreach (CUIListScript script in scriptArray1)
                        {
                            script.SetElementAmount(view.Count);
                            for (int j = 0; j < view.Count; j++)
                            {
                                CUIListElementScript elemenet = script.GetElemenet(j);
                                Transform transform3 = script.GetElemenet(j).transform;
                                Image component = transform3.Find("imageIcon").GetComponent<Image>();
                                Image image = transform3.Find("imageIconGray").GetComponent<Image>();
                                Text text = transform3.Find("lblName").GetComponent<Text>();
                                GameObject gameObject = transform3.Find("imgExperienceMark").gameObject;
                                Transform transform4 = transform3.Find("expCardPanel");
                                item = view[j];
                                bool bActive = masterRoleInfo.IsValidExperienceSkin(heroId, item.dwSkinID);
                                gameObject.CustomSetActive(bActive);
                                bool flag2 = !masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) && CBagSystem.IsHaveSkinExpCard(item.dwID);
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
                                if (masterRoleInfo.IsCanUseSkin(heroId, item.dwSkinID) || CBagSystem.IsHaveSkinExpCard(item.dwID))
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
                                CUIEventScript script3 = transform3.GetComponent<CUIEventScript>();
                                stUIEventParams eventParams = new stUIEventParams {
                                    tagUInt = item.dwSkinID,
                                    commonBool = bActive
                                };
                                script3.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSelect_BanPick_SkinSelect, eventParams);
                            }
                            script.SelectElement(index, true);
                        }
                        transform2.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void InitSystem(CUIFormScript form)
        {
            CUICommonSystem.SetObjActive(form.transform.Find("Top/Timer/CountDownMovie"), false);
            this.InitAddedSkillPanel();
            this.InitMenu(false);
            Singleton<CReplayKitSys>.GetInstance().InitReplayKit(form.transform.Find("ReplayKit"), true, true);
        }

        public void InitTeamHeroList(CUIListScript listScript, COM_PLAYERCAMP camp)
        {
            <InitTeamHeroList>c__AnonStorey73 storey = new <InitTeamHeroList>c__AnonStorey73 {
                listScript = listScript
            };
            List<uint> teamHeroList = Singleton<CHeroSelectBaseSystem>.instance.GetTeamHeroList(camp);
            storey.listScript.SetElementAmount(teamHeroList.Count);
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
            MemberInfo info3 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState.bPos);
            ListView<MemberInfo> view = null;
            MemberInfo mInfo = null;
            Transform transform = null;
            uint id = 0;
            IHeroData data = null;
            if (masterMemberInfo != null)
            {
                for (int i = 0; i < teamHeroList.Count; i++)
                {
                    <InitTeamHeroList>c__AnonStorey72 storey2 = new <InitTeamHeroList>c__AnonStorey72 {
                        <>f__ref$115 = storey
                    };
                    view = Singleton<CHeroSelectBaseSystem>.instance.roomInfo[camp];
                    mInfo = view[i];
                    id = teamHeroList[i];
                    if ((view == null) || (mInfo == null))
                    {
                        return;
                    }
                    transform = storey.listScript.GetElemenet(i).transform;
                    GameObject gameObject = transform.Find("BgState/NormalBg").gameObject;
                    GameObject obj3 = transform.Find("BgState/NextBg").gameObject;
                    GameObject obj4 = transform.Find("BgState/CurrentBg").gameObject;
                    CUITimerScript component = transform.Find("BgState/CurrentBg/Timer").GetComponent<CUITimerScript>();
                    gameObject.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    component.gameObject.CustomSetActive(false);
                    if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
                    {
                        gameObject.CustomSetActive(true);
                    }
                    else
                    {
                        if ((Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enPick))
                        {
                            return;
                        }
                        if (mInfo == memberInfo)
                        {
                            obj4.CustomSetActive(true);
                            component.gameObject.CustomSetActive(true);
                            if (!component.IsRunning())
                            {
                                component.SetTotalTime((float) (Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.dwTimeout / 0x3e8));
                                component.ReStartTimer();
                            }
                        }
                        else if (mInfo == info3)
                        {
                            obj3.CustomSetActive(true);
                            component.EndTimer();
                        }
                        else
                        {
                            gameObject.CustomSetActive(true);
                            component.EndTimer();
                        }
                    }
                    GameObject item = transform.Find("heroItemCell").gameObject;
                    Text text = item.transform.Find("lblName").gameObject.GetComponent<Text>();
                    GameObject obj6 = transform.Find("heroItemCell/readyIcon").gameObject;
                    Image image = item.transform.Find("imageIcon").gameObject.GetComponent<Image>();
                    if (id != 0)
                    {
                        data = CHeroDataFactory.CreateHeroData(id);
                        if (data != null)
                        {
                            CUICommonSystem.SetHeroItemData(storey.listScript.m_belongedFormScript, item, data, enHeroHeadType.enIcon, false);
                        }
                        image.gameObject.CustomSetActive(true);
                    }
                    if (mInfo == masterMemberInfo)
                    {
                        string[] args = new string[] { mInfo.MemberName };
                        text.text = Singleton<CTextManager>.instance.GetText("Pvp_PlayerName", args);
                    }
                    else
                    {
                        text.text = mInfo.MemberName;
                    }
                    obj6.CustomSetActive(mInfo.isPrepare);
                    CUICommonSystem.SetObjActive(item.transform.Find("VoiceIcon"), false);
                    Button button = transform.Find("ExchangeBtn").GetComponent<Button>();
                    if (masterMemberInfo.camp != camp)
                    {
                        button.gameObject.CustomSetActive(false);
                    }
                    else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enSwap)
                    {
                        button.gameObject.CustomSetActive(false);
                    }
                    else if ((Singleton<CHeroSelectBaseSystem>.instance.m_swapState != enSwapHeroState.enReqing) && (mInfo != masterMemberInfo))
                    {
                        if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(masterMemberInfo, id) && Singleton<CHeroSelectBaseSystem>.instance.roomInfo.IsHaveHeroByID(mInfo, masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID))
                        {
                            button.gameObject.CustomSetActive(true);
                            CUIEventScript script2 = button.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                script2.m_onClickEventParams.tagUInt = mInfo.dwObjId;
                            }
                        }
                        else
                        {
                            button.gameObject.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        button.gameObject.CustomSetActive(false);
                    }
                    storey2.selSkillCell = transform.Find("selSkillItemCell").gameObject;
                    storey2.selSkillID = mInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
                    if ((storey2.selSkillID != 0) && (camp == masterMemberInfo.camp))
                    {
                        GameDataMgr.addedSkiilDatabin.Accept(new Action<ResSkillUnlock>(storey2.<>m__5C));
                    }
                    else
                    {
                        storey2.selSkillCell.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void OnCloseAddedSkillPanel(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(true);
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.transform.Find("PanelAddSkill").gameObject.CustomSetActive(false);
                if (Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
                {
                    Singleton<CChatController>.instance.Set_Show_Bottom(true);
                    Singleton<CChatController>.instance.SetEntryNodeVoiceBtnShowable(true);
                }
            }
        }

        private void OnCloseSymbolProp(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(true);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                form.gameObject.transform.Find(s_symbolPropPanelPath).gameObject.gameObject.CustomSetActive(false);
            }
        }

        public void OnConfirmAddedSkill(CUIEvent uiEvent)
        {
            uint num = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            uint tag = (uint) uiEvent.m_eventParams.tag;
            if (((num == 0) || (Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill == null)) || !CAddSkillSys.IsSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill, tag))
            {
                DebugHelper.Assert(false, string.Format("CHeroSelectBanPickSystem heroID[{0}] addedSkillID[{1}]", num, tag));
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x48e);
                msg.stPkgData.stUnlockSkillSelReq.dwHeroID = num;
                msg.stPkgData.stUnlockSkillSelReq.dwSkillID = tag;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            }
            this.OnCloseAddedSkillPanel(null);
        }

        public void OnHeroSkinWearSuc(uint heroId, uint skinId)
        {
            this.RefreshCenter();
        }

        public void OnHeroSymbolPageSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint id = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            if (id != 0)
            {
                CHeroInfo info2;
                uiEvent.m_srcFormScript.gameObject.transform.Find("Bottom").Find("Panel_SymbolChange/DropList/List").gameObject.CustomSetActive(false);
                bool flag = masterRoleInfo.GetHeroInfo(id, out info2, true);
                if (flag && (selectedIndex != info2.m_selectPageIndex))
                {
                    CHeroSelectBaseSystem.SendHeroSelectSymbolPage(id, selectedIndex, false);
                }
                else if ((!flag && masterRoleInfo.IsFreeHero(id)) && (selectedIndex != masterRoleInfo.GetFreeHeroSymbolId(id)))
                {
                    CHeroSelectBaseSystem.SendHeroSelectSymbolPage(id, selectedIndex, false);
                }
            }
        }

        public void OnOpenAddedSkillPanel(CUIEvent uiEvent)
        {
            this.SetEffectNoteVisiable(false);
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
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                uint id = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                if (id != 0)
                {
                    CHeroInfo info2;
                    if (masterRoleInfo.GetHeroInfo(id, out info2, true))
                    {
                        this.OpenSymbolPropPanel(form, info2.m_selectPageIndex);
                    }
                    else if (masterRoleInfo.IsFreeHero(id))
                    {
                        int freeHeroSymbolId = masterRoleInfo.GetFreeHeroSymbolId(id);
                        this.OpenSymbolPropPanel(form, freeHeroSymbolId);
                    }
                }
            }
        }

        public void OnSelectedAddedSkill(CUIEvent uiEvent)
        {
            uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
            if (heroId != 0)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    uint tag = (uint) uiEvent.m_eventParams.tag;
                    form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) tag;
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(tag);
                    if (dataByKey != null)
                    {
                        string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, heroId);
                        form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().text = skillDescLobby;
                    }
                }
            }
        }

        public void OnSymbolPageChange()
        {
            this.RefreshSymbolPage();
        }

        public void OnSymbolPageDownBtnClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find("Bottom/Panel_SymbolChange/DropList/List");
                this.SetEffectNoteVisiable(transform.gameObject.activeSelf);
                transform.gameObject.CustomSetActive(!transform.gameObject.activeSelf);
            }
        }

        private void OnTimerCountDown(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcFormScript != null) && (uiEvent.m_srcWidget != null))
            {
                Transform transform = uiEvent.m_srcFormScript.transform.Find("Top/Timer/CountDownMovie");
                CUITimerScript component = uiEvent.m_srcWidget.GetComponent<CUITimerScript>();
                if ((component.GetCurrentTime() <= 6.1f) && !transform.gameObject.activeSelf)
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
            this.RefreshCenter();
        }

        public void OpenForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroSelectFormPath, false, true);
            if ((form != null) && (Singleton<CHeroSelectBaseSystem>.instance.roomInfo != null))
            {
                this.m_banHeroList = CHeroDataFactory.GetBanHeroList();
                this.InitSystem(form);
                this.RefreshAll();
                Singleton<CSoundManager>.GetInstance().LoadBank("Music_BanPick", CSoundManager.BankType.Lobby);
            }
        }

        private void OpenSymbolPropPanel(CUIFormScript form, int pageIndex)
        {
            this.SetEffectNoteVisiable(false);
            GameObject gameObject = form.transform.Find(s_symbolPropPanelPath).gameObject;
            CSymbolSystem.RefreshSymbolPageProp(gameObject.gameObject.transform.Find("basePropPanel").gameObject.transform.Find("List").gameObject, pageIndex, true);
            gameObject.gameObject.CustomSetActive(true);
        }

        public void PlayCurrentBgAnimation()
        {
            if (Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath) != null)
            {
                MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                if (memberInfo != null)
                {
                    Transform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(memberInfo.ullUid, memberInfo.camp);
                    if (teamPlayerElement != null)
                    {
                        CUICommonSystem.PlayAnimation(teamPlayerElement.Find("BgState/CurrentBg"), null);
                    }
                }
            }
        }

        public void PlayStepTitleAnimation()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUICommonSystem.PlayAnimation(form.transform.Find("Top/Tips"), null);
            }
        }

        public void RefreshAddedSkillItem()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                GameObject gameObject = form.transform.Find("Bottom/AddedSkillItem").gameObject;
                gameObject.CustomSetActive(false);
                uint heroId = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if ((CAddSkillSys.IsSelSkillAvailable() && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)) && ((heroId != 0) && (masterMemberInfo != null)))
                {
                    uint dwSelSkillID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.stSkill.dwSelSkillID;
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(dwSelSkillID);
                    bool flag = true;
                    if (dataByKey == null)
                    {
                        DebugHelper.Assert(false, string.Format("ResSkillCfgInfo[{0}] can not be found!", dwSelSkillID));
                    }
                    else
                    {
                        gameObject.CustomSetActive(true);
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                        gameObject.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, form, true, false, false);
                        string skillDescLobby = CUICommonSystem.GetSkillDescLobby(dataByKey.szSkillDesc, heroId);
                        if (flag)
                        {
                            form.transform.Find("PanelAddSkill/AddSkilltxt").GetComponent<Text>().text = skillDescLobby;
                            form.transform.Find("PanelAddSkill/btnConfirm").GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int) dwSelSkillID;
                            ListView<ResSkillUnlock> selSkillAvailable = CAddSkillSys.GetSelSkillAvailable(Singleton<CHeroSelectBaseSystem>.instance.m_mapUnUseSkill);
                            for (int i = 0; i < selSkillAvailable.Count; i++)
                            {
                                if (selSkillAvailable[i].dwUnlockSkillID == dwSelSkillID)
                                {
                                    form.transform.Find("PanelAddSkill/ToggleList").GetComponent<CUIToggleListScript>().SelectElement(i, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshAll()
        {
            this.RefreshTop();
            this.RefreshBottom();
            this.RefreshLeft();
            this.RefreshRight();
            this.RefreshCenter();
            this.RefreshSwapPanel();
        }

        public void RefreshBottom()
        {
            this.RefreshSymbolPage();
            this.RefreshAddedSkillItem();
        }

        public void RefreshCenter()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                this.InitHeroList(form, false);
                this.InitSkinList(form, 0);
            }
        }

        public void RefreshLeft()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUIListScript component = form.transform.Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    this.InitTeamHeroList(component, (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID) ? masterMemberInfo.camp : COM_PLAYERCAMP.COM_PLAYERCAMP_1);
                }
            }
        }

        public void RefreshRight()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    CUIListScript component = form.transform.Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
                    this.InitTeamHeroList(component, (masterMemberInfo.camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID) ? Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp) : COM_PLAYERCAMP.COM_PLAYERCAMP_2);
                }
            }
        }

        public void RefreshSwapPanel()
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enSwap)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
                if (form != null)
                {
                    Transform transform = form.transform.Find("PanelSwap/PanelSwapHero");
                    transform.gameObject.CustomSetActive(false);
                    if ((Singleton<CHeroSelectBaseSystem>.instance.m_swapState != enSwapHeroState.enIdle) && (Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo != null))
                    {
                        MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                        MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID);
                        MemberInfo info3 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID);
                        if (((masterMemberInfo != null) && (memberInfo != null)) && (info3 != null))
                        {
                            IHeroData data = CHeroDataFactory.CreateHeroData(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            IHeroData data2 = CHeroDataFactory.CreateHeroData(memberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            IHeroData data3 = CHeroDataFactory.CreateHeroData(info3.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                            if (((data != null) && (data2 != null)) && (data3 != null))
                            {
                                GameObject gameObject = transform.Find("heroItemCell1").gameObject;
                                GameObject item = transform.Find("heroItemCell2").gameObject;
                                GameObject obj4 = transform.Find("btnConfirmSwap").gameObject;
                                GameObject obj5 = transform.Find("btnConfirmSwapCanel").gameObject;
                                if (Singleton<CHeroSelectBaseSystem>.instance.m_swapState == enSwapHeroState.enSwapAllow)
                                {
                                    CUICommonSystem.SetHeroItemData(form, gameObject, data2, enHeroHeadType.enIcon, false);
                                    CUICommonSystem.SetHeroItemData(form, item, data, enHeroHeadType.enIcon, false);
                                    obj4.CustomSetActive(true);
                                    obj5.CustomSetActive(true);
                                }
                                else
                                {
                                    CUICommonSystem.SetHeroItemData(form, gameObject, data3, enHeroHeadType.enIcon, false);
                                    CUICommonSystem.SetHeroItemData(form, item, data, enHeroHeadType.enIcon, false);
                                    obj4.CustomSetActive(false);
                                    obj5.CustomSetActive(true);
                                }
                                RectTransform teamPlayerElement = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerElement(masterMemberInfo.ullUid, masterMemberInfo.camp) as RectTransform;
                                if (teamPlayerElement != null)
                                {
                                    RectTransform transform3 = transform.transform as RectTransform;
                                    transform3.anchoredPosition = new Vector2(teamPlayerElement.anchoredPosition.x + teamPlayerElement.rect.width, teamPlayerElement.anchoredPosition.y);
                                    transform.gameObject.CustomSetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshSymbolPage()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                uint key = Singleton<CHeroSelectBaseSystem>.instance.m_selectHeroIDList[0];
                Transform transform = form.gameObject.transform.Find("Bottom/Panel_SymbolChange");
                transform.gameObject.CustomSetActive(false);
                if ((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL) && (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep != enBanPickStep.enBan)) && (GameDataMgr.heroDatabin.GetDataByKey(key) != null))
                {
                    CHeroInfo info2;
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    int selectIndex = 0;
                    if (masterRoleInfo.GetHeroInfo(key, out info2, true))
                    {
                        selectIndex = info2.m_selectPageIndex;
                    }
                    else if (masterRoleInfo.IsFreeHero(key))
                    {
                        selectIndex = masterRoleInfo.GetFreeHeroSymbolId(key);
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

        public void RefreshTop()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find("Top/Timer");
                Transform transform2 = form.transform.Find("Top/Tips");
                Text component = form.transform.Find("Top/Tips/lblTitle").GetComponent<Text>();
                if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enBan)
                {
                    component.gameObject.CustomSetActive(true);
                    transform2.gameObject.CustomSetActive(true);
                    transform.gameObject.CustomSetActive(false);
                    component.text = Singleton<CTextManager>.instance.GetText("BP_Title_1");
                }
                else if (Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep == enBanPickStep.enPick)
                {
                    component.gameObject.CustomSetActive(true);
                    transform2.gameObject.CustomSetActive(true);
                    transform.gameObject.CustomSetActive(false);
                    component.text = Singleton<CTextManager>.instance.GetText("BP_Title_2");
                }
                else
                {
                    transform2.gameObject.CustomSetActive(false);
                    transform.gameObject.CustomSetActive(true);
                }
                CUIListScript listScript = form.transform.Find("Top/LeftListBan").GetComponent<CUIListScript>();
                CUIListScript script3 = form.transform.Find("Top/RightListBan").GetComponent<CUIListScript>();
                MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    COM_PLAYERCAMP camp;
                    COM_PLAYERCAMP enemyCamp;
                    if (masterMemberInfo.camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
                    {
                        camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                        enemyCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    }
                    else
                    {
                        camp = masterMemberInfo.camp;
                        enemyCamp = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetEnemyCamp(masterMemberInfo.camp);
                    }
                    this.InitBanHeroList(listScript, camp);
                    this.InitBanHeroList(script3, enemyCamp);
                }
            }
        }

        private void SetEffectNoteVisiable(bool isShow)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_heroSelectFormPath);
            if (form != null)
            {
                CUIListScript component = form.transform.Find("PanelLeft/TeamHeroInfo").GetComponent<CUIListScript>();
                CUIListScript script3 = form.transform.Find("PanelRight/TeamHeroInfo").GetComponent<CUIListScript>();
                CUIListScript[] scriptArray = new CUIListScript[] { component, script3 };
                int elementAmount = 0;
                CUIListScript script4 = null;
                for (int i = 0; i < scriptArray.Length; i++)
                {
                    script4 = scriptArray[i];
                    elementAmount = script4.GetElementAmount();
                    for (int j = 0; j < elementAmount; j++)
                    {
                        CUICommonSystem.SetObjActive(script4.GetElemenet(j).transform.Find("BgState/CurrentBg/UI_BR_effect"), isShow);
                    }
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
                Transform transform = form.transform.Find("Top/Timer/CountDown");
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

        [CompilerGenerated]
        private sealed class <InitTeamHeroList>c__AnonStorey72
        {
            internal CHeroSelectBanPickSystem.<InitTeamHeroList>c__AnonStorey73 <>f__ref$115;
            internal GameObject selSkillCell;
            internal uint selSkillID;

            internal void <>m__5C(ResSkillUnlock rule)
            {
                if ((rule != null) && (rule.dwUnlockSkillID == this.selSkillID))
                {
                    ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey(this.selSkillID);
                    if (dataByKey != null)
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(dataByKey.szIconPath));
                        this.selSkillCell.transform.Find("Icon").GetComponent<Image>().SetSprite(prefabPath, this.<>f__ref$115.listScript.m_belongedFormScript, true, false, false);
                        this.selSkillCell.CustomSetActive(true);
                    }
                    else
                    {
                        DebugHelper.Assert(false, string.Format("SelSkill ResSkillCfgInfo[{0}] can not be find!!", this.selSkillID));
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InitTeamHeroList>c__AnonStorey73
        {
            internal CUIListScript listScript;
        }
    }
}

