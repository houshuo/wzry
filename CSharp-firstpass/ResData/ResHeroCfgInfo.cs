namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResHeroCfgInfo : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_SkillInfo[] astSkill = new ResDT_SkillInfo[5];
        public static readonly uint BASEVERSION = 1;
        public byte bAttackDistanceType;
        public byte bAttackType;
        public byte bDamageType;
        public byte bExpandType;
        public byte bIOSHide;
        public byte bIsTrainUse;
        public byte bMainJob;
        public byte bMinorJob;
        public byte bType;
        public static readonly uint CURRVERSION = 1;
        public uint dwCfgID;
        public uint dwEnergyType;
        public uint dwShowSortId;
        public uint dwSymbolRcmdID;
        public uint dwWakeSkinID;
        public uint dwWakeTalentID;
        public CrypticInteger iAtkGrowth;
        public CrypticInteger iAtkSpdAddLvlup;
        public CrypticInteger iBaseAtkSpd;
        public CrypticInteger iBaseATT;
        public CrypticInteger iBaseDEF;
        public CrypticInteger iBaseHP;
        public CrypticInteger iBaseHPAdd;
        public CrypticInteger iBaseINT;
        public CrypticInteger iBaseRES;
        public CrypticInteger iBaseSpeed;
        public CrypticInteger iCritEft;
        public CrypticInteger iCritRate;
        public CrypticInteger iDefGrowth;
        public CrypticInteger iEnergy;
        public CrypticInteger iEnergyGrowth;
        public CrypticInteger iEnergyRec;
        public CrypticInteger iEnergyRecGrowth;
        public CrypticInteger iHPAddLvlup;
        public CrypticInteger iHpGrowth;
        public int iInitialStar;
        public int iPassiveID1;
        public int iPassiveID2;
        public CrypticInteger iPhyDamage;
        public int iPVPNeedQuality;
        public int iPVPNeedStar;
        public int iPVPNeedSubQuality;
        public int iRecommendPosition;
        public CrypticInteger iResistGrowth;
        public int iSightR;
        public CrypticInteger iSpellDamage;
        public CrypticInteger iSpellGrowth;
        public int iStartedDifficulty;
        public CrypticInteger iViability;
        public int[] JobFeature;
        public static readonly uint LENGTH_szAI_Entry = 0x80;
        public static readonly uint LENGTH_szAI_Hard = 0x80;
        public static readonly uint LENGTH_szAI_Normal = 0x80;
        public static readonly uint LENGTH_szAI_Simple = 0x80;
        public static readonly uint LENGTH_szAI_Warm = 0x80;
        public static readonly uint LENGTH_szAI_WarmSimple = 0x80;
        public static readonly uint LENGTH_szAttackRangeDesc = 0x10;
        public static readonly uint LENGTH_szBorn_Age = 0x80;
        public static readonly uint LENGTH_szCharacterInfo = 0x80;
        public static readonly uint LENGTH_szHeroDesc = 0x100;
        public static readonly uint LENGTH_szHeroSound = 0x40;
        public static readonly uint LENGTH_szHeroTips = 0x1000;
        public static readonly uint LENGTH_szHeroTitle = 0x80;
        public static readonly uint LENGTH_szImagePath = 0x80;
        public static readonly uint LENGTH_szName = 0x20;
        public static readonly uint LENGTH_szNamePinYin = 0x20;
        public static readonly uint LENGTH_szRevive_Age = 0x80;
        public static readonly uint LENGTH_szStoryUrl = 0x100;
        public static readonly uint LENGTH_szWakeDesc = 0x200;
        public string szAI_Entry;
        public byte[] szAI_Entry_ByteArray;
        public string szAI_Hard;
        public byte[] szAI_Hard_ByteArray;
        public string szAI_Normal;
        public byte[] szAI_Normal_ByteArray;
        public string szAI_Simple;
        public byte[] szAI_Simple_ByteArray;
        public string szAI_Warm;
        public byte[] szAI_Warm_ByteArray;
        public string szAI_WarmSimple;
        public byte[] szAI_WarmSimple_ByteArray;
        public string szAttackRangeDesc;
        public byte[] szAttackRangeDesc_ByteArray;
        public string szBorn_Age;
        public byte[] szBorn_Age_ByteArray;
        public string szCharacterInfo;
        public byte[] szCharacterInfo_ByteArray = new byte[1];
        public string szHeroDesc;
        public byte[] szHeroDesc_ByteArray;
        public string szHeroSound;
        public byte[] szHeroSound_ByteArray;
        public string szHeroTips;
        public byte[] szHeroTips_ByteArray;
        public string szHeroTitle;
        public byte[] szHeroTitle_ByteArray = new byte[1];
        public string szImagePath;
        public byte[] szImagePath_ByteArray = new byte[1];
        public string szName;
        public byte[] szName_ByteArray = new byte[1];
        public string szNamePinYin;
        public byte[] szNamePinYin_ByteArray = new byte[1];
        public string szRevive_Age;
        public byte[] szRevive_Age_ByteArray;
        public string szStoryUrl;
        public byte[] szStoryUrl_ByteArray = new byte[1];
        public string szWakeDesc;
        public byte[] szWakeDesc_ByteArray;
        public ushort wPVPNeedLevel;

        public ResHeroCfgInfo()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astSkill[i] = new ResDT_SkillInfo();
            }
            this.JobFeature = new int[2];
            this.szHeroDesc_ByteArray = new byte[1];
            this.szAI_Entry_ByteArray = new byte[1];
            this.szAI_Simple_ByteArray = new byte[1];
            this.szAI_Normal_ByteArray = new byte[1];
            this.szAI_Hard_ByteArray = new byte[1];
            this.szAI_WarmSimple_ByteArray = new byte[1];
            this.szAI_Warm_ByteArray = new byte[1];
            this.szWakeDesc_ByteArray = new byte[1];
            this.szAttackRangeDesc_ByteArray = new byte[1];
            this.szHeroTips_ByteArray = new byte[1];
            this.szHeroSound_ByteArray = new byte[1];
            this.szBorn_Age_ByteArray = new byte[1];
            this.szRevive_Age_ByteArray = new byte[1];
            this.szName = string.Empty;
            this.szNamePinYin = string.Empty;
            this.szHeroTitle = string.Empty;
            this.szStoryUrl = string.Empty;
            this.szImagePath = string.Empty;
            this.szCharacterInfo = string.Empty;
            this.szHeroDesc = string.Empty;
            this.szAI_Entry = string.Empty;
            this.szAI_Simple = string.Empty;
            this.szAI_Normal = string.Empty;
            this.szAI_Hard = string.Empty;
            this.szAI_WarmSimple = string.Empty;
            this.szAI_Warm = string.Empty;
            this.szWakeDesc = string.Empty;
            this.szAttackRangeDesc = string.Empty;
            this.szHeroTips = string.Empty;
            this.szHeroSound = string.Empty;
            this.szBorn_Age = string.Empty;
            this.szRevive_Age = string.Empty;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            int dest = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            int num13 = 0;
            int num14 = 0;
            int num15 = 0;
            int num16 = 0;
            int num17 = 0;
            int num18 = 0;
            int num19 = 0;
            int num20 = 0;
            int num21 = 0;
            int num22 = 0;
            int num23 = 0;
            int num24 = 0;
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
            type = srcBuf.readUInt32(ref this.dwCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x20;
                if (this.szName_ByteArray.GetLength(0) < count)
                {
                    this.szName_ByteArray = new byte[LENGTH_szName];
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num26 = 0x20;
                if (this.szNamePinYin_ByteArray.GetLength(0) < num26)
                {
                    this.szNamePinYin_ByteArray = new byte[LENGTH_szNamePinYin];
                }
                type = srcBuf.readCString(ref this.szNamePinYin_ByteArray, num26);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsTrainUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num27 = 0x80;
                if (this.szHeroTitle_ByteArray.GetLength(0) < num27)
                {
                    this.szHeroTitle_ByteArray = new byte[LENGTH_szHeroTitle];
                }
                type = srcBuf.readCString(ref this.szHeroTitle_ByteArray, num27);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num28 = 0x100;
                if (this.szStoryUrl_ByteArray.GetLength(0) < num28)
                {
                    this.szStoryUrl_ByteArray = new byte[LENGTH_szStoryUrl];
                }
                type = srcBuf.readCString(ref this.szStoryUrl_ByteArray, num28);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num29 = 0x80;
                if (this.szImagePath_ByteArray.GetLength(0) < num29)
                {
                    this.szImagePath_ByteArray = new byte[LENGTH_szImagePath];
                }
                type = srcBuf.readCString(ref this.szImagePath_ByteArray, num29);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num30 = 0x80;
                if (this.szCharacterInfo_ByteArray.GetLength(0) < num30)
                {
                    this.szCharacterInfo_ByteArray = new byte[LENGTH_szCharacterInfo];
                }
                type = srcBuf.readCString(ref this.szCharacterInfo_ByteArray, num30);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iRecommendPosition);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAttackDistanceType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSightR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref dest);
                this.iBaseHP = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num2);
                this.iBaseHPAdd = num2;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num3);
                this.iHPAddLvlup = num3;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num4);
                this.iBaseATT = num4;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num5);
                this.iBaseINT = num5;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num6);
                this.iBaseDEF = num6;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num7);
                this.iBaseRES = num7;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num8);
                this.iBaseSpeed = num8;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num9);
                this.iAtkSpdAddLvlup = num9;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num10);
                this.iBaseAtkSpd = num10;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num11);
                this.iCritRate = num11;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num12);
                this.iCritEft = num12;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num13);
                this.iHpGrowth = num13;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num14);
                this.iAtkGrowth = num14;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num15);
                this.iSpellGrowth = num15;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num16);
                this.iDefGrowth = num16;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num17);
                this.iResistGrowth = num17;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPassiveID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPassiveID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astSkill[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readInt32(ref this.iInitialStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bExpandType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wPVPNeedLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedSubQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num18);
                this.iViability = num18;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num19);
                this.iPhyDamage = num19;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num20);
                this.iSpellDamage = num20;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iStartedDifficulty);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMainJob);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMinorJob);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 2; j++)
                {
                    type = srcBuf.readInt32(ref this.JobFeature[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bDamageType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAttackType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num33 = 0x100;
                if (this.szHeroDesc_ByteArray.GetLength(0) < num33)
                {
                    this.szHeroDesc_ByteArray = new byte[LENGTH_szHeroDesc];
                }
                type = srcBuf.readCString(ref this.szHeroDesc_ByteArray, num33);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIOSHide);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwShowSortId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num34 = 0x80;
                if (this.szAI_Entry_ByteArray.GetLength(0) < num34)
                {
                    this.szAI_Entry_ByteArray = new byte[LENGTH_szAI_Entry];
                }
                type = srcBuf.readCString(ref this.szAI_Entry_ByteArray, num34);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num35 = 0x80;
                if (this.szAI_Simple_ByteArray.GetLength(0) < num35)
                {
                    this.szAI_Simple_ByteArray = new byte[LENGTH_szAI_Simple];
                }
                type = srcBuf.readCString(ref this.szAI_Simple_ByteArray, num35);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num36 = 0x80;
                if (this.szAI_Normal_ByteArray.GetLength(0) < num36)
                {
                    this.szAI_Normal_ByteArray = new byte[LENGTH_szAI_Normal];
                }
                type = srcBuf.readCString(ref this.szAI_Normal_ByteArray, num36);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num37 = 0x80;
                if (this.szAI_Hard_ByteArray.GetLength(0) < num37)
                {
                    this.szAI_Hard_ByteArray = new byte[LENGTH_szAI_Hard];
                }
                type = srcBuf.readCString(ref this.szAI_Hard_ByteArray, num37);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num38 = 0x80;
                if (this.szAI_WarmSimple_ByteArray.GetLength(0) < num38)
                {
                    this.szAI_WarmSimple_ByteArray = new byte[LENGTH_szAI_WarmSimple];
                }
                type = srcBuf.readCString(ref this.szAI_WarmSimple_ByteArray, num38);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num39 = 0x80;
                if (this.szAI_Warm_ByteArray.GetLength(0) < num39)
                {
                    this.szAI_Warm_ByteArray = new byte[LENGTH_szAI_Warm];
                }
                type = srcBuf.readCString(ref this.szAI_Warm_ByteArray, num39);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num40 = 0x200;
                if (this.szWakeDesc_ByteArray.GetLength(0) < num40)
                {
                    this.szWakeDesc_ByteArray = new byte[LENGTH_szWakeDesc];
                }
                type = srcBuf.readCString(ref this.szWakeDesc_ByteArray, num40);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwWakeTalentID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwWakeSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num41 = 0x10;
                if (this.szAttackRangeDesc_ByteArray.GetLength(0) < num41)
                {
                    this.szAttackRangeDesc_ByteArray = new byte[LENGTH_szAttackRangeDesc];
                }
                type = srcBuf.readCString(ref this.szAttackRangeDesc_ByteArray, num41);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwEnergyType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num21);
                this.iEnergy = num21;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num22);
                this.iEnergyGrowth = num22;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num23);
                this.iEnergyRec = num23;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num24);
                this.iEnergyRecGrowth = num24;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num42 = 0x1000;
                if (this.szHeroTips_ByteArray.GetLength(0) < num42)
                {
                    this.szHeroTips_ByteArray = new byte[LENGTH_szHeroTips];
                }
                type = srcBuf.readCString(ref this.szHeroTips_ByteArray, num42);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num43 = 0x40;
                if (this.szHeroSound_ByteArray.GetLength(0) < num43)
                {
                    this.szHeroSound_ByteArray = new byte[LENGTH_szHeroSound];
                }
                type = srcBuf.readCString(ref this.szHeroSound_ByteArray, num43);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSymbolRcmdID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num44 = 0x80;
                if (this.szBorn_Age_ByteArray.GetLength(0) < num44)
                {
                    this.szBorn_Age_ByteArray = new byte[LENGTH_szBorn_Age];
                }
                type = srcBuf.readCString(ref this.szBorn_Age_ByteArray, num44);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num45 = 0x80;
                if (this.szRevive_Age_ByteArray.GetLength(0) < num45)
                {
                    this.szRevive_Age_ByteArray = new byte[LENGTH_szRevive_Age];
                }
                type = srcBuf.readCString(ref this.szRevive_Age_ByteArray, num45);
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
            this.szNamePinYin = StringHelper.UTF8BytesToString(ref this.szNamePinYin_ByteArray);
            this.szNamePinYin_ByteArray = null;
            this.szHeroTitle = StringHelper.UTF8BytesToString(ref this.szHeroTitle_ByteArray);
            this.szHeroTitle_ByteArray = null;
            this.szStoryUrl = StringHelper.UTF8BytesToString(ref this.szStoryUrl_ByteArray);
            this.szStoryUrl_ByteArray = null;
            this.szImagePath = StringHelper.UTF8BytesToString(ref this.szImagePath_ByteArray);
            this.szImagePath_ByteArray = null;
            this.szCharacterInfo = StringHelper.UTF8BytesToString(ref this.szCharacterInfo_ByteArray);
            this.szCharacterInfo_ByteArray = null;
            this.szHeroDesc = StringHelper.UTF8BytesToString(ref this.szHeroDesc_ByteArray);
            this.szHeroDesc_ByteArray = null;
            this.szAI_Entry = StringHelper.UTF8BytesToString(ref this.szAI_Entry_ByteArray);
            this.szAI_Entry_ByteArray = null;
            this.szAI_Simple = StringHelper.UTF8BytesToString(ref this.szAI_Simple_ByteArray);
            this.szAI_Simple_ByteArray = null;
            this.szAI_Normal = StringHelper.UTF8BytesToString(ref this.szAI_Normal_ByteArray);
            this.szAI_Normal_ByteArray = null;
            this.szAI_Hard = StringHelper.UTF8BytesToString(ref this.szAI_Hard_ByteArray);
            this.szAI_Hard_ByteArray = null;
            this.szAI_WarmSimple = StringHelper.UTF8BytesToString(ref this.szAI_WarmSimple_ByteArray);
            this.szAI_WarmSimple_ByteArray = null;
            this.szAI_Warm = StringHelper.UTF8BytesToString(ref this.szAI_Warm_ByteArray);
            this.szAI_Warm_ByteArray = null;
            this.szWakeDesc = StringHelper.UTF8BytesToString(ref this.szWakeDesc_ByteArray);
            this.szWakeDesc_ByteArray = null;
            this.szAttackRangeDesc = StringHelper.UTF8BytesToString(ref this.szAttackRangeDesc_ByteArray);
            this.szAttackRangeDesc_ByteArray = null;
            this.szHeroTips = StringHelper.UTF8BytesToString(ref this.szHeroTips_ByteArray);
            this.szHeroTips_ByteArray = null;
            this.szHeroSound = StringHelper.UTF8BytesToString(ref this.szHeroSound_ByteArray);
            this.szHeroSound_ByteArray = null;
            this.szBorn_Age = StringHelper.UTF8BytesToString(ref this.szBorn_Age_ByteArray);
            this.szBorn_Age_ByteArray = null;
            this.szRevive_Age = StringHelper.UTF8BytesToString(ref this.szRevive_Age_ByteArray);
            this.szRevive_Age_ByteArray = null;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            int dest = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            int num13 = 0;
            int num14 = 0;
            int num15 = 0;
            int num16 = 0;
            int num17 = 0;
            int num18 = 0;
            int num19 = 0;
            int num20 = 0;
            int num21 = 0;
            int num22 = 0;
            int num23 = 0;
            int num24 = 0;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt32(ref this.dwCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                uint num25 = 0;
                type = srcBuf.readUInt32(ref num25);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num25 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num25 > this.szName_ByteArray.GetLength(0))
                {
                    if (num25 > LENGTH_szName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szName_ByteArray = new byte[num25];
                }
                if (1 > num25)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, (int) num25);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szName_ByteArray[((int) num25) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num26 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
                if (num25 != num26)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num27 = 0;
                type = srcBuf.readUInt32(ref num27);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num27 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num27 > this.szNamePinYin_ByteArray.GetLength(0))
                {
                    if (num27 > LENGTH_szNamePinYin)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szNamePinYin_ByteArray = new byte[num27];
                }
                if (1 > num27)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szNamePinYin_ByteArray, (int) num27);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szNamePinYin_ByteArray[((int) num27) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num28 = TdrTypeUtil.cstrlen(this.szNamePinYin_ByteArray) + 1;
                if (num27 != num28)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIsTrainUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num29 = 0;
                type = srcBuf.readUInt32(ref num29);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num29 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num29 > this.szHeroTitle_ByteArray.GetLength(0))
                {
                    if (num29 > LENGTH_szHeroTitle)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroTitle_ByteArray = new byte[num29];
                }
                if (1 > num29)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroTitle_ByteArray, (int) num29);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroTitle_ByteArray[((int) num29) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num30 = TdrTypeUtil.cstrlen(this.szHeroTitle_ByteArray) + 1;
                if (num29 != num30)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num31 = 0;
                type = srcBuf.readUInt32(ref num31);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num31 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num31 > this.szStoryUrl_ByteArray.GetLength(0))
                {
                    if (num31 > LENGTH_szStoryUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szStoryUrl_ByteArray = new byte[num31];
                }
                if (1 > num31)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szStoryUrl_ByteArray, (int) num31);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szStoryUrl_ByteArray[((int) num31) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num32 = TdrTypeUtil.cstrlen(this.szStoryUrl_ByteArray) + 1;
                if (num31 != num32)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num33 = 0;
                type = srcBuf.readUInt32(ref num33);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num33 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num33 > this.szImagePath_ByteArray.GetLength(0))
                {
                    if (num33 > LENGTH_szImagePath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szImagePath_ByteArray = new byte[num33];
                }
                if (1 > num33)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szImagePath_ByteArray, (int) num33);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szImagePath_ByteArray[((int) num33) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num34 = TdrTypeUtil.cstrlen(this.szImagePath_ByteArray) + 1;
                if (num33 != num34)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num35 = 0;
                type = srcBuf.readUInt32(ref num35);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num35 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num35 > this.szCharacterInfo_ByteArray.GetLength(0))
                {
                    if (num35 > LENGTH_szCharacterInfo)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCharacterInfo_ByteArray = new byte[num35];
                }
                if (1 > num35)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCharacterInfo_ByteArray, (int) num35);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCharacterInfo_ByteArray[((int) num35) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num36 = TdrTypeUtil.cstrlen(this.szCharacterInfo_ByteArray) + 1;
                if (num35 != num36)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iRecommendPosition);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAttackDistanceType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSightR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref dest);
                this.iBaseHP = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num2);
                this.iBaseHPAdd = num2;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num3);
                this.iHPAddLvlup = num3;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num4);
                this.iBaseATT = num4;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num5);
                this.iBaseINT = num5;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num6);
                this.iBaseDEF = num6;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num7);
                this.iBaseRES = num7;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num8);
                this.iBaseSpeed = num8;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num9);
                this.iAtkSpdAddLvlup = num9;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num10);
                this.iBaseAtkSpd = num10;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num11);
                this.iCritRate = num11;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num12);
                this.iCritEft = num12;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num13);
                this.iHpGrowth = num13;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num14);
                this.iAtkGrowth = num14;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num15);
                this.iSpellGrowth = num15;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num16);
                this.iDefGrowth = num16;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num17);
                this.iResistGrowth = num17;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPassiveID1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPassiveID2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astSkill[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readInt32(ref this.iInitialStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bExpandType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wPVPNeedLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedSubQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPVPNeedStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num18);
                this.iViability = num18;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num19);
                this.iPhyDamage = num19;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num20);
                this.iSpellDamage = num20;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iStartedDifficulty);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMainJob);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMinorJob);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 2; j++)
                {
                    type = srcBuf.readInt32(ref this.JobFeature[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bDamageType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAttackType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num39 = 0;
                type = srcBuf.readUInt32(ref num39);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num39 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num39 > this.szHeroDesc_ByteArray.GetLength(0))
                {
                    if (num39 > LENGTH_szHeroDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroDesc_ByteArray = new byte[num39];
                }
                if (1 > num39)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroDesc_ByteArray, (int) num39);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroDesc_ByteArray[((int) num39) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num40 = TdrTypeUtil.cstrlen(this.szHeroDesc_ByteArray) + 1;
                if (num39 != num40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIOSHide);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwShowSortId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num41 = 0;
                type = srcBuf.readUInt32(ref num41);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num41 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num41 > this.szAI_Entry_ByteArray.GetLength(0))
                {
                    if (num41 > LENGTH_szAI_Entry)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_Entry_ByteArray = new byte[num41];
                }
                if (1 > num41)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_Entry_ByteArray, (int) num41);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_Entry_ByteArray[((int) num41) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num42 = TdrTypeUtil.cstrlen(this.szAI_Entry_ByteArray) + 1;
                if (num41 != num42)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num43 = 0;
                type = srcBuf.readUInt32(ref num43);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num43 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num43 > this.szAI_Simple_ByteArray.GetLength(0))
                {
                    if (num43 > LENGTH_szAI_Simple)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_Simple_ByteArray = new byte[num43];
                }
                if (1 > num43)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_Simple_ByteArray, (int) num43);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_Simple_ByteArray[((int) num43) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num44 = TdrTypeUtil.cstrlen(this.szAI_Simple_ByteArray) + 1;
                if (num43 != num44)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num45 = 0;
                type = srcBuf.readUInt32(ref num45);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num45 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num45 > this.szAI_Normal_ByteArray.GetLength(0))
                {
                    if (num45 > LENGTH_szAI_Normal)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_Normal_ByteArray = new byte[num45];
                }
                if (1 > num45)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_Normal_ByteArray, (int) num45);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_Normal_ByteArray[((int) num45) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num46 = TdrTypeUtil.cstrlen(this.szAI_Normal_ByteArray) + 1;
                if (num45 != num46)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num47 = 0;
                type = srcBuf.readUInt32(ref num47);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num47 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num47 > this.szAI_Hard_ByteArray.GetLength(0))
                {
                    if (num47 > LENGTH_szAI_Hard)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_Hard_ByteArray = new byte[num47];
                }
                if (1 > num47)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_Hard_ByteArray, (int) num47);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_Hard_ByteArray[((int) num47) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num48 = TdrTypeUtil.cstrlen(this.szAI_Hard_ByteArray) + 1;
                if (num47 != num48)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num49 = 0;
                type = srcBuf.readUInt32(ref num49);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num49 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num49 > this.szAI_WarmSimple_ByteArray.GetLength(0))
                {
                    if (num49 > LENGTH_szAI_WarmSimple)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_WarmSimple_ByteArray = new byte[num49];
                }
                if (1 > num49)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_WarmSimple_ByteArray, (int) num49);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_WarmSimple_ByteArray[((int) num49) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num50 = TdrTypeUtil.cstrlen(this.szAI_WarmSimple_ByteArray) + 1;
                if (num49 != num50)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num51 = 0;
                type = srcBuf.readUInt32(ref num51);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num51 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num51 > this.szAI_Warm_ByteArray.GetLength(0))
                {
                    if (num51 > LENGTH_szAI_Warm)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAI_Warm_ByteArray = new byte[num51];
                }
                if (1 > num51)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAI_Warm_ByteArray, (int) num51);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAI_Warm_ByteArray[((int) num51) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num52 = TdrTypeUtil.cstrlen(this.szAI_Warm_ByteArray) + 1;
                if (num51 != num52)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num53 = 0;
                type = srcBuf.readUInt32(ref num53);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num53 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num53 > this.szWakeDesc_ByteArray.GetLength(0))
                {
                    if (num53 > LENGTH_szWakeDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szWakeDesc_ByteArray = new byte[num53];
                }
                if (1 > num53)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szWakeDesc_ByteArray, (int) num53);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szWakeDesc_ByteArray[((int) num53) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num54 = TdrTypeUtil.cstrlen(this.szWakeDesc_ByteArray) + 1;
                if (num53 != num54)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwWakeTalentID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwWakeSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num55 = 0;
                type = srcBuf.readUInt32(ref num55);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num55 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num55 > this.szAttackRangeDesc_ByteArray.GetLength(0))
                {
                    if (num55 > LENGTH_szAttackRangeDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAttackRangeDesc_ByteArray = new byte[num55];
                }
                if (1 > num55)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAttackRangeDesc_ByteArray, (int) num55);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAttackRangeDesc_ByteArray[((int) num55) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num56 = TdrTypeUtil.cstrlen(this.szAttackRangeDesc_ByteArray) + 1;
                if (num55 != num56)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwEnergyType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num21);
                this.iEnergy = num21;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num22);
                this.iEnergyGrowth = num22;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num23);
                this.iEnergyRec = num23;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num24);
                this.iEnergyRecGrowth = num24;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num57 = 0;
                type = srcBuf.readUInt32(ref num57);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num57 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num57 > this.szHeroTips_ByteArray.GetLength(0))
                {
                    if (num57 > LENGTH_szHeroTips)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroTips_ByteArray = new byte[num57];
                }
                if (1 > num57)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroTips_ByteArray, (int) num57);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroTips_ByteArray[((int) num57) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num58 = TdrTypeUtil.cstrlen(this.szHeroTips_ByteArray) + 1;
                if (num57 != num58)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num59 = 0;
                type = srcBuf.readUInt32(ref num59);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num59 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num59 > this.szHeroSound_ByteArray.GetLength(0))
                {
                    if (num59 > LENGTH_szHeroSound)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroSound_ByteArray = new byte[num59];
                }
                if (1 > num59)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroSound_ByteArray, (int) num59);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroSound_ByteArray[((int) num59) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num60 = TdrTypeUtil.cstrlen(this.szHeroSound_ByteArray) + 1;
                if (num59 != num60)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwSymbolRcmdID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num61 = 0;
                type = srcBuf.readUInt32(ref num61);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num61 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num61 > this.szBorn_Age_ByteArray.GetLength(0))
                {
                    if (num61 > LENGTH_szBorn_Age)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szBorn_Age_ByteArray = new byte[num61];
                }
                if (1 > num61)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szBorn_Age_ByteArray, (int) num61);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szBorn_Age_ByteArray[((int) num61) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num62 = TdrTypeUtil.cstrlen(this.szBorn_Age_ByteArray) + 1;
                if (num61 != num62)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num63 = 0;
                type = srcBuf.readUInt32(ref num63);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num63 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num63 > this.szRevive_Age_ByteArray.GetLength(0))
                {
                    if (num63 > LENGTH_szRevive_Age)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRevive_Age_ByteArray = new byte[num63];
                }
                if (1 > num63)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRevive_Age_ByteArray, (int) num63);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szRevive_Age_ByteArray[((int) num63) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num64 = TdrTypeUtil.cstrlen(this.szRevive_Age_ByteArray) + 1;
                    if (num63 != num64)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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

