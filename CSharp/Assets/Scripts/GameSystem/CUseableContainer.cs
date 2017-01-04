namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CUseableContainer : CContainer
    {
        private ListView<CUseable> m_useableList = new ListView<CUseable>();
        private const int WEIGHT_ITEM_GET_TIME = 1;
        private const ulong WEIGHT_ITEM_TYPE = 0x174876e800L;
        private const ulong WEIGHT_ITEM_TYPE_TYPE = 0x2540be400L;

        public CUseableContainer(enCONTAINER_TYPE type)
        {
            this.Init(type);
        }

        public void Add(CUseable useable)
        {
            this.m_useableList.Add(useable);
        }

        public CUseable Add(COM_ITEM_TYPE useableType, ulong objID, uint baseID, int iCount, int addTime)
        {
            CUseable useableByObjID = null;
            if (((useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) || (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)) || (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL))
            {
                useableByObjID = this.GetUseableByObjID(objID);
                if (useableByObjID == null)
                {
                    CUseable useable = CUseableManager.CreateUseable(useableType, objID, baseID, iCount, addTime);
                    this.Add(useable);
                    return useable;
                }
                useableByObjID.m_stackCount += iCount;
                useableByObjID.ResetTime();
            }
            return useableByObjID;
        }

        public void Clear()
        {
            this.m_useableList.Clear();
        }

        private void ComputeSortItemValue()
        {
            for (int i = 0; i < this.m_useableList.Count; i++)
            {
                if (this.m_useableList[i] != null)
                {
                    CUseable useable = this.m_useableList[i];
                    useable.m_itemSortNum = 0L;
                    useable.m_itemSortNum += (ulong) ((9 - useable.m_type) * 0x174876e800L);
                    if (useable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item = useable as CItem;
                        useable.m_itemSortNum += (4L - item.m_itemData.bType) * ((ulong) 0x2540be400L);
                    }
                    useable.m_itemSortNum += useable.m_getTime * ((ulong) 1L);
                    if ((useable.m_itemSortNum >= ulong.MaxValue) || (useable.m_itemSortNum <= 0L))
                    {
                        useable.m_itemSortNum = 0L;
                    }
                }
            }
        }

        public int GetCurUseableCount()
        {
            return this.m_useableList.Count;
        }

        public int GetMaxAddTime()
        {
            int addTime = 0;
            CUseable useable = null;
            if (this.m_useableList != null)
            {
                for (int i = 0; i < this.m_useableList.Count; i++)
                {
                    useable = this.m_useableList[i];
                    if ((useable != null) && (useable.m_addTime > addTime))
                    {
                        addTime = useable.m_addTime;
                    }
                }
            }
            return addTime;
        }

        public CUseable GetUseableByBaseID(COM_ITEM_TYPE itemType, uint baseID)
        {
            if (this.m_useableList != null)
            {
                for (int i = 0; i < this.m_useableList.Count; i++)
                {
                    CUseable useable2 = this.m_useableList[i];
                    if (((useable2 != null) && (useable2.m_baseID == baseID)) && (useable2.m_type == itemType))
                    {
                        return useable2;
                    }
                }
            }
            return null;
        }

        public CUseable GetUseableByIndex(int index)
        {
            if ((this.m_useableList.Count > index) && (index >= 0))
            {
                return this.m_useableList[index];
            }
            return null;
        }

        public CUseable GetUseableByObjID(ulong objID)
        {
            if (this.m_useableList != null)
            {
                for (int i = 0; i < this.m_useableList.Count; i++)
                {
                    CUseable useable2 = this.m_useableList[i];
                    if ((useable2 != null) && (useable2.m_objID == objID))
                    {
                        return useable2;
                    }
                }
            }
            return null;
        }

        public int GetUseableStackCount(COM_ITEM_TYPE itemType, uint baseID)
        {
            int num = 0;
            if (this.m_useableList != null)
            {
                for (int i = 0; i < this.m_useableList.Count; i++)
                {
                    CUseable useable = this.m_useableList[i];
                    if (((useable != null) && (useable.m_type == itemType)) && (useable.m_baseID == baseID))
                    {
                        num += useable.m_stackCount;
                    }
                }
            }
            return num;
        }

        public int GetUsebableIndexByUid(ulong uid)
        {
            for (int i = 0; i < this.m_useableList.Count; i++)
            {
                if (this.m_useableList[i].m_objID == uid)
                {
                    return i;
                }
            }
            return -1;
        }

        public override void Init(enCONTAINER_TYPE type)
        {
            base.m_type = type;
            this.m_useableList.Clear();
        }

        public void Remove(CUseable useable)
        {
            if ((this.m_useableList != null) && (useable != null))
            {
                this.m_useableList.Remove(useable);
            }
        }

        public void Remove(ulong objID, int iCount)
        {
            CUseable useableByObjID = null;
            useableByObjID = this.GetUseableByObjID(objID);
            if (useableByObjID != null)
            {
                useableByObjID.m_stackCount -= iCount;
                if (useableByObjID.m_stackCount <= 0)
                {
                    this.Remove(useableByObjID);
                }
            }
        }

        public void SortItemBag()
        {
            this.ComputeSortItemValue();
            this.SortItemBySortItemValue();
        }

        private void SortItemBySortItemValue()
        {
            for (int i = 0; i < this.m_useableList.Count; i++)
            {
                for (int j = i + 1; j < this.m_useableList.Count; j++)
                {
                    CUseable useable2 = this.m_useableList[i];
                    CUseable useable3 = this.m_useableList[j];
                    if (useable2.m_itemSortNum < useable3.m_itemSortNum)
                    {
                        CUseable useable = this.m_useableList[i];
                        this.m_useableList[i] = this.m_useableList[j];
                        this.m_useableList[j] = useable;
                    }
                }
            }
        }
    }
}

