using System;
using UnityEngine;

public class SMaterialEffect_Curve : SMaterialEffect_Base
{
    public RGBCurve curve;
    public float maxTime;
    public string paramName = string.Empty;
    public SMaterialEffect_Base.STimer timer = new SMaterialEffect_Base.STimer();

    public override void OnRelease()
    {
        base.owner = null;
        this.curve = null;
    }

    public override void Play()
    {
        base.Play();
        this.timer.Start();
        this.maxTime = this.curve.length;
        this.maxTime = Mathf.Max(this.maxTime, 0.001f);
        this.SetColor(0f);
    }

    private void SetColor(float time)
    {
        Vector3 vector = this.curve.Eval(time);
        Vector4 vector2 = new Vector4(vector.x, vector.y, vector.z, 1f);
        ListView<Material> mats = base.owner.mats;
        for (int i = 0; i < mats.Count; i++)
        {
            mats[i].SetVector(this.paramName, vector2);
        }
    }

    public override bool Update()
    {
        float time = this.timer.Update();
        this.SetColor(time);
        if (time >= this.maxTime)
        {
            this.Stop();
            return true;
        }
        return false;
    }
}

