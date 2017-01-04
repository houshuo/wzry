namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;

    public class NoticeActivity : Activity
    {
        private ResWealText _config;

        public NoticeActivity(ActivitySys mgr, ResWealText config) : base(mgr, config.stCommon)
        {
            this._config = config;
        }

        public void Jump()
        {
            if (this._config.dwJumpEntrance > 0)
            {
                CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) this._config.dwJumpEntrance);
            }
            else if (!string.IsNullOrEmpty(this._config.szJumpBtnToUrl))
            {
                CUICommonSystem.OpenUrl(this._config.szJumpBtnToUrl, true);
            }
        }

        public override bool Completed
        {
            get
            {
                return (base.timeState > Activity.TimeState.Going);
            }
        }

        public override uint ID
        {
            get
            {
                return this._config.dwID;
            }
        }

        public string JumpLabel
        {
            get
            {
                return Utility.UTF8Convert(this._config.szJumpBtnText);
            }
        }

        public override COM_WEAL_TYPE Type
        {
            get
            {
                return (COM_WEAL_TYPE) 0x65;
            }
        }
    }
}

