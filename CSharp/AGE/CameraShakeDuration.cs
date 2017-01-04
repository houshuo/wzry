namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    public class CameraShakeDuration : DurationEvent
    {
        private bool enableFixedCam;
        private bool enterShaking;
        public bool filter_allies;
        public bool filter_enemy;
        public bool filter_self;
        public bool filter_target;
        private Vector3 lastOffset = Vector3.zero;
        private Vector3 originPos = Vector3.zero;
        private float recovery = 0.1f;
        public static int shakeDistance = 0x3a98;
        public Vector3 shakeRange = Vector3.zero;
        private Vector3 shock = Vector3.zero;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;
        private GameObject targetObject;
        public bool useAccumOffset;
        public bool useMainCamera;

        public bool CheckShakeDistance(ActorRoot captain, ActorRoot user)
        {
            if ((captain == null) || (user == null))
            {
                return false;
            }
            if (captain == user)
            {
                return true;
            }
            VInt3 num = captain.location - user.location;
            long shakeDistance = CameraShakeDuration.shakeDistance;
            shakeDistance *= shakeDistance;
            return (num.sqrMagnitudeLong2D <= shakeDistance);
        }

        public override BaseEvent Clone()
        {
            CameraShakeDuration duration = ClassObjPool<CameraShakeDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            CameraShakeDuration duration = src as CameraShakeDuration;
            this.useMainCamera = duration.useMainCamera;
            this.targetId = duration.targetId;
            this.shakeRange = duration.shakeRange;
            this.originPos = duration.originPos;
            this.shock = duration.shock;
            this.recovery = duration.recovery;
            this.enableFixedCam = duration.enableFixedCam;
            this.targetObject = duration.targetObject;
            this.enterShaking = duration.enterShaking;
            this.filter_target = duration.filter_target;
            this.filter_self = duration.filter_self;
            this.filter_enemy = duration.filter_enemy;
            this.filter_allies = duration.filter_allies;
            this.useAccumOffset = duration.useAccumOffset;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            if (!Singleton<BattleLogic>.GetInstance().IsModifyingCamera && this.ShouldShake(_action))
            {
                if (this.useMainCamera && (Camera.main != null))
                {
                    this.targetObject = Camera.main.gameObject;
                }
                else
                {
                    this.targetObject = _action.GetGameObject(this.targetId);
                }
                if ((this.targetObject == null) || (this.targetObject.transform == null))
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    Singleton<BattleLogic>.GetInstance().IsModifyingCamera = true;
                    this.enterShaking = true;
                    this.originPos = this.targetObject.transform.localPosition;
                    this.shock = this.shakeRange;
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.enterShaking)
            {
                if (this.useMainCamera && (Camera.main != null))
                {
                    this.targetObject = Camera.main.gameObject;
                }
                else
                {
                    this.targetObject = _action.GetGameObject(this.targetId);
                }
                if ((this.targetObject == null) || (this.targetObject.transform == null))
                {
                    this.enterShaking = false;
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    this.shock = Vector3.zero;
                    if (this.useAccumOffset)
                    {
                        Transform transform = this.targetObject.transform;
                        transform.localPosition -= this.lastOffset;
                    }
                    else
                    {
                        this.targetObject.transform.localPosition = this.originPos;
                    }
                    Singleton<BattleLogic>.GetInstance().IsModifyingCamera = false;
                    this.enterShaking = false;
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.useMainCamera = false;
            this.targetId = -1;
            this.shakeRange = Vector3.zero;
            this.originPos = Vector3.zero;
            this.shock = Vector3.zero;
            this.recovery = 0.1f;
            this.enableFixedCam = false;
            this.targetObject = null;
            this.enterShaking = false;
            this.filter_target = false;
            this.filter_self = false;
            this.filter_enemy = false;
            this.filter_allies = false;
            this.useAccumOffset = false;
            this.lastOffset = Vector3.zero;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (this.enterShaking)
            {
                if (this.useMainCamera && (Camera.main != null))
                {
                    this.targetObject = Camera.main.gameObject;
                }
                else
                {
                    this.targetObject = _action.GetGameObject(this.targetId);
                }
                if ((this.targetObject == null) || (this.targetObject.transform == null))
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    float x = UnityEngine.Random.Range(-this.shock.x, this.shock.x);
                    float y = UnityEngine.Random.Range(-this.shock.y, this.shock.y);
                    Vector3 vector = new Vector3(x, y, UnityEngine.Random.Range(-this.shock.z, this.shock.z));
                    if (this.useAccumOffset)
                    {
                        Transform transform = this.targetObject.transform;
                        transform.localPosition += vector - this.lastOffset;
                        this.lastOffset = vector;
                    }
                    else
                    {
                        this.targetObject.transform.localPosition = vector + this.originPos;
                    }
                    this.shock = (Vector3) (this.shock * (1f - this.recovery));
                }
            }
        }

        public bool ShouldShake(AGE.Action _action)
        {
            BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
            SkillUseContext context = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            if ((refParamObject == null) || (context == null))
            {
                return true;
            }
            PoolObjHandle<ActorRoot> originator = context.Originator;
            if (ActorHelper.IsHostCtrlActor(ref originator) && this.filter_self)
            {
                return true;
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((this.filter_target && (hostPlayer != null)) && (context.TargetActor == hostPlayer.Captain))
            {
                return true;
            }
            if (hostPlayer != null)
            {
                Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(originator.handle.TheActorMeta.PlayerId);
                if (player != null)
                {
                    if (this.filter_enemy && (player.PlayerCamp != hostPlayer.PlayerCamp))
                    {
                        return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
                    }
                    if (this.filter_allies && (player.PlayerCamp == hostPlayer.PlayerCamp))
                    {
                        return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
                    }
                }
                else if (this.filter_enemy)
                {
                    return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
                }
            }
            return false;
        }

        public override bool SupportEditMode()
        {
            return false;
        }
    }
}

