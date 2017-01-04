using System;

public class AkTriggerDisable : AkTriggerBase
{
    private void OnDisable()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

