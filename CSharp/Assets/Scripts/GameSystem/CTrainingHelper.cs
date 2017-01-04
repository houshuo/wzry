namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CTrainingHelper : Singleton<CTrainingHelper>
    {
        private GameObject m_aiBtnToggle;
        private bool m_aiToggleFlag;
        private GameObject m_cdBtnToggle;
        private bool m_cdToggleFlag;
        private CUIFormScript m_form;
        private GameObject m_invincibleBtnToggle;
        private bool m_invincibleToggleFlag;
        private GameObject m_openBtn;
        private GameObject m_panelObj;
        private GameObject m_soldierBtnToggle;
        private bool m_soldierToggleFlag;

        private int ActTarListToMask(List<EActTarget> inList)
        {
            int num = 0;
            List<EActTarget>.Enumerator enumerator = inList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EActTarget current = enumerator.Current;
                num |= ((int) 1) << current;
            }
            return num;
        }

        private int ActTarListToMask(params EActTarget[] inList)
        {
            return this.ActTarListToMask(new List<EActTarget>(inList));
        }

        private List<EActTarget> ActTarMaskToList(int actTarMask)
        {
            List<EActTarget> list = new List<EActTarget>();
            int num = 2;
            for (int i = 0; i < num; i++)
            {
                if ((actTarMask & (((int) 1) << i)) > 0)
                {
                    list.Add((EActTarget) ((byte) i));
                }
            }
            return list;
        }

        private int CheatActListToMask(List<ECheatAct> inList)
        {
            int num = 0;
            List<ECheatAct>.Enumerator enumerator = inList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ECheatAct current = enumerator.Current;
                num |= ((int) 1) << current;
            }
            return num;
        }

        private int CheatActListToMask(params ECheatAct[] inList)
        {
            return this.CheatActListToMask(new List<ECheatAct>(inList));
        }

        private List<ECheatAct> CheatActMaskToList(int cheatActMask)
        {
            List<ECheatAct> list = new List<ECheatAct>();
            int num = 10;
            for (int i = 0; i < num; i++)
            {
                if ((cheatActMask & (((int) 1) << i)) > 0)
                {
                    list.Add((ECheatAct) ((byte) i));
                }
            }
            return list;
        }

        private void DoCheatAction(ECheatAct inAct, EActTarget inTar, int inParam)
        {
            COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
            COM_PLAYERCAMP com_playercamp2 = (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2;
            COM_PLAYERCAMP inCamp = (inTar != EActTarget.Hostile) ? playerCamp : com_playercamp2;
            switch (inAct)
            {
                case ECheatAct.LevelUp:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.LevelUp));
                    break;

                case ECheatAct.SetLevel:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ResetSkillLevel));
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.SetLevel));
                    break;

                case ECheatAct.FullHp:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.FullHp));
                    break;

                case ECheatAct.FullEp:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.FullEp));
                    break;

                case ECheatAct.ToggleInvincible:
                {
                    SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                    bool flag = (curLvelContext != null) && curLvelContext.IsMobaMode();
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ActorControl is HeroWrapper))
                    {
                        HeroWrapper actorControl = (HeroWrapper) hostPlayer.Captain.handle.ActorControl;
                        actorControl.bGodMode = !actorControl.bGodMode;
                    }
                    this.m_invincibleToggleFlag = !this.m_invincibleToggleFlag;
                    this.RefreshBtnToggleInvincible();
                    break;
                }
                case ECheatAct.ToggleAi:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ToggleAi));
                    this.m_aiToggleFlag = !this.m_aiToggleFlag;
                    this.RefreshBtnToggleAi();
                    break;

                case ECheatAct.ToggleSoldier:
                    Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(this.m_soldierToggleFlag);
                    this.m_soldierToggleFlag = !this.m_soldierToggleFlag;
                    this.RefreshBtnToggleSoldier();
                    break;

                case ECheatAct.ResetSoldier:
                    Singleton<BattleLogic>.instance.mapLogic.ResetSoldierRegion();
                    Singleton<BattleLogic>.instance.dynamicProperty.ResetTimer();
                    Singleton<GameObjMgr>.GetInstance().KillSoldiers();
                    OrganVisiter(COM_PLAYERCAMP.COM_PLAYERCAMP_1, inParam, new Action<ActorRoot, int>(CTrainingHelper.ReviveTower));
                    OrganVisiter(COM_PLAYERCAMP.COM_PLAYERCAMP_2, inParam, new Action<ActorRoot, int>(CTrainingHelper.ReviveTower));
                    break;

                case ECheatAct.AddGold:
                {
                    SLevelContext context2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
                    bool flag2 = (context2 != null) && context2.IsMobaMode();
                    Player player3 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (((player3 != null) && (player3.Captain != 0)) && (player3.Captain.handle.ValueComponent != null))
                    {
                        player3.Captain.handle.ValueComponent.ChangeGoldCoinInBattle(0x3e8, true, true, new Vector3(), false);
                    }
                    break;
                }
                case ECheatAct.ToggleZeroCd:
                    HeroVisiter(inCamp, inParam, new Action<ActorRoot, int>(CTrainingHelper.ToggleZeroCd));
                    this.m_cdToggleFlag = !this.m_cdToggleFlag;
                    this.RefreshBtnToggleCd();
                    break;
            }
        }

        private static void FullEp(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.ValueComponent != null))
            {
                inActor.ValueComponent.RecoverEp();
            }
        }

        private static void FullHp(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.ValueComponent != null))
            {
                inActor.ValueComponent.RecoverHp();
            }
        }

        private static void HeroVisiter(COM_PLAYERCAMP inCamp, int inParam, Action<ActorRoot, int> inFunc)
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.HeroActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != 0)
                {
                    PoolObjHandle<ActorRoot> current = enumerator.Current;
                    ActorRoot handle = current.handle;
                    if (handle.TheActorMeta.ActorCamp == inCamp)
                    {
                        inFunc(handle, inParam);
                    }
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperOpen, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperClose, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperInit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperInit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperUninit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperUninit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Training_HelperCheat, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperCheat));
        }

        private void InitBtnToggle()
        {
            this.m_aiToggleFlag = false;
            this.m_soldierToggleFlag = false;
            this.m_invincibleToggleFlag = false;
            this.m_cdToggleFlag = false;
            this.RefreshBtnToggleAi();
            this.RefreshBtnToggleCd();
            this.RefreshBtnToggleInvincible();
            this.RefreshBtnToggleSoldier();
        }

        private void InitGeneralFuncList()
        {
            DebugHelper.Assert(this.m_panelObj != null);
            if (this.m_panelObj != null)
            {
                Transform transform = this.m_panelObj.transform.FindChild("GeneralFuncList");
                CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
                ECheatAct[] inList = new ECheatAct[] { ECheatAct.ToggleSoldier, ECheatAct.ResetSoldier };
                component.m_onClickEventParams.tag = this.CheatActListToMask(inList);
                component.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                this.m_soldierBtnToggle = component.gameObject;
                CUIMiniEventScript script2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray2 = new ECheatAct[] { ECheatAct.AddGold };
                script2.m_onClickEventParams.tag = this.CheatActListToMask(actArray2);
                script2.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                script2.m_onClickEventParams.tag3 = 0x3e8;
            }
        }

        private void InitHostileFuncList()
        {
            DebugHelper.Assert(this.m_panelObj != null);
            if (this.m_panelObj != null)
            {
                Transform transform = this.m_panelObj.transform.FindChild("HostileFuncList");
                CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
                ECheatAct[] inList = new ECheatAct[3];
                inList[1] = ECheatAct.FullHp;
                inList[2] = ECheatAct.FullEp;
                component.m_onClickEventParams.tag = this.CheatActListToMask(inList);
                EActTarget[] targetArray1 = new EActTarget[] { EActTarget.Hostile };
                component.m_onClickEventParams.tag2 = this.ActTarListToMask(targetArray1);
                CUIMiniEventScript script2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray2 = new ECheatAct[] { ECheatAct.SetLevel, ECheatAct.FullHp, ECheatAct.FullEp };
                script2.m_onClickEventParams.tag = this.CheatActListToMask(actArray2);
                EActTarget[] targetArray2 = new EActTarget[] { EActTarget.Hostile };
                script2.m_onClickEventParams.tag2 = this.ActTarListToMask(targetArray2);
                script2.m_onClickEventParams.tag3 = 1;
                CUIMiniEventScript script3 = transform.GetChild(2).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray3 = new ECheatAct[] { ECheatAct.ToggleAi };
                script3.m_onClickEventParams.tag = this.CheatActListToMask(actArray3);
                EActTarget[] targetArray3 = new EActTarget[] { EActTarget.Hostile };
                script3.m_onClickEventParams.tag2 = this.ActTarListToMask(targetArray3);
                this.m_aiBtnToggle = script3.gameObject;
            }
        }

        private void InitSelfFuncList()
        {
            DebugHelper.Assert(this.m_panelObj != null);
            if (this.m_panelObj != null)
            {
                Transform transform = this.m_panelObj.transform.FindChild("SelfFuncList");
                CUIMiniEventScript component = transform.GetChild(0).GetComponent<CUIMiniEventScript>();
                ECheatAct[] inList = new ECheatAct[3];
                inList[1] = ECheatAct.FullHp;
                inList[2] = ECheatAct.FullEp;
                component.m_onClickEventParams.tag = this.CheatActListToMask(inList);
                component.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                CUIMiniEventScript script2 = transform.GetChild(1).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray2 = new ECheatAct[] { ECheatAct.SetLevel, ECheatAct.FullHp, ECheatAct.FullEp };
                script2.m_onClickEventParams.tag = this.CheatActListToMask(actArray2);
                script2.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                script2.m_onClickEventParams.tag3 = 1;
                CUIMiniEventScript script3 = transform.GetChild(2).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray3 = new ECheatAct[] { ECheatAct.ToggleZeroCd };
                script3.m_onClickEventParams.tag = this.CheatActListToMask(actArray3);
                script3.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                this.m_cdBtnToggle = script3.gameObject;
                CUIMiniEventScript script4 = transform.GetChild(3).GetComponent<CUIMiniEventScript>();
                ECheatAct[] actArray4 = new ECheatAct[] { ECheatAct.ToggleInvincible };
                script4.m_onClickEventParams.tag = this.CheatActListToMask(actArray4);
                script4.m_onClickEventParams.tag2 = this.ActTarListToMask(new EActTarget[1]);
                this.m_invincibleBtnToggle = script4.gameObject;
            }
        }

        private static void LevelUp(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.ValueComponent != null))
            {
                inActor.ValueComponent.ForceSoulLevelUp();
            }
        }

        private void OnTrainingHelperCheat(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            int actTarMask = uiEvent.m_eventParams.tag2;
            int inParam = uiEvent.m_eventParams.tag3;
            List<ECheatAct> list = this.CheatActMaskToList(tag);
            List<EActTarget> list2 = this.ActTarMaskToList(actTarMask);
            List<ECheatAct>.Enumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ECheatAct current = enumerator.Current;
                List<EActTarget>.Enumerator enumerator2 = list2.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    EActTarget inTar = enumerator2.Current;
                    this.DoCheatAction(current, inTar, inParam);
                }
            }
        }

        private void OnTrainingHelperClose(CUIEvent uiEvent)
        {
            this.m_openBtn.CustomSetActive(true);
            this.m_panelObj.CustomSetActive(false);
            if (this.m_form != null)
            {
                Transform transform = this.m_form.transform.FindChild("MapPanel");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(true);
                }
            }
        }

        private void OnTrainingHelperInit(CUIEvent uiEvent)
        {
            if (this.m_form == null)
            {
                this.m_form = Singleton<CBattleSystem>.instance.FightFormScript;
                DebugHelper.Assert(this.m_form != null);
                this.m_openBtn = this.m_form.transform.FindChild("Panel_Prop/ButtonOpen").gameObject;
                this.m_panelObj = this.m_form.transform.FindChild("Panel_Prop/Panel_BaseProp").gameObject;
                this.InitHostileFuncList();
                this.InitSelfFuncList();
                this.InitGeneralFuncList();
                this.InitBtnToggle();
                this.m_form.transform.FindChild("Panel_Prop").gameObject.CustomSetActive(true);
                Transform transform = this.m_form.transform.FindChild("MapPanel");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        private void OnTrainingHelperOpen(CUIEvent uiEvent)
        {
            this.m_openBtn.CustomSetActive(false);
            this.m_panelObj.CustomSetActive(true);
            if (this.m_form != null)
            {
                Transform transform = this.m_form.transform.FindChild("MapPanel");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        private void OnTrainingHelperUninit(CUIEvent uiEvent)
        {
            if (this.m_form != null)
            {
                this.m_form.transform.FindChild("Panel_Prop").gameObject.CustomSetActive(false);
                this.m_form = null;
                this.m_openBtn = null;
                this.m_panelObj = null;
            }
        }

        private static void OrganVisiter(COM_PLAYERCAMP inCamp, int inParam, Action<ActorRoot, int> inFunc)
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = Singleton<GameObjMgr>.instance.OrganActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != 0)
                {
                    PoolObjHandle<ActorRoot> current = enumerator.Current;
                    ActorRoot handle = current.handle;
                    if (handle.TheActorMeta.ActorCamp == inCamp)
                    {
                        inFunc(handle, inParam);
                    }
                }
            }
        }

        private void RefreshBtnToggle(bool bFlag, GameObject inBtn)
        {
            if (inBtn != null)
            {
                GameObject gameObject = inBtn.transform.GetChild(0).gameObject;
                GameObject obj3 = inBtn.transform.GetChild(1).gameObject;
                if (bFlag)
                {
                    gameObject.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                }
                else
                {
                    gameObject.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                }
            }
        }

        private void RefreshBtnToggleAi()
        {
            this.RefreshBtnToggle(this.m_aiToggleFlag, this.m_aiBtnToggle);
        }

        private void RefreshBtnToggleCd()
        {
            this.RefreshBtnToggle(this.m_cdToggleFlag, this.m_cdBtnToggle);
        }

        private void RefreshBtnToggleInvincible()
        {
            this.RefreshBtnToggle(this.m_invincibleToggleFlag, this.m_invincibleBtnToggle);
        }

        private void RefreshBtnToggleSoldier()
        {
            this.RefreshBtnToggle(this.m_soldierToggleFlag, this.m_soldierBtnToggle);
        }

        private static void ResetSkillLevel(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.SkillControl != null))
            {
                inActor.SkillControl.ResetSkillLevel();
            }
        }

        private static void ReviveTower(ActorRoot inActor, int inParam)
        {
            if (((inActor != null) && (inActor.ActorControl != null)) && inActor.ActorControl.IsDeadState)
            {
                inActor.ActorControl.Revive(false);
                inActor.RecoverOriginalActorMesh();
                if (inActor.ActorMesh != null)
                {
                    inActor.ActorMesh.SetLayer("Actor", "Particles", true);
                }
            }
        }

        private static void SetLevel(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.ValueComponent != null))
            {
                inActor.ValueComponent.ForceSetSoulLevel(inParam);
            }
        }

        private static void SpawnDynamicActor(ref ActorSpawnInfo inSpawnInfo)
        {
            ActorMeta actorMeta = new ActorMeta {
                ActorType = inSpawnInfo.ActorType,
                ConfigId = inSpawnInfo.ConfigId,
                ActorCamp = inSpawnInfo.CampType
            };
            VInt3 bornPos = inSpawnInfo.BornPos;
            VInt3 bornDir = inSpawnInfo.BornDir;
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref actorMeta, bornPos, bornDir, false, true);
            if (actor != 0)
            {
                actor.handle.InitActor();
                actor.handle.PrepareFight();
                Singleton<GameObjMgr>.instance.AddActor(actor);
                actor.handle.StartFight();
            }
        }

        private static void ToggleAi(ActorRoot inActor, int inParam)
        {
            if (((inActor != null) && (inActor.ActorAgent != null)) && (inActor.ActorControl != null))
            {
                if (!inActor.ActorAgent.bPaused && !inActor.ActorControl.IsDeadState)
                {
                    inActor.ActorControl.CmdStopMove();
                    if (inActor.AnimControl != null)
                    {
                        PlayAnimParam param = new PlayAnimParam {
                            animName = "Idle",
                            blendTime = 0f,
                            loop = true,
                            layer = 0,
                            speed = 1f
                        };
                        inActor.AnimControl.Play(param);
                    }
                }
                inActor.ActorAgent.SetPaused(!inActor.ActorAgent.bPaused);
            }
        }

        private static void ToggleZeroCd(ActorRoot inActor, int inParam)
        {
            if ((inActor != null) && (inActor.SkillControl != null))
            {
                inActor.SkillControl.ToggleZeroCd();
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperCheat, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperCheat));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperUninit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperUninit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperInit, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperInit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperOpen, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Training_HelperClose, new CUIEventManager.OnUIEventHandler(this.OnTrainingHelperClose));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ActorSpawnInfo
        {
            public int ConfigId;
            public ActorTypeDef ActorType;
            public COM_PLAYERCAMP CampType;
            public VInt3 BornPos;
            public VInt3 BornDir;
            public ActorSpawnInfo(int inCfgId, ActorTypeDef inActorType, COM_PLAYERCAMP inCampType, VInt3 inPos, VInt3 inDir)
            {
                this.ConfigId = inCfgId;
                this.ActorType = inActorType;
                this.CampType = inCampType;
                this.BornPos = inPos;
                this.BornDir = inDir;
            }
        }

        private enum EActTarget : byte
        {
            Count = 2,
            Friendly = 0,
            Hostile = 1
        }

        private enum ECheatAct : byte
        {
            AddGold = 8,
            Count = 10,
            FullEp = 3,
            FullHp = 2,
            LevelUp = 0,
            ResetSoldier = 7,
            SetLevel = 1,
            ToggleAi = 5,
            ToggleInvincible = 4,
            ToggleSoldier = 6,
            ToggleZeroCd = 9
        }
    }
}

