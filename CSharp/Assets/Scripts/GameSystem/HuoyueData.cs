namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class HuoyueData
    {
        public uint day_curNum;
        public List<ushort> day_huoyue_list = new List<ushort>();
        public List<ushort> have_get_list_day = new List<ushort>();
        public List<ushort> have_get_list_week = new List<ushort>();
        public uint max_dayhuoyue_num;
        public DictionaryView<ushort, CUseable> useable_cfg = new DictionaryView<ushort, CUseable>();
        public uint week_curNum;
        public ushort week_reward1;
        public uint week_reward1_cost;
        public ushort week_reward2;
        public uint week_reward2_cost;

        public bool BAlready_Reward(RES_HUOYUEDU_TYPE type, ushort id)
        {
            List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
            return list.Contains(id);
        }

        private uint Calc_Day_Max_Num()
        {
            uint dwHuoYueDu = 0;
            for (int i = 0; i < this.day_huoyue_list.Count; i++)
            {
                ResHuoYueDuReward reward = null;
                GameDataMgr.huoyueduDict.TryGetValue(this.day_huoyue_list[i], out reward);
                if ((reward != null) && (reward.dwHuoYueDu > dwHuoYueDu))
                {
                    dwHuoYueDu = reward.dwHuoYueDu;
                }
            }
            return dwHuoYueDu;
        }

        public void Clear()
        {
            this.day_curNum = this.week_curNum = 0;
            this.have_get_list_day.Clear();
            this.have_get_list_week.Clear();
        }

        public void Get_Reward(RES_HUOYUEDU_TYPE type, ushort id)
        {
            List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
            DebugHelper.Assert(!list.Contains(id));
            list.Add(id);
        }

        public ResHuoYueDuReward GetRewardCfg(ushort id)
        {
            ResHuoYueDuReward reward;
            GameDataMgr.huoyueduDict.TryGetValue(id, out reward);
            return reward;
        }

        public CUseable GetUsable(ushort id)
        {
            CUseable useable = null;
            this.useable_cfg.TryGetValue(id, out useable);
            if (useable != null)
            {
                return useable;
            }
            ResHuoYueDuReward rewardCfg = this.GetRewardCfg(id);
            ResDT_HuoYueDuReward_PeriodInfo info = this.IsInTime(rewardCfg);
            if (info == null)
            {
                return CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, rewardCfg.dwRewardID, 0);
            }
            return CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, info.dwRewardID, (int) info.dwRewardNum);
        }

        public ResDT_HuoYueDuReward_PeriodInfo IsInTime(ResHuoYueDuReward cfg)
        {
            for (int i = 0; i < cfg.astPeriodInfo.Length; i++)
            {
                ResDT_HuoYueDuReward_PeriodInfo info = cfg.astPeriodInfo[i];
                int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                if ((currentUTCTime >= info.dwStartTimeGen) && (currentUTCTime <= info.dwEndTimeGen))
                {
                    return info;
                }
            }
            return null;
        }

        public void ParseHuoyuedu(ResHuoYueDuReward Cfg)
        {
            if (Cfg != null)
            {
                if ((Cfg.bHuoYueDuType == 1) && !this.day_huoyue_list.Contains(Cfg.wID))
                {
                    this.day_huoyue_list.Add(Cfg.wID);
                }
                if (Cfg.bHuoYueDuType == 2)
                {
                    if (this.week_reward1 == 0)
                    {
                        this.week_reward1 = Cfg.wID;
                        this.week_reward1_cost = Cfg.dwHuoYueDu;
                    }
                    else if (this.week_reward2 == 0)
                    {
                        this.week_reward2 = Cfg.wID;
                        this.week_reward2_cost = Cfg.dwHuoYueDu;
                    }
                }
                this.max_dayhuoyue_num = this.Calc_Day_Max_Num();
            }
        }

        public void PrintInfo(RES_HUOYUEDU_TYPE type)
        {
            List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
            uint num = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.week_curNum : this.day_curNum;
            object[] objArray1 = new object[] { "---ctask 活跃度数据: type:", type, ",值:", num, ",已领取奖励: " };
            string str = string.Concat(objArray1);
            for (int i = 0; i < list.Count; i++)
            {
                str = str + list[i] + ", ";
            }
        }

        public void Set(RES_HUOYUEDU_TYPE type, uint num, int length, ushort[] ary)
        {
            if (type == RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY)
            {
                this.day_curNum = num;
            }
            else if (type == RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK)
            {
                this.week_curNum = num;
            }
            List<ushort> list = (type != RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY) ? this.have_get_list_week : this.have_get_list_day;
            list.Clear();
            for (int i = 0; i < length; i++)
            {
                list.Add(ary[i]);
            }
            this.PrintInfo(type);
        }
    }
}

