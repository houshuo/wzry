namespace Assets.Scripts.Framework
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using Pathfinding.RVO;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class GameLogic : Singleton<GameLogic>, IGameModule
    {
        private bool bHasTailPart;
        public bool bInLogicTick;
        public uint GameRunningTick;
        public uint HashCheckFreq = 500;
        private int nTailPartDelta;
        private BattleLogic optBattle;
        private DropItemMgr optDropMgr;
        private LobbyLogic optLobby;
        private GameObjMgr optObjMgr;
        private CRoleInfoManager optRoleMgr;

        public void ClearLogicData()
        {
            Singleton<CRoleInfoManager>.instance.ClearMasterRoleInfo();
            Singleton<CAdventureSys>.GetInstance().Clear();
            Singleton<CMatchingSystem>.GetInstance().Clear();
            Singleton<CRoomSystem>.GetInstance().Clear();
            Singleton<CSymbolSystem>.GetInstance().Clear();
            Singleton<ActivitySys>.GetInstance().Clear();
            Singleton<CFriendContoller>.instance.ClearAll();
            Singleton<CChatController>.instance.ClearAll();
            Singleton<BurnExpeditionController>.instance.ClearAll();
            Singleton<InBattleMsgMgr>.instance.ClearData();
            if (MonoSingleton<NewbieGuideManager>.HasInstance())
            {
                MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
                MonoSingleton<NewbieGuideManager>.ClearDestroy();
            }
            Singleton<CMailSys>.instance.Clear();
            Singleton<CTaskSys>.instance.Clear();
            Singleton<CGuildSystem>.GetInstance().Clear();
            GameDataMgr.ClearServerResData();
            Singleton<CMallFactoryShopController>.GetInstance().Clear();
            Singleton<CMallMysteryShop>.GetInstance().Clear();
            Singleton<RankingSystem>.GetInstance().ClearAll();
            Singleton<CLobbySystem>.GetInstance().Clear();
            Singleton<CUnionBattleRankSystem>.GetInstance().Clear();
            Singleton<HeadIconSys>.instance.Clear();
            Singleton<CLoudSpeakerSys>.instance.Clear();
            Singleton<COBSystem>.instance.Clear();
            Singleton<CInviteSystem>.instance.Clear();
            Singleton<CArenaSystem>.instance.Clear();
        }

        public override void Init()
        {
            this.optObjMgr = Singleton<GameObjMgr>.GetInstance();
            this.optLobby = Singleton<LobbyLogic>.GetInstance();
            this.optBattle = Singleton<BattleLogic>.GetInstance();
            Singleton<GameInput>.GetInstance();
            this.optDropMgr = Singleton<DropItemMgr>.GetInstance();
            this.optRoleMgr = Singleton<CRoleInfoManager>.GetInstance();
        }

        public void LateUpdate()
        {
            this.optObjMgr.LateUpdate();
        }

        public void OnPlayerLogout()
        {
            Singleton<NetworkModule>.GetInstance().CloseAllServerConnect();
            this.ClearLogicData();
        }

        public void OpenLobby()
        {
            Singleton<LobbyLogic>.GetInstance().OpenLobby();
        }

        private void SampleFrameSyncData()
        {
            if ((Singleton<FrameSynchr>.instance.bActive && !Singleton<WatchController>.GetInstance().IsWatching) && (((Singleton<FrameSynchr>.instance.CurFrameNum % this.HashCheckFreq) == 0) && Singleton<BattleLogic>.instance.isFighting))
            {
                COM_PLAYERCAMP com_playercamp;
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                int num = (1 + (heroActors.Count * 6)) + 1;
                int[] src = new int[num];
                int num2 = 0;
                src[num2++] = (int) Singleton<FrameSynchr>.instance.CurFrameNum;
                for (int i = 0; i < heroActors.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = heroActors[i];
                    ActorRoot root = handle.handle;
                    src[num2++] = (int) root.ObjID;
                    src[num2++] = root.location.x;
                    src[num2++] = root.location.y;
                    src[num2++] = root.location.z;
                    src[num2++] = (int) root.ActorControl.myBehavior;
                    src[num2++] = root.ValueComponent.actorHp;
                }
                src[num2++] = MonoSingleton<ActionManager>.GetInstance().ActionUpdatingCount;
                byte[] dst = new byte[num * 4];
                Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
                MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                provider.Initialize();
                provider.TransformFinalBlock(dst, 0, dst.Length);
                ulong num4 = (ulong) BitConverter.ToInt64(provider.Hash, 0);
                ulong num5 = (ulong) BitConverter.ToInt64(provider.Hash, 8);
                ulong num6 = num4 ^ num5;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x500);
                msg.stPkgData.stRelayHashChk.dwKFrapsNo = Singleton<FrameSynchr>.instance.CurFrameNum;
                msg.stPkgData.stRelayHashChk.ullHashToChk = num6;
                CampInfo campInfoByCamp = null;
                campInfoByCamp = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
                int headPoints = campInfoByCamp.HeadPoints;
                int num8 = 0;
                if (campInfoByCamp.campType == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    com_playercamp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                }
                else
                {
                    com_playercamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                }
                num8 = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(com_playercamp).HeadPoints;
                int num9 = headPoints - num8;
                msg.stPkgData.stRelayHashChk.iCampKillCntDiff = num9;
                Singleton<NetworkModule>.instance.SendGameMsg(ref msg, 0);
            }
        }

        public void UpdateFrame()
        {
        }

        public void UpdateLogic(int nDelta, bool bPart)
        {
            this.GameRunningTick += (uint) nDelta;
            DebugHelper.Assert(!this.bHasTailPart);
            this.UpdateLogicPartA(nDelta);
            this.bHasTailPart = true;
            this.nTailPartDelta = nDelta;
            if (!bPart)
            {
                this.UpdateTails();
            }
        }

        private void UpdateLogicPartA(int nDelta)
        {
            this.bInLogicTick = true;
            Singleton<GameEventSys>.instance.UpdateEvent();
            ActionManager.Instance.UpdateLogic(nDelta);
            if (MTileHandlerHelper.Instance != null)
            {
                MTileHandlerHelper.Instance.UpdateLogic();
            }
            if (RVOSimulator.Instance != null)
            {
                RVOSimulator.Instance.UpdateLogic(nDelta);
            }
            this.optLobby.UpdateLogic(nDelta);
            this.optBattle.UpdateLogic(nDelta);
            this.bInLogicTick = false;
        }

        private void UpdateLogicPartB(int nDelta)
        {
            this.bInLogicTick = true;
            this.optObjMgr.UpdateLogic(nDelta);
            this.optDropMgr.UpdateLogic(nDelta);
            this.optRoleMgr.UpdateLogic(nDelta);
            if (Singleton<ShenFuSystem>.instance != null)
            {
                Singleton<ShenFuSystem>.instance.UpdateLogic(nDelta);
            }
            Singleton<CTimerManager>.instance.UpdateLogic(nDelta);
            this.SampleFrameSyncData();
            this.bInLogicTick = false;
        }

        public bool UpdateTails()
        {
            if (!this.bHasTailPart)
            {
                return false;
            }
            try
            {
                this.UpdateLogicPartB(this.nTailPartDelta);
                this.bHasTailPart = false;
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception in GameLogic.UpdateTails:{0}, \n {1}", inParameters);
                this.bHasTailPart = false;
            }
            return true;
        }
    }
}

