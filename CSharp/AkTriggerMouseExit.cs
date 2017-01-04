using System;

public class AkTriggerMouseExit : AkTriggerBase
{
    private void OnMouseExit()
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(null);
        }
    }
}

