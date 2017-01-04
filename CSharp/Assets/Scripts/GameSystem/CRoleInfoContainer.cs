namespace Assets.Scripts.GameSystem
{
    using System;

    public class CRoleInfoContainer : CContainer
    {
        private ListView<CRoleInfo> m_roleInfoList = new ListView<CRoleInfo>();

        private void Add(CRoleInfo roleInfo)
        {
            if (this.m_roleInfoList != null)
            {
                this.m_roleInfoList.Add(roleInfo);
            }
        }

        public void AddRoleInfo(CRoleInfo roleInfo)
        {
            this.Add(roleInfo);
        }

        public ulong AddRoleInfoByType(enROLEINFO_TYPE roleType, ulong uuID)
        {
            CRoleInfo roleInfo = new CRoleInfo(roleType, uuID);
            if (roleInfo != null)
            {
                this.Add(roleInfo);
            }
            return uuID;
        }

        public void Clear()
        {
            this.m_roleInfoList.Clear();
        }

        public CRoleInfo FindRoleInfoByID(ulong uuID)
        {
            if (this.m_roleInfoList != null)
            {
                for (int i = 0; i < this.m_roleInfoList.Count; i++)
                {
                    if ((this.m_roleInfoList[i] != null) && (this.m_roleInfoList[i].playerUllUID == uuID))
                    {
                        return this.m_roleInfoList[i];
                    }
                }
            }
            return null;
        }

        public int GetContainerSize()
        {
            if (this.m_roleInfoList != null)
            {
                return this.m_roleInfoList.Count;
            }
            return 0;
        }

        public CRoleInfo GetRoleInfoByIndex(int index)
        {
            if (((this.m_roleInfoList != null) && (this.m_roleInfoList.Count > 0)) && (index < this.m_roleInfoList.Count))
            {
                return this.m_roleInfoList[index];
            }
            return null;
        }

        private void Remove(CRoleInfo roleInfo)
        {
            if (this.m_roleInfoList != null)
            {
                this.m_roleInfoList.Remove(roleInfo);
            }
        }

        public void RemoveRoleInfoByType(enROLEINFO_TYPE roleType)
        {
            if ((this.m_roleInfoList != null) && (this.m_roleInfoList.Count > 0))
            {
                CRoleInfo roleInfo = null;
                int num = 0;
                while (num < this.m_roleInfoList.Count)
                {
                    roleInfo = this.m_roleInfoList[num];
                    if ((roleInfo != null) && (roleInfo.m_roleType == roleType))
                    {
                        this.Remove(roleInfo);
                    }
                    else
                    {
                        num++;
                    }
                }
            }
        }

        public void RemoveRoleInfoByUUID(ulong uuid)
        {
            if ((this.m_roleInfoList != null) && (this.m_roleInfoList.Count > 0))
            {
                CRoleInfo roleInfo = null;
                for (int i = 0; i < this.m_roleInfoList.Count; i++)
                {
                    roleInfo = this.m_roleInfoList[i];
                    if ((roleInfo != null) && (roleInfo.playerUllUID == uuid))
                    {
                        break;
                    }
                }
                this.Remove(roleInfo);
            }
        }
    }
}

