using Assets.Scripts.Common;
using System;
using UnityEngine;

public class Moba_Camera : MonoBehaviour
{
    private Vector2 _currentCameraRotation = Vector3.zero;
    private static Camera _currentMobaCamera;
    private float _currentZoomAmount;
    private float _currentZoomRate = 1f;
    public float _lockTransitionRate = 1f;
    private bool bEnableDisplacement;
    private Plane[] CachedPlanes = new Plane[6];
    private bool changeInCamera = true;
    private float deltaMouseDeadZone = 0.2f;
    public Moba_Camera_Inputs inputs = new Moba_Camera_Inputs();
    private const float MAXROTATIONXAXIS = 89f;
    private const float MINROTATIONXAXIS = -89f;
    private Vector2 PreRelativeDisplacement;
    private Vector2 RelativeDisplacement;
    public Moba_Camera_Requirements requirements = new Moba_Camera_Requirements();
    public Moba_Camera_Settings settings = new Moba_Camera_Settings();
    private Vector3 StartLocation;
    public bool useFixedUpdate;

    private void Awake()
    {
        _currentMobaCamera = this.requirements.camera;
    }

    private Plane[] CalcFrustum(Camera InCamera)
    {
        GeometryUtilityUser.CalculateFrustumPlanes(InCamera, ref this.CachedPlanes);
        return this.CachedPlanes;
    }

