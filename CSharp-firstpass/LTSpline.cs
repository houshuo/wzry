using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class LTSpline
{
    private int currPt;
    private float[] lengthRatio;
    private float[] lengths;
    private int numSections;
    public bool orientToPath;
    public bool orientToPath2d;
    public Vector3[] pts;
    private float totalLength;

    public LTSpline(params Vector3[] pts)
    {
        this.pts = new Vector3[pts.Length];
        Array.Copy(pts, this.pts, pts.Length);
        this.numSections = pts.Length - 3;
        int num = 20;
        this.lengthRatio = new float[num];
        this.lengths = new float[num];
        Vector3 vector = new Vector3(float.PositiveInfinity, 0f, 0f);
        this.totalLength = 0f;
        for (int i = 0; i < num; i++)
        {
            float t = (i * 1f) / ((float) num);
            Vector3 vector2 = this.interp(t);
            if (i >= 1)
            {
                Vector3 vector3 = vector2 - vector;
                this.lengths[i] = vector3.magnitude;
            }
            this.totalLength += this.lengths[i];
            vector = vector2;
        }
        float num4 = 0f;
        for (int j = 0; j < this.lengths.Length; j++)
        {
            float num6 = (j * 1f) / ((float) (this.lengths.Length - 1));
            this.currPt = Mathf.Min(Mathf.FloorToInt(num6 * this.numSections), this.numSections - 1);
            float num7 = this.lengths[j] / this.totalLength;
            this.lengthRatio[j] = num4 + num7;
        }
    }

    public void gizmoDraw(float t = -1f)
    {
        if ((this.lengthRatio != null) && (this.lengthRatio.Length > 0))
        {
            Vector3 to = this.point(0f);
            for (int i = 1; i <= 120; i++)
            {
                float ratio = ((float) i) / 120f;
                Vector3 from = this.point(ratio);
                Gizmos.DrawLine(from, to);
                to = from;
            }
        }
    }

    public Vector3 interp(float t)
    {
        this.currPt = Mathf.Min(Mathf.FloorToInt(t * this.numSections), this.numSections - 1);
        float num = (t * this.numSections) - this.currPt;
        Vector3 vector = this.pts[this.currPt];
        Vector3 vector2 = this.pts[this.currPt + 1];
        Vector3 vector3 = this.pts[this.currPt + 2];
        Vector3 vector4 = this.pts[this.currPt + 3];
        return (Vector3) (0.5f * (((((((-vector + (3f * vector2)) - (3f * vector3)) + vector4) * ((num * num) * num)) + (((((2f * vector) - (5f * vector2)) + (4f * vector3)) - vector4) * (num * num))) + ((-vector + vector3) * num)) + (2f * vector2)));
    }

    public float map(float t)
    {
        for (int i = 0; i < this.lengthRatio.Length; i++)
        {
            if (this.lengthRatio[i] >= t)
            {
                return (this.lengthRatio[i] + (t - this.lengthRatio[i]));
            }
        }
        return 1f;
    }

    public void place(Transform transform, float ratio)
    {
        this.place(transform, ratio, Vector3.up);
    }

    public void place(Transform transform, float ratio, Vector3 worldUp)
    {
        transform.position = this.point(ratio);
        ratio += 0.001f;
        if (ratio <= 1f)
        {
            transform.LookAt(this.point(ratio), worldUp);
        }
    }

    public void place2d(Transform transform, float ratio)
    {
        transform.position = this.point(ratio);
        ratio += 0.001f;
        if (ratio <= 1f)
        {
            Vector3 vector = this.point(ratio) - transform.position;
            float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
            transform.eulerAngles = new Vector3(0f, 0f, z);
        }
    }

    public void placeLocal(Transform transform, float ratio)
    {
        this.placeLocal(transform, ratio, Vector3.up);
    }

    public void placeLocal(Transform transform, float ratio, Vector3 worldUp)
    {
        transform.localPosition = this.point(ratio);
        ratio += 0.001f;
        if (ratio <= 1f)
        {
            transform.LookAt(transform.parent.TransformPoint(this.point(ratio)), worldUp);
        }
    }

    public void placeLocal2d(Transform transform, float ratio)
    {
        transform.localPosition = this.point(ratio);
        ratio += 0.001f;
        if (ratio <= 1f)
        {
            Vector3 vector = transform.parent.TransformPoint(this.point(ratio)) - transform.localPosition;
            float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
            transform.eulerAngles = new Vector3(0f, 0f, z);
        }
    }

    public Vector3 point(float ratio)
    {
        float t = this.map(ratio);
        return this.interp(t);
    }

    public Vector3 Velocity(float t)
    {
        t = this.map(t);
        int num = this.pts.Length - 3;
        int index = Mathf.Min(Mathf.FloorToInt(t * num), num - 1);
        float num3 = (t * num) - index;
        Vector3 vector = this.pts[index];
        Vector3 vector2 = this.pts[index + 1];
        Vector3 vector3 = this.pts[index + 2];
        Vector3 vector4 = this.pts[index + 3];
        return (Vector3) (((((1.5f * (((-vector + (3f * vector2)) - (3f * vector3)) + vector4)) * (num3 * num3)) + (((((2f * vector) - (5f * vector2)) + (4f * vector3)) - vector4) * num3)) + (0.5f * vector3)) - (0.5f * vector));
    }
}

