namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    public class CFriendReCallData
    {
        private ListView<CDFriendReCallData> _reCallList = new ListView<CDFriendReCallData>();
        private static uint inviteLimitSec;

        public void Add(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            if (this.GetFriendData(uniq, friendType) != null)
            {
                this.RemoveFriendReCallData(uniq, friendType);
            }
            CDFriendReCallData data = new CDFriendReCallData {
                ullUid = uniq.ullUid,
                dwLogicWorldId = uniq.dwLogicWorldId,
                friendType = friendType
            };
            UT.Add2List<CDFriendReCallData>(data, this._reCallList);
        }

        public bool BInCd(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            return (this.GetFriendReCallDataIndex(uniq, friendType) != -1);
        }

        public static bool BLose(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            COMDT_FRIEND_INFO comdt_friend_info = null;
            if (friendType == COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME)
            {
                comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(uniq.ullUid, CFriendModel.FriendType.GameFriend);
            }
            else if (friendType == COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS)
            {
                comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(uniq.ullUid, CFriendModel.FriendType.SNS);
            }
            return ((comdt_friend_info != null) && ((CRoleInfo.GetCurrentUTCTime() - comdt_friend_info.dwLastLoginTime) >= InviteLimitSec));
        }

        public void Clear()
        {
            this._reCallList.Clear();
        }

        private CDFriendReCallData GetFriendData(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            int friendReCallDataIndex = this.GetFriendReCallDataIndex(uniq, friendType);
            if (friendReCallDataIndex == -1)
            {
                return null;
            }
            return this._reCallList[friendReCallDataIndex];
        }

        private int GetFriendReCallDataIndex(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            if (uniq != null)
            {
                CDFriendReCallData data = null;
                for (int i = 0; i < this._reCallList.Count; i++)
                {
                    data = this._reCallList[i];
                    if (((data.ullUid == uniq.ullUid) && (data.dwLogicWorldId == uniq.dwLogicWorldId)) && (data.friendType == friendType))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private void RemoveFriendReCallData(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            int friendReCallDataIndex = this.GetFriendReCallDataIndex(uniq, friendType);
            if (friendReCallDataIndex != -1)
            {
                this._reCallList.RemoveAt(friendReCallDataIndex);
            }
        }

        private static uint InviteLimitSec
        {
            get
            {
                if (inviteLimitSec == 0)
                {
                    inviteLimitSec = 0x15180 * GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9e).dwConfValue;
                }
                return inviteLimitSec;
            }
        }

        public class CDFriendReCallData
        {
            public uint dwLogicWorldId;
            public COM_FRIEND_TYPE friendType;
            public ulong ullUid;
        }
    }
}

