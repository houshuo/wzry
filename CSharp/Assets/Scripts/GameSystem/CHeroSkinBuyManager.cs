namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CHeroSkinBuyManager : Singleton<CHeroSkinBuyManager>
    {
        private ListView<COMDT_FRIEND_INFO> detailFriendList = new ListLinqView<COMDT_FRIEND_INFO>();
        private uint m_buyHeroIDForFriend;
        private uint m_buyPriceForFriend;
        private uint m_buySkinIDForFriend;
        private ListView<COMDT_FRIEND_INFO> m_friendList;
        private bool m_isBuySkinForFriend;
        public static string s_buyHeroSkin3DFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin_3D.prefab";
        public static string s_buyHeroSkinFormPath = "UGUI/Form/System/HeroInfo/Form_Buy_HeroSkin.prefab";
        public static string s_heroBuyFormPath = "UGUI/Form/System/Mall/Form_MallBuyHero.prefab";
        public static string s_heroBuyFriendPath = "UGUI/Form/System/HeroInfo/Form_BuyForFriend.prefab";

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenBuyHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBuySkinForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_Buy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyConfirm, new CUIEventManager.OnUIEventHandler(this.OnHeroSkinBuyConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHero, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_BuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ConfirmBuyHeroForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OpenBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenBuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_BuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_BuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ConfirmBuyHeroSkinForFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_SearchFriend, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_SearchFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnFriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnFriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnCloseBuySkinForm, new CUIEventManager.OnUIEventHandler(this.OnCloseBuySkinForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_OnUseSkinExpCard, new CUIEventManager.OnUIEventHandler(this.OnUseSkinExpCard));
        }

        private void InitBuyForFriendForm(CUIFormScript form, bool bSkin, uint heroId, uint skinId = 0, ulong friendUid = 0, uint worldId = 0, bool isSns = false)
        {
            Transform transform9;
            CUIEventScript script;
            uint payValue = 0;
            if (!bSkin)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                DebugHelper.Assert(dataByKey != null);
                if (dataByKey == null)
                {
                    goto Label_03F6;
                }
                form.transform.Find("Panel/Title/titleText").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Give_Title");
                GameObject gameObject = form.transform.Find("Panel/skinBgImage/skinIconImage").gameObject;
                form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), form, false, true, true);
                form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(false);
                Transform transform6 = form.transform.Find("Panel/skinPricePanel");
                Transform costIcon = transform6.Find("costImage");
                SetPayCostIcon(form, costIcon, enPayType.DianQuan);
                SetPayCostTypeText(transform6.Find("costTypeText"), enPayType.DianQuan);
                transform9 = transform6.Find("costPanel");
                if (transform9 == null)
                {
                    goto Label_03F6;
                }
                ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(heroId).promotion();
                stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
                {
                    if (((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan)) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                    {
                        payValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
                        break;
                    }
                }
            }
            else
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                DebugHelper.Assert(heroSkin != null);
                if (heroSkin != null)
                {
                    form.transform.Find("Panel/Title/titleText").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Give_Title");
                    Image image = form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
                    image.SetSprite(prefabPath, form, true, false, false);
                    form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
                    form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(true);
                    GameObject listObj = form.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
                    CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                    CUICommonSystem.SetListProp(listObj, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                    Transform transform = form.transform.Find("Panel/skinPricePanel");
                    Transform transform2 = transform.Find("costImage");
                    SetPayCostIcon(form, transform2, enPayType.DianQuan);
                    SetPayCostTypeText(transform.Find("costTypeText"), enPayType.DianQuan);
                    Transform transform4 = transform.Find("costPanel");
                    if (transform4 != null)
                    {
                        stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
                        for (int j = 0; j < skinPayInfoSet.m_payInfoCount; j++)
                        {
                            if (((skinPayInfoSet.m_payInfos[j].m_payType == enPayType.Diamond) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan)) || (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan))
                            {
                                payValue = skinPayInfoSet.m_payInfos[j].m_payValue;
                                break;
                            }
                        }
                        SetPayCurrentPrice(transform4.Find("costText"), payValue);
                    }
                }
                goto Label_03F6;
            }
            SetPayCurrentPrice(transform9.Find("costText"), payValue);
        Label_03F6:
            script = form.transform.Find("Panel/SearchFriend/Button").GetComponent<CUIEventScript>();
            script.m_onClickEventParams.friendHeroSkinPar.bSkin = bSkin;
            script.m_onClickEventParams.friendHeroSkinPar.heroId = heroId;
            script.m_onClickEventParams.friendHeroSkinPar.skinId = skinId;
            script.m_onClickEventParams.friendHeroSkinPar.price = payValue;
            Text component = form.transform.Find("Panel/TipTxt").GetComponent<Text>();
            uint[] conditionParam = Singleton<CFunctionUnlockSys>.instance.GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO, RES_UNLOCKCONDITION_TYPE.RES_UNLOCKCONDITIONTYPE_ABOVELEVEL);
            uint num4 = (conditionParam.Length <= 1) ? 1 : conditionParam[0];
            string[] args = new string[] { num4.ToString() };
            component.text = Singleton<CTextManager>.GetInstance().GetText("Buy_For_Friend_Tip", args);
            ListView<COMDT_FRIEND_INFO> allFriend = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend();
            CUIListScript list = form.transform.Find("Panel/List").GetComponent<CUIListScript>();
            this.UpdateFriendList(ref allFriend, ref list, bSkin, heroId, skinId, payValue, friendUid, worldId, isSns);
        }

        [MessageHandler(0x71a)]
        public static void OnBuyHero(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stBuyHeroRsp.iResult == 0)
            {
                DebugHelper.Assert(GameDataMgr.heroDatabin.GetDataByKey(msg.stPkgData.stBuyHeroRsp.dwHeroID) != null);
                Singleton<CHeroInfoSystem2>.GetInstance().OnNtyAddHero(msg.stPkgData.stBuyHeroRsp.dwHeroID);
                CUICommonSystem.ShowNewHeroOrSkin(msg.stPkgData.stBuyHeroRsp.dwHeroID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, 0);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x71a, msg.stPkgData.stBuyHeroRsp.iResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x727)]
        public static void OnBuyHeroForFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stPresentHeroRsp.iResult != 0)
            {
                string strContent = Utility.ProtErrCodeToStr(0x729, msg.stPkgData.stPresentHeroRsp.iResult);
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x71c)]
        public static void OnBuyHeroSkinRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_BUYHEROSKIN_RSP stBuyHeroSkinRsp = msg.stPkgData.stBuyHeroSkinRsp;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "Master Role Info Is Null");
            if (masterRoleInfo != null)
            {
                if (stBuyHeroSkinRsp.iResult == 0)
                {
                    masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
                    Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
                    CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                }
                else
                {
                    CS_ADDHEROSKIN_ERRCODE iResult = (CS_ADDHEROSKIN_ERRCODE) stBuyHeroSkinRsp.iResult;
                    CTextManager instance = Singleton<CTextManager>.GetInstance();
                    switch (iResult)
                    {
                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKININVALID:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Invalid"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_PROMOTION:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongSale"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_BUYFAIL:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_WrongMethod"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_NOHERO:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_NoHero"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_SKINHAS:
                            if (((stBuyHeroSkinRsp.dwHeroID == 0) || (stBuyHeroSkinRsp.dwSkinID == 0)) || masterRoleInfo.IsHaveHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, false))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_AlreadyHave"), false, 1.5f, null, new object[0]);
                                break;
                            }
                            masterRoleInfo.OnAddHeroSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID);
                            Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinBuySuc(stBuyHeroSkinRsp.dwHeroID);
                            CUICommonSystem.ShowNewHeroOrSkin(stBuyHeroSkinRsp.dwHeroID, stBuyHeroSkinRsp.dwSkinID, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, 0);
                            return;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COINLIMIT:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Money"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_COUPONS:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Dianjuan"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_OTHER:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_Other"), false, 1.5f, null, new object[0]);
                            break;

                        case CS_ADDHEROSKIN_ERRCODE.CS_ADDHEROSKIN_RANKGRADE:
                            Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("BuySkin_Error_RankGrade"), false, 1.5f, null, new object[0]);
                            break;

                        default:
                            Singleton<CUIManager>.GetInstance().OpenTips(iResult.ToString(), false, 1.5f, null, new object[0]);
                            break;
                    }
                }
            }
        }

        [MessageHandler(0x729)]
        public static void OnBuySkinForFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stPresentSkinRsp.iResult != 0)
            {
                string strContent = Utility.ProtErrCodeToStr(0x729, msg.stPkgData.stPresentSkinRsp.iResult);
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
        }

        private void OnCloseBuySkinForm(CUIEvent uiEvent)
        {
            Singleton<CHeroSelectNormalSystem>.instance.ResetHero3DObj();
        }

        private void OnFriendListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            RefreshFriendListElementForGift(uiEvent.m_srcWidget, this.m_friendList[srcWidgetIndexInBelongedList], this.m_isBuySkinForFriend);
            CUIEventScript component = uiEvent.m_srcWidget.transform.FindChild("InviteButton").GetComponent<CUIEventScript>();
            component.m_onClickEventParams.commonUInt64Param1 = this.m_friendList[srcWidgetIndexInBelongedList].stUin.ullUid;
            component.m_onClickEventParams.tagUInt = this.m_friendList[srcWidgetIndexInBelongedList].stUin.dwLogicWorldId;
            component.m_onClickEventParams.tagStr = CUIUtility.RemoveEmoji(Utility.UTF8Convert(this.m_friendList[srcWidgetIndexInBelongedList].szUserName));
            if (this.m_isBuySkinForFriend)
            {
                component.m_onClickEventID = enUIEventID.HeroSkin_BuyHeroSkinForFriend;
                component.m_onClickEventParams.heroSkinParam.heroId = this.m_buyHeroIDForFriend;
                component.m_onClickEventParams.heroSkinParam.skinId = this.m_buySkinIDForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
            }
            else
            {
                component.m_onClickEventID = enUIEventID.HeroView_BuyHeroForFriend;
                component.m_onClickEventParams.commonUInt32Param1 = this.m_buyPriceForFriend;
                component.m_onClickEventParams.tag = (int) this.m_buyHeroIDForFriend;
            }
        }

        public void OnHeroInfo_BuyHero(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroId;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            DebugHelper.Assert(dataByKey != null);
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHero;
            uIEvent.m_eventParams.heroId = heroId;
            switch (tag)
            {
                case enPayType.GoldCoin:
                    uIEvent.m_eventParams.tag = 1;
                    break;

                case enPayType.DianQuan:
                    uIEvent.m_eventParams.tag = 0;
                    break;

                case enPayType.Diamond:
                    uIEvent.m_eventParams.tag = 2;
                    break;

                case enPayType.DiamondAndDianQuan:
                    uIEvent.m_eventParams.tag = 3;
                    break;
            }
            CMallSystem.TryToPay(enPayPurpose.Buy, StringHelper.UTF8BytesToString(ref dataByKey.szName), tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
        }

        private void OnHeroInfo_BuyHeroForFriend(CUIEvent uiEvent)
        {
            uiEvent.m_eventID = enUIEventID.HeroView_ConfirmBuyHeroForFriend;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            int tag = uiEvent.m_eventParams.tag;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long) tag);
            string tagStr = uiEvent.m_eventParams.tagStr;
            DebugHelper.Assert(dataByKey != null);
            if (dataByKey != null)
            {
                string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), dataByKey.szName, tagStr);
                CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, payValue, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
            }
        }

        private void OnHeroInfo_BuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            uiEvent.m_eventID = enUIEventID.HeroSkin_ConfirmBuyHeroSkinForFriend;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            string tagStr = uiEvent.m_eventParams.tagStr;
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
            DebugHelper.Assert(heroSkin != null);
            if (heroSkin != null)
            {
                string goodName = string.Format(Singleton<CTextManager>.GetInstance().GetText("BuyForFriendWithName"), heroSkin.szSkinName, tagStr);
                CMallSystem.TryToPay(enPayPurpose.Buy, goodName, enPayType.DianQuan, payValue, uiEvent.m_eventID, ref uiEvent.m_eventParams, enUIEventID.None, true, true, true);
            }
        }

        public void OnHeroInfo_ConfirmBuyHero(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            ReqBuyHero(uiEvent.m_eventParams.heroId, tag);
        }

        private void OnHeroInfo_ConfirmBuyHeroForFriend(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            COMDT_ACNT_UNIQ friendUin = new COMDT_ACNT_UNIQ {
                ullUid = uiEvent.m_eventParams.commonUInt64Param1,
                dwLogicWorldId = uiEvent.m_eventParams.tagUInt
            };
            ReqBuyHeroForFriend((uint) tag, ref friendUin);
        }

        private void OnHeroInfo_ConfirmBuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            COMDT_ACNT_UNIQ friendUin = new COMDT_ACNT_UNIQ {
                ullUid = uiEvent.m_eventParams.commonUInt64Param1,
                dwLogicWorldId = uiEvent.m_eventParams.tagUInt
            };
            ReqBuySkinForFriend(heroId, skinId, ref friendUin);
        }

        private void OnHeroInfo_OpenBuyHeroForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroId;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFriendPath, false, true);
            if (form != null)
            {
                this.InitBuyForFriendForm(form, false, heroId, 0, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, uiEvent.m_eventParams.commonBool);
            }
        }

        private void OnHeroInfo_OpenBuyHeroSkinForFriend(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFriendPath, false, true);
            if (form != null)
            {
                this.InitBuyForFriendForm(form, true, heroId, skinId, uiEvent.m_eventParams.commonUInt64Param1, uiEvent.m_eventParams.commonUInt32Param1, uiEvent.m_eventParams.commonBool);
            }
        }

        private void OnHeroInfo_SearchFriend(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript component = srcFormScript.transform.Find("Panel/List").GetComponent<CUIListScript>();
            InputField field = srcFormScript.transform.Find("Panel/SearchFriend/InputField").GetComponent<InputField>();
            if (field != null)
            {
                ListView<COMDT_FRIEND_INFO> allFriend = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend();
                if (field.text != string.Empty)
                {
                    ListView<COMDT_FRIEND_INFO> view2 = allFriend;
                    allFriend = new ListView<COMDT_FRIEND_INFO>();
                    for (int i = 0; i < view2.Count; i++)
                    {
                        COMDT_FRIEND_INFO item = view2[i];
                        if (StringHelper.UTF8BytesToString(ref item.szUserName).Contains(field.text))
                        {
                            allFriend.Add(item);
                        }
                    }
                }
                bool bSkin = uiEvent.m_eventParams.friendHeroSkinPar.bSkin;
                uint heroId = uiEvent.m_eventParams.friendHeroSkinPar.heroId;
                uint skinId = uiEvent.m_eventParams.friendHeroSkinPar.skinId;
                uint price = uiEvent.m_eventParams.friendHeroSkinPar.price;
                if (allFriend.Count == 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Friend_SearchNoResult"), false, 1.5f, null, new object[0]);
                }
                this.UpdateFriendList(ref allFriend, ref component, bSkin, heroId, skinId, price, 0L, 0, false);
            }
        }

        public void OnHeroSkin_Buy(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            bool isCanCharge = uiEvent.m_eventParams.heroSkinParam.isCanCharge;
            string goodName = StringHelper.UTF8BytesToString(ref CSkinInfo.GetHeroSkin(heroId, skinId).szSkinName);
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            uIEvent.m_eventID = enUIEventID.HeroSkin_BuyConfirm;
            uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
            uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
            uIEvent.m_eventParams.tag = (int) tag;
            uIEvent.m_eventParams.commonUInt32Param1 = payValue;
            CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, isCanCharge, false);
        }

        public void OnHeroSkinBuyConfirm(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
            uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
            BUY_HEROSKIN_TYPE buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
            switch (tag)
            {
                case enPayType.DianQuan:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_COUPONS;
                    break;

                case enPayType.Diamond:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
                    break;

                case enPayType.DiamondAndDianQuan:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_MIXPAY;
                    break;

                default:
                    buyType = BUY_HEROSKIN_TYPE.BUY_HEROSKIN_TYPE_DIAMOND;
                    break;
            }
            ReqBuyHeroSkin(heroId, skinId, buyType, false);
        }

        private void OnOpenBuyHeroForm(CUIEvent uiEvent)
        {
            OpenBuyHeroForm(uiEvent.m_srcFormScript, uiEvent.m_eventParams.heroId, new stPayInfoSet(), enUIEventID.None);
        }

        private void OnOpenBuySkinForm(CUIEvent uiEvent)
        {
            OpenBuyHeroSkinForm(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId, uiEvent.m_eventParams.heroSkinParam.isCanCharge, new stPayInfoSet(), enUIEventID.None);
        }

        private void OnUseSkinExpCard(CUIEvent uiEvent)
        {
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
            if (heroSkin != null)
            {
                CBagSystem.UseSkinExpCard(heroSkin.dwID);
            }
        }

        public static void OpenBuyHeroForm(CUIFormScript srcform, uint heroId, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = 0)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_heroBuyFormPath, false, true);
            if (formScript != null)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                if (dataByKey != null)
                {
                    formScript.transform.Find("heroInfoPanel/title/Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Title");
                    GameObject gameObject = formScript.transform.Find("heroInfoPanel/heroItem").gameObject;
                    Text component = gameObject.transform.Find("heroNameText").GetComponent<Text>();
                    CUICommonSystem.SetHeroItemImage(formScript, gameObject, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false);
                    component.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                    GameObject pricePanel = formScript.transform.Find("heroInfoPanel/heroPricePanel").gameObject;
                    if (payInfoSet.m_payInfoCount > 0)
                    {
                        SetHeroBuyPricePanel(formScript, pricePanel, ref payInfoSet, heroId, btnClickEventID);
                    }
                    else
                    {
                        ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(heroId).promotion();
                        stPayInfoSet payInfoSetOfGood = new stPayInfoSet();
                        payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
                        SetHeroBuyPricePanel(formScript, pricePanel, ref payInfoSetOfGood, heroId, btnClickEventID);
                    }
                    Transform transform = formScript.transform.Find("heroInfoPanel/heroPricePanel/pnlDiamondBuy/buyForFriendBtn");
                    if (transform != null)
                    {
                        if (ShouldShowBuyForFriend(false, heroId, 0, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
                        {
                            transform.gameObject.CustomSetActive(true);
                            CUIEventScript script2 = transform.GetComponent<CUIEventScript>();
                            if (script2 != null)
                            {
                                script2.m_onClickEventParams.heroId = heroId;
                            }
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public static void OpenBuyHeroSkinForm(uint heroId, uint skinId, bool isCanCharge, stPayInfoSet payInfoSet, enUIEventID btnClickEventID = 0)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_buyHeroSkinFormPath, false, true);
            if (formScript != null)
            {
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                if (heroSkin != null)
                {
                    Transform transform = formScript.gameObject.transform.Find("Panel");
                    Image component = transform.Find("skinBgImage/skinIconImage").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
                    component.SetSprite(prefabPath, formScript, true, false, false);
                    transform.Find("skinNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
                    GameObject gameObject = transform.Find("Panel_Prop/List_Prop").gameObject;
                    CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                    CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (payInfoSet.m_payInfoCount == 0)
                    {
                        ResSkinPromotion resPromotion = new ResSkinPromotion();
                        resPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
                        payInfoSet = CMallSystem.GetPayInfoSetOfGood(heroSkin, resPromotion);
                    }
                    Transform skinPricePanel = transform.Find("skinPricePanel");
                    if (payInfoSet.m_payInfoCount > 0)
                    {
                        SetSkinPricePanel(formScript, skinPricePanel, ref payInfoSet.m_payInfos[0]);
                        Transform transform3 = skinPricePanel.Find("buyButton");
                        if (masterRoleInfo != null)
                        {
                            if (!masterRoleInfo.IsHaveHero(heroId, false))
                            {
                                if (transform3 != null)
                                {
                                    Transform transform4 = transform3.Find("Text");
                                    if (transform4 != null)
                                    {
                                        Text text2 = transform4.GetComponent<Text>();
                                        if (text2 != null)
                                        {
                                            text2.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Text_1");
                                        }
                                    }
                                    CUIEventScript script2 = transform3.gameObject.GetComponent<CUIEventScript>();
                                    if (script2 != null)
                                    {
                                        stUIEventParams eventParams = new stUIEventParams();
                                        eventParams.openHeroFormPar.heroId = heroId;
                                        eventParams.openHeroFormPar.skinId = skinId;
                                        eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
                                    }
                                }
                            }
                            else if (transform3 != null)
                            {
                                CUIEventScript script3 = transform3.gameObject.GetComponent<CUIEventScript>();
                                if (script3 != null)
                                {
                                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                                    if (btnClickEventID == enUIEventID.None)
                                    {
                                        uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
                                    }
                                    else
                                    {
                                        uIEvent.m_eventID = btnClickEventID;
                                    }
                                    uIEvent.m_eventParams.tag = (int) payInfoSet.m_payInfos[0].m_payType;
                                    uIEvent.m_eventParams.commonUInt32Param1 = payInfoSet.m_payInfos[0].m_payValue;
                                    uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
                                    uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
                                    uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
                                    script3.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
                                }
                            }
                        }
                    }
                    Transform transform5 = formScript.transform.Find("Panel/skinPricePanel/buyForFriendButton");
                    if (transform5 != null)
                    {
                        if (ShouldShowBuyForFriend(true, heroId, skinId, btnClickEventID == enUIEventID.Mall_Mystery_On_Buy_Item))
                        {
                            transform5.gameObject.CustomSetActive(true);
                            CUIEventScript script4 = transform5.GetComponent<CUIEventScript>();
                            if (script4 != null)
                            {
                                script4.m_onClickEventParams.heroSkinParam.heroId = heroId;
                                script4.m_onClickEventParams.heroSkinParam.skinId = skinId;
                            }
                        }
                        else
                        {
                            transform5.gameObject.CustomSetActive(false);
                        }
                    }
                    GameObject widget = formScript.GetWidget(0);
                    GameObject buyButtonGo = formScript.GetWidget(1);
                    SetRankLimitWidgets(heroId, skinId, widget, buyButtonGo);
                }
            }
        }

        public static void OpenBuyHeroSkinForm3D(uint heroId, uint skinId, bool isCanCharge)
        {
            if (skinId != 0)
            {
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_buyHeroSkin3DFormPath, false, true);
                if (formScript != null)
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    if (heroSkin != null)
                    {
                        Transform transform = formScript.transform.Find("Panel");
                        transform.Find("skinNameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
                        GameObject gameObject = transform.Find("Panel_Prop/List_Prop").gameObject;
                        CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                        CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo == null)
                        {
                            DebugHelper.Assert(false, "OpenBuyHeroSkinForm3D role is null");
                        }
                        else
                        {
                            Transform transform2 = transform.Find("BtnGroup/useExpCardButton");
                            if (transform2 != null)
                            {
                                transform2.gameObject.CustomSetActive(false);
                                if (CBagSystem.IsHaveSkinExpCard(heroSkin.dwID) && !masterRoleInfo.IsHaveHeroSkin(heroId, skinId, true))
                                {
                                    transform2.gameObject.CustomSetActive(true);
                                    CUIEventScript script2 = transform2.GetComponent<CUIEventScript>();
                                    if (script2 != null)
                                    {
                                        stUIEventParams eventParams = new stUIEventParams();
                                        eventParams.heroSkinParam.heroId = heroId;
                                        eventParams.heroSkinParam.skinId = skinId;
                                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OnUseSkinExpCard, eventParams);
                                    }
                                }
                            }
                            Transform transform3 = transform.Find("pricePanel");
                            Transform transform4 = transform.Find("getPathText");
                            Transform transform5 = transform.Find("BtnGroup/buyButton");
                            if ((transform3 != null) && (transform5 != null))
                            {
                                transform3.gameObject.CustomSetActive(false);
                                transform5.gameObject.CustomSetActive(false);
                            }
                            if (transform4 != null)
                            {
                                transform4.gameObject.CustomSetActive(false);
                            }
                            if (masterRoleInfo.IsHaveHero(heroId, false) && CSkinInfo.IsCanBuy(heroId, skinId))
                            {
                                stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroSkin.dwHeroID, heroSkin.dwSkinID);
                                if (skinPayInfoSet.m_payInfoCount <= 0)
                                {
                                    return;
                                }
                                if ((transform3 != null) && (transform5 != null))
                                {
                                    transform3.gameObject.CustomSetActive(true);
                                    transform5.gameObject.CustomSetActive(true);
                                    Transform transform6 = transform3.Find("costImage");
                                    if (transform6 != null)
                                    {
                                        Image image = transform6.gameObject.GetComponent<Image>();
                                        if (image != null)
                                        {
                                            image.SetSprite(CMallSystem.GetPayTypeIconPath(skinPayInfoSet.m_payInfos[0].m_payType), formScript, true, false, false);
                                        }
                                    }
                                    Transform transform7 = transform3.Find("priceText");
                                    if (transform7 != null)
                                    {
                                        Text text2 = transform7.gameObject.GetComponent<Text>();
                                        if (text2 != null)
                                        {
                                            text2.text = skinPayInfoSet.m_payInfos[0].m_payValue.ToString();
                                        }
                                    }
                                    if (transform5 != null)
                                    {
                                        CUIEventScript script3 = transform5.gameObject.GetComponent<CUIEventScript>();
                                        if (script3 != null)
                                        {
                                            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                                            uIEvent.m_eventID = enUIEventID.HeroSkin_Buy;
                                            uIEvent.m_eventParams.tag = (int) skinPayInfoSet.m_payInfos[0].m_payType;
                                            uIEvent.m_eventParams.commonUInt32Param1 = skinPayInfoSet.m_payInfos[0].m_payValue;
                                            uIEvent.m_eventParams.heroSkinParam.heroId = heroId;
                                            uIEvent.m_eventParams.heroSkinParam.skinId = skinId;
                                            uIEvent.m_eventParams.heroSkinParam.isCanCharge = isCanCharge;
                                            script3.SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
                                        }
                                    }
                                }
                            }
                            else if (transform4 != null)
                            {
                                transform4.gameObject.CustomSetActive(true);
                                if (masterRoleInfo.IsHaveHero(heroId, false))
                                {
                                    transform4.GetComponent<Text>().text = CHeroInfoSystem2.GetSkinCannotBuyStr(heroSkin);
                                }
                                else
                                {
                                    transform4.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("HeroSelect_GetHeroFirstTip");
                                }
                            }
                            CUI3DImageScript component = transform.Find("3DImage").gameObject.GetComponent<CUI3DImageScript>();
                            ObjNameData data = CUICommonSystem.GetHeroPrefabPath(heroId, (int) skinId, true);
                            GameObject model = component.AddGameObject(data.ObjectName, false, false);
                            if (model != null)
                            {
                                if (data.ActorInfo != null)
                                {
                                    model.transform.localScale = new Vector3(data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale, data.ActorInfo.LobbyScale);
                                }
                                DynamicShadow.EnableDynamicShow(component.gameObject, true);
                                CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
                                instance.Set3DModel(model);
                                instance.InitAnimatList();
                                instance.InitAnimatSoundList(heroId, skinId);
                                instance.OnModePlayAnima("Come");
                            }
                            GameObject widget = formScript.GetWidget(0);
                            GameObject buyButtonGo = formScript.GetWidget(1);
                            SetRankLimitWidgets(heroId, skinId, widget, buyButtonGo);
                        }
                    }
                }
            }
        }

        public static void RefreshFriendListElementForGift(GameObject element, COMDT_FRIEND_INFO friend, bool bSkin)
        {
            CInviteView.UpdateFriendListElementBase(element, ref friend);
            Transform transform = element.transform.Find("Gender");
            if (transform != null)
            {
                COM_SNSGENDER bGender = (COM_SNSGENDER) friend.bGender;
                transform.gameObject.CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
                switch (bGender)
                {
                    case COM_SNSGENDER.COM_SNSGENDER_MALE:
                        CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_boy", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                        break;

                    case COM_SNSGENDER.COM_SNSGENDER_FEMALE:
                        CUIUtility.SetImageSprite(transform.GetComponent<Image>(), string.Format("{0}icon/Ico_girl", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                        break;
                }
            }
        }

        public static void ReqBuyHero(uint HeroId, int BuyType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x719);
            msg.stPkgData.stBuyHeroReq.dwHeroID = HeroId;
            msg.stPkgData.stBuyHeroReq.bBuyType = (byte) BuyType;
            IHeroData data = CHeroDataFactory.CreateHeroData(HeroId);
            if (data != null)
            {
                if (data.promotion() != null)
                {
                    msg.stPkgData.stBuyHeroReq.bIsPromotion = Convert.ToByte(true);
                }
                else
                {
                    msg.stPkgData.stBuyHeroReq.bIsPromotion = 0;
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void ReqBuyHeroForFriend(uint heroId, ref COMDT_ACNT_UNIQ friendUin)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x726);
            msg.stPkgData.stPresentHeroReq.stFriendUin = friendUin;
            msg.stPkgData.stPresentHeroReq.dwHeroID = heroId;
            IHeroData data = CHeroDataFactory.CreateHeroData(heroId);
            if (data != null)
            {
                if (data.promotion() != null)
                {
                    msg.stPkgData.stPresentHeroReq.bIsPromotion = Convert.ToByte(true);
                }
                else
                {
                    msg.stPkgData.stPresentHeroReq.bIsPromotion = 0;
                }
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqBuyHeroSkin(uint heroId, uint skinId, BUY_HEROSKIN_TYPE buyType, bool isSendGameSvr = false)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x71b);
            msg.stPkgData.stBuyHeroSkinReq.dwHeroID = heroId;
            msg.stPkgData.stBuyHeroSkinReq.dwSkinID = skinId;
            msg.stPkgData.stBuyHeroSkinReq.bBuyType = (byte) buyType;
            ResSkinPromotion promotion = new ResSkinPromotion();
            stPayInfoSet set = new stPayInfoSet();
            if (CSkinInfo.GetSkinPromotion(heroId, skinId) != null)
            {
                msg.stPkgData.stBuyHeroSkinReq.bIsPromotion = Convert.ToByte(true);
            }
            else
            {
                msg.stPkgData.stBuyHeroSkinReq.bIsPromotion = 0;
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void ReqBuySkinForFriend(uint heroId, uint skinId, ref COMDT_ACNT_UNIQ friendUin)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x728);
            msg.stPkgData.stPresentSkinReq.stFriendUin = friendUin;
            msg.stPkgData.stPresentSkinReq.dwSkinID = CSkinInfo.GetSkinCfgId(heroId, skinId);
            if (CSkinInfo.GetSkinPromotion(heroId, skinId) != null)
            {
                msg.stPkgData.stPresentSkinReq.bIsPromotion = Convert.ToByte(true);
            }
            else
            {
                msg.stPkgData.stPresentSkinReq.bIsPromotion = 0;
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SetHeroBuyPricePanel(CUIFormScript formScript, GameObject pricePanel, ref stPayInfoSet payInfoSet, uint heroID, enUIEventID btnClickEventID = 0)
        {
            if ((null != formScript) && (pricePanel != null))
            {
                Transform payInfoPanel = pricePanel.transform.Find("pnlCoinBuy");
                Transform transform2 = pricePanel.transform.Find("pnlDiamondBuy");
                Transform transform3 = pricePanel.transform.Find("Text");
                if ((payInfoPanel != null) && (transform2 != null))
                {
                    payInfoPanel.gameObject.CustomSetActive(false);
                    transform2.gameObject.CustomSetActive(false);
                    if (transform3 != null)
                    {
                        transform3.gameObject.CustomSetActive(payInfoSet.m_payInfoCount > 1);
                    }
                    for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
                    {
                        if (payInfoSet.m_payInfos[i].m_payType == enPayType.GoldCoin)
                        {
                            payInfoPanel.gameObject.CustomSetActive(true);
                            SetPayInfoPanel(formScript, payInfoPanel, ref payInfoSet.m_payInfos[i], heroID, btnClickEventID);
                        }
                        else
                        {
                            transform2.gameObject.CustomSetActive(true);
                            SetPayInfoPanel(formScript, transform2, ref payInfoSet.m_payInfos[i], heroID, btnClickEventID);
                        }
                    }
                }
            }
        }

        public static void SetPayCostIcon(CUIFormScript formScript, Transform costIcon, enPayType payType)
        {
            if ((null != formScript) && (null != costIcon))
            {
                Image component = costIcon.GetComponent<Image>();
                if (component != null)
                {
                    component.SetSprite(CMallSystem.GetPayTypeIconPath(payType), formScript, true, false, false);
                }
            }
        }

        public static void SetPayCostTypeText(Transform costTypeText, enPayType payType)
        {
            if (costTypeText != null)
            {
                Text component = costTypeText.GetComponent<Text>();
                if (component != null)
                {
                    component.text = CMallSystem.GetPayTypeText(payType);
                }
            }
        }

        public static void SetPayCurrentPrice(Transform currentPrice, uint payValue)
        {
            if (currentPrice != null)
            {
                Text component = currentPrice.GetComponent<Text>();
                if (component != null)
                {
                    component.text = payValue.ToString();
                }
            }
        }

        private static void SetPayInfoPanel(CUIFormScript formScript, Transform payInfoPanel, ref stPayInfo payInfo, uint heroID, enUIEventID btnClickEventID)
        {
            Transform costIcon = payInfoPanel.Find("costImage");
            SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
            SetPayCostTypeText(payInfoPanel.Find("costTypeText"), payInfo.m_payType);
            Transform transform3 = payInfoPanel.Find("costPanel");
            if (transform3 != null)
            {
                Transform transform4 = transform3.Find("oldPricePanel");
                if (transform4 != null)
                {
                    transform4.gameObject.CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
                    SetPayOldPrice(transform4.Find("oldPriceText"), payInfo.m_oriValue);
                }
                SetPayCurrentPrice(transform3.Find("costText"), payInfo.m_payValue);
            }
            Text text = null;
            Transform transform7 = payInfoPanel.Find("buyBtn");
            if (transform7 != null)
            {
                Transform transform8 = transform7.Find("Text");
                if (transform8 != null)
                {
                    text = transform8.gameObject.GetComponent<Text>();
                    if (text != null)
                    {
                        text.text = CMallSystem.GetPriceTypeBuyString(payInfo.m_payType);
                    }
                }
                CUIEventScript component = transform7.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams {
                    tag = (int) payInfo.m_payType,
                    commonUInt32Param1 = payInfo.m_payValue,
                    heroId = heroID
                };
                if (btnClickEventID != enUIEventID.None)
                {
                    component.SetUIEvent(enUIEventType.Click, btnClickEventID, eventParams);
                }
                else
                {
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroView_BuyHero, eventParams);
                }
            }
        }

        public static void SetPayOldPrice(Transform oldPrice, uint oriValue)
        {
            if (oldPrice != null)
            {
                Text component = oldPrice.GetComponent<Text>();
                if (component != null)
                {
                    component.text = oriValue.ToString();
                }
            }
        }

        private static void SetRankLimitWidgets(uint heroId, uint skinId, GameObject rankLimitTextGo, GameObject buyButtonGo)
        {
            RES_RANK_LIMIT_TYPE res_rank_limit_type;
            byte num;
            bool flag;
            ulong num2;
            if (CSkinInfo.IsBuyForbiddenForRankBigGrade(heroId, skinId, out res_rank_limit_type, out num, out num2, out flag))
            {
                CUICommonSystem.SetButtonEnable(buyButtonGo.GetComponent<Button>(), false, false, true);
            }
            if (CSkinInfo.IsCanBuy(heroId, skinId) && flag)
            {
                rankLimitTextGo.CustomSetActive(true);
                Text component = rankLimitTextGo.GetComponent<Text>();
                component.text = string.Empty;
                string rankBigGradeName = CLadderView.GetRankBigGradeName(num);
                switch (res_rank_limit_type)
                {
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_CURGRADE:
                    {
                        string[] args = new string[] { rankBigGradeName };
                        component.text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Current_Grade", args);
                        break;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_SEASONGRADE:
                    {
                        string[] textArray2 = new string[] { rankBigGradeName };
                        component.text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_Season_Highest_Grade", textArray2);
                        break;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_MAXGRADE:
                    {
                        string[] textArray3 = new string[] { rankBigGradeName };
                        component.text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Highest_Grade", textArray3);
                        break;
                    }
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_HISTORYGRADE:
                    {
                        string[] textArray4 = new string[] { Singleton<CLadderSystem>.GetInstance().GetLadderSeasonName(num2), rankBigGradeName };
                        component.text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Buy_Skin_Ladder_Limit_History_Grade", textArray4);
                        break;
                    }
                }
            }
            else
            {
                rankLimitTextGo.CustomSetActive(false);
            }
        }

        private static void SetSkinPricePanel(CUIFormScript formScript, Transform skinPricePanel, ref stPayInfo payInfo)
        {
            if ((null != formScript) && (null != skinPricePanel))
            {
                Transform costIcon = skinPricePanel.Find("costImage");
                SetPayCostIcon(formScript, costIcon, payInfo.m_payType);
                SetPayCostTypeText(skinPricePanel.Find("costTypeText"), payInfo.m_payType);
                Transform transform3 = skinPricePanel.Find("costPanel");
                if (transform3 != null)
                {
                    Transform transform4 = transform3.Find("oldPricePanel");
                    if (transform4 != null)
                    {
                        transform4.gameObject.CustomSetActive(payInfo.m_oriValue != payInfo.m_payValue);
                        SetPayOldPrice(transform4.Find("oldPriceText"), payInfo.m_oriValue);
                    }
                    SetPayCurrentPrice(transform3.Find("costText"), payInfo.m_payValue);
                }
            }
        }

        public static bool ShouldShowBuyForFriend(bool bSkin, uint heroId, uint skinId = 0, bool forceNotShow = false)
        {
            if (!forceNotShow)
            {
                if (bSkin)
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
                    DebugHelper.Assert(heroSkin != null);
                    if (heroSkin != null)
                    {
                        stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroSkin);
                        return ((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && GameDataMgr.IsSkinCanBuyNow(heroSkin.dwID)) && GameDataMgr.IsSkinCanBuyForFriend(heroSkin.dwID));
                    }
                }
                else
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                    DebugHelper.Assert(dataByKey != null);
                    if (dataByKey != null)
                    {
                        stPayInfoSet set2 = CMallSystem.GetPayInfoSetOfGood(dataByKey);
                        bool flag = false;
                        for (int i = 0; i < set2.m_payInfoCount; i++)
                        {
                            if (((set2.m_payInfos[i].m_payType == enPayType.Diamond) || (set2.m_payInfos[i].m_payType == enPayType.DianQuan)) || (set2.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan))
                            {
                                flag = true;
                                break;
                            }
                        }
                        return (((Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PRESENTHERO) && flag) && GameDataMgr.IsHeroAvailableAtShop(dataByKey.dwCfgID)) && GameDataMgr.IsHeroCanBuyForFriend(dataByKey.dwCfgID));
                    }
                }
            }
            return false;
        }

        public override void UnInit()
        {
        }

        private void UpdateFriendList(ref ListView<COMDT_FRIEND_INFO> allFriends, ref CUIListScript list, bool bSkin, uint heroId, uint skinId, uint price, ulong friendUid = 0, uint worldId = 0, bool isSns = false)
        {
            if ((friendUid > 0L) && (worldId > 0))
            {
                this.detailFriendList.Clear();
                for (int i = 0; i < allFriends.Count; i++)
                {
                    if ((allFriends[i].stUin.ullUid == friendUid) && (allFriends[i].stUin.dwLogicWorldId == worldId))
                    {
                        this.detailFriendList.Add(allFriends[i]);
                        this.m_friendList = this.detailFriendList;
                        break;
                    }
                }
            }
            else
            {
                this.m_friendList = allFriends;
            }
            this.m_isBuySkinForFriend = bSkin;
            this.m_buyHeroIDForFriend = heroId;
            this.m_buySkinIDForFriend = skinId;
            this.m_buyPriceForFriend = price;
            list.SetElementAmount(0);
            list.SetElementAmount(this.m_friendList.Count);
        }

        public enum enBuyHeroSkin3DFormWidget
        {
            Buy_Rank_Limit_Text,
            Buy_Button
        }

        public enum enBuyHeroSkinFormWidget
        {
            Buy_Rank_Limit_Text,
            Buy_Button
        }
    }
}

