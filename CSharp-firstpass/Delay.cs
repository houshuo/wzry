using System;
using UnityEngine;

public class Delay : MonoBehaviour, IPooledMonoBehaviour
{
    private bool _started;
    public float delayTime = 1f;

    private void DelayFunc()
    {
        base.gameObject.SetActive(true);
    }

    private void DoStart()
    {
        base.gameObject.SetActive(false);
        base.Invoke("DelayFunc", this.delayTime);
        this._started = true;
    }

    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (!this._started)
        {
            this.DoStart();
        }
    }

    public void OnRecycle()
    {
        this._started = false;
    }

    private void Start()
    {
        if (!this._started)
        {
            this.DoStart();
        }
    }
}

