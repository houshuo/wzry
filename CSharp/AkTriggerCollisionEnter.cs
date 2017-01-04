using System;
using UnityEngine;

public class AkTriggerCollisionEnter : AkTriggerBase
{
    private void OnCollisionEnter(Collision in_other)
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(in_other.gameObject);
        }
    }
}

