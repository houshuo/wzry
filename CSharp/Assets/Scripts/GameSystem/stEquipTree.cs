namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stEquipTree
    {
        public ushort m_rootEquipID;
        public uint m_2ndEquipCount;
        public ushort[] m_2ndEquipIDs;
        public uint[] m_3rdEquipCounts;
        public ushort[][] m_3rdEquipIDs;
        public uint m_backEquipCount;
        public ushort[] m_backEquipIDs;
        public stEquipTree(int _2ndEquipMaxCount, int _3rdEquipMaxCountPer2ndEquip, int backEquipMaxCount)
        {
            this.m_rootEquipID = 0;
            this.m_2ndEquipCount = 0;
            this.m_2ndEquipIDs = new ushort[_2ndEquipMaxCount];
            this.m_3rdEquipCounts = new uint[_2ndEquipMaxCount];
            this.m_3rdEquipIDs = new ushort[_2ndEquipMaxCount][];
            for (int i = 0; i < _2ndEquipMaxCount; i++)
            {
                this.m_3rdEquipCounts[i] = 0;
                this.m_3rdEquipIDs[i] = new ushort[_3rdEquipMaxCountPer2ndEquip];
            }
            this.m_backEquipCount = 0;
            this.m_backEquipIDs = new ushort[backEquipMaxCount];
        }

        public void Clear()
        {
            this.m_rootEquipID = 0;
            this.m_2ndEquipCount = 0;
            for (int i = 0; i < this.m_2ndEquipIDs.Length; i++)
            {
                this.m_2ndEquipIDs[i] = 0;
            }
            for (int j = 0; j < this.m_3rdEquipCounts.Length; j++)
            {
                this.m_3rdEquipCounts[j] = 0;
            }
            for (int k = 0; k < this.m_3rdEquipIDs.Length; k++)
            {
                for (int n = 0; n < this.m_3rdEquipIDs[k].Length; n++)
                {
                    this.m_3rdEquipIDs[k][n] = 0;
                }
            }
            this.m_backEquipCount = 0;
            for (int m = 0; m < this.m_backEquipIDs.Length; m++)
            {
                this.m_backEquipIDs[m] = 0;
            }
        }

        public void Create(ushort rootEquipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
        {
            this.Clear();
            if (rootEquipID != 0)
            {
                this.m_rootEquipID = rootEquipID;
                CEquipInfo info = null;
                if (equipInfoDictionary.TryGetValue(rootEquipID, out info))
                {
                    for (int i = 0; i < info.m_resEquipInBattle.PreEquipID.Length; i++)
                    {
                        if (info.m_resEquipInBattle.PreEquipID[i] > 0)
                        {
                            this.m_2ndEquipIDs[this.m_2ndEquipCount] = info.m_resEquipInBattle.PreEquipID[i];
                            this.m_2ndEquipCount++;
                        }
                    }
                    for (int j = 0; j < this.m_2ndEquipCount; j++)
                    {
                        CEquipInfo info2 = null;
                        if (equipInfoDictionary.TryGetValue(this.m_2ndEquipIDs[j], out info2))
                        {
                            for (int k = 0; k < info2.m_resEquipInBattle.PreEquipID.Length; k++)
                            {
                                if (info2.m_resEquipInBattle.PreEquipID[k] > 0)
                                {
                                    this.m_3rdEquipIDs[j][this.m_3rdEquipCounts[j]] = info2.m_resEquipInBattle.PreEquipID[k];
                                    this.m_3rdEquipCounts[j]++;
                                }
                            }
                        }
                    }
                }
                this.AppendBackEquipIDs(rootEquipID, equipInfoDictionary, ref this.m_backEquipCount, ref this.m_backEquipIDs);
            }
        }

        private void AppendBackEquipIDs(ushort equipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary, ref uint backEquipIDTotalCount, ref ushort[] backEquipIDs)
        {
            if (equipID != 0)
            {
                CEquipInfo info = null;
                if ((equipInfoDictionary.TryGetValue(equipID, out info) && (info.m_backEquipIDs != null)) && (info.m_backEquipIDs.Count > 0))
                {
                    for (int i = 0; i < info.m_backEquipIDs.Count; i++)
                    {
                        backEquipIDs[backEquipIDTotalCount] = info.m_backEquipIDs[i];
                        backEquipIDTotalCount++;
                    }
                }
            }
        }
    }
}

