namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct stBattleEquipParams
    {
        public CEquipInfo equipInfo;
        public Transform equipItemObj;
        public int pos;
        public int m_indexInQuicklyBuyList;
    }
}

