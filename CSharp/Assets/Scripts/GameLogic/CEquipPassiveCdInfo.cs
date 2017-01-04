namespace Assets.Scripts.GameLogic
{
    using System;

    public class CEquipPassiveCdInfo
    {
        public int m_passiveCd;
        public uint m_passiveSkillId;

        public CEquipPassiveCdInfo(uint passiveSkillID, int cd)
        {
            this.m_passiveSkillId = passiveSkillID;
            this.m_passiveCd = cd;
        }
    }
}

