using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;

public class NewbieGuideCheckSkipConditionUtil
{
    public static bool CheckSkipCondition(NewbieGuideSkipConditionItem item, uint[] param)
    {
        switch (item.wType)
        {
            case 1:
            {
                bool flag = false;
                if ((param == null) || (param.Length <= 0))
                {
                    return Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int) item.Param[0]);
                }
                if (param[0] == item.Param[0])
                {
                    flag = Singleton<CAdventureSys>.GetInstance().IsLevelFinished((int) param[0]);
                }
                return flag;
            }
            case 2:
                if (!MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieGuideComplete(item.Param[0]))
                {
                    return false;
                }
                return true;

            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13:
            case 20:
            case 0x15:
            case 0x16:
            case 0x17:
            case 0x18:
            case 0x1f:
            case 0x20:
            case 0x21:
            case 0x22:
            case 0x23:
            case 0x24:
            case 0x25:
            case 0x26:
            case 40:
            case 0x29:
            case 0x2b:
            case 0x2d:
            case 0x2e:
            case 0x2f:
            case 0x30:
            case 0x33:
            {
                int inIndex = TranslateFromSkipCond((NewbieGuideSkipConditionType) item.wType);
                if (inIndex != -1)
                {
                    return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsGuidedStateSet(inIndex);
                }
                return true;
            }
            case 30:
                return MonoSingleton<NewbieGuideManager>.GetInstance().IsNewbieBitSet((int) item.Param[0]);

