using System;
using UnityEngine;

public class SMaterialEffect_HighLit : SMaterialEffect_Base
{
    public Vector3 color = new Vector3(1f, 1f, 0f);
    public float maxExp = 1.5f;
    public float maxFactor = 2f;
    public float minExp = 2f;
    public float minFactor = 1f;
    public string paramName = "_RimColor";
    public float period = 0.8f;
    public SMaterialEffect_Base.STimer timer = new SMaterialEffect_Base.STimer();

    public SMaterialEffect_HighLit()
    {
        base.enableKeyword = "_RIM_COLOR_ON";
        base.disableKeyword = "_RIM_COLOR_OFF";
    }

    public override void Play()
    {
        base.Play();
        this.timer.Start();
        this.SetColor();
    }

    private void SetColor()
    {
        this.timer.Update();
        float t = Mathf.PingPong(this.timer.curTime, this.period);
        float num2 = Mathf.Lerp(this.minExp, this.maxExp, t);
        float num3 = Mathf.Lerp(this.minFactor, this.maxFactor, t);
        Vector4 vector = new Vector4 {
            x = this.color.x * num3,
            y = this.color.y * num3,
            z = this.color.z * num3,
            w = num2
        };
        ListView<Material> mats = base.owner.mats;
        if (mats != null)
        {
            for (int i = 0; i < mats.Count; i++)
            {
                Material material = mats[i];
                if (material != null)
                {
                    material.SetVector(this.paramName, vector);
                }
            }
        }
    }

    public override bool Update()
    {
        this.SetColor();
        return false;
    }
}

