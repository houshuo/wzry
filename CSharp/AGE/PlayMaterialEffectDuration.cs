namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Material")]
    public class PlayMaterialEffectDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actor_;
        public MaterialEffectType effectType;
        public Vector3 highLitColor;
        private int playingId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            PlayMaterialEffectDuration duration = ClassObjPool<PlayMaterialEffectDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PlayMaterialEffectDuration duration = src as PlayMaterialEffectDuration;
            this.targetId = duration.targetId;
            this.effectType = duration.effectType;
            this.highLitColor = duration.highLitColor;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.actor_ = _action.GetActorHandle(this.targetId);
            if (this.actor_ != 0)
            {
                MaterialHurtEffect matHurtEffect = this.actor_.handle.MatHurtEffect;
                if (matHurtEffect == null)
                {
                    this.actor_.Release();
                }
                else
                {
                    switch (this.effectType)
                    {
                        case MaterialEffectType.Freeze:
                            this.playingId = matHurtEffect.PlayFreezeEffect();
                            break;

                        case MaterialEffectType.Stone:
                            this.playingId = matHurtEffect.PlayStoneEffect();
                            break;

                        case MaterialEffectType.Translucent:
                            matHurtEffect.SetTranslucent(true);
                            break;

                        case MaterialEffectType.HighLit:
                            matHurtEffect.PlayHighLitEffect(this.highLitColor);
                            break;
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.actor_ != 0)
            {
                MaterialHurtEffect matHurtEffect = this.actor_.handle.MatHurtEffect;
                if (matHurtEffect != null)
                {
                    switch (this.effectType)
                    {
                        case MaterialEffectType.Freeze:
                            matHurtEffect.StopFreezeEffect(this.playingId);
                            break;

                        case MaterialEffectType.Stone:
                            matHurtEffect.StopStoneEffect(this.playingId);
                            break;

                        case MaterialEffectType.Translucent:
                            matHurtEffect.SetTranslucent(false);
                            break;

                        case MaterialEffectType.HighLit:
                            matHurtEffect.StopHighLitEffect();
                            break;
                    }
                }
            }
        }

        public override void OnRelease()
        {
            base.OnRelease();
            this.actor_.Release();
            this.playingId = -1;
            this.targetId = 0;
        }
    }
}

