namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CEquipRelationPath
    {
        public CEquipLineSet[] m_equipLineSets;
        public bool[][] m_equipNodes = new bool[3][];
        private Dictionary<ushort, Vector2> m_equipPositionDictionary = new Dictionary<ushort, Vector2>();

        public CEquipRelationPath()
        {
            for (int i = 0; i < this.m_equipNodes.Length; i++)
            {
                this.m_equipNodes[i] = new bool[8];
            }
            this.m_equipLineSets = new CEquipLineSet[2];
            for (int j = 0; j < this.m_equipLineSets.Length; j++)
            {
                this.m_equipLineSets[j] = new CEquipLineSet();
            }
        }

        public void Clear()
        {
            for (int i = 0; i < this.m_equipNodes.Length; i++)
            {
                for (int k = 0; k < this.m_equipNodes[i].Length; k++)
                {
                    this.m_equipNodes[i][k] = false;
                }
            }
            for (int j = 0; j < this.m_equipLineSets.Length; j++)
            {
                this.m_equipLineSets[j].Clear();
            }
            this.m_equipPositionDictionary.Clear();
        }

        public void Display(ushort equipID, List<ushort>[] equipList, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
        {
            this.Reset();
            this.SetEquipPositionDictionary(equipList);
            this.EnableEquipNode(equipID);
            this.EnablePreEquipNode(equipID, equipInfoDictionary);
            this.EnableBackEquipNode(equipID, equipInfoDictionary);
        }

        private void DisplayEquipLineSet(ushort equipID1, ushort equipID2)
        {
            Vector2 zero = Vector2.zero;
            Vector2 vector2 = Vector2.zero;
            if ((this.m_equipPositionDictionary.TryGetValue(equipID1, out zero) && this.m_equipPositionDictionary.TryGetValue(equipID2, out vector2)) && (Mathf.Abs((float) (zero.x - vector2.x)) == 1f))
            {
                Vector2 vector3 = (zero.x >= vector2.x) ? vector2 : zero;
                Vector2 vector4 = (zero.x >= vector2.x) ? zero : vector2;
                int x = (int) vector3.x;
                this.m_equipLineSets[x].DisplayHorizontalLine((int) vector3.y, CEquipLineSet.enHorizontalLineType.Left);
                this.m_equipLineSets[x].DisplayHorizontalLine((int) vector4.y, CEquipLineSet.enHorizontalLineType.Right);
                this.m_equipLineSets[x].DisplayVerticalLine((int) Mathf.Min(vector3.y, vector4.y), (int) Mathf.Max(vector3.y, vector4.y));
            }
        }

        private void EnableBackEquipNode(ushort equipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
        {
            CEquipInfo info = null;
            if (equipInfoDictionary.TryGetValue(equipID, out info) && (info.m_backEquipIDs != null))
            {
                for (int i = 0; i < info.m_backEquipIDs.Count; i++)
                {
                    this.EnableEquipNode(info.m_backEquipIDs[i]);
                    this.DisplayEquipLineSet(equipID, info.m_backEquipIDs[i]);
                    this.EnableBackEquipNode(info.m_backEquipIDs[i], equipInfoDictionary);
                }
            }
        }

        private void EnableEquipNode(ushort equipID)
        {
            Vector2 zero = Vector2.zero;
            if (((this.m_equipPositionDictionary.TryGetValue(equipID, out zero) && (zero.x >= 0f)) && ((zero.x < this.m_equipNodes.Length) && (zero.y >= 0f))) && (zero.y < this.m_equipNodes[(int) zero.x].Length))
            {
                this.m_equipNodes[(int) zero.x][(int) zero.y] = true;
            }
        }

        private void EnablePreEquipNode(ushort equipID, Dictionary<ushort, CEquipInfo> equipInfoDictionary)
        {
            CEquipInfo info = null;
            if (equipInfoDictionary.TryGetValue(equipID, out info) && (info.m_resEquipInBattle != null))
            {
                for (int i = 0; i < info.m_resEquipInBattle.PreEquipID.Length; i++)
                {
                    if (info.m_resEquipInBattle.PreEquipID[i] > 0)
                    {
                        this.EnableEquipNode(info.m_resEquipInBattle.PreEquipID[i]);
                        this.DisplayEquipLineSet(info.m_resEquipInBattle.PreEquipID[i], equipID);
                        this.EnablePreEquipNode(info.m_resEquipInBattle.PreEquipID[i], equipInfoDictionary);
                    }
                }
            }
        }

        public void InitializeHorizontalLine(int index, int row, CEquipLineSet.enHorizontalLineType type, GameObject gameObject)
        {
            this.m_equipLineSets[index].InitializeHorizontalLine(row, type, gameObject);
        }

        public void InitializeVerticalLine(int index, int startRow, int endRow, GameObject gameObject)
        {
            this.m_equipLineSets[index].InitializeVerticalLine(startRow, endRow, gameObject);
        }

        public void Reset()
        {
            for (int i = 0; i < this.m_equipNodes.Length; i++)
            {
                for (int k = 0; k < this.m_equipNodes[i].Length; k++)
                {
                    this.m_equipNodes[i][k] = false;
                }
            }
            for (int j = 0; j < this.m_equipLineSets.Length; j++)
            {
                this.m_equipLineSets[j].HideAllLines();
            }
            this.m_equipPositionDictionary.Clear();
        }

        private void SetEquipPositionDictionary(List<ushort>[] equipList)
        {
            for (int i = 0; i < equipList.Length; i++)
            {
                for (int j = 0; j < equipList[i].Count; j++)
                {
                    if ((equipList[i][j] > 0) && !this.m_equipPositionDictionary.ContainsKey(equipList[i][j]))
                    {
                        this.m_equipPositionDictionary.Add(equipList[i][j], new Vector2((float) i, (float) j));
                    }
                }
            }
        }
    }
}

