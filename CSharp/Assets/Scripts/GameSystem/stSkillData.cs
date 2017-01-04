namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stSkillData
    {
        public int slotId;
        public int skillId;
        public int level;
        public string descTip;
        public string levelUpTip;
        public ResSkillCfgInfo cfgInfo;
        public ResSkillLvlUpInfo cfgLevelUpInfo;
    }
}

