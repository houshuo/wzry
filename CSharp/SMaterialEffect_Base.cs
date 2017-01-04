using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class SMaterialEffect_Base : PooledClassObject
{
    public string disableKeyword;
    public string enableKeyword;
    public MaterialHurtEffect owner;
    public int playingId;
    public static int s_playingId;
    public int type;

    protected SMaterialEffect_Base()
    {
    }

    public void AllocId()
    {
        this.playingId = ++s_playingId;
    }

    public virtual void OnMeshChanged(ListView<Material> oldMats, ListView<Material> newMats)
    {
        setMatsKeyword(oldMats, this.enableKeyword, this.disableKeyword);
        setMatsKeyword(newMats, this.disableKeyword, this.enableKeyword);
    }

    public virtual void Play()
    {
        setMatsKeyword(this.owner.mats, this.disableKeyword, this.enableKeyword);
    }

    public static void setMatsKeyword(ListView<Material> mats, string disableKW, string enabledKW)
    {
        if (mats != null)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                Material material = mats[i];
                material.DisableKeyword(disableKW);
                material.EnableKeyword(enabledKW);
            }
        }
    }

    public virtual void Stop()
    {
        setMatsKeyword(this.owner.mats, this.enableKeyword, this.disableKeyword);
    }

    public abstract bool Update();
    public static bool UpdateFadeState(out float factor, ref FadeState fadeState, ref STimer fadeTime, float fadeInterval)
    {
        bool flag = false;
        factor = 1f;
        if (fadeState == FadeState.FadeIn)
        {
            float num = fadeTime.Update();
            if (num >= fadeInterval)
            {
                factor = 1f;
                fadeState = FadeState.Normal;
            }
            else
            {
                factor = num / fadeInterval;
            }
            return true;
        }
        if (fadeState != FadeState.FadeOut)
        {
            return flag;
        }
        float num2 = fadeTime.Update();
        if (num2 >= fadeInterval)
        {
            factor = 0f;
            fadeState = FadeState.Stopped;
        }
        else
        {
            factor = 1f - (num2 / fadeInterval);
        }
        return true;
    }

    public enum FadeState
    {
        FadeIn,
        Normal,
        FadeOut,
        Stopped
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STimer
    {
        public long startFrameTick;
        public float curTime;
        public void Start()
        {
            this.curTime = 0f;
            this.startFrameTick = (long) Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        public float Update()
        {
            FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
            if (instance.bActive)
            {
                long num3 = ((long) instance.LogicFrameTick) - this.startFrameTick;
                return (num3 * 0.001f);
            }
            this.curTime += Time.deltaTime;
            return this.curTime;
        }
    }
}

