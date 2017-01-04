using System;
using UnityEngine;

[Serializable]
public class RGBCurve : ScriptableObject
{
    public AnimationCurve B = new AnimationCurve();
    public AnimationCurve G = new AnimationCurve();
    public AnimationCurve R = new AnimationCurve();

    public Vector3 Eval(float t)
    {
        return new Vector3 { x = this.R.Evaluate(t), y = this.G.Evaluate(t), z = this.B.Evaluate(t) };
    }

    public static float MaxTime(AnimationCurve curve)
    {
        if ((curve == null) || (curve.length == 0))
        {
            return 0f;
        }
        int length = curve.length;
        Keyframe keyframe = curve[length - 1];
        return keyframe.time;
    }

    public float length
    {
        get
        {
            float a = MaxTime(this.R);
            float b = MaxTime(this.G);
            float num3 = MaxTime(this.B);
            return Mathf.Max(Mathf.Max(a, b), num3);
        }
    }
}

