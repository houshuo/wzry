namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Drama")]
    public class SkillHudCtrlTick : TickEvent
    {
        public bool bActivate;
        public bool bAllSkillSlots;
        public bool bHideOtherBtn;
        public bool bHighlight;
        public bool bHighlightJoystick;
        public bool bHighlightLearnBtn;
        public bool bHighlightOterBtn;
        public bool bJoystick;
        public bool bNoActivatingOthers;
        public bool bPauseGame;
        public bool bPlayerShowAnim;
        public bool bShow;
        public bool bShowLearnBtn;
        public bool bSkillSlot;
        public bool bYes;
        public enRestSkillSlotType restSkillBtnType;
        public SkillSlotType SlotType;

        public override BaseEvent Clone()
        {
            SkillHudCtrlTick tick = ClassObjPool<SkillHudCtrlTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillHudCtrlTick tick = src as SkillHudCtrlTick;
            this.bSkillSlot = tick.bSkillSlot;
            this.SlotType = tick.SlotType;
            this.bAllSkillSlots = tick.bAllSkillSlots;
            this.bHighlight = tick.bHighlight;
            this.bHighlightLearnBtn = tick.bHighlightLearnBtn;
            this.bActivate = tick.bActivate;
            this.bShow = tick.bShow;
            this.bShowLearnBtn = tick.bShowLearnBtn;
            this.bYes = tick.bYes;
            this.bPauseGame = tick.bPauseGame;
            this.bNoActivatingOthers = tick.bNoActivatingOthers;
            this.bJoystick = tick.bJoystick;
            this.bHighlightJoystick = tick.bHighlightJoystick;
            this.restSkillBtnType = tick.restSkillBtnType;
            this.bHideOtherBtn = tick.bHideOtherBtn;
            this.bHighlightOterBtn = tick.bHighlightOterBtn;
            this.bPlayerShowAnim = tick.bPlayerShowAnim;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (Singleton<CBattleSystem>.GetInstance().IsFormOpen)
            {
                this.ProcessJoystick();
                this.ProcessSkillSlot();
            }
        }

        private void ProcessJoystick()
        {
            if (this.bJoystick)
            {
                Singleton<BattleSkillHudControl>.GetInstance().HighlightJoystick(this.bHighlightJoystick);
            }
        }

        private void ProcessSkillSlot()
        {
            if (this.bSkillSlot)
            {
                if (this.bShow)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().Show(this.SlotType, this.bYes, this.bAllSkillSlots, this.bPlayerShowAnim);
                }
                if (this.bShowLearnBtn)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().ShowLearnBtn(this.SlotType, this.bYes, this.bAllSkillSlots);
                }
                if (this.bActivate)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().Activate(this.SlotType, this.bYes, this.bAllSkillSlots);
                }
                if (this.bHighlight)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().Highlight(this.SlotType, this.bYes, this.bAllSkillSlots, !this.bNoActivatingOthers, this.bPauseGame);
                }
                if (this.bHighlightLearnBtn)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().HighlightLearnBtn(this.SlotType, this.bYes, this.bAllSkillSlots, !this.bNoActivatingOthers, this.bPauseGame);
                }
                if (this.bHideOtherBtn)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().ShowRestkSkillBtn(this.restSkillBtnType, this.bYes);
                }
                if (this.bHighlightOterBtn)
                {
                    Singleton<BattleSkillHudControl>.GetInstance().HighlishtRestSkillBtn(this.restSkillBtnType, this.bHighlightOterBtn, !this.bNoActivatingOthers, this.bPauseGame);
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

