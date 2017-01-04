namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class BattleUiWidgetSwtichTick : TickEvent
    {
        public bool bAutoAi;
        public bool bBattleInfoView;
        public bool bDetailInfo;
        public bool bFreeCamera;
        public bool bPauseBtn;
        public bool bResumeBtn;
        public bool bSettingMenu;
        public bool bTrainingExit;
        public bool bTurnOn;

        public override BaseEvent Clone()
        {
            BattleUiWidgetSwtichTick tick = ClassObjPool<BattleUiWidgetSwtichTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BattleUiWidgetSwtichTick tick = src as BattleUiWidgetSwtichTick;
            this.bAutoAi = tick.bAutoAi;
            this.bFreeCamera = tick.bFreeCamera;
            this.bSettingMenu = tick.bSettingMenu;
            this.bBattleInfoView = tick.bBattleInfoView;
            this.bPauseBtn = tick.bPauseBtn;
            this.bResumeBtn = tick.bResumeBtn;
            this.bTrainingExit = tick.bTrainingExit;
            this.bDetailInfo = tick.bDetailInfo;
            this.bTurnOn = tick.bTurnOn;
        }

        private void EnableBattleInfoView(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "PanelBtn/btnViewBattleInfo");
        }

        private void EnableBattleUiByName(bool bInEnable, string inName)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                Transform transform = form.transform.FindChild(inName);
                GameObject obj2 = (transform == null) ? null : transform.gameObject;
                if (obj2 != null)
                {
                    obj2.CustomSetActive(bInEnable);
                }
            }
        }

        private void EnableDetailInfoBtn(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "ButtonViewSkillInfo");
        }

        private void EnablePauseBtn(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "panelTopRight/PauseBtn");
        }

        private void EnableResumeBtn(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "panelTopRight/ResumeBtn");
        }

        private void EnableSettingMenu(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "PanelBtn/MenuBtn");
        }

        private void EnableToggleAuto(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "PanelBtn/ToggleAutoBtn");
        }

        private void EnableToggleFreeCam(bool bInEnable)
        {
        }

        private void EnableTrainingExit(bool bInEnable)
        {
            this.EnableBattleUiByName(bInEnable, "Image/ButtonTrainingExit");
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            if (this.bAutoAi)
            {
                this.EnableToggleAuto(this.bTurnOn);
            }
            if (this.bFreeCamera)
            {
                this.EnableToggleFreeCam(this.bTurnOn);
            }
            if (this.bSettingMenu)
            {
                this.EnableSettingMenu(this.bTurnOn);
            }
            if (this.bBattleInfoView)
            {
                this.EnableBattleInfoView(this.bTurnOn);
            }
            if (this.bPauseBtn)
            {
                this.EnablePauseBtn(this.bTurnOn);
            }
            if (this.bResumeBtn)
            {
                this.EnableResumeBtn(this.bTurnOn);
            }
            if (this.bTrainingExit)
            {
                this.EnableTrainingExit(this.bTurnOn);
            }
            if (this.bDetailInfo)
            {
                this.EnableDetailInfoBtn(this.bTurnOn);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

