namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACHIEVEMENT_INFO : ProtocolObject
    {
        public COMDT_ACHIEVEMENT_DATA[] astAchievementData = new COMDT_ACHIEVEMENT_DATA[400];
        public COMDT_ACHIEVEMENT_DONE_DATA[] astDoneData;
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x201;
        public static readonly uint CURRVERSION = 0x29;
        public uint dwAchievementNum;
        public uint dwAchievePoint;
        public uint dwDoneTypeNum;
        public static readonly uint VERSION_dwAchievePoint = 0x29;

        public COMDT_ACHIEVEMENT_INFO()
        {
            for (int i = 0; i < 400; i++)
            {
                this.astAchievementData[i] = (COMDT_ACHIEVEMENT_DATA) ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DATA.CLASS_ID);
            }
            this.astDoneData = new COMDT_ACHIEVEMENT_DONE_DATA[100];
            for (int j = 0; j < 100; j++)
            {
                this.astDoneData[j] = (COMDT_ACHIEVEMENT_DONE_DATA) ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DONE_DATA.CLASS_ID);
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
            this.dwAchievementNum = 0;
            if (this.astAchievementData != null)
            {
                for (int i = 0; i < this.astAchievementData.Length; i++)
                {
                    if (this.astAchievementData[i] != null)
                    {
                        this.astAchievementData[i].Release();
                        this.astAchievementData[i] = null;
                    }
                }
            }
            this.dwDoneTypeNum = 0;
            if (this.astDoneData != null)
            {
                for (int j = 0; j < this.astDoneData.Length; j++)
                {
                    if (this.astDoneData[j] != null)
                    {
                        this.astDoneData[j].Release();
                        this.astDoneData[j] = null;
                    }
                }
            }
            this.dwAchievePoint = 0;
        }

        public override void OnUse()
        {
            if (this.astAchievementData != null)
            {
                for (int i = 0; i < this.astAchievementData.Length; i++)
                {
                    this.astAchievementData[i] = (COMDT_ACHIEVEMENT_DATA) ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DATA.CLASS_ID);
                }
            }
            if (this.astDoneData != null)
            {
                for (int j = 0; j < this.astDoneData.Length; j++)
                {
                    this.astDoneData[j] = (COMDT_ACHIEVEMENT_DONE_DATA) ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DONE_DATA.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwAchievementNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (400 < this.dwAchievementNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAchievementData.Length < this.dwAchievementNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwAchievementNum; i++)
                {
                    type = this.astAchievementData[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwDoneTypeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (100 < this.dwDoneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astDoneData.Length < this.dwDoneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.dwDoneTypeNum; j++)
                {
                    type = this.astDoneData[j].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwAchievePoint <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwAchievePoint);
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
            type = srcBuf.readUInt32(ref this.dwAchievementNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (400 < this.dwAchievementNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwAchievementNum; i++)
                {
                    type = this.astAchievementData[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwDoneTypeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (100 < this.dwDoneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.dwDoneTypeNum; j++)
                {
                    type = this.astDoneData[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwAchievePoint <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwAchievePoint);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwAchievePoint = 0;
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

