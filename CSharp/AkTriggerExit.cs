using System;
using UnityEngine;

public class AkTriggerExit : AkTriggerBase
{
    private void OnTriggerExit(Collider in_other)
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(in_other.gameObject);
        }
    }
}

