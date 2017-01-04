using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrailPoolHelper : MonoBehaviour, IPooledMonoBehaviour
{
    private bool _awaked;
    private bool _started;
    private TrailRenderer _trail;

    private void Awake()
    {
        this.OnCreate();
    }

    private void DoStart()
    {
        base.StartCoroutine(ResetTrail(this._trail));
    }

    public void OnCreate()
    {
        if (!this._awaked)
        {
            this._awaked = true;
            this._trail = base.GetComponent<TrailRenderer>();
        }
    }

    public void OnGet()
    {
        if (!this._started)
        {
            this._started = true;
            this.DoStart();
        }
    }

    public void OnRecycle()
    {
        this._started = false;
    }

    [DebuggerHidden]
    private static IEnumerator ResetTrail(TrailRenderer trail)
    {
        return new <ResetTrail>c__IteratorD { trail = trail, <$>trail = trail };
    }

    private void Start()
    {
        this.OnGet();
    }

    [CompilerGenerated]
    private sealed class <ResetTrail>c__IteratorD : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal TrailRenderer <$>trail;
        internal float <trailTime>__0;
        internal TrailRenderer trail;

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
                    this.<trailTime>__0 = this.trail.time;
                    this.trail.time = 0f;
                    this.$current = 0;
                    this.$PC = 1;
                    return true;

                case 1:
                    this.trail.time = this.<trailTime>__0;
                    this.$PC = -1;
                    break;
            }
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
}

