namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Drama")]
    public class DialogProcessorTick : TickEvent
    {
        public int DialogGroupId;

        public override BaseEvent Clone()
        {
            DialogProcessorTick tick = ClassObjPool<DialogProcessorTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            DialogProcessorTick tick = src as DialogProcessorTick;
            this.DialogGroupId = tick.DialogGroupId;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            int dialogGroupId = this.DialogGroupId;
            if (dialogGroupId <= 0)
            {
                _action.refParams.GetRefParam("DialogGroupIdRaw", ref dialogGroupId);
            }
            if (dialogGroupId > 0)
            {
                MonoSingleton<DialogueProcessor>.GetInstance().StartDialogue(dialogGroupId);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

