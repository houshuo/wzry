namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class FRAME_CMD_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x22;
        public static readonly uint CURRVERSION = 1;
        public ProtocolObject dataObject;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.pack(ref destBuf, cutVer);
            }
            return type;
        }

        public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(selector, ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public ProtocolObject select(long selector)
        {
            if (selector <= 0x22L)
            {
                this.select_1_34(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_34(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 0x22L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYERMOVE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYERMOVE) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERMOVE.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYERMOVEDIRECTION))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYERMOVEDIRECTION) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERMOVEDIRECTION.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is FRAMEPKG_CMD_STOPMOVE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_STOPMOVE) ProtocolObjectPool.Get(FRAMEPKG_CMD_STOPMOVE.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is FRAMEPKG_CMD_ATTACKPOSITION))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_ATTACKPOSITION) ProtocolObjectPool.Get(FRAMEPKG_CMD_ATTACKPOSITION.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is FRAMEPKG_CMD_ATTACKACTOR))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_ATTACKACTOR) ProtocolObjectPool.Get(FRAMEPKG_CMD_ATTACKACTOR.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is FRAMEPKG_CMD_LEARNSKILL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_LEARNSKILL) ProtocolObjectPool.Get(FRAMEPKG_CMD_LEARNSKILL.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is FRAMEPKG_CMD_USECURVETRACKSKILL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_USECURVETRACKSKILL) ProtocolObjectPool.Get(FRAMEPKG_CMD_USECURVETRACKSKILL.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is FRAMEPKG_CMD_USECOMMONATTACK))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_USECOMMONATTACK) ProtocolObjectPool.Get(FRAMEPKG_CMD_USECOMMONATTACK.CLASS_ID);
                        }
                        return;

                    case 10:
                        if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_AI))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SWITCH_AI) ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_AI.CLASS_ID);
                        }
                        return;

                    case 11:
                        if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_CAPTAIN))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SWITCH_CAPTAIN) ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_CAPTAIN.CLASS_ID);
                        }
                        return;

                    case 12:
                        if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_SUPER_KILLER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SWITCH_SUPER_KILLER) ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_SUPER_KILLER.CLASS_ID);
                        }
                        return;

                    case 13:
                        if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_GOD_MODE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SWITCH_GOD_MODE) ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_GOD_MODE.CLASS_ID);
                        }
                        return;

                    case 14:
                        if (!(this.dataObject is FRAMEPKG_CMD_LEARN_TALENT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_LEARN_TALENT) ProtocolObjectPool.Get(FRAMEPKG_CMD_LEARN_TALENT.CLASS_ID);
                        }
                        return;

                    case 15:
                        if (!(this.dataObject is FRAMEPKG_CMD_TESTCOMMANDDELAY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_TESTCOMMANDDELAY) ProtocolObjectPool.Get(FRAMEPKG_CMD_TESTCOMMANDDELAY.CLASS_ID);
                        }
                        return;

                    case 0x10:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYERRUNAWAY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYERRUNAWAY) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERRUNAWAY.CLASS_ID);
                        }
                        return;

                    case 0x11:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYERDISCONNECT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYERDISCONNECT) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERDISCONNECT.CLASS_ID);
                        }
                        return;

                    case 0x12:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYERRECONNECT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYERRECONNECT) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERRECONNECT.CLASS_ID);
                        }
                        return;

                    case 0x13:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYATTACKTARGETMODE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYATTACKTARGETMODE) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYATTACKTARGETMODE.CLASS_ID);
                        }
                        return;

                    case 20:
                        if (!(this.dataObject is FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER) ProtocolObjectPool.Get(FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER.CLASS_ID);
                        }
                        return;

                    case 0x15:
                        if (!(this.dataObject is FRAMEPKG_CMD_ASSISTSTATECHG))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_ASSISTSTATECHG) ProtocolObjectPool.Get(FRAMEPKG_CMD_ASSISTSTATECHG.CLASS_ID);
                        }
                        return;

                    case 0x16:
                        if (!(this.dataObject is FRAMEPKG_CMD_CHGAUTOAI))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_CHGAUTOAI) ProtocolObjectPool.Get(FRAMEPKG_CMD_CHGAUTOAI.CLASS_ID);
                        }
                        return;

                    case 0x17:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_BUY_EQUIP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYER_BUY_EQUIP) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_BUY_EQUIP.CLASS_ID);
                        }
                        return;

                    case 0x18:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_SELL_EQUIP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYER_SELL_EQUIP) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_SELL_EQUIP.CLASS_ID);
                        }
                        return;

                    case 0x19:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE.CLASS_ID);
                        }
                        return;

                    case 0x1a:
                        if (!(this.dataObject is FRAMEPKG_CMD_SET_SKILL_LEVEL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SET_SKILL_LEVEL) ProtocolObjectPool.Get(FRAMEPKG_CMD_SET_SKILL_LEVEL.CLASS_ID);
                        }
                        return;

                    case 0x1b:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE.CLASS_ID);
                        }
                        return;

                    case 0x1c:
                        if (!(this.dataObject is FFRAMEPKG_CMD_LOCKATTACKTARGET))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FFRAMEPKG_CMD_LOCKATTACKTARGET) ProtocolObjectPool.Get(FFRAMEPKG_CMD_LOCKATTACKTARGET.CLASS_ID);
                        }
                        return;

                    case 0x1d:
                        if (!(this.dataObject is FRAMEPKG_CMD_Signal_Btn_Position))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_Signal_Btn_Position) ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_Btn_Position.CLASS_ID);
                        }
                        return;

                    case 30:
                        if (!(this.dataObject is FRAMEPKG_CMD_Signal_MiniMap_Position))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_Signal_MiniMap_Position) ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_MiniMap_Position.CLASS_ID);
                        }
                        return;

                    case 0x1f:
                        if (!(this.dataObject is FRAMEPKG_CMD_Signal_MiniMap_Target))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_Signal_MiniMap_Target) ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_MiniMap_Target.CLASS_ID);
                        }
                        return;

                    case 0x20:
                        if (!(this.dataObject is FRAMEPKG_CMD_SVRNTF_GAMEOVER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_SVRNTF_GAMEOVER) ProtocolObjectPool.Get(FRAMEPKG_CMD_SVRNTF_GAMEOVER.CLASS_ID);
                        }
                        return;

                    case 0x21:
                        if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP) ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP.CLASS_ID);
                        }
                        return;
                }
            }
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.unpack(ref srcBuf, cutVer);
            }
            return type;
        }

        public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(selector, ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }

        public FRAMEPKG_CMD_ASSISTSTATECHG stCmdAssistStateChg
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_ASSISTSTATECHG);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_CHGAUTOAI stCmdChgAutoAI
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_CHGAUTOAI);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE stCmdPlayCommonAttackMode
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE stCmdPlayerAddGoldCoinInBattle
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_ATTACKACTOR stCmdPlayerAttackPlayer
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_ATTACKACTOR);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_ATTACKPOSITION stCmdPlayerAttackPosition
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_ATTACKPOSITION);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYATTACKTARGETMODE stCmdPlayerAttackTargetMode
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYATTACKTARGETMODE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYER_BUY_EQUIP stCmdPlayerBuyEquip
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYER_BUY_EQUIP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP stCmdPlayerBuyHorizonEquip
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYERDISCONNECT stCmdPlayerDisconnect
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYERDISCONNECT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_LEARNSKILL stCmdPlayerLearnSkill
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_LEARNSKILL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_LEARN_TALENT stCmdPlayerLearnTalent
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_LEARN_TALENT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FFRAMEPKG_CMD_LOCKATTACKTARGET stCmdPlayerLockAttackTarget
        {
            get
            {
                return (this.dataObject as FFRAMEPKG_CMD_LOCKATTACKTARGET);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYERMOVE stCmdPlayerMove
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYERMOVE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYERMOVEDIRECTION stCmdPlayerMoveDirection
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYERMOVEDIRECTION);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYERRECONNECT stCmdPlayerReconnect
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYERRECONNECT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYERRUNAWAY stCmdPlayerRunaway
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYERRUNAWAY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_PLAYER_SELL_EQUIP stCmdPlayerSellEquip
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_PLAYER_SELL_EQUIP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_STOPMOVE stCmdPlayerStopMove
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_STOPMOVE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SWITCH_CAPTAIN stCmdPlayerSwitchCaptain
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SWITCH_CAPTAIN);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SWITCH_GOD_MODE stCmdPlayerSwitchGodMode
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SWITCH_GOD_MODE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SWITCH_SUPER_KILLER stCmdPlayerSwitchSuperKiller
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SWITCH_SUPER_KILLER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SWITCH_AI stCmdPlayerSwithAutoAI
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SWITCH_AI);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_USECOMMONATTACK stCmdPlayerUseCommonAttack
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_USECOMMONATTACK);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_USECURVETRACKSKILL stCmdPlayerUseCurveTrackSkill
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_USECURVETRACKSKILL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SET_SKILL_LEVEL stCmdSetSkillLevel
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SET_SKILL_LEVEL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_Signal_Btn_Position stCmdSignalBtnPosition
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_Signal_Btn_Position);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_Signal_MiniMap_Position stCmdSignalMiniMapPosition
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_Signal_MiniMap_Position);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_Signal_MiniMap_Target stCmdSignalMiniMapTarget
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_Signal_MiniMap_Target);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER stCmdSvrNtfChgFrameLater
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_SVRNTF_GAMEOVER stCmdSvrNtfGameover
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_SVRNTF_GAMEOVER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public FRAMEPKG_CMD_TESTCOMMANDDELAY stCmdTestCommandDelay
        {
            get
            {
                return (this.dataObject as FRAMEPKG_CMD_TESTCOMMANDDELAY);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

