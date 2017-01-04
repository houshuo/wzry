namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class BounceBulletDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> attackActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackId = -1;
        public bool bMoveRotate = true;
        private int curEffectCount;
        [ObjectTemplate(new System.Type[] {  })]
        public int destId = -1;
        private VInt3 destPosition = VInt3.zero;
        private Dictionary<uint, int> effectCountMap = new Dictionary<uint, int>();
        public int gravity;
        private AccelerateMotionControler gravityControler;
        private int lastTime;
        public int maxEffectCount = 3;
        public int maxTargetCount = 1;
        private PoolObjHandle<ActorRoot> moveActor;
        public int searchRadius = 0xc350;
        private bool stopCondtion;
        private PoolObjHandle<ActorRoot> tarActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_2;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_3;
        public int velocity = 0x3a98;

        private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
        {
            if ((actor != null) && !this.stopCondtion)
            {
                VInt3 location = actor.location;
                VInt3 num2 = this.destPosition - location;
                int newMagn = (int) ((this.velocity * nDelta) / 0x3e8);
                Vector3 position = actor.gameObject.transform.position;
                if (this.gravity < 0)
                {
                    VInt num4;
                    num2.y = 0;
                    position += (Vector3) num2.NormalizeTo(newMagn);
                    position.y += ((float) this.gravityControler.GetMotionLerpDistance((int) nDelta)) / 1000f;
                    if (PathfindingUtility.GetGroundY((VInt3) position, out num4) && (position.y < ((float) num4)))
                    {
                        position.y = (float) num4;
                    }
                }
                else
                {
                    position += (Vector3) num2.NormalizeTo(newMagn);
                }
                actor.gameObject.transform.position = position;
            }
        }

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.stopCondtion;
        }

        public override BaseEvent Clone()
        {
            BounceBulletDuration duration = ClassObjPool<BounceBulletDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BounceBulletDuration duration = src as BounceBulletDuration;
            this.targetId = duration.targetId;
            this.attackId = duration.attackId;
            this.destId = duration.destId;
            this.effectCountMap.Clear();
            this.velocity = duration.velocity;
            this.bMoveRotate = duration.bMoveRotate;
            this.tarActor = duration.tarActor;
            this.moveActor = duration.moveActor;
            this.attackActor = duration.attackActor;
            this.destPosition = duration.destPosition;
            this.stopCondtion = duration.stopCondtion;
            this.lastTime = duration.lastTime;
            this.maxTargetCount = duration.maxTargetCount;
            this.maxEffectCount = duration.maxEffectCount;
            this.searchRadius = duration.searchRadius;
            this.gravity = duration.gravity;
            this.TargetSkillCombine_1 = duration.TargetSkillCombine_1;
            this.TargetSkillCombine_2 = duration.TargetSkillCombine_2;
            this.TargetSkillCombine_3 = duration.TargetSkillCombine_3;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.stopCondtion = false;
            this.Init(_action);
            base.Enter(_action, _track);
        }

        private void Init(AGE.Action _action)
        {
            this.moveActor = _action.GetActorHandle(this.targetId);
            if (this.moveActor == 0)
            {
                this.stopCondtion = true;
            }
            else
            {
                this.tarActor = _action.GetActorHandle(this.destId);
                this.attackActor = _action.GetActorHandle(this.attackId);
                if ((this.tarActor == 0) || (this.attackActor == 0))
                {
                    this.stopCondtion = true;
                }
                else
                {
                    this.gravityControler = new AccelerateMotionControler();
                    this.moveActor.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
                }
            }
        }

        private bool InitGravity()
        {
            VInt3 num = this.destPosition - this.moveActor.handle.location;
            int num2 = (int) IntMath.Divide((long) (num.magnitude2D * 0x3e8L), (long) this.velocity);
            if (num2 == 0)
            {
                return false;
            }
            VInt groundY = 0;
            if (PathfindingUtility.GetGroundY(this.destPosition, out groundY))
            {
                this.gravityControler.InitMotionControler(num2, groundY.i - this.moveActor.handle.location.y, this.gravity);
            }
            else
            {
                this.gravityControler.InitMotionControler(num2, 0, this.gravity);
            }
            return true;
        }

        private void InitTarget()
        {
            VInt3 one = VInt3.one;
            int iBulletHeight = 0;
            this.destPosition = this.tarActor.handle.location;
            CActorInfo charInfo = this.tarActor.handle.CharInfo;
            if (charInfo != null)
            {
                iBulletHeight = charInfo.iBulletHeight;
                one = this.moveActor.handle.location - this.destPosition;
                one.y = 0;
                one = one.NormalizeTo(0x3e8);
                this.destPosition += IntMath.Divide(one, (long) charInfo.iCollisionSize.x, 0x3e8L);
            }
            this.destPosition.y += iBulletHeight;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.moveActor != 0)
            {
                this.moveActor.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
            }
            this.tarActor.Release();
            this.moveActor.Release();
        }

        private void MoveToTarget(AGE.Action _action, int _localTime)
        {
            int num = _localTime - this.lastTime;
            this.lastTime = _localTime;
            VInt3 location = this.moveActor.handle.location;
            this.InitTarget();
            VInt3 num3 = this.destPosition - location;
            if (this.bMoveRotate)
            {
                this.RotateMoveBullet(num3);
            }
            int newMagn = (this.velocity * num) / 0x3e8;
            if ((newMagn * newMagn) >= num3.sqrMagnitudeLong2D)
            {
                this.moveActor.handle.location = this.destPosition;
                if (this.curEffectCount < this.maxEffectCount)
                {
                    this.curEffectCount++;
                    this.SpawnBuff(_action);
                    this.tarActor = this.SearchTarget();
                    if (this.tarActor == 0)
                    {
                        this.stopCondtion = true;
                    }
                    else
                    {
                        int num5 = 0;
                        if (this.effectCountMap.TryGetValue(this.tarActor.handle.ObjID, out num5))
                        {
                            this.effectCountMap[this.tarActor.handle.ObjID] = ++num5;
                        }
                        else
                        {
                            this.effectCountMap.Add(this.tarActor.handle.ObjID, ++num5);
                        }
                    }
                }
                else
                {
                    this.stopCondtion = true;
                }
            }
            else
            {
                VInt3 num6;
                if ((this.gravity < 0) && this.InitGravity())
                {
                    VInt num7;
                    num3.y = 0;
                    num6 = location + num3.NormalizeTo(newMagn);
                    num6.y += this.gravityControler.GetMotionDeltaDistance(num);
                    if (PathfindingUtility.GetGroundY(num6, out num7) && (num6.y < num7.i))
                    {
                        num6.y = num7.i;
                    }
                }
                else
                {
                    num6 = location + num3.NormalizeTo(newMagn);
                }
                this.moveActor.handle.location = num6;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.tarActor.Release();
            this.moveActor.Release();
            this.attackActor.Release();
            this.gravityControler = null;
            this.effectCountMap.Clear();
            this.destPosition = VInt3.zero;
            this.stopCondtion = false;
            this.lastTime = 0;
            this.curEffectCount = 0;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (((this.moveActor != 0) && (this.tarActor != 0)) && !this.stopCondtion)
            {
                this.MoveToTarget(_action, _localTime);
                base.Process(_action, _track, _localTime);
            }
        }

        private void RotateMoveBullet(VInt3 _dir)
        {
            if (_dir != VInt3.zero)
            {
                this.moveActor.handle.forward = _dir.NormalizeTo(0x3e8);
                Quaternion identity = Quaternion.identity;
                identity = Quaternion.LookRotation((Vector3) _dir);
                this.moveActor.handle.rotation = identity;
            }
        }

        private PoolObjHandle<ActorRoot> SearchTarget()
        {
            int num = 0;
            PoolObjHandle<ActorRoot> handle = new PoolObjHandle<ActorRoot>();
            PoolObjHandle<ActorRoot> handle2 = new PoolObjHandle<ActorRoot>();
            int num2 = 4;
            ulong num3 = (ulong) (this.searchRadius * this.searchRadius);
            ulong num4 = num3;
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            for (int i = 0; i < gameActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle3 = gameActors[i];
                ActorRoot target = handle3.handle;
                if ((((this.attackActor.handle.CanAttack(target) && ((num2 & (((int) 1) << target.TheActorMeta.ActorType)) <= 0)) && (target.ObjID != this.tarActor.handle.ObjID)) && target.HorizonMarker.IsVisibleFor(this.attackActor.handle.TheActorMeta.ActorCamp)) && (!this.effectCountMap.TryGetValue(target.ObjID, out num) || (num < this.maxTargetCount)))
                {
                    if (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        VInt3 num8 = target.location - this.tarActor.handle.location;
                        ulong num6 = (ulong) num8.sqrMagnitudeLong2D;
                        if (num6 < num3)
                        {
                            handle = handle3;
                            num3 = num6;
                        }
                    }
                    else
                    {
                        VInt3 num9 = target.location - this.tarActor.handle.location;
                        ulong num7 = (ulong) num9.sqrMagnitudeLong2D;
                        if (num7 < num4)
                        {
                            handle2 = handle3;
                            num4 = num7;
                        }
                    }
                }
            }
            return ((handle == 0) ? handle2 : handle);
        }

        private void SpawnBuff(AGE.Action _action)
        {
            if ((this.tarActor != 0) && (this.attackActor != 0))
            {
                bool flag = false;
                SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                if (refParamObject != null)
                {
                    bool introduced2 = this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_1, false);
                    flag = introduced2 | this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_2, false);
                    if (flag | this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_3, false))
                    {
                        this.tarActor.handle.ActorControl.BeAttackHit(this.attackActor);
                    }
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

