namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("Animation")]
    public class PlayAnimDuration : DurationEvent
    {
        public bool alwaysAnimate;
        public bool applyActionSpeed;
        public bool bLoop;
        public string clipName = string.Empty;
        public float crossFadeTime;
        public float endTime = 99999f;
        public int layer = 1;
        private Dictionary<int, Animation> m_animationCache = new Dictionary<int, Animation>();
        public bool playNextAnim;
        public float startTime;
        [ObjectTemplate(new System.Type[] { typeof(Animation) })]
        public int targetId;

        public override BaseEvent Clone()
        {
            PlayAnimDuration duration = ClassObjPool<PlayAnimDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlayAnimDuration duration = src as PlayAnimDuration;
            this.targetId = duration.targetId;
            this.clipName = duration.clipName;
            this.crossFadeTime = duration.crossFadeTime;
            this.layer = duration.layer;
            this.bLoop = duration.bLoop;
            this.startTime = duration.startTime;
            this.endTime = duration.endTime;
            this.applyActionSpeed = duration.applyActionSpeed;
            this.playNextAnim = duration.playNextAnim;
            this.alwaysAnimate = duration.alwaysAnimate;
            this.m_animationCache.Clear();
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if ((gameObject != null) && (base.length != 0))
            {
                GameObject actorMesh = gameObject;
                PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
                if (this.GetAnimation(gameObject) == null)
                {
                    if (actorHandle != 0)
                    {
                        actorMesh = actorHandle.handle.ActorMesh;
                    }
                    else
                    {
                        actorMesh = null;
                    }
                }
                if (actorMesh != null)
                {
                    Animation animation = this.GetAnimation(actorMesh);
                    AnimationState state = animation[this.clipName];
                    if (state != null)
                    {
                        if (this.alwaysAnimate && (animation.cullingType != AnimationCullingType.AlwaysAnimate))
                        {
                            animation.cullingType = AnimationCullingType.AlwaysAnimate;
                        }
                        float num = 1f;
                        if (this.startTime < 0f)
                        {
                            this.startTime = 0f;
                        }
                        if (this.endTime > state.clip.length)
                        {
                            this.endTime = state.clip.length;
                        }
                        if (!this.bLoop)
                        {
                            num = ((this.endTime - this.startTime) / base.lengthSec) * (!this.applyActionSpeed ? 1f : _action.playSpeed.single);
                        }
                        else
                        {
                            num = !this.applyActionSpeed ? 1f : _action.playSpeed.single;
                        }
                        AnimPlayComponent animControl = actorHandle.handle.AnimControl;
                        if (animControl != null)
                        {
                            PlayAnimParam param = new PlayAnimParam {
                                animName = this.clipName,
                                blendTime = this.crossFadeTime,
                                loop = this.bLoop,
                                layer = this.layer,
                                speed = num
                            };
                            animControl.Play(param);
                        }
                        else
                        {
                            if (state.enabled)
                            {
                                animation.Stop();
                            }
                            if (this.crossFadeTime > 0f)
                            {
                                animation.CrossFade(this.clipName, this.crossFadeTime);
                            }
                            else
                            {
                                animation.Play(this.clipName);
                            }
                        }
                        state.speed = num;
                    }
                }
            }
        }

        private Animation GetAnimation(GameObject obj)
        {
            int instanceID = obj.GetInstanceID();
            Animation component = null;
            if (!this.m_animationCache.TryGetValue(instanceID, out component))
            {
                component = obj.GetComponent<Animation>();
                this.m_animationCache.Add(instanceID, component);
            }
            return component;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if ((gameObject != null) && (base.length != 0))
            {
                GameObject actorMesh = gameObject;
                PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
                if (this.GetAnimation(gameObject) == null)
                {
                    if (actorHandle != 0)
                    {
                        actorMesh = actorHandle.handle.ActorMesh;
                    }
                    else
                    {
                        actorMesh = null;
                    }
                }
                if (actorMesh != null)
                {
                    if (actorHandle != 0)
                    {
                        actorHandle.handle.AnimControl.Stop(this.clipName, this.playNextAnim);
                    }
                    else
                    {
                        this.GetAnimation(actorMesh)[this.clipName].enabled = false;
                    }
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.clipName = string.Empty;
            this.crossFadeTime = 0f;
            this.layer = 1;
            this.bLoop = false;
            this.startTime = 0f;
            this.endTime = 99999f;
            this.applyActionSpeed = false;
            this.alwaysAnimate = false;
            this.m_animationCache.Clear();
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

