namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class SevenDayCheckSystem : Singleton<SevenDayCheckSystem>
    {
        private CheckInPhase _availablePhase;
        private CheckInActivity _curActivity;
        private CUIFormScript _form;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cache6;
        public readonly string FormName = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/Form_SevenDayCheck.prefab");
        public bool IsShowingLoginOpen;

        protected void ActivityEvent(Activity acty)
        {
            this.UpdateCheckView();
        }

        internal void Clear()
        {
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
        }

        protected void OnCloseSevenDayCheckForm(CUIEvent uiEvent)
        {
            if (this._curActivity != null)
            {
                this._curActivity.OnMaskStateChange -= new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                this._curActivity.OnTimeStateChange -= new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                this._curActivity = null;
            }
            if (this._form != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(this.FormName);
                this._form = null;
            }
        }

        protected void OnLoginOpen(CUIEvent uiEvent)
        {
            if (this._form == null)
            {
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = actv => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_WEAL;
                }
                ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cache5);
                if ((activityList != null) && (activityList.Count > 0))
                {
                    this._curActivity = (CheckInActivity) activityList[0];
                    ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                    bool flag = false;
                    for (int i = 0; i < phaseList.Count; i++)
                    {
                        if (phaseList[i].ReadyForGet)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        this._curActivity.OnMaskStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._curActivity.OnTimeStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                        this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
                        this.UpdateCheckView();
                    }
                    else
                    {
                        this._curActivity = null;
                    }
                }
            }
        }

        protected void OnOpenSevenDayCheckForm(CUIEvent uiEvent)
        {
            if (this._form == null)
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = actv => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_WEAL;
                }
                ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cache6);
                if ((activityList != null) && (activityList.Count > 0))
                {
                    this._form = Singleton<CUIManager>.GetInstance().OpenForm(this.FormName, false, true);
                    this._curActivity = (CheckInActivity) activityList[0];
                    this._curActivity.OnMaskStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                    this._curActivity.OnTimeStateChange += new Assets.Scripts.GameSystem.Activity.ActivityEvent(this.ActivityEvent);
                    this.UpdateCheckView();
                }
            }
        }

        protected void OnRequeset(CUIEvent uiEvent)
        {
            if (((this._form != null) && (this._curActivity != null)) && (this._availablePhase != null))
            {
                this._availablePhase.DrawReward();
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_CloseForm);
            }
        }

        private void SetItem(CUseable usable, GameObject uiNode, bool received, bool ready, uint vipLv)
        {
            SevenDayCheckHelper component = uiNode.GetComponent<SevenDayCheckHelper>();
            CUIUtility.SetImageSprite(component.Icon.GetComponent<Image>(), usable.GetIconPath(), this._form, true, false, false);
            component.ItemName.GetComponent<Text>().text = usable.m_name;
            if (vipLv > 0)
            {
                component.NobeRoot.CustomSetActive(true);
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.Nobe.GetComponent<Image>(), (int) vipLv, false);
            }
            else
            {
                component.NobeRoot.CustomSetActive(false);
            }
            if ((((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))) || ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExChangeCoupons(usable.m_baseID)))
            {
                component.IconBg.CustomSetActive(true);
            }
            else
            {
                component.IconBg.CustomSetActive(false);
            }
            Transform transform = component.Tiyan.transform;
            if (transform != null)
            {
                if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(usable.m_baseID))
                {
                    transform.gameObject.CustomSetActive(true);
                    transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false));
                }
                else if ((usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(usable.m_baseID))
                {
                    transform.gameObject.CustomSetActive(true);
                    transform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false));
                }
                else
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
            Transform transform2 = component.ItemNumText.transform;
            if (transform2 != null)
            {
                Text target = transform2.GetComponent<Text>();
                if (usable.m_stackCount < 0x2710)
                {
                    target.text = usable.m_stackCount.ToString();
                }
                else
                {
                    target.text = (usable.m_stackCount / 0x2710) + "万";
                }
                CUICommonSystem.AppendMultipleText(target, usable.m_stackMulti);
                if (usable.m_stackCount <= 1)
                {
                    target.gameObject.CustomSetActive(false);
                    component.ItemNum.CustomSetActive(false);
                }
                else
                {
                    component.ItemNum.CustomSetActive(true);
                    component.ItemNumText.CustomSetActive(true);
                }
                if (usable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    if (((CSymbolItem) usable).IsGuildSymbol())
                    {
                        target.text = string.Empty;
                    }
                    else
                    {
                        target.text = usable.GetSalableCount().ToString();
                    }
                }
            }
            if (received)
            {
                component.GrayMask.CustomSetActive(true);
            }
            else
            {
                component.GrayMask.CustomSetActive(false);
            }
            if (ready)
            {
                component.Effect.CustomSetActive(true);
            }
            else
            {
                component.Effect.CustomSetActive(false);
            }
            CUIEventScript script = uiNode.GetComponent<CUIEventScript>();
            stUIEventParams eventParams = new stUIEventParams {
                iconUseable = usable
            };
            script.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
            script.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            script.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
            script.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSevenDayCheckForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_Request, new CUIEventManager.OnUIEventHandler(this.OnRequeset));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SevenCheck_LoginOpen, new CUIEventManager.OnUIEventHandler(this.OnLoginOpen));
        }

        private void UpdateCheckView()
        {
            if (this._curActivity != null)
            {
                ListView<ActivityPhase> phaseList = this._curActivity.PhaseList;
                Transform transform = this._form.gameObject.transform.FindChild("Panel/ItemContainer");
                bool isEnable = false;
                for (int i = 0; i < phaseList.Count; i++)
                {
                    CheckInPhase phase = phaseList[i] as CheckInPhase;
                    bool marked = phase.Marked;
                    bool readyForGet = phase.ReadyForGet;
                    if (readyForGet)
                    {
                        this._availablePhase = phase;
                    }
                    uint gameVipDoubleLv = phase.GetGameVipDoubleLv();
                    CUseable usable = phase.GetUseable(0);
                    if (usable != null)
                    {
                        GameObject gameObject = transform.FindChild(string.Format("itemCell{0}", i + 1)).gameObject;
                        if (gameObject != null)
                        {
                            this.SetItem(usable, gameObject, marked, readyForGet, gameVipDoubleLv);
                        }
                    }
                    if (!isEnable && readyForGet)
                    {
                        isEnable = true;
                    }
                }
                CUICommonSystem.SetButtonEnable(this._form.gameObject.transform.FindChild("Panel/BtnCheck").GetComponent<Button>(), isEnable, isEnable, true);
                Transform transform3 = this._form.gameObject.transform.FindChild("Panel/MeinvPic");
                MonoSingleton<BannerImageSys>.GetInstance().TrySetCheckInImage(transform3.gameObject.GetComponent<Image>());
                this._form.gameObject.transform.FindChild("Panel/Title/Text").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Title");
                this._form.gameObject.transform.FindChild("Panel/Desc").gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Desc");
                DateTime time = Utility.ToUtcTime2Local(this._curActivity.StartTime);
                DateTime time2 = Utility.ToUtcTime2Local(this._curActivity.CloseTime);
                string str = string.Format("{0}.{1}.{2}", time.Year, time.Month, time.Day);
                string str2 = string.Format("{0}.{1}.{2}", time2.Year, time2.Month, time2.Day);
                string[] args = new string[] { str, str2 };
                string text = Singleton<CTextManager>.instance.GetText("SevenCheckIn_Date", args);
                this._form.gameObject.transform.FindChild("Panel/Date").gameObject.GetComponent<Text>().text = text;
            }
        }
    }
}

