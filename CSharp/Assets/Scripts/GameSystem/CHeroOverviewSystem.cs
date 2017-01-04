namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CHeroOverviewSystem : Singleton<CHeroOverviewSystem>
    {
        protected ListView<IHeroData> m_heroList = new ListView<IHeroData>();
        protected enHeroJobType m_selectHeroType;
        public static string s_heroViewFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Overview.prefab";

        protected void FilterHeroData()
        {
            ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
            for (int i = 0; i < allHeroList.Count; i++)
            {
                if (((this.m_selectHeroType == enHeroJobType.All) || (allHeroList[i].bMainJob == ((byte) this.m_selectHeroType))) || (allHeroList[i].bMinorJob == ((byte) this.m_selectHeroType)))
                {
                    IHeroData item = CHeroDataFactory.CreateHeroData(allHeroList[i].dwCfgID);
                    this.m_heroList.Add(item);
                }
            }
            SortHeroList(ref this.m_heroList);
        }

        public IHeroData GetHeroDataBuyIndex(int index)
        {
            if ((index >= 0) && (index < this.m_heroList.Count))
            {
                return this.m_heroList[index];
            }
            return null;
        }

        public int GetHeroIndexByConfigId(uint inCfgId)
        {
            for (int i = 0; i < this.m_heroList.Count; i++)
            {
                IHeroData data = this.m_heroList[i];
                if (data.cfgID == inCfgId)
                {
                    return i;
                }
            }
            return 0;
        }

        public int GetHeroListCount()
        {
            return this.m_heroList.Count;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroViewForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroView_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroView_ItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroView_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroView_MenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_CloseForm));
            Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
        }

        private void OnHeroInfo_CloseForm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                SortHeroList(ref this.m_heroList);
                form.transform.Find("Panel_Hero/List").gameObject.GetComponent<CUIListScript>().SetElementAmount(this.m_heroList.Count);
            }
        }

        private void OnHeroInfo_Compose(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x456);
            msg.stPkgData.stItemComp.wTargetType = 4;
            msg.stPkgData.stItemComp.dwTargetID = uiEvent.m_eventParams.heroId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnHeroInfoChange(uint heroId)
        {
            CHeroInfo info2;
            if ((Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath) != null) && Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfo(heroId, out info2, false))
            {
                this.RefreshHeroListElement(heroId);
            }
        }

        public void OnHeroView_CloseForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroViewFormPath);
        }

        public void OnHeroView_ItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject gameObject = uiEvent.m_srcWidget.transform.Find("heroItem").gameObject;
            SetPveHeroItemData(uiEvent.m_srcFormScript, gameObject, this.m_heroList[srcWidgetIndexInBelongedList]);
        }

        public void OnHeroView_MenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_selectHeroType = (enHeroJobType) selectedIndex;
            this.ResetHeroListData();
        }

        public void OnNtyAddHero(uint id)
        {
            this.ResetHeroListData();
        }

        public void OnOpenHeroViewForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_heroViewFormPath, false, true);
            if (script != null)
            {
                this.m_selectHeroType = enHeroJobType.All;
                string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                CUICommonSystem.InitMenuPanel(script.transform.Find("Panel_Menu/List").gameObject, titleList, (int) this.m_selectHeroType);
                this.ResetHeroListData();
                CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroListBtn);
            }
        }

        private void RefreshHeroListElement(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                CUIListScript component = form.gameObject.transform.Find("Panel_Hero/List").gameObject.GetComponent<CUIListScript>();
                for (int i = 0; i < this.m_heroList.Count; i++)
                {
                    if (this.m_heroList[i].cfgID == heroId)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(i);
                        if (elemenet != null)
                        {
                            GameObject gameObject = elemenet.gameObject.transform.Find("heroItem").gameObject;
                            SetPveHeroItemData(form, gameObject, this.m_heroList[i]);
                        }
                        break;
                    }
                }
            }
        }

        protected virtual void ResetHeroListData()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroViewFormPath);
            if (form != null)
            {
                CUIListScript component = form.gameObject.transform.Find("Panel_Hero/List").gameObject.GetComponent<CUIListScript>();
                this.m_heroList.Clear();
                this.FilterHeroData();
                component.SetElementAmount(this.m_heroList.Count);
            }
        }

        public static void SetPveHeroItemData(CUIFormScript formScript, GameObject listItem, IHeroData data)
        {
            if ((listItem != null) && (data != null))
            {
                bool bPlayerOwn = data.bPlayerOwn;
                Transform transform = listItem.transform;
                Transform transform2 = transform.Find("heroProficiencyImg");
                Transform transform3 = transform.Find("heroProficiencyBgImg");
                CUICommonSystem.SetHeroProficiencyIconImage(formScript, transform2.gameObject, data.proficiencyLV);
                CUICommonSystem.SetHeroProficiencyBgImage(formScript, transform3.gameObject, data.proficiencyLV, false);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    transform3.GetComponent<Image>().color = !masterRoleInfo.IsHaveHero(data.cfgID, true) ? CUIUtility.s_Color_GrayShader : Color.white;
                    transform2.GetComponent<Image>().color = !masterRoleInfo.IsHaveHero(data.cfgID, true) ? CUIUtility.s_Color_GrayShader : Color.white;
                }
                bool flag2 = false;
                bool bActive = false;
                bool flag4 = false;
                if (masterRoleInfo != null)
                {
                    flag2 = masterRoleInfo.IsFreeHero(data.cfgID);
                    bActive = masterRoleInfo.IsCreditFreeHero(data.cfgID);
                    flag4 = masterRoleInfo.IsValidExperienceHero(data.cfgID);
                    CUICommonSystem.SetHeroItemImage(formScript, listItem, masterRoleInfo.GetHeroSkinPic(data.cfgID), enHeroHeadType.enBust, (!bPlayerOwn && !flag2) && !flag4);
                }
                GameObject gameObject = transform.Find("profession").gameObject;
                CUICommonSystem.SetHeroJob(formScript, gameObject, (enHeroJobType) data.heroType);
                transform.Find("heroNameText").GetComponent<Text>().text = data.heroName;
                Transform transform4 = transform.Find("TxtFree");
                Transform transform5 = transform.Find("TxtCreditFree");
                if (transform4 != null)
                {
                    transform4.gameObject.CustomSetActive(flag2 && !bActive);
                }
                if (transform5 != null)
                {
                    transform5.gameObject.CustomSetActive(bActive);
                }
                transform.Find("imgExperienceMark").gameObject.CustomSetActive(data.IsValidExperienceHero());
                CUIEventScript component = listItem.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.openHeroFormPar.heroId = data.cfgID;
                eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, eventParams);
            }
        }

        public static void SortHeroList(ref ListView<IHeroData> heroList)
        {
            <SortHeroList>c__AnonStorey71 storey = new <SortHeroList>c__AnonStorey71 {
                role = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo()
            };
            if (storey.role != null)
            {
                heroList.Sort(new Comparison<IHeroData>(storey.<>m__5B));
            }
        }

        public override void UnInit()
        {
        }

        [CompilerGenerated]
        private sealed class <SortHeroList>c__AnonStorey71
        {
            internal CRoleInfo role;

            internal int <>m__5B(IHeroData heroData1, IHeroData heroData2)
            {
                if (heroData1.bPlayerOwn && !heroData2.bPlayerOwn)
                {
                    return -1;
                }
                if (heroData2.bPlayerOwn && !heroData1.bPlayerOwn)
                {
                    return 1;
                }
                if (this.role.IsFreeHero(heroData1.cfgID) && !this.role.IsFreeHero(heroData2.cfgID))
                {
                    return -1;
                }
                if (this.role.IsFreeHero(heroData2.cfgID) && !this.role.IsFreeHero(heroData1.cfgID))
                {
                    return 1;
                }
                if (this.role.IsValidExperienceHero(heroData1.cfgID) && !this.role.IsValidExperienceHero(heroData2.cfgID))
                {
                    return -1;
                }
                if (this.role.IsValidExperienceHero(heroData2.cfgID) && !this.role.IsValidExperienceHero(heroData1.cfgID))
                {
                    return 1;
                }
                return heroData1.sortId.CompareTo(heroData2.sortId);
            }
        }
    }
}

