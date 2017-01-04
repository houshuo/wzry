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

    public class UrlAction
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;
        public Action action;
        public RES_GAME_ENTRANCE_TYPE form;
        private static char[] InnerSpliter = new char[] { '#', '&' };
        private static char[] MultiSpliter = new char[] { '\n' };
        public ulong overTime;
        private static char[] ParamSpliter = new char[] { '=' };
        public uint prodID;
        public int prodSpec;
        public COM_ITEM_TYPE prodType;
        public int showTime = 0xbb8;
        public string target;
        public string url;

        public bool Execute()
        {
            switch (this.action)
            {
                case Action.openUrl:
                    CUICommonSystem.OpenUrl(this.url, false);
                    goto Label_0235;

                case Action.openForm:
                    CUICommonSystem.JumpForm(this.form);
                    goto Label_0235;

                case Action.buy:
                    switch (this.prodType)
                    {
                        case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
                            if (GameDataMgr.specSaleDict.ContainsKey(this.prodID))
                            {
                                CMallFactoryShopController.ShopProduct theSp = new CMallFactoryShopController.ShopProduct(GameDataMgr.specSaleDict[this.prodID]);
                                Singleton<CMallFactoryShopController>.GetInstance().StartShopProduct(theSp);
                                goto Label_0235;
                            }
                            return false;

                        case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        {
                            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (masterRoleInfo == null)
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips("internalError", false, 1.5f, null, new object[0]);
                                return false;
                            }
                            if (masterRoleInfo.IsHaveHero(this.prodID, false))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips("hasOwnHero", true, 1.5f, null, new object[0]);
                                return false;
                            }
                            CUIEvent uiEvent = new CUIEvent {
                                m_eventID = enUIEventID.HeroInfo_OpenBuyHeroForm
                            };
                            uiEvent.m_eventParams.heroId = this.prodID;
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                            goto Label_0235;
                        }
                        case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                        {
                            CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                            if (info2 == null)
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips("internalError", false, 1.5f, null, new object[0]);
                                return false;
                            }
                            if (!info2.IsHaveHero(this.prodID, false))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips("Skin_NeedToBuyAHero", true, 1.5f, null, new object[0]);
                                return false;
                            }
                            if (info2.IsHaveHeroSkin(this.prodID, (uint) this.prodSpec, false))
                            {
                                Singleton<CUIManager>.GetInstance().OpenTips("hasOwnHeroSkin", true, 1.5f, null, new object[0]);
                                return false;
                            }
                            CUIEvent event3 = new CUIEvent {
                                m_eventID = enUIEventID.HeroSkin_OpenBuySkinForm
                            };
                            event3.m_eventParams.heroSkinParam.heroId = this.prodID;
                            event3.m_eventParams.heroSkinParam.skinId = (uint) this.prodSpec;
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                            goto Label_0235;
                        }
                    }
                    break;

                default:
                    return false;
            }
            return false;
        Label_0235:
            return true;
        }

        public static ListView<UrlAction> ParseFromText(string text, char[] spliter = null)
        {
            ListView<UrlAction> view = new ListView<UrlAction>();
            if (spliter == null)
            {
                spliter = MultiSpliter;
            }
            try
            {
                string[] strArray = text.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strArray.Length; i++)
                {
                    string[] strArray2 = strArray[i].Split(InnerSpliter, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray2.Length > 0)
                    {
                        DictionaryView<string, string> view2 = new DictionaryView<string, string>();
                        for (int j = 1; j < strArray2.Length; j++)
                        {
                            string[] strArray3 = strArray2[j].Split(ParamSpliter);
                            if ((strArray3 != null) && (strArray3.Length == 2))
                            {
                                view2.Add(strArray3[0], strArray3[1]);
                            }
                        }
                        UrlAction item = new UrlAction {
                            target = strArray2[0],
                            action = Action.none
                        };
                        if (view2.ContainsKey("action"))
                        {
                            string key = view2["action"];
                            if (key != null)
                            {
                                int num3;
                                if (<>f__switch$map2 == null)
                                {
                                    Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                                    dictionary.Add("openUrl", 0);
                                    dictionary.Add("openForm", 1);
                                    dictionary.Add("buy", 2);
                                    <>f__switch$map2 = dictionary;
                                }
                                if (<>f__switch$map2.TryGetValue(key, out num3))
                                {
                                    switch (num3)
                                    {
                                        case 0:
                                            item.action = Action.openUrl;
                                            item.url = view2["url"];
                                            break;

                                        case 1:
                                            item.action = Action.openForm;
                                            item.form = (RES_GAME_ENTRANCE_TYPE) int.Parse(view2["form"]);
                                            break;

                                        case 2:
                                            item.action = Action.buy;
                                            item.prodType = (COM_ITEM_TYPE) int.Parse(view2["prodType"]);
                                            item.prodID = uint.Parse(view2["prodID"]);
                                            if (view2.ContainsKey("prodSpec"))
                                            {
                                                int.TryParse(view2["prodSpec"], out item.prodSpec);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        if (view2.ContainsKey("overTime"))
                        {
                            ulong.TryParse(view2["overTime"], out item.overTime);
                        }
                        if (view2.ContainsKey("showTime"))
                        {
                            int.TryParse(view2["showTime"], out item.showTime);
                        }
                        view.Add(item);
                    }
                }
            }
            catch (Exception)
            {
            }
            return view;
        }

        public enum Action
        {
            none,
            openUrl,
            openForm,
            buy,
            COUNT
        }
    }
}

