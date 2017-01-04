namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Drama")]
    public class PauseGameDuration : DurationEvent
    {
        public bool bEffectTimeScale = true;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId;

        public override BaseEvent Clone()
        {
            PauseGameDuration duration = ClassObjPool<PauseGameDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PauseGameDuration duration = src as PauseGameDuration;
            this.srcId = duration.srcId;
            this.bEffectTimeScale = duration.bEffectTimeScale;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, this.bEffectTimeScale);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.srcId = 0;
            this.bEffectTimeScale = true;
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

