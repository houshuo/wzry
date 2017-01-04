using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

public class CameraSystem : MonoSingleton<CameraSystem>
{
    private bool bEnableCameraMovment;
    protected bool bFreeCamera;
    protected bool bFreeRotate;
    protected Plane[] CachedFrustum;
    private float CurrentSpeed;
    private float LastUpdateTime;
    public Moba_Camera MobaCamera;
    private static float s_CameraMoveScale = 0.02f;

    public bool CheckVisiblity(Bounds InBounds)
    {
        if (this.CachedFrustum != null)
        {
            return GeometryUtility.TestPlanesAABB(this.CachedFrustum, InBounds);
        }
        return true;
    }

    public void MoveCamera(float offX, float offY)
    {
        if (this.MobaCamera != null)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
            {
                offX = -offX;
                offY = -offY;
            }
            this.MobaCamera.settings.absoluteLockLocation.x += offX * s_CameraMoveScale;
            this.MobaCamera.settings.absoluteLockLocation.z += offY * s_CameraMoveScale;
        }
    }

    private void OnCameraHeightChanged()
    {
        if (this.MobaCamera != null)
        {
            this.MobaCamera.currentZoomRate = GameSettings.CameraHeightRateValue;
            this.MobaCamera.CameraUpdate();
        }
    }

    private void OnFocusSwitched(ref DefaultGameEventParam prm)
    {
        if ((prm.src != 0) && ActorHelper.IsHostCtrlActor(ref prm.src))
        {
            this.SetFocusActor(prm.src);
            if (!prm.src.handle.ActorControl.IsDeadState && !this.bFreeCamera)
            {
                this.enableLockedCamera = true;
                this.enableAbsoluteLocationLockCamera = false;
            }
        }
    }

    private void OnPlayerDead(ref GameDeadEventParam prm)
    {
        if (((prm.src != 0) && ActorHelper.IsHostCtrlActor(ref prm.src)) && (!this.bFreeCamera && !Singleton<WatchController>.GetInstance().IsWatching))
        {
            if ((this.MobaCamera != null) && (prm.src.handle.ActorControl != null))
            {
                this.MobaCamera.SetAbsoluteLockLocation((Vector3) prm.src.handle.ActorControl.actorLocation);
            }
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.EnableCameraDragPanelForDead();
            }
            this.enableLockedCamera = false;
            this.enableAbsoluteLocationLockCamera = true;
            this.StopDisplacement();
            if (this.MobaCamera != null)
            {
                this.MobaCamera._lockTransitionRate = 1f;
            }
        }
    }

    private void OnPlayerRevive(ref DefaultGameEventParam prm)
    {
        if (((prm.src != 0) && ActorHelper.IsHostCtrlActor(ref prm.src)) && (!this.bFreeCamera && !Singleton<WatchController>.GetInstance().IsWatching))
        {
            this.enableLockedCamera = true;
            this.enableAbsoluteLocationLockCamera = false;
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.DisableCameraDragPanelForRevive();
            }
        }
    }

    public void PrepareFight()
    {
        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
        DebugHelper.Assert(hostPlayer != null, "local player is null in CameraSystem.PerpareFight", null);
        PoolObjHandle<ActorRoot> focus = (hostPlayer == null) ? new PoolObjHandle<ActorRoot>() : hostPlayer.Captain;
        this.SetFocusActor(focus);
        if (this.MobaCamera != null)
        {
            this.MobaCamera.currentZoomRate = GameSettings.CameraHeightRateValue;
            this.MobaCamera.CameraUpdate();
        }
        Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnFocusSwitched));
        Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnPlayerDead));
        Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
        Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
        Singleton<GameEventSys>.instance.RmvEventHandler(GameEventDef.Event_CameraHeightChange, new Action(this.OnCameraHeightChanged));
        Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnFocusSwitched));
        Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnPlayerDead));
        Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
        Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, new RefAction<DefaultGameEventParam>(this.OnPlayerRevive));
        Singleton<GameEventSys>.instance.AddEventHandler(GameEventDef.Event_CameraHeightChange, new Action(this.OnCameraHeightChanged));
        this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
    }

    private void SetCameraLerp(int timerSequence)
    {
        if (this.MobaCamera != null)
        {
        }
    }

    public void SetFocusActor(PoolObjHandle<ActorRoot> focus)
    {
        if (this.MobaCamera == null)
        {
            GameObject obj2 = GameObject.Find("MainCamera");
            if (obj2 != null)
            {
                this.MobaCamera = obj2.GetComponent<Moba_Camera>();
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
                {
                    this.MobaCamera.settings.rotation.defualtRotation = new Vector2(this.MobaCamera.settings.rotation.defualtRotation.x, 180f);
                    this.MobaCamera.currentCameraRotation = (Vector3) this.MobaCamera.settings.rotation.defualtRotation;
                }
                this.MobaCamera.currentZoomRate = GameSettings.CameraHeightRateValue;
            }
        }
        if (this.MobaCamera != null)
        {
            this.MobaCamera.SetTargetTransform(focus);
            this.MobaCamera.SetCameraLocked(true);
        }
    }

    public void SetFocusActorForce(PoolObjHandle<ActorRoot> focus, float inNewZoomAmount)
    {
        if (this.MobaCamera != null)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.m_isCameraFlip)
            {
                this.MobaCamera.settings.rotation.defualtRotation = new Vector2(this.MobaCamera.settings.rotation.defualtRotation.x, 180f);
            }
            this.MobaCamera.currentCameraRotation = (Vector3) this.MobaCamera.settings.rotation.defualtRotation;
            this.MobaCamera.currentZoomRate = GameSettings.CameraHeightRateValue;
            this.MobaCamera.SetCameraZoom(inNewZoomAmount);
            this.MobaCamera.SetTargetTransform(focus);
            this.MobaCamera.SetCameraLocked(true);
        }
    }

    private void Start()
    {
    }

    private void StopDisplacement()
    {
        if (this.MobaCamera != null)
        {
            this.MobaCamera.StopCameraRelativeDisplacement();
        }
    }

    public void ToggleFreeCamera()
    {
        this.bFreeCamera = !this.bFreeCamera;
        this.enableLockedCamera = !this.bFreeCamera;
        this.enableAbsoluteLocationLockCamera = false;
    }

    public void ToggleFreeDragCamera(bool bFree)
    {
        this.enableLockedCamera = !bFree;
        this.enableAbsoluteLocationLockCamera = bFree;
    }

    public void ToggleRotate()
    {
        this.bFreeRotate = !this.bFreeRotate;
        this.MobaCamera.lockRotateX = !this.bFreeRotate;
        this.MobaCamera.lockRotateY = !this.bFreeRotate;
    }

    private void Update()
    {
        this.CachedFrustum = (this.MobaCamera == null) ? null : this.MobaCamera.frustum;
    }

    public void UpdateCameraMovement(ref Vector2 axis)
    {
        if (this.bEnableCameraMovment)
        {
            float b = Time.realtimeSinceStartup - this.LastUpdateTime;
            if ((MonoSingleton<GlobalConfig>.instance.bResetCameraSpeedWhenZero && (axis.x == 0f)) && (axis.y == 0f))
            {
                this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
            }
            float a = (MonoSingleton<GlobalConfig>.instance.CameraMoveSpeedMax - this.CurrentSpeed) / MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration;
            float num3 = Mathf.Min(a, b);
            this.CurrentSpeed += MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration * num3;
            Vector2 inOffset = (Vector2) (((axis.normalized * this.CurrentSpeed) * b) + ((((axis.normalized * 0.5f) * MonoSingleton<GlobalConfig>.instance.CameraMoveAcceleration) * num3) * num3));
            if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
            {
                inOffset = (Vector2) (inOffset * -1f);
            }
            this.LastUpdateTime = Time.realtimeSinceStartup;
            if (this.MobaCamera != null)
            {
                this.MobaCamera.UpdateCameraRelativeDisplacement(ref inOffset);
            }
        }
    }

    public void UpdatePanelCameraMovement(ref Vector2 InMovement)
    {
        if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
        {
            InMovement = (Vector2) (InMovement * -1f);
        }
        InMovement = (Vector2) (InMovement * MonoSingleton<GlobalConfig>.instance.PanelCameraMoveSpeed);
        if (this.MobaCamera != null)
        {
            this.MobaCamera.UpdateCameraRelativeDisplacement(ref InMovement);
        }
    }

    protected bool enableAbsoluteLocationLockCamera
    {
        get
        {
            return ((this.MobaCamera != null) && this.MobaCamera.GetAbsoluteLocked());
        }
        set
        {
            if (this.MobaCamera != null)
            {
                this.MobaCamera.SetAbsoluteLocked(value);
            }
        }
    }

    public bool enableCameraMovement
    {
        get
        {
            return this.bEnableCameraMovment;
        }
        set
        {
            this.bEnableCameraMovment = value;
            if (this.MobaCamera != null)
            {
                this.MobaCamera.SetEnableDisplacement(this.bEnableCameraMovment);
            }
            if (!this.bEnableCameraMovment)
            {
                this.StopDisplacement();
                this.CurrentSpeed = MonoSingleton<GlobalConfig>.instance.CameraMoveSpeed;
            }
            else
            {
                this.LastUpdateTime = Time.realtimeSinceStartup;
                if (this.MobaCamera != null)
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if ((hostPlayer != null) && (hostPlayer.Captain != 0))
                    {
                        Vector3 location = (Vector3) hostPlayer.Captain.handle.location;
                        this.MobaCamera.SetStartLocation(ref location);
                    }
                }
            }
        }
    }

    public bool enableLockedCamera
    {
        get
        {
            return ((this.MobaCamera != null) && this.MobaCamera.GetCameraLocked());
        }
        protected set
        {
            if (this.MobaCamera != null)
            {
                this.MobaCamera.SetCameraLocked(value);
            }
        }
    }
}

