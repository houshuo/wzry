namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stEquipInfo
    {
        public ushort m_equipID;
        public uint m_amount;
        public uint m_maxAmount;
    }
}

