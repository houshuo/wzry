namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class BannerWidget : ActivityWidget
    {
        private int _curStepIndex;
        private float _lastScrollTime;
        private bool _leftToRight;
        private CUIStepListScript _stepList;
        private Text _timeCD;
        private ListView<UrlAction> _urlaList;
        public const float SCROLL_TIME_SPAN = 3.5f;

        public BannerWidget(GameObject node, ActivityView view) : base(node, view)
        {
            this._stepList = Utility.GetComponetInChild<CUIStepListScript>(node, "StepList");
            this._stepList.SetDontUpdate(true);
            this._timeCD = Utility.GetComponetInChild<Text>(node, "TimeCD");
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_BannerClick, new CUIEventManager.OnUIEventHandler(this.OnClick));
            this.Validate();
        }

        public override void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_BannerClick, new CUIEventManager.OnUIEventHandler(this.OnClick));
        }

        private void OnClick(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((this._urlaList != null) && (srcWidgetIndexInBelongedList < this._urlaList.Count))
            {
                UrlAction action = this._urlaList[srcWidgetIndexInBelongedList];
                if (action.Execute())
                {
                    base.view.form.Close();
                }
            }
        }

        public override void Update()
        {
            this.updateAutoScroll();
            this.updateOverTime();
        }

        private void updateAutoScroll()
        {
            int count = this._urlaList.Count;
            if (((count > 1) && (this._curStepIndex < count)) && (Time.time > (this._lastScrollTime + (this._urlaList[this._curStepIndex].showTime * 0.001f))))
            {
                this._lastScrollTime = Time.time;
                if ((((this._curStepIndex + 1) == count) && this._leftToRight) || ((this._curStepIndex == 0) && !this._leftToRight))
                {
                    this._leftToRight = !this._leftToRight;
                }
                this._curStepIndex += !this._leftToRight ? -1 : 1;
                this._stepList.MoveElementInScrollArea(this._curStepIndex, false);
            }
        }

        private void updateOverTime()
        {
            UrlAction action = this._urlaList[this._curStepIndex];
            if (action.overTime > 0L)
            {
                DateTime time = Utility.ULongToDateTime(action.overTime);
                DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                TimeSpan span = (TimeSpan) (time - time2);
                string str = Singleton<CTextManager>.GetInstance().GetText("TIME_SPAN_FORMAT").Replace("{0}", span.Days.ToString()).Replace("{1}", span.Hours.ToString()).Replace("{2}", span.Minutes.ToString()).Replace("{3}", span.Seconds.ToString());
                this._timeCD.gameObject.CustomSetActive(time >= time2);
                this._timeCD.text = str;
            }
            else
            {
                this._timeCD.gameObject.CustomSetActive(false);
            }
        }

        public override void Validate()
        {
            this._urlaList = UrlAction.ParseFromText(base.view.activity.Content, null);
            if (this._urlaList.Count > 0)
            {
                this._stepList.SetElementAmount(this._urlaList.Count);
                for (int i = 0; i < this._urlaList.Count; i++)
                {
                    UrlAction action = this._urlaList[i];
                    CUIListElementScript elemenet = this._stepList.GetElemenet(i);
                    if (null != elemenet)
                    {
                        CUIHttpImageScript component = elemenet.GetComponent<CUIHttpImageScript>();
                        if (null != component)
                        {
                            component.SetImageUrl(action.target);
                        }
                    }
                }
                this._curStepIndex = 0;
                this._leftToRight = true;
                this._stepList.MoveElementInScrollArea(this._curStepIndex, true);
                this._lastScrollTime = Time.time;
                this.updateOverTime();
            }
        }
    }
}

