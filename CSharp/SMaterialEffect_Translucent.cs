using System;
using UnityEngine;

public class SMaterialEffect_Translucent : SMaterialEffect_Base
{
    public bool enableFade = true;
    public float fadeInterval = 0.05f;
    public SMaterialEffect_Base.FadeState fadeState;
    public SMaterialEffect_Base.STimer fadeTimer = new SMaterialEffect_Base.STimer();
    public float minAlpha = 0.3f;

    public void BeginFadeOut()
    {
        if (this.fadeState != SMaterialEffect_Base.FadeState.FadeOut)
        {
            this.fadeTimer.Start();
            this.fadeState = SMaterialEffect_Base.FadeState.FadeOut;
        }
    }

    public override void OnMeshChanged(ListView<Material> oldMats, ListView<Material> newMats)
    {
        this.SetTranslucent(oldMats, false);
        this.SetTranslucent(newMats, true);
    }

    public override void OnRelease()
    {
        base.owner = null;
        this.minAlpha = 0.3f;
        this.enableFade = true;
        this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
        this.fadeInterval = 0.05f;
    }

    public override void Play()
    {
        if (this.fadeInterval == 0f)
        {
            this.enableFade = false;
        }
        float factor = !this.enableFade ? 1f : 0f;
        this.SetTranslucent(base.owner.mats, true);
        this.SetAlpha(factor);
        this.fadeState = SMaterialEffect_Base.FadeState.FadeIn;
        this.fadeTimer.Start();
    }

    private void SetAlpha(float factor)
    {
        float num = Mathf.Lerp(1f, this.minAlpha, factor);
        ListView<Material> mats = base.owner.mats;
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].SetFloat("_AlphaVal", num);
        }
    }

    private void SetTranslucent(ListView<Material> mats, bool b)
    {
        if (mats != null)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                bool flag;
                bool flag2;
                bool flag3;
                Material material = mats[i];
                string name = material.shader.name;
                HeroMaterialUtility.GetShaderProperty(name, out flag, out flag2, out flag3);
                string str2 = HeroMaterialUtility.MakeShaderName(name, flag, b, flag3);
                if (str2 != material.shader.name)
                {
                    material.shader = Shader.Find(str2);
                }
            }
        }
    }

    public override void Stop()
    {
        this.SetTranslucent(base.owner.mats, false);
    }

    public override bool Update()
    {
        if (this.enableFade)
        {
            float factor = 1f;
            if (SMaterialEffect_Base.UpdateFadeState(out factor, ref this.fadeState, ref this.fadeTimer, this.fadeInterval))
            {
                this.SetAlpha(factor);
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

