using System;

public class AkTriggerEnable : AkTriggerBase
{
    private void OnEnable()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

