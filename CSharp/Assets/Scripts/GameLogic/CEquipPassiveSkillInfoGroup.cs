namespace Assets.Scripts.GameLogic
{
    using System;

    public class CEquipPassiveSkillInfoGroup
    {
        public ListView<CEquipPassiveSkillInfo> m_equipPassiveSkillInfos = new ListView<CEquipPassiveSkillInfo>();
        public ushort m_groupID;
        public bool m_isChanged;

        public CEquipPassiveSkillInfoGroup(ushort groupID)
        {
            this.m_groupID = groupID;
            this.m_isChanged = false;
        }
    }
}

