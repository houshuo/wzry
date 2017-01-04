using Assets.Scripts.UI;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CTaskShow : MonoBehaviour
{
    private Action _action;
    private bool bFinish;
    public bool bForward = true;
    public bool bValid;
    private CanvasGroup el_0;
    public CUIFormScript formScript;
    private float step = 0.03f;
    public uint taskid;
    public Type type;

    private void Awake()
    {
        this.el_0 = base.gameObject.GetComponent<CanvasGroup>();
    }

    private void decrease()
    {
        this.el_0.alpha -= this.step;
    }

    private void Increase()
    {
        this.el_0.alpha += this.step;
    }

    public void Play(Type type, CUIFormScript formScript, uint newTaskid = 0)
    {
        this.bValid = true;
        this.type = type;
        this.formScript = formScript;
        this.taskid = newTaskid;
        if (type == Type.Hide_ShowNew)
        {
            this.bFinish = false;
            this.el_0.alpha = 1f;
        }
        else if (type == Type.Show)
        {
            this.el_0.alpha = 0f;
        }
        else if (type == Type.Hide)
        {
            this.el_0.alpha = 1f;
        }
    }

    public void PlayParticle(bool bShowEffect)
    {
        string parPath = bShowEffect ? "UGUI/Particle/UI_renwu_effect_01/UI_renwu_effect_01" : "UGUI/Particle/UI_renwu_effect_02/UI_renwu_effect_02";
        Singleton<CUIParticleSystem>.instance.AddParticle(parPath, 1.5f, base.gameObject, this.formScript);
        string eventName = !bShowEffect ? "UI_renwu_xiaoshi" : "UI_renwu_shuaxin";
        Singleton<CSoundManager>.instance.PostEvent(eventName, null);
    }

    public void Reset()
    {
        this.el_0.alpha = !this.bForward ? ((float) 0) : ((float) 1);
    }

    private void Start()
    {
        this.el_0 = base.gameObject.GetComponent<CanvasGroup>();
        this.Reset();
    }

    public void Stop()
    {
        this.step = 0.03f;
        this.bValid = false;
        this._action = null;
    }

    private void Update()
    {
        if (this.bValid)
        {
            if (this.type == Type.Hide_ShowNew)
            {
                if (this.el_0.alpha >= 1f)
                {
                    if (this.bFinish)
                    {
                        this.Stop();
                    }
                    else
                    {
                        this.PlayParticle(false);
                        this._action = new Action(this.decrease);
                    }
                }
                if (this.el_0.alpha <= 0f)
                {
                    if ((this.taskid > 0) && (this.formScript != null))
                    {
                        this.step = 0.06f;
                    }
                    this.PlayParticle(true);
                    this.bFinish = true;
                    this._action = new Action(this.Increase);
                }
            }
            else if (this.type == Type.Show)
            {
                if (this.el_0.alpha <= 0f)
                {
                    this._action = new Action(this.Increase);
                }
                if (this.el_0.alpha >= 1f)
                {
                    this.Stop();
                }
            }
            else if (this.type == Type.Hide)
            {
                if (this.el_0.alpha >= 1f)
                {
                    this._action = new Action(this.decrease);
                }
                if (this.el_0.alpha <= 0f)
                {
                    this.Stop();
                }
            }
        }
        if (this.bValid && (this._action != null))
        {
            this._action();
        }
    }

    public enum Type
    {
        None,
        Hide,
        Show,
        Hide_ShowNew
    }
}

