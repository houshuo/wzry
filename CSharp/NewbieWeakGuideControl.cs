using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

internal class NewbieWeakGuideControl : Singleton<NewbieWeakGuideControl>
{
    private NewbieGuideWeakConf curToAddEffectConf;
    public NewbieWeakGuideMainLineConf curWeakMainLineConf;
    private NewbieWeakGuideImpl guideImpl;
    private uint m_CurStep;
    private ListView<NewbieGuideWeakConf> mConfList;
    public DictionaryView<uint, GameObject> mEffectCache;
    private ListView<NewbieGuideWeakConf> mToaddConfList;
    private float tryToAddEffectTime;
    private const float TYR_TO_ADD_EFFECT_TIME = 5f;

    private bool AddEffect(NewbieGuideWeakConf conf)
    {
        if (this.HasEffect(conf.dwID))
        {
            return false;
        }
        this.curToAddEffectConf = conf;
        this.mToaddConfList.Add(conf);
        this.tryToAddEffectTime = 0f;
        return true;
    }

    private void AddEffectProcess()
    {
        GameObject obj2 = null;
        if ((this.curToAddEffectConf != null) && (!this.mEffectCache.TryGetValue(this.curToAddEffectConf.dwID, out obj2) && this.guideImpl.AddEffect(this.curToAddEffectConf, this, out obj2)))
        {
            this.mToaddConfList.RemoveAt(0);
            if (obj2 != null)
            {
                this.TryToAddClickEvent(this.curToAddEffectConf.dwID, obj2);
                this.mEffectCache.Add(this.curToAddEffectConf.dwID, obj2);
            }
        }
    }

    private void CheckNext()
    {
        int count = this.mConfList.Count;
        if (this.m_CurStep <= count)
        {
            NewbieGuideWeakConf conf = this.mConfList[((int) this.m_CurStep) - 1];
            this.AddEffect(conf);
        }
        else
        {
            this.CompleteAll();
        }
    }

    private void checkTryToAddEffectTimeOut()
    {
        this.tryToAddEffectTime += Time.deltaTime;
        if (this.tryToAddEffectTime >= 5f)
        {
            this.mToaddConfList.Clear();
        }
    }

    private void clear()
    {
        this.curWeakMainLineConf = null;
        this.isGuiding = false;
        this.mToaddConfList.Clear();
    }

    private void ClickHandler(CUIEvent uiEvent)
    {
        uint weakGuideId = uiEvent.m_eventParams.weakGuideId;
        GameObject obj2 = null;
        this.mEffectCache.TryGetValue(uiEvent.m_eventParams.weakGuideId, out obj2);
        if (obj2 != null)
        {
            CUIEventScript componentInParent = obj2.GetComponentInParent<CUIEventScript>();
            if (componentInParent != null)
            {
                componentInParent.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(componentInParent.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
            }
            this.Complete(weakGuideId, 0);
        }
    }

    public void CloseGuideForm()
    {
        if (this.guideImpl != null)
        {
            this.guideImpl.CloseGuideForm();
        }
    }

    public void Complete(uint weakGuideId, uint nextStep = 0)
    {
        NewbieGuideWeakConf conf = this.mConfList[((int) this.m_CurStep) - 1];
        this.RemoveEffect(weakGuideId);
        if (conf.Param[2] > 0)
        {
            this.CompleteAll();
        }
        else
        {
            if (nextStep == 0)
            {
                this.m_CurStep++;
            }
            else
            {
                this.m_CurStep = nextStep;
            }
            this.CheckNext();
        }
    }

    private void CompleteAll()
    {
        if (this.curWeakMainLineConf.bOnlyOnce > 0)
        {
            MonoSingleton<NewbieGuideManager>.GetInstance().SetWeakGuideComplete(this.curWeakMainLineConf.dwID, true);
        }
        uint[] param = new uint[] { this.curWeakMainLineConf.dwID };
        MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.preNewBieWeakGuideComleteAll, param);
        this.RemoveAllEffect();
        this.clear();
    }

