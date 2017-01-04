namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using UnityEngine;

    public abstract class BaseSkill : PooledClassObject
    {
        public string ActionName = string.Empty;
        public bool bAgeImmeExcute;
        protected PoolObjHandle<AGE.Action> curAction = new PoolObjHandle<AGE.Action>();
        public SkillUseContext skillContext = new SkillUseContext();
        public int SkillID;

        protected BaseSkill()
        {
        }

        public SkillUseContext GetSkillUseContext()
        {
            if (this.curAction == 0)
            {
                return null;
            }
            return this.curAction.handle.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
        }

        public PoolObjHandle<ActorRoot> GetTargetActor()
        {
            SkillUseContext skillUseContext = this.GetSkillUseContext();
            if (skillUseContext == null)
            {
                return new PoolObjHandle<ActorRoot>(null);
            }
            return skillUseContext.TargetActor;
        }

        public virtual void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
        {
            action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStoped);
            if ((this.curAction != 0) && (action == this.curAction))
            {
                this.curAction.Release();
            }
        }

        public override void OnRelease()
        {
            this.SkillID = 0;
            this.ActionName = string.Empty;
            this.curAction.Release();
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.SkillID = 0;
            this.ActionName = string.Empty;
            this.bAgeImmeExcute = false;
            this.curAction.Release();
            this.skillContext.Reset();
        }

        public virtual void Stop()
        {
            if (this.curAction != 0)
            {
                this.curAction.handle.Stop(false);
                this.curAction.Release();
            }
        }

        public virtual bool Use(PoolObjHandle<ActorRoot> user)
        {
            return ((((user != 0) && (this.skillContext != null)) && !string.IsNullOrEmpty(this.ActionName)) && this.UseImpl(user));
        }

        public virtual bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
        {
            if (((user == 0) || (this.skillContext == null)) || string.IsNullOrEmpty(this.ActionName))
            {
                return false;
            }
            this.skillContext.Copy(ref param);
            return this.UseImpl(user);
        }

        private bool UseImpl(PoolObjHandle<ActorRoot> user)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            VInt3 forward = VInt3.forward;
            switch (this.skillContext.AppointType)
            {
                case SkillRangeAppointType.Auto:
                case SkillRangeAppointType.Target:
                    flag = true;
                    break;

                case SkillRangeAppointType.Pos:
                    flag2 = true;
                    break;

                case SkillRangeAppointType.Directional:
                    flag3 = true;
                    forward = this.skillContext.UseVector;
                    break;

                case SkillRangeAppointType.Track:
                    flag2 = true;
                    flag3 = true;
                    forward = this.skillContext.EndVector - this.skillContext.UseVector;
                    if (forward.sqrMagnitudeLong < 1L)
                    {
                        forward = VInt3.forward;
                    }
                    break;
            }
            if (flag && (this.skillContext.TargetActor == 0))
            {
                return false;
            }
            if (flag)
            {
                GameObject[] objArray1 = new GameObject[] { user.handle.gameObject, this.skillContext.TargetActor.handle.gameObject };
                this.curAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, objArray1));
            }
            else
            {
                GameObject[] objArray2 = new GameObject[] { user.handle.gameObject };
                this.curAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, objArray2));
            }
            if (this.curAction == 0)
            {
                return false;
            }
            this.curAction.handle.onActionStop += new ActionStopDelegate(this.OnActionStoped);
            this.curAction.handle.refParams.AddRefParam("SkillObj", this);
            this.curAction.handle.refParams.AddRefParam("SkillContext", this.skillContext);
            if (flag)
            {
                this.curAction.handle.refParams.AddRefParam("TargetActor", this.skillContext.TargetActor);
            }
            if (flag2)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetPos", this.skillContext.UseVector);
            }
            if (flag3)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetDir", forward);
            }
            this.curAction.handle.refParams.SetRefParam("_BulletPos", this.skillContext.BulletPos);
            if (this.bAgeImmeExcute)
            {
                this.curAction.handle.UpdateLogic((int) Singleton<FrameSynchr>.GetInstance().FrameDelta);
            }
            return true;
        }

        public PoolObjHandle<AGE.Action> CurAction
        {
            get
            {
                return this.curAction;
            }
        }

        public virtual bool isBuff
        {
            get
            {
                return false;
            }
        }

        public virtual bool isBullet
        {
            get
            {
                return false;
            }
        }

        public bool isFinish
        {
            get
            {
                return (this.curAction == 0);
            }
        }
    }
}

