using System;
using System.Runtime.CompilerServices;
using System.Threading;

public class BackgroundWorker : Singleton<BackgroundWorker>
{
    private bool bRequestExit;
    private ListView<BackgroudDelegate> PendingWork = new ListView<BackgroudDelegate>();
    public int ThreadID;
    private ListView<BackgroudDelegate> WorkingList = new ListView<BackgroudDelegate>();
    private Thread WorkingThread;

    public void AddBackgroudOperation(BackgroudDelegate InDelegate)
    {
        ListView<BackgroudDelegate> pendingWork = this.PendingWork;
        lock (pendingWork)
        {
            this.PendingWork.Add(InDelegate);
        }
    }

    protected void Entry()
    {
        while (!this.bRequestExit)
        {
            ListView<BackgroudDelegate> pendingWork = this.PendingWork;
            lock (pendingWork)
            {
                Swap<ListView<BackgroudDelegate>>(ref this.PendingWork, ref this.WorkingList);
            }
            int count = this.WorkingList.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    this.WorkingList[i]();
                }
                catch (Exception)
                {
                }
            }
            this.WorkingList.Clear();
            Thread.Sleep(60);
        }
    }

    public override void Init()
    {
        this.WorkingThread = new Thread(new ThreadStart(BackgroundWorker.StaticEntry));
        this.ThreadID = this.WorkingThread.ManagedThreadId;
        this.WorkingThread.Start();
    }

    protected static void StaticEntry()
    {
        Singleton<BackgroundWorker>.instance.Entry();
    }

    private static void Swap<T>(ref T a, ref T b)
    {
        T local = a;
        a = b;
        b = local;
    }

    public override void UnInit()
    {
        this.bRequestExit = true;
        this.WorkingThread.Join();
        this.WorkingThread = null;
    }

    public delegate void BackgroudDelegate();
}

