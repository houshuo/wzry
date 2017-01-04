using System;
using UnityEngine;

public class AkTriggerCollisionExit : AkTriggerBase
{
    private void OnCollisionExit(Collision in_other)
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(in_other.gameObject);
        }
    }
}

