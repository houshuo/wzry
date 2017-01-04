namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CSkillData
    {
        private uint m_heroCfgId;
        public uint SelSkillID;
        private int[] skillIdArr = new int[5];
        private CrypticInt32[] skillLevelArr = new CrypticInt32[5];
        private static readonly string[] slotBgStrs = new string[] { "Common_Bg_Physicalbg", "Common_Bg_Spellbg", "Common_Bg_Realbg", "Common_Bg_Controlbg" };

        public static int CalcEscapeValue(string escapeString, CHeroInfo heroInfo, int skillLevel, int heroSoulLevel = 1)
        {
            char ch;
            string str = string.Empty;
            str2 = string.Empty;
            char[] separator = new char[] { '+', '-' };
            string[] strArray = escapeString.Split(separator);
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            List<char> list3 = new List<char>();
            List<char> list4 = new List<char>();
            for (int i = 0; i < strArray.Length; i++)
            {
                list2.Clear();
                list4.Clear();
                str = strArray[i];
                char[] chArray2 = new char[] { '*', '/' };
                foreach (string str2 in str.Split(chArray2))
                {
                    int num3;
                    if (int.TryParse(str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParsePlayerProperty(heroInfo, str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseSkillProperty(str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseSkillLevel(str2, out num3, skillLevel))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseHeroSoulLevel(str2, out num3, heroSoulLevel))
                    {
                        list2.Add(num3);
                    }
                    else
                    {
                        object[] inParameters = new object[] { str2 };
                        DebugHelper.Assert(false, "Skill Data Desc[{0}] can not be parsed..", inParameters);
                        return 0;
                    }
                }
                for (int m = 0; m < str.Length; m++)
                {
                    ch = str[m];
                    if (ch.Equals('*') || ch.Equals('/'))
                    {
                        list4.Add(ch);
                    }
                }
                int item = list2[0];
                for (int n = 0; n < list4.Count; n++)
                {
                    switch (list4[n])
                    {
                        case '*':
                            item *= list2[n + 1];
                            break;

                        case '/':
                            item /= list2[n + 1];
                            break;
                    }
                }
                list.Add(item);
            }
            for (int j = 0; j < escapeString.Length; j++)
            {
                ch = escapeString[j];
                if (ch.Equals('+') || ch.Equals('-'))
                {
                    list3.Add(ch);
                }
            }
            int num = list[0];
            for (int k = 0; k < list3.Count; k++)
            {
                switch (list3[k])
                {
                    case '+':
                        num += list[k + 1];
                        break;

                    case '-':
                        num -= list[k + 1];
                        break;
                }
            }
            return num;
        }

        public static int CalcEscapeValue(string escapeString, ValueDataInfo[] valueData, int skillLevel, int heroSoulLevel = 1)
        {
            char ch;
            string str = string.Empty;
            str2 = string.Empty;
            char[] separator = new char[] { '+', '-' };
            string[] strArray = escapeString.Split(separator);
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            List<char> list3 = new List<char>();
            List<char> list4 = new List<char>();
            for (int i = 0; i < strArray.Length; i++)
            {
                list2.Clear();
                list4.Clear();
                str = strArray[i];
                char[] chArray2 = new char[] { '*', '/' };
                foreach (string str2 in str.Split(chArray2))
                {
                    int num3;
                    if (int.TryParse(str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParsePlayerProperty(valueData, str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseSkillProperty(str2, out num3))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseSkillLevel(str2, out num3, skillLevel))
                    {
                        list2.Add(num3);
                    }
                    else if (ParseHeroSoulLevel(str2, out num3, heroSoulLevel))
                    {
                        list2.Add(num3);
                    }
                    else
                    {
                        object[] inParameters = new object[] { str2 };
                        DebugHelper.Assert(false, "Skill Data Desc[{0}] can not be parsed..", inParameters);
                        return 0;
                    }
                }
                for (int m = 0; m < str.Length; m++)
                {
                    ch = str[m];
                    if (ch.Equals('*') || ch.Equals('/'))
                    {
                        list4.Add(ch);
                    }
                }
                int item = list2[0];
                for (int n = 0; n < list4.Count; n++)
                {
                    switch (list4[n])
                    {
                        case '*':
                            item *= list2[n + 1];
                            break;

                        case '/':
                            item /= list2[n + 1];
                            break;
                    }
                }
                list.Add(item);
            }
            for (int j = 0; j < escapeString.Length; j++)
            {
                ch = escapeString[j];
                if (ch.Equals('+') || ch.Equals('-'))
                {
                    list3.Add(ch);
                }
            }
            int num = list[0];
            for (int k = 0; k < list3.Count; k++)
            {
                switch (list3[k])
                {
                    case '+':
                        num += list[k + 1];
                        break;

                    case '-':
                        num -= list[k + 1];
                        break;
                }
            }
            return num;
        }

        public unsafe stSkillData CreateSkillData(int slotId)
        {
            stSkillData data;
            data.slotId = slotId;
            data.skillId = this.skillIdArr[slotId];
            data.cfgInfo = GameDataMgr.skillDatabin.GetDataByKey((long) this.skillIdArr[slotId]);
            data.level = *((int*) &(this.skillLevelArr[slotId]));
            data.cfgLevelUpInfo = GameDataMgr.skillLvlUpDatabin.GetDataByKey((uint) this.skillLevelArr[slotId]);
            data.descTip = this.GetSkillDesc(slotId);
            data.levelUpTip = this.GetSkillUpTip(slotId);
            return data;
        }

        public void CreateSvrSkillInfo(ref COMDT_SKILLARRAY svrSkillArr)
        {
            if (svrSkillArr != null)
            {
                for (int i = 0; i < this.skillLevelArr.Length; i++)
                {
                    svrSkillArr.astSkillInfo[i].bUnlocked = (this.skillLevelArr[i] <= 0) ? ((byte) 0) : ((byte) 1);
                    svrSkillArr.astSkillInfo[i].wLevel = (ushort) this.skillLevelArr[i];
                }
            }
        }

        public unsafe int GetCombatEft()
        {
            int num = 0;
            for (int i = 0; i < this.skillIdArr.Length; i++)
            {
                if ((this.skillIdArr[i] > 0) && (this.skillLevelArr[i] > 0))
                {
                    num += GetSkillCombatEft(this.skillIdArr[i], *((int*) &(this.skillLevelArr[i])));
                }
            }
            return num;
        }

        public static string GetEffectDesc(SkillEffectType skillEffectType)
        {
            uint num = (uint) skillEffectType;
            return Singleton<CTextManager>.instance.GetText(string.Format("{0}{1}", "Skill_Common_Effect_Type_", num.ToString()));
        }

        public static string GetEffectSlotBg(SkillEffectType skillEffectType)
        {
            return string.Format("{0}{1}", "UGUI/Sprite/Common/", slotBgStrs[(int) ((IntPtr) (skillEffectType - 1))]);
        }

        public static string[] GetEscapeString(string skillDesc)
        {
            if (skillDesc == null)
            {
                return null;
            }
            List<string> list = new List<string>();
            int index = skillDesc.IndexOf("[");
            for (int i = skillDesc.IndexOf("]"); (index != -1) && (i != -1); i = skillDesc.IndexOf("]", (int) (i + 1)))
            {
                list.Add(skillDesc.Substring(index + 1, (i - index) - 1));
                index = skillDesc.IndexOf("[", (int) (i + 1));
            }
            return list.ToArray();
        }

        public static ResSkillCfgInfo GetSkillCfgInfo(int skillId)
        {
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) skillId);
            if (dataByKey == null)
            {
                return null;
            }
            return dataByKey;
        }

        public static int GetSkillCombatEft(int skillId, int level)
        {
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) skillId);
            if (((dataByKey != null) && (dataByKey.iSkillCombatType >= 1)) && (dataByKey.iSkillCombatType <= 5))
            {
                ResSkillLvlUpInfo info2 = GameDataMgr.skillLvlUpDatabin.GetDataByKey((uint) level);
                if (info2 != null)
                {
                    return info2.SkillCombatEft[dataByKey.iSkillCombatType - 1];
                }
            }
            return 0;
        }

        public string GetSkillDesc(int slotId)
        {
            CHeroInfo info2;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfo(this.m_heroCfgId, out info2, true);
            DebugHelper.Assert(info2 != null);
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) this.skillIdArr[slotId]);
            DebugHelper.Assert(dataByKey != null);
            string skillDesc = StringHelper.UTF8BytesToString(ref dataByKey.szSkillDesc);
            string[] escapeString = GetEscapeString(skillDesc);
            if (escapeString != null)
            {
                for (int i = 0; i < escapeString.Length; i++)
                {
                    skillDesc = skillDesc.Replace("[" + escapeString[i] + "]", CalcEscapeValue(escapeString[i], info2, this.GetSkillLevel(slotId), 1).ToString());
                }
            }
            return skillDesc;
        }

        public int GetSkillId(int slotId)
        {
            return this.skillIdArr[slotId];
        }

        public unsafe int GetSkillLevel(int slotId)
        {
            return *(((int*) &(this.skillLevelArr[slotId])));
        }

        public static int GetSkillMaxLevel()
        {
            return GameDataMgr.skillLvlUpDatabin.Count();
        }

        public string GetSkillUpTip(int slotId)
        {
            CHeroInfo info2;
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfo(this.m_heroCfgId, out info2, true);
            DebugHelper.Assert(info2 != null);
            ResSkillCfgInfo dataByKey = GameDataMgr.skillDatabin.GetDataByKey((long) this.skillIdArr[slotId]);
            DebugHelper.Assert(dataByKey != null);
            string skillDesc = StringHelper.UTF8BytesToString(ref dataByKey.szSkillUpTip);
            string[] escapeString = GetEscapeString(skillDesc);
            if (escapeString != null)
            {
                for (int i = 0; i < escapeString.Length; i++)
                {
                    int num2 = CalcEscapeValue(escapeString[i], info2, this.GetSkillLevel(slotId), 1);
                    string newValue = string.Empty;
                    if (num2 != 0)
                    {
                        newValue = num2.ToString();
                    }
                    skillDesc = skillDesc.Replace("[" + escapeString[i] + "]", newValue);
                }
            }
            return skillDesc;
        }

        public void InitSkillData(ResHeroCfgInfo heroCfgInfo, COMDT_SKILLARRAY svrSkillArr)
        {
            if (heroCfgInfo != null)
            {
                this.m_heroCfgId = heroCfgInfo.dwCfgID;
                for (int j = 0; j < heroCfgInfo.astSkill.Length; j++)
                {
                    this.skillIdArr[j] = heroCfgInfo.astSkill[j].iSkillID;
                }
            }
            for (int i = 0; i < svrSkillArr.astSkillInfo.Length; i++)
            {
                if (svrSkillArr.astSkillInfo[i].bUnlocked > 0)
                {
                    this.skillLevelArr[i] = (CrypticInt32) svrSkillArr.astSkillInfo[i].wLevel;
                }
                else
                {
                    this.skillLevelArr[i] = 0;
                }
            }
            this.SelSkillID = svrSkillArr.dwSelSkillID;
        }

        private static bool ParseHeroSoulLevel(string s, out int value, int heroSoulLevel)
        {
            value = 0;
            if ((s[0] == 'h') && (s[1] == 'l'))
            {
                value = heroSoulLevel;
                return true;
            }
            return false;
        }

        private static bool ParsePlayerProperty(CHeroInfo heroInfo, string s, out int value)
        {
            value = 0;
            if (s[0] == 'k')
            {
                RES_FUNCEFT_TYPE res_funceft_type = (RES_FUNCEFT_TYPE) Convert.ToInt32(s.Substring(1));
                value = heroInfo.mActorValue[res_funceft_type].totalValue;
                return true;
            }
            return false;
        }

        private static bool ParsePlayerProperty(ValueDataInfo[] valueData, string s, out int value)
        {
            value = 0;
            if (s[0] == 'k')
            {
                int index = Convert.ToInt32(s.Substring(1));
                value = valueData[index].totalValue;
                return true;
            }
            return false;
        }

        private static bool ParseSkillLevel(string s, out int value, int skillLevel)
        {
            value = 0;
            if ((s[0] == 's') && (s[1] == 'l'))
            {
                value = skillLevel;
                return true;
            }
            return false;
        }

        private static bool ParseSkillProperty(string s, out int value)
        {
            value = 0;
            int index = s.IndexOf('p');
            int num2 = s.IndexOf('q');
            int num3 = s.IndexOf('g');
            int length = s.IndexOf('t');
            int num5 = s.IndexOf('a');
            int num6 = s.IndexOf('b');
            if (index != -1)
            {
                if (num2 != -1)
                {
                    int num7 = Convert.ToInt32(s.Substring(0, index));
                    int num8 = Convert.ToInt32(s.Substring(index + 1, (num2 - index) - 1));
                    int num9 = Convert.ToInt32(s.Substring(num2 + 1));
                    ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) num7);
                    object[] inParameters = new object[] { num7, s };
                    DebugHelper.Assert(dataByKey != null, "ResSkillCombineCfgInfo[{0}] can not be found! from string:\"{1}\"", inParameters);
                    if (dataByKey != null)
                    {
                        value = dataByKey.astSkillFuncInfo[num8 - 1].astSkillFuncParam[num9 - 1].iParam;
                    }
                    return true;
                }
                if (num3 != -1)
                {
                    int num10 = Convert.ToInt32(s.Substring(0, index));
                    int num11 = Convert.ToInt32(s.Substring(index + 1, (num3 - index) - 1));
                    int num12 = Convert.ToInt32(s.Substring(num3 + 1));
                    ResSkillCombineCfgInfo info2 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) num10);
                    DebugHelper.Assert(info2 != null);
                    if (info2 != null)
                    {
                        value = info2.astSkillFuncInfo[num11 - 1].astSkillFuncGroup[num12 - 1].iParam;
                    }
                    return true;
                }
            }
            else if (length != -1)
            {
                if (num5 != -1)
                {
                    int num13 = Convert.ToInt32(s.Substring(0, length));
                    ResSkillCombineCfgInfo info3 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) num13);
                    DebugHelper.Assert(info3 != null);
                    if (info3 != null)
                    {
                        value = info3.iDuration;
                    }
                    return true;
                }
                if (num6 != -1)
                {
                    int num14 = Convert.ToInt32(s.Substring(0, length));
                    ResSkillCombineCfgInfo info4 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) num14);
                    DebugHelper.Assert(info4 != null);
                    if (info4 != null)
                    {
                        value = info4.iDurationGrow;
                    }
                    return true;
                }
            }
            return false;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            for (int i = 1; i <= 4; i++)
            {
                preloadTab.AddSprite(GetEffectSlotBg((SkillEffectType) i));
            }
        }

        public void SetSkillLevel(int slotId, int level)
        {
            this.skillLevelArr[slotId] = level;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("HeroSkillLevelChange", this.m_heroCfgId);
        }

        public void UnLockSkill(CSDT_UNLOCKSKILL unlockSkill)
        {
            for (int i = 0; i < unlockSkill.bUnlockCnt; i++)
            {
                int slotId = unlockSkill.szSkillSlot[i];
                this.SetSkillLevel(slotId, 1);
            }
        }

        public void UnLockSkill(int slotId)
        {
            this.SetSkillLevel(slotId, 1);
        }
    }
}

