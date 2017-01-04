namespace Assets.Scripts.GameLogic
{
    using System;

    public class CEquipBuffInfoGroup
    {
        public ListView<CEquipBuffInfo> m_equipBuffInfos = new ListView<CEquipBuffInfo>();
        public ushort m_groupID;
        public bool m_isChanged;

        public CEquipBuffInfoGroup(ushort groupID)
        {
            this.m_groupID = groupID;
            this.m_isChanged = false;
        }
    }
}

