namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using System.Text;
    using tsf4g_tdr_csharp;

    public class COMDT_CLIENT_INFO_DATA : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1f8;
        public static readonly uint CURRVERSION = 1;
        public int iLoginChannel;
        public int iMemorySize;
        public static readonly uint LENGTH_szClientVersion = 0x40;
        public static readonly uint LENGTH_szCltBuildNumber = 0x40;
        public static readonly uint LENGTH_szCltSvnVersion = 0x40;
        public static readonly uint LENGTH_szNetwork = 0x40;
        public static readonly uint LENGTH_szSystemHardware = 0x40;
        public static readonly uint LENGTH_szSystemSoftware = 0x40;
        public static readonly uint LENGTH_szTelecomOper = 0x40;
        public byte[] szClientVersion = new byte[0x40];
        public byte[] szCltBuildNumber = new byte[0x40];
        public byte[] szCltSvnVersion = new byte[0x40];
        public byte[] szNetwork = new byte[0x40];
        public byte[] szSystemHardware = new byte[0x40];
        public byte[] szSystemSoftware = new byte[0x40];
        public byte[] szTelecomOper = new byte[0x40];

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            string s = "NULL";
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            if ((bytes.GetLength(0) + 1) > this.szSystemSoftware.GetLength(0))
            {
                if (bytes.GetLength(0) >= LENGTH_szSystemSoftware)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szSystemSoftware = new byte[bytes.GetLength(0) + 1];
            }
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.szSystemSoftware[i] = bytes[i];
            }
            this.szSystemSoftware[bytes.GetLength(0)] = 0;
            string str2 = "NULL";
            byte[] buffer2 = Encoding.ASCII.GetBytes(str2);
            if ((buffer2.GetLength(0) + 1) > this.szSystemHardware.GetLength(0))
            {
                if (buffer2.GetLength(0) >= LENGTH_szSystemHardware)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szSystemHardware = new byte[buffer2.GetLength(0) + 1];
            }
            for (int j = 0; j < buffer2.GetLength(0); j++)
            {
                this.szSystemHardware[j] = buffer2[j];
            }
            this.szSystemHardware[buffer2.GetLength(0)] = 0;
            string str3 = "NULL";
            byte[] buffer3 = Encoding.ASCII.GetBytes(str3);
            if ((buffer3.GetLength(0) + 1) > this.szTelecomOper.GetLength(0))
            {
                if (buffer3.GetLength(0) >= LENGTH_szTelecomOper)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szTelecomOper = new byte[buffer3.GetLength(0) + 1];
            }
            for (int k = 0; k < buffer3.GetLength(0); k++)
            {
                this.szTelecomOper[k] = buffer3[k];
            }
            this.szTelecomOper[buffer3.GetLength(0)] = 0;
            string str4 = "NULL";
            byte[] buffer4 = Encoding.ASCII.GetBytes(str4);
            if ((buffer4.GetLength(0) + 1) > this.szNetwork.GetLength(0))
            {
                if (buffer4.GetLength(0) >= LENGTH_szNetwork)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szNetwork = new byte[buffer4.GetLength(0) + 1];
            }
            for (int m = 0; m < buffer4.GetLength(0); m++)
            {
                this.szNetwork[m] = buffer4[m];
            }
            this.szNetwork[buffer4.GetLength(0)] = 0;
            this.iLoginChannel = 0;
            string str5 = "NULL";
            byte[] buffer5 = Encoding.ASCII.GetBytes(str5);
            if ((buffer5.GetLength(0) + 1) > this.szClientVersion.GetLength(0))
            {
                if (buffer5.GetLength(0) >= LENGTH_szClientVersion)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szClientVersion = new byte[buffer5.GetLength(0) + 1];
            }
            for (int n = 0; n < buffer5.GetLength(0); n++)
            {
                this.szClientVersion[n] = buffer5[n];
            }
            this.szClientVersion[buffer5.GetLength(0)] = 0;
            this.iMemorySize = 0;
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
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
            int pos = destBuf.getUsedSize();
            type = destBuf.reserve(4);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szSystemSoftware);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szSystemSoftware, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = destBuf.getUsedSize() - num2;
                type = destBuf.writeUInt32((uint) num4, pos);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = destBuf.getUsedSize();
                int num7 = TdrTypeUtil.cstrlen(this.szSystemHardware);
                if (num7 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szSystemHardware, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = destBuf.getUsedSize() - num6;
                type = destBuf.writeUInt32((uint) num8, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num10 = destBuf.getUsedSize();
                int num11 = TdrTypeUtil.cstrlen(this.szTelecomOper);
                if (num11 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szTelecomOper, num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num12 = destBuf.getUsedSize() - num10;
                type = destBuf.writeUInt32((uint) num12, num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num13 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num14 = destBuf.getUsedSize();
                int num15 = TdrTypeUtil.cstrlen(this.szNetwork);
                if (num15 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szNetwork, num15);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num16 = destBuf.getUsedSize() - num14;
                type = destBuf.writeUInt32((uint) num16, num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iLoginChannel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num17 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num18 = destBuf.getUsedSize();
                int num19 = TdrTypeUtil.cstrlen(this.szClientVersion);
                if (num19 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szClientVersion, num19);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num20 = destBuf.getUsedSize() - num18;
                type = destBuf.writeUInt32((uint) num20, num17);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iMemorySize);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num21 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num22 = destBuf.getUsedSize();
                int num23 = TdrTypeUtil.cstrlen(this.szCltBuildNumber);
                if (num23 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szCltBuildNumber, num23);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num24 = destBuf.getUsedSize() - num22;
                type = destBuf.writeUInt32((uint) num24, num21);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num25 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num26 = destBuf.getUsedSize();
                int num27 = TdrTypeUtil.cstrlen(this.szCltSvnVersion);
                if (num27 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szCltSvnVersion, num27);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num28 = destBuf.getUsedSize() - num26;
                type = destBuf.writeUInt32((uint) num28, num25);
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
            uint dest = 0;
            type = srcBuf.readUInt32(ref dest);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (dest > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (dest > this.szSystemSoftware.GetLength(0))
                {
                    if (dest > LENGTH_szSystemSoftware)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSystemSoftware = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSystemSoftware, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSystemSoftware[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szSystemSoftware) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num3 = 0;
                type = srcBuf.readUInt32(ref num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num3 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num3 > this.szSystemHardware.GetLength(0))
                {
                    if (num3 > LENGTH_szSystemHardware)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSystemHardware = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSystemHardware, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSystemHardware[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szSystemHardware) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num5 = 0;
                type = srcBuf.readUInt32(ref num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num5 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num5 > this.szTelecomOper.GetLength(0))
                {
                    if (num5 > LENGTH_szTelecomOper)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTelecomOper = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTelecomOper, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szTelecomOper[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szTelecomOper) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num7 = 0;
                type = srcBuf.readUInt32(ref num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num7 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num7 > this.szNetwork.GetLength(0))
                {
                    if (num7 > LENGTH_szNetwork)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szNetwork = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szNetwork, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szNetwork[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szNetwork) + 1;
                if (num7 != num8)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iLoginChannel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num9 = 0;
                type = srcBuf.readUInt32(ref num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num9 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num9 > this.szClientVersion.GetLength(0))
                {
                    if (num9 > LENGTH_szClientVersion)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szClientVersion = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szClientVersion, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szClientVersion[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szClientVersion) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iMemorySize);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num11 = 0;
                type = srcBuf.readUInt32(ref num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num11 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num11 > this.szCltBuildNumber.GetLength(0))
                {
                    if (num11 > LENGTH_szCltBuildNumber)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCltBuildNumber = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCltBuildNumber, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCltBuildNumber[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szCltBuildNumber) + 1;
                if (num11 != num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num13 = 0;
                type = srcBuf.readUInt32(ref num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num13 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num13 > this.szCltSvnVersion.GetLength(0))
                {
                    if (num13 > LENGTH_szCltSvnVersion)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCltSvnVersion = new byte[num13];
                }
                if (1 > num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCltSvnVersion, (int) num13);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szCltSvnVersion[((int) num13) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num14 = TdrTypeUtil.cstrlen(this.szCltSvnVersion) + 1;
                    if (num13 != num14)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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

