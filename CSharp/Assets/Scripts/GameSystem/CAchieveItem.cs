namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;

    public class CAchieveItem : IComparable
    {
        public uint m_cfgId;
        public ResAchievement m_cfgInfo;
        private int m_doneCnt;
        public RES_ACHIEVE_DONE_TYPE m_doneType;
        private COM_ACHIEVEMENT_STATE m_state;

        public int CompareTo(object obj)
        {
            CAchieveItem item = obj as CAchieveItem;
            if (this.m_state == item.m_state)
            {
                if (this.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN)
                {
                    if (this.m_cfgInfo.dwClassification > item.m_cfgInfo.dwClassification)
                    {
                        return -1;
                    }
                    if (this.m_cfgInfo.dwClassification < item.m_cfgInfo.dwClassification)
                    {
                        return 1;
                    }
                }
                return this.m_cfgId.CompareTo(item.m_cfgId);
            }
            if (this.m_state != COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN)
            {
                if (this.m_state != COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_UNFIN)
                {
                    return 1;
                }
                if (item.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN)
                {
                    return 1;
                }
            }
            return -1;
        }

        public int GetAchieveDoneCnt()
        {
            return this.m_doneCnt;
        }

        public uint GetAchievementAwardCnt(RES_REWARDS_TYPE rewardType)
        {
            for (int i = 0; i < this.m_cfgInfo.astReward.Length; i++)
            {
                if (this.m_cfgInfo.astReward[i].bRewardType == ((byte) rewardType))
                {
                    return this.m_cfgInfo.astReward[i].dwRewardNum;
                }
            }
            return 0;
        }

        public uint GetAchievementAwardId(RES_REWARDS_TYPE rewardType)
        {
            for (int i = 0; i < this.m_cfgInfo.astReward.Length; i++)
            {
                if (this.m_cfgInfo.astReward[i].bRewardType == ((byte) rewardType))
                {
                    return this.m_cfgInfo.astReward[i].dwRewardID;
                }
            }
            return 0;
        }

        public string GetAchievementBgIconPath()
        {
            return (CUIUtility.s_Sprite_Dynamic_Task_Dir + this.m_cfgInfo.dwBgImage);
        }

        public string GetAchievementDesc()
        {
            string format = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szDesc);
            if (this.m_cfgInfo.dwDoneParm == 0)
            {
                return string.Format(format, this.m_cfgInfo.dwDoneCondi);
            }
            return string.Format(format, this.m_cfgInfo.dwDoneCondi, this.m_cfgInfo.dwDoneParm);
        }

        public string GetAchievementIconPath()
        {
            return (CUIUtility.s_Sprite_Dynamic_Task_Dir + this.m_cfgInfo.dwImage);
        }

        public string GetAchievementName()
        {
            return StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szName);
        }

        public string GetAchievementTips()
        {
            return string.Format(StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szTips), Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Name);
        }

        public static string GetAchievementTypeName(int type)
        {
            string str;
            switch (((RES_ACHIEVE_TYPE) type))
            {
                case RES_ACHIEVE_TYPE.RES_ACHIEVE_GROWING:
                    str = "Achievement_Type_Growing";
                    break;

                case RES_ACHIEVE_TYPE.RES_ACHIEVE_HERO:
                    str = "Achievement_Type_Hero";
                    break;

                case RES_ACHIEVE_TYPE.RES_ACHIEVE_PVP:
                    str = "Achievement_Type_Pvp";
                    break;

                case RES_ACHIEVE_TYPE.RES_ACHIEVE_PVE:
                    str = "Achievement_Type_Pve";
                    break;

                case RES_ACHIEVE_TYPE.RES_ACHIEVE_SOCIALLY:
                    str = "Achievement_Type_Socially";
                    break;

                case RES_ACHIEVE_TYPE.RES_ACHIEVE_RANK:
                    str = "Achievement_Type_Rank";
                    break;

                default:
                    str = "ERROR_ACHIEVE_TYPE";
                    break;
            }
            return Singleton<CTextManager>.GetInstance().GetText(str);
        }

        public COM_ACHIEVEMENT_STATE GetAchieveState()
        {
            return this.m_state;
        }

        public void InitStateData(COMDT_ACHIEVEMENT_DATA stateInfo)
        {
            this.m_cfgId = stateInfo.dwID;
            this.m_state = (COM_ACHIEVEMENT_STATE) stateInfo.bState;
            this.m_cfgInfo = GameDataMgr.achieveDatabin.GetDataByKey(this.m_cfgId);
            if (this.m_cfgInfo != null)
            {
                this.m_doneType = (RES_ACHIEVE_DONE_TYPE) this.m_cfgInfo.dwDoneType;
            }
        }

        public bool IsCanGetReward()
        {
            return (COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.m_state);
        }

        public bool IsDelayPopUp()
        {
            return (this.m_cfgInfo.bIsPopUpDelay > 0);
        }

        public bool IsFinish()
        {
            return ((this.m_state == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD) || (COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN == this.m_state));
        }

        public bool IsGotReward()
        {
            return (COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD == this.m_state);
        }

        public bool IsHideForegroundIcon()
        {
            return (this.m_cfgInfo.bHideForegroundImage > 0);
        }

        public bool IsNeedShow()
        {
            if ((this.m_cfgInfo.dwPreAchievementID > 0) && !CAchieveInfo.GetAchieveInfo().GetAchieveItemById(this.m_cfgInfo.dwPreAchievementID).IsFinish())
            {
                return false;
            }
            return true;
        }

        public void SetAchieveDoneCnt(int cnt)
        {
            this.m_doneCnt = cnt;
        }

        public void SetAchieveState(COM_ACHIEVEMENT_STATE stateVal)
        {
            this.m_state = stateVal;
        }

        public void SetDoneData(ref int[] achieveDoneArr)
        {
            if ((achieveDoneArr != null) && ((this.m_doneType >= RES_ACHIEVE_DONE_TYPE.RES_ACHIEVE_DONE_GET_GOLD) && (this.m_doneType < achieveDoneArr.Length)))
            {
                this.m_doneCnt = achieveDoneArr[(int) this.m_doneType];
            }
        }
    }
}

