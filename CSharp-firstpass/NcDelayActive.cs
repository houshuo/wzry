using System;

public class NcDelayActive : NcEffectBehaviour
{
    public bool m_bActiveRecursively = true;
    protected bool m_bAddedInvoke;
    protected float m_fAliveTime;
    public float m_fDelayTime;
    public float m_fParentDelayTime;
    protected float m_fStartedTime;
    public string NotAvailable = "This component is not available.";

    public float GetParentDelayTime(bool bCheckStarted)
    {
        return 0f;
    }
}

