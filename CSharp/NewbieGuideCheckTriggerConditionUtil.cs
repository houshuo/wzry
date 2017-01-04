using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

public class NewbieGuideCheckTriggerConditionUtil
{
    public static uint AvailableHeroId;
    public static uint AvailableItemId;
    public static int AvailableSlotType;
    public static uint AvailableSymbolId;
    public static int AvailableSymbolPos;
    public static CTask AvailableTask;

    private static bool CheckCompleteNewbieDungeonCondition(NewbieGuideTriggerConditionItem condition)
    {
        return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(0);
    }

    private static bool CheckCompleteNormalDungeonCondition(NewbieGuideTriggerConditionItem condition)
    {
        return Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int) condition.Param[0]);
    }

    private static bool CheckOwnCompleteNewbieGuideCondition(NewbieGuideTriggerConditionItem condition)
    {
        return MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieGuideComplete(condition.Param[0]);
    }

    private static bool CheckOwnCompleteNewWeakGuideCondition(NewbieGuideTriggerConditionItem condition)
    {
        return MonoSingleton<NewbieGuideManager>.GetInstance().IsWeakLineComplete(condition.Param[0]);
    }

    public static bool CheckTriggerCondition(uint id, NewbieGuideTriggerConditionItem condition)
    {
        bool flag6;
        uint dwCfgID;
        switch (condition.wType)
        {
            case 1:
                return CheckCompleteNewbieDungeonCondition(condition);

            case 2:
                return CheckCompleteNormalDungeonCondition(condition);

            case 3:
                return CheckOwnCompleteNewbieGuideCondition(condition);

            case 4:
                return CheckUnCompleteNewbieGuideCondition(condition);

            case 5:
                return false;

            case 6:
                return false;

            case 7:
                AvailableTask = null;
                return Singleton<CTaskSys>.instance.model.AnyTaskOfState(COM_TASK_STATE.COM_TASK_HAVEDONE, RES_TASK_TYPE.RES_TASKTYPE_MAIN, out AvailableTask);

            case 8:
                return false;

            case 9:
                return false;

            case 10:
                return false;

            case 11:
            {
                uint num3 = condition.Param[0];
                uint num4 = condition.Param[1];
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    return false;
                }
                uint haveHeroCount = (uint) masterRoleInfo.GetHaveHeroCount(false);
                switch (num4)
                {
                    case 0:
                        return (haveHeroCount == num3);

                    case 1:
                        return (haveHeroCount > num3);

                    case 2:
                        return (haveHeroCount < num3);
                }
                DebugHelper.Assert(false);
                return false;
            }
            case 12:
            {
                flag6 = false;
                dwCfgID = condition.Param[0];
                RES_SHOPBUY_COINTYPE coinType = (condition.Param[1] != 0) ? RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN : RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS;
                CRoleInfo info5 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info5 != null)
                {
                    if (dwCfgID <= 0)
                    {
                        ListView<ResHeroCfgInfo>.Enumerator enumerator = CHeroDataFactory.GetAllHeroList().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            dwCfgID = enumerator.Current.dwCfgID;
                            flag6 |= info5.CheckHeroBuyable(dwCfgID, coinType);
                            if (flag6)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    flag6 = info5.CheckHeroBuyable(dwCfgID, coinType);
                }
                break;
            }
            case 13:
                return Singleton<CShopSystem>.GetInstance().IsNormalShopItemsInited();

            case 14:
                return (((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin >= condition.Param[0]) && Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP)) && Singleton<CShopSystem>.GetInstance().IsNormalShopItemsInited());

            case 15:
                return CAdventureSys.IsChapterFullStar(Singleton<CAdventureSys>.GetInstance().currentChapter, Singleton<CAdventureSys>.GetInstance().currentDifficulty);

            case 0x10:
            {
                uint num7 = condition.Param[0];
                if (num7 <= 0)
                {
                    return false;
                }
                if (!Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int) num7))
                {
                    return false;
                }
                return CAdventureSys.IsLevelFullStar((int) num7);
            }
            case 0x11:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPMODE);

            case 0x12:
                return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, CAdventureSys.MOPUP_TICKET_ID) >= condition.Param[0]);

            case 0x13:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ZONGSHILIAN);

            case 20:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ZHUANGZIHUANMENG);

            case 0x15:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG);

            case 0x16:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ELITELEVEL);

            case 0x17:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION);

            case 0x18:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL);

            case 0x19:
                return (Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BLACKSHOP) && Singleton<CShopSystem>.GetInstance().IsMysteryShopAvailable());

            case 0x1a:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPCOINSHOP);

            case 0x1b:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK);

            case 0x1c:
                return false;

            case 30:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(0x1a);

            case 0x1f:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(0x1b);

            case 0x20:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(0x15);

            case 0x21:
            {
                CRoleInfo info7 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                return ((info7 != null) && (info7.GoldCoin >= condition.Param[0]));
            }
            case 0x22:
                return false;

            case 0x24:
                return false;

            case 0x25:
            {
                bool flag = false;
                uint baseID = condition.Param[0];
                CRoleInfo info = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info != null)
                {
                    CUseableContainer useableContainer = info.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if ((useableContainer != null) && (useableContainer.GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, baseID) != null))
                    {
                        flag = true;
                    }
                }
                AvailableItemId = !flag ? 0 : baseID;
                return flag;
            }
            case 0x26:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(9);

            case 0x27:
            {
                uint num2 = condition.Param[0];
                CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                return ((info3 != null) && (info3.GetHaveHeroCount(false) >= num2));
            }
            case 40:
            {
                CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info2 == null)
                {
                    return false;
                }
                return info2.m_symbolInfo.CheckAnyWearSymbol(out AvailableSymbolPos, out AvailableSymbolId);
            }
            case 0x29:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA);

            case 0x2a:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(0x2c);

            case 0x2b:
            {
                uint key = condition.Param[0];
                ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(key);
                if (dataByKey == null)
                {
                    goto Label_057D;
                }
                CRoleInfo info9 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info9 == null)
                {
                    goto Label_057D;
                }
                return (info9.SymbolCoin >= dataByKey.dwMakeCoin);
            }
            case 0x2c:
                return Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL);

            case 0x2d:
                return CAddSkillSys.IsSelSkillAvailable();

            case 0x2e:
                return CheckOwnCompleteNewWeakGuideCondition(condition);

            case 0x2f:
                return CheckOwnCompleteNewWeakGuideCondition(condition);

            case 0x30:
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.m_pageCount <= 1)
                {
                    return false;
                }
                return true;

            case 0x31:
            {
                LevelRewardData levelRewardData = Singleton<CTaskSys>.instance.model.GetLevelRewardData((int) condition.Param[0]);
                if (levelRewardData == null)
                {
                    return false;
                }
                return !levelRewardData.m_bHasGetReward;
            }
            case 50:
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_freeDrawInfo[4].dwLeftFreeDrawCnt <= 0)
                {
                    return false;
                }
                return true;

            case 0x33:
                return Singleton<CFunctionUnlockSys>.instance.FucIsUnlock((RES_SPECIALFUNCUNLOCK_TYPE) condition.Param[0]);

            case 0x34:
            {
                CRoleInfo info12 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (((info12 == null) || !info12.IsOldPlayer()) || info12.IsOldPlayerGuided())
                {
                    return false;
                }
                return true;
            }
            case 0x35:
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if (curLvelContext == null)
                {
                    return false;
                }
                return curLvelContext.IsMultilModeWithWarmBattle();
            }
            case 0x36:
            {
                bool flag12 = false;
                CRoleInfo info13 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info13 != null)
                {
                    flag12 = info13.IsGuidedStateSet(0x62);
                }
                return flag12;
            }
            case 0x37:
                return CBattleGuideManager.EnableHeroVictoryTips();

            case 0x38:
                return Singleton<GameReplayModule>.GetInstance().HasRecord;

            default:
                return false;
        }
        if (flag6)
        {
            AvailableHeroId = dwCfgID;
        }
        return flag6;
    Label_057D:
        return false;
    }

    private static bool CheckUnCompleteNewbieGuideCondition(NewbieGuideTriggerConditionItem condition)
    {
        return !MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieGuideComplete(condition.Param[0]);
    }
}

