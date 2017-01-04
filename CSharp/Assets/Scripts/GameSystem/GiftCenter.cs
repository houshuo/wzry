namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class GiftCenter
    {
        private CMallSortHelper.HeroSortType _curFilterHeroSortType;
        private enHeroJobType _curFilterJob;
        private MenuPage _curFilterMenuPage = MenuPage.Skin;
        private CMallSortHelper.SkinSortType _curFilterSkinSortType;
        private bool _curFriendIsSns;
        private ulong _curFriendUid;
        private CMallSortHelper.HeroSortType _curHeroSortType;
        private enHeroJobType _curJobView;
        private MenuPage _curMenuPage;
        private CMallSortHelper.SkinSortType _curSkinSortType;
        private uint _curWorldId;
        private GameObject _fakeLoading;
        private List<int> _filterTempList = new List<int>();
        private CUIFormScript _giftCenterForm;
        private ListView<ResHeroCfgInfo> _heroList = new ListView<ResHeroCfgInfo>();
        private int _heroTotalNum;
        private CUIListScript _itemList;
        private CUIListScript _menuList;
        private stPayInfoSet _payInfoTemp;
        private ListView<ResHeroSkin> _skinList = new ListView<ResHeroSkin>();
        private int _skinTotalNum;
        private CUIListScript _sortList;
        private GameObject _sortTitle;
        private CUIListScript _subMenuList;
        private int _tempListNum;

        private void CleanSortHeroList()
        {
            CMallSortHelper.CreateHeroSorter().SetSortType(this._curHeroSortType);
            if (this._heroList.Count > this._heroTotalNum)
            {
                this._heroList.RemoveRange(this._heroTotalNum + 1, this._heroList.Count - this._heroTotalNum);
            }
            this._heroList.Sort(CMallSortHelper.CreateHeroSorter());
            if (CMallSortHelper.CreateHeroSorter().IsDesc())
            {
                this._heroList.Reverse();
            }
        }

        private void CleanSortSkinList()
        {
            CMallSortHelper.CreateSkinSorter().SetSortType(this._curSkinSortType);
            if (this._skinList.Count > this._skinTotalNum)
            {
                this._skinList.RemoveRange(this._skinTotalNum + 1, this._skinList.Count - this._skinTotalNum);
            }
            this._skinList.Sort(CMallSortHelper.CreateSkinSorter());
            if (CMallSortHelper.CreateSkinSorter().IsDesc())
            {
                this._skinList.Reverse();
            }
        }

        private void DataClearWhenClose()
        {
            this._filterTempList.Clear();
            this._tempListNum = 0;
        }

        private string GetJobName(enHeroJobType job)
        {
            switch (job)
            {
                case enHeroJobType.All:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");

                case enHeroJobType.Tank:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");

                case enHeroJobType.Soldier:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");

                case enHeroJobType.Assassin:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");

                case enHeroJobType.Master:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");

                case enHeroJobType.Archer:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");

                case enHeroJobType.Aid:
                    return Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
            }
            return null;
        }

        public CMallItem.OldPriceType GetOldPriceType()
        {
            switch (this._payInfoTemp.m_payInfoCount)
            {
                case 0:
                    return CMallItem.OldPriceType.None;

                case 1:
                    if (this._payInfoTemp.m_payInfos[0].m_oriValue == this._payInfoTemp.m_payInfos[0].m_payValue)
                    {
                        return CMallItem.OldPriceType.None;
                    }
                    return CMallItem.OldPriceType.FirstOne;

                case 2:
                    if ((this._payInfoTemp.m_payInfos[0].m_oriValue == this._payInfoTemp.m_payInfos[0].m_payValue) || (this._payInfoTemp.m_payInfos[1].m_oriValue == this._payInfoTemp.m_payInfos[1].m_payValue))
                    {
                        if (this._payInfoTemp.m_payInfos[0].m_oriValue != this._payInfoTemp.m_payInfos[0].m_payValue)
                        {
                            return CMallItem.OldPriceType.FirstOne;
                        }
                        if (this._payInfoTemp.m_payInfos[1].m_oriValue != this._payInfoTemp.m_payInfos[1].m_payValue)
                        {
                            return CMallItem.OldPriceType.SecondOne;
                        }
                        return CMallItem.OldPriceType.None;
                    }
                    return CMallItem.OldPriceType.Both;
            }
            return CMallItem.OldPriceType.None;
        }

        public void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftCenter_Open, new CUIEventManager.OnUIEventHandler(this.OnOpenGiftCenter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftCenter_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseGiftCenter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftFadeInAnim_End, new CUIEventManager.OnUIEventHandler(this.OnFadeInAnimEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_OnGiftMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnMenuChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_OnGiftSubMenueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubMenuChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftEnable, new CUIEventManager.OnUIEventHandler(this.OnItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortListClicked));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftSortChange, new CUIEventManager.OnUIEventHandler(this.OnSortChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftShowDetail, new CUIEventManager.OnUIEventHandler(this.OnShowDetail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftGive, new CUIEventManager.OnUIEventHandler(this.OnGiveFriend));
        }

        private void InitAllWidgets()
        {
            this._itemList = this._giftCenterForm.m_formWidgets[2].GetComponent<CUIListScript>();
            this._menuList = this._giftCenterForm.m_formWidgets[0].GetComponent<CUIListScript>();
            this._subMenuList = this._giftCenterForm.m_formWidgets[1].GetComponent<CUIListScript>();
            this._fakeLoading = this._giftCenterForm.m_formWidgets[3];
            this._fakeLoading.CustomSetActive(false);
            this._sortList = this._giftCenterForm.m_formWidgets[5].GetComponent<CUIListScript>();
            this._sortTitle = this._giftCenterForm.m_formWidgets[4];
        }

        private void InitMenuAndSubMenu()
        {
            if ((this._heroTotalNum > 0) && (this._skinTotalNum > 0))
            {
                this._menuList.SetElementAmount(2);
                this._curMenuPage = MenuPage.Hero;
                CUIListElementScript elemenet = this._menuList.GetElemenet(1);
                if (elemenet != null)
                {
                    elemenet.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
                }
                elemenet = this._menuList.GetElemenet(0);
                if (elemenet != null)
                {
                    elemenet.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
                }
            }
            else if (this._heroTotalNum > 0)
            {
                this._menuList.SetElementAmount(1);
                this._curMenuPage = MenuPage.Hero;
                CUIListElementScript script2 = this._menuList.GetElemenet(0);
                if (script2 != null)
                {
                    script2.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
                }
            }
            else
            {
                this._menuList.SetElementAmount(1);
                this._curMenuPage = MenuPage.Skin;
                CUIListElementScript script3 = this._menuList.GetElemenet(0);
                if (script3 != null)
                {
                    script3.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
                }
            }
            this._menuList.SelectElement(0, false);
            this._subMenuList.SetElementAmount(7);
            for (int i = 0; i < 7; i++)
            {
                CUIListElementScript script4 = this._subMenuList.GetElemenet(i);
                if (script4 != null)
                {
                    script4.gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>().text = this.GetJobName((enHeroJobType) i);
                }
            }
            this._subMenuList.SelectElement(0, false);
            this._curJobView = enHeroJobType.All;
            this.SetSortContent();
        }

        private void OnCloseGiftCenter(CUIEvent uiEvent)
        {
            if (this._giftCenterForm != null)
            {
                this.UnInitAllWidgets();
                this._giftCenterForm = null;
                this.DataClearWhenClose();
            }
        }

        private void OnFadeInAnimEnd(CUIEvent uiEvent)
        {
            this.UpdateItemList();
            this._fakeLoading.CustomSetActive(false);
        }

        private void OnGiveFriend(CUIEvent euiEvent)
        {
        }

        private void OnItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((this._curJobView != enHeroJobType.All) && (srcWidgetIndexInBelongedList < this._filterTempList.Count))
            {
                srcWidgetIndexInBelongedList = this._filterTempList[srcWidgetIndexInBelongedList];
            }
            if ((this._curMenuPage == MenuPage.Skin) && (srcWidgetIndexInBelongedList < this._skinList.Count))
            {
                ResHeroSkin skinInfo = this._skinList[srcWidgetIndexInBelongedList];
                CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
                this.SetSkinItem(component, skinInfo, uiEvent.m_srcFormScript);
            }
            else if (srcWidgetIndexInBelongedList < this._heroList.Count)
            {
                ResHeroCfgInfo heroInfo = this._heroList[srcWidgetIndexInBelongedList];
                CMallItemWidget mallWidget = srcWidget.GetComponent<CMallItemWidget>();
                this.SetHeroItem(mallWidget, heroInfo, uiEvent.m_srcFormScript);
            }
        }

        private void OnMenuChanged(CUIEvent uiEvent)
        {
            if (this._menuList != null)
            {
                int selectedIndex = this._menuList.GetSelectedIndex();
                this._curMenuPage = (MenuPage) selectedIndex;
                this._curSkinSortType = CMallSortHelper.SkinSortType.Default;
                this._curHeroSortType = CMallSortHelper.HeroSortType.Default;
                this.SetSortContent();
                if (this._curJobView != enHeroJobType.All)
                {
                    this.RealIndexGetReady(false);
                }
                this.UpdateItemList();
            }
        }

        private void OnOpenGiftCenter(CUIEvent uiEvent)
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= 5)
            {
                Singleton<CMiShuSystem>.GetInstance().HideNewFlag(uiEvent.m_srcWidget, enNewFlagKey.New_GiftCenterBtn_V1);
            }
            this.OpenGiftCenter(0L, 0, false);
        }

        private void OnShowDetail(CUIEvent euiEvent)
        {
            uint heroId = euiEvent.m_eventParams.openHeroFormPar.heroId;
            uint skinId = euiEvent.m_eventParams.openHeroFormPar.skinId;
            uint price = euiEvent.m_eventParams.commonUInt32Param1;
            this.OnShowGiveFriendSkin(heroId, skinId, price);
        }

        private void OnShowGiveFriendSkin(uint heroId, uint skinId, uint price)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/System/", "Mall/Form_GiveHeroSkin_3D.prefab"), false, true);
                script.transform.Find("Panel/skinNameText").GetComponent<Text>().text = heroSkin.szSkinName;
                GameObject gameObject = script.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
                CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                script.transform.Find("Panel/pricePanel/priceText").GetComponent<Text>().text = price.ToString();
                CUIEventScript component = script.transform.Find("Panel/BtnGroup/buyButton").gameObject.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.heroSkinParam.heroId = heroId;
                eventParams.heroSkinParam.skinId = skinId;
                eventParams.heroSkinParam.isCanCharge = true;
                eventParams.commonUInt64Param1 = this._curFriendUid;
                eventParams.commonBool = this._curFriendIsSns;
                eventParams.commonUInt32Param1 = this._curWorldId;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, eventParams);
                CUI3DImageScript script3 = script.transform.Find("Panel/3DImage").gameObject.GetComponent<CUI3DImageScript>();
                ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroId, (int) skinId, true);
                GameObject model = script3.AddGameObject(data.ObjectName, false, false);
                if (model != null)
                {
                    if (data.ActorInfo != null)
                    {
                        model.transform.localScale = new Vector3(data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale);
                    }
                    DynamicShadow.EnableDynamicShow(script3.gameObject, true);
                    CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                    instance.Set3DModel(model);
                    instance.InitAnimatList();
                    instance.InitAnimatSoundList(heroId, skinId);
                    instance.OnModePlayAnima("Come");
                }
            }
        }

        private void OnSortChanged(CUIEvent uiEvent)
        {
            if (this._sortList != null)
            {
                int selectedIndex = this._sortList.GetSelectedIndex();
                this.SetCurSortType();
                this._sortList.gameObject.CustomSetActive(false);
                this.SetCurSortTitleName();
                this.RealIndexGetReady(true);
                this.UpdateItemList();
            }
        }

        private void OnSortListClicked(CUIEvent euiEvent)
        {
            this._sortList.gameObject.CustomSetActive(!this._sortList.gameObject.activeSelf);
        }

        private void OnSubMenuChanged(CUIEvent uiEvent)
        {
            if (this._subMenuList != null)
            {
                int selectedIndex = this._subMenuList.GetSelectedIndex();
                this._curJobView = (enHeroJobType) selectedIndex;
                if (this._curJobView != enHeroJobType.All)
                {
                    this.RealIndexGetReady(false);
                }
                this.UpdateItemList();
            }
        }

        public void OpenGiftCenter(ulong uId = 0, uint worldId = 0, bool isSns = false)
        {
            if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel < 5)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Gift_LevelLimit"), false, 1.5f, null, new object[0]);
            }
            else if (this._giftCenterForm == null)
            {
                this._curFriendUid = uId;
                this._curWorldId = worldId;
                this._curFriendIsSns = isSns;
                this._giftCenterForm = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/System/", "Mall/Form_GiftCenter.prefab"), false, true);
                if (this._giftCenterForm != null)
                {
                    this.InitAllWidgets();
                    this.PrepareData();
                    this.InitMenuAndSubMenu();
                }
            }
        }

        private void PrepareData()
        {
            GameDataMgr.GetAllHeroList(ref this._heroList, out this._heroTotalNum, enHeroJobType.All, true, true);
            GameDataMgr.GetAllSkinList(ref this._skinList, out this._skinTotalNum, enHeroJobType.All, false, true, true);
        }

        private void RealIndexGetReady(bool forceUpdate = false)
        {
            if (((forceUpdate || (this._curFilterJob != this._curJobView)) || (this._curFilterMenuPage != this._curMenuPage)) || (((this._curFilterMenuPage != MenuPage.Hero) || (this._curFilterHeroSortType != this._curHeroSortType)) && ((this._curFilterMenuPage != MenuPage.Skin) || (this._curFilterSkinSortType != this._curSkinSortType))))
            {
                this._curFilterJob = this._curJobView;
                this._curFilterMenuPage = this._curMenuPage;
                int num = 0;
                if (this._curMenuPage == MenuPage.Skin)
                {
                    this._curFilterSkinSortType = this._curSkinSortType;
                    this.CleanSortSkinList();
                    for (int i = 0; i < this._skinTotalNum; i++)
                    {
                        ResHeroSkin skin = this._skinList[i];
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(skin.dwHeroID);
                        if ((dataByKey.bMainJob == ((byte) this._curJobView)) || (dataByKey.bMinorJob == ((byte) this._curJobView)))
                        {
                            if (num < this._filterTempList.Count)
                            {
                                this._filterTempList[num] = i;
                            }
                            else
                            {
                                this._filterTempList.Add(i);
                            }
                            num++;
                        }
                    }
                    this._tempListNum = num;
                }
                else
                {
                    this._curFilterHeroSortType = this._curHeroSortType;
                    this.CleanSortHeroList();
                    for (int j = 0; j < this._heroTotalNum; j++)
                    {
                        ResHeroCfgInfo info2 = this._heroList[j];
                        if ((info2.bMainJob == ((byte) this._curJobView)) || (info2.bMinorJob == ((byte) this._curJobView)))
                        {
                            if (num < this._filterTempList.Count)
                            {
                                this._filterTempList[num] = j;
                            }
                            else
                            {
                                this._filterTempList.Add(j);
                            }
                            num++;
                        }
                    }
                    this._tempListNum = num;
                }
            }
        }

        private void SetCurSortTitleName()
        {
            if (this._curMenuPage == MenuPage.Hero)
            {
                this._sortTitle.GetComponent<Text>().text = CMallSortHelper.CreateHeroSorter().GetSortTypeName(this._curHeroSortType);
            }
            else
            {
                this._sortTitle.GetComponent<Text>().text = CMallSortHelper.CreateSkinSorter().GetSortTypeName(this._curSkinSortType);
            }
        }

        private void SetCurSortType()
        {
            if (this._sortList.GetSelectedIndex() == 3)
            {
                this._curHeroSortType = CMallSortHelper.HeroSortType.ReleaseTime;
                this._curSkinSortType = CMallSortHelper.SkinSortType.ReleaseTime;
            }
            else
            {
                this._curHeroSortType = (CMallSortHelper.HeroSortType) this._sortList.GetSelectedIndex();
                this._curSkinSortType = (CMallSortHelper.SkinSortType) this._sortList.GetSelectedIndex();
            }
        }

        private void SetHeroItem(CMallItemWidget mallWidget, ResHeroCfgInfo heroInfo, CUIFormScript form)
        {
            Image component = mallWidget.m_icon.GetComponent<Image>();
            component.color = CUIUtility.s_Color_White;
            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, heroInfo.szImagePath);
            component.SetSprite(prefabPath, form, false, true, true);
            mallWidget.m_skinLabel.CustomSetActive(false);
            mallWidget.m_topNameLeftText.GetComponent<Text>().text = heroInfo.szName;
            mallWidget.m_topNameRightText.CustomSetActive(false);
            IHeroData data = CHeroDataFactory.CreateHeroData(heroInfo.dwCfgID);
            if (data != null)
            {
                ResHeroPromotion resPromotion = data.promotion();
                this._payInfoTemp = CMallSystem.GetPayInfoSetOfGood(heroInfo, resPromotion);
                uint num = this.SetItemPriceInfo(mallWidget, ref this._payInfoTemp);
                this.SetItemTag(mallWidget, resPromotion, null, form);
                stUIEventParams eventParams = new stUIEventParams {
                    heroId = heroInfo.dwCfgID,
                    commonUInt64Param1 = this._curFriendUid,
                    commonBool = this._curFriendIsSns,
                    commonUInt32Param1 = this._curWorldId
                };
                mallWidget.m_buyBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_OpenBuyHeroForFriend, eventParams);
                mallWidget.m_item.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.None, eventParams);
            }
        }

        public uint SetItemPriceInfo(CMallItemWidget itemWidget, ref stPayInfoSet payInfoSet)
        {
            uint payValue = 0;
            if (itemWidget.m_priceContainer != null)
            {
                itemWidget.m_priceContainer.SetActive(true);
                CMallItem.OldPriceType oldPriceType = this.GetOldPriceType();
                CUIListScript component = itemWidget.m_priceContainer.GetComponent<CUIListScript>();
                component.SetElementAmount(1);
                itemWidget.m_orTextContainer.CustomSetActive(false);
                CUIListElementScript elemenet = component.GetElemenet(0);
                if (elemenet == null)
                {
                    return payValue;
                }
                GameObject widget = elemenet.GetWidget(0);
                GameObject obj3 = elemenet.GetWidget(1);
                GameObject obj4 = elemenet.GetWidget(2);
                GameObject obj5 = elemenet.GetWidget(4);
                GameObject obj6 = elemenet.GetWidget(3);
                GameObject obj7 = elemenet.GetWidget(5);
                if ((((widget == null) || (obj3 == null)) || ((obj4 == null) || (obj5 == null))) || ((obj6 == null) || (obj7 == null)))
                {
                    return payValue;
                }
                for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
                {
                    if ((payInfoSet.m_payInfos[i].m_payType == enPayType.Diamond) || (payInfoSet.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                    {
                        payInfoSet.m_payInfos[i].m_payType = enPayType.DianQuan;
                    }
                }
                for (int j = 0; j < payInfoSet.m_payInfoCount; j++)
                {
                    if (((payInfoSet.m_payInfos[j].m_payType != enPayType.DianQuan) && (payInfoSet.m_payInfos[j].m_payType != enPayType.Diamond)) && (payInfoSet.m_payInfos[j].m_payType != enPayType.DiamondAndDianQuan))
                    {
                        continue;
                    }
                    payValue = payInfoSet.m_payInfos[j].m_payValue;
                    switch (oldPriceType)
                    {
                        case CMallItem.OldPriceType.None:
                            itemWidget.m_middleOrText.CustomSetActive(true);
                            itemWidget.m_bottomOrText.CustomSetActive(false);
                            widget.SetActive(false);
                            obj3.SetActive(false);
                            obj4.SetActive(false);
                            obj6.SetActive(true);
                            obj6.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                            obj7.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                            return payValue;

                        case CMallItem.OldPriceType.FirstOne:
                            itemWidget.m_middleOrText.CustomSetActive(false);
                            itemWidget.m_bottomOrText.CustomSetActive(true);
                            if (j != 0)
                            {
                                break;
                            }
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
                            obj4.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                            obj5.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                            return payValue;

                        case CMallItem.OldPriceType.SecondOne:
                            itemWidget.m_middleOrText.CustomSetActive(false);
                            itemWidget.m_bottomOrText.CustomSetActive(true);
                            if (j != 1)
                            {
                                goto Label_045D;
                            }
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
                            obj4.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                            obj5.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                            return payValue;

                        case CMallItem.OldPriceType.Both:
                            itemWidget.m_middleOrText.CustomSetActive(true);
                            itemWidget.m_bottomOrText.CustomSetActive(false);
                            obj3.SetActive(false);
                            obj6.SetActive(false);
                            widget.SetActive(true);
                            obj4.SetActive(true);
                            widget.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_oriValue.ToString();
                            obj4.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                            obj5.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                            return payValue;

                        default:
                            return payValue;
                    }
                    obj3.SetActive(false);
                    widget.SetActive(false);
                    obj6.SetActive(false);
                    obj4.SetActive(true);
                    obj4.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                    obj5.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                    return payValue;
                Label_045D:
                    obj3.SetActive(false);
                    widget.SetActive(false);
                    obj6.SetActive(false);
                    obj4.SetActive(true);
                    obj4.GetComponent<Text>().text = payInfoSet.m_payInfos[j].m_payValue.ToString();
                    obj5.GetComponent<Image>().SetSprite(CMallSystem.GetPayTypeIconPath(payInfoSet.m_payInfos[j].m_payType), this._giftCenterForm, true, false, false);
                    return payValue;
                }
            }
            return payValue;
        }

        private void SetItemTag(CMallItemWidget itemWidget, ResHeroPromotion heroPromotion, ResSkinPromotion skinPromotion, CUIFormScript form)
        {
            string str = null;
            string text = null;
            int num2;
            RES_LUCKYDRAW_ITEMTAG bTag = RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
            if (heroPromotion != null)
            {
                bTag = (RES_LUCKYDRAW_ITEMTAG) heroPromotion.bTag;
            }
            else if (skinPromotion != null)
            {
                bTag = (RES_LUCKYDRAW_ITEMTAG) skinPromotion.bTag;
            }
            switch (bTag)
            {
                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
                {
                    num2 = 0;
                    uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                    if (heroPromotion == null)
                    {
                        if (skinPromotion != null)
                        {
                            if (skinPromotion.dwOnTimeGen > currentUTCTime)
                            {
                                num2 = (int) (skinPromotion.dwOffTimeGen - skinPromotion.dwOnTimeGen);
                            }
                            else
                            {
                                num2 = (int) (skinPromotion.dwOffTimeGen - currentUTCTime);
                            }
                        }
                        break;
                    }
                    if (heroPromotion.dwOnTimeGen <= currentUTCTime)
                    {
                        num2 = (int) (heroPromotion.dwOffTimeGen - currentUTCTime);
                        break;
                    }
                    num2 = (int) (heroPromotion.dwOffTimeGen - heroPromotion.dwOnTimeGen);
                    break;
                }
                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
                    str = "UGUI/Sprite/Common/Product_New.prefab";
                    text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
                    goto Label_01D8;

                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
                    str = "UGUI/Sprite/Common/Product_Hot.prefab";
                    text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
                    goto Label_01D8;

                case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
                {
                    float num = 100f;
                    if (heroPromotion != null)
                    {
                        num = ((float) heroPromotion.dwDiscount) / 10f;
                    }
                    else if (skinPromotion != null)
                    {
                        num = ((float) skinPromotion.dwDiscount) / 10f;
                    }
                    str = "UGUI/Sprite/Common/Product_Discount.prefab";
                    if (Math.Abs((float) (num % 1f)) < float.Epsilon)
                    {
                        text = string.Format("{0}折", ((int) num).ToString("D"));
                    }
                    else
                    {
                        text = string.Format("{0}折", num.ToString("0.0"));
                    }
                    goto Label_01D8;
                }
                default:
                    goto Label_01D8;
            }
            if (num2 > 0)
            {
                int num4 = (int) Math.Ceiling(((double) num2) / 86400.0);
                if (num4 > 0)
                {
                    str = "UGUI/Sprite/Common/Product_Unusual.prefab";
                    string[] args = new string[] { num4.ToString() };
                    text = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", args);
                }
            }
        Label_01D8:
            if ((itemWidget.m_tagContainer != null) && (!string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(text)))
            {
                itemWidget.m_tagContainer.SetActive(true);
                itemWidget.m_tagContainer.GetComponent<Image>().SetSprite(str, form, false, true, true);
                if (itemWidget.m_tagText != null)
                {
                    itemWidget.m_tagText.GetComponent<Text>().text = text;
                }
            }
            else
            {
                itemWidget.m_tagContainer.CustomSetActive(false);
            }
        }

        private void SetSelectedElementInSortList()
        {
            if ((this._curMenuPage == MenuPage.Hero) && (this._curHeroSortType == CMallSortHelper.HeroSortType.ReleaseTime))
            {
                this._sortList.SelectElement(3, true);
            }
            else if (this._curMenuPage == MenuPage.Hero)
            {
                this._sortList.SelectElement((int) this._curHeroSortType, true);
            }
            else
            {
                this._sortList.SelectElement((int) this._curSkinSortType, true);
            }
        }

        private void SetSkinItem(CMallItemWidget mallWidget, ResHeroSkin skinInfo, CUIFormScript form)
        {
            Image component = mallWidget.m_icon.GetComponent<Image>();
            component.color = CUIUtility.s_Color_White;
            string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, skinInfo.szSkinPicID);
            component.SetSprite(prefabPath, form, false, true, true);
            mallWidget.m_skinLabel.CustomSetActive(true);
            CUICommonSystem.SetHeroSkinLabelPic(form, mallWidget.m_skinLabel, skinInfo.dwHeroID, skinInfo.dwSkinID);
            mallWidget.m_topNameLeftText.GetComponent<Text>().text = skinInfo.szHeroName;
            mallWidget.m_topNameRightText.CustomSetActive(true);
            mallWidget.m_topNameRightText.GetComponent<Text>().text = skinInfo.szSkinName;
            ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(skinInfo.dwHeroID, skinInfo.dwSkinID);
            this._payInfoTemp = CMallSystem.GetPayInfoSetOfGood(skinInfo, skinPromotion);
            uint num = this.SetItemPriceInfo(mallWidget, ref this._payInfoTemp);
            this.SetItemTag(mallWidget, null, skinPromotion, form);
            stUIEventParams eventParams = new stUIEventParams();
            eventParams.openHeroFormPar.heroId = skinInfo.dwHeroID;
            eventParams.openHeroFormPar.skinId = skinInfo.dwSkinID;
            eventParams.commonUInt32Param1 = num;
            mallWidget.m_item.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Mall_GiftShowDetail, eventParams);
            stUIEventParams params2 = new stUIEventParams();
            params2.heroSkinParam.heroId = skinInfo.dwHeroID;
            params2.heroSkinParam.skinId = skinInfo.dwSkinID;
            params2.heroSkinParam.isCanCharge = true;
            params2.commonUInt64Param1 = this._curFriendUid;
            params2.commonBool = this._curFriendIsSns;
            params2.commonUInt32Param1 = this._curWorldId;
            mallWidget.m_buyBtn.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, params2);
        }

        private void SetSortContent()
        {
            int amount = 0;
            switch (this._curMenuPage)
            {
                case MenuPage.Hero:
                {
                    amount = 4;
                    this._sortList.SetElementAmount(amount);
                    int index = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (i != 3)
                        {
                            Text component = this._sortList.GetElemenet(index).transform.Find("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = CMallSortHelper.CreateHeroSorter().GetSortTypeName((CMallSortHelper.HeroSortType) i);
                            }
                            index++;
                        }
                    }
                    break;
                }
                case MenuPage.Skin:
                    amount = 4;
                    this._sortList.SetElementAmount(amount);
                    for (int j = 0; j < 4; j++)
                    {
                        Text text2 = this._sortList.GetElemenet(j).transform.Find("Text").GetComponent<Text>();
                        if (text2 != null)
                        {
                            text2.text = CMallSortHelper.CreateSkinSorter().GetSortTypeName((CMallSortHelper.SkinSortType) j);
                        }
                    }
                    break;
            }
            this._sortList.m_alwaysDispatchSelectedChangeEvent = true;
            this.SetSelectedElementInSortList();
            this.SetCurSortTitleName();
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftCenter_Open, new CUIEventManager.OnUIEventHandler(this.OnOpenGiftCenter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftCenter_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseGiftCenter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftFadeInAnim_End, new CUIEventManager.OnUIEventHandler(this.OnFadeInAnimEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_OnGiftMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnMenuChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_OnGiftSubMenueChanged, new CUIEventManager.OnUIEventHandler(this.OnSubMenuChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftEnable, new CUIEventManager.OnUIEventHandler(this.OnItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortListClicked));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_GiftSortChange, new CUIEventManager.OnUIEventHandler(this.OnSortChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftShowDetail, new CUIEventManager.OnUIEventHandler(this.OnShowDetail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_GiftGive, new CUIEventManager.OnUIEventHandler(this.OnGiveFriend));
        }

        private void UnInitAllWidgets()
        {
            this._itemList = null;
            this._menuList = null;
            this._subMenuList = null;
            this._fakeLoading = null;
            this._sortList = null;
            this._sortTitle = null;
        }

        private void UpdateItemList()
        {
            if (this._curMenuPage == MenuPage.Skin)
            {
                this._itemList.SetElementAmount((this._curJobView != enHeroJobType.All) ? this._tempListNum : this._skinTotalNum);
            }
            else
            {
                this._itemList.SetElementAmount((this._curJobView != enHeroJobType.All) ? this._tempListNum : this._heroTotalNum);
            }
        }

        public enum MenuPage
        {
            Hero,
            Skin
        }

        public enum Widgets
        {
            MenuList,
            SubMenuList,
            ItemList,
            FakeLoading,
            SortTitleText,
            SortList
        }
    }
}

