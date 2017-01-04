using System;

public abstract class BaseState : IState
{
    protected BaseState()
    {
    }

    public virtual void OnStateEnter()
    {
    }

    public virtual void OnStateLeave()
    {
    }

    public virtual void OnStateOverride()
    {
    }

    public virtual void OnStateResume()
    {
    }

    public virtual string name
    {
        get
        {
            return base.GetType().Name;
        }
    }
}

