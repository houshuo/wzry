namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class CHeroDataFactory
    {
        [CompilerGenerated]
        private static Action<ResHeroCfgInfo> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<ResHeroCfgInfo> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<ResHeroCfgInfo> <>f__am$cache5;
        private static ListView<IHeroData> m_banHeroList;
        private static ListView<ResHeroCfgInfo> m_CfgHeroList;
        private static ListView<ResHeroCfgInfo> m_CfgHeroShopList;

        public static IHeroData CreateCustomHeroData(uint id)
        {
            return new CCustomHeroData(id);
        }

        public static IHeroData CreateHeroData(uint id)
        {
            CHeroInfo info2;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CreateHeroData ---- Master role is null");
            if ((masterRoleInfo != null) && masterRoleInfo.GetHeroInfo(id, out info2, true))
            {
                return new CHeroInfoData(info2);
            }
            return new CHeroCfgData(id);
        }

        public static ListView<ResHeroCfgInfo> GetAllHeroList()
        {
            if (m_CfgHeroList == null)
            {
                m_CfgHeroList = new ListView<ResHeroCfgInfo>();
            }
            if (m_CfgHeroList.Count > 0)
            {
                m_CfgHeroList.Clear();
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = delegate (ResHeroCfgInfo x) {
                    if (GameDataMgr.IsHeroAvailable(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || (x.bIOSHide == 0)))
                    {
                        m_CfgHeroList.Add(x);
                    }
                };
            }
            GameDataMgr.heroDatabin.Accept(<>f__am$cache3);
            return m_CfgHeroList;
        }

        public static ListView<ResHeroCfgInfo> GetAllHeroListAtShop()
        {
            if (m_CfgHeroShopList == null)
            {
                m_CfgHeroShopList = new ListView<ResHeroCfgInfo>();
            }
            if (m_CfgHeroShopList.Count > 0)
            {
                m_CfgHeroShopList.Clear();
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (ResHeroCfgInfo x) {
                    if (GameDataMgr.IsHeroAvailableAtShop(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || (x.bIOSHide == 0)))
                    {
                        m_CfgHeroShopList.Add(x);
                    }
                };
            }
            GameDataMgr.heroDatabin.Accept(<>f__am$cache5);
            return m_CfgHeroShopList;
        }

        public static ListView<IHeroData> GetBanHeroList()
        {
            if (m_banHeroList == null)
            {
                m_banHeroList = new ListView<IHeroData>();
            }
            if (m_banHeroList.Count > 0)
            {
                m_banHeroList.Clear();
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = delegate (ResHeroCfgInfo x) {
                    if (GameDataMgr.IsHeroAvailable(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || (x.bIOSHide == 0)))
                    {
                        m_banHeroList.Add(CreateHeroData(x.dwCfgID));
                    }
                };
            }
            GameDataMgr.heroDatabin.Accept(<>f__am$cache4);
            return m_banHeroList;
        }

        public static ListView<IHeroData> GetHostHeroList(bool isIncludeValidExperienceHero)
        {
            ListView<IHeroData> heroList = new ListView<IHeroData>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (isIncludeValidExperienceHero)
                    {
                        KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                        if (!masterRoleInfo.IsOwnHero(current.Key))
                        {
                            KeyValuePair<uint, CHeroInfo> pair2 = enumerator.Current;
                            if (!masterRoleInfo.IsValidExperienceHero(pair2.Key))
                            {
                                continue;
                            }
                        }
                        KeyValuePair<uint, CHeroInfo> pair3 = enumerator.Current;
                        heroList.Add(CreateHeroData(pair3.Key));
                    }
                    else
                    {
                        KeyValuePair<uint, CHeroInfo> pair4 = enumerator.Current;
                        if (masterRoleInfo.IsOwnHero(pair4.Key))
                        {
                            KeyValuePair<uint, CHeroInfo> pair5 = enumerator.Current;
                            heroList.Add(CreateHeroData(pair5.Key));
                        }
                    }
                }
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    for (int i = heroList.Count - 1; i >= 0; i--)
                    {
                        IHeroData item = heroList[i];
                        if (item.heroCfgInfo.bIOSHide > 0)
                        {
                            heroList.Remove(item);
                        }
                    }
                }
                CHeroOverviewSystem.SortHeroList(ref heroList);
            }
            return heroList;
        }

        public static ListView<IHeroData> GetPvPHeroList()
        {
            ListView<IHeroData> heroList = new ListView<IHeroData>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                    if ((current.Value.MaskBits & 2) > 0)
                    {
                        KeyValuePair<uint, CHeroInfo> pair2 = enumerator.Current;
                        heroList.Add(CreateHeroData(pair2.Key));
                    }
                }
                for (int i = 0; i < masterRoleInfo.freeHeroList.Count; i++)
                {
                    if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList[i].dwFreeHeroID))
                    {
                        heroList.Add(CreateHeroData(masterRoleInfo.freeHeroList[i].dwFreeHeroID));
                    }
                }
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    for (int j = heroList.Count - 1; j >= 0; j--)
                    {
                        IHeroData item = heroList[j];
                        if (item.heroCfgInfo.bIOSHide > 0)
                        {
                            heroList.Remove(item);
                        }
                    }
                }
                CHeroOverviewSystem.SortHeroList(ref heroList);
            }
            return heroList;
        }

        public static ListView<IHeroData> GetTrainingHeroList()
        {
            ListView<IHeroData> heroList = new ListView<IHeroData>();
            List<uint> list = new List<uint>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                    if ((current.Value.MaskBits & 2) > 0)
                    {
                        KeyValuePair<uint, CHeroInfo> pair2 = enumerator.Current;
                        heroList.Add(CreateHeroData(pair2.Key));
                        KeyValuePair<uint, CHeroInfo> pair3 = enumerator.Current;
                        list.Add(pair3.Key);
                    }
                }
                for (int i = 0; i < masterRoleInfo.freeHeroList.Count; i++)
                {
                    if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList[i].dwFreeHeroID))
                    {
                        heroList.Add(CreateHeroData(masterRoleInfo.freeHeroList[i].dwFreeHeroID));
                        list.Add(masterRoleInfo.freeHeroList[i].dwFreeHeroID);
                    }
                }
                ListView<ResHeroCfgInfo> allHeroList = GetAllHeroList();
                for (int j = 0; j < allHeroList.Count; j++)
                {
                    if ((allHeroList[j].bIsTrainUse == 1) && !list.Contains(allHeroList[j].dwCfgID))
                    {
                        heroList.Add(CreateHeroData(allHeroList[j].dwCfgID));
                    }
                }
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    for (int k = heroList.Count - 1; k >= 0; k--)
                    {
                        IHeroData item = heroList[k];
                        if (item.heroCfgInfo.bIOSHide > 0)
                        {
                            heroList.Remove(item);
                        }
                    }
                }
                CHeroOverviewSystem.SortHeroList(ref heroList);
            }
            return heroList;
        }

        public static bool IsHeroCanUse(uint heroID)
        {
            bool flag = false;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                if (masterRoleInfo.GetHeroInfoDic().ContainsKey(heroID))
                {
                    return true;
                }
                if (masterRoleInfo.IsFreeHero(heroID))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static void ResetBufferList()
        {
            if ((m_CfgHeroList != null) && (m_CfgHeroList.Count > 0))
            {
                m_CfgHeroList.Clear();
            }
            if ((m_CfgHeroShopList != null) && (m_CfgHeroShopList.Count > 0))
            {
                m_CfgHeroShopList.Clear();
            }
        }
    }
}

