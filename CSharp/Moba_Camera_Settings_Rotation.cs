using System;
using UnityEngine;

[Serializable]
public class Moba_Camera_Settings_Rotation
{
    public Vector2 cameraRotationRate = new Vector2(100f, 100f);
    public bool constRotationRate;
    public Vector2 defualtRotation = new Vector2(-45f, 0f);
    public bool lockRotationX = true;
    public bool lockRotationY = true;
}

