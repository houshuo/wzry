namespace Assets.Scripts.GameSystem
{
    using System;

    public class CBreakSymbolItem : IComparable
    {
        public bool bBreak;
        public bool bBreakToggle;
        public CSymbolItem symbol;

        public CBreakSymbolItem(CSymbolItem tempSymbol, bool btempBreak)
        {
            this.symbol = tempSymbol;
            this.bBreak = btempBreak;
            this.bBreakToggle = btempBreak;
        }

        public int CompareTo(object obj)
        {
            CBreakSymbolItem item = obj as CBreakSymbolItem;
            if (this.symbol.m_SymbolData.wLevel < item.symbol.m_SymbolData.wLevel)
            {
                return 1;
            }
            if (this.symbol.m_SymbolData.wLevel > item.symbol.m_SymbolData.wLevel)
            {
                return -1;
            }
            if (this.symbol.m_SymbolData.bColor != item.symbol.m_SymbolData.bColor)
            {
                return ((this.symbol.m_SymbolData.bColor <= item.symbol.m_SymbolData.bColor) ? -1 : 1);
            }
            if (this.symbol.m_SymbolData.dwID == item.symbol.m_SymbolData.dwID)
            {
                return 0;
            }
            return ((this.symbol.m_SymbolData.dwID <= item.symbol.m_SymbolData.dwID) ? -1 : 1);
        }
    }
}

