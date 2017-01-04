using System;

public class AkTriggerMouseDown : AkTriggerBase
{
    private void OnMouseDown()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

