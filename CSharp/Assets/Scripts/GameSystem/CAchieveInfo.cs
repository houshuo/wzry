namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    [MessageHandlerClass]
    public class CAchieveInfo
    {
        public int[] m_achieveDoneArr = new int[0x2b];
        public ListView<CAchieveItem> m_achieveList = new ListView<CAchieveItem>();

        public static CAchieveInfo GetAchieveInfo()
        {
            return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_achieveInfo;
        }

        public CAchieveItem GetAchieveItemById(uint achievementId)
        {
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if (this.m_achieveList[i].m_cfgId == achievementId)
                {
                    return this.m_achieveList[i];
                }
            }
            return null;
        }

        public int GetFinishAchievePoint(int achieveType)
        {
            int num = 0;
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if ((((this.m_achieveList[i] != null) && this.m_achieveList[i].IsFinish()) && (this.m_achieveList[i].m_cfgInfo != null)) && ((this.m_achieveList[i].m_cfgInfo.dwType == achieveType) || (achieveType == 0)))
                {
                    num += (int) this.m_achieveList[i].m_cfgInfo.dwPoint;
                }
            }
            return num;
        }

        public int GetGotRewardAchievePoint(int achieveType)
        {
            int num = 0;
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if ((((this.m_achieveList[i] != null) && this.m_achieveList[i].IsGotReward()) && (this.m_achieveList[i].m_cfgInfo != null)) && ((this.m_achieveList[i].m_cfgInfo.dwType == achieveType) || (achieveType == 0)))
                {
                    num += (int) this.m_achieveList[i].m_cfgInfo.dwPoint;
                }
            }
            return num;
        }

        public ListView<CAchieveItem> GetNeedShowAchieveItemsByType(int achieveType)
        {
            ListView<CAchieveItem> view = new ListView<CAchieveItem>();
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if (((this.m_achieveList[i] != null) && (this.m_achieveList[i].m_cfgInfo != null)) && ((this.m_achieveList[i].m_cfgInfo.dwType == achieveType) && this.m_achieveList[i].IsNeedShow()))
                {
                    view.Add(this.m_achieveList[i]);
                }
            }
            return view;
        }

        public int GetTotalAchievePoint(int achieveType)
        {
            int num = 0;
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if (((this.m_achieveList[i] != null) && (this.m_achieveList[i].m_cfgInfo != null)) && ((this.m_achieveList[i].m_cfgInfo.dwType == achieveType) || (achieveType == 0)))
                {
                    num += (int) this.m_achieveList[i].m_cfgInfo.dwPoint;
                }
            }
            return num;
        }

        public void InitAchieveInfo(COMDT_ACHIEVEMENT_INFO svrAchieveInfo)
        {
            this.m_achieveList.Clear();
            int index = 0;
            for (index = 0; index < 0x2b; index++)
            {
                this.m_achieveDoneArr[index] = 0;
            }
            int dwDoneType = 0;
            for (index = 0; index < svrAchieveInfo.dwDoneTypeNum; index++)
            {
                dwDoneType = (int) svrAchieveInfo.astDoneData[index].dwDoneType;
                this.m_achieveDoneArr[dwDoneType] = svrAchieveInfo.astDoneData[index].iDoneCnt;
            }
            for (index = 0; index < svrAchieveInfo.dwAchievementNum; index++)
            {
                CAchieveItem item = new CAchieveItem();
                item.InitStateData(svrAchieveInfo.astAchievementData[index]);
                item.SetDoneData(ref this.m_achieveDoneArr);
                this.m_achieveList.Add(item);
            }
        }

        public bool IsHaveFinishButNotGetRewardAchievement(int achieveType)
        {
            for (int i = 0; i < this.m_achieveList.Count; i++)
            {
                if ((((this.m_achieveList[i] != null) && (this.m_achieveList[i].m_cfgInfo != null)) && ((this.m_achieveList[i].m_cfgInfo.dwType == achieveType) || (achieveType == 0))) && (this.m_achieveList[i].GetAchieveState() == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnAchieveDoneDataChange(COMDT_ACHIEVEMENT_DONE_DATA donwData)
        {
            int count = this.m_achieveList.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.m_achieveList[i].m_doneType == ((RES_ACHIEVE_DONE_TYPE) donwData.dwDoneType))
                {
                    this.m_achieveList[i].SetAchieveDoneCnt(donwData.iDoneCnt);
                }
            }
        }

        public void OnAchieveStateChange(COMDT_ACHIEVEMENT_DATA achieveData)
        {
            int count = this.m_achieveList.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.m_achieveList[i].m_cfgId == achieveData.dwID)
                {
                    this.m_achieveList[i].SetAchieveState((COM_ACHIEVEMENT_STATE) achieveData.bState);
                    return;
                }
            }
        }
    }
}

