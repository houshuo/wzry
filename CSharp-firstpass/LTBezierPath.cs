using System;
using UnityEngine;

public class LTBezierPath
{
    private LTBezier[] beziers;
    public float length;
    private float[] lengthRatio;
    public bool orientToPath;
    public bool orientToPath2d;
    public Vector3[] pts;

    public LTBezierPath()
    {
    }

    public LTBezierPath(Vector3[] pts_)
    {
        this.setPoints(pts_);
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
        float num = 0f;
        for (int i = 0; i < this.lengthRatio.Length; i++)
        {
            num += this.lengthRatio[i];
            if (num >= ratio)
            {
                return this.beziers[i].point((ratio - (num - this.lengthRatio[i])) / this.lengthRatio[i]);
            }
        }
        return this.beziers[this.lengthRatio.Length - 1].point(1f);
    }

    public void setPoints(Vector3[] pts_)
    {
        int num2;
        if (pts_.Length < 4)
        {
            LeanTween.logError("LeanTween - When passing values for a vector path, you must pass four or more values!");
        }
        if ((pts_.Length % 4) != 0)
        {
            LeanTween.logError("LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2...");
        }
        this.pts = pts_;
        int index = 0;
        this.beziers = new LTBezier[this.pts.Length / 4];
        this.lengthRatio = new float[this.beziers.Length];
        this.length = 0f;
        for (num2 = 0; num2 < this.pts.Length; num2 += 4)
        {
            this.beziers[index] = new LTBezier(this.pts[num2], this.pts[num2 + 2], this.pts[num2 + 1], this.pts[num2 + 3], 0.05f);
            this.length += this.beziers[index].length;
            index++;
        }
        for (num2 = 0; num2 < this.beziers.Length; num2++)
        {
            this.lengthRatio[num2] = this.beziers[num2].length / this.length;
        }
    }
}

