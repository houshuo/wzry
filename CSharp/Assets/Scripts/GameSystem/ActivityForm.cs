namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;

    public abstract class ActivityForm
    {
        private ActivitySys _sys;

        public ActivityForm(ActivitySys sys)
        {
            this._sys = sys;
        }

        public abstract void Close();
        public abstract void Open();
        public abstract void Update();

        public abstract CUIFormScript formScript { get; }

        public ActivitySys Sys
        {
            get
            {
                return this._sys;
            }
        }
    }
}

