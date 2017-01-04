using System;
using System.Threading;

public abstract class WorkerThreadBase<T> : Singleton<T> where T: class, new()
{
    private bool m_isWorkDataPrepared;
    private bool m_isWorkFinished;
    private object m_lock;

    protected WorkerThreadBase()
    {
        this.m_lock = new object();
        this.m_isWorkDataPrepared = true;
        this.m_isWorkFinished = true;
    }

    private void _DoStart()
    {
        while (true)
        {
            Monitor.Enter(this.m_lock);
            while (!this.m_isWorkDataPrepared)
            {
                Monitor.PulseAll(this.m_lock);
                Monitor.Wait(this.m_lock);
            }
            try
            {
                this._Run();
            }
            catch (Exception)
            {
            }
            this.m_isWorkFinished = true;
            this.m_isWorkDataPrepared = false;
            Monitor.PulseAll(this.m_lock);
            Monitor.Exit(this.m_lock);
        }
    }

    protected abstract void _PrepareWorkerData();
    protected abstract void _Run();
    protected virtual void BeforeStart()
    {
    }

    protected void GetLock()
    {
        Monitor.Enter(this.m_lock);
    }

    public void PrepareWorkerData()
    {
        Monitor.Enter(this.m_lock);
        try
        {
            this._PrepareWorkerData();
        }
        catch (Exception)
        {
        }
        this.m_isWorkDataPrepared = true;
        Monitor.PulseAll(this.m_lock);
        Monitor.Exit(this.m_lock);
    }

    protected void ReleaseLock()
    {
        Monitor.PulseAll(this.m_lock);
        Monitor.Exit(this.m_lock);
    }

    public void Start()
    {
        this.BeforeStart();
        new Thread(new ThreadStart(this._DoStart)).Start();
    }

    public void WaitWorkerThread()
    {
        Monitor.Enter(this.m_lock);
        while (!this.m_isWorkFinished)
        {
            Monitor.PulseAll(this.m_lock);
            Monitor.Wait(this.m_lock);
        }
        this.m_isWorkFinished = false;
        Monitor.PulseAll(this.m_lock);
        Monitor.Exit(this.m_lock);
    }
}

