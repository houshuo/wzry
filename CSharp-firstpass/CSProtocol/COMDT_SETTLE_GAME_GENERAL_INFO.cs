namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SETTLE_GAME_GENERAL_INFO : ProtocolObject
    {
        public COMDT_INGAME_ADVANTAGE_INFO[] astAdvantageDetail;
        public COMDT_INGAME_EQUIP_INFO[] astEquipDetail;
        public COMDT_GAME_30SEC_INFO[] astGame30SecDetail;
        public COMDT_RUNE_PICK_UP_INFO[] astRuneTypePickUpNum = new COMDT_RUNE_PICK_UP_INFO[10];
        public byte bAdvantageNum;
        public static readonly uint BASEVERSION = 1;
        public byte bEquipNum;
        public byte bGame30SecNum;
        public uint[] BigDragonBattleToDeadTimeDtail;
        public byte bKillBigDragonNum;
        public byte bKillDragonNum;
        public byte bPanelBuyItemCnt;
        public byte bQuickBuyItemCnt;
        public byte bRuneTypeNum;
        public byte bSelfCampHaveWinningFlag;
        public byte bSelfCampKillBaseCnt;
        public byte bSelfCampKillTowerCnt;
        public static readonly int CLASS_ID = 150;
        public uint[] ClickDetail;
        public static readonly uint CURRVERSION = 1;
        public uint[] DragonBattleToDeadTimeDtail;
        public uint dwBigDragonBattleToDeadTimeNum;
        public uint dwBoardSortNum;
        public uint dwCamp1TowerFirstAttackTime;
        public uint dwCamp2TowerFirstAttackTime;
        public uint dwClickNum;
        public uint dwDownRoadTower1DesTime;
        public uint dwDownRoadTower2DesTime;
        public uint dwDownRoadTower3DesTime;
        public uint dwDragonBattleToDeadTimeNum;
        public uint dwFBTime;
        public uint dwMaxInGameCoin;
        public uint dwMidRoadTower1DesTime;
        public uint dwMidRoadTower2DesTime;
        public uint dwMidRoadTower3DesTime;
        public uint dwMonsterDeadNum;
        public uint dwOperateNum;
        public uint dwSelfCampBaseBlood;
        public uint dwTotalInGameCoin;
        public uint dwUpRoadTower1DesTime;
        public uint dwUpRoadTower2DesTime;
        public uint dwUpRoadTower3DesTime;
        public int iBossAttackCount;
        public int iBossAttackMax;
        public int iBossAttackMin;
        public int iBossAttackTotal;
        public int iBossCount;
        public int iBossHPMax;
        public int iBossHPMin;
        public int iBossHurtMax;
        public int iBossHurtMin;
        public int iBossHurtTotal;
        public int iBossSkillDamageMax;
        public int iBossSkillDamageMin;
        public int iBossUseSkillCount;
        public int iBuildingAttackDamageMax;
        public int iBuildingAttackDamageMin;
        public int iBuildingAttackRange;
        public int iBuildingHPMax;
        public int iBuildingHPMin;
        public int iBuildingHurtCount;
        public int iBuildingHurtMax;
        public int iBuildingHurtMin;
        public int iBuildingHurtTotal;
        public int iCommunicationCount1;
        public int iCommunicationCount2;
        public int iCurrentDisparity;
        public int iEnemyAttackMax;
        public int iEnemyAttackMin;
        public int iEnemyBuildingAttackRange;
        public int iEnemyBuildingDamageMax;
        public int iEnemyBuildingDamageMin;
        public int iEnemyBuildingHPMax;
        public int iEnemyBuildingHPMin;
        public int iEnemyBuildingHurtMax;
        public int iEnemyBuildingHurtMin;
        public int iEnemyBuildingHurtTotal;
        public int iEnemyHPMax;
        public int iEnemyHPMin;
        public int iExperienceHPAdd1;
        public int iExperienceHPAdd2;
        public int iExperienceHPAdd3;
        public int iExperienceHPAdd4;
        public int iExperienceHPAddTotal;
        public int iMaxSoldierCnt;
        public int iMaxTowerAttackDistance;
        public int iPauseTimeTotal;
        public int iTimeUse;
        public uint[] KillBigDragonTime = new uint[10];
        public uint[] KillDragonTime = new uint[10];
        public uint[] MonsterDeadDetail;
        public uint[] OperateType;

        public COMDT_SETTLE_GAME_GENERAL_INFO()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astRuneTypePickUpNum[i] = (COMDT_RUNE_PICK_UP_INFO) ProtocolObjectPool.Get(COMDT_RUNE_PICK_UP_INFO.CLASS_ID);
            }
            this.astGame30SecDetail = new COMDT_GAME_30SEC_INFO[60];
            for (int j = 0; j < 60; j++)
            {
                this.astGame30SecDetail[j] = (COMDT_GAME_30SEC_INFO) ProtocolObjectPool.Get(COMDT_GAME_30SEC_INFO.CLASS_ID);
            }
            this.astEquipDetail = new COMDT_INGAME_EQUIP_INFO[6];
            for (int k = 0; k < 6; k++)
            {
                this.astEquipDetail[k] = (COMDT_INGAME_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
            }
            this.ClickDetail = new uint[30];
            this.astAdvantageDetail = new COMDT_INGAME_ADVANTAGE_INFO[0x10];
            for (int m = 0; m < 0x10; m++)
            {
                this.astAdvantageDetail[m] = (COMDT_INGAME_ADVANTAGE_INFO) ProtocolObjectPool.Get(COMDT_INGAME_ADVANTAGE_INFO.CLASS_ID);
            }
            this.OperateType = new uint[20];
            this.BigDragonBattleToDeadTimeDtail = new uint[10];
            this.DragonBattleToDeadTimeDtail = new uint[10];
            this.MonsterDeadDetail = new uint[20];
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
            this.dwFBTime = 0;
            this.bKillDragonNum = 0;
            this.bKillBigDragonNum = 0;
            this.bRuneTypeNum = 0;
            if (this.astRuneTypePickUpNum != null)
            {
                for (int i = 0; i < this.astRuneTypePickUpNum.Length; i++)
                {
                    if (this.astRuneTypePickUpNum[i] != null)
                    {
                        this.astRuneTypePickUpNum[i].Release();
                        this.astRuneTypePickUpNum[i] = null;
                    }
                }
            }
            this.bGame30SecNum = 0;
            if (this.astGame30SecDetail != null)
            {
                for (int j = 0; j < this.astGame30SecDetail.Length; j++)
                {
                    if (this.astGame30SecDetail[j] != null)
                    {
                        this.astGame30SecDetail[j].Release();
                        this.astGame30SecDetail[j] = null;
                    }
                }
            }
            this.bEquipNum = 0;
            if (this.astEquipDetail != null)
            {
                for (int k = 0; k < this.astEquipDetail.Length; k++)
                {
                    if (this.astEquipDetail[k] != null)
                    {
                        this.astEquipDetail[k].Release();
                        this.astEquipDetail[k] = null;
                    }
                }
            }
            this.dwTotalInGameCoin = 0;
            this.dwMaxInGameCoin = 0;
            this.dwCamp1TowerFirstAttackTime = 0;
            this.dwCamp2TowerFirstAttackTime = 0;
            this.dwUpRoadTower1DesTime = 0;
            this.dwUpRoadTower2DesTime = 0;
            this.dwUpRoadTower3DesTime = 0;
            this.dwMidRoadTower1DesTime = 0;
            this.dwMidRoadTower2DesTime = 0;
            this.dwMidRoadTower3DesTime = 0;
            this.dwDownRoadTower1DesTime = 0;
            this.dwDownRoadTower2DesTime = 0;
            this.dwDownRoadTower3DesTime = 0;
            this.iTimeUse = 0;
            this.iPauseTimeTotal = 0;
            this.iBuildingAttackRange = 0;
            this.iBuildingAttackDamageMax = 0;
            this.iBuildingAttackDamageMin = 0;
            this.iBuildingHPMax = 0;
            this.iBuildingHPMin = 0;
            this.iBuildingHurtCount = 0;
            this.iBuildingHurtMax = 0;
            this.iBuildingHurtMin = 0;
            this.iBuildingHurtTotal = 0;
            this.iExperienceHPAdd1 = 0;
            this.iExperienceHPAdd2 = 0;
            this.iExperienceHPAdd3 = 0;
            this.iExperienceHPAdd4 = 0;
            this.iExperienceHPAddTotal = 0;
            this.iBossCount = 0;
            this.iBossHPMax = 0;
            this.iBossHPMin = 0;
            this.iBossHurtMax = 0;
            this.iBossHurtMin = 0;
            this.iBossHurtTotal = 0;
            this.iEnemyBuildingHPMax = 0;
            this.iEnemyBuildingHPMin = 0;
            this.iEnemyBuildingHurtMax = 0;
            this.iEnemyBuildingHurtMin = 0;
            this.iEnemyBuildingHurtTotal = 0;
            this.iEnemyHPMax = 0;
            this.iEnemyHPMin = 0;
            this.iBossAttackCount = 0;
            this.iBossUseSkillCount = 0;
            this.iBossAttackMax = 0;
            this.iBossAttackMin = 0;
            this.iBossSkillDamageMax = 0;
            this.iBossSkillDamageMin = 0;
            this.iBossAttackTotal = 0;
            this.iEnemyAttackMax = 0;
            this.iEnemyAttackMin = 0;
            this.iEnemyBuildingAttackRange = 0;
            this.iEnemyBuildingDamageMax = 0;
            this.iEnemyBuildingDamageMin = 0;
            this.iCommunicationCount1 = 0;
            this.iCommunicationCount2 = 0;
            this.iMaxSoldierCnt = 0;
            this.iMaxTowerAttackDistance = 0;
            this.bSelfCampKillTowerCnt = 0;
            this.bQuickBuyItemCnt = 0;
            this.bPanelBuyItemCnt = 0;
            this.bSelfCampKillBaseCnt = 0;
            this.dwSelfCampBaseBlood = 0;
            this.iCurrentDisparity = 0;
            this.bSelfCampHaveWinningFlag = 0;
            this.dwBoardSortNum = 0;
            this.dwClickNum = 0;
            this.bAdvantageNum = 0;
            if (this.astAdvantageDetail != null)
            {
                for (int m = 0; m < this.astAdvantageDetail.Length; m++)
                {
                    if (this.astAdvantageDetail[m] != null)
                    {
                        this.astAdvantageDetail[m].Release();
                        this.astAdvantageDetail[m] = null;
                    }
                }
            }
            this.dwOperateNum = 0;
            this.dwBigDragonBattleToDeadTimeNum = 0;
            this.dwDragonBattleToDeadTimeNum = 0;
            this.dwMonsterDeadNum = 0;
        }

        public override void OnUse()
        {
            if (this.astRuneTypePickUpNum != null)
            {
                for (int i = 0; i < this.astRuneTypePickUpNum.Length; i++)
                {
                    this.astRuneTypePickUpNum[i] = (COMDT_RUNE_PICK_UP_INFO) ProtocolObjectPool.Get(COMDT_RUNE_PICK_UP_INFO.CLASS_ID);
                }
            }
            if (this.astGame30SecDetail != null)
            {
                for (int j = 0; j < this.astGame30SecDetail.Length; j++)
                {
                    this.astGame30SecDetail[j] = (COMDT_GAME_30SEC_INFO) ProtocolObjectPool.Get(COMDT_GAME_30SEC_INFO.CLASS_ID);
                }
            }
            if (this.astEquipDetail != null)
            {
                for (int k = 0; k < this.astEquipDetail.Length; k++)
                {
                    this.astEquipDetail[k] = (COMDT_INGAME_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
                }
            }
            if (this.astAdvantageDetail != null)
            {
                for (int m = 0; m < this.astAdvantageDetail.Length; m++)
                {
                    this.astAdvantageDetail[m] = (COMDT_INGAME_ADVANTAGE_INFO) ProtocolObjectPool.Get(COMDT_INGAME_ADVANTAGE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwFBTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bKillDragonNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bKillDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.KillDragonTime.Length < this.bKillDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bKillDragonNum; i++)
                {
                    type = destBuf.writeUInt32(this.KillDragonTime[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bKillBigDragonNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bKillBigDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.KillBigDragonTime.Length < this.bKillBigDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.bKillBigDragonNum; j++)
                {
                    type = destBuf.writeUInt32(this.KillBigDragonTime[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bRuneTypeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bRuneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRuneTypePickUpNum.Length < this.bRuneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int k = 0; k < this.bRuneTypeNum; k++)
                {
                    type = this.astRuneTypePickUpNum[k].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bGame30SecNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (60 < this.bGame30SecNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astGame30SecDetail.Length < this.bGame30SecNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int m = 0; m < this.bGame30SecNum; m++)
                {
                    type = this.astGame30SecDetail[m].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bEquipNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (6 < this.bEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astEquipDetail.Length < this.bEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int n = 0; n < this.bEquipNum; n++)
                {
                    type = this.astEquipDetail[n].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwTotalInGameCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMaxInGameCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCamp1TowerFirstAttackTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCamp2TowerFirstAttackTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwUpRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwUpRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwUpRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMidRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMidRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMidRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDownRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDownRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDownRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iTimeUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iPauseTimeTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingAttackRange);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingAttackDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingAttackDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHurtCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBuildingHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iExperienceHPAdd1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iExperienceHPAdd2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iExperienceHPAdd3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iExperienceHPAdd4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iExperienceHPAddTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossAttackCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossUseSkillCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossSkillDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossSkillDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iBossAttackTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingAttackRange);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iEnemyBuildingDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iCommunicationCount1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iCommunicationCount2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iMaxSoldierCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iMaxTowerAttackDistance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bSelfCampKillTowerCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bQuickBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bPanelBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bSelfCampKillBaseCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwSelfCampBaseBlood);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iCurrentDisparity);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bSelfCampHaveWinningFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwBoardSortNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwClickNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (30 < this.dwClickNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.ClickDetail.Length < this.dwClickNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num6 = 0; num6 < this.dwClickNum; num6++)
                {
                    type = destBuf.writeUInt32(this.ClickDetail[num6]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bAdvantageNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.bAdvantageNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAdvantageDetail.Length < this.bAdvantageNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num7 = 0; num7 < this.bAdvantageNum; num7++)
                {
                    type = this.astAdvantageDetail[num7].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwOperateNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.dwOperateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.OperateType.Length < this.dwOperateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num8 = 0; num8 < this.dwOperateNum; num8++)
                {
                    type = destBuf.writeUInt32(this.OperateType[num8]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwBigDragonBattleToDeadTimeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwBigDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.BigDragonBattleToDeadTimeDtail.Length < this.dwBigDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num9 = 0; num9 < this.dwBigDragonBattleToDeadTimeNum; num9++)
                {
                    type = destBuf.writeUInt32(this.BigDragonBattleToDeadTimeDtail[num9]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwDragonBattleToDeadTimeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.DragonBattleToDeadTimeDtail.Length < this.dwDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num10 = 0; num10 < this.dwDragonBattleToDeadTimeNum; num10++)
                {
                    type = destBuf.writeUInt32(this.DragonBattleToDeadTimeDtail[num10]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwMonsterDeadNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.dwMonsterDeadNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.MonsterDeadDetail.Length < this.dwMonsterDeadNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int num11 = 0; num11 < this.dwMonsterDeadNum; num11++)
                {
                    type = destBuf.writeUInt32(this.MonsterDeadDetail[num11]);
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
            type = srcBuf.readUInt32(ref this.dwFBTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bKillDragonNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bKillDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.KillDragonTime = new uint[this.bKillDragonNum];
                for (int i = 0; i < this.bKillDragonNum; i++)
                {
                    type = srcBuf.readUInt32(ref this.KillDragonTime[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bKillBigDragonNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bKillBigDragonNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.KillBigDragonTime = new uint[this.bKillBigDragonNum];
                for (int j = 0; j < this.bKillBigDragonNum; j++)
                {
                    type = srcBuf.readUInt32(ref this.KillBigDragonTime[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bRuneTypeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bRuneTypeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int k = 0; k < this.bRuneTypeNum; k++)
                {
                    type = this.astRuneTypePickUpNum[k].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bGame30SecNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (60 < this.bGame30SecNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int m = 0; m < this.bGame30SecNum; m++)
                {
                    type = this.astGame30SecDetail[m].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bEquipNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (6 < this.bEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int n = 0; n < this.bEquipNum; n++)
                {
                    type = this.astEquipDetail[n].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwTotalInGameCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMaxInGameCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCamp1TowerFirstAttackTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCamp2TowerFirstAttackTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUpRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUpRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUpRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMidRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMidRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMidRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDownRoadTower1DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDownRoadTower2DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDownRoadTower3DesTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iTimeUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iPauseTimeTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingAttackRange);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingAttackDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingAttackDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHurtCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBuildingHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExperienceHPAdd1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExperienceHPAdd2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExperienceHPAdd3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExperienceHPAdd4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iExperienceHPAddTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingHurtMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingHurtMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingHurtTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyHPMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyHPMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossAttackCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossUseSkillCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossSkillDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossSkillDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBossAttackTotal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyAttackMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyAttackMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingAttackRange);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingDamageMax);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnemyBuildingDamageMin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCommunicationCount1);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCommunicationCount2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMaxSoldierCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMaxTowerAttackDistance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSelfCampKillTowerCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bQuickBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPanelBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSelfCampKillBaseCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSelfCampBaseBlood);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCurrentDisparity);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSelfCampHaveWinningFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBoardSortNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwClickNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (30 < this.dwClickNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.ClickDetail = new uint[this.dwClickNum];
                for (int num6 = 0; num6 < this.dwClickNum; num6++)
                {
                    type = srcBuf.readUInt32(ref this.ClickDetail[num6]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bAdvantageNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.bAdvantageNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int num7 = 0; num7 < this.bAdvantageNum; num7++)
                {
                    type = this.astAdvantageDetail[num7].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwOperateNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.dwOperateNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.OperateType = new uint[this.dwOperateNum];
                for (int num8 = 0; num8 < this.dwOperateNum; num8++)
                {
                    type = srcBuf.readUInt32(ref this.OperateType[num8]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwBigDragonBattleToDeadTimeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwBigDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.BigDragonBattleToDeadTimeDtail = new uint[this.dwBigDragonBattleToDeadTimeNum];
                for (int num9 = 0; num9 < this.dwBigDragonBattleToDeadTimeNum; num9++)
                {
                    type = srcBuf.readUInt32(ref this.BigDragonBattleToDeadTimeDtail[num9]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwDragonBattleToDeadTimeNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwDragonBattleToDeadTimeNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.DragonBattleToDeadTimeDtail = new uint[this.dwDragonBattleToDeadTimeNum];
                for (int num10 = 0; num10 < this.dwDragonBattleToDeadTimeNum; num10++)
                {
                    type = srcBuf.readUInt32(ref this.DragonBattleToDeadTimeDtail[num10]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwMonsterDeadNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.dwMonsterDeadNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.MonsterDeadDetail = new uint[this.dwMonsterDeadNum];
                for (int num11 = 0; num11 < this.dwMonsterDeadNum; num11++)
                {
                    type = srcBuf.readUInt32(ref this.MonsterDeadDetail[num11]);
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

