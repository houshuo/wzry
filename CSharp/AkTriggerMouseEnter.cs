using System;

public class AkTriggerMouseEnter : AkTriggerBase
{
    private void OnMouseEnter()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

