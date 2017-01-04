namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CheckInWidget : ActivityWidget
    {
        private Text _awardDesc;
        private Text _awardTitle;
        private GameObject _getBtn;
        private Text _getBtnText;
        private Text _progLabel;
        private uint _remainSeconds;
        private Text _timeRemain;

        public CheckInWidget(GameObject node, ActivityView view) : base(node, view)
        {
            this._awardTitle = Utility.GetComponetInChild<Text>(node, "Content/AwardTitle");
            this._awardDesc = Utility.GetComponetInChild<Text>(node, "Content/AwardDesc");
            this._timeRemain = Utility.GetComponetInChild<Text>(node, "Content/TimeRemain");
            this._progLabel = Utility.GetComponetInChild<Text>(node, "Content/Progress");
            this._getBtn = Utility.FindChild(node, "GetAward");
            this._getBtnText = Utility.GetComponetInChild<Text>(this._getBtn, "Text");
            this.Validate();
            view.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChanged);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Wealfare_CheckInGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
        }

        public override void Clear()
        {
            base.view.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChanged);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Wealfare_CheckInGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
        }

        private void OnClickGet(CUIEvent uiEvent)
        {
            ActivityPhase curPhase = base.view.activity.CurPhase;
            if (curPhase != null)
            {
                curPhase.DrawReward();
            }
        }

        private void OnStateChanged(Activity actv)
        {
            this.Validate();
        }

        public override void Update()
        {
            if (this._remainSeconds > 0)
            {
                this._remainSeconds--;
                this._timeRemain.text = Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(this._remainSeconds));
            }
        }

        public override void Validate()
        {
            ActivityPhase curPhase = base.view.activity.CurPhase;
            if (curPhase != null)
            {
                if (curPhase.RewardDesc.Length > 0)
                {
                    this._awardDesc.text = curPhase.RewardDesc;
                }
                else
                {
                    CUseable useable = curPhase.GetUseable(0);
                    if (useable != null)
                    {
                        this._awardDesc.text = useable.m_name + ":" + curPhase.GetDropCount(0);
                    }
                }
                if (curPhase.ReadyForGet)
                {
                    this._getBtn.GetComponent<CUIEventScript>().enabled = true;
                    this._getBtn.GetComponent<Button>().interactable = true;
                    this._getBtnText.color = Color.white;
                    this._awardTitle.text = Singleton<CTextManager>.GetInstance().GetText("awardToday");
                    this._getBtnText.text = Singleton<CTextManager>.GetInstance().GetText("get");
                    this._remainSeconds = 0;
                    this._timeRemain.text = Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(0));
                }
                else
                {
                    this._getBtn.GetComponent<CUIEventScript>().enabled = false;
                    this._getBtn.GetComponent<Button>().interactable = false;
                    this._getBtnText.color = Color.gray;
                    if (base.view.activity.Completed)
                    {
                        this._awardTitle.text = Singleton<CTextManager>.GetInstance().GetText("awardToday");
                        this._getBtnText.text = Singleton<CTextManager>.GetInstance().GetText("finished");
                        this._remainSeconds = 0;
                        this._timeRemain.text = Singleton<CTextManager>.GetInstance().GetText("congraduFinish");
                    }
                    else
                    {
                        this._awardTitle.text = Singleton<CTextManager>.GetInstance().GetText("awardTomorrow");
                        this._getBtnText.text = Singleton<CTextManager>.GetInstance().GetText("notInTime");
                        DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                        DateTime time2 = time.AddDays(1.0);
                        time2 = new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0);
                        TimeSpan span = (TimeSpan) (time2 - time);
                        this._remainSeconds = (uint) span.TotalSeconds;
                        this._timeRemain.text = Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(this._remainSeconds));
                    }
                }
                this._progLabel.text = Singleton<CTextManager>.GetInstance().GetText("CheckInProgress").Replace("{0}", base.view.activity.Current.ToString()).Replace("{1}", base.view.activity.Target.ToString());
            }
        }
    }
}

