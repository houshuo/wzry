using System;
using UnityEngine;

public class LTBezier
{
    private Vector3 a;
    private Vector3 aa;
    private float[] arcLengths;
    private Vector3 bb;
    private Vector3 cc;
    private float len;
    public float length;

    public LTBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision)
    {
        this.a = a;
        this.aa = (-a + ((Vector3) (3f * (b - c)))) + d;
        this.bb = (Vector3) ((3f * (a + c)) - (6f * b));
        this.cc = (Vector3) (3f * (b - a));
        this.len = 1f / precision;
        this.arcLengths = new float[((int) this.len) + 1];
        this.arcLengths[0] = 0f;
        Vector3 vector = a;
        float num = 0f;
        for (int i = 1; i <= this.len; i++)
        {
            Vector3 vector2 = this.bezierPoint(i * precision);
            Vector3 vector3 = vector - vector2;
            num += vector3.magnitude;
            this.arcLengths[i] = num;
            vector = vector2;
        }
        this.length = num;
    }

    private Vector3 bezierPoint(float t)
    {
        return (((Vector3) (((((this.aa * t) + this.bb) * t) + this.cc) * t)) + this.a);
    }

    private float map(float u)
    {
        float num = u * this.arcLengths[(int) this.len];
        int num2 = 0;
        int len = (int) this.len;
        int index = 0;
        while (num2 < len)
        {
            index = num2 + (((int) (((float) (len - num2)) / 2f)) | 0);
            if (this.arcLengths[index] < num)
            {
                num2 = index + 1;
            }
            else
            {
                len = index;
            }
        }
        if (this.arcLengths[index] > num)
        {
            index--;
        }
        if (index < 0)
        {
            index = 0;
        }
        return ((index + ((num - this.arcLengths[index]) / (this.arcLengths[index + 1] - this.arcLengths[index]))) / this.len);
    }

    public Vector3 point(float t)
    {
        return this.bezierPoint(this.map(t));
    }
}

