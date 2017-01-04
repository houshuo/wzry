namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class RewardWidget : ActivityWidget
    {
        private GameObject _elementTmpl;
        private ListView<RewardListItem> _rewardList;
        private const float SPACING_Y = 5f;

        public RewardWidget(GameObject node, ActivityView view) : base(node, view)
        {
            this._elementTmpl = Utility.FindChild(node, "Template");
            float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
            ListView<ActivityPhase> view2 = new ListView<ActivityPhase>(view.activity.PhaseList.Count);
            view2.AddRange(view.activity.PhaseList);
            view2.Sort(new Comparison<ActivityPhase>(RewardWidget.APSort));
            this._rewardList = new ListView<RewardListItem>();
            for (int i = 0; i < view2.Count; i++)
            {
                GameObject obj2 = null;
                if (i > 0)
                {
                    obj2 = (GameObject) UnityEngine.Object.Instantiate(this._elementTmpl);
                    obj2.transform.SetParent(this._elementTmpl.transform.parent);
                    obj2.transform.localPosition = this._rewardList[i - 1].root.transform.localPosition + new Vector3(0f, -(height + 5f), 0f);
                    obj2.transform.localScale = Vector3.one;
                    obj2.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    this._elementTmpl.SetActive(true);
                    obj2 = this._elementTmpl;
                }
                RewardListItem item = new RewardListItem(obj2, this, view2[i]);
                this._rewardList.Add(item);
            }
            if (this._rewardList.Count == 0)
            {
                this._elementTmpl.SetActive(false);
            }
            node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (this._rewardList.Count <= 0) ? 0f : ((height * this._rewardList.Count) + ((this._rewardList.Count - 1) * 5f)));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ClickGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
            this.Validate();
        }

        private static int APSort(ActivityPhase x, ActivityPhase y)
        {
            bool readyForGet = x.ReadyForGet;
            bool flag2 = y.ReadyForGet;
            if (readyForGet == flag2)
            {
                return ((x.Marked != y.Marked) ? (!x.Marked ? -1 : 1) : ((int) (x.ID - y.ID)));
            }
            return (!readyForGet ? 1 : -1);
        }

        public override void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ClickGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
            for (int i = 0; i < this._rewardList.Count; i++)
            {
                this._rewardList[i].Clear();
                if (i > 0)
                {
                    UnityEngine.Object.Destroy(this._rewardList[i].root);
                }
            }
            this._rewardList = null;
            this._elementTmpl = null;
        }

        private void OnClickGet(CUIEvent uiEvent)
        {
            for (int i = 0; i < this._rewardList.Count; i++)
            {
                RewardListItem item = this._rewardList[i];
                if (item.getBtn == uiEvent.m_srcWidget)
                {
                    if (item.actvPhase.ReadyForGet)
                    {
                        item.actvPhase.DrawReward();
                    }
                    else
                    {
                        base.view.form.Close();
                        item.actvPhase.AchieveJump();
                    }
                    break;
                }
            }
        }

        public override void Validate()
        {
            for (int i = 0; i < this._rewardList.Count; i++)
            {
                this._rewardList[i].Validate();
            }
        }

        public class RewardListItem
        {
            public ActivityPhase actvPhase;
            public GameObject[] cellList;
            public Image flag;
            public GameObject getBtn;
            public Text getBtnTxt;
            public RewardWidget ownerWgt;
            public const int REWARD_ITEM_COUNT = 4;
            public GameObject root;
            public Text tips;

            public RewardListItem(GameObject node, RewardWidget ownWgt, ActivityPhase ap)
            {
                this.root = node;
                this.ownerWgt = ownWgt;
                this.actvPhase = ap;
                this.cellList = new GameObject[4];
                for (int i = 0; i < 4; i++)
                {
                    this.cellList[i] = Utility.FindChild(node, "Items/Item" + i);
                }
                this.getBtn = Utility.FindChild(node, "GetAward");
                this.getBtnTxt = Utility.GetComponetInChild<Text>(this.getBtn, "Text");
                this.tips = Utility.GetComponetInChild<Text>(node, "Tips");
                this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
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
                if (this.actvPhase.Target > 0)
                {
                    this.tips.gameObject.SetActive(true);
                    this.tips.text = this.actvPhase.Tips;
                }
                else
                {
                    this.tips.gameObject.SetActive(false);
                }
                if (this.actvPhase.Marked)
                {
                    this.flag.gameObject.SetActive(true);
                    this.getBtn.SetActive(false);
                }
                else
                {
                    this.flag.gameObject.SetActive(false);
                    this.getBtn.SetActive(true);
                    if (this.actvPhase.ReadyForGet)
                    {
                        this.getBtn.GetComponent<CUIEventScript>().enabled = true;
                        this.getBtn.GetComponent<Button>().interactable = true;
                        this.getBtn.GetComponent<Image>().SetSprite((this.ownerWgt.view.form as CampaignForm).GetDynamicImage(CampaignForm.DynamicAssets.ButtonYellowImage));
                        this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText("get");
                        this.getBtnTxt.color = Color.white;
                    }
                    else
                    {
                        this.getBtn.GetComponent<Image>().SetSprite((this.ownerWgt.view.form as CampaignForm).GetDynamicImage(CampaignForm.DynamicAssets.ButtonBlueImage));
                        if (this.actvPhase.AchieveStateValid)
                        {
                            this.getBtn.GetComponent<CUIEventScript>().enabled = true;
                            this.getBtn.GetComponent<Button>().interactable = true;
                            this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText("gotoFinish");
                            this.getBtnTxt.color = Color.white;
                        }
                        else
                        {
                            this.getBtn.GetComponent<CUIEventScript>().enabled = false;
                            this.getBtn.GetComponent<Button>().interactable = false;
                            this.getBtnTxt.text = Singleton<CTextManager>.GetInstance().GetText((this.actvPhase.timeState != ActivityPhase.TimeState.Closed) ? "notInTime" : "outOfTime");
                            this.getBtnTxt.color = Color.gray;
                        }
                    }
                }
                int num = 0;
                for (int i = 0; i < this.cellList.Length; i++)
                {
                    GameObject obj2 = this.cellList[i];
                    CUseable itemUseable = this.actvPhase.GetUseable(i);
                    if (itemUseable != null)
                    {
                        obj2.CustomSetActive(true);
                        itemUseable.m_stackMulti = (int) this.actvPhase.MultipleTimes;
                        CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, obj2, itemUseable, true, false);
                        num++;
                    }
                    else
                    {
                        itemUseable = this.actvPhase.GetExtraUseable(i - num);
                        if (itemUseable != null)
                        {
                            obj2.CustomSetActive(true);
                            CUICommonSystem.SetItemCell(this.ownerWgt.view.form.formScript, obj2, itemUseable, true, false);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                    }
                    if (itemUseable != null)
                    {
                        GameObject obj3 = Utility.FindChild(obj2, "flag");
                        if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                        {
                            obj3.CustomSetActive(true);
                            Utility.GetComponetInChild<Text>(obj3, "Text").text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_Buy_Tab");
                        }
                        else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                        {
                            obj3.CustomSetActive(true);
                            Utility.GetComponetInChild<Text>(obj3, "Text").text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_Buy_Tab");
                        }
                        else
                        {
                            obj3.CustomSetActive(false);
                        }
                    }
                }
            }
        }
    }
}

