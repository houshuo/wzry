using Assets.Scripts.Common;
using System;
using UnityEngine;

[Serializable]
public class Moba_Camera_Settings
{
    public Vector3 absoluteLockLocation;
    public bool cameraLocked;
    public PoolObjHandle<ActorRoot> lockTarget;
    public Moba_Camera_Settings_Movement movement = new Moba_Camera_Settings_Movement();
    public Moba_Camera_Settings_Rotation rotation = new Moba_Camera_Settings_Rotation();
    public float targetHeight;
    public bool useAbsoluteLock;
    public bool useBoundaries = true;
    public Moba_Camera_Settings_Zoom zoom = new Moba_Camera_Settings_Zoom();
}

