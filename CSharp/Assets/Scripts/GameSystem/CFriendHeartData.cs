namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CFriendHeartData
    {
        private ListView<CDHeartData> _sendHeartList = new ListView<CDHeartData>();
        public static uint heartTimer_DoorValue = 0x1b7740;

        public void Add(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            if (this.GetFriendData(uniq, friendType) != null)
            {
                this.RemoveCDHeartData(uniq, friendType);
            }
            CDHeartData data = new CDHeartData {
                ullUid = uniq.ullUid,
                dwLogicWorldId = uniq.dwLogicWorldId,
                friendType = friendType,
                bCanSend = false
            };
            UT.Add2List<CDHeartData>(data, this._sendHeartList);
        }

        public bool BCanSendHeart(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            int heartDataIndex = this.GetHeartDataIndex(uniq, friendType);
            return ((heartDataIndex == -1) || this._sendHeartList[heartDataIndex].bCanSend);
        }

        public void Clear()
        {
            this._sendHeartList.Clear();
        }

        private CDHeartData GetFriendData(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            int heartDataIndex = this.GetHeartDataIndex(uniq, friendType);
            if (heartDataIndex == -1)
            {
                return null;
            }
            return this._sendHeartList[heartDataIndex];
        }

        private int GetHeartDataIndex(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            if (uniq != null)
            {
                CDHeartData data = null;
                for (int i = 0; i < this._sendHeartList.Count; i++)
                {
                    data = this._sendHeartList[i];
                    if (((data.ullUid == uniq.ullUid) && (data.dwLogicWorldId == uniq.dwLogicWorldId)) && (data.friendType == friendType))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private void RemoveCDHeartData(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            int heartDataIndex = this.GetHeartDataIndex(uniq, friendType);
            if (heartDataIndex != -1)
            {
                this._sendHeartList.RemoveAt(heartDataIndex);
            }
        }

        public class CDHeartData
        {
            public bool bCanSend;
            public uint dwLogicWorldId;
            public COM_FRIEND_TYPE friendType;
            public ulong ullUid;
        }
    }
}

