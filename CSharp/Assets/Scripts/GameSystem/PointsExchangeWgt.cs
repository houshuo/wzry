namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PointsExchangeWgt : ActivityWidget
    {
        private ListView<PointsExchangeElement> _elementList;
        private GameObject _elementTmpl;
        private const float SpacingY = 5f;

        public PointsExchangeWgt(GameObject node, ActivityView view) : base(node, view)
        {
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            this._elementList = new ListView<PointsExchangeElement>();
            this._elementTmpl = Utility.FindChild(node, "Template");
            float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
            for (int i = 0; i < phaseList.Count; i++)
            {
                GameObject uiItem = null;
                uiItem = (GameObject) UnityEngine.Object.Instantiate(this._elementTmpl);
                if (uiItem != null)
                {
                    uiItem.GetComponent<RectTransform>().sizeDelta = this._elementTmpl.GetComponent<RectTransform>().sizeDelta;
                    uiItem.transform.SetParent(this._elementTmpl.transform.parent);
                    uiItem.transform.localPosition = this._elementTmpl.transform.localPosition + new Vector3(0f, -(height + 5f) * i, 0f);
                    uiItem.transform.localScale = Vector3.one;
                    uiItem.transform.localRotation = Quaternion.identity;
                    this._elementList.Add(new PointsExchangeElement(phaseList[i] as PointsExchangePhase, uiItem, this, i));
                }
            }
            node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (height * phaseList.Count) + ((phaseList.Count - 1) * 5f));
            this._elementTmpl.CustomSetActive(false);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_PtExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            if (this._elementList != null)
            {
                for (int i = 0; i < this._elementList.Count; i++)
                {
                    if (this._elementList[i].uiItem != null)
                    {
                        UnityEngine.Object.Destroy(this._elementList[i].uiItem);
                    }
                }
                this._elementList = null;
            }
            this._elementTmpl = null;
        }

        private void OnClickExchange(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                int num = (int) uiEvent.m_eventParams.commonUInt32Param1;
                if ((num >= 0) && (num < this._elementList.Count))
                {
                    uint dwResItemID = this._elementList[num].phase.Config.dwResItemID;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) this._elementList[num].phase.Config.wResItemType, this._elementList[num].phase.Config.dwResItemID, 1);
                    string str = (useable != null) ? useable.m_name : string.Empty;
                    stUIEventParams par = new stUIEventParams {
                        commonUInt32Param1 = (uint) num
                    };
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), str), enUIEventID.Activity_PtExchangeConfirm, enUIEventID.None, par, false);
                }
            }
        }

        private void OnClickExchangeConfirm(CUIEvent uiEvent)
        {
            if (this._elementList != null)
            {
                int num = (int) uiEvent.m_eventParams.commonUInt32Param1;
                if ((num >= 0) && (num < this._elementList.Count))
                {
                    this._elementList[num].phase.DrawReward();
                }
            }
        }

        private void OnStateChange(Activity acty)
        {
            this.Validate();
            ActivityView view = base.view;
            if (view != null)
            {
                ListView<ActivityWidget> widgetList = view.WidgetList;
                for (int i = 0; i < widgetList.Count; i++)
                {
                    IntrodWidget widget = widgetList[i] as IntrodWidget;
                    if (widget != null)
                    {
                        widget.Validate();
                        break;
                    }
                }
            }
        }

        public override void Validate()
        {
            if (this._elementList != null)
            {
                for (int i = 0; i < this._elementList.Count; i++)
                {
                    this._elementList[i].Validate();
                }
            }
        }

        private class PointsExchangeElement
        {
            public int index;
            public PointsExchangeWgt owner;
            public PointsExchangePhase phase;
            public GameObject uiItem;

            public PointsExchangeElement(PointsExchangePhase phase, GameObject uiItem, PointsExchangeWgt owner, int index)
            {
                this.phase = phase;
                this.uiItem = uiItem;
                this.owner = owner;
                this.index = index;
                this.Validate();
            }

            public void Validate()
            {
                if ((this.phase != null) && (this.uiItem != null))
                {
                    this.uiItem.CustomSetActive(true);
                    ResDT_PointExchange config = this.phase.Config;
                    PointsExchangeActivity owner = this.phase.Owner as PointsExchangeActivity;
                    if ((owner != null) && (owner.PointsConfig != null))
                    {
                        ResWealPointExchange pointsConfig = owner.PointsConfig;
                        GameObject gameObject = this.uiItem.transform.FindChild("DuihuanBtn").gameObject;
                        gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) this.index;
                        uint maxExchangeCount = owner.GetMaxExchangeCount(this.index);
                        uint exchangeCount = owner.GetExchangeCount(this.index);
                        uint dwPointCnt = config.dwPointCnt;
                        uint jiFen = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().JiFen;
                        bool isEnable = (jiFen >= dwPointCnt) && ((maxExchangeCount == 0) || (exchangeCount < maxExchangeCount));
                        if (this.owner.view.activity.timeState != Activity.TimeState.Going)
                        {
                            CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), false, false, true);
                        }
                        else
                        {
                            CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), isEnable, isEnable, true);
                        }
                        CUseable itemUseable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianJuanJiFen, 1);
                        GameObject itemCell = this.uiItem.transform.FindChild("Panel/PointsCell").gameObject;
                        CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false);
                        CUseable useable2 = CUseableManager.CreateUseable((COM_ITEM_TYPE) config.wResItemType, config.dwResItemID, config.wResItemCnt);
                        GameObject obj4 = this.uiItem.transform.FindChild("Panel/GetItemCell").gameObject;
                        CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable2, true, false);
                        Text component = this.uiItem.transform.FindChild("Panel/PointsCell/ItemCount").gameObject.GetComponent<Text>();
                        if (jiFen < config.dwPointCnt)
                        {
                            component.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), jiFen, dwPointCnt);
                            CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), false, false, true);
                        }
                        else
                        {
                            component.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), jiFen, dwPointCnt);
                        }
                        GameObject obj5 = this.uiItem.transform.FindChild("ExchangeCount").gameObject;
                        if (maxExchangeCount > 0)
                        {
                            obj5.CustomSetActive(true);
                            obj5.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount);
                        }
                        else
                        {
                            obj5.CustomSetActive(false);
                        }
                    }
                }
            }
        }
    }
}

