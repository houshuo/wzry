using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class NgInterpolate
{
    private static Vector3 Bezier(Function ease, Vector3[] points, float elapsedTime, float duration)
    {
        for (int i = points.Length - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                points[j].x = ease(points[j].x, points[j + 1].x - points[j].x, elapsedTime, duration);
                points[j].y = ease(points[j].y, points[j + 1].y - points[j].y, elapsedTime, duration);
                points[j].z = ease(points[j].z, points[j + 1].z - points[j].z, elapsedTime, duration);
            }
        }
        return points[0];
    }

    private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
    {
        float num = elapsedTime / duration;
        float num2 = num * num;
        float num3 = num2 * num;
        return (Vector3) ((((previous * (((-0.5f * num3) + num2) - (0.5f * num))) + (start * (((1.5f * num3) + (-2.5f * num2)) + 1f))) + (end * (((-1.5f * num3) + (2f * num2)) + (0.5f * num)))) + (next * ((0.5f * num3) - (0.5f * num2))));
    }

    public static Function Ease(EaseType type)
    {
        switch (type)
        {
            case EaseType.Linear:
                return new Function(NgInterpolate.Linear);

            case EaseType.EaseInQuad:
                return new Function(NgInterpolate.EaseInQuad);

            case EaseType.EaseOutQuad:
                return new Function(NgInterpolate.EaseOutQuad);

            case EaseType.EaseInOutQuad:
                return new Function(NgInterpolate.EaseInOutQuad);

            case EaseType.EaseInCubic:
                return new Function(NgInterpolate.EaseInCubic);

            case EaseType.EaseOutCubic:
                return new Function(NgInterpolate.EaseOutCubic);

            case EaseType.EaseInOutCubic:
                return new Function(NgInterpolate.EaseInOutCubic);

            case EaseType.EaseInQuart:
                return new Function(NgInterpolate.EaseInQuart);

            case EaseType.EaseOutQuart:
                return new Function(NgInterpolate.EaseOutQuart);

            case EaseType.EaseInOutQuart:
                return new Function(NgInterpolate.EaseInOutQuart);

            case EaseType.EaseInQuint:
                return new Function(NgInterpolate.EaseInQuint);

            case EaseType.EaseOutQuint:
                return new Function(NgInterpolate.EaseOutQuint);

            case EaseType.EaseInOutQuint:
                return new Function(NgInterpolate.EaseInOutQuint);

            case EaseType.EaseInSine:
                return new Function(NgInterpolate.EaseInSine);

            case EaseType.EaseOutSine:
                return new Function(NgInterpolate.EaseOutSine);

            case EaseType.EaseInOutSine:
                return new Function(NgInterpolate.EaseInOutSine);

            case EaseType.EaseInExpo:
                return new Function(NgInterpolate.EaseInExpo);

            case EaseType.EaseOutExpo:
                return new Function(NgInterpolate.EaseOutExpo);

            case EaseType.EaseInOutExpo:
                return new Function(NgInterpolate.EaseInOutExpo);

            case EaseType.EaseInCirc:
                return new Function(NgInterpolate.EaseInCirc);

            case EaseType.EaseOutCirc:
                return new Function(NgInterpolate.EaseOutCirc);

            case EaseType.EaseInOutCirc:
                return new Function(NgInterpolate.EaseInOutCirc);
        }
        return null;
    }

    private static Vector3 Ease(Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration)
    {
        start.x = ease(start.x, distance.x, elapsedTime, duration);
        start.y = ease(start.y, distance.y, elapsedTime, duration);
        start.z = ease(start.z, distance.z, elapsedTime, duration);
        return start;
    }

    private static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return ((-distance * (Mathf.Sqrt(1f - (elapsedTime * elapsedTime)) - 1f)) + start);
    }

    private static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return ((((distance * elapsedTime) * elapsedTime) * elapsedTime) + start);
    }

    private static float EaseInExpo(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return ((distance * Mathf.Pow(2f, 10f * ((elapsedTime / duration) - 1f))) + start);
    }

    private static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return (((-distance / 2f) * (Mathf.Sqrt(1f - (elapsedTime * elapsedTime)) - 1f)) + start);
        }
        elapsedTime -= 2f;
        return (((distance / 2f) * (Mathf.Sqrt(1f - (elapsedTime * elapsedTime)) + 1f)) + start);
    }

    private static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return (((((distance / 2f) * elapsedTime) * elapsedTime) * elapsedTime) + start);
        }
        elapsedTime -= 2f;
        return (((distance / 2f) * (((elapsedTime * elapsedTime) * elapsedTime) + 2f)) + start);
    }

    private static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return (((distance / 2f) * Mathf.Pow(2f, 10f * (elapsedTime - 1f))) + start);
        }
        elapsedTime--;
        return (((distance / 2f) * (-Mathf.Pow(2f, -10f * elapsedTime) + 2f)) + start);
    }

    private static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return ((((distance / 2f) * elapsedTime) * elapsedTime) + start);
        }
        elapsedTime--;
        return (((-distance / 2f) * ((elapsedTime * (elapsedTime - 2f)) - 1f)) + start);
    }

    private static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return ((((((distance / 2f) * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + start);
        }
        elapsedTime -= 2f;
        return (((-distance / 2f) * ((((elapsedTime * elapsedTime) * elapsedTime) * elapsedTime) - 2f)) + start);
    }

    private static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f;
        if (elapsedTime < 1f)
        {
            return (((((((distance / 2f) * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + start);
        }
        elapsedTime -= 2f;
        return (((distance / 2f) * (((((elapsedTime * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + 2f)) + start);
    }

    private static float EaseInOutSine(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return (((-distance / 2f) * (Mathf.Cos((3.141593f * elapsedTime) / duration) - 1f)) + start);
    }

    private static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return (((distance * elapsedTime) * elapsedTime) + start);
    }

    private static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return (((((distance * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + start);
    }

    private static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return ((((((distance * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + start);
    }

    private static float EaseInSine(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return (((-distance * Mathf.Cos((elapsedTime / duration) * 1.570796f)) + distance) + start);
    }

    private static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        elapsedTime--;
        return ((distance * Mathf.Sqrt(1f - (elapsedTime * elapsedTime))) + start);
    }

    private static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        elapsedTime--;
        return ((distance * (((elapsedTime * elapsedTime) * elapsedTime) + 1f)) + start);
    }

    private static float EaseOutExpo(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return ((distance * (-Mathf.Pow(2f, (-10f * elapsedTime) / duration) + 1f)) + start);
    }

    private static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        return (((-distance * elapsedTime) * (elapsedTime - 2f)) + start);
    }

    private static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        elapsedTime--;
        return ((-distance * ((((elapsedTime * elapsedTime) * elapsedTime) * elapsedTime) - 1f)) + start);
    }

    private static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
    {
        elapsedTime = (elapsedTime <= duration) ? (elapsedTime / duration) : 1f;
        elapsedTime--;
        return ((distance * (((((elapsedTime * elapsedTime) * elapsedTime) * elapsedTime) * elapsedTime) + 1f)) + start);
    }

    private static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return ((distance * Mathf.Sin((elapsedTime / duration) * 1.570796f)) + start);
    }

    private static Vector3 Identity(Vector3 v)
    {
        return v;
    }

    private static float Linear(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration)
        {
            elapsedTime = duration;
        }
        return ((distance * (elapsedTime / duration)) + start);
    }

    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, int slices)
    {
        IEnumerable<float> steps = NewCounter(0, slices + 1, 1);
        return NewBezier<Transform>(ease, nodes, new ToVector3<Transform>(NgInterpolate.TransformDotPosition), (float) (slices + 1), steps);
    }

    public static IEnumerable<Vector3> NewBezier(Function ease, Transform[] nodes, float duration)
    {
        IEnumerable<float> steps = NewTimer(duration);
        return NewBezier<Transform>(ease, nodes, new ToVector3<Transform>(NgInterpolate.TransformDotPosition), duration, steps);
    }

    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, int slices)
    {
        IEnumerable<float> steps = NewCounter(0, slices + 1, 1);
        return NewBezier<Vector3>(ease, points, new ToVector3<Vector3>(NgInterpolate.Identity), (float) (slices + 1), steps);
    }

    public static IEnumerable<Vector3> NewBezier(Function ease, Vector3[] points, float duration)
    {
        IEnumerable<float> steps = NewTimer(duration);
        return NewBezier<Vector3>(ease, points, new ToVector3<Vector3>(NgInterpolate.Identity), duration, steps);
    }

    [DebuggerHidden]
    private static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps)
    {
        return new <NewBezier>c__IteratorB<T> { nodes = nodes, steps = steps, toVector3 = toVector3, ease = ease, maxStep = maxStep, <$>nodes = nodes, <$>steps = steps, <$>toVector3 = toVector3, <$>ease = ease, <$>maxStep = maxStep, $PC = -2 };
    }

    public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop)
    {
        return NewCatmullRom<Transform>(nodes, new ToVector3<Transform>(NgInterpolate.TransformDotPosition), slices, loop);
    }

    public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
    {
        return NewCatmullRom<Vector3>(points, new ToVector3<Vector3>(NgInterpolate.Identity), slices, loop);
    }

    [DebuggerHidden]
    private static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop)
    {
        return new <NewCatmullRom>c__IteratorC<T> { nodes = nodes, toVector3 = toVector3, loop = loop, slices = slices, <$>nodes = nodes, <$>toVector3 = toVector3, <$>loop = loop, <$>slices = slices, $PC = -2 };
    }

    [DebuggerHidden]
    private static IEnumerable<float> NewCounter(int start, int end, int step)
    {
        return new <NewCounter>c__Iterator9 { start = start, end = end, step = step, <$>start = start, <$>end = end, <$>step = step, $PC = -2 };
    }

    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices)
    {
        IEnumerable<float> driver = NewCounter(0, slices + 1, 1);
        return NewEase(ease, start, end, (float) (slices + 1), driver);
    }

    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration)
    {
        IEnumerable<float> driver = NewTimer(duration);
        return NewEase(ease, start, end, duration, driver);
    }

    [DebuggerHidden]
    private static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
    {
        return new <NewEase>c__IteratorA { end = end, start = start, driver = driver, ease = ease, total = total, <$>end = end, <$>start = start, <$>driver = driver, <$>ease = ease, <$>total = total };
    }

    [DebuggerHidden]
    private static IEnumerable<float> NewTimer(float duration)
    {
        return new <NewTimer>c__Iterator8 { duration = duration, <$>duration = duration, $PC = -2 };
    }

    private static Vector3 TransformDotPosition(Transform t)
    {
        return t.position;
    }

    [CompilerGenerated]
    private sealed class <NewBezier>c__IteratorB<T> : IDisposable, IEnumerable, IEnumerator, IEnumerable<Vector3>, IEnumerator<Vector3>
    {
        internal Vector3 $current;
        internal int $PC;
        internal NgInterpolate.Function <$>ease;
        internal float <$>maxStep;
        internal IList <$>nodes;
        internal IEnumerable<float> <$>steps;
        internal NgInterpolate.ToVector3<T> <$>toVector3;
        internal IEnumerator<float> <$s_116>__1;
        internal int <i>__3;
        internal Vector3[] <points>__0;
        internal float <step>__2;
        internal NgInterpolate.Function ease;
        internal float maxStep;
        internal IList nodes;
        internal IEnumerable<float> steps;
        internal NgInterpolate.ToVector3<T> toVector3;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        if (this.<$s_116>__1 == null)
                        {
                        }
                        this.<$s_116>__1.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    if (this.nodes.Count < 2)
                    {
                        goto Label_0147;
                    }
                    this.<points>__0 = new Vector3[this.nodes.Count];
                    this.<$s_116>__1 = this.steps.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_014E;
            }
            try
            {
                while (this.<$s_116>__1.MoveNext())
                {
                    this.<step>__2 = this.<$s_116>__1.Current;
                    this.<i>__3 = 0;
                    while (this.<i>__3 < this.nodes.Count)
                    {
                        this.<points>__0[this.<i>__3] = this.toVector3((T) this.nodes[this.<i>__3]);
                        this.<i>__3++;
                    }
                    this.$current = NgInterpolate.Bezier(this.ease, this.<points>__0, this.<step>__2, this.maxStep);
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_116>__1 == null)
                {
                }
                this.<$s_116>__1.Dispose();
            }
        Label_0147:
            this.$PC = -1;
        Label_014E:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new NgInterpolate.<NewBezier>c__IteratorB<T> { nodes = this.<$>nodes, steps = this.<$>steps, toVector3 = this.<$>toVector3, ease = this.<$>ease, maxStep = this.<$>maxStep };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<UnityEngine.Vector3>.GetEnumerator();
        }

        Vector3 IEnumerator<Vector3>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <NewCatmullRom>c__IteratorC<T> : IDisposable, IEnumerable, IEnumerator, IEnumerable<Vector3>, IEnumerator<Vector3>
    {
        internal Vector3 $current;
        internal int $PC;
        internal bool <$>loop;
        internal IList <$>nodes;
        internal int <$>slices;
        internal NgInterpolate.ToVector3<T> <$>toVector3;
        internal int <current>__1;
        internal int <end>__4;
        internal int <last>__0;
        internal int <next>__5;
        internal int <previous>__2;
        internal int <start>__3;
        internal int <step>__7;
        internal int <stepCount>__6;
        internal bool loop;
        internal IList nodes;
        internal int slices;
        internal NgInterpolate.ToVector3<T> toVector3;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (this.nodes.Count < 2)
                    {
                        break;
                    }
                    this.$current = this.toVector3((T) this.nodes[0]);
                    this.$PC = 1;
                    goto Label_0277;

                case 1:
                    this.<last>__0 = this.nodes.Count - 1;
                    this.<current>__1 = 0;
                    while (this.loop || (this.<current>__1 < this.<last>__0))
                    {
                        if (this.loop && (this.<current>__1 > this.<last>__0))
                        {
                            this.<current>__1 = 0;
                        }
                        this.<previous>__2 = (this.<current>__1 != 0) ? (this.<current>__1 - 1) : (!this.loop ? this.<current>__1 : this.<last>__0);
                        this.<start>__3 = this.<current>__1;
                        this.<end>__4 = (this.<current>__1 != this.<last>__0) ? (this.<current>__1 + 1) : (!this.loop ? this.<current>__1 : 0);
                        this.<next>__5 = (this.<end>__4 != this.<last>__0) ? (this.<end>__4 + 1) : (!this.loop ? this.<end>__4 : 0);
                        this.<stepCount>__6 = this.slices + 1;
                        this.<step>__7 = 1;
                        while (this.<step>__7 <= this.<stepCount>__6)
                        {
                            this.$current = NgInterpolate.CatmullRom(this.toVector3((T) this.nodes[this.<previous>__2]), this.toVector3((T) this.nodes[this.<start>__3]), this.toVector3((T) this.nodes[this.<end>__4]), this.toVector3((T) this.nodes[this.<next>__5]), (float) this.<step>__7, (float) this.<stepCount>__6);
                            this.$PC = 2;
                            goto Label_0277;
                        Label_0225:
                            this.<step>__7++;
                        }
                        this.<current>__1++;
                    }
                    break;

                case 2:
                    goto Label_0225;

                default:
                    goto Label_0275;
            }
            this.$PC = -1;
        Label_0275:
            return false;
        Label_0277:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new NgInterpolate.<NewCatmullRom>c__IteratorC<T> { nodes = this.<$>nodes, toVector3 = this.<$>toVector3, loop = this.<$>loop, slices = this.<$>slices };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<UnityEngine.Vector3>.GetEnumerator();
        }

        Vector3 IEnumerator<Vector3>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <NewCounter>c__Iterator9 : IDisposable, IEnumerable, IEnumerator, IEnumerable<float>, IEnumerator<float>
    {
        internal float $current;
        internal int $PC;
        internal int <$>end;
        internal int <$>start;
        internal int <$>step;
        internal int <i>__0;
        internal int end;
        internal int start;
        internal int step;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<i>__0 = this.start;
                    break;

                case 1:
                    this.<i>__0 += this.step;
                    break;

                default:
                    goto Label_0076;
            }
            if (this.<i>__0 <= this.end)
            {
                this.$current = this.<i>__0;
                this.$PC = 1;
                return true;
            }
            this.$PC = -1;
        Label_0076:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<float> IEnumerable<float>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new NgInterpolate.<NewCounter>c__Iterator9 { start = this.<$>start, end = this.<$>end, step = this.<$>step };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<float>.GetEnumerator();
        }

        float IEnumerator<float>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <NewEase>c__IteratorA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal IEnumerable<float> <$>driver;
        internal NgInterpolate.Function <$>ease;
        internal Vector3 <$>end;
        internal Vector3 <$>start;
        internal float <$>total;
        internal IEnumerator<float> <$s_115>__1;
        internal Vector3 <distance>__0;
        internal float <i>__2;
        internal IEnumerable<float> driver;
        internal NgInterpolate.Function ease;
        internal Vector3 end;
        internal Vector3 start;
        internal float total;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        if (this.<$s_115>__1 == null)
                        {
                        }
                        this.<$s_115>__1.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<distance>__0 = this.end - this.start;
                    this.<$s_115>__1 = this.driver.GetEnumerator();
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00E2;
            }
            try
            {
                while (this.<$s_115>__1.MoveNext())
                {
                    this.<i>__2 = this.<$s_115>__1.Current;
                    this.$current = NgInterpolate.Ease(this.ease, this.start, this.<distance>__0, this.<i>__2, this.total);
                    this.$PC = 1;
                    flag = true;
                    return true;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_115>__1 == null)
                {
                }
                this.<$s_115>__1.Dispose();
            }
            this.$PC = -1;
        Label_00E2:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <NewTimer>c__Iterator8 : IDisposable, IEnumerable, IEnumerator, IEnumerable<float>, IEnumerator<float>
    {
        internal float $current;
        internal int $PC;
        internal float <$>duration;
        internal float <elapsedTime>__0;
        internal float duration;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<elapsedTime>__0 = 0f;
                    break;

                case 1:
                    this.<elapsedTime>__0 += Time.deltaTime;
                    if (this.<elapsedTime>__0 < this.duration)
                    {
                        break;
                    }
                    this.$current = this.<elapsedTime>__0;
                    this.$PC = 2;
                    goto Label_00A2;

                case 2:
                    break;

                default:
                    goto Label_00A0;
            }
            if (this.<elapsedTime>__0 < this.duration)
            {
                this.$current = this.<elapsedTime>__0;
                this.$PC = 1;
                goto Label_00A2;
            }
            this.$PC = -1;
        Label_00A0:
            return false;
        Label_00A2:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<float> IEnumerable<float>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new NgInterpolate.<NewTimer>c__Iterator8 { duration = this.<$>duration };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<float>.GetEnumerator();
        }

        float IEnumerator<float>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc
    }

    public delegate float Function(float a, float b, float c, float d);

    public delegate Vector3 ToVector3<T>(T v);
}

