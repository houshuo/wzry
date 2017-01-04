namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class PlaySoundTick : TickEvent
    {
        [AssetReference(AssetRefType.Sound)]
        public string eventName = string.Empty;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            PlaySoundTick tick = ClassObjPool<PlaySoundTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlaySoundTick tick = src as PlaySoundTick;
            this.targetId = tick.targetId;
            this.eventName = tick.eventName;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.eventName = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            Singleton<CSoundManager>.instance.PlayBattleSound(this.eventName, actorHandle, gameObject);
        }
    }
}