    private void CalculateCameraBoundaries()
    {
        if ((this.settings.useBoundaries && !(!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight))) && !Moba_Camera_Boundaries.isPointInBoundary(this.requirements.pivot.position))
        {
            Moba_Camera_Boundary closestBoundary = Moba_Camera_Boundaries.GetClosestBoundary(this.requirements.pivot.position);
            if (closestBoundary != null)
            {
                this.requirements.pivot.position = Moba_Camera_Boundaries.GetClosestPointOnBoundary(closestBoundary, this.requirements.pivot.position);
                this.RelativeDisplacement = this.PreRelativeDisplacement;
            }
        }
    }

    private void CalculateCameraMovement()
    {
        DebugHelper.Assert((this.requirements != null) && (this.requirements.pivot != null), "requirements != null && requirements.pivot!=null", null);
        if ((this.requirements != null) && (this.requirements.pivot != null))
        {
            DebugHelper.Assert(this.settings != null, "settings is null", null);
            if (((!this.inputs.useKeyCodeInputs ? (!Input.GetButtonDown(this.inputs.axis.button_lock_camera) ? 0 : this.settings.lockTarget) : Input.GetKeyDown(this.inputs.keycodes.LockCamera)) != 0) && (this.settings.lockTarget != 0))
            {
                this.settings.cameraLocked = !this.settings.cameraLocked;
            }
            if (this.settings.useAbsoluteLock)
            {
                Vector3 absoluteLockLocation = this.settings.absoluteLockLocation;
                absoluteLockLocation.y += this.settings.targetHeight;
                Vector3 vector7 = this.requirements.pivot.position - absoluteLockLocation;
                if (vector7.magnitude > 0.001f)
                {
                    if (this.settings.movement.useDefualtHeight && !this.settings.movement.useLockTargetHeight)
                    {
                        absoluteLockLocation.y = this.settings.movement.defualtHeight;
                    }
                    else if (!this.settings.movement.useLockTargetHeight)
                    {
                        absoluteLockLocation.y = this.requirements.pivot.position.y;
                    }
                    this.requirements.pivot.position = Vector3.Lerp(this.requirements.pivot.position, absoluteLockLocation, this.settings.movement.lockTransitionRate);
                }
            }
            else if ((this.settings.lockTarget != 0) && (this.settings.cameraLocked || (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_char_focus) : Input.GetKey(this.inputs.keycodes.characterFocus))))
            {
                Vector3 position = this.settings.lockTarget.handle.gameObject.transform.position;
                position.y += this.settings.targetHeight;
                bool bComposePlayerMovement = MonoSingleton<GlobalConfig>.instance.bComposePlayerMovement;
                Vector3 vector9 = this.requirements.pivot.position - position;
                if (vector9.magnitude > 0.001f)
                {
                    float x;
                    float z;
                    if (this.settings.movement.useDefualtHeight && !this.settings.movement.useLockTargetHeight)
                    {
                        position.y = this.settings.movement.defualtHeight;
                    }
                    else if (!this.settings.movement.useLockTargetHeight)
                    {
                        position.y = this.requirements.pivot.position.y;
                    }
                    this.requirements.pivot.position = Vector3.Lerp(this.requirements.pivot.position, position, this._lockTransitionRate);
                    if (this.bEnableDisplacement)
                    {
                        x = (!bComposePlayerMovement ? this.StartLocation.x : this.requirements.pivot.position.x) + (this.RelativeDisplacement.x / 1000f);
                        z = (!bComposePlayerMovement ? this.StartLocation.z : this.requirements.pivot.position.z) + (this.RelativeDisplacement.y / 1000f);
                    }
                    else
                    {
                        x = this.requirements.pivot.position.x;
                        z = this.requirements.pivot.position.z;
                    }
                    this.requirements.pivot.position = new Vector3(x, this.requirements.pivot.position.y, z);
                }
                else if ((this.RelativeDisplacement.x != 0f) || (this.RelativeDisplacement.y != 0f))
                {
                    float num4;
                    float num5;
                    if (this.bEnableDisplacement)
                    {
                        num4 = (!bComposePlayerMovement ? this.StartLocation.x : this.requirements.pivot.position.x) + (this.RelativeDisplacement.x / 1000f);
                        num5 = (!bComposePlayerMovement ? this.StartLocation.z : this.requirements.pivot.position.z) + (this.RelativeDisplacement.y / 1000f);
                    }
                    else
                    {
                        num4 = this.requirements.pivot.position.x;
                        num5 = this.requirements.pivot.position.z;
                    }
                    this.requirements.pivot.position = new Vector3(num4, this.requirements.pivot.position.y, num5);
                }
            }
            else
            {
                Vector3 vector3 = new Vector3(0f, 0f, 0f);
                if (((Input.mousePosition.x < this.settings.movement.edgeHoverOffset) && this.settings.movement.edgeHoverMovement) || (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_camera_move_left) : Input.GetKey(this.inputs.keycodes.CameraMoveLeft)))
                {
                    vector3 -= this.requirements.pivot.transform.right;
                }
                if (((Input.mousePosition.x > (Screen.width - this.settings.movement.edgeHoverOffset)) && this.settings.movement.edgeHoverMovement) || (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_camera_move_right) : Input.GetKey(this.inputs.keycodes.CameraMoveRight)))
                {
                    vector3 += this.requirements.pivot.transform.right;
                }
                if (((Input.mousePosition.y < this.settings.movement.edgeHoverOffset) && this.settings.movement.edgeHoverMovement) || (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_camera_move_backward) : Input.GetKey(this.inputs.keycodes.CameraMoveBackward)))
                {
                    vector3 -= this.requirements.pivot.transform.forward;
                }
                if (((Input.mousePosition.y > (Screen.height - this.settings.movement.edgeHoverOffset)) && this.settings.movement.edgeHoverMovement) || (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_camera_move_forward) : Input.GetKey(this.inputs.keycodes.CameraMoveForward)))
                {
                    vector3 += this.requirements.pivot.transform.forward;
                }
                this.requirements.pivot.position += (Vector3) ((vector3.normalized * this.settings.movement.cameraMovementRate) * Time.deltaTime);
                Vector3 zero = Vector3.zero;
                Vector3 a = new Vector3(0f, this.requirements.pivot.position.y, 0f);
                if (this.settings.movement.useDefualtHeight)
                {
                    zero.y = this.settings.movement.defualtHeight;
                }
                else
                {
                    zero.y = this.requirements.pivot.position.y;
                }
                Vector3 vector27 = zero - a;
                if (vector27.magnitude > 0.001f)
                {
                    Vector3 vector6 = Vector3.Lerp(a, zero, this.settings.movement.lockTransitionRate);
                    this.requirements.pivot.position = new Vector3(this.requirements.pivot.position.x, vector6.y, this.requirements.pivot.position.z);
                }
            }
        }
    }

    private void CalculateCameraRotation()
    {
        float num = 0f;
        float num2 = 0f;
        Screen.lockCursor = false;
        if (!this.inputs.useKeyCodeInputs ? Input.GetButton(this.inputs.axis.button_rotate_camera) : (Input.GetKey(this.inputs.keycodes.RotateCamera) && this.inputs.useKeyCodeInputs))
        {
            Screen.lockCursor = true;
            if (!this.settings.rotation.lockRotationX)
            {
                float axis = Input.GetAxis(this.inputs.axis.DeltaMouseVertical);
                if (axis != 0.0)
                {
                    if (this.settings.rotation.constRotationRate)
                    {
                        if (axis > this.deltaMouseDeadZone)
                        {
                            num = 1f;
                        }
                        else if (axis < -this.deltaMouseDeadZone)
                        {
                            num = -1f;
                        }
                    }
                    else
                    {
                        num = axis;
                    }
                    this.changeInCamera = true;
                }
            }
            if (!this.settings.rotation.lockRotationY)
            {
                float num4 = Input.GetAxis(this.inputs.axis.DeltaMouseHorizontal);
                if (num4 != 0f)
                {
                    if (this.settings.rotation.constRotationRate)
                    {
                        if (num4 > this.deltaMouseDeadZone)
                        {
                            num2 = 1f;
                        }
                        else if (num4 < -this.deltaMouseDeadZone)
                        {
                            num2 = -1f;
                        }
                    }
                    else
                    {
                        num2 = -1f * num4;
                    }
                    this.changeInCamera = true;
                }
            }
        }
        this._currentCameraRotation.y += (num2 * this.settings.rotation.cameraRotationRate.y) * Time.deltaTime;
        this._currentCameraRotation.x += (num * this.settings.rotation.cameraRotationRate.x) * Time.deltaTime;
    }

    private void CalculateCameraUpdates()
    {
        if (this.changeInCamera)
        {
            if (this.settings.zoom.maxZoom < this.settings.zoom.minZoom)
            {
                this.settings.zoom.maxZoom = this.settings.zoom.minZoom + 1f;
            }
            if (this._currentZoomAmount < this.settings.zoom.minZoom)
            {
                this._currentZoomAmount = this.settings.zoom.minZoom;
            }
            if (this._currentZoomAmount > this.settings.zoom.maxZoom)
            {
                this._currentZoomAmount = this.settings.zoom.maxZoom;
            }
            if (this._currentCameraRotation.x > 89f)
            {
                this._currentCameraRotation.x = 89f;
            }
            else if (this._currentCameraRotation.x < -89f)
            {
                this._currentCameraRotation.x = -89f;
            }
            Vector3 forward = (Vector3) (Quaternion.AngleAxis(this._currentCameraRotation.y, Vector3.up) * Vector3.forward);
            this.requirements.pivot.transform.rotation = Quaternion.LookRotation(forward);
            Vector3 vector2 = this.requirements.pivot.transform.TransformDirection(Vector3.forward);
            vector2 = (Vector3) (Quaternion.AngleAxis(this._currentCameraRotation.x, this.requirements.pivot.transform.TransformDirection(Vector3.right)) * vector2);
            this.requirements.offset.position = ((Vector3) ((-vector2 * this._currentZoomAmount) * this._currentZoomRate)) + this.requirements.pivot.position;
            this.requirements.offset.transform.LookAt(this.requirements.pivot);
            this.changeInCamera = false;
        }
    }

    private void CalculateCameraZoom()
    {
        float num = 0f;
        int num2 = 1;
        float axis = Input.GetAxis(this.inputs.axis.DeltaScrollWheel);
        if (axis != 0f)
        {
            this.changeInCamera = true;
            if (this.settings.zoom.constZoomRate)
            {
                if (axis != 0.0)
                {
                    if (axis > 0.0)
                    {
                        num = 1f;
                    }
                    else
                    {
                        num = -1f;
                    }
                }
            }
            else
            {
                num = axis;
            }
        }
        if (!this.settings.zoom.invertZoom)
        {
            num2 = -1;
        }
        this._currentZoomAmount += ((num * this.settings.zoom.zoomRate) * num2) * Time.deltaTime;
    }

    public void CameraUpdate()
    {
        this.CalculateCameraZoom();
        this.CalculateCameraRotation();
        this.CalculateCameraMovement();
        this.CalculateCameraUpdates();
        this.CalculateCameraBoundaries();
    }

    private void FixedUpdate()
    {
        if (this.useFixedUpdate)
        {
            this.CameraUpdate();
        }
    }

    public bool GetAbsoluteLocked()
    {
        return this.settings.useAbsoluteLock;
    }

    public Camera GetCamera()
    {
        return this.requirements.camera;
    }

    public bool GetCameraLocked()
    {
        return this.settings.cameraLocked;
    }

    private static bool IsEqual(float InFirst, float InSecond)
    {
        return (Math.Abs((float) (InFirst - InSecond)) < 0.001f);
    }

    private static bool IsEqual(Plane InFirst, Plane InSecond)
    {
        return (IsEqual(InFirst.normal, InSecond.normal) && IsEqual(InFirst.distance, InSecond.distance));
    }

    private static bool IsEqual(Vector3 InFirst, Vector3 InSecond)
    {
        return ((IsEqual(InFirst.x, InSecond.x) && IsEqual(InFirst.y, InSecond.y)) && IsEqual(InFirst.y, InSecond.y));
    }

    private void OnDestroy()
    {
        _currentMobaCamera = null;
    }

    public void SetAbsoluteLocked(bool locked)
    {
        this.settings.useAbsoluteLock = locked;
    }

    public void SetAbsoluteLockLocation(Vector3 pos)
    {
        this.settings.absoluteLockLocation = pos;
    }

    public void SetCameraLocked(bool locked)
    {
        this.settings.cameraLocked = locked;
    }

    public void SetCameraRotation(Vector2 rotation)
    {
        this.currentCameraRotation = (Vector3) new Vector2(rotation.x, rotation.y);
    }

    public void SetCameraRotation(float x, float y)
    {
        this.currentCameraRotation = (Vector3) new Vector2(x, y);
    }

    public void SetCameraZoom(float amount)
    {
        this.currentZoomAmount = amount;
    }

    public void SetEnableDisplacement(bool bInEnableDisplacement)
    {
        this.bEnableDisplacement = bInEnableDisplacement;
    }

    public void SetStartLocation(ref Vector3 InLocation)
    {
        this.StartLocation = InLocation;
    }

    public void SetTargetTransform(PoolObjHandle<ActorRoot> t)
    {
        DebugHelper.Assert((t != 0) && (t.handle.CharInfo != null), "invalid parameter for SetTargetTransform", null);
        if ((t != 0) && (t.handle.CharInfo != null))
        {
            this.settings.lockTarget = t;
            this.settings.targetHeight = t.handle.CharInfo.iBulletHeight * 0.001f;
        }
    }

    private void Start()
    {
        if (((this.requirements.pivot == null) || (this.requirements.offset == null)) || (this.requirements.camera == null))
        {
            string str = string.Empty;
            if (this.requirements.pivot == null)
            {
                str = str + " / Pivot";
                base.enabled = false;
            }
            if (this.requirements.offset == null)
            {
                str = str + " / Offset";
                base.enabled = false;
            }
            if (this.requirements.camera == null)
            {
                str = str + " / Camera";
                base.enabled = false;
            }
        }
        this._currentZoomAmount = this.settings.zoom.defaultZoom;
        this._currentCameraRotation = this.settings.rotation.defualtRotation;
        if (this.settings.movement.useDefualtHeight && base.enabled)
        {
            DebugHelper.Assert((this.requirements.pivot != null) && (this.requirements.pivot.transform != null), null, null);
            if (((this.requirements != null) && (this.requirements.pivot != null)) && (this.requirements.pivot.transform != null))
            {
                Vector3 position = this.requirements.pivot.transform.position;
                position.y = this.settings.movement.defualtHeight;
                this.requirements.pivot.transform.position = position;
            }
        }
        if (this.requirements.camera != null)
        {
            string[] layerNames = new string[] { "Hide" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(layerNames);
            string[] textArray2 = new string[] { "UI" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray2);
            string[] textArray3 = new string[] { "UIRaw" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray3);
            string[] textArray4 = new string[] { "UI_Background" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray4);
            string[] textArray5 = new string[] { "UI_Foreground" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray5);
            string[] textArray6 = new string[] { "UI_BottomBG" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray6);
            string[] textArray7 = new string[] { "3DUI" };
            this.requirements.camera.cullingMask &= ~LayerMask.GetMask(textArray7);
        }
        Singleton<Camera_UI3D>.GetInstance().Reset();
    }

    public void StopCameraRelativeDisplacement()
    {
        this.RelativeDisplacement = this.PreRelativeDisplacement = new Vector2();
    }

    private void Update()
    {
        if (!this.useFixedUpdate)
        {
            this.CameraUpdate();
        }
    }

    public void UpdateCameraRelativeDisplacement(ref Vector2 InOffset)
    {
        this.PreRelativeDisplacement = this.RelativeDisplacement;
        this.RelativeDisplacement += InOffset;
    }

    public Vector3 currentCameraRotation
    {
        get
        {
            return (Vector3) this._currentCameraRotation;
        }
        set
        {
            this._currentCameraRotation = value;
            this.changeInCamera = true;
        }
    }

    public static Camera currentMobaCamera
    {
        get
        {
            return _currentMobaCamera;
        }
    }

    public float currentZoomAmount
    {
        get
        {
            return this._currentZoomAmount;
        }
        set
        {
            this._currentZoomAmount = value;
            this.changeInCamera = true;
        }
    }

    public float currentZoomRate
    {
        get
        {
            return this._currentZoomRate;
        }
        set
        {
            this._currentZoomRate = value;
            this.changeInCamera = true;
        }
    }

    public Plane[] frustum
    {
        get
        {
            return (((this.requirements == null) || (this.requirements.camera == null)) ? null : this.CalcFrustum(this.requirements.camera));
        }
    }

    public bool lockRotateX
    {
        get
        {
            return this.settings.rotation.lockRotationX;
        }
        set
        {
            this.settings.rotation.lockRotationX = value;
        }
    }

    public bool lockRotateY
    {
        get
        {
            return this.settings.rotation.lockRotationY;
        }
        set
        {
            this.settings.rotation.lockRotationY = value;
        }
    }
}

