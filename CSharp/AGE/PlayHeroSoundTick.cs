namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class PlayHeroSoundTick : TickEvent
    {
        protected string CachedEventName;
        [AssetReference(AssetRefType.Sound)]
        public string eventName;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            PlayHeroSoundTick tick = ClassObjPool<PlayHeroSoundTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlayHeroSoundTick tick = src as PlayHeroSoundTick;
            this.targetId = tick.targetId;
            this.eventName = tick.eventName;
            this.CachedEventName = tick.CachedEventName;
            if (string.IsNullOrEmpty(this.CachedEventName) && !string.IsNullOrEmpty(this.eventName))
            {
                this.CachedEventName = this.eventName + "_Down";
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.eventName = string.Empty;
            this.CachedEventName = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (this.ShouldUseNormal(_action, _track, ref actorHandle))
            {
                Singleton<CSoundManager>.instance.PlayBattleSound(this.eventName, actorHandle, gameObject);
            }
            else
            {
                if (string.IsNullOrEmpty(this.CachedEventName))
                {
                    this.CachedEventName = this.eventName + "_Down";
                }
                Singleton<CSoundManager>.instance.PlayBattleSound(this.CachedEventName, actorHandle, gameObject);
            }
        }

        protected bool ShouldUseNormal(AGE.Action _action, Track _track, ref PoolObjHandle<ActorRoot> Actor)
        {
            if ((Actor != 0) && ActorHelper.IsHostCtrlActor(ref Actor))
            {
                return true;
            }
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            return (((refParamObject != null) && (refParamObject.Originator != 0)) && ActorHelper.IsHostCtrlActor(ref refParamObject.Originator));
        }
    }
}

