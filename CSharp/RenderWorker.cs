using System;

public class RenderWorker : WorkerThreadBase<RenderWorker>
{
    protected override void _PrepareWorkerData()
    {
    }

    protected override void _Run()
    {
    }

    protected override void BeforeStart()
    {
        base.BeforeStart();
    }

    public void BeginLevel()
    {
        base.GetLock();
        try
        {
        }
        catch (Exception)
        {
        }
        finally
        {
            base.ReleaseLock();
        }
    }

    public void EndLevel()
    {
        base.GetLock();
        try
        {
        }
        catch (Exception)
        {
        }
        finally
        {
            base.ReleaseLock();
        }
    }
}

