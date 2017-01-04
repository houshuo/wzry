namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [EventCategory("MMGame/Skill")]
    public class MoveCityProcessBarDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        private ulong curTime;
        private Image m_process;
        private GameObject processBar;
        private ulong starTime;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        private ulong totalTime;

        public override BaseEvent Clone()
        {
            MoveCityProcessBarDuration duration = ClassObjPool<MoveCityProcessBarDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MoveCityProcessBarDuration duration = src as MoveCityProcessBarDuration;
            this.targetId = duration.targetId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            this.starTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
            this.totalTime = (ulong) base.length;
            if ((this.actorObj != 0) && ActorHelper.IsHostCtrlActor(ref this.actorObj))
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
                if (form != null)
                {
                    this.processBar = Utility.FindChild(form.gameObject, "GoBackProcessBar");
                    if (this.processBar != null)
                    {
                        this.processBar.CustomSetActive(true);
                        Transform transform = this.processBar.transform.Find("GoBackTime");
                        this.m_process = (transform == null) ? null : transform.GetComponent<Image>();
                        this.m_process.CustomFillAmount(0f);
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.processBar != null)
            {
                this.processBar.CustomSetActive(false);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.curTime = 0L;
            this.starTime = 0L;
            this.totalTime = 0L;
            this.processBar = null;
            this.m_process = null;
            this.actorObj.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
            if (this.m_process != null)
            {
                this.curTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                float num = ((float) (this.curTime - this.starTime)) / ((float) this.totalTime);
                num = (num >= 0f) ? num : 0f;
                num = (num <= 1f) ? num : 1f;
                this.m_process.CustomFillAmount(num);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

