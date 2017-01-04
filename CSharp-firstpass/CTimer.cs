using System;
using System.Runtime.CompilerServices;

public class CTimer
{
    private int m_currentTime;
    private bool m_isFinished;
    private bool m_isRunning;
    private int m_loop = 1;
    private int m_sequence;
    private OnTimeUpHandler m_timeUpHandler;
    private int m_totalTime;

    public CTimer(int time, int loop, OnTimeUpHandler timeUpHandler, int sequence)
    {
        if (loop == 0)
        {
            loop = -1;
        }
        this.m_totalTime = time;
        this.m_loop = loop;
        this.m_timeUpHandler = timeUpHandler;
        this.m_sequence = sequence;
        this.m_currentTime = 0;
        this.m_isRunning = true;
        this.m_isFinished = false;
    }

    public void Finish()
    {
        this.m_isFinished = true;
    }

    public int GetLeftTime()
    {
        return (this.m_totalTime - this.m_currentTime);
    }

    public bool IsDelegateMatched(OnTimeUpHandler timeUpHandler)
    {
        return (this.m_timeUpHandler == timeUpHandler);
    }

    public bool IsFinished()
    {
        return this.m_isFinished;
    }

    public bool IsSequenceMatched(int sequence)
    {
        return (this.m_sequence == sequence);
    }

    public void Pause()
    {
        this.m_isRunning = false;
    }

    public void Reset()
    {
        this.m_currentTime = 0;
    }

    public void ResetTotalTime(int totalTime)
    {
        if (this.m_totalTime != totalTime)
        {
            this.m_currentTime = 0;
            this.m_totalTime = totalTime;
        }
    }

    public void Resume()
    {
        this.m_isRunning = true;
    }

    public void Update(int deltaTime)
    {
        if (!this.m_isFinished && this.m_isRunning)
        {
            if (this.m_loop == 0)
            {
                this.m_isFinished = true;
            }
            else
            {
                this.m_currentTime += deltaTime;
                if (this.m_currentTime >= this.m_totalTime)
                {
                    if (this.m_timeUpHandler != null)
                    {
                        this.m_timeUpHandler(this.m_sequence);
                    }
                    this.m_currentTime = 0;
                    this.m_loop--;
                }
            }
        }
    }

    public int CurrentTime
    {
        get
        {
            return this.m_currentTime;
        }
    }

    public delegate void OnTimeUpHandler(int timerSequence);
}

