namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class CRecommendEquipInfo : IComparable
    {
        public ushort m_equipID;
        public bool m_hasBeenBought;
        public ResEquipInBattle m_resEquipInBattle;

        public CRecommendEquipInfo(ushort equipID, ResEquipInBattle resEquipInBattle)
        {
            this.m_equipID = equipID;
            this.m_resEquipInBattle = resEquipInBattle;
            this.m_hasBeenBought = false;
        }

        public int CompareTo(object obj)
        {
            CRecommendEquipInfo info = obj as CRecommendEquipInfo;
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
    }
}

