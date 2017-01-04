namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResDT_LevelCommonInfo : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_ExpCompensateInfo[] astExpCompensateDetail = new ResDT_ExpCompensateInfo[5];
        public static readonly uint BASEVERSION = 1;
        public byte bBattleEquipLimit;
        public byte bBirthLevelConfig;
        public byte bCameraFlip;
        public byte bChaosPickRule;
        public byte bDealHangUp;
        public byte bFinResultType;
        public byte bHeadPtsUpperLimit;
        public byte bHeroNum;
        public byte bIsAllowHeroDup;
        public byte bIsEnableFow;
        public byte bIsEnableOrnamentSlot;
        public byte bIsEnableShopHorizonTab;
        public byte bIsOpenExpCompensate;
        public byte bMaxAcntNum;
        public byte bRandPickHero;
        public byte bShowHonor;
        public byte bSrvLeastDestoryBaseNum;
        public byte bSrvLeastDestoryTowerNum;
        public byte bSrvLeastKillCntNum;
        public byte bSupportCameraDrag;
        public byte bValidRoomType;
        public static readonly uint CURRVERSION = 1;
        public uint dwAddLoseCondStarId;
        public uint dwAddWinCondStarId;
        public uint dwAttackOrderID;
        public uint dwBattleTaskOfCamp1;
        public uint dwBattleTaskOfCamp2;
        public uint dwChatID;
        public uint dwCooldownReduceUpperLimit;
        public uint dwDynamicPropertyCfg;
        public uint dwHeroFormId;
        public uint dwJudgeNum;
        public uint dwSoulAllocId;
        public uint dwSoulID;
        public uint dwTimeDuration;
        public uint dwUnLockCondID;
        public float fBigMapFowScale;
        public float fMapFowScale;
        public int iBigMapHeight;
        public int iBigMapWidth;
        public int iExtraPassiveSkillId;
        public int iExtraSkill2Id;
        public int iExtraSkillId;
        public int iHeroAIType;
        public int iMapHeight;
        public int iMapWidth;
        public int iOrnamentSkillId;
        public int iOrnamentSwitchCD;
        public int iPvpDifficulty;
        public int iSoldierActivateCountDelay1;
        public int iSoldierActivateCountDelay2;
        public int iSoldierActivateDelay;
        public static readonly uint LENGTH_szAmbientSoundEvent = 0x20;
        public static readonly uint LENGTH_szArtistFileName = 0x20;
        public static readonly uint LENGTH_szBankResourceName = 0x20;
        public static readonly uint LENGTH_szBigMapPath = 0x80;
        public static readonly uint LENGTH_szDesignFileName = 0x20;
        public static readonly uint LENGTH_szGameMatchName = 0x100;
        public static readonly uint LENGTH_szMapPath = 0x80;
        public static readonly uint LENGTH_szMusicEndEvent = 0x20;
        public static readonly uint LENGTH_szMusicStartEvent = 0x20;
        public static readonly uint LENGTH_szName = 0x20;
        public static readonly uint LENGTH_szThumbnailPath = 0x80;
        public ResDT_PickRuleInfo stPickRuleInfo;
        public ResDT_UnUseSkill stUnUseSkillInfo;
        public string szAmbientSoundEvent;
        public byte[] szAmbientSoundEvent_ByteArray;
        public string szArtistFileName;
        public byte[] szArtistFileName_ByteArray = new byte[1];
        public string szBankResourceName;
        public byte[] szBankResourceName_ByteArray;
        public string szBigMapPath;
        public byte[] szBigMapPath_ByteArray = new byte[1];
        public string szDesignFileName;
        public byte[] szDesignFileName_ByteArray = new byte[1];
        public string szGameMatchName;
        public byte[] szGameMatchName_ByteArray;
        public string szMapPath;
        public byte[] szMapPath_ByteArray = new byte[1];
        public string szMusicEndEvent;
        public byte[] szMusicEndEvent_ByteArray;
        public string szMusicStartEvent;
        public byte[] szMusicStartEvent_ByteArray;
        public string szName;
        public byte[] szName_ByteArray = new byte[1];
        public string szThumbnailPath;
        public byte[] szThumbnailPath_ByteArray = new byte[1];
        public ushort wOriginalGoldCoinInBattle;

        public ResDT_LevelCommonInfo()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astExpCompensateDetail[i] = new ResDT_ExpCompensateInfo();
            }
            this.stPickRuleInfo = new ResDT_PickRuleInfo();
            this.stUnUseSkillInfo = new ResDT_UnUseSkill();
            this.szMusicStartEvent_ByteArray = new byte[1];
            this.szMusicEndEvent_ByteArray = new byte[1];
            this.szAmbientSoundEvent_ByteArray = new byte[1];
            this.szBankResourceName_ByteArray = new byte[1];
            this.szGameMatchName_ByteArray = new byte[1];
            this.szName = string.Empty;
            this.szDesignFileName = string.Empty;
            this.szArtistFileName = string.Empty;
            this.szThumbnailPath = string.Empty;
            this.szMapPath = string.Empty;
            this.szBigMapPath = string.Empty;
            this.szMusicStartEvent = string.Empty;
            this.szMusicEndEvent = string.Empty;
            this.szAmbientSoundEvent = string.Empty;
            this.szBankResourceName = string.Empty;
            this.szGameMatchName = string.Empty;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            srcBuf.disableEndian();
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            int count = 0x20;
            if (this.szName_ByteArray.GetLength(0) < count)
            {
                this.szName_ByteArray = new byte[LENGTH_szName];
            }
            type = srcBuf.readCString(ref this.szName_ByteArray, count);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = 0x20;
                if (this.szDesignFileName_ByteArray.GetLength(0) < num2)
                {
                    this.szDesignFileName_ByteArray = new byte[LENGTH_szDesignFileName];
                }
                type = srcBuf.readCString(ref this.szDesignFileName_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x20;
                if (this.szArtistFileName_ByteArray.GetLength(0) < num3)
                {
                    this.szArtistFileName_ByteArray = new byte[LENGTH_szArtistFileName];
                }
                type = srcBuf.readCString(ref this.szArtistFileName_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMaxAcntNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bValidRoomType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsAllowHeroDup);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHeroFormId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iHeroAIType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szThumbnailPath_ByteArray.GetLength(0) < num4)
                {
                    this.szThumbnailPath_ByteArray = new byte[LENGTH_szThumbnailPath];
                }
                type = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szMapPath_ByteArray.GetLength(0) < num5)
                {
                    this.szMapPath_ByteArray = new byte[LENGTH_szMapPath];
                }
                type = srcBuf.readCString(ref this.szMapPath_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = 0x80;
                if (this.szBigMapPath_ByteArray.GetLength(0) < num6)
                {
                    this.szBigMapPath_ByteArray = new byte[LENGTH_szBigMapPath];
                }
                type = srcBuf.readCString(ref this.szBigMapPath_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMapWidth);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMapHeight);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBigMapWidth);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBigMapHeight);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readFloat(ref this.fMapFowScale);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readFloat(ref this.fBigMapFowScale);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSoulID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsOpenExpCompensate);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astExpCompensateDetail[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readInt32(ref this.iExtraSkillId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExtraSkill2Id);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExtraPassiveSkillId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bFinResultType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bRandPickHero);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAddWinCondStarId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAddLoseCondStarId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTimeDuration);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSoulAllocId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCameraFlip);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateCountDelay1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateCountDelay2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateDelay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBattleEquipLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBirthLevelConfig);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bChaosPickRule);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bHeadPtsUpperLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastDestoryTowerNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastDestoryBaseNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastKillCntNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bDealHangUp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUnLockCondID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bShowHonor);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPickRuleInfo.load(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCooldownReduceUpperLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPvpDifficulty);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwChatID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stUnUseSkillInfo.load(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAttackOrderID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wOriginalGoldCoinInBattle);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x20;
                if (this.szMusicStartEvent_ByteArray.GetLength(0) < num8)
                {
                    this.szMusicStartEvent_ByteArray = new byte[LENGTH_szMusicStartEvent];
                }
                type = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = 0x20;
                if (this.szMusicEndEvent_ByteArray.GetLength(0) < num9)
                {
                    this.szMusicEndEvent_ByteArray = new byte[LENGTH_szMusicEndEvent];
                }
                type = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num10 = 0x20;
                if (this.szAmbientSoundEvent_ByteArray.GetLength(0) < num10)
                {
                    this.szAmbientSoundEvent_ByteArray = new byte[LENGTH_szAmbientSoundEvent];
                }
                type = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num11 = 0x20;
                if (this.szBankResourceName_ByteArray.GetLength(0) < num11)
                {
                    this.szBankResourceName_ByteArray = new byte[LENGTH_szBankResourceName];
                }
                type = srcBuf.readCString(ref this.szBankResourceName_ByteArray, num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSupportCameraDrag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num12 = 0x100;
                if (this.szGameMatchName_ByteArray.GetLength(0) < num12)
                {
                    this.szGameMatchName_ByteArray = new byte[LENGTH_szGameMatchName];
                }
                type = srcBuf.readCString(ref this.szGameMatchName_ByteArray, num12);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsEnableFow);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsEnableShopHorizonTab);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsEnableOrnamentSlot);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iOrnamentSkillId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iOrnamentSwitchCD);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwJudgeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.TransferData();
            }
            return type;
        }

        public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.load(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }

        private void TransferData()
        {
            this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
            this.szName_ByteArray = null;
            this.szDesignFileName = StringHelper.UTF8BytesToString(ref this.szDesignFileName_ByteArray);
            this.szDesignFileName_ByteArray = null;
            this.szArtistFileName = StringHelper.UTF8BytesToString(ref this.szArtistFileName_ByteArray);
            this.szArtistFileName_ByteArray = null;
            this.szThumbnailPath = StringHelper.UTF8BytesToString(ref this.szThumbnailPath_ByteArray);
            this.szThumbnailPath_ByteArray = null;
            this.szMapPath = StringHelper.UTF8BytesToString(ref this.szMapPath_ByteArray);
            this.szMapPath_ByteArray = null;
            this.szBigMapPath = StringHelper.UTF8BytesToString(ref this.szBigMapPath_ByteArray);
            this.szBigMapPath_ByteArray = null;
            this.szMusicStartEvent = StringHelper.UTF8BytesToString(ref this.szMusicStartEvent_ByteArray);
            this.szMusicStartEvent_ByteArray = null;
            this.szMusicEndEvent = StringHelper.UTF8BytesToString(ref this.szMusicEndEvent_ByteArray);
            this.szMusicEndEvent_ByteArray = null;
            this.szAmbientSoundEvent = StringHelper.UTF8BytesToString(ref this.szAmbientSoundEvent_ByteArray);
            this.szAmbientSoundEvent_ByteArray = null;
            this.szBankResourceName = StringHelper.UTF8BytesToString(ref this.szBankResourceName_ByteArray);
            this.szBankResourceName_ByteArray = null;
            this.szGameMatchName = StringHelper.UTF8BytesToString(ref this.szGameMatchName_ByteArray);
            this.szGameMatchName_ByteArray = null;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
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
                if (dest > this.szName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
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
                if (num3 > this.szDesignFileName_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szDesignFileName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szDesignFileName_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szDesignFileName_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szDesignFileName_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szDesignFileName_ByteArray) + 1;
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
                if (num5 > this.szArtistFileName_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szArtistFileName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szArtistFileName_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szArtistFileName_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szArtistFileName_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szArtistFileName_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bMaxAcntNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bValidRoomType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsAllowHeroDup);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHeroFormId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iHeroAIType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                if (num7 > this.szThumbnailPath_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szThumbnailPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szThumbnailPath_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szThumbnailPath_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szThumbnailPath_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szThumbnailPath_ByteArray) + 1;
                if (num7 != num8)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num9 > this.szMapPath_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szMapPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMapPath_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMapPath_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMapPath_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szMapPath_ByteArray) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num11 > this.szBigMapPath_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szBigMapPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szBigMapPath_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szBigMapPath_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szBigMapPath_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szBigMapPath_ByteArray) + 1;
                if (num11 != num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iMapWidth);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMapHeight);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBigMapWidth);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBigMapHeight);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readFloat(ref this.fMapFowScale);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readFloat(ref this.fBigMapFowScale);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSoulID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsOpenExpCompensate);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astExpCompensateDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readInt32(ref this.iExtraSkillId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExtraSkill2Id);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExtraPassiveSkillId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bFinResultType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bRandPickHero);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAddWinCondStarId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAddLoseCondStarId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTimeDuration);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSoulAllocId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCameraFlip);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateCountDelay1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateCountDelay2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSoldierActivateDelay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBattleEquipLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBirthLevelConfig);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bChaosPickRule);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bHeadPtsUpperLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastDestoryTowerNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastDestoryBaseNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSrvLeastKillCntNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bDealHangUp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUnLockCondID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bShowHonor);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPickRuleInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCooldownReduceUpperLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPvpDifficulty);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwChatID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stUnUseSkillInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAttackOrderID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDynamicPropertyCfg);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wOriginalGoldCoinInBattle);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBattleTaskOfCamp2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num14 = 0;
                type = srcBuf.readUInt32(ref num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num14 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num14 > this.szMusicStartEvent_ByteArray.GetLength(0))
                {
                    if (num14 > LENGTH_szMusicStartEvent)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMusicStartEvent_ByteArray = new byte[num14];
                }
                if (1 > num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMusicStartEvent_ByteArray, (int) num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMusicStartEvent_ByteArray[((int) num14) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num15 = TdrTypeUtil.cstrlen(this.szMusicStartEvent_ByteArray) + 1;
                if (num14 != num15)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num16 = 0;
                type = srcBuf.readUInt32(ref num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num16 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num16 > this.szMusicEndEvent_ByteArray.GetLength(0))
                {
                    if (num16 > LENGTH_szMusicEndEvent)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMusicEndEvent_ByteArray = new byte[num16];
                }
                if (1 > num16)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMusicEndEvent_ByteArray, (int) num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMusicEndEvent_ByteArray[((int) num16) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num17 = TdrTypeUtil.cstrlen(this.szMusicEndEvent_ByteArray) + 1;
                if (num16 != num17)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num18 = 0;
                type = srcBuf.readUInt32(ref num18);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num18 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num18 > this.szAmbientSoundEvent_ByteArray.GetLength(0))
                {
                    if (num18 > LENGTH_szAmbientSoundEvent)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAmbientSoundEvent_ByteArray = new byte[num18];
                }
                if (1 > num18)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAmbientSoundEvent_ByteArray, (int) num18);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAmbientSoundEvent_ByteArray[((int) num18) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num19 = TdrTypeUtil.cstrlen(this.szAmbientSoundEvent_ByteArray) + 1;
                if (num18 != num19)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num20 = 0;
                type = srcBuf.readUInt32(ref num20);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num20 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num20 > this.szBankResourceName_ByteArray.GetLength(0))
                {
                    if (num20 > LENGTH_szBankResourceName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szBankResourceName_ByteArray = new byte[num20];
                }
                if (1 > num20)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szBankResourceName_ByteArray, (int) num20);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szBankResourceName_ByteArray[((int) num20) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num21 = TdrTypeUtil.cstrlen(this.szBankResourceName_ByteArray) + 1;
                if (num20 != num21)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bSupportCameraDrag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num22 = 0;
                type = srcBuf.readUInt32(ref num22);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num22 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num22 > this.szGameMatchName_ByteArray.GetLength(0))
                {
                    if (num22 > LENGTH_szGameMatchName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGameMatchName_ByteArray = new byte[num22];
                }
                if (1 > num22)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGameMatchName_ByteArray, (int) num22);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szGameMatchName_ByteArray[((int) num22) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num23 = TdrTypeUtil.cstrlen(this.szGameMatchName_ByteArray) + 1;
                    if (num22 != num23)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bIsEnableFow);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bIsEnableShopHorizonTab);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bIsEnableOrnamentSlot);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iOrnamentSkillId);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iOrnamentSwitchCD);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwJudgeNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    this.TransferData();
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
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }
    }
}

