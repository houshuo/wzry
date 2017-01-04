namespace Assets.Scripts.GameLogic
{
    using System;

    public class SKILLSTATISTICTINFO
    {
        public float fLastUseTime = 0f;
        public int iadValue = 0;
        public int iapValue = 0;
        public int iAttackDistanceMax = 0;
        public int ihemoFadeRate = 0;
        public int iHitAllHurtTotalMax = 0;
        public int iHitAllHurtTotalMin = -1;
        public int iHitCount = 0;
        public int iHitCountMax = 0;
        public int iHitCountMin = -1;
        public int ihpValue = 0;
        public int ihurtCount = 0;
        public int iHurtMax = 0;
        public int iHurtMin = -1;
        public int iHurtToHeroTotal = 0;
        public int iHurtTotal = 0;
        public int ihurtValue = 0;
        public int iloseHpValue = 0;
        public int iSkillCfgID = 0;
        public int iTmpHitAllHurtCountIndex = 0;
        public int iTmpHitAllHurtTotal = 0;
        public uint uiCDIntervalMax = 0;
        public uint uiCDIntervalMin = uint.MaxValue;
        public uint uiUsedTimes = 0;

        public SKILLSTATISTICTINFO(int i)
        {
        }
    }
}

