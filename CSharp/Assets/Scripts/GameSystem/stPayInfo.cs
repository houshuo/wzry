namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stPayInfo
    {
        public enPayType m_payType;
        public uint m_payValue;
        public uint m_oriValue;
        public uint m_discountForDisplay;
    }
}

