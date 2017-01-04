using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AkUnityEventHandler : MonoBehaviour
{
    public const int AWAKE_TRIGGER_ID = 0x449d8dae;
    public const int DESTROY_TRIGGER_ID = -358577003;
    private bool didDestroy;
    public const int MAX_NB_TRIGGERS = 0x20;
    public const int START_TRIGGER_ID = 0x4c66e1f7;
    public List<int> triggerList = new List<int> { 0x4c66e1f7 };
    public static Dictionary<uint, string> triggerTypes = AkTriggerBase.GetAllDerivedTypes();
    public bool useOtherObject;

    protected AkUnityEventHandler()
    {
    }

    protected virtual void Awake()
    {
        this.RegisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
        if (this.triggerList.Contains(0x449d8dae))
        {
            this.HandleEvent(null);
        }
    }

    public void DoDestroy()
    {
        this.UnregisterTriggers(this.triggerList, new AkTriggerBase.Trigger(this.HandleEvent));
        if (this.triggerList.Contains(-358577003))
        {
            this.HandleEvent(null);
        }
        this.didDestroy = true;
    }

    public abstract void HandleEvent(GameObject in_gameObject);
    protected virtual void OnDestroy()
    {
        if (!this.didDestroy)
        {
            this.DoDestroy();
        }
    }

    protected void RegisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
    {
        foreach (uint num in in_triggerList)
        {
            string str = string.Empty;
            if (((triggerTypes.TryGetValue(num, out str) && (str != "Awake")) && (str != "Start")) && (str != "Destroy"))
            {
                AkTriggerBase component = (AkTriggerBase) base.GetComponent(UtilityPlugin.GetType(str));
                if (component == null)
                {
                    component = (AkTriggerBase) base.gameObject.AddComponent(UtilityPlugin.GetType(str));
                }
                component.triggerDelegate = (AkTriggerBase.Trigger) Delegate.Combine(component.triggerDelegate, in_delegate);
            }
        }
    }

    protected virtual void Start()
    {
        if (this.triggerList.Contains(0x4c66e1f7))
        {
            this.HandleEvent(null);
        }
    }

    protected void UnregisterTriggers(List<int> in_triggerList, AkTriggerBase.Trigger in_delegate)
    {
        foreach (uint num in in_triggerList)
        {
            string str = string.Empty;
            if (((triggerTypes.TryGetValue(num, out str) && (str != "Awake")) && (str != "Start")) && (str != "Destroy"))
            {
                AkTriggerBase component = (AkTriggerBase) base.GetComponent(UtilityPlugin.GetType(str));
                if (component != null)
                {
                    component.triggerDelegate = (AkTriggerBase.Trigger) Delegate.Remove(component.triggerDelegate, in_delegate);
                    if (component.triggerDelegate == null)
                    {
                        UnityEngine.Object.Destroy(component);
                    }
                }
            }
        }
    }
}

