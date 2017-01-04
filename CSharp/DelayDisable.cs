using System;
using UnityEngine;

public class DelayDisable : MonoBehaviour, IPooledMonoBehaviour
{
    public float m_disableDelaySecond = 5f;
    private bool m_done;
    private bool m_initEnableState;
    private bool m_started;
    private float m_startTime;

    public void Awake()
    {
        this.m_initEnableState = base.gameObject.activeSelf;
    }

    private void DoStart()
    {
        base.gameObject.SetActive(this.m_initEnableState);
        this.m_done = false;
        this.m_startTime = Time.realtimeSinceStartup;
        this.m_started = true;
    }

    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (!this.m_started)
        {
            this.DoStart();
        }
    }

    public void OnRecycle()
    {
    }

    private void Start()
    {
        if (!this.m_started)
        {
            this.DoStart();
        }
    }

    private void Update()
    {
        if (!this.m_done && ((Time.realtimeSinceStartup - this.m_startTime) > this.m_disableDelaySecond))
        {
            base.gameObject.CustomSetActive(false);
            this.m_done = true;
        }
    }
}

