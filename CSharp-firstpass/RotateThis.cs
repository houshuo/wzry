using System;
using UnityEngine;

public class RotateThis : MonoBehaviour, IPooledMonoBehaviour
{
    public float rotationSpeedX = 90f;
    public float rotationSpeedY;
    public float rotationSpeedZ;

    public void OnCreate()
    {
    }

    public void OnGet()
    {
    }

    public void OnRecycle()
    {
    }

    private void Update()
    {
        base.transform.Rotate((float) (this.rotationSpeedX * Time.deltaTime), this.rotationSpeedY * Time.deltaTime, (float) (this.rotationSpeedZ * Time.deltaTime));
    }
}