            case 0x2a:
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    break;
                }
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (useableContainer == null)
                {
                    break;
                }
                return (useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, item.Param[0]) >= 2);
            }
            case 0x2c:
                return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().IsNewbieAchieveSet((int) (item.Param[0] + NewbieGuideManager.WEAKGUIDE_BIT_OFFSET));

            case 0x31:
            {
                CRoleInfo info3 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (info3 != null)
                {
                    bool flag2 = info3.IsGuidedStateSet(0x59);
                    bool flag3 = info3.IsGuidedStateSet(90);
                    bool flag4 = flag2 || flag3;
                    return (info3.IsGuidedStateSet(0x59) || info3.IsGuidedStateSet(90));
                }
                return false;
            }
            default:
                return true;
        }
        return false;
    }

    public static int TranslateFromSkipCond(NewbieGuideSkipConditionType inCondType)
    {
        switch (inCondType)
        {
            case NewbieGuideSkipConditionType.hasCompleteEquipping:
                return 1;

            case NewbieGuideSkipConditionType.hasRewardTaskPvp:
                return 8;

            case NewbieGuideSkipConditionType.hasCompleteHeroAdv:
                return 2;

            case NewbieGuideSkipConditionType.hasSummonedHero:
                return 3;

            case NewbieGuideSkipConditionType.hasOverThreeHeroes:
                return 7;

            case NewbieGuideSkipConditionType.hasCompleteHeroStar:
                return 4;

            case NewbieGuideSkipConditionType.hasHeroSkillUpgraded:
                return 5;

            case NewbieGuideSkipConditionType.hasBoughtHero:
                return 9;

            case NewbieGuideSkipConditionType.hasBoughtItem:
                return 10;

            case NewbieGuideSkipConditionType.hasGotChapterReward:
                return 11;

            case NewbieGuideSkipConditionType.hasMopup:
                return 12;

            case NewbieGuideSkipConditionType.hasEnteredPvP:
                return 13;

            case NewbieGuideSkipConditionType.hasEnteredTrial:
                return 14;

            case NewbieGuideSkipConditionType.hasEnteredZhuangzi:
                return 15;

            case NewbieGuideSkipConditionType.hasEnteredBurning:
                return 0x10;

            case NewbieGuideSkipConditionType.hasEnteredElitePvE:
                return 0x11;

            case NewbieGuideSkipConditionType.hasEnteredGuild:
                return 0x12;

            case NewbieGuideSkipConditionType.hasUsedSymbol:
                return 0x13;

            case NewbieGuideSkipConditionType.hasEnteredMysteryShop:
                return 20;

            case NewbieGuideSkipConditionType.hasAdvancedEquip:
                return 30;

            case NewbieGuideSkipConditionType.hasCompleteBaseGuide:
                return 0;

            case NewbieGuideSkipConditionType.hasCompleteHumanAi33Match:
                return 0x1a;

            case NewbieGuideSkipConditionType.hasCompleteHuman33Match:
                return 0x1b;

            case NewbieGuideSkipConditionType.hasComplete33Guide:
                return 0x15;

            case NewbieGuideSkipConditionType.hasCompleteLottery:
                return 6;

            case NewbieGuideSkipConditionType.hasIncreaseEquip:
                return 0x1d;

            case NewbieGuideSkipConditionType.hasRewardTaskPve:
                return 0x22;

            case NewbieGuideSkipConditionType.hasCompleteHeroUp:
                return 0x20;

            case NewbieGuideSkipConditionType.hasEnteredTournament:
                return 0x21;

            case NewbieGuideSkipConditionType.hasCompleteHumanAi33:
                return 0x2c;

            case NewbieGuideSkipConditionType.hasManufacuredSymbol:
                return 0x3a;

            case NewbieGuideSkipConditionType.hasCoinDrawFive:
                return 80;

            case NewbieGuideSkipConditionType.hasCompleteTrainLevel55:
                return 0x62;

            case NewbieGuideSkipConditionType.hasComplete11Match:
                return 0x51;

            case NewbieGuideSkipConditionType.hasCompleteTrainLevel33:
                return 0x55;

            case NewbieGuideSkipConditionType.hasDiamondDraw:
                return 100;

            case NewbieGuideSkipConditionType.hasCompleteCoronaGuide:
                return 0x53;
        }
        DebugHelper.Assert(false);
        return -1;
    }

    public static NewbieGuideSkipConditionType TranslateToSkipCond(int inNewbieType)
    {
        NewbieGuideSkipConditionType invalid = NewbieGuideSkipConditionType.Invalid;
        switch (inNewbieType)
        {
            case 0:
                return NewbieGuideSkipConditionType.hasCompleteBaseGuide;

            case 1:
                return NewbieGuideSkipConditionType.hasCompleteEquipping;

            case 2:
                return NewbieGuideSkipConditionType.hasCompleteHeroAdv;

            case 3:
                return NewbieGuideSkipConditionType.hasSummonedHero;

            case 4:
                return NewbieGuideSkipConditionType.hasCompleteHeroStar;

            case 5:
                return NewbieGuideSkipConditionType.hasHeroSkillUpgraded;

            case 6:
                return NewbieGuideSkipConditionType.hasCompleteLottery;

            case 7:
                return NewbieGuideSkipConditionType.hasOverThreeHeroes;

            case 8:
                return NewbieGuideSkipConditionType.hasRewardTaskPvp;

            case 9:
                return NewbieGuideSkipConditionType.hasBoughtHero;

            case 10:
                return NewbieGuideSkipConditionType.hasBoughtItem;

            case 11:
                return NewbieGuideSkipConditionType.hasGotChapterReward;

            case 12:
                return NewbieGuideSkipConditionType.hasMopup;

            case 13:
                return NewbieGuideSkipConditionType.hasEnteredPvP;

            case 14:
                return NewbieGuideSkipConditionType.hasEnteredTrial;

            case 15:
                return NewbieGuideSkipConditionType.hasEnteredZhuangzi;

            case 0x10:
                return NewbieGuideSkipConditionType.hasEnteredBurning;

            case 0x11:
                return NewbieGuideSkipConditionType.hasEnteredElitePvE;

            case 0x12:
                return NewbieGuideSkipConditionType.hasEnteredGuild;

            case 0x13:
                return NewbieGuideSkipConditionType.hasUsedSymbol;

            case 20:
                return NewbieGuideSkipConditionType.hasEnteredMysteryShop;

            case 0x15:
                return NewbieGuideSkipConditionType.hasComplete33Guide;

            case 0x1a:
                return NewbieGuideSkipConditionType.hasCompleteHumanAi33Match;

            case 0x1b:
                return NewbieGuideSkipConditionType.hasCompleteHuman33Match;

            case 0x1d:
                return NewbieGuideSkipConditionType.hasIncreaseEquip;

            case 30:
                return NewbieGuideSkipConditionType.hasAdvancedEquip;

            case 0x20:
                return NewbieGuideSkipConditionType.hasCompleteHeroUp;

            case 0x21:
                return NewbieGuideSkipConditionType.hasEnteredTournament;

            case 0x22:
                return NewbieGuideSkipConditionType.hasRewardTaskPve;

            case 0x2c:
                return NewbieGuideSkipConditionType.hasCompleteHumanAi33;

            case 0x3a:
                return NewbieGuideSkipConditionType.hasManufacuredSymbol;

            case 80:
                return NewbieGuideSkipConditionType.hasCoinDrawFive;

            case 0x51:
                return NewbieGuideSkipConditionType.hasComplete11Match;

            case 0x53:
                return NewbieGuideSkipConditionType.hasCompleteCoronaGuide;

            case 0x55:
                return NewbieGuideSkipConditionType.hasCompleteTrainLevel33;

            case 0x62:
                return NewbieGuideSkipConditionType.hasCompleteTrainLevel55;

            case 0x63:
                return invalid;

            case 100:
                return NewbieGuideSkipConditionType.hasDiamondDraw;
        }
        return invalid;
    }
}