    public void ForceCompleteWeakGuide()
    {
        if (this.curWeakMainLineConf != null)
        {
            this.CompleteAll();
        }
    }

    private bool HasEffect(uint weakGuideId)
    {
        if ((this.mEffectCache != null) && this.mEffectCache.ContainsKey(weakGuideId))
        {
            if (this.mEffectCache[weakGuideId] != null)
            {
                return true;
            }
            this.mEffectCache.Remove(weakGuideId);
            this.guideImpl.ClearEffectText();
        }
        return false;
    }

    public override void Init()
    {
        base.Init();
        this.mEffectCache = new DictionaryView<uint, GameObject>();
        this.guideImpl = new NewbieWeakGuideImpl();
        this.mToaddConfList = new ListView<NewbieGuideWeakConf>();
        this.guideImpl.Init();
    }

    public void OpenGuideForm()
    {
        if (this.guideImpl != null)
        {
            this.guideImpl.OpenGuideForm();
        }
    }

    public void RemoveAllEffect()
    {
        List<uint> list = new List<uint>(this.mEffectCache.Keys);
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            uint weakGuideId = list[i];
            this.RemoveEffect(weakGuideId);
        }
    }

    private void RemoveEffect(uint weakGuideId)
    {
        GameObject obj2 = null;
        if (this.mEffectCache.TryGetValue(weakGuideId, out obj2))
        {
            this.mEffectCache.Remove(weakGuideId);
            if (obj2 != null)
            {
                UnityEngine.Object.Destroy(obj2);
                obj2 = null;
            }
            this.guideImpl.ClearEffectText();
        }
    }

    public bool TriggerWeakGuide(uint mainLineId, uint startIndex = 1)
    {
        if (this.isGuiding)
        {
            this.RemoveAllEffect();
        }
        this.clear();
        this.curWeakMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieWeakGuideMainLineConf(mainLineId);
        this.isGuiding = true;
        this.mConfList = Singleton<NewbieGuideDataManager>.GetInstance().GetWeakScriptList(mainLineId);
        this.m_CurStep = startIndex;
        this.OpenGuideForm();
        if ((this.mConfList == null) && (this.curWeakMainLineConf != null))
        {
            this.CompleteAll();
        }
        else
        {
            this.CheckNext();
        }
        return true;
    }

    private void TryToAddClickEvent(uint id, GameObject EffectObj)
    {
        if (EffectObj != null)
        {
            CUIEventScript componentInParent = EffectObj.GetComponentInParent<CUIEventScript>();
            if (componentInParent != null)
            {
                componentInParent.m_onClickEventParams.weakGuideId = id;
                componentInParent.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(componentInParent.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
            }
            CUIMiniEventScript script2 = EffectObj.GetComponentInParent<CUIMiniEventScript>();
            if (script2 != null)
            {
                script2.m_onClickEventParams.weakGuideId = id;
                script2.onClick = (CUIMiniEventScript.OnUIEventHandler) Delegate.Combine(script2.onClick, new CUIMiniEventScript.OnUIEventHandler(this.ClickHandler));
            }
        }
    }

    public override void UnInit()
    {
        this.RemoveAllEffect();
        this.CloseGuideForm();
        this.guideImpl.UnInit();
        this.guideImpl = null;
        this.mToaddConfList.Clear();
        this.mToaddConfList = null;
        base.UnInit();
    }

    public void Update()
    {
        if (this.guideImpl != null)
        {
            this.guideImpl.Update();
        }
        if ((this.mToaddConfList != null) && (this.mToaddConfList.Count > 0))
        {
            this.checkTryToAddEffectTimeOut();
            this.AddEffectProcess();
        }
    }

    public bool isGuiding { get; private set; }

    private string logTitle
    {
        get
        {
            return "[<color=cyan>新手弱引导</color>][<color=green>eason</color>]";
        }
    }
}

