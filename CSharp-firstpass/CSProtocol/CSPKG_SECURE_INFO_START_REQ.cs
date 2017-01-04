namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPKG_SECURE_INFO_START_REQ : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x26b;
        public static readonly uint CURRVERSION = 1;
        public uint dwClientStartTime;
        public int iSvrBossAttackMax;
        public int iSvrBossAttackMin;
        public int iSvrBossCount;
        public int iSvrBossHPMax;
        public int iSvrBossHPMin;
        public int iSvrBossHPTotal;
        public int iSvrBuildingAttackMax;
        public int iSvrBuildingAttackMin;
        public int iSvrBuildingHPMax;
        public int iSvrBuildingHPMin;
        public int iSvrEmenyAttackMax;
        public int iSvrEmenyAttackMin;
        public int iSvrEmenyBattlePoint;
        public int iSvrEmenyBuildingAttackMax;
        public int iSvrEmenyBuildingAttackMin;
        public int iSvrEmenyBuildingHPMax;
        public int iSvrEmenyBuildingHPMin;
        public int iSvrEmenyCardATN1;
        public int iSvrEmenyCardATN2;
        public int iSvrEmenyCardATN3;
        public int iSvrEmenyCardHP1;
        public int iSvrEmenyCardHP2;
        public int iSvrEmenyCardHP3;
        public int iSvrEmenyCardID1;
        public int iSvrEmenyCardID2;
        public int iSvrEmenyCardID3;
        public int iSvrEmenyCardINT1;
        public int iSvrEmenyCardINT2;
        public int iSvrEmenyCardINT3;
        public int iSvrEmenyCardPhyDef1;
        public int iSvrEmenyCardPhyDef2;
        public int iSvrEmenyCardPhyDef3;
        public int iSvrEmenyCardSpeed1;
        public int iSvrEmenyCardSpeed2;
        public int iSvrEmenyCardSpeed3;
        public int iSvrEmenyCardSpellDef1;
        public int iSvrEmenyCardSpellDef2;
        public int iSvrEmenyCardSpellDef3;
        public int iSvrEmenyHPMax;
        public int iSvrEmenyHPMin;
        public int iSvrEmenyHPTotal;
        public int iSvrUserBattlePoint;
        public int iSvrUserCardATN1;
        public int iSvrUserCardATN2;
        public int iSvrUserCardATN3;
        public int iSvrUserCardHP1;
        public int iSvrUserCardHP2;
        public int iSvrUserCardHP3;
        public int iSvrUserCardID1;
        public int iSvrUserCardID2;
        public int iSvrUserCardID3;
        public int iSvrUserCardINT1;
        public int iSvrUserCardINT2;
        public int iSvrUserCardINT3;
        public int iSvrUserCardPhyDef1;
        public int iSvrUserCardPhyDef2;
        public int iSvrUserCardPhyDef3;
        public int iSvrUserCardSpeed1;
        public int iSvrUserCardSpeed2;
        public int iSvrUserCardSpeed3;
        public int iSvrUserCardSpellDef1;
        public int iSvrUserCardSpellDef2;
        public int iSvrUserCardSpellDef3;

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
            this.dwClientStartTime = 0;
            this.iSvrBossCount = 0;
            this.iSvrBossHPMax = 0;
            this.iSvrBossHPMin = 0;
            this.iSvrBossHPTotal = 0;
            this.iSvrEmenyBuildingHPMax = 0;
            this.iSvrEmenyBuildingHPMin = 0;
            this.iSvrEmenyHPTotal = 0;
            this.iSvrEmenyHPMax = 0;
            this.iSvrEmenyHPMin = 0;
            this.iSvrBossAttackMax = 0;
            this.iSvrBossAttackMin = 0;
            this.iSvrEmenyBuildingAttackMax = 0;
            this.iSvrEmenyBuildingAttackMin = 0;
            this.iSvrEmenyAttackMax = 0;
            this.iSvrEmenyAttackMin = 0;
            this.iSvrBuildingHPMax = 0;
            this.iSvrBuildingHPMin = 0;
            this.iSvrBuildingAttackMax = 0;
            this.iSvrBuildingAttackMin = 0;
            this.iSvrUserBattlePoint = 0;
            this.iSvrUserCardID1 = 0;
            this.iSvrUserCardHP1 = 0;
            this.iSvrUserCardATN1 = 0;
            this.iSvrUserCardINT1 = 0;
            this.iSvrUserCardSpeed1 = 0;
            this.iSvrUserCardPhyDef1 = 0;
            this.iSvrUserCardSpellDef1 = 0;
            this.iSvrUserCardID2 = 0;
            this.iSvrUserCardHP2 = 0;
            this.iSvrUserCardATN2 = 0;
            this.iSvrUserCardINT2 = 0;
            this.iSvrUserCardSpeed2 = 0;
            this.iSvrUserCardPhyDef2 = 0;
            this.iSvrUserCardSpellDef2 = 0;
            this.iSvrUserCardID3 = 0;
            this.iSvrUserCardHP3 = 0;
            this.iSvrUserCardATN3 = 0;
            this.iSvrUserCardINT3 = 0;
            this.iSvrUserCardSpeed3 = 0;
            this.iSvrUserCardPhyDef3 = 0;
            this.iSvrUserCardSpellDef3 = 0;
            this.iSvrEmenyBattlePoint = 0;
            this.iSvrEmenyCardID1 = 0;
            this.iSvrEmenyCardHP1 = 0;
            this.iSvrEmenyCardATN1 = 0;
            this.iSvrEmenyCardINT1 = 0;
            this.iSvrEmenyCardSpeed1 = 0;
            this.iSvrEmenyCardPhyDef1 = 0;
            this.iSvrEmenyCardSpellDef1 = 0;
            this.iSvrEmenyCardID2 = 0;
            this.iSvrEmenyCardHP2 = 0;
            this.iSvrEmenyCardATN2 = 0;
            this.iSvrEmenyCardINT2 = 0;
            this.iSvrEmenyCardSpeed2 = 0;
            this.iSvrEmenyCardPhyDef2 = 0;
            this.iSvrEmenyCardSpellDef2 = 0;
            this.iSvrEmenyCardID3 = 0;
            this.iSvrEmenyCardHP3 = 0;
            this.iSvrEmenyCardATN3 = 0;
            this.iSvrEmenyCardINT3 = 0;
            this.iSvrEmenyCardSpeed3 = 0;
            this.iSvrEmenyCardPhyDef3 = 0;
            this.iSvrEmenyCardSpellDef3 = 0;
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
            type = destBuf.writeUInt32(this.dwClientStartTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeInt32(this.iSvrBossCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBossHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBossHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBossHPTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyHPTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBossAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBossAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyBuildingAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyBuildingAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBuildingAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrBuildingAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserBattlePoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardHP1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardATN1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardINT1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpeed1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardPhyDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpellDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardHP2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardATN2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardINT2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpeed2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardPhyDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpellDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardID3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardHP3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardATN3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardINT3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpeed3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardPhyDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrUserCardSpellDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyBattlePoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardHP1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardATN1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardINT1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpeed1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardPhyDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpellDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardHP2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardATN2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardINT2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpeed2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardPhyDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpellDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardID3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardHP3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardATN3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardINT3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpeed3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardPhyDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iSvrEmenyCardSpellDef3);
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
            type = srcBuf.readUInt32(ref this.dwClientStartTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iSvrBossCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBossHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBossHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBossHPTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyHPTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBossAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBossAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyBuildingAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyBuildingAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBuildingAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrBuildingAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserBattlePoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardHP1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardATN1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardINT1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpeed1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardPhyDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpellDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardHP2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardATN2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardINT2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpeed2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardPhyDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpellDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardID3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardHP3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardATN3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardINT3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpeed3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardPhyDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrUserCardSpellDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyBattlePoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardHP1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardATN1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardINT1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpeed1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardPhyDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpellDef1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardHP2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardATN2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardINT2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpeed2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardPhyDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpellDef2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardID3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardHP3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardATN3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardINT3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpeed3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardPhyDef3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSvrEmenyCardSpellDef3);
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

