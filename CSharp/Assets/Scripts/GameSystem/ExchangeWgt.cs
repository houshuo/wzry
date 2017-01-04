namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ExchangeWgt : ActivityWidget
    {
        private ListView<ExchangeElement> _elementList;
        private GameObject _elementTmpl;
        private const float SpacingY = 5f;

        public ExchangeWgt(GameObject node, ActivityView view) : base(node, view)
        {
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            this._elementList = new ListView<ExchangeElement>();
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
                    this._elementList.Add(new ExchangeElement(phaseList[(phaseList.Count - i) - 1] as ExchangePhase, uiItem, this, i));
                }
            }
            node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (height * phaseList.Count) + ((phaseList.Count - 1) * 5f));
            this._elementTmpl.CustomSetActive(false);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Exchange, new CUIEventManager.OnUIEventHandler(this.OnClickExchange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ExchangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnClickExchangeConfirm));
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
                    ResDT_Item_Info stResItemInfo = this._elementList[num].phase.Config.stResItemInfo;
                    CUseable useable = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, 1);
                    string str = (useable != null) ? useable.m_name : string.Empty;
                    stUIEventParams par = new stUIEventParams {
                        commonUInt32Param1 = (uint) num
                    };
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("confirmExchange"), str), enUIEventID.Activity_ExchangeConfirm, enUIEventID.None, par, false);
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

        private class ExchangeElement
        {
            public int index;
            public ExchangeWgt owner;
            public ExchangePhase phase;
            public GameObject uiItem;

            public ExchangeElement(ExchangePhase phase, GameObject uiItem, ExchangeWgt owner, int index)
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
                    ResDT_Item_Info info = null;
                    ResDT_Item_Info info2 = null;
                    ResDT_Item_Info stResItemInfo = null;
                    stResItemInfo = this.phase.Config.stResItemInfo;
                    if (this.phase.Config.bColItemCnt > 0)
                    {
                        info = this.phase.Config.astColItemInfo[0];
                    }
                    if (this.phase.Config.bColItemCnt > 1)
                    {
                        info2 = this.phase.Config.astColItemInfo[1];
                    }
                    CUseableContainer useableContainer = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if (useableContainer != null)
                    {
                        int num = (info != null) ? useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info.wItemType, info.dwItemID) : 0;
                        int num2 = (info2 != null) ? useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID) : 0;
                        if (stResItemInfo != null)
                        {
                            GameObject gameObject = this.uiItem.transform.FindChild("DuihuanBtn").gameObject;
                            gameObject.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) this.index;
                            bool isEnable = this.owner.view.activity.timeState == Activity.TimeState.Going;
                            CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), isEnable, isEnable, true);
                            if (info != null)
                            {
                                CUseable useable2 = CUseableManager.CreateUseable((COM_ITEM_TYPE) info.wItemType, info.dwItemID, 1);
                                GameObject obj3 = this.uiItem.transform.FindChild("Panel/ItemCell1").gameObject;
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj3, useable2, true, false);
                                int useableStackCount = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info.wItemType, info.dwItemID);
                                ushort wItemCnt = info.wItemCnt;
                                Text component = this.uiItem.transform.FindChild("Panel/ItemCell1/ItemCount").gameObject.GetComponent<Text>();
                                if (useableStackCount < wItemCnt)
                                {
                                    component.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), useableStackCount, wItemCnt);
                                    CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), false, false, true);
                                }
                                else
                                {
                                    component.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), useableStackCount, wItemCnt);
                                }
                            }
                            if (info2 != null)
                            {
                                CUseable useable3 = CUseableManager.CreateUseable((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID, 1);
                                GameObject obj4 = this.uiItem.transform.FindChild("Panel/ItemCell2").gameObject;
                                obj4.CustomSetActive(true);
                                CUICommonSystem.SetItemCell(this.owner.view.form.formScript, obj4, useable3, true, false);
                                int num5 = useableContainer.GetUseableStackCount((COM_ITEM_TYPE) info2.wItemType, info2.dwItemID);
                                ushort num6 = info2.wItemCnt;
                                Text text2 = this.uiItem.transform.FindChild("Panel/ItemCell2/ItemCount").gameObject.GetComponent<Text>();
                                if (num5 < num6)
                                {
                                    text2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemNotEnoughCount"), num5, num6);
                                    CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), false, false, true);
                                }
                                else
                                {
                                    text2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_ItemCount"), num5, num6);
                                }
                            }
                            else
                            {
                                this.uiItem.transform.FindChild("Panel/ItemCell2").gameObject.CustomSetActive(false);
                                this.uiItem.transform.FindChild("Panel/Add").gameObject.CustomSetActive(false);
                            }
                            CUseable itemUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE) stResItemInfo.wItemType, stResItemInfo.dwItemID, stResItemInfo.wItemCnt);
                            GameObject itemCell = this.uiItem.transform.FindChild("Panel/GetItemCell").gameObject;
                            CUICommonSystem.SetItemCell(this.owner.view.form.formScript, itemCell, itemUseable, true, false);
                            ExchangeActivity activity = this.owner.view.activity as ExchangeActivity;
                            if (activity != null)
                            {
                                GameObject obj8 = this.uiItem.transform.FindChild("ExchangeCount").gameObject;
                                uint maxExchangeCount = activity.GetMaxExchangeCount(this.phase.Config.bIdx);
                                uint exchangeCount = activity.GetExchangeCount(this.phase.Config.bIdx);
                                if (maxExchangeCount > 0)
                                {
                                    obj8.CustomSetActive(true);
                                    obj8.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Exchange_TimeLimit"), exchangeCount, maxExchangeCount);
                                    if (exchangeCount >= maxExchangeCount)
                                    {
                                        CUICommonSystem.SetButtonEnable(gameObject.GetComponent<Button>(), false, false, true);
                                    }
                                }
                                else
                                {
                                    obj8.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

