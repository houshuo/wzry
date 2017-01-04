namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using behaviac;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [TypeMetaInfo("ObjWrapperAgent", "具有ObjWrapper能力的代理")]
    public class ObjAgent : BTBaseAgent, IPooledMonoBehaviour, IActorComponent
    {
        private const int DENGER_COOL_TICKS = 30;
        private const int Hp_Rate = 0x2710;
        private int m_closeToTargetFrame;
        private PoolObjHandle<AGE.Action> m_currentAction = new PoolObjHandle<AGE.Action>();
        private int m_dengerCoolTick = 30;
        public int m_frame;
        public bool m_isActionPlaying;
        private int m_sound_Interval = 0x4650;
        public ObjWrapper m_wrapper;
        public const string path_HeadExclamationMark = "Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01";
        public const long SKILL_ALLOWABLE_ERROR_VALUE = 0x2af8L;
        private const int Sound_Interval = 0x4650;

        [MethodMetaInfo("中断当前正在施放的技能", "")]
        public bool AbortCurUseSkill()
        {
            return true;
        }

        [MethodMetaInfo("用其他技能中断当前正在施放的技能", "")]
        public EBTStatus AbortCurUseSkillByType(SkillAbortType abortType)
        {
            SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
            if (curUseSkillSlot == null)
            {
                return EBTStatus.BT_SUCCESS;
            }
            if (curUseSkillSlot.ImmediateAbort(abortType))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("高级模式下普攻搜索敌人", "高级模式下普攻搜索敌人")]
        public uint AdvanceCommonAttackSearchEnemy(int srchR)
        {
            uint num = 0;
            BaseAttackMode currentAttackMode = this.m_wrapper.GetCurrentAttackMode();
            if (currentAttackMode != null)
            {
                num = currentAttackMode.CommonAttackSearchEnemy(srchR);
            }
            return num;
        }

        public virtual void Born(ActorRoot actor)
        {
            this.m_wrapper = actor.ActorControl;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((((curLvelContext == null) || !curLvelContext.IsMobaMode()) || ((curLvelContext.m_warmHeroAiDiffInfo == null) || (ownerPlayer == null))) || !ownerPlayer.Computer)
            {
                if (((actor.CharInfo != null) && !string.IsNullOrEmpty(actor.CharInfo.BtResourcePath)) && (actor.ActorControl != null))
                {
                    base.m_AgentFileName = actor.CharInfo.BtResourcePath;
                    base.SetCurAgentActive();
                }
            }
            else
            {
                int iAILevel = curLvelContext.m_warmHeroAiDiffInfo.iAILevel;
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) actor.TheActorMeta.ConfigId);
                switch (iAILevel)
                {
                    case 1:
                        base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Entry);
                        break;

                    case 2:
                        base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Simple);
                        break;

                    case 3:
                        base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Normal);
                        break;

                    case 4:
                        base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Hard);
                        break;

                    case 5:
                        if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
                        }
                        else
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_WarmSimple);
                        }
                        if (curLvelContext.IsGameTypeLadder())
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
                        }
                        break;

                    case 6:
                        if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
                        }
                        else
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_WarmSimple);
                        }
                        if (curLvelContext.IsGameTypeLadder())
                        {
                            base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
                        }
                        break;

                    default:
                        base.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Normal);
                        break;
                }
                base.SetCurAgentActive();
            }
        }

        [MethodMetaInfo("高级模式下取消普攻模式", "高级模式下取消普攻模式")]
        public bool CancelCommonAttackMode()
        {
            return this.m_wrapper.CancelCommonAttackMode();
        }

        [MethodMetaInfo("是否可以移动", "是否可以移动")]
        public EBTStatus CanMove()
        {
            if (this.m_wrapper.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
            {
                return EBTStatus.BT_FAILURE;
            }
            return EBTStatus.BT_SUCCESS;
        }

        [MethodMetaInfo("是否能使用技能", "是否能使用指定的技能")]
        public EBTStatus CanUseSkill(SkillSlotType InSlot)
        {
            if (this.m_wrapper.CanUseSkill(InSlot))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("切换目标为攻击自己的敌人", "切换目标为攻击自己的敌人")]
        public void ChangeTargetToAtker()
        {
            this.m_wrapper.myTarget = this.m_wrapper.myLastAtker;
        }

        [MethodMetaInfo("检查技能是否能对该目标类型释放", "检查技能是否能对该目标类型释放")]
        public EBTStatus CheckSkillFilter(SkillSlotType InSlot, uint objID)
        {
            uint dwSkillTargetFilter = this.m_wrapper.GetSkill(InSlot).cfgData.dwSkillTargetFilter;
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if ((dwSkillTargetFilter & (((int) 1) << actor.handle.TheActorMeta.ActorType)) > 0L)
            {
                return EBTStatus.BT_FAILURE;
            }
            return EBTStatus.BT_SUCCESS;
        }

        [MethodMetaInfo("检测技能释放位置", "检测技能释放位置")]
        public bool CheckUseSkillPosition()
        {
            if (this.m_wrapper != null)
            {
                SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
                Skill skill = this.m_wrapper.GetSkill(curSkillUseInfo.SlotType);
                if (skill != null)
                {
                    long num = 0L;
                    long num2 = 0L;
                    switch (curSkillUseInfo.AppointType)
                    {
                        case SkillRangeAppointType.Target:
                        {
                            if ((curSkillUseInfo.TargetActor == 0) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
                            {
                                return false;
                            }
                            num = (long) (skill.cfgData.iMaxAttackDistance * skill.cfgData.iMaxAttackDistance);
                            VInt3 num4 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                            num2 = num4.sqrMagnitudeLong2D - num;
                            if (num2 > (num * 0.09))
                            {
                                return false;
                            }
                            return true;
                        }
                        case SkillRangeAppointType.Pos:
                        {
                            num = (long) (skill.cfgData.iMaxAttackDistance * skill.cfgData.iMaxAttackDistance);
                            VInt3 num3 = this.m_wrapper.actorLocation - curSkillUseInfo.UseVector;
                            num2 = num3.sqrMagnitudeLong2D - num;
                            return (num2 <= (num * 0.09));
                        }
                        case SkillRangeAppointType.Directional:
                            return true;
                    }
                }
            }
            return false;
        }

        [MethodMetaInfo("清除需要帮助他人的标记", "清除需要帮助他人的标记")]
        public void ClearHelpOther()
        {
            this.m_wrapper.m_isNeedToHelpOther = false;
        }

        [MethodMetaInfo("清除移动指令", "")]
        public void ClearMoveCMD()
        {
            this.m_wrapper.ClearMoveCommandWithOutNotice();
        }

        [MethodMetaInfo("清空选择的目标", "即没有目标")]
        public void ClearTarget()
        {
            this.m_wrapper.ClearTarget();
        }

        [MethodMetaInfo("高级模式下关闭普攻空放", "高级模式下关闭普攻空放")]
        public bool DisableSpecialCommonAttack()
        {
            if (this.m_wrapper.actor.SkillControl.SkillUseCache != null)
            {
                this.m_wrapper.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(false);
            }
            return true;
        }

        [MethodMetaInfo("执行当前命令移动", "执行命令的移动")]
        public void ExMoveCmd()
        {
            this.m_wrapper.BTExMoveCmd();
        }

        [MethodMetaInfo("获取角色的类型", "英雄，怪物，还是建筑")]
        public ActorTypeDef GetActorType(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return ActorTypeDef.Invalid;
            }
            return actor.handle.TheActorMeta.ActorType;
        }

        [MethodMetaInfo("获取攻击的范围", "")]
        public int GetAttackRange()
        {
            return this.m_wrapper.AttackRange;
        }

        [MethodMetaInfo("获取男爵死亡次数", "获取男爵死亡次数")]
        public int GetBaronDeadTimes()
        {
            if (Singleton<BattleStatistic>.instance.m_battleDeadStat != null)
            {
                return Singleton<BattleStatistic>.instance.m_battleDeadStat.GetBaronDeadCount();
            }
            return 0;
        }

        [MethodMetaInfo("获取出生点Index", "")]
        public int GetCampIndex()
        {
            int bornPointIndex = -1;
            if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                ActorServerData actorData = new ActorServerData();
                actorDataProvider.GetActorServerData(ref this.m_wrapper.actor.TheActorMeta, ref actorData);
                bornPointIndex = actorData.TheExtraInfo.BornPointIndex;
            }
            return bornPointIndex;
        }

        [MethodMetaInfo("获取队长", "获取队长")]
        public uint GetCaptain()
        {
            return ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr).Captain.handle.ObjID;
        }

        [MethodMetaInfo("获取当前命令的目的地", "获取当前命令的目的地")]
        public Vector3 GetCmdDest()
        {
            if (this.m_wrapper.curMoveCommand.cmdType == 1)
            {
                FrameCommand<MoveToPosCommand> curMoveCommand = (FrameCommand<MoveToPosCommand>) this.m_wrapper.curMoveCommand;
                return curMoveCommand.cmdData.destPosition.vec3;
            }
            if (this.m_wrapper.curMoveCommand.cmdType == 4)
            {
                FrameCommand<AttackPositionCommand> command2 = (FrameCommand<AttackPositionCommand>) this.m_wrapper.curMoveCommand;
                return command2.cmdData.WorldPos.vec3;
            }
            return Vector3.zero;
        }

        [MethodMetaInfo("当前的行为", "用于决定AI走哪个分支")]
        public ObjBehaviMode GetCurBehavior()
        {
            return this.m_wrapper.myBehavior;
        }

        [MethodMetaInfo("获取当前魂值等级", "获取当前魂值等级")]
        public int GetCurLevel()
        {
            return this.m_wrapper.actor.ValueComponent.actorSoulLevel;
        }

        [MethodMetaInfo("获取当前路径最后的方向", "获取当前路径最后的方向")]
        public Vector3 GetCurRouteLastForward()
        {
            if (((this.m_wrapper.m_curWaypointsHolder == null) || (this.m_wrapper.m_curWaypointsHolder.wayPoints == null)) || (this.m_wrapper.m_curWaypointsHolder.wayPoints.Length <= 1))
            {
                return Vector3.zero;
            }
            Waypoint endPoint = this.m_wrapper.m_curWaypointsHolder.endPoint;
            Waypoint waypoint2 = this.m_wrapper.m_curWaypointsHolder.wayPoints[this.m_wrapper.m_curWaypointsHolder.wayPoints.Length - 2];
            return (endPoint.transform.position - waypoint2.transform.position);
        }

        [MethodMetaInfo("获取当前技能Type", "获取当前分支对应的技能Type")]
        public SkillSlotType GetCurSkillSlotType()
        {
            return this.m_wrapper.curSkillUseInfo.SlotType;
        }

        [MethodMetaInfo("获取小龙的ID", "获取小龙的ID")]
        public uint GetDragonId()
        {
            if ((Singleton<BattleLogic>.GetInstance().m_dragonSpawn != null) && (Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList().Count > 0))
            {
                PoolObjHandle<ActorRoot> handle = Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList()[0];
                if (handle != 0)
                {
                    return handle.handle.ObjID;
                }
            }
            return 0;
        }

        [MethodMetaInfo("获取范围内的敌人数量", "获取范围内的敌人数量")]
        public int GetEnemyCountInRange(int srchR)
        {
            return Singleton<TargetSearcher>.GetInstance().GetEnemyCountInRange(this.m_wrapper.actor, srchR);
        }

        [MethodMetaInfo("获取范围内的敌人英雄数量", "获取范围内的敌人英雄数量")]
        public int GetEnemyHeroCountInRange(int srchR)
        {
            return Singleton<TargetSearcher>.GetInstance().GetEnemyHeroCountInRange(this.m_wrapper.actor, srchR);
        }

        [MethodMetaInfo("获取指定角色的攻击目标", "获取指定角色的攻击目标")]
        public uint GetGivenActorTarget(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if ((((actor != 0) && (actor.handle.ActorControl != null)) && ((actor.handle.ActorControl.myTarget != 0) && (actor.handle.ActorControl.myTarget.handle.ActorControl != null))) && !actor.handle.ActorControl.myTarget.handle.ActorControl.IsDeadState)
            {
                return actor.handle.ActorControl.myTarget.handle.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("获取攻击自己的敌方英雄", "")]
        public uint GetHeroWhoAttackSelf()
        {
            return this.m_wrapper.LastHeroAtker.handle.ObjID;
        }

        [MethodMetaInfo("获取当前持有者玩的队长", "注意,这只能用在只有一个玩家,其他都是电脑的情况下")]
        public uint GetHostPlayrCaptain()
        {
            return Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID;
        }

        [MethodMetaInfo("获取当前生命值比率", "10000表示满血")]
        public int GetHPPercent()
        {
            return ((this.m_wrapper.actor.ValueComponent.actorHp * 0x2710) / this.m_wrapper.actor.ValueComponent.actorHpTotal);
        }

        [MethodMetaInfo("获取领导者", "由后台下发")]
        public uint GetLeader()
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.m_wrapper.m_leaderID);
            if (actor == 0)
            {
                return 0;
            }
            if (actor.handle.ActorControl.IsDeadState)
            {
                ActorRoot nearestSelfCampHero = Singleton<TargetSearcher>.instance.GetNearestSelfCampHero(this.m_wrapper.actor, 0x3e80);
                if (nearestSelfCampHero != null)
                {
                    return nearestSelfCampHero.ObjID;
                }
            }
            return this.m_wrapper.m_leaderID;
        }

        [MethodMetaInfo("获取低血量的队友", "HPRate是比例,10000表示1; InSlot技能槽位,用于过滤")]
        public uint GetLowHpTeamMember(int srchR, int HPRate, SkillSlotType InSlot)
        {
            uint filter = 0;
            Skill skill = this.m_wrapper.GetSkill(InSlot);
            if ((skill != null) && (skill.cfgData != null))
            {
                filter = skill.cfgData.dwSkillTargetFilter;
            }
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetLowHpTeamMember((ActorRoot) this.m_wrapper.actorPtr, srchR, HPRate, filter);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("地图AI模式", "地图AI模式")]
        public RES_LEVEL_HEROAITYPE GetMapAIMode()
        {
            return Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_heroAiType;
        }

        [MethodMetaInfo("获取主人", "召唤的怪物才有这个功能")]
        public uint GetMaster()
        {
            MonsterWrapper wrapper = this.m_wrapper as MonsterWrapper;
            if ((wrapper != null) && (wrapper.hostActor != 0))
            {
                return wrapper.hostActor.handle.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("获取移动命令的ID", "获取移动命令的ID")]
        public uint GetMoveCmdId()
        {
            if (this.m_wrapper.curMoveCommand == null)
            {
                return 0;
            }
            return this.m_wrapper.curMoveCommand.cmdId;
        }

        [MethodMetaInfo("获取自己当前的位置", "")]
        public Vector3 GetMyCurPos()
        {
            return this.m_wrapper.actor.location.vec3;
        }

        [MethodMetaInfo("获取自己当前的朝向", "")]
        public Vector3 GetMyForward()
        {
            return this.m_wrapper.actor.forward.vec3;
        }

        [MethodMetaInfo("获取自己的ID", "")]
        public uint GetMyObjID()
        {
            return this.m_wrapper.actor.ObjID;
        }

        [MethodMetaInfo("自己攻击目标的ID", "")]
        public uint GetMyTargetID()
        {
            uint objID = 0;
            if (this.m_wrapper.myTarget != 0)
            {
                objID = this.m_wrapper.myTarget.handle.ObjID;
            }
            return objID;
        }

        [MethodMetaInfo("选择范围内的敌人", "选择范围内的敌人")]
        public uint GetNearestEnemy(int srchR)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(this.m_wrapper.actor, srchR, 0, true);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,优先小兵,然后英雄", "选择范围内的敌人,优先小兵,然后英雄")]
        public uint GetNearestEnemyDogfaceFirst(int srchR)
        {
            ActorRoot nearestEnemyDogfaceFirst = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyDogfaceFirst(this.m_wrapper.actor, srchR);
            if (nearestEnemyDogfaceFirst != null)
            {
                return nearestEnemyDogfaceFirst.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,优先小兵,超级兵》近战兵》远程兵,然后英雄", "选择范围内的敌人,优先小兵,超级兵》近战兵》远程兵,然后英雄")]
        public uint GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(int srchR)
        {
            ActorRoot nearestEnemyDogfaceFirstAndDogfaceHasPriority = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(this.m_wrapper.actor, srchR);
            if (nearestEnemyDogfaceFirstAndDogfaceHasPriority != null)
            {
                return nearestEnemyDogfaceFirstAndDogfaceHasPriority.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,忽略视野", "选择范围内的敌人,忽略视野，包括野怪")]
        public uint GetNearestEnemyIgnoreVisible(int srchR)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyIgnoreVisible((ActorRoot) this.m_wrapper.actorPtr, srchR, 0);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,不包括没有处于战斗状态的野怪", "选择范围内的敌人,不包括没有处于战斗状态的野怪")]
        public uint GetNearestEnemyWithoutNotInBattleJungleMonster(int srchR)
        {
            ActorRoot nearestEnemyWithoutNotInBattleJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutNotInBattleJungleMonster((ActorRoot) this.m_wrapper.actorPtr, srchR);
            if (nearestEnemyWithoutNotInBattleJungleMonster != null)
            {
                return nearestEnemyWithoutNotInBattleJungleMonster.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,不包含指定Actor且不包括没有处于战斗状态的野怪", "选择范围内的敌人,不包括没有处于战斗状态的野怪且不包含指定Actor")]
        public uint GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(int srchR, uint withOutActor)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor((ActorRoot) this.m_wrapper.actorPtr, srchR, withOutActor);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,带优先级，不包括没有处于战斗状态的野怪", "选择范围内的敌人,带优先级,不包括没有处于战斗状态的野怪")]
        public uint GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(int srchR, TargetPriority priotity)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster((ActorRoot) this.m_wrapper.actorPtr, srchR, priotity);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内的敌人,带优先级，不包含指定Actor且不包括没有处于战斗状态的野怪,,", "选择范围内的敌人,带优先级,不包含指定Actor且不包括没有处于战斗状态的野怪")]
        public uint GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(int srchR, TargetPriority priotity, uint withOutActor)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor((ActorRoot) this.m_wrapper.actorPtr, srchR, priotity, withOutActor);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("选择范围内最近的敌人", "TargetPriority表示敌人的类型")]
        public uint GetNearestEnemyWithTargetPriority(int srchR, TargetPriority priotity)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(this.m_wrapper.actor, srchR, priotity, 0, true);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("获取最近的队友", "获取最近的队友")]
        public uint GetNearestMember()
        {
            ActorRoot root = null;
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr).GetAllHeroes().GetEnumerator();
            ulong maxValue = ulong.MaxValue;
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                ActorRoot handle = current.handle;
                if (this.m_wrapper.actor.ObjID != handle.ObjID)
                {
                    VInt3 num3 = handle.location - this.m_wrapper.actor.location;
                    ulong num2 = (ulong) num3.sqrMagnitudeLong2D;
                    if (num2 < maxValue)
                    {
                        root = handle;
                        maxValue = num2;
                    }
                }
            }
            if (root == null)
            {
                return 0;
            }
            return root.ObjID;
        }

        [MethodMetaInfo("获取最近的非队长队友", "获取最近的非队长队友")]
        public uint GetNearestMemberNotCaptain()
        {
            ActorRoot root = null;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
            ulong maxValue = ulong.MaxValue;
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                ActorRoot handle = current.handle;
                if ((this.m_wrapper.actor.ObjID != handle.ObjID) && (ownerPlayer.Captain.handle.ObjID != handle.ObjID))
                {
                    VInt3 num3 = handle.location - this.m_wrapper.actor.location;
                    ulong num2 = (ulong) num3.sqrMagnitudeLong2D;
                    if (num2 < maxValue)
                    {
                        root = handle;
                        maxValue = num2;
                    }
                }
            }
            if (root == null)
            {
                return 0;
            }
            return root.ObjID;
        }

        [MethodMetaInfo("获取最近神符的位置", "获取最近神符的位置,如果返回的位置的Y小于-1000则表示符文无效")]
        public Vector3 GetNearestShenfuInRange(int range)
        {
            foreach (KeyValuePair<int, ShenFuObjects> pair in Singleton<ShenFuSystem>.GetInstance()._shenFuTriggerPool)
            {
                if (pair.Value.ShenFu.activeSelf)
                {
                    Vector3 position = pair.Value.ShenFu.transform.position;
                    VInt3 num = new VInt3(position);
                    long num2 = range;
                    VInt3 num3 = this.m_wrapper.actorLocation - num;
                    if (num3.sqrMagnitudeLong2D <= (num2 * num2))
                    {
                        return position;
                    }
                }
            }
            return new Vector3(0f, -10000f, 0f);
        }

        [MethodMetaInfo("获取需要帮助的角色ID", "获取需要帮助的角色ID")]
        public uint GetNeedHelpTarget()
        {
            if (this.m_wrapper.m_needToHelpTarget != 0)
            {
                return this.m_wrapper.m_needToHelpTarget.handle.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("获取范围内的友军数量,包含自己", "获取范围内的友军数量,包含自己")]
        public int GetOurCampActorsCount(int srchR)
        {
            int num = 1;
            List<ActorRoot> ourCampActors = Singleton<TargetSearcher>.GetInstance().GetOurCampActors(this.m_wrapper.actor, srchR);
            if (ourCampActors != null)
            {
                return (num + ourCampActors.Count);
            }
            return num;
        }

        [MethodMetaInfo("获取失控的类型", "获取失控的类型")]
        public OutOfControlType GetOutOfControlType()
        {
            return this.m_wrapper.m_outOfControl.m_outOfControlType;
        }

        [MethodMetaInfo("获取多边形边上的一点", "index表示第几条边")]
        public Vector3 GetPolygonEdgePoint(int index)
        {
            VInt2 randomPoint = this.m_wrapper.m_rangePolygon.GetRandomPoint(index);
            VInt3 num2 = new VInt3(randomPoint.x, this.m_wrapper.actor.location.y, randomPoint.y);
            return num2.vec3;
        }

        [MethodMetaInfo("获取追击范围", "")]
        public int GetPursuitRange()
        {
            MonsterWrapper wrapper = this.m_wrapper.actor.AsMonster();
            if ((wrapper != null) && (wrapper.cfgInfo != null))
            {
                return wrapper.cfgInfo.iPursuitR;
            }
            return 0;
        }

        [MethodMetaInfo("获取该关卡最大英雄个数", "是不是网络对战")]
        public int GetPvPLevelMaxHeroNum()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide())
            {
                return curLvelContext.m_pvpPlayerNum;
            }
            return 0;
        }

        [MethodMetaInfo("获取远离指定的Actor的随机点", "获取远离指定的Actor的随机点")]
        public virtual Vector3 GetRandomFarPoint(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return Vector3.zero;
            }
            ActorRoot handle = actor.handle;
            VInt3 location = handle.location;
            VInt3 num2 = this.m_wrapper.actor.location - location;
            num2 = num2.NormalizeTo(0x3e8);
            for (int i = 0; i < 20; i++)
            {
                VInt3 target = this.m_wrapper.actor.location + (num2 * (FrameRandom.Random(5) + 6));
                target.x += FrameRandom.Random(0x3a98) * (((FrameRandom.Random(0xbb8) % 2) != 0) ? -1 : 1);
                target.z += FrameRandom.Random(0x3a98) * (((FrameRandom.Random(0xbb8) % 2) != 0) ? -1 : 1);
                if (PathfindingUtility.IsValidTarget(handle, target))
                {
                    return target.vec3;
                }
            }
            return this.m_wrapper.actor.location.vec3;
        }

        [MethodMetaInfo("获取指定点周围的随机点", "获取指定点周围的随机点")]
        public Vector3 GetRandomPointAroundGivenPoint(Vector3 aimPos, int range)
        {
            int num = FrameRandom.Random((uint) (range * 2)) - range;
            int num2 = FrameRandom.Random((uint) (range * 2)) - range;
            Vector3 position = new Vector3(aimPos.x + (num * 0.001f), aimPos.y, aimPos.z + (num2 * 0.001f));
            if (PathfindingUtility.IsValidTarget(this.m_wrapper.actor, new VInt3(position)))
            {
                return position;
            }
            return aimPos;
        }

        [MethodMetaInfo("获取指定点朝向随机点", "获取指定点朝向随机点")]
        public Vector3 GetRandomPointByGivenPoint(Vector3 aimPos, int range, int distance)
        {
            Vector3 vector = aimPos - this.m_wrapper.actor.location.vec3;
            Vector3 vector2 = this.m_wrapper.actor.location.vec3 + ((Vector3) (vector.normalized * (distance * 0.001f)));
            return this.GetRandomPointAroundGivenPoint(vector2, range);
        }

        [MethodMetaInfo("获取指定点朝向随机点,指定最小随机", "获取指定点朝向随机点,指定最小随机")]
        public Vector3 GetRandomPointByGivenPointAndMinRange(Vector3 aimPos, int maxRange, int minRange, int distance)
        {
            Vector3 vector = aimPos - this.m_wrapper.actor.location.vec3;
            Vector3 vector2 = this.m_wrapper.actor.location.vec3 + ((Vector3) (vector.normalized * (distance * 0.001f)));
            int num = maxRange - minRange;
            int num2 = FrameRandom.Random((uint) (num * 2)) - num;
            int num3 = FrameRandom.Random((uint) (num * 2)) - num;
            if (num2 > 0)
            {
                num2 += minRange;
            }
            else
            {
                num2 -= minRange;
            }
            if (num3 > 0)
            {
                num3 += minRange;
            }
            else
            {
                num3 -= minRange;
            }
            Vector3 position = new Vector3(vector2.x + (num2 * 0.001f), vector2.y, vector2.z + (num3 * 0.001f));
            if (PathfindingUtility.IsValidTarget(this.m_wrapper.actor, new VInt3(position)))
            {
                return position;
            }
            return vector2;
        }

        [MethodMetaInfo("获取最近的回血点位置", "")]
        public Vector3 GetRestoredHpPos()
        {
            Vector3 zero = Vector3.zero;
            VInt3 outPosWorld = VInt3.zero;
            VInt3 forward = VInt3.forward;
            if (Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.m_wrapper.actor.TheActorMeta, true, out outPosWorld, out forward))
            {
                zero = (Vector3) outPosWorld;
            }
            return zero;
        }

        [MethodMetaInfo("获取路径点中的当前点的位置", "前提是已设定好路径")]
        public Vector3 GetRouteCurWaypointPos()
        {
            return this.m_wrapper.GetRouteCurWaypointPos().vec3;
        }

        [MethodMetaInfo("获取路径点中的当前点的位置,用于沿路径点返回", "前提是已设定好路径")]
        public Vector3 GetRouteCurWaypointPosPre()
        {
            return this.m_wrapper.GetRouteCurWaypointPosPre().vec3;
        }

        [MethodMetaInfo("获取搜索的范围", "")]
        public int GetSearchRange()
        {
            return this.m_wrapper.SearchRange;
        }

        [MethodMetaInfo("获取视野范围", "")]
        public int GetSightArea()
        {
            return this.m_wrapper.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_SightArea].totalValue;
        }

        [MethodMetaInfo("获取指定技能的攻击范围", "获取指定技能的攻击范围")]
        public int GetSkillAttackRange(SkillSlotType InSlot)
        {
            Skill skill = this.m_wrapper.GetSkill(InSlot);
            if ((skill != null) && (skill.cfgData != null))
            {
                return (int) skill.cfgData.iMaxAttackDistance;
            }
            return 0;
        }

        [MethodMetaInfo("获取指定技能的搜索范围", "获取指定技能的搜索范围")]
        public int GetSkillSearchRange(SkillSlotType InSlot)
        {
            Skill skill = this.m_wrapper.GetSkill(InSlot);
            if ((skill != null) && (skill.cfgData != null))
            {
                return skill.cfgData.iMaxSearchDistance;
            }
            return 0;
        }

        [MethodMetaInfo("获取技能目标规则", "获取技能目标规则")]
        public SkillTargetRule GetSkillTargetRule(SkillSlotType InSlot)
        {
            Skill skill = this.m_wrapper.GetSkill(InSlot);
            if ((skill != null) && (skill.cfgData != null))
            {
                return (SkillTargetRule) skill.cfgData.dwSkillTargetRule;
            }
            return SkillTargetRule.LowerHpEnermy;
        }

        [MethodMetaInfo("获取召唤师技能类型", "获取召唤师技能类型")]
        public RES_SUMMONERSKILL_TYPE GetSummonerSkillType(SkillSlotType InSlot)
        {
            Skill skill = this.m_wrapper.GetSkill(InSlot);
            if ((skill != null) && (skill.cfgData != null))
            {
                return (RES_SUMMONERSKILL_TYPE) skill.cfgData.bSkillType;
            }
            return RES_SUMMONERSKILL_TYPE.RES_SUMMONERSKILL_HITMONSTER;
        }

        [MethodMetaInfo("获取队友攻击目标", "获取队友攻击目标")]
        public uint GetTeamMemberTarget()
        {
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr).GetAllHeroes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                ActorRoot handle = current.handle;
                if ((((handle != null) && (handle.ActorControl != null)) && ((handle.ActorControl.myTarget != 0) && (handle.ActorControl.myTarget.handle.ActorControl != null))) && (!handle.ActorControl.myTarget.handle.ActorControl.IsDeadState && !this.m_wrapper.actor.IsSelfCamp(handle.ActorControl.myTarget.handle)))
                {
                    return handle.ActorControl.myTarget.handle.ObjID;
                }
            }
            return 0;
        }

        [MethodMetaInfo("获取恐惧自己的敌人", "获取恐惧自己的敌人")]
        public uint GetTerrorMeActor()
        {
            if (this.m_wrapper.m_terrorMeActor != 0)
            {
                return this.m_wrapper.m_terrorMeActor.handle.ObjID;
            }
            return 0;
        }

        [MethodMetaInfo("是否有塔在范围内，且塔下没有自己的小兵", "是否有塔在范围内，且塔下没有自己的小兵")]
        public EBTStatus HasEnemyBuildingAndEnemyBuildingWillAttackSelf(int srchR)
        {
            if (Singleton<TargetSearcher>.GetInstance().HasEnemyBuildingAndEnemyBuildingWillAttackSelf(this.m_wrapper.actor, srchR))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否有敌人在范围内", "是否有敌人在范围内")]
        public EBTStatus HasEnemyInRange(int range)
        {
            ulong num = (ulong) (range * range);
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = gameActors[i];
                ActorRoot actor = handle.handle;
                if (!this.m_wrapper.actor.IsSelfCamp(actor))
                {
                    MonsterWrapper wrapper = actor.AsMonster();
                    if (wrapper != null)
                    {
                        ResMonsterCfgInfo cfgInfo = wrapper.cfgInfo;
                        if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                        {
                            ObjAgent actorAgent = actor.ActorAgent;
                            if (((actorAgent.GetCurBehavior() == ObjBehaviMode.State_Idle) || (actorAgent.GetCurBehavior() == ObjBehaviMode.State_Dead)) || (actorAgent.GetCurBehavior() == ObjBehaviMode.State_Null))
                            {
                                continue;
                            }
                        }
                    }
                    VInt3 num5 = actor.location - this.m_wrapper.actor.location;
                    if (num5.sqrMagnitudeLong2D < num)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否有队友的血量小于指定的比率值", "是否有队友的血量小于指定的比率值")]
        public bool HasMemberHpLessThan(int hpRate)
        {
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr).GetAllHeroes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                ActorRoot handle = current.handle;
                if (hpRate > ((handle.ValueComponent.actorHp * 0x2710) / handle.ValueComponent.actorHpTotal))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodMetaInfo("移动命令未完成", "")]
        public bool HasMoveCMD()
        {
            return (this.m_wrapper.curMoveCommand != null);
        }

        [MethodMetaInfo("普攻命令未完成", "")]
        public bool HasNormalAttackCMD()
        {
            SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
            return ((skillUseCache != null) && skillUseCache.GetCommonAttackMode());
        }

        [MethodMetaInfo("是否已有路径", "是否有路径")]
        public bool HasRoute()
        {
            if (this.m_wrapper.m_curWaypointsHolder == null)
            {
                return false;
            }
            return true;
        }

        [MethodMetaInfo("帮助他人攻击", "切换由他人传过的攻击目标")]
        public void HelpToAttack()
        {
            this.m_wrapper.HelpToAttack();
        }

        [MethodMetaInfo("是否打断普攻", "是否打断普攻")]
        public bool IsAbortNormalAttack()
        {
            SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
            if (curUseSkillSlot == null)
            {
                return true;
            }
            SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
            if (skillUseCache != null)
            {
                if (skillUseCache.GetCommonAttackMode())
                {
                    bool flag = curUseSkillSlot.IsAbort(SkillAbortType.TYPE_SKILL_0) && curUseSkillSlot.IsCDReady;
                    if (flag)
                    {
                        curUseSkillSlot.Abort(SkillAbortType.TYPE_SKILL_0);
                    }
                    return flag;
                }
                if (this.m_wrapper.curMoveCommand != null)
                {
                    return curUseSkillSlot.Abort(SkillAbortType.TYPE_MOVE);
                }
            }
            return false;
        }

        [MethodMetaInfo("是否打断当前技能", "是否打断当前技能")]
        public bool IsAbortUseSkill()
        {
            SkillSlotType type;
            SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
            if (curUseSkillSlot == null)
            {
                return true;
            }
            SkillCache skillUseCache = null;
            skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
            return ((((skillUseCache != null) && skillUseCache.GetCacheSkillSlotType(out type)) && curUseSkillSlot.IsAbort((SkillAbortType) type)) || ((this.m_wrapper.curMoveCommand != null) && curUseSkillSlot.IsAbort(SkillAbortType.TYPE_MOVE)));
        }

        [MethodMetaInfo("对象是否处于攻击模式", "处在被人打和打人的模式")]
        public bool IsActorInBattle(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            ObjAgent actorAgent = actor.handle.ActorAgent;
            return (((actorAgent.GetCurBehavior() != ObjBehaviMode.State_Idle) && (actorAgent.GetCurBehavior() != ObjBehaviMode.State_Dead)) && (actorAgent.GetCurBehavior() != ObjBehaviMode.State_Null));
        }

        [MethodMetaInfo("判断角色是否活的", "是否活的")]
        public EBTStatus IsAlive(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor != 0)
            {
                return (!actor.handle.ActorControl.IsDeadState ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE);
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否在一定范围内己方强于敌人", "是否在一定范围内己方强于敌人 strengthRate代表了对方强弱的比率,如0.8,表示是否强于对方血量的0.8")]
        public EBTStatus IsAroundTeamThanStrongThanEnemise(int srchR, int strengthRate)
        {
            ulong num = (ulong) (srchR * srchR);
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
            int count = heroActors.Count;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                ActorRoot actor = handle.handle;
                VInt3 num9 = actor.location - this.m_wrapper.actor.location;
                if (num9.sqrMagnitudeLong2D < num)
                {
                    if (this.m_wrapper.actor.IsSelfCamp(actor))
                    {
                        num3 += actor.ValueComponent.actorHp;
                        num4 += actor.ValueComponent.actorHpTotal;
                    }
                    else
                    {
                        num5 += actor.ValueComponent.actorHp;
                        num6 += actor.ValueComponent.actorHpTotal;
                    }
                }
            }
            if (num5 == 0)
            {
                return EBTStatus.BT_SUCCESS;
            }
            if (((num3 * num6) * 0x2710) > ((num5 * num4) * strengthRate))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否被敌方攻击", "")]
        public bool IsAttackByEnemy()
        {
            return this.m_wrapper.m_isAttacked;
        }

        [MethodMetaInfo("是否被敌方英雄攻击", "")]
        public bool IsAttackedByEnemyHero()
        {
            return this.m_wrapper.m_isAttackedByEnemyHero;
        }

        [MethodMetaInfo("是否在攻击建筑时被其他英雄和小兵攻击", "是否在攻击建筑时被其他英雄和小兵攻击")]
        public EBTStatus IsAttackingBuildingAndHeroOrSoldierAttackMe()
        {
            if (((this.m_wrapper.myLastAtker != 0) && (this.m_wrapper.myTarget != 0)) && ((this.m_wrapper.myTarget.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) && (this.m_wrapper.myLastAtker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否需要朝普攻目标移动", "攻击距离不够的情况下需要移动")]
        public virtual bool IsAttackMoveToTarget()
        {
            if (this.m_wrapper == null)
            {
                return false;
            }
            SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
            Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
            if ((nextSkill == null) || (curSkillUseInfo.AppointType != SkillRangeAppointType.Target))
            {
                return false;
            }
            if (((curSkillUseInfo.TargetActor == 0) || (curSkillUseInfo.TargetActor.handle.ActorAgent == null)) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
            {
                return false;
            }
            DebugHelper.Assert(nextSkill.cfgData != null, "skillObj.cfgData != null");
            DebugHelper.Assert(curSkillUseInfo.TargetActor.handle.shape != null, "skillContext.TargetActor.handle.shape!=null");
            if ((nextSkill.cfgData == null) || (curSkillUseInfo.TargetActor.handle.shape == null))
            {
                return false;
            }
            long num = (long) (nextSkill.cfgData.iMaxAttackDistance + curSkillUseInfo.TargetActor.handle.shape.AvgCollisionRadius);
            num *= num;
            VInt3 num2 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
            return (num2.sqrMagnitudeLong2D > num);
        }

        [MethodMetaInfo("是否是自动AI", "是否是自动AI")]
        public bool IsAutoAI()
        {
            bool computer = false;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
            if (ownerPlayer != null)
            {
                computer = ownerPlayer.Computer;
            }
            return (this.m_wrapper.m_isAutoAI || computer);
        }

        [MethodMetaInfo("是否是基地", "是否是基地")]
        public EBTStatus IsBase(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if ((actor != 0) && ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) && (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否能移动到Actor目标点左边", "是否能移动到Actor目标点左边")]
        public bool IsCanMoveToActorLeft(uint objID, int unit)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            ActorRoot handle = actor.handle;
            VInt3 target = handle.location + new VInt3((handle.forward.z * -1) * unit, handle.forward.y * unit, handle.forward.x * unit);
            return PathfindingUtility.IsValidTarget(handle, target);
        }

        [MethodMetaInfo("是否能移动到Actor目标点右边", "是否能移动到Actor目标点右边")]
        public bool IsCanMoveToActorRight(uint objID, int unit)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            ActorRoot handle = actor.handle;
            VInt3 target = handle.location + new VInt3(handle.forward.z * unit, handle.forward.y * unit, (handle.forward.x * unit) * -1);
            return PathfindingUtility.IsValidTarget(handle, target);
        }

        [MethodMetaInfo("后续普攻是否打断当前技能", "")]
        public bool IsContinueAbortUseSkill()
        {
            return false;
        }

        [MethodMetaInfo("技能使用完成是否继续普攻", "")]
        public bool IsContinueCommonAttack()
        {
            bool commonAttackMode = false;
            SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
            OperateMode playerOperateMode = ActorHelper.GetPlayerOperateMode(ref this.m_wrapper.actorPtr);
            if (skillUseCache != null)
            {
                commonAttackMode = skillUseCache.GetCommonAttackMode();
            }
            if (playerOperateMode == OperateMode.DefaultMode)
            {
                return commonAttackMode;
            }
            if (commonAttackMode)
            {
                return true;
            }
            Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
            if (((curUseSkill != null) && (curUseSkill.cfgData != null)) && (((curUseSkill.AppointType == SkillRangeAppointType.Target) && (curUseSkill.cfgData.dwSkillTargetRule != 2)) || (curUseSkill.AppointType == SkillRangeAppointType.Pos)))
            {
                LockTargetAttackMode lockTargetAttackModeControl = this.m_wrapper.actor.LockTargetAttackModeControl;
                if (lockTargetAttackModeControl != null)
                {
                    uint lockTargetID = lockTargetAttackModeControl.GetLockTargetID();
                    if (lockTargetAttackModeControl.IsValidLockTargetID(lockTargetID) && (skillUseCache != null))
                    {
                        skillUseCache.SetCommonAttackMode(true);
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodMetaInfo("是否被当前持有者操控", "是否被当前持有者操控")]
        public EBTStatus IsControlByHostPlayer()
        {
            if (ActorHelper.IsHostCtrlActor(ref this.m_wrapper.actorPtr))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否被人控制(是否是队长)", "是否被人控制,被人控制的就是队长")]
        public bool IsControlByMan()
        {
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
            if ((ownerPlayer != null) && (ownerPlayer.Captain != 0))
            {
                return this.m_wrapper.m_isControledByMan;
            }
            return true;
        }

        [MethodMetaInfo("当前移动是否完成", "")]
        public virtual bool IsCurMoveCompleted()
        {
            return (this.m_wrapper.actor.MovementComponent.isFinished || !this.m_wrapper.actor.MovementComponent.isMoving);
        }

        [MethodMetaInfo("当前路径点是不是最后一个路径点", "前提是已设定好路径")]
        public bool IsCurWaypointEndPoint()
        {
            return this.m_wrapper.m_isCurWaypointEndPoint;
        }

        [MethodMetaInfo("当前路径点是不是起始点", "当前路径点是不是起始点")]
        public EBTStatus IsCurWayPointStartPoint()
        {
            if (this.m_wrapper.m_isStartPoint)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("路径点中的当前点是否有效", "前提是已设定好路径")]
        public bool IsCurWaypointValid()
        {
            return this.m_wrapper.IsCurWaypointValid();
        }

        [MethodMetaInfo("自己同指定Actor目标的距离是否小于指定值", "")]
        public bool IsDistanceToActorLessThanRange(uint objID, int range)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            ActorRoot handle = actor.handle;
            long num = range;
            if (handle.CharInfo != null)
            {
                num += handle.CharInfo.iCollisionSize.x;
            }
            VInt3 num2 = this.m_wrapper.actorLocation - handle.location;
            return (num2.sqrMagnitudeLong2D < (num * num));
        }

        [MethodMetaInfo("自己同指定Actor目标的距离是否大于指定值", "")]
        public bool IsDistanceToActorMoreThanRange(uint objID, int range)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            ActorRoot handle = actor.handle;
            long num = range;
            if (handle.CharInfo != null)
            {
                num += handle.CharInfo.iCollisionSize.x;
            }
            VInt3 num2 = this.m_wrapper.actorLocation - handle.location;
            return (num2.sqrMagnitudeLong2D > (num * num));
        }

        [MethodMetaInfo("自己同指定位置目标的距离是否小于指定值", "")]
        public bool IsDistanceToPosLessThanRange(Vector3 aimPos, int range)
        {
            VInt3 num = new VInt3(aimPos);
            long num2 = range;
            VInt3 num3 = this.m_wrapper.actorLocation - num;
            return (num3.sqrMagnitudeLong2D < (num2 * num2));
        }

        [MethodMetaInfo("自己同指定位置目标的距离是否大于指定值", "")]
        public bool IsDistanceToPosMoreThanRange(Vector3 aimPos, int range)
        {
            VInt3 num = new VInt3(aimPos);
            long num2 = range;
            VInt3 num3 = this.m_wrapper.actorLocation - num;
            return (num3.sqrMagnitudeLong2D > (num2 * num2));
        }

        [MethodMetaInfo("小龙是否活着", "就是小龙是否被刷怪点刷出来了")]
        public EBTStatus IsDragonAlive()
        {
            if ((Singleton<BattleLogic>.GetInstance().m_dragonSpawn != null) && (Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList().Count > 0))
            {
                PoolObjHandle<ActorRoot> handle = Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList()[0];
                if (handle != 0)
                {
                    ActorRoot root = handle.handle;
                    if ((root.ActorControl.myBehavior != ObjBehaviMode.State_Dead) && (root.ActorControl.myBehavior != ObjBehaviMode.State_GameOver))
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否跟随其他玩家", "掉线后，有玩家选择跟随玩家时，这里返回成功")]
        public EBTStatus IsFollowOtherPlayer()
        {
            if (this.m_wrapper.m_followOther)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否有指定的阵营的人在范围内", "是否有指定的阵营的人在范围内")]
        public EBTStatus IsGivenCampActorsInRange(COM_PLAYERCAMP camp, int range)
        {
            ulong num = (ulong) (range * range);
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = gameActors[i];
                ActorRoot root = handle.handle;
                if (root.TheActorMeta.ActorCamp == camp)
                {
                    VInt3 num5 = root.location - this.m_wrapper.actor.location;
                    if (num5.sqrMagnitudeLong2D < num)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("普攻立即释放", "")]
        public bool IsImmediateAttack()
        {
            return ((this.m_wrapper.actor.SkillControl != null) && this.m_wrapper.actor.SkillControl.bImmediateAttack);
        }

        [MethodMetaInfo("是否在战斗中", "是否在战斗中")]
        public EBTStatus IsInBattle()
        {
            if (this.m_wrapper.IsInBattle)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        public bool IsInDanger()
        {
            return (this.m_dengerCoolTick > 0);
        }

        [MethodMetaInfo("对象是否是野区怪物", "不是兵线上的就是野区的小怪物")]
        public bool IsJungleMonster(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                return false;
            }
            MonsterWrapper wrapper = actor.handle.AsMonster();
            if ((wrapper == null) || (wrapper.cfgInfo == null))
            {
                return false;
            }
            return (wrapper.cfgInfo.bMonsterType == 2);
        }

        [MethodMetaInfo("是不是排位赛", "是不是排位赛")]
        public EBTStatus IsLadder()
        {
            if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsGameTypeLadder())
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否低智商AI", "是否低智商AI,条件是个人人机对战，个人的等级在5级以下的敌方电脑AI")]
        public EBTStatus IsLowAI()
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel <= 5)
            {
                List<Player> allPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers();
                int num = 0;
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    if (!allPlayers[i].Computer)
                    {
                        num++;
                        if (num > 1)
                        {
                            return EBTStatus.BT_FAILURE;
                        }
                    }
                }
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (!Singleton<GamePlayerCenter>.GetInstance().IsAtSameCamp(hostPlayer.PlayerId, this.m_wrapper.actor.TheActorMeta.PlayerId))
                {
                    return EBTStatus.BT_SUCCESS;
                }
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("移动命令是否完成", "移动是否完成,这个是指大的移动命令是否完成")]
        public virtual bool IsMoveCMDCompleted()
        {
            if (this.m_wrapper.actor.MovementComponent.isFinished)
            {
                if (this.m_wrapper.curMoveCommand == null)
                {
                    return true;
                }
                if ((this.m_wrapper.curMoveCommand.cmdId > 0) && (this.m_wrapper.curMoveCommand.cmdId == this.m_wrapper.actor.MovementComponent.uCommandId))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodMetaInfo("是否需要帮助他人攻击", "是否需要帮助他人攻击")]
        public bool IsNeedToHelpOther()
        {
            if (!this.m_wrapper.m_isNeedToHelpOther)
            {
                return false;
            }
            if (this.m_wrapper.m_needToHelpTarget == 0)
            {
                return false;
            }
            if (this.m_wrapper.m_needToHelpTarget.handle.ObjID == 0)
            {
                return false;
            }
            return true;
        }

        [MethodMetaInfo("是否需要播出生动画", "是否需要播出生动画")]
        public EBTStatus IsNeedToPlayBornAge()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (((curLvelContext != null) && curLvelContext.IsMobaModeWithOutGuide()) && (curLvelContext.m_heroAiType == RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_FREEDOM))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否是掉线玩家", "是否是掉线玩家")]
        public EBTStatus IsOffline()
        {
            if (this.m_wrapper.m_offline)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否进入极危险区域", "是否越塔等")]
        public EBTStatus IsOverTower()
        {
            if (this.IsInDanger())
            {
                return EBTStatus.BT_SUCCESS;
            }
            if (Singleton<TargetSearcher>.GetInstance().HasCantAttackEnemyBuilding(this.m_wrapper.actor, 0x1f40))
            {
                this.SetInDanger();
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否在播放动画", "")]
        public EBTStatus IsPlayingAnimation()
        {
            if (this.m_wrapper.IsPlayingAnimation())
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是不是网络对战", "是不是网络对战")]
        public EBTStatus IsPlayOnNetwork()
        {
            if (Singleton<FrameSynchr>.GetInstance().bActive)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("是否找到普攻目标", "是否找到普攻目标")]
        public bool IsSearchCommonAttackTarget(uint objID)
        {
            if ((this.m_wrapper.actor.SkillControl != null) && this.m_wrapper.actor.SkillControl.bImmediateAttack)
            {
                return false;
            }
            if (objID <= 0)
            {
                this.m_wrapper.ClearTarget();
                return false;
            }
            if (Singleton<GameObjMgr>.GetInstance().GetActor(objID) == 0)
            {
                this.m_wrapper.ClearTarget();
                return false;
            }
            return true;
        }

        [MethodMetaInfo("指定技能是否准备好", "")]
        public bool IsSkillCDReady(SkillSlotType InSlot)
        {
            SkillSlot slot = null;
            if (!this.m_wrapper.actor.SkillControl.TryGetSkillSlot(InSlot, out slot))
            {
                return false;
            }
            return (((slot.SkillObj != null) && (slot.SkillObj.cfgData != null)) && slot.IsCDReady);
        }

        [MethodMetaInfo("是否需要朝技能目标点移动", "是否需要朝技能目标点移动")]
        public virtual bool IsSkillMoveToTarget()
        {
            if (this.m_wrapper != null)
            {
                SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
                Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
                if (nextSkill != null)
                {
                    long num;
                    switch (curSkillUseInfo.AppointType)
                    {
                        case SkillRangeAppointType.Target:
                        {
                            if ((curSkillUseInfo.TargetActor == 0) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
                            {
                                return false;
                            }
                            num = (long) (nextSkill.cfgData.iMaxAttackDistance + curSkillUseInfo.TargetActor.handle.shape.AvgCollisionRadius);
                            num *= num;
                            VInt3 num5 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                            return (num5.sqrMagnitudeLong2D > num);
                        }
                        case SkillRangeAppointType.Pos:
                        {
                            long num2 = (nextSkill.cfgData.iMaxAttackDistance * 0x2af8L) / 0x2710L;
                            num = num2 * num2;
                            VInt3 num4 = this.m_wrapper.actorLocation - curSkillUseInfo.UseVector;
                            long num3 = num4.sqrMagnitudeLong2D - num;
                            if (num3 <= 0L)
                            {
                                return false;
                            }
                            return true;
                        }
                        case SkillRangeAppointType.Directional:
                            return false;
                    }
                }
            }
            return false;
        }

        [MethodMetaInfo("判断目标是否可被攻击", "判断目标是否可被攻击,可被攻击的前提是活的,不是无敌的,不是一个阵营的")]
        public bool IsTargetCanBeAttacked(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                this.m_wrapper.ClearTarget();
                return false;
            }
            bool flag = false;
            flag = this.m_wrapper.CanAttack((ActorRoot) actor) && actor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp);
            if (!flag)
            {
                this.m_wrapper.ClearTarget();
            }
            return flag;
        }

        [MethodMetaInfo("判断目标是否可被攻击,忽略是否可见", "判断目标是否可被攻击,可被攻击的前提是活的,不是无敌的,不是一个阵营的")]
        public bool IsTargetCanBeAttackedIgnoreVisible(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor == 0)
            {
                this.m_wrapper.ClearTarget();
                this.m_wrapper.CancelCommonAttackMode();
                return false;
            }
            bool flag = false;
            flag = this.m_wrapper.CanAttack((ActorRoot) actor);
            if (!flag)
            {
                this.m_wrapper.ClearTarget();
                this.m_wrapper.CancelCommonAttackMode();
            }
            return flag;
        }

        [MethodMetaInfo("是否高级模式普攻", "是否高级模式普攻")]
        public bool IsUseAdvanceCommonAttack()
        {
            return this.m_wrapper.IsUseAdvanceCommonAttack();
        }

        [MethodMetaInfo("技能使用是否完成", "技能使用是否完成")]
        public bool IsUseSkillCompleted()
        {
            Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
            if ((curUseSkill != null) && !curUseSkill.isFinish)
            {
                return false;
            }
            return true;
        }

        [MethodMetaInfo("当前技能是否能被打断", "前技能是否能被打断")]
        public bool IsUseSkillCompletedOrCanAbort()
        {
            Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
            if (curUseSkill == null)
            {
                return true;
            }
            if (curUseSkill.skillAbort.AbortWithAI())
            {
                this.m_wrapper.actor.SkillControl.ForceAbortCurUseSkill();
                return true;
            }
            return false;
        }

        [MethodMetaInfo("高级模式下是否执行普攻空放", "高级模式下是否执行普攻空放")]
        public bool IsUseSpecialCommonAttack()
        {
            bool flag = this.m_wrapper.IsUseAdvanceCommonAttack();
            if ((this.m_wrapper.actor.SkillControl == null) || !this.m_wrapper.actor.SkillControl.bImmediateAttack)
            {
                if (!flag)
                {
                    return true;
                }
                if (this.m_wrapper.actor.SkillControl.SkillUseCache != null)
                {
                    return this.m_wrapper.actor.SkillControl.SkillUseCache.GetSpecialCommonAttack();
                }
            }
            return true;
        }

        [MethodMetaInfo("判断目标是否可见", "判断目标是否可见")]
        public EBTStatus IsVisible(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if ((actor != 0) && actor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("远离指定的Actor", "远离指定的Actor")]
        public virtual void LeaveActor(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor != 0)
            {
                VInt3 location = actor.handle.location;
                VInt3 direction = this.m_wrapper.actor.location - location;
                this.m_wrapper.RealMoveDirection(direction, 0);
            }
        }

        [MethodMetaInfo("远离指定的点", "远离指定的点")]
        public virtual void LeavePoint(Vector3 dest)
        {
            VInt3 num = new VInt3(dest);
            VInt3 direction = this.m_wrapper.actor.location - num;
            this.m_wrapper.RealMoveDirection(direction, 0);
        }

        [MethodMetaInfo("设定自己的朝向", "设定自己的朝向")]
        public virtual void LookAtDirection(Vector3 dest)
        {
            if (dest != Vector3.zero)
            {
                VInt3 inDirection = new VInt3(dest);
                this.m_wrapper.actor.MovementComponent.SetRotate(inDirection, true);
            }
        }

        [MethodMetaInfo("高级模式下普通攻击寻敌", "高级模式下普通攻击寻敌")]
        public virtual EBTStatus MoveToCommonAttackTargetWithRange(int range)
        {
            if (this.m_wrapper != null)
            {
                SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
                Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
                if ((nextSkill != null) && (curSkillUseInfo.AppointType == SkillRangeAppointType.Target))
                {
                    if (this.m_wrapper.actor.SkillControl.SkillUseCache.IsExistNewAttackCommand())
                    {
                        this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
                        return EBTStatus.BT_FAILURE;
                    }
                    if (((curSkillUseInfo.TargetActor == 0) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState) || !curSkillUseInfo.TargetActor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
                    {
                        this.m_wrapper.TerminateMove();
                        this.m_wrapper.ClearTarget();
                        this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
                        return EBTStatus.BT_FAILURE;
                    }
                    long num2 = range * range;
                    VInt3 num3 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                    if (num3.sqrMagnitudeLong2D > num2)
                    {
                        this.m_wrapper.TerminateMove();
                        this.m_wrapper.ClearTarget();
                        this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
                        return EBTStatus.BT_FAILURE;
                    }
                    long num = (long) (nextSkill.cfgData.iMaxAttackDistance + curSkillUseInfo.TargetActor.handle.shape.AvgCollisionRadius);
                    num *= num;
                    VInt3 num4 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                    if (num4.sqrMagnitudeLong2D > num)
                    {
                        this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0);
                        this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(true);
                        return EBTStatus.BT_RUNNING;
                    }
                    this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
                    return EBTStatus.BT_SUCCESS;
                }
                this.m_wrapper.ClearTarget();
                this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("朝技能目标点移动", "朝技能目标点移动,前提是当前技能已经设置")]
        public virtual EBTStatus MoveToSkillTarget()
        {
            if (this.m_closeToTargetFrame > 0)
            {
                this.m_closeToTargetFrame--;
                return EBTStatus.BT_RUNNING;
            }
            this.m_closeToTargetFrame = 1;
            if (this.m_wrapper == null)
            {
                this.m_closeToTargetFrame = 0;
                return EBTStatus.BT_FAILURE;
            }
            SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
            Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
            if (nextSkill != null)
            {
                long num;
                switch (curSkillUseInfo.AppointType)
                {
                    case SkillRangeAppointType.Target:
                    {
                        if ((curSkillUseInfo.TargetActor == 0) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
                        {
                            this.m_closeToTargetFrame = 0;
                            return EBTStatus.BT_FAILURE;
                        }
                        num = (long) (nextSkill.cfgData.iMaxAttackDistance + curSkillUseInfo.TargetActor.handle.shape.AvgCollisionRadius);
                        num *= num;
                        VInt3 num3 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                        if (num3.sqrMagnitudeLong2D > num)
                        {
                            this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0);
                            return EBTStatus.BT_RUNNING;
                        }
                        this.m_closeToTargetFrame = 0;
                        return EBTStatus.BT_SUCCESS;
                    }
                    case SkillRangeAppointType.Pos:
                    {
                        num = (long) (nextSkill.cfgData.iMaxAttackDistance * nextSkill.cfgData.iMaxAttackDistance);
                        VInt3 num2 = this.m_wrapper.actorLocation - curSkillUseInfo.UseVector;
                        if (num2.sqrMagnitudeLong2D <= num)
                        {
                            this.m_closeToTargetFrame = 0;
                            return EBTStatus.BT_SUCCESS;
                        }
                        this.m_wrapper.RealMovePosition(curSkillUseInfo.UseVector, 0);
                        return EBTStatus.BT_RUNNING;
                    }
                    case SkillRangeAppointType.Directional:
                        this.m_closeToTargetFrame = 0;
                        return EBTStatus.BT_SUCCESS;
                }
            }
            this.m_closeToTargetFrame = 0;
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("朝技能目标点移动,大于指定的范围则丢失目标", "朝技能目标点移动,大于指定的范围则丢失目标,前提是当前技能已经设置")]
        public virtual EBTStatus MoveToSkillTargetWithRange(int range)
        {
            if (this.m_closeToTargetFrame > 0)
            {
                this.m_closeToTargetFrame--;
                return EBTStatus.BT_RUNNING;
            }
            this.m_closeToTargetFrame = 1;
            if (this.m_wrapper == null)
            {
                this.m_closeToTargetFrame = 0;
                return EBTStatus.BT_FAILURE;
            }
            SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
            Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
            if (nextSkill != null)
            {
                long num;
                switch (curSkillUseInfo.AppointType)
                {
                    case SkillRangeAppointType.Target:
                    {
                        if (((curSkillUseInfo.TargetActor == 0) || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState) || !curSkillUseInfo.TargetActor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
                        {
                            this.m_closeToTargetFrame = 0;
                            this.m_wrapper.TerminateMove();
                            this.m_wrapper.ClearTarget();
                            return EBTStatus.BT_FAILURE;
                        }
                        long num2 = range * range;
                        VInt3 num4 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                        if (num4.sqrMagnitudeLong2D > num2)
                        {
                            this.m_closeToTargetFrame = 0;
                            this.m_wrapper.TerminateMove();
                            this.m_wrapper.ClearTarget();
                            return EBTStatus.BT_FAILURE;
                        }
                        num = (long) (nextSkill.cfgData.iMaxAttackDistance + curSkillUseInfo.TargetActor.handle.shape.AvgCollisionRadius);
                        num *= num;
                        VInt3 num5 = this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location;
                        if (num5.sqrMagnitudeLong2D > num)
                        {
                            this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0);
                            return EBTStatus.BT_RUNNING;
                        }
                        this.m_closeToTargetFrame = 0;
                        return EBTStatus.BT_SUCCESS;
                    }
                    case SkillRangeAppointType.Pos:
                    {
                        num = (long) (nextSkill.cfgData.iMaxAttackDistance * nextSkill.cfgData.iMaxAttackDistance);
                        VInt3 num3 = this.m_wrapper.actorLocation - curSkillUseInfo.UseVector;
                        if (num3.sqrMagnitudeLong2D > num)
                        {
                            this.m_wrapper.RealMovePosition(curSkillUseInfo.UseVector, 0);
                            return EBTStatus.BT_RUNNING;
                        }
                        this.m_closeToTargetFrame = 0;
                        return EBTStatus.BT_SUCCESS;
                    }
                }
            }
            this.m_closeToTargetFrame = 0;
            this.m_wrapper.ClearTarget();
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("向目标移动", "前提是已设定目标")]
        public void MoveToTarget()
        {
            this.m_wrapper.MoveToTarget();
        }

        [MethodMetaInfo("普攻选择范围内的敌人", "普攻选择范围内的敌人")]
        public uint NormalAttackSearchEnemy(int srchR)
        {
            SelectEnemyType selectLowHp;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
            if (((ownerPlayer != null) && (this.m_wrapper.myTarget != 0)) && ownerPlayer.bCommonAttackLockMode)
            {
                if (this.m_wrapper.IsTargetObjInSearchDistance())
                {
                    return this.m_wrapper.myTarget.handle.ObjID;
                }
                this.m_wrapper.ClearTarget();
            }
            if (ownerPlayer == null)
            {
                selectLowHp = SelectEnemyType.SelectLowHp;
            }
            else
            {
                selectLowHp = ownerPlayer.AttackTargetMode;
            }
            if (selectLowHp == SelectEnemyType.SelectLowHp)
            {
                return Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchLowestHpTarget(this.m_wrapper, srchR);
            }
            return Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchNearestTarget(this.m_wrapper, srchR);
        }

        [MethodMetaInfo("通知系统进入战斗", "通知进入战斗")]
        public void NotifyEventSysEnterCombat()
        {
            this.m_wrapper.SetSelfInBattle();
        }

        [MethodMetaInfo("通知系统脱离战斗", "通知脱离战斗")]
        public void NotifyEventSysExitCombat()
        {
            this.m_wrapper.SetSelfExitBattle();
        }

        [MethodMetaInfo("通知友军自己被攻击", "通知友军自己被攻击,range是半径")]
        public void NotifySelfCampSelfBeAttacked(int range)
        {
            this.m_wrapper.NotifySelfCampSelfBeAttacked(range);
        }

        [MethodMetaInfo("通知友军自己要主动攻击谁", "通知友军自己要主动攻击,range是半径")]
        public void NotifySelfCampSelfWillAttack(int range)
        {
            this.m_wrapper.NotifySelfCampSelfWillAttack(range);
        }

        public virtual void OnCreate()
        {
        }

        public virtual void OnGet()
        {
            this.m_wrapper = null;
            this.m_isActionPlaying = false;
            this.m_currentAction.Release();
            this.m_closeToTargetFrame = 0;
            this.m_dengerCoolTick = 30;
        }

        public virtual void OnRecycle()
        {
            this.m_wrapper = null;
            this.m_currentAction.Release();
        }

        [MethodMetaInfo("播放一段ageAction", "")]
        public EBTStatus PlayAgeAction(string actionName)
        {
            if (this.m_isActionPlaying)
            {
                if (this.m_currentAction != 0)
                {
                    return EBTStatus.BT_RUNNING;
                }
                this.m_isActionPlaying = false;
                return EBTStatus.BT_SUCCESS;
            }
            if ((actionName == null) || (actionName.Length <= 0))
            {
                return EBTStatus.BT_FAILURE;
            }
            GameObject[] objArray1 = new GameObject[] { base.gameObject };
            this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(actionName, true, false, objArray1));
            SkillUseContext context = new SkillUseContext {
                Originator = this.m_wrapper.actorPtr
            };
            this.m_currentAction.handle.refParams.AddRefParam("SkillContext", context);
            if (this.m_currentAction == 0)
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_isActionPlaying = true;
            return EBTStatus.BT_RUNNING;
        }

        [MethodMetaInfo("播放Animation", "")]
        public void PlayAnimation(string animationName, float blendTime, int layer, bool loop)
        {
            this.m_wrapper.PlayAnimation(animationName, blendTime, layer, loop);
        }

        [MethodMetaInfo("播放一段出生ageAction", "")]
        public EBTStatus PlayBornAgeAction()
        {
            if (this.m_wrapper.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
            {
                return EBTStatus.BT_INVALID;
            }
            this.m_wrapper.actor.Visible = true;
            string actionName = StringHelper.UTF8BytesToString(ref GameDataMgr.heroDatabin.GetDataByKey((uint) this.m_wrapper.actor.TheActorMeta.ConfigId).szBorn_Age);
            return this.PlayAgeAction(actionName);
        }

        [MethodMetaInfo("播放一段死亡剧情ageAction", "")]
        public EBTStatus PlayDeadAgeAction()
        {
            if (this.m_isActionPlaying)
            {
                if (this.m_currentAction != 0)
                {
                    return EBTStatus.BT_RUNNING;
                }
                this.m_isActionPlaying = false;
                return EBTStatus.BT_SUCCESS;
            }
            string deadAgePath = this.m_wrapper.actor.CharInfo.deadAgePath;
            if ((deadAgePath == null) || (deadAgePath.Length <= 0))
            {
                return EBTStatus.BT_FAILURE;
            }
            GameObject[] objArray1 = new GameObject[] { base.gameObject };
            this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(deadAgePath, true, false, objArray1));
            if (this.m_currentAction == 0)
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_isActionPlaying = true;
            return EBTStatus.BT_RUNNING;
        }

        [MethodMetaInfo("播放死亡后移动到指定点ageAction", "专为")]
        public EBTStatus PlayDeadMoveToPositionAgeAction()
        {
            if (this.m_isActionPlaying)
            {
                if (this.m_currentAction != 0)
                {
                    return EBTStatus.BT_RUNNING;
                }
                this.m_isActionPlaying = false;
                return EBTStatus.BT_SUCCESS;
            }
            string deadAgePath = this.m_wrapper.actor.CharInfo.deadAgePath;
            if ((deadAgePath == null) || (deadAgePath.Length <= 0))
            {
                return EBTStatus.BT_FAILURE;
            }
            GameObject[] objArray1 = new GameObject[] { base.gameObject, this.m_wrapper.m_deadPointGo };
            this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(deadAgePath, true, false, objArray1));
            if (this.m_currentAction == 0)
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_isActionPlaying = true;
            return EBTStatus.BT_RUNNING;
        }

        [MethodMetaInfo("播放剧情对话", "播放剧情对话")]
        public void PlayDialogue(int groupId)
        {
            if (groupId > 0)
            {
                MonoSingleton<DialogueProcessor>.GetInstance().StartDialogue(groupId);
            }
        }

        [MethodMetaInfo("播放一段AgeHelper的action", "")]
        public EBTStatus PlayHelperAgeAction(string actionName)
        {
            ActionHelper component = base.gameObject.GetComponent<ActionHelper>();
            if (component == null)
            {
                return EBTStatus.BT_FAILURE;
            }
            if (this.m_isActionPlaying)
            {
                if (this.m_currentAction != 0)
                {
                    return EBTStatus.BT_RUNNING;
                }
                this.m_isActionPlaying = false;
                return EBTStatus.BT_SUCCESS;
            }
            if ((actionName == null) || (actionName.Length <= 0))
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_currentAction = new PoolObjHandle<AGE.Action>(component.PlayAction(actionName));
            if (this.m_currentAction == 0)
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_isActionPlaying = true;
            return EBTStatus.BT_RUNNING;
        }

        [MethodMetaInfo("播放英雄动作声音", "播放英雄动作声音")]
        public void PlayHeroActSound(EActType actType)
        {
            if (this.m_sound_Interval >= 0x4650)
            {
                string soundName = null;
                for (int i = 0; i < this.m_wrapper.actor.CharInfo.ActSounds.Length; i++)
                {
                    if (this.m_wrapper.actor.CharInfo.ActSounds[i].SoundActType == actType)
                    {
                        soundName = this.m_wrapper.actor.CharInfo.ActSounds[i].ActSoundName;
                        break;
                    }
                }
                if (Singleton<CSoundManager>.GetInstance().PlayHeroActSound(soundName) != 0)
                {
                    this.m_sound_Interval = 0;
                }
            }
        }

        [MethodMetaInfo("播放一段复活ageAction", "")]
        public EBTStatus PlayReviveAgeAction()
        {
            if (this.m_wrapper.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
            {
                return EBTStatus.BT_INVALID;
            }
            string actionName = StringHelper.UTF8BytesToString(ref GameDataMgr.heroDatabin.GetDataByKey((uint) this.m_wrapper.actor.TheActorMeta.ConfigId).szRevive_Age);
            return this.PlayAgeAction(actionName);
        }

        public static void Preload(ref ActorPreloadTab result)
        {
            result.AddParticle("Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01");
        }

        [MethodMetaInfo("朝某个方向移动", "朝某个方向移动")]
        public virtual void RealMoveDirection(Vector3 dest)
        {
            VInt3 direction = new VInt3(dest);
            this.m_wrapper.RealMoveDirection(direction, 0);
        }

        [MethodMetaInfo("朝某个方向移动带ID", "朝某个方向移动带ID")]
        public virtual void RealMoveDirectionWithID(Vector3 dest, uint id)
        {
            VInt3 direction = new VInt3(dest);
            this.m_wrapper.RealMoveDirection(direction, id);
        }

        [MethodMetaInfo("移动到目标点", "移动到目标点")]
        public virtual void RealMovePosition(Vector3 dest)
        {
            VInt3 num = new VInt3(dest);
            this.m_wrapper.RealMovePosition(num, 0);
        }

        [MethodMetaInfo("移动到目标点带ID", "移动到目标点带ID")]
        public virtual void RealMovePositionWithID(Vector3 dest, uint id)
        {
            VInt3 num = new VInt3(dest);
            this.m_wrapper.RealMovePosition(num, id);
        }

        [MethodMetaInfo("移动到Actor目标点", "移动到Actor目标点")]
        public virtual void RealMoveToActor(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor != 0)
            {
                this.m_wrapper.RealMovePosition(actor.handle.location, 0);
            }
        }

        [MethodMetaInfo("移动到Actor目标点左边", "移动到Actor目标点左边")]
        public virtual void RealMoveToActorLeft(uint objID, int unit)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor != 0)
            {
                ActorRoot handle = actor.handle;
                VInt3 dest = handle.location + new VInt3((handle.forward.z * -1) * unit, handle.forward.y * unit, handle.forward.x * unit);
                this.m_wrapper.RealMovePosition(dest, 0);
            }
        }

        [MethodMetaInfo("移动到Actor目标点右边", "移动到Actor目标点右边")]
        public virtual void RealMoveToActorRight(uint objID, int unit)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            if (actor != 0)
            {
                ActorRoot handle = actor.handle;
                VInt3 dest = handle.location + new VInt3(handle.forward.z * unit, handle.forward.y * unit, (handle.forward.x * unit) * -1);
                this.m_wrapper.RealMovePosition(dest, 0);
            }
        }

        [MethodMetaInfo("使用技能", "")]
        public EBTStatus RealUseSkill(SkillSlotType InSlot)
        {
            if (this.m_wrapper.RealUseSkill(InSlot))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        public void Reset()
        {
            this.StopCurAgeAction();
            this.m_currentAction.Release();
            this.m_closeToTargetFrame = 0;
            this.m_dengerCoolTick = 30;
            this.m_frame = 0;
        }

        [MethodMetaInfo("重置寻路路径", "设置寻路路点从第一个开始")]
        public EBTStatus ResetRouteStartPoint()
        {
            if (((this.m_wrapper.m_curWaypointsHolder != null) && (this.m_wrapper.m_curWaypointsHolder.startPoint != null)) && (this.m_wrapper.m_curWaypointTarget.transform != null))
            {
                this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
                this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
            }
            return EBTStatus.BT_SUCCESS;
        }

        [MethodMetaInfo("选择离自己最近的一条兵线", "选择离自己最近的一条兵线")]
        public EBTStatus SelectNearestRoute()
        {
            if ((Singleton<BattleLogic>.GetInstance() == null) || (Singleton<BattleLogic>.GetInstance().mapLogic == null))
            {
                return EBTStatus.BT_FAILURE;
            }
            ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
            if ((waypointsList == null) || (waypointsList.Count == 0))
            {
                return EBTStatus.BT_FAILURE;
            }
            long num = 0x7fffffffffffffffL;
            WaypointsHolder holder = null;
            for (int i = 0; i < waypointsList.Count; i++)
            {
                VInt3 num3 = new VInt3(waypointsList[i].startPoint.transform.position);
                VInt3 num5 = this.m_wrapper.actorLocation - num3;
                long num4 = num5.sqrMagnitudeLong2D;
                if (num4 < num)
                {
                    holder = waypointsList[i];
                    num = num4;
                }
            }
            if (holder == null)
            {
                return EBTStatus.BT_FAILURE;
            }
            this.m_wrapper.m_curWaypointsHolder = holder;
            this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
            this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
            return EBTStatus.BT_SUCCESS;
        }

        [MethodMetaInfo("选择一条兵线做路径", "随机选择一条兵线")]
        public bool SelectRoute()
        {
            if (this.m_wrapper == null)
            {
                DebugHelper.Assert(false, "m_wrapper为空");
                return false;
            }
            if (Singleton<BattleLogic>.GetInstance().mapLogic == null)
            {
                object[] inParameters = new object[] { Singleton<GameStateCtrl>.instance.currentStateName };
                DebugHelper.Assert(false, "BattleLogic.GetInstance().mapLogic为空, GameState:{0}", inParameters);
                return false;
            }
            ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
            if ((waypointsList == null) || (waypointsList.Count == 0))
            {
                return false;
            }
            int num = FrameRandom.Random(0x2710) % waypointsList.Count;
            if (waypointsList[num] == null)
            {
                DebugHelper.Assert(false, "routeList[index]为空");
                return false;
            }
            this.m_wrapper.m_curWaypointsHolder = waypointsList[num];
            this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
            this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
            return true;
        }

        [MethodMetaInfo("根据在阵营中的位置选择一条兵线", "根据在阵营中的位置选择一条兵线")]
        public EBTStatus SelectRouteBySelfCampIndex()
        {
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            ActorServerData actorData = new ActorServerData();
            actorDataProvider.GetActorServerData(ref this.m_wrapper.actor.TheActorMeta, ref actorData);
            int bornPointIndex = actorData.TheExtraInfo.BornPointIndex;
            ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
            if ((waypointsList == null) || (waypointsList.Count == 0))
            {
                return EBTStatus.BT_INVALID;
            }
            if (bornPointIndex < 0)
            {
                return EBTStatus.BT_INVALID;
            }
            for (int i = 0; i < waypointsList.Count; i++)
            {
                if ((waypointsList[i] != null) && (waypointsList[i].m_index == bornPointIndex))
                {
                    this.m_wrapper.m_curWaypointsHolder = waypointsList[i];
                    this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
                    this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
                    return EBTStatus.BT_SUCCESS;
                }
            }
            bornPointIndex = bornPointIndex % waypointsList.Count;
            this.m_wrapper.m_curWaypointsHolder = waypointsList[bornPointIndex];
            this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
            this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
            return EBTStatus.BT_SUCCESS;
        }

        [MethodMetaInfo("根据在队伍中的位置选择一条兵线", "随机选择一条兵线")]
        public bool SelectRouteBySelfIndex()
        {
            ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
            if ((waypointsList != null) && (waypointsList.Count != 0))
            {
                Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
                if ((ownerPlayer == null) || (this.m_wrapper == null))
                {
                    return false;
                }
                int heroTeamPosIndex = ownerPlayer.GetHeroTeamPosIndex((uint) this.m_wrapper.actor.TheActorMeta.ConfigId);
                if (heroTeamPosIndex >= 0)
                {
                    for (int i = 0; i < waypointsList.Count; i++)
                    {
                        if (waypointsList[i] == null)
                        {
                            return false;
                        }
                        if (waypointsList[i].m_index == heroTeamPosIndex)
                        {
                            this.m_wrapper.m_curWaypointsHolder = waypointsList[i];
                            this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
                            this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
                            return true;
                        }
                    }
                    if (waypointsList.Count > heroTeamPosIndex)
                    {
                        this.m_wrapper.m_curWaypointsHolder = waypointsList[heroTeamPosIndex];
                        this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
                        this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
                        return true;
                    }
                    if (waypointsList.Count > 0)
                    {
                        this.m_wrapper.m_curWaypointsHolder = waypointsList[0];
                        this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
                        this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodMetaInfo("设定目标", "")]
        public void SelectTarget(uint objID)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
            this.m_wrapper.SelectTarget(actor);
        }

        [MethodMetaInfo("设置是否走纯AI", "设置AI模式")]
        public void SetAutoAI(bool isAuto)
        {
            this.m_wrapper.m_isAutoAI = isAuto;
        }

        [MethodMetaInfo("设置当前的行为", "")]
        public void SetCurBehavior(ObjBehaviMode behaviMode)
        {
            if (!this.m_wrapper.IsDeadState)
            {
                this.m_wrapper.SetObjBehaviMode(behaviMode);
            }
        }

        public void SetInDanger()
        {
            if (Singleton<FrameSynchr>.instance.bActive)
            {
                this.m_dengerCoolTick = 30;
            }
            else
            {
                this.m_dengerCoolTick = 60;
            }
        }

        [MethodMetaInfo("设置自己进入危险状态", "设置自己进入危险状态")]
        public void SetInDanger(int frame)
        {
            this.m_dengerCoolTick = frame;
        }

        [MethodMetaInfo("设置是否被敌方攻击", "")]
        public void SetIsAttackByEnemy(bool yesOrNot)
        {
            this.m_wrapper.m_isAttacked = yesOrNot;
        }

        [MethodMetaInfo("设置是否被敌方英雄攻击", "")]
        public void SetIsAttackByEnemyHero(bool yesOrNot)
        {
            this.m_wrapper.m_isAttackedByEnemyHero = yesOrNot;
        }

        [MethodMetaInfo("设定技能", "")]
        public EBTStatus SetSkill(SkillSlotType InSlot)
        {
            if (this.m_wrapper.SetSkill(InSlot, false))
            {
                return EBTStatus.BT_SUCCESS;
            }
            return EBTStatus.BT_FAILURE;
        }

        [MethodMetaInfo("设定技能特殊释放", "")]
        public void SetSkillSpecial(SkillSlotType InSlot)
        {
            this.m_wrapper.SetSkill(InSlot, true);
        }

        [MethodMetaInfo("切换目标为嘲讽自己的敌人", "切换目标为嘲讽自己的敌人")]
        public void SetTauntMeActorAsMyTarget()
        {
            this.m_wrapper.myTarget = this.m_wrapper.m_tauntMeActor;
        }

        [MethodMetaInfo("发送攻击信号", "发送攻击信号")]
        public void ShowAtkSignal()
        {
            SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
            if (panel != null)
            {
                Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
                uint playerId = ownerPlayer.PlayerId;
                uint configId = (uint) ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
                panel.ExecCommand(playerId, configId, 2, 0, 0, 0, 0, 0, 0);
            }
        }

        [MethodMetaInfo("显示头部感叹号", "显示头部感叹号")]
        public void ShowHeadExclamationMark()
        {
            this.m_wrapper.actor.HudControl.ShowHeadExclamationMark("Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01", 2f);
        }

        [MethodMetaInfo("随机发送信号", "随机发送信号")]
        public void ShowRandomSignal()
        {
            SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
            if (panel != null)
            {
                Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
                uint playerId = ownerPlayer.PlayerId;
                uint configId = (uint) ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
                List<PoolObjHandle<ActorRoot>> towerActors = Singleton<GameObjMgr>.GetInstance().TowerActors;
                int num4 = FrameRandom.Random((uint) towerActors.Count);
                int signalID = 1 + FrameRandom.Random(4);
                PoolObjHandle<ActorRoot> handle = towerActors[num4];
                ActorRoot root = handle.handle;
                if (root.IsSelfCamp(this.m_wrapper.actor))
                {
                    if (signalID == 2)
                    {
                        signalID = 1;
                    }
                }
                else if (signalID == 3)
                {
                    signalID = 1;
                }
                int num6 = (root.location.x + FrameRandom.Random(0x2710)) - 0x1388;
                int y = root.location.y;
                int num8 = (root.location.z + FrameRandom.Random(0x1770)) - 0xbb8;
                panel.ExecCommand(playerId, configId, signalID, num6 / 0x3e8, y / 0x3e8, num8 / 0x3e8, 0, 0, 0);
            }
        }

        [MethodMetaInfo("发送集合信号", "发送集合信号")]
        public void ShowTogetherSignal()
        {
            SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
            if (panel != null)
            {
                Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
                uint playerId = ownerPlayer.PlayerId;
                uint configId = (uint) ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
                panel.ExecCommand(playerId, configId, 4, 0, 0, 0, 0, 0, 0);
            }
        }

        private void Start()
        {
        }

        [MethodMetaInfo("停止当前播放的AgeAction", "停止当前播放的AgeAction")]
        public void StopCurAgeAction()
        {
            if (this.m_isActionPlaying)
            {
                if (this.m_currentAction != 0)
                {
                    ActionManager.Instance.StopAction((AGE.Action) this.m_currentAction);
                }
                this.m_isActionPlaying = false;
            }
        }

        [MethodMetaInfo("停止移动并清空移动命令", "停止移动,清除移动命令,停止移动组件")]
        public void StopMove()
        {
            this.m_wrapper.CmdStopMove();
        }

        [MethodMetaInfo("切换到下一个行为", "切换到下一个行为,如果有的话")]
        public void SwitchToNextBehavior()
        {
            if ((this.m_wrapper.nextBehavior != ObjBehaviMode.State_Null) && !this.m_wrapper.IsDeadState)
            {
                this.m_wrapper.SetObjBehaviMode(this.m_wrapper.nextBehavior);
                this.m_wrapper.nextBehavior = ObjBehaviMode.State_Null;
            }
            else if (this.m_wrapper.m_isAutoAI && (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                this.m_wrapper.SetObjBehaviMode(ObjBehaviMode.State_Idle);
            }
        }

        [MethodMetaInfo("终止当前的移动", "仅仅停止移动组件,不走了")]
        public void TerminateMove()
        {
            this.m_wrapper.TerminateMove();
        }

        private void Update()
        {
        }

        public override void UpdateLogic(int delta)
        {
            if (!base.m_isPaused)
            {
                int num = 1;
                if (!Singleton<FrameSynchr>.GetInstance().bActive)
                {
                    num = 2;
                }
                this.m_frame++;
                if (this.m_wrapper != null)
                {
                    if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        if (this.m_wrapper.myBehavior == ObjBehaviMode.State_AutoAI)
                        {
                            if (((this.m_frame + ((int) this.m_wrapper.actor.ObjID)) % (3 * num)) == 0)
                            {
                                base.UpdateLogic(delta);
                            }
                        }
                        else
                        {
                            base.UpdateLogic(delta);
                        }
                    }
                    else
                    {
                        MonsterWrapper wrapper = this.m_wrapper.actor.AsMonster();
                        if (((wrapper != null) && (wrapper.cfgInfo != null)) && (wrapper.cfgInfo.bIsBoss > 0))
                        {
                            if (((this.m_frame + ((int) this.m_wrapper.actor.ObjID)) % (4 * num)) == 0)
                            {
                                base.UpdateLogic(delta);
                            }
                        }
                        else if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                        {
                            if (((this.m_frame + ((int) this.m_wrapper.actor.ObjID)) % (5 * num)) == 0)
                            {
                                base.UpdateLogic(delta);
                            }
                        }
                        else if (((this.m_frame + ((int) this.m_wrapper.actor.ObjID)) % (6 * num)) == 0)
                        {
                            base.UpdateLogic(delta);
                        }
                    }
                }
                if (this.m_dengerCoolTick > 0)
                {
                    this.m_dengerCoolTick--;
                }
                if (this.m_sound_Interval < 0x4650)
                {
                    this.m_sound_Interval += delta;
                }
            }
        }

        [MethodMetaInfo("使用普攻缓存", "使用普攻缓存")]
        public bool UseCommonAttackCache()
        {
            bool flag = false;
            SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
            if (skillUseCache != null)
            {
                flag = skillUseCache.IsCacheCommonAttack();
                if (flag)
                {
                    skillUseCache.UseSkillCache(this.m_wrapper.actorPtr);
                }
            }
            return flag;
        }

        [MethodMetaInfo("使用回城", "使用回城")]
        public void UseGoHomeSkill()
        {
            this.m_wrapper.UseGoHomeSkill();
        }

        [MethodMetaInfo("对自己使用回血技能", "对自己使用技能,会自动判断该技能是否准备好")]
        public void UseHpRecoverSkillToSelf()
        {
            this.m_wrapper.UseHpRecoverSkillToSelf();
        }
    }
}

