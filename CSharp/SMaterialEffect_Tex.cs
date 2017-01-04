using System;
using UnityEngine;

public class SMaterialEffect_Tex : SMaterialEffect_Base
{
    public bool enableFade;
    public float factorScale = 1f;
    public float fadeInterval;
    public string fadeParamName;
    public SMaterialEffect_Base.FadeState fadeState;
    public SMaterialEffect_Base.STimer fadeTimer = new SMaterialEffect_Base.STimer();
    public bool hasFadeFactor;
    public Texture tex;
    public string texParamName;

    public void BeginFadeOut()
    {
        if (this.fadeState != SMaterialEffect_Base.FadeState.FadeOut)
        {
            this.fadeTimer.Start();
            this.fadeState = SMaterialEffect_Base.FadeState.FadeOut;
        }
    }

    public override void OnRelease()
    {
        base.owner = null;
        this.texParamName = string.Empty;
        this.fadeParamName = string.Empty;
        this.tex = null;
        this.factorScale = 1f;
        this.enableFade = false;
        this.hasFadeFactor = false;
        this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
        this.fadeInterval = 0f;
    }

    public override void Play()
    {
        base.Play();
        ListView<Material> mats = base.owner.mats;
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].SetTexture(this.texParamName, this.tex);
        }
        if (this.fadeInterval == 0f)
        {
            this.enableFade = false;
        }
        if (this.hasFadeFactor)
        {
            float factor = !this.enableFade ? 1f : 0f;
            this.SetFactor(factor);
        }
        else
        {
            this.enableFade = false;
        }
        this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
        this.fadeTimer.Start();
    }

    public void Replay(Texture newTex)
    {
        base.AllocId();
        if (this.tex != newTex)
        {
            this.tex = newTex;
            ListView<Material> mats = base.owner.mats;
            for (int i = 0; i < mats.Count; i++)
            {
                mats[i].SetTexture(this.texParamName, newTex);
            }
        }
        if (this.fadeState == SMaterialEffect_Base.FadeState.FadeOut)
        {
            this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
        }
    }

    private void SetFactor(float factor)
    {
        factor *= this.factorScale;
        ListView<Material> mats = base.owner.mats;
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].SetFloat(this.fadeParamName, factor);
        }
    }

    public override bool Update()
    {
        if (this.enableFade)
        {
            float factor = 1f;
            if (SMaterialEffect_Base.UpdateFadeState(out factor, ref this.fadeState, ref this.fadeTimer, this.fadeInterval))
            {
                this.SetFactor(factor);
            }
            if (this.fadeState == SMaterialEffect_Base.FadeState.Stopped)
            {
                this.Stop();
                return true;
            }
        }
        return false;
    }
}

