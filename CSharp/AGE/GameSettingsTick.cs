namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using System;

    [EventCategory("MMGame/GameSettings")]
    public class GameSettingsTick : TickEvent
    {
        public bool bResetCastType;
        public Assets.Scripts.Framework.CommonAttactType CommonAttactType;
        public CastType UseCastType;

        public override BaseEvent Clone()
        {
            GameSettingsTick tick = ClassObjPool<GameSettingsTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            GameSettingsTick tick = src as GameSettingsTick;
            this.UseCastType = tick.UseCastType;
            this.bResetCastType = tick.bResetCastType;
            this.CommonAttactType = tick.CommonAttactType;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            if (this.bResetCastType)
            {
                Singleton<GameInput>.instance.SetSmartUse(GameSettings.TheCastType == CastType.SmartCast);
            }
            else
            {
                Singleton<GameInput>.instance.SetSmartUse(this.UseCastType == CastType.SmartCast);
            }
            GameSettings.TheCommonAttackType = this.CommonAttactType;
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

