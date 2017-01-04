namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    public class TriggerNewbieEquipTick : TickEvent
    {
        public bool bHide;
        public eNewbieEuipTimeType triggerTimeType;

        public override BaseEvent Clone()
        {
            TriggerNewbieEquipTick tick = ClassObjPool<TriggerNewbieEquipTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerNewbieEquipTick tick = src as TriggerNewbieEquipTick;
            this.triggerTimeType = tick.triggerTimeType;
            this.bHide = tick.bHide;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            switch (this.triggerTimeType)
            {
                case eNewbieEuipTimeType.BuyFirstEquip:
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onBuyFirstEquip, new uint[0]);
                    break;

                case eNewbieEuipTimeType.HasEnoughMoneyBuyEquip:
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onHasEnoughMoneyButEquip, new uint[0]);
                    break;

                case eNewbieEuipTimeType.HideEquipPanel:
                    Singleton<CBattleGuideManager>.GetInstance().ShowBuyEuipPanel(!this.bHide);
                    break;

                case eNewbieEuipTimeType.EquipShopIntroduce:
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEuipShopGuide, new uint[0]);
                    break;
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        public enum eNewbieEuipTimeType
        {
            BuyFirstEquip,
            HasEnoughMoneyBuyEquip,
            HideEquipPanel,
            EquipShopIntroduce
        }
    }
}

