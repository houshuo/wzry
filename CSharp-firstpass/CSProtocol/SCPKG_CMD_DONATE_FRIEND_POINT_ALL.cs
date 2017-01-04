namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_CMD_DONATE_FRIEND_POINT_ALL : ProtocolObject
    {
        public COMDT_ACNT_UNIQ[] astFriendList = new COMDT_ACNT_UNIQ[220];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x360;
        public static readonly uint CURRVERSION = 1;
        public uint dwDonateSuccNum;
        public uint dwResult;

        public SCPKG_CMD_DONATE_FRIEND_POINT_ALL()
        {
            for (int i = 0; i < 220; i++)
            {
                this.astFriendList[i] = (COMDT_ACNT_UNIQ) ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.dwDonateSuccNum = 0;
            if (this.astFriendList != null)
            {
                for (int i = 0; i < this.astFriendList.Length; i++)
                {
                    if (this.astFriendList[i] != null)
                    {
                        this.astFriendList[i].Release();
                        this.astFriendList[i] = null;
                    }
                }
            }
            this.dwResult = 0;
        }

        public override void OnUse()
        {
            if (this.astFriendList != null)
            {
                for (int i = 0; i < this.astFriendList.Length; i++)
                {
                    this.astFriendList[i] = (COMDT_ACNT_UNIQ) ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
                }
            }
        }

        public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = destBuf.writeUInt32(this.dwDonateSuccNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (220 < this.dwDonateSuccNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astFriendList.Length < this.dwDonateSuccNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwDonateSuccNum; i++)
                {
                    type = this.astFriendList[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt32(ref this.dwDonateSuccNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (220 < this.dwDonateSuccNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwDonateSuccNum; i++)
                {
                    type = this.astFriendList[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

