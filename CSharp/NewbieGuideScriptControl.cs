using Assets.Scripts.UI;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewbieGuideScriptControl : MonoBehaviour
{
    public AddScriptDelegate addScriptDelegate;
    private NewbieGuideMainLineConf curMainLineConf;
    public const string FormGuideMaskPath = "UGUI/Form/System/Dialog/Form_GuideMask";
    private float m_timeOutTimer;
    private ListView<NewbieGuideScriptConf> mConfList;
    private NewbieGuideBaseScript mCurrentScript;
    private int mCurrentScriptIndex;
    private int mSavePoint;
    private const float TimeOut = 30f;

    public event NewbieGuideScriptControlDelegate CompleteEvent;

    public event NewbieGuideScriptControlDelegate SaveEvent;

    private NewbieGuideBaseScript AddScript(NewbieGuideScriptType type)
    {
        if (this.addScriptDelegate != null)
        {
            return this.addScriptDelegate(type, base.gameObject);
        }
        return null;
    }

    private void CheckNext()
    {
        if (this.currentScriptIndex < this.mConfList.Count)
        {
            if (!Singleton<NetworkModule>.GetInstance().lobbySvr.connected && (this.curMainLineConf.bIndependentNet != 1))
            {
                MonoSingleton<NewbieGuideManager>.instance.ForceCompleteNewbieGuide();
            }
            else
            {
                NewbieGuideScriptConf conf = this.mConfList[this.currentScriptIndex];
                this.mCurrentScript = this.AddScript((NewbieGuideScriptType) conf.wType);
                if (null != this.mCurrentScript)
                {
                    this.mCurrentScript.SetData(conf);
                    this.mCurrentScript.CompleteEvent += new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.ScriptCompleteHandler);
                    this.mCurrentScript.onCompleteAll += new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.CompleteAll);
                }
                else
                {
                    this.CompleteAll();
                }
            }
        }
        else
        {
            this.CompleteAll();
        }
        if ((this.mCurrentScript != null) && this.mCurrentScript.IsTimeOutSkip())
        {
            this.m_timeOutTimer = 30f;
        }
    }

    private void CheckSave()
    {
        if (this.SaveEvent != null)
        {
            this.SaveEvent();
        }
    }

    public bool CheckSavePoint()
    {
        if (0 >= this.mSavePoint)
        {
            return false;
        }
        return ((this.mCurrentScriptIndex + 1) >= this.mSavePoint);
    }

    public static void CloseGuideForm()
    {
        if (FormGuideMask != null)
        {
            FormGuideMask.transform.FindChild("GuideTextStatic").transform.gameObject.CustomSetActive(false);
            Singleton<CUIManager>.GetInstance().CloseForm(FormGuideMask);
            FormGuideMask = null;
        }
    }

    private void CompleteAll()
    {
        CloseGuideForm();
        this.curMainLineConf = null;
        this.currentMainLineId = 0;
        if (this.CompleteEvent != null)
        {
            this.CompleteEvent();
        }
    }

    private void DestroyCurrentScript()
    {
        if (null != this.mCurrentScript)
        {
            this.mCurrentScript.CompleteEvent -= new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.ScriptCompleteHandler);
            this.mCurrentScript.onCompleteAll -= new NewbieGuideBaseScript.NewbieGuideBaseScriptDelegate(this.CompleteAll);
            UnityEngine.Object.Destroy(this.mCurrentScript);
            this.mCurrentScript = null;
        }
    }

    public static void OpenGuideForm()
    {
        if (FormGuideMask == null)
        {
            FormGuideMask = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_GuideMask", true, true);
            if (FormGuideMask != null)
            {
                Transform transform = FormGuideMask.transform.FindChild("GuideTextStatic");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }
    }

    private void ScriptCheckSaveHandler()
    {
        this.CheckSave();
    }

    private void ScriptCompleteHandler()
    {
        this.ScriptCheckSaveHandler();
        this.DestroyCurrentScript();
        this.SetCurrentScriptIndex(this.currentScriptIndex + 1);
        this.CheckNext();
    }

    private void SetCurrentScriptIndex(int value)
    {
        this.mCurrentScriptIndex = value;
    }

    public void SetData(uint id, int startIndex)
    {
        this.currentMainLineId = id;
        this.startIndex = startIndex;
        this.curMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(this.currentMainLineId);
    }

    private void Start()
    {
        OpenGuideForm();
        this.mConfList = Singleton<NewbieGuideDataManager>.GetInstance().GetScriptList(this.currentMainLineId);
        if (this.mConfList == null)
        {
            this.CompleteAll();
        }
        else
        {
            NewbieGuideMainLineConf newbieGuideMainLineConf = Singleton<NewbieGuideDataManager>.GetInstance().GetNewbieGuideMainLineConf(this.currentMainLineId);
            if (newbieGuideMainLineConf != null)
            {
                this.mSavePoint = newbieGuideMainLineConf.iSavePoint;
                if (this.startIndex > 0)
                {
                    this.SetCurrentScriptIndex(this.startIndex - 1);
                }
                else
                {
                    this.SetCurrentScriptIndex(0);
                }
                this.CheckNext();
            }
        }
    }

    public void Stop()
    {
        if (this.mCurrentScript != null)
        {
            this.mCurrentScript.Stop();
        }
        this.DestroyCurrentScript();
        CloseGuideForm();
    }

    private void Update()
    {
        if (MonoSingleton<NewbieGuideManager>.GetInstance().bTimeOutSkip && (this.m_timeOutTimer > 0f))
        {
            this.m_timeOutTimer -= Time.deltaTime;
            if (this.m_timeOutTimer <= 0f)
            {
                this.m_timeOutTimer = 0f;
                MonoSingleton<NewbieGuideManager>.instance.ForceCompleteNewbieGuide();
            }
        }
    }

    public uint currentMainLineId { get; private set; }

    public NewbieGuideBaseScript currentScript
    {
        get
        {
            return this.mCurrentScript;
        }
    }

    public int currentScriptIndex
    {
        get
        {
            return this.mCurrentScriptIndex;
        }
    }

    public static CUIFormScript FormGuideMask
    {
        [CompilerGenerated]
        get
        {
            return <FormGuideMask>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            <FormGuideMask>k__BackingField = value;
        }
    }

    private string logTitle
    {
        get
        {
            return "[<color=cyan>新手引导</color>][<color=green>eason</color>]";
        }
    }

    public int savePoint
    {
        get
        {
            return this.mSavePoint;
        }
    }

    public int startIndex { get; private set; }

    public delegate NewbieGuideBaseScript AddScriptDelegate(NewbieGuideScriptType type, GameObject gameObject);

    public delegate void NewbieGuideScriptControlDelegate();
}

