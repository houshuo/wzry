using System;

public class AkTriggerMouseUp : AkTriggerBase
{
    private void OnMouseUp()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

