using System;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public int rotateDirection = 1;
    public int rotateSpeed;

    private void Update()
    {
        base.gameObject.transform.Rotate((Vector3) (((Vector3.forward * Time.deltaTime) * this.rotateSpeed) * this.rotateDirection));
    }
}

