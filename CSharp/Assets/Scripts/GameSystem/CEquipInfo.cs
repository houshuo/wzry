namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class CEquipInfo : IComparable
    {
        public List<ushort> m_backEquipIDs;
        public string[] m_equipBuffDescs = new string[3];
        public string m_equipDesc;
        public string m_equipIconPath;
        public ushort m_equipID;
        public string m_equipName;
        public string m_equipPropertyDesc;
        public ushort[] m_requiredEquipIDs;
        public ResEquipInBattle m_resEquipInBattle;

        public CEquipInfo(ushort equipID)
        {
            this.m_equipID = equipID;
            this.m_resEquipInBattle = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if (this.m_resEquipInBattle != null)
            {
                this.m_equipName = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szName);
                this.m_equipDesc = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szDesc);
                this.m_equipIconPath = CUIUtility.s_Sprite_System_BattleEquip_Dir + StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.szIcon);
                for (int i = 0; i < 3; i++)
                {
                    this.m_equipBuffDescs[i] = StringHelper.UTF8BytesToString(ref this.m_resEquipInBattle.astEffectCombine[i].szDesc);
                }
                this.m_requiredEquipIDs = this.GetRequiredEquipIDs(this.m_resEquipInBattle);
                this.m_equipPropertyDesc = string.Empty;
                string equipPropertyValueDesc = this.GetEquipPropertyValueDesc(this.m_resEquipInBattle);
                if (!string.IsNullOrEmpty(equipPropertyValueDesc))
                {
                    this.m_equipPropertyDesc = this.m_equipPropertyDesc + equipPropertyValueDesc + "\n";
                }
                string equipPassiveSkillDesc = this.GetEquipPassiveSkillDesc(this.m_resEquipInBattle);
                string equipPassiveEftDesc = this.GetEquipPassiveEftDesc(this.m_resEquipInBattle);
                string str4 = equipPassiveSkillDesc + equipPassiveEftDesc;
                if (!string.IsNullOrEmpty(str4))
                {
                    this.m_equipPropertyDesc = this.m_equipPropertyDesc + str4;
                }
            }
        }

        public void AddBackEquipID(ushort backEquipID)
        {
            if (this.m_backEquipIDs == null)
            {
                this.m_backEquipIDs = new List<ushort>();
            }
            if (!this.m_backEquipIDs.Contains(backEquipID))
            {
                this.m_backEquipIDs.Add(backEquipID);
            }
        }

        public int CompareTo(object obj)
        {
            CEquipInfo info = obj as CEquipInfo;
            if (this.m_resEquipInBattle.dwBuyPrice > info.m_resEquipInBattle.dwBuyPrice)
            {
                return -1;
            }
            if (this.m_resEquipInBattle.dwBuyPrice == info.m_resEquipInBattle.dwBuyPrice)
            {
                if (this.m_equipID > info.m_equipID)
                {
                    return -1;
                }
                if (this.m_equipID == info.m_equipID)
                {
                    return 0;
                }
            }
            return 1;
        }

        private string GetEquipPassiveEftDesc(ResEquipInBattle resEquipInBattle)
        {
            if (resEquipInBattle == null)
            {
                return string.Empty;
            }
            string str = string.Empty;
            for (int i = 0; i < resEquipInBattle.astEffectCombine.Length; i++)
            {
                if (!string.IsNullOrEmpty(resEquipInBattle.astEffectCombine[i].szDesc))
                {
                    str = str + resEquipInBattle.astEffectCombine[i].szDesc + "\n";
                }
            }
            return str;
        }

        private string GetEquipPassiveSkillDesc(ResEquipInBattle resEquipInBattle)
        {
            if (resEquipInBattle == null)
            {
                return string.Empty;
            }
            string str = string.Empty;
            for (int i = 0; i < resEquipInBattle.astPassiveSkill.Length; i++)
            {
                if (!string.IsNullOrEmpty(resEquipInBattle.astPassiveSkill[i].szDesc))
                {
                    str = str + resEquipInBattle.astPassiveSkill[i].szDesc + "\n";
                }
            }
            return str;
        }

        private string GetEquipPropertyValueDesc(ResEquipInBattle resEquipInBattle)
        {
            if (resEquipInBattle == null)
            {
                return string.Empty;
            }
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            string str = string.Empty;
            if (resEquipInBattle.dwPhyAttack > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwPhyAttack, instance.GetText("Hero_Prop_PhyAtkPt"));
            }
            if (resEquipInBattle.dwAttackSpeed > 0)
            {
                str = string.Format("{0}+{1}% {2}\n", str, resEquipInBattle.dwAttackSpeed / 100, instance.GetText("Hero_Prop_AtkSpd"));
            }
            if (resEquipInBattle.dwCriticalHit > 0)
            {
                str = string.Format("{0}+{1}% {2}\n", str, resEquipInBattle.dwCriticalHit / 100, instance.GetText("Hero_Prop_CritRate"));
            }
            if (resEquipInBattle.dwHealthSteal > 0)
            {
                str = string.Format("{0}+{1}% {2}\n", str, resEquipInBattle.dwHealthSteal / 100, instance.GetText("Hero_Prop_PhyVamp"));
            }
            if (resEquipInBattle.dwMagicAttack > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwMagicAttack, instance.GetText("Hero_Prop_MgcAtkPt"));
            }
            if (resEquipInBattle.dwCDReduce > 0)
            {
                str = string.Format("{0}+{1}% {2}\n", str, resEquipInBattle.dwCDReduce / 100, instance.GetText("Hero_Prop_CdReduce"));
            }
            if (resEquipInBattle.dwMagicPoint > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwMagicPoint, instance.GetText("Hero_Prop_MaxEp"));
            }
            if (resEquipInBattle.dwMagicRecover > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwMagicRecover, instance.GetText("Hero_Prop_EpRecover"));
            }
            if (resEquipInBattle.dwPhyDefence > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwPhyDefence, instance.GetText("Hero_Prop_PhyDefPt"));
            }
            if (resEquipInBattle.dwMagicDefence > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwMagicDefence, instance.GetText("Hero_Prop_MgcDefPt"));
            }
            if (resEquipInBattle.dwHealthPoint > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwHealthPoint, instance.GetText("Hero_Prop_MaxHp"));
            }
            if (resEquipInBattle.dwHealthRecover > 0)
            {
                str = string.Format("{0}+{1} {2}\n", str, resEquipInBattle.dwHealthRecover, instance.GetText("Hero_Prop_HpRecover"));
            }
            if (resEquipInBattle.dwMoveSpeed > 0)
            {
                str = string.Format("{0}+{1}% {2}\n", str, resEquipInBattle.dwMoveSpeed / 100, instance.GetText("Hero_Prop_MoveSpd"));
            }
            return str;
        }

        private ushort[] GetRequiredEquipIDs(ResEquipInBattle resEquipInBattle)
        {
            string str = StringHelper.UTF8BytesToString(ref resEquipInBattle.szRequireEquip);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            char[] separator = new char[] { ',' };
            string[] strArray = str.Split(separator);
            ushort[] numArray = new ushort[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                try
                {
                    numArray[i] = ushort.Parse(strArray[i].Trim());
                }
                catch (Exception)
                {
                    numArray[i] = 0;
                }
            }
            return numArray;
        }
    }
}

