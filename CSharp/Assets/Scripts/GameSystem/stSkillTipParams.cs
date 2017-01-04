namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stSkillTipParams
    {
        public string strTipText;
        public string strTipTitle;
        public string skillName;
        public uint[] skillEffect;
        public string skillCoolDown;
        public string skillEnergyCost;
        public int itemIndex;
    }
}

