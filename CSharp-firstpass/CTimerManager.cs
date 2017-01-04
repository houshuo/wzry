using System;
using System.Collections.Generic;
using UnityEngine;

public class CTimerManager : Singleton<CTimerManager>
{
    private List<CTimer>[] m_timers;
    private int m_timerSequence;

    public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler)
    {
        return this.AddTimer(time, loop, onTimeUpHandler, false);
    }

    public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
    {
        this.m_timerSequence++;
        this.m_timers[!useFrameSync ? 0 : 1].Add(new CTimer(time, loop, onTimeUpHandler, this.m_timerSequence));
        return this.m_timerSequence;
    }

    public int GetLeftTime(int sequence)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            return (timer.GetLeftTime() / 0x3e8);
        }
        return -1;
    }

    public CTimer GetTimer(int sequence)
    {
        for (int i = 0; i < this.m_timers.Length; i++)
        {
            List<CTimer> list = this.m_timers[i];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].IsSequenceMatched(sequence))
                {
                    return list[j];
                }
            }
        }
        return null;
    }

    public int GetTimerCurrent(int sequence)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            return timer.CurrentTime;
        }
        return -1;
    }

    public override void Init()
    {
        this.m_timers = new List<CTimer>[Enum.GetValues(typeof(enTimerType)).Length];
        for (int i = 0; i < this.m_timers.Length; i++)
        {
            this.m_timers[i] = new List<CTimer>();
        }
        this.m_timerSequence = 0;
    }

    public void PauseTimer(int sequence)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            timer.Pause();
        }
    }

    public void RemoveAllTimer()
    {
        for (int i = 0; i < this.m_timers.Length; i++)
        {
            this.m_timers[i].Clear();
        }
    }

    public void RemoveAllTimer(bool useFrameSync)
    {
        this.m_timers[!useFrameSync ? 0 : 1].Clear();
    }

    public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler)
    {
        this.RemoveTimer(onTimeUpHandler, false);
    }

    public void RemoveTimer(int sequence)
    {
        for (int i = 0; i < this.m_timers.Length; i++)
        {
            List<CTimer> list = this.m_timers[i];
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].IsSequenceMatched(sequence))
                {
                    list[j].Finish();
                    return;
                }
            }
        }
    }

    public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync)
    {
        List<CTimer> list = this.m_timers[!useFrameSync ? 0 : 1];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsDelegateMatched(onTimeUpHandler))
            {
                list[i].Finish();
            }
        }
    }

    public void RemoveTimerSafely(ref int sequence)
    {
        if (sequence != 0)
        {
            this.RemoveTimer(sequence);
            sequence = 0;
        }
    }

    public void ResetTimer(int sequence)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            timer.Reset();
        }
    }

    public void ResetTimerTotalTime(int sequence, int totalTime)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            timer.ResetTotalTime(totalTime);
        }
    }

    public void ResumeTimer(int sequence)
    {
        CTimer timer = this.GetTimer(sequence);
        if (timer != null)
        {
            timer.Resume();
        }
    }

    public void Update()
    {
        this.UpdateTimer((int) (Time.deltaTime * 1000f), enTimerType.Normal);
    }

    public void UpdateLogic(int delta)
    {
        this.UpdateTimer(delta, enTimerType.FrameSync);
    }

    private void UpdateTimer(int delta, enTimerType timerType)
    {
        List<CTimer> list = this.m_timers[(int) timerType];
        int index = 0;
        while (index < list.Count)
        {
            if (list[index].IsFinished())
            {
                list.RemoveAt(index);
            }
            else
            {
                list[index].Update(delta);
                index++;
            }
        }
    }

    private enum enTimerType
    {
        Normal,
        FrameSync
    }
}

