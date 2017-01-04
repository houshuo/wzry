namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stSymbolEventParams
    {
        public int page;
        public int pos;
        public CSymbolItem symbol;
    }
}

