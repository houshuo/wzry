namespace Assets.Scripts.GameLogic
{
    using System;

    public class CEquipBuffInfo : IComparable
    {
        public ushort m_buffGroupID;
        public uint m_buffID;
        public CrypticInt32 m_equipBuyPrice;
        public ushort m_equipID;
        public bool m_isEnabled;
        public bool m_isNeedRemoved;
        public uint m_sequence;

        public CEquipBuffInfo(ushort equipID, uint equipBuyPrice, uint buffID, ushort buffGroupID, uint sequence)
        {
            this.m_equipID = equipID;
            this.m_equipBuyPrice = (CrypticInt32) equipBuyPrice;
            this.m_buffID = buffID;
            this.m_buffGroupID = buffGroupID;
            this.m_sequence = sequence;
            this.m_isEnabled = false;
            this.m_isNeedRemoved = false;
        }

        public int CompareTo(object obj)
        {
            CEquipBuffInfo info = obj as CEquipBuffInfo;
            if ((this.m_buffID == info.m_buffID) && (this.m_isEnabled != info.m_isEnabled))
            {
                return (!this.m_isEnabled ? -1 : 1);
            }
            if (((uint) this.m_equipBuyPrice) > ((uint) info.m_equipBuyPrice))
            {
                return 1;
            }
            if (((uint) this.m_equipBuyPrice) == ((uint) info.m_equipBuyPrice))
            {
                if (this.m_sequence > info.m_sequence)
                {
                    return 1;
                }
                if (this.m_sequence == info.m_sequence)
                {
                    return 0;
                }
            }
            return -1;
        }

        public bool IsEqual(uint buffID, ushort buffGroupID)
        {
            return ((this.m_buffID == buffID) && (this.m_buffGroupID == buffGroupID));
        }
    }
}

