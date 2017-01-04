namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class BurnExpeditionModel
    {
        public COMDT_BURNING_LEVEL_PROGRESS _data;
        private DictionaryView<uint, Dictionary<uint, int>> _hero_map = new DictionaryView<uint, Dictionary<uint, int>>();
        private DictionaryView<uint, COMDT_BURNING_LEVEL_DETAIL> _mapDetails = new DictionaryView<uint, COMDT_BURNING_LEVEL_DETAIL>();
        private DictionaryView<int, string> buff_icons = new DictionaryView<int, string>();
        private DictionaryView<int, string> buff_ids = new DictionaryView<int, string>();
        public int curSelect_BoxIndex = -1;
        public int curSelect_BuffIndex;
        public int curSelect_LevelIndex = -1;
        public int lastUnlockLevelIndex = -1;
        private ListView<int> levelRandomRobotIcon = new ListView<int>(6);
        private int[] robotIconInts;

        public BurnExpeditionModel()
        {
            this.curDifficultyType = EDifficultyType.Normal;
            GameDataMgr.burnBuffMap.Accept(new Action<ResBurningBuff>(this.OnVisit));
            this.robotIconInts = new int[] { 
                0x497fa, 0x49804, 0x4980e, 0x49818, 0x49822, 0x4982c, 0x49836, 0x49840, 0x4984a, 0x49854, 0x4985e, 0x49868, 0x49872, 0x4987c, 0x49886, 0x49890, 
                0x4989a, 0x498a4, 0x498d6, 0x498e0, 0x498ea, 0x498f4, 0x49912, 0x4991c, 0x49926, 0x4994e, 0x49980, 0x499b2, 0x499bc, 0x499d0, 0x497fa, 0x49804, 
                0x4980e, 0x49818, 0x49822, 0x4982c, 0x49836, 0x49840, 0x4984a, 0x49854, 0x4985e, 0x49868, 0x49872, 0x4987c, 0x49886, 0x49890, 0x4989a, 0x498a4, 
                0x498d6, 0x498e0, 0x498ea, 0x498f4, 0x49912, 0x4991c, 0x49926, 0x4994e, 0x49980, 0x499b2, 0x499bc, 0x499d0
             };
        }

        private COMDT_BURNING_LEVEL_DETAIL _getLevelDetail(EDifficultyType type)
        {
            if (this._mapDetails.ContainsKey((uint) type))
            {
                return this._mapDetails[(uint) type];
            }
            return null;
        }

        public void CalcProgress()
        {
            this.lastUnlockLevelIndex = this.Get_LastUnlockLevelIndex(this.curDifficultyType);
        }

        public bool Can_Reset()
        {
            return true;
        }

        public void ClearAll()
        {
            this._data = null;
            this.curSelect_LevelIndex = -1;
            this.curSelect_BoxIndex = -1;
            this.curSelect_BuffIndex = 0;
            this.lastUnlockLevelIndex = -1;
            this._hero_map.Clear();
            this._mapDetails.Clear();
        }

        public void FinishBox(int levelIndex)
        {
            this.Set_Box_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED);
        }

        public void FinishLevel(int levelIndex)
        {
            this.Set_Level_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED);
        }

        public bool Get_Box_Info(uint playerLevel, int levelIndex, out uint goldNum, out uint burnNum)
        {
            burnNum = 0;
            object[] rawDatas = GameDataMgr.burnRewrad.RawDatas;
            int length = rawDatas.Length;
            for (int i = 0; i < length; i++)
            {
                ResBurningReward reward = rawDatas[i] as ResBurningReward;
                if (((reward != null) && (reward.dwAcntLevel == playerLevel)) && (reward.iLevelID == BurnExpeditionUT.GetLevelCFGID(levelIndex)))
                {
                    goldNum = reward.dwRewardCoin;
                    burnNum = reward.dwBurningCoin;
                    return true;
                }
            }
            goldNum = 0;
            burnNum = 0;
            return false;
        }

        public string Get_Buff_Description(int buffid)
        {
            if (this.buff_ids.ContainsKey(buffid))
            {
                return this.buff_ids[buffid];
            }
            return string.Format(UT.GetText("Burn_Error_Find_Buff"), buffid);
        }

        public string Get_Buff_Icon(int buffid)
        {
            if (this.buff_icons.ContainsKey(buffid))
            {
                return this.buff_icons[buffid];
            }
            return string.Format(UT.GetText("Burn_Error_Find_Buff"), buffid);
        }

        public uint[] Get_Buffs(int levelIndex)
        {
            return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].stLuckyBuffDetail.SkillCombineID;
        }

        public COM_LEVEL_STATUS Get_ChestRewardStatus(int levelNo)
        {
            return (COM_LEVEL_STATUS) this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelNo].bRewardStatus;
        }

        public COMDT_BURNING_ENEMY_TEAM_INFO Get_CurLevel_ENEMY_TEAM_INFO()
        {
            return this.Get_ENEMY_TEAM_INFO(this.curDifficultyType, this.curSelect_LevelIndex);
        }

        public COMDT_PLAYERINFO Get_Current_Enemy_PlayerInfo()
        {
            COMDT_BURNING_ENEMY_TEAM_INFO comdt_burning_enemy_team_info = this.Get_CurLevel_ENEMY_TEAM_INFO();
            if (comdt_burning_enemy_team_info == null)
            {
                return null;
            }
            if (comdt_burning_enemy_team_info.bType == 1)
            {
                return comdt_burning_enemy_team_info.stDetail.stRealMan.stEnemyDetail;
            }
            return comdt_burning_enemy_team_info.stDetail.stRobot.stEnemyDetail;
        }

        public uint Get_CurSelected_BuffId()
        {
            return this.Get_Buffs(this.curSelect_LevelIndex)[this.curSelect_BuffIndex];
        }

        public List<uint> Get_Enemy_HeroIDS()
        {
            List<uint> list = new List<uint>(3);
            COMDT_PLAYERINFO comdt_playerinfo = this.Get_Current_Enemy_PlayerInfo();
            if (comdt_playerinfo != null)
            {
                for (int i = 0; i < comdt_playerinfo.astChoiceHero.Length; i++)
                {
                    COMDT_CHOICEHERO comdt_choicehero = comdt_playerinfo.astChoiceHero[i];
                    if ((comdt_choicehero != null) && (comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID != 0))
                    {
                        list.Add(comdt_choicehero.stBaseInfo.stCommonInfo.dwHeroID);
                    }
                }
            }
            return list;
        }

        public COMDT_BURNING_ENEMY_TEAM_INFO Get_ENEMY_TEAM_INFO(EDifficultyType type, int levelNo)
        {
            return this._getLevelDetail(type).astLevelDetail[levelNo].stEnemyDetail;
        }

        public int Get_HeroHP(uint heroCfgID)
        {
            return (int) ((((float) this.Get_HeroHP_Percent(heroCfgID)) / 10000f) * this.Get_HeroMaxHP(heroCfgID));
        }

        public int Get_HeroHP_Percent(uint heroCfgID)
        {
            Dictionary<uint, int> dictionary = this._hero_map[(uint) this.curDifficultyType];
            if ((dictionary != null) && dictionary.ContainsKey(heroCfgID))
            {
                return dictionary[heroCfgID];
            }
            return -1;
        }

        public int Get_HeroMaxHP(uint heroCfgID)
        {
            CHeroInfo info2;
            int totalValue = -1;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo.GetHeroInfo(heroCfgID, out info2, true))
            {
                if (info2 != null)
                {
                    totalValue = info2.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                }
                return totalValue;
            }
            if (masterRoleInfo.IsFreeHero(heroCfgID))
            {
                totalValue = ((CCustomHeroData) CHeroDataFactory.CreateCustomHeroData(heroCfgID)).heroMaxHP;
            }
            return totalValue;
        }

        public uint Get_LastPlayTime(EDifficultyType type)
        {
            return this._getLevelDetail(type).dwLastPlayTime;
        }

        public int Get_LastUnlockLevelIndex(EDifficultyType type)
        {
            COMDT_BURNING_LEVEL_INFO[] astLevelDetail = this._getLevelDetail(type).astLevelDetail;
            for (int i = this.Get_LevelNum(type) - 1; i >= 0; i--)
            {
                if (astLevelDetail[i].bLevelStatus == 1)
                {
                    return i;
                }
            }
            return -1;
        }

        public COMDT_BURNING_LEVEL_INFO[] Get_LevelArray(EDifficultyType type)
        {
            return this._getLevelDetail(type).astLevelDetail;
        }

        public int Get_LevelID(int levelIndex)
        {
            return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].iLevelID;
        }

        public int Get_LevelID(EDifficultyType type, int levelNo)
        {
            return this._getLevelDetail(type).astLevelDetail[levelNo].iLevelID;
        }

        public byte Get_LevelNo(int levelIndex)
        {
            return this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].bLevelNo;
        }

        public int Get_LevelNum(EDifficultyType type)
        {
            return this._getLevelDetail(type).bLevelNum;
        }

        public COM_LEVEL_STATUS Get_LevelStatus(int levelNo)
        {
            return (COM_LEVEL_STATUS) this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelNo].bLevelStatus;
        }

        public int Get_ResetNum(EDifficultyType type)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x2b);
            if (dataByKey == null)
            {
                return 0;
            }
            int dwConfValue = (int) dataByKey.dwConfValue;
            if (this._getLevelDetail(type) == null)
            {
                return 0;
            }
            return (dwConfValue - this._getLevelDetail(type).bResetNum);
        }

        public string GetPlayerHearUrl()
        {
            COMDT_BURNING_ENEMY_TEAM_INFO comdt_burning_enemy_team_info = this.Get_CurLevel_ENEMY_TEAM_INFO();
            if (comdt_burning_enemy_team_info.bType == 1)
            {
                return UT.Bytes2String(comdt_burning_enemy_team_info.stDetail.stRealMan.szHeadUrl);
            }
            return string.Empty;
        }

        public int GetRandomRobotIcon(int index)
        {
            return this.levelRandomRobotIcon[index];
        }

        public bool IsAllCompelte()
        {
            foreach (COMDT_BURNING_LEVEL_INFO comdt_burning_level_info in this._getLevelDetail(this.curDifficultyType).astLevelDetail)
            {
                if (comdt_burning_level_info == null)
                {
                    return false;
                }
                if (comdt_burning_level_info.bLevelStatus != 2)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsHeroInRecord(uint heroCfgID)
        {
            Dictionary<uint, int> dictionary = this._hero_map[(uint) this.curDifficultyType];
            if (dictionary == null)
            {
                return false;
            }
            return dictionary.ContainsKey(heroCfgID);
        }

        private void OnVisit(ResBurningBuff InBuff)
        {
            this.buff_ids.Add(InBuff.iSkillCombineID, StringHelper.UTF8BytesToString(ref InBuff.szBuffDesc));
            this.buff_icons.Add(InBuff.iSkillCombineID, StringHelper.UTF8BytesToString(ref InBuff.szBuffIcon));
        }

        public void RandomRobotIcon()
        {
            this.levelRandomRobotIcon.Clear();
            for (int i = 0; i < 6; i++)
            {
                int item = this.robotIconInts[UnityEngine.Random.Range(0, this.robotIconInts.Length)];
                this.levelRandomRobotIcon.Add(item);
            }
        }

        public void Record_HeroData()
        {
            for (int i = 0; i < this._data.bDiffNum; i++)
            {
                Dictionary<uint, int> dictionary = this._hero_map[(uint) i];
                COMDT_BURNING_LEVEL_DETAIL comdt_burning_level_detail = this._data.astDiffDetail[i];
                for (int j = 0; j < comdt_burning_level_detail.stHeroDetail.wHeroNum; j++)
                {
                    COMDT_BURNING_HERO_INFO comdt_burning_hero_info = comdt_burning_level_detail.stHeroDetail.astHeroList[j];
                    if (dictionary.ContainsKey(comdt_burning_hero_info.dwHeroID))
                    {
                        dictionary[comdt_burning_hero_info.dwHeroID] = (int) comdt_burning_hero_info.dwBloodTTH;
                    }
                    else
                    {
                        dictionary.Add(comdt_burning_hero_info.dwHeroID, (int) comdt_burning_hero_info.dwBloodTTH);
                    }
                }
            }
        }

        public void Reset_Data()
        {
            foreach (KeyValuePair<uint, Dictionary<uint, int>> pair in this._hero_map)
            {
                if (pair.Value != null)
                {
                    pair.Value.Clear();
                }
            }
            this.curSelect_LevelIndex = -1;
        }

        private void Set_Box_State(EDifficultyType type, int boxIndex, COM_LEVEL_STATUS state)
        {
            this._getLevelDetail(this.curDifficultyType).astLevelDetail[boxIndex].bRewardStatus = (byte) state;
        }

        public void Set_ENEMY_TEAM_INFO(EDifficultyType type, int levelNo, COMDT_BURNING_ENEMY_TEAM_INFO info)
        {
            this._getLevelDetail(type).astLevelDetail[levelNo].stEnemyDetail = info;
        }

        private void Set_Level_State(EDifficultyType type, int levelIndex, COM_LEVEL_STATUS state)
        {
            this._getLevelDetail(this.curDifficultyType).astLevelDetail[levelIndex].bLevelStatus = (byte) state;
        }

        public void SetHero_Hp(uint heroCfgID, int hp)
        {
            Dictionary<uint, int> dictionary = this._hero_map[(uint) this.curDifficultyType];
            if (dictionary.ContainsKey(heroCfgID))
            {
                dictionary[heroCfgID] = hp;
            }
            else
            {
                dictionary.Add(heroCfgID, hp);
            }
        }

        public void SetLevelDetail(EDifficultyType type, COMDT_BURNING_LEVEL_DETAIL detail)
        {
            if (!this._mapDetails.ContainsKey((uint) type))
            {
                this._mapDetails.Add((uint) type, detail);
            }
            else
            {
                this._mapDetails[(uint) type] = detail;
            }
        }

        public void SetProgress(COMDT_BURNING_LEVEL_PROGRESS data)
        {
            if (data != null)
            {
                this._data = data;
                this._mapDetails.Clear();
                for (int i = 0; i < data.bDiffNum; i++)
                {
                    COMDT_BURNING_LEVEL_DETAIL comdt_burning_level_detail = data.astDiffDetail[i];
                    EDifficultyType bDifficultType = (EDifficultyType) comdt_burning_level_detail.bDifficultType;
                    if (!this._mapDetails.ContainsKey((uint) bDifficultType))
                    {
                        this._mapDetails.Add((uint) bDifficultType, comdt_burning_level_detail);
                    }
                    if (!this._hero_map.ContainsKey((uint) bDifficultType))
                    {
                        this._hero_map.Add((uint) bDifficultType, new Dictionary<uint, int>());
                    }
                    Dictionary<uint, int> dictionary = this._hero_map[comdt_burning_level_detail.bDifficultType];
                    if (dictionary != null)
                    {
                        for (int j = 0; j < comdt_burning_level_detail.stHeroDetail.wHeroNum; j++)
                        {
                            COMDT_BURNING_HERO_INFO comdt_burning_hero_info = comdt_burning_level_detail.stHeroDetail.astHeroList[j];
                            if (dictionary.ContainsKey(comdt_burning_hero_info.dwHeroID))
                            {
                                dictionary[comdt_burning_hero_info.dwHeroID] = (int) comdt_burning_hero_info.dwBloodTTH;
                            }
                            else
                            {
                                dictionary.Add(comdt_burning_hero_info.dwHeroID, (int) comdt_burning_hero_info.dwBloodTTH);
                            }
                        }
                    }
                }
                this.CalcProgress();
            }
        }

        public void UnLockBox(int levelIndex)
        {
            this.Set_Box_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
        }

        public void UnLockLevel(int levelIndex)
        {
            this.Set_Level_State(this.curDifficultyType, levelIndex, COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
        }

        public EDifficultyType curDifficultyType { get; set; }

        public enum EDifficultyType
        {
            Hard = 2,
            Normal = 1
        }

        public class HeroData
        {
            public int HP;
            public int maxHP;
        }
    }
}

