using System;
using UnityEngine;

public class AkTriggerEnter : AkTriggerBase
{
    private void OnTriggerEnter(Collider in_other)
    {
        if (base.triggerDelegate != null)
        {
            base.triggerDelegate(in_other.gameObject);
        }
    }
}

