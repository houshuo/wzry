namespace AGE
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CurveContainer
    {
        protected List<Curve> curves;
        protected const float maxSlope = 1000f;

        private CurveContainer(string[] _curveNames)
        {
            foreach (string str in _curveNames)
            {
                this.AddCurve(str);
            }
        }

        public void AddCurve(string _name)
        {
            this.curves.Add(new Curve(_name));
        }

        public Color SampleColor(float _time)
        {
            float[] numArray = this.SampleCurves(_time);
            return new Color(numArray[0], numArray[1], numArray[2], numArray[3]);
        }

        public float[] SampleCurves(float _curTime)
        {
            float[] numArray = new float[this.curves.Count];
            for (int i = 0; i < this.curves.Count; i++)
            {
                numArray[i] = this.curves[i].Sample(_curTime);
            }
            return numArray;
        }

        public Quaternion SampleEulerAngle(float _time)
        {
            float[] numArray = this.SampleCurves(_time);
            return Quaternion.Euler(numArray[0], numArray[1], numArray[2]);
        }

        public float SampleFloat(float _time)
        {
            return this.SampleCurves(_time)[0];
        }

        public Vector2 SampleVector2(float _time)
        {
            float[] numArray = this.SampleCurves(_time);
            return new Vector2(numArray[0], numArray[1]);
        }

        public Vector3 SampleVector3(float _time)
        {
            float[] numArray = this.SampleCurves(_time);
            return new Vector3(numArray[0], numArray[1], numArray[2]);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Curve
        {
            public ListView<Point> points;
            public readonly string name;
            public Curve(string _name)
            {
                this.name = _name;
                this.points = new ListView<Point>();
            }

            private float LocatePoint(float _curTime)
            {
                int count = this.points.Count;
                if (_curTime < 0f)
                {
                    _curTime = 0f;
                }
                else if (_curTime > 1f)
                {
                    _curTime = 1f;
                }
                int num3 = -1;
                int num4 = 0;
                int num5 = this.points.Count - 1;
                while (num4 != num5)
                {
                    int num6 = ((num4 + num5) / 2) + 1;
                    Point point = this.points[num6];
                    if (_curTime < point.time)
                    {
                        num5 = num6 - 1;
                    }
                    else
                    {
                        num4 = num6;
                    }
                }
                if (num4 == 0)
                {
                    Point point2 = this.points[0];
                    if (_curTime < point2.time)
                    {
                        num3 = -1;
                        goto Label_00B9;
                    }
                }
                num3 = num4;
            Label_00B9:
                if (num3 < 0)
                {
                    Point point3 = this.points[0];
                    return (-1f + (_curTime / point3.time));
                }
                if (num3 == (count - 1))
                {
                    Point point4 = this.points[count - 1];
                    Point point5 = this.points[count - 1];
                    return ((count - 1) + ((_curTime - point4.time) / (1f - point5.time)));
                }
                Point point6 = this.points[num3];
                Point point7 = this.points[num3 + 1];
                Point point8 = this.points[num3];
                return (num3 + ((_curTime - point6.time) / (point7.time - point8.time)));
            }

            public float Sample(float _curTime)
            {
                if (this.points.Count == 0)
                {
                    return 0f;
                }
                float num = this.LocatePoint(_curTime);
                if (num < 0f)
                {
                    Point point = this.points[0];
                    return point.value;
                }
                if (num >= (this.points.Count - 1))
                {
                    Point point2 = this.points[this.points.Count - 1];
                    return point2.value;
                }
                int num2 = (int) num;
                Point point3 = this.points[num2];
                Point point4 = this.points[num2 + 1];
                Point point5 = this.points[num2];
                float num3 = (_curTime - point3.time) / (point4.time - point5.time);
                Point point6 = this.points[num2];
                if (Math.Abs(point6.slope) < 1000f)
                {
                    Point point7 = this.points[num2 + 1];
                    if (Math.Abs(point7.slope) < 1000f)
                    {
                        Point point8 = this.points[num2];
                        Point point9 = this.points[num2];
                        Point point10 = this.points[num2];
                        float num4 = ((_curTime - point8.time) * point9.slope) + point10.value;
                        Point point11 = this.points[num2 + 1];
                        Point point12 = this.points[num2 + 1];
                        Point point13 = this.points[num2 + 1];
                        float num5 = ((_curTime - point11.time) * point12.slope) + point13.value;
                        Point point14 = this.points[num2];
                        Point point15 = this.points[num2 + 1];
                        float num6 = ((point14.slope * 2.3f) + (point15.slope * 1f)) / 3.3f;
                        Point point16 = this.points[num2];
                        Point point17 = this.points[num2 + 1];
                        float num7 = ((point16.slope * 1f) + (point17.slope * 2.3f)) / 3.3f;
                        Point point18 = this.points[num2];
                        Point point19 = this.points[num2];
                        float num8 = ((_curTime - point18.time) * num6) + point19.value;
                        Point point20 = this.points[num2 + 1];
                        Point point21 = this.points[num2 + 1];
                        float num9 = ((_curTime - point20.time) * num7) + point21.value;
                        float num10 = num3;
                        float num11 = 1f - num10;
                        return ((((((num4 * num11) * num11) * num11) + ((((num8 * num11) * num11) * num10) * 3f)) + ((((num9 * num11) * num10) * num10) * 3f)) + (((num5 * num10) * num10) * num10));
                    }
                }
                Point point22 = this.points[num2];
                return point22.slope;
            }

            public int AddPoint(float _time, float _value, float _slope, bool _addDirectly)
            {
                Point item = new Point {
                    time = _time,
                    value = _value,
                    slope = _slope
                };
                int index = 0;
                if ((this.points.Count == 0) || _addDirectly)
                {
                    this.points.Add(item);
                    return index;
                }
                index = (int) (this.LocatePoint(_time) + 1f);
                if (index > this.points.Count)
                {
                    index = this.points.Count;
                }
                this.points.Insert(index, item);
                return index;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public float time;
                public float value;
                public float slope;
            }
        }
    }
}

