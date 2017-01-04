namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CampaignForm : ActivityForm
    {
        private ListView<ActivityMenuItem> _actvMenuList;
        private int _initStep;
        private int _initTimer;
        private int _selectedIndex;
        private GameObject _title;
        private Image _titleImage;
        private Text _titleText;
        private CUIFormScript _uiForm;
        private CUIListScript _uiListMenu;
        private CampaignFormView _view;
        private ScrollRect _viewScroll;
        [CompilerGenerated]
        private static Comparison<Activity> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cacheF;
        private string[] m_strTitleList;
        private GameObject[] m_TitleListObj;
        private CUIListScript m_TitleMenuList;
        public static string s_formPath = (CUIUtility.s_Form_Activity_Dir + "Form_Activity.prefab");

        public CampaignForm(ActivitySys sys) : base(sys)
        {
            this.m_strTitleList = new string[] { "精彩活动", "游戏公告" };
            this.m_TitleListObj = new GameObject[2];
            this._uiForm = null;
        }

        private void BuildMenuList()
        {
            if (this._actvMenuList != null)
            {
                for (int j = 0; j < this._actvMenuList.Count; j++)
                {
                    this._actvMenuList[j].Clear();
                }
                this._actvMenuList = null;
            }
            this._actvMenuList = new ListView<ActivityMenuItem>();
            this._selectedIndex = -1;
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = actv => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY;
            }
            ListView<Activity> activityList = base.Sys.GetActivityList(<>f__am$cacheF);
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = delegate (Activity l, Activity r) {
                    bool readyForGet = l.ReadyForGet;
                    bool flag2 = r.ReadyForGet;
                    if (readyForGet != flag2)
                    {
                        return !readyForGet ? 1 : -1;
                    }
                    bool completed = l.Completed;
                    bool flag4 = r.Completed;
                    if (completed != flag4)
                    {
                        return !completed ? -1 : 1;
                    }
                    if (l.FlagType != r.FlagType)
                    {
                        return (int) (r.FlagType - l.FlagType);
                    }
                    return (int) (l.Sequence - r.Sequence);
                };
            }
            activityList.Sort(<>f__am$cache10);
            this._uiListMenu.SetElementAmount(activityList.Count);
            for (int i = 0; i < activityList.Count; i++)
            {
                Activity activity = activityList[i];
                ActivityMenuItem item = new ActivityMenuItem(this._uiListMenu.GetElemenet(i).gameObject, activity);
                this._actvMenuList.Add(item);
            }
        }

        public override void Close()
        {
            if (this._actvMenuList != null)
            {
                for (int i = 0; i < this._actvMenuList.Count; i++)
                {
                    this._actvMenuList[i].Clear();
                }
                this._actvMenuList = null;
            }
            if (this._view != null)
            {
                this._view.Clear();
                this._view = null;
            }
            if (null != this._uiForm)
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
                CUIFormScript formScript = this._uiForm;
                this._uiForm = null;
                this._uiListMenu = null;
                this.m_TitleMenuList = null;
                this.m_TitleListObj = null;
                Singleton<CUIManager>.GetInstance().CloseForm(formScript);
                MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeLoseTipsInfo();
                MonoSingleton<PandroaSys>.GetInstance().ShowPopNews();
            }
            MonoSingleton<IDIPSys>.GetInstance().OnCloseIDIPForm(null);
            Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
        }

        private GameObject GetActivityRedObj()
        {
            if ((this._uiForm != null) && ((this.m_TitleListObj != null) && (this.m_TitleListObj.Length >= 2)))
            {
                return this.m_TitleListObj[0];
            }
            return null;
        }

        public Image GetDynamicImage(DynamicAssets index)
        {
            return this._uiForm.GetWidget((int) index).GetComponent<Image>();
        }

        public GameObject GetIDIPRedObj()
        {
            if ((this._uiForm != null) && ((this.m_TitleListObj != null) && (this.m_TitleListObj.Length >= 2)))
            {
                return this.m_TitleListObj[1];
            }
            return null;
        }

        private void onInitTimer(int seq)
        {
            int num2;
            int num3;
            this._initStep = num3 = this._initStep + 1;
            switch (num3)
            {
                case 1:
                    this._title = Utility.FindChild(this._uiForm.gameObject, "Panel/Title");
                    this._titleText = Utility.GetComponetInChild<Text>(this._title, "Text");
                    this._titleImage = Utility.GetComponetInChild<Image>(this._title, "Image");
                    this._uiListMenu = Utility.GetComponetInChild<CUIListScript>(this._uiForm.gameObject, "Panel/Panle_Activity/Menu/List");
                    this._viewScroll = Utility.GetComponetInChild<ScrollRect>(this._uiForm.gameObject, "Panel/Panle_Activity/ScrollRect");
                    this._view = new CampaignFormView(Utility.FindChild(this._uiForm.gameObject, "Panel/Panle_Activity/ScrollRect/Content"), this, null);
                    return;

                case 2:
                    this.m_TitleMenuList = Utility.GetComponetInChild<CUIListScript>(this._uiForm.gameObject, "Panel/TitleMenu/List");
                    this.m_TitleMenuList.SetElementAmount(this.m_strTitleList.Length);
                    this.m_TitleListObj = new GameObject[this.m_strTitleList.Length];
                    for (int i = 0; i < this.m_strTitleList.Length; i++)
                    {
                        CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(i);
                        Text componetInChild = Utility.GetComponetInChild<Text>(elemenet.gameObject, "Text");
                        if (componetInChild != null)
                        {
                            componetInChild.text = this.m_strTitleList[i];
                        }
                        this.m_TitleListObj[i] = elemenet.gameObject;
                    }
                    this.m_TitleMenuList.SelectElement(0, true);
                    return;

                case 3:
                    this.BuildMenuList();
                    return;

                case 4:
                {
                    num2 = -1;
                    bool flag = true;
                    while (++num2 < this._actvMenuList.Count)
                    {
                        if ((flag && this._actvMenuList[num2].activity.ReadyForGet) || (!flag && this._actvMenuList[num2].activity.ReadyForDot))
                        {
                            break;
                        }
                        if (flag && ((num2 + 1) == this._actvMenuList.Count))
                        {
                            num2 = -1;
                            flag = false;
                        }
                    }
                    break;
                }
                default:
                    return;
            }
            if (num2 >= this._actvMenuList.Count)
            {
                num2 = 0;
            }
            this._uiListMenu.SelectElement(num2, true);
            this.SelectMenuItem(num2);
            Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
            this.UpdateTitelRedDot();
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
            this._initStep = 0;
            Singleton<ActivitySys>.GetInstance().OnCampaignFormOpened();
        }

        private void OnSelectActivity(CUIEvent uiEvent)
        {
            this.SelectMenuItem(uiEvent.m_srcWidgetIndexInBelongedList);
            CUICommonSystem.CloseUseableTips();
        }

        private void OnSelectTitleMenu(CUIEvent uiEvent)
        {
            if (this._uiForm != null)
            {
                if (uiEvent.m_srcWidgetIndexInBelongedList == 0)
                {
                    Transform transform = this._uiForm.gameObject.transform.Find("Panel/Panle_Activity");
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(true);
                    }
                    Transform transform2 = this._uiForm.gameObject.transform.Find("Panel/Panle_IDIP");
                    if (transform2 != null)
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                }
                else if (uiEvent.m_srcWidgetIndexInBelongedList == 1)
                {
                    Transform transform3 = this._uiForm.gameObject.transform.Find("Panel/Panle_Activity");
                    if (transform3 != null)
                    {
                        transform3.gameObject.CustomSetActive(false);
                    }
                    Transform transform4 = this._uiForm.gameObject.transform.Find("Panel/Panle_IDIP");
                    if (transform4 != null)
                    {
                        transform4.gameObject.CustomSetActive(true);
                    }
                    MonoSingleton<IDIPSys>.GetInstance().OnOpenIDIPForm(this._uiForm);
                }
            }
        }

        private void OnValidateActivityRedSpot()
        {
            this.UpdateTitelRedDot();
        }

        public override void Open()
        {
            if (null == this._uiForm)
            {
                this._uiForm = Singleton<CUIManager>.GetInstance().OpenForm(s_formPath, false, true);
                if (null != this._uiForm)
                {
                    this._initTimer = Singleton<CTimerManager>.GetInstance().AddTimer(80, 4, new CTimer.OnTimeUpHandler(this.onInitTimer));
                    this._initStep = 0;
                    this.PandroaUpdateBtn();
                }
            }
        }

        public void PandroaUpdateBtn()
        {
            if (this._uiForm != null)
            {
                MonoSingleton<PandroaSys>.GetInstance().ShowActiveActBoxBtn(this._uiForm);
            }
        }

        private void SelectMenuItem(int index)
        {
            if ((index < 0) || (index >= this._actvMenuList.Count))
            {
                this._titleImage.gameObject.CustomSetActive(false);
                this._titleText.gameObject.CustomSetActive(true);
                this._titleText.text = Singleton<CTextManager>.GetInstance().GetText("activityEmptyTitle");
                this._view.SetActivity(null);
            }
            else if (index != this._selectedIndex)
            {
                this._selectedIndex = index;
                ActivityMenuItem item = this._actvMenuList[this._selectedIndex];
                string title = item.activity.Title;
                if (string.IsNullOrEmpty(title))
                {
                    this._title.CustomSetActive(false);
                }
                else
                {
                    this._title.CustomSetActive(true);
                    if (item.activity.IsImageTitle)
                    {
                        this._titleText.gameObject.CustomSetActive(false);
                        this._titleImage.gameObject.CustomSetActive(true);
                        this._titleImage.SetSprite(CUIUtility.GetSpritePrefeb(ActivitySys.SpriteRootDir + title, false, false));
                        this._titleImage.SetNativeSize();
                    }
                    else
                    {
                        this._titleImage.gameObject.CustomSetActive(false);
                        this._titleText.gameObject.CustomSetActive(true);
                        this._titleText.text = title;
                    }
                }
                this._view.SetActivity(item.activity);
                this._viewScroll.verticalNormalizedPosition = 1f;
                this.Update();
                item.activity.Visited = true;
            }
        }

        public override void Update()
        {
            if (((this._view != null) && (this._uiForm != null)) && ((this._uiForm.gameObject != null) && (this._initTimer == 0)))
            {
                this._view.Update();
            }
        }

        public void UpdateTitelRedDot()
        {
            if ((this._uiForm != null) && (this.m_TitleMenuList != null))
            {
                CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(0);
                if (elemenet != null)
                {
                    if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
                    {
                        uint reveivableRedDot = Singleton<ActivitySys>.GetInstance().GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY);
                        CUICommonSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, (int) reveivableRedDot);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(elemenet.gameObject);
                    }
                }
            }
        }

        public override CUIFormScript formScript
        {
            get
            {
                return this._uiForm;
            }
        }

        public class ActivityMenuItem
        {
            public Activity activity;
            public Image flag;
            public Text flagText;
            public Image hotspot;
            public Text name;
            public GameObject root;

            public ActivityMenuItem(GameObject node, Activity actv)
            {
                this.root = node;
                this.activity = actv;
                this.name = Utility.GetComponetInChild<Text>(node, "Name");
                this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
                this.flagText = Utility.GetComponetInChild<Text>(node, "Flag/Text");
                this.hotspot = Utility.GetComponetInChild<Image>(node, "Hotspot");
                this.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
                this.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChange);
                this.Validate();
            }

            public void Clear()
            {
                this.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
                this.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            }

            private void OnStateChange(Activity actv)
            {
                DebugHelper.Assert(this.hotspot != null, "hotspot != null");
                if (this.hotspot != null)
                {
                    this.hotspot.gameObject.SetActive(this.activity.ReadyForDot);
                }
            }

            public void Validate()
            {
                this.name.text = this.activity.Name;
                RES_WEAL_COLORBAR_TYPE flagType = this.activity.FlagType;
                if (flagType != RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NULL)
                {
                    this.flag.gameObject.CustomSetActive(true);
                    string key = flagType.ToString();
                    this.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/" + key, false, false));
                    this.flagText.text = Singleton<CTextManager>.GetInstance().GetText(key);
                }
                else
                {
                    this.flag.gameObject.CustomSetActive(false);
                }
                this.hotspot.gameObject.CustomSetActive(this.activity.ReadyForDot);
            }
        }

        public enum DynamicAssets
        {
            ButtonBlueImage,
            ButtonYellowImage
        }
    }
}

