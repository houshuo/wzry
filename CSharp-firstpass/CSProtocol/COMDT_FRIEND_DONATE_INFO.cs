namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_FRIEND_DONATE_INFO : ProtocolObject
    {
        public COMDT_DONATE_ITEM[] astDonateAcntList = new COMDT_DONATE_ITEM[100];
        public static readonly uint BASEVERSION = 1;
        public byte bDonateNum;
        public byte bGetCoinNums;
        public static readonly int CLASS_ID = 0x204;
        public static readonly uint CURRVERSION = 0x7d;
        public uint dwLastDonateTime;
        public static readonly uint VERSION_bGetCoinNums = 0x7d;

        public COMDT_FRIEND_DONATE_INFO()
        {
            for (int i = 0; i < 100; i++)
            {
                this.astDonateAcntList[i] = (COMDT_DONATE_ITEM) ProtocolObjectPool.Get(COMDT_DONATE_ITEM.CLASS_ID);
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
            this.dwLastDonateTime = 0;
            this.bDonateNum = 0;
            this.bGetCoinNums = 0;
            if (this.astDonateAcntList != null)
            {
                for (int i = 0; i < this.astDonateAcntList.Length; i++)
                {
                    if (this.astDonateAcntList[i] != null)
                    {
                        this.astDonateAcntList[i].Release();
                        this.astDonateAcntList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astDonateAcntList != null)
            {
                for (int i = 0; i < this.astDonateAcntList.Length; i++)
                {
                    this.astDonateAcntList[i] = (COMDT_DONATE_ITEM) ProtocolObjectPool.Get(COMDT_DONATE_ITEM.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwLastDonateTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bDonateNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bGetCoinNums <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bGetCoinNums);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (100 < this.bDonateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astDonateAcntList.Length < this.bDonateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bDonateNum; i++)
                {
                    type = this.astDonateAcntList[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
            type = srcBuf.readUInt32(ref this.dwLastDonateTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bDonateNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bGetCoinNums <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bGetCoinNums);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bGetCoinNums = 0;
                }
                if (100 < this.bDonateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bDonateNum; i++)
                {
                    type = this.astDonateAcntList[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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

