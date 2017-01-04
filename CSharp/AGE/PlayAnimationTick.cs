namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("Animation")]
    public class PlayAnimationTick : TickEvent
    {
        public bool alwaysAnimate;
        public bool applyActionSpeed;
        public bool bNoTimeScale;
        public string clipName = string.Empty;
        public float crossFadeTime;
        public int layer = 1;
        public bool loop;
        private Dictionary<int, Animation> m_animationCache = new Dictionary<int, Animation>();
        public float playSpeed = 1f;
        [ObjectTemplate(new System.Type[] { typeof(Animation) })]
        public int targetId;

        public override BaseEvent Clone()
        {
            PlayAnimationTick tick = ClassObjPool<PlayAnimationTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlayAnimationTick tick = src as PlayAnimationTick;
            this.targetId = tick.targetId;
            this.clipName = tick.clipName;
            this.crossFadeTime = tick.crossFadeTime;
            this.playSpeed = tick.playSpeed;
            this.layer = tick.layer;
            this.loop = tick.loop;
            this.bNoTimeScale = tick.bNoTimeScale;
            this.alwaysAnimate = tick.alwaysAnimate;
            this.applyActionSpeed = tick.applyActionSpeed;
            this.m_animationCache.Clear();
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

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.clipName = string.Empty;
            this.crossFadeTime = 0f;
            this.playSpeed = 1f;
            this.layer = 1;
            this.loop = false;
            this.bNoTimeScale = false;
            this.alwaysAnimate = false;
            this.applyActionSpeed = false;
            this.m_animationCache.Clear();
        }

        public override void PostProcess(AGE.Action _action, Track _track, int _localTime)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
            {
                GameObject actorMesh = gameObject;
                if (this.GetAnimation(gameObject) == null)
                {
                    PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
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
                    AnimationState state = this.GetAnimation(actorMesh)[this.clipName];
                    if (state != null)
                    {
                        state.speed = this.playSpeed * (!this.applyActionSpeed ? 1f : _action.playSpeed.single);
                    }
                }
            }
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
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
                        float num = this.playSpeed * (!this.applyActionSpeed ? 1f : _action.playSpeed.single);
                        if (this.bNoTimeScale)
                        {
                            DialogueProcessor.PlayAnimNoTimeScale(animation, this.clipName, this.loop, null);
                        }
                        else
                        {
                            AnimPlayComponent component = (actorHandle == 0) ? null : actorHandle.handle.AnimControl;
                            if (component != null)
                            {
                                PlayAnimParam param = new PlayAnimParam {
                                    animName = this.clipName,
                                    blendTime = this.crossFadeTime,
                                    loop = this.loop,
                                    layer = this.layer,
                                    speed = num
                                };
                                component.Play(param);
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
                        }
                        state.speed = num;
                    }
                }
            }
        }

        public override void ProcessBlend(AGE.Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
            {
                GameObject actorMesh = gameObject;
                if (this.GetAnimation(gameObject) == null)
                {
                    PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
                    if (actorHandle != 0)
                    {
                        actorMesh = actorHandle.handle.ActorMesh;
                    }
                    else
                    {
                        actorMesh = null;
                    }
                }
                if ((actorMesh != null) && (_prevEvent != null))
                {
                    AnimationState state = this.GetAnimation(actorMesh)[this.clipName];
                    if (state != null)
                    {
                        state.speed = this.playSpeed * (!this.applyActionSpeed ? 1f : _action.playSpeed.single);
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

