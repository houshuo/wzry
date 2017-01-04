namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class CExperienceCardSystem : Singleton<CExperienceCardSystem>
    {
        public static string HeroExperienceCardFramePath = (CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Bg_card_hreo");
        public static string HeroExperienceCardMarkPath = (CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Img_free_hero");
        public static string SkinExperienceCardFramePath = (CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Bg_card_skin");
        public static string SkinExperienceCardMarkPath = (CUIUtility.s_Sprite_Dynamic_ExperienceCard_Dir + "Img_free_skin");

        public override void Init()
        {
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, int>("HeroExperienceAdd", new Action<uint, int>(this.OnExperienceHeroAdd));
            Singleton<EventRouter>.GetInstance().AddEventHandler<string, uint, uint>("HeroExperienceTimeUpdate", new Action<string, uint, uint>(this.OnHeroExperienceTimeUpdate));
        }

        private void OnExperienceHeroAdd(uint heroId, int experienceDays)
        {
            if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
            {
                CUICommonSystem.ShowNewHeroOrSkin(heroId, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0, experienceDays);
            }
            else if (!Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode() && (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal))
            {
                IHeroData selectHeroData = CHeroDataFactory.CreateHeroData(heroId);
                Singleton<CHeroSelectNormalSystem>.instance.HeroSelect_SelectHero(selectHeroData);
            }
        }

        private void OnHeroExperienceTimeUpdate(string heroName, uint oldDeadLine, uint newDeadLine)
        {
            if ((0 < oldDeadLine) && (oldDeadLine < newDeadLine))
            {
                int experienceHeroOrSkinExtendDays = CHeroInfo.GetExperienceHeroOrSkinExtendDays(newDeadLine - oldDeadLine);
                object[] replaceArr = new object[] { heroName, experienceHeroOrSkinExtendDays };
                Singleton<CUIManager>.GetInstance().OpenTips("ExpCard_ExtendDays", true, 1f, null, replaceArr);
            }
        }

        [MessageHandler(0x723)]
        public static void OnReceiveLimitSkinDel(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            SCPKG_LIMITSKIN_DEL stLimitSkinDel = msg.stPkgData.stLimitSkinDel;
            for (int i = 0; i < stLimitSkinDel.dwNum; i++)
            {
                if (masterRoleInfo.heroExperienceSkinDic.ContainsKey(stLimitSkinDel.SkinID[i]))
                {
                    masterRoleInfo.heroExperienceSkinDic.Remove(stLimitSkinDel.SkinID[i]);
                }
            }
        }

        [MessageHandler(0x724)]
        public static void OnReceiveUseExpCardNtf(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            switch (msg.stPkgData.stUseExpCardNtf.iResult)
            {
                case 2:
                    Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_HeroOwn", true, 1.5f, null, new object[0]);
                    break;

                case 3:
                    Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_InsertHero", true, 1.5f, null, new object[0]);
                    break;

                case 4:
                    Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_SkinOwn", true, 1.5f, null, new object[0]);
                    break;

                case 5:
                    Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_InsertSkin", true, 1.5f, null, new object[0]);
                    break;

                case 6:
                    Singleton<CUIManager>.GetInstance().OpenTips("ExpCardError_Other", true, 1.5f, null, new object[0]);
                    break;
            }
        }

        [MessageHandler(0x722)]
        public static void OnRecieveLimitSkinAdd(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            COMDT_HERO_LIMIT_SKIN_LIST stLimitSkinList = msg.stPkgData.stLimitSkinAdd.stLimitSkinList;
            for (int i = 0; i < stLimitSkinList.dwNum; i++)
            {
                COMDT_HERO_LIMIT_SKIN comdt_hero_limit_skin = stLimitSkinList.astSkinList[i];
                if (masterRoleInfo.heroExperienceSkinDic.ContainsKey(comdt_hero_limit_skin.dwSkinID))
                {
                    uint num2 = masterRoleInfo.heroExperienceSkinDic[comdt_hero_limit_skin.dwSkinID];
                    masterRoleInfo.heroExperienceSkinDic[comdt_hero_limit_skin.dwSkinID] = comdt_hero_limit_skin.dwDeadLine;
                    if ((0 < num2) && (num2 < masterRoleInfo.heroExperienceSkinDic[comdt_hero_limit_skin.dwSkinID]))
                    {
                        int experienceHeroOrSkinExtendDays = CHeroInfo.GetExperienceHeroOrSkinExtendDays(masterRoleInfo.heroExperienceSkinDic[comdt_hero_limit_skin.dwSkinID] - num2);
                        string skinName = CSkinInfo.GetSkinName(comdt_hero_limit_skin.dwSkinID);
                        object[] replaceArr = new object[] { skinName, experienceHeroOrSkinExtendDays };
                        Singleton<CUIManager>.GetInstance().OpenTips("ExpCard_ExtendDays", true, 1f, null, replaceArr);
                    }
                }
                else
                {
                    uint num4;
                    uint num5;
                    masterRoleInfo.heroExperienceSkinDic.Add(comdt_hero_limit_skin.dwSkinID, comdt_hero_limit_skin.dwDeadLine);
                    CSkinInfo.ResolveHeroSkin(comdt_hero_limit_skin.dwSkinID, out num4, out num5);
                    if (!Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
                    {
                        CUICommonSystem.ShowNewHeroOrSkin(num4, num5, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0, CHeroInfo.GetExperienceHeroOrSkinValidDays(comdt_hero_limit_skin.dwDeadLine));
                    }
                    else if (!Singleton<CHeroSelectBaseSystem>.instance.IsMultilMode())
                    {
                        CHeroInfoSystem2.ReqWearHeroSkin(num4, num5, true);
                    }
                }
            }
        }
    }
}

