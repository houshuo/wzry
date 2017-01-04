using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider)), RequireComponent(typeof(Rigidbody)), AddComponentMenu("Wwise/AkEnvironmentPortal"), ExecuteInEditMode]
public class AkEnvironmentPortal : MonoBehaviour
{
    public Vector3 axis = new Vector3(1f, 0f, 0f);
    public AkEnvironment[] environments = new AkEnvironment[2];

    public float GetAuxSendValueForPosition(Vector3 in_position, int index)
    {
        float num = Vector3.Dot(Vector3.Scale(base.GetComponent<BoxCollider>().size, base.transform.lossyScale), this.axis);
        Vector3 rhs = Vector3.Normalize((Vector3) (base.transform.rotation * this.axis));
        float num2 = Vector3.Dot(in_position - (base.transform.position - ((Vector3) ((num * 0.5f) * rhs))), rhs);
        if (index == 0)
        {
            return (((num - num2) * (num - num2)) / (num * num));
        }
        return ((num2 * num2) / (num * num));
    }
}

