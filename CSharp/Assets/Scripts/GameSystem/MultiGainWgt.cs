namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MultiGainWgt : ActivityWidget
    {
        private ListView<MultiGainListItem> _elementList;
        private GameObject _elementTmpl;
        private const float SPACING_Y = 5f;

        public MultiGainWgt(GameObject node, ActivityView view) : base(node, view)
        {
            this._elementTmpl = Utility.FindChild(node, "Template");
            float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            this._elementList = new ListView<MultiGainListItem>();
            for (int i = 0; i < phaseList.Count; i++)
            {
                GameObject obj2 = null;
                if (i > 0)
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(this._elementTmpl);
                    obj2.transform.SetParent(this._elementTmpl.transform.parent);
                    obj2.transform.localPosition = this._elementList[i - 1].root.transform.localPosition + new Vector3(0f, -(height + 5f), 0f);
                    obj2.transform.localScale = Vector3.one;
                    obj2.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    this._elementTmpl.SetActive(true);
                    obj2 = this._elementTmpl;
                }
                MultiGainListItem item = new MultiGainListItem(obj2, (MultiGainPhase) phaseList[i]);
                this._elementList.Add(item);
            }
            if (this._elementList.Count == 0)
            {
                this._elementTmpl.SetActive(false);
            }
            node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (this._elementList.Count <= 0) ? 0f : ((height * this._elementList.Count) + ((this._elementList.Count - 1) * 5f)));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGoto));
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
            this.Validate();
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGoto));
            for (int i = 0; i < this._elementList.Count; i++)
            {
                this._elementList[i].Clear();
                if (i > 0)
                {
                    UnityEngine.Object.Destroy(this._elementList[i].root);
                }
            }
            this._elementList = null;
            this._elementTmpl = null;
        }

        private void OnClickGoto(CUIEvent uiEvent)
        {
            for (int i = 0; i < this._elementList.Count; i++)
            {
                MultiGainListItem item = this._elementList[i];
                if (item.gotoBtn == uiEvent.m_srcWidget)
                {
                    base.view.form.Close();
                    item.actvPhase.AchieveJump();
                    break;
                }
            }
        }

        private void OnStateChange(Activity acty)
        {
            this.Validate();
        }

        public override void Validate()
        {
            for (int i = 0; i < this._elementList.Count; i++)
            {
                this._elementList[i].Validate();
            }
        }

        public class MultiGainListItem
        {
            public MultiGainPhase actvPhase;
            public GameObject gotoBtn;
            public Text gotoBtnTxt;
            public Text remainTimes;
            public const int REWARD_ITEM_COUNT = 4;
            public GameObject root;
            public Text tips;
            public Text title;

            public MultiGainListItem(GameObject node, MultiGainPhase ap)
            {
                this.root = node;
                this.actvPhase = ap;
                this.gotoBtn = Utility.FindChild(node, "Goto");
                this.gotoBtnTxt = Utility.GetComponetInChild<Text>(this.gotoBtn, "Text");
                this.title = Utility.GetComponetInChild<Text>(node, "Title");
                this.tips = Utility.GetComponetInChild<Text>(node, "Tips");
                this.remainTimes = Utility.GetComponetInChild<Text>(node, "RemainTimes");
                this.actvPhase.OnMaskStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
                this.actvPhase.OnTimeStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
            }

            public void Clear()
            {
                this.actvPhase.OnMaskStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
                this.actvPhase.OnTimeStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
            }

            private void OnStateChange(ActivityPhase ap)
            {
                this.Validate();
            }

            public void Validate()
            {
                this.title.text = this.actvPhase.Desc;
                this.tips.text = this.actvPhase.Tips;
                this.remainTimes.text = (this.actvPhase.LimitTimes <= 0) ? Singleton<CTextManager>.GetInstance().GetText("noLimit") : string.Format("{0:D}/{1:D}", this.actvPhase.RemainTimes, this.actvPhase.LimitTimes);
                if (this.actvPhase.timeState == ActivityPhase.TimeState.Started)
                {
                    bool readyForGo = this.actvPhase.ReadyForGo;
                    this.gotoBtn.GetComponent<CUIEventScript>().enabled = readyForGo;
                    this.gotoBtn.GetComponent<Button>().interactable = readyForGo;
                    this.gotoBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText(!readyForGo ? "finished" : "gotoFinish");
                    this.gotoBtnTxt.color = !readyForGo ? Color.gray : Color.white;
                }
                else
                {
                    this.gotoBtn.GetComponent<CUIEventScript>().enabled = false;
                    this.gotoBtn.GetComponent<Button>().interactable = false;
                    this.gotoBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText((this.actvPhase.timeState != ActivityPhase.TimeState.Closed) ? "notInTime" : "outOfTime");
                    this.gotoBtnTxt.color = Color.gray;
                }
            }
        }
    }
}

