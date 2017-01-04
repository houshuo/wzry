namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    public sealed class JoystickMode : GameInputMode
    {
        public int[] m_buttonStates;
        private Vector2 m_cameraAxisFromUI;
        public Vector2 m_dpad;
        public int m_dpadState;
        public Vector2 m_leftAxis;
        private Vector2 m_leftAxisFromUI;
        private enInputMode m_leftAxisInputMode;
        public int m_leftAxisState;
        public float m_leftTrigger;
        public int m_leftTriggerState;
        public Vector2 m_rightAxis;
        public int m_rightAxisState;
        public float m_rightTrigger;
        public int m_rightTriggerState;
        private SkillSlotType m_selectedSkillSlot;
        public static string[] s_joystickButtons = new string[] { "JoystickButtonX", "JoystickButtonY", "JoystickButtonA", "JoystickButtonB", "JoystickButtonL", "JoystickButtonR", "JoystickButtonSelect", "JoystickButtonStart", "JoystickButtonL3", "JoystickButtonR3" };
        public static string s_joystickDPadHorizontal = "JoystickDPadHorizontal";
        public static string s_joystickDPadVertical = "JoystickDPadVertical";
        public static string s_joystickLeftAxisHorizontal = "JoystickLeftAxisHorizontal";
        public static string s_joystickLeftAxisVertical = "JoystickLeftAxisVertical";
        public static string s_joystickLeftTrigger = "JoystickLeftTrigger";
        public static string s_joystickRightAxisHorizontal = "JoystickRightAxisHorizontal";
        public static string s_joystickRightAxisVertical = "JoystickRightAxisVertical";
        public static string s_joystickRightTrigger = "JoystickRightTrigger";

        public JoystickMode(GameInput InSys) : base(InSys)
        {
            this.m_buttonStates = new int[s_joystickButtons.Length];
            this.m_selectedSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
        }

        private void HandleCameraMoveInput(ref Vector2 axis)
        {
            MonoSingleton<CameraSystem>.instance.UpdateCameraMovement(ref axis);
        }

        private void HandleMoveInput(Vector2 axis, enInputMode inputMode)
        {
            if (axis != Vector2.zero)
            {
                if (((inputMode == enInputMode.UI) || (inputMode == this.m_leftAxisInputMode)) || (this.m_leftAxisInputMode == enInputMode.None))
                {
                    this.m_leftAxisInputMode = inputMode;
                    Singleton<GameInput>.GetInstance().SendMoveDirection(Vector2.zero, axis);
                }
            }
            else if (inputMode == this.m_leftAxisInputMode)
            {
                this.m_leftAxisInputMode = enInputMode.None;
                Singleton<GameInput>.GetInstance().SendStopMove(false);
            }
        }

        private void HandleUIInput()
        {
            this.HandleMoveInput(this.m_leftAxisFromUI, enInputMode.UI);
            this.HandleCameraMoveInput(ref this.m_cameraAxisFromUI);
        }

        private void OnCameraAxisChanged(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && ((hostPlayer.Captain.handle.ActorControl != null) && !hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                CUIJoystickScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIJoystickScript;
                if (srcWidgetScript != null)
                {
                    this.m_cameraAxisFromUI = srcWidgetScript.GetAxis();
                }
            }
        }

        private void OnCameraAxisPushed(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.instance.DispatchUIEvent(enUIEventID.Battle_CloseBigMap);
            MonoSingleton<CameraSystem>.instance.enableCameraMovement = true;
        }

        private void OnCameraAxisReleased(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.instance.enableCameraMovement = false;
        }

        private void OnLeftAxisChanged(CUIEvent uiEvent)
        {
            CUIJoystickScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIJoystickScript;
            if (srcWidgetScript != null)
            {
                this.m_leftAxisFromUI = srcWidgetScript.GetAxis();
            }
        }

        private void OnPanelCameraDraging(CUIEvent uiEvent)
        {
            Vector2 inMovement = new Vector2(uiEvent.m_pointerEventData.delta.x, uiEvent.m_pointerEventData.delta.y);
            MonoSingleton<CameraSystem>.instance.UpdatePanelCameraMovement(ref inMovement);
            Vector3 position = MonoSingleton<CameraSystem>.instance.MobaCamera.requirements.pivot.position;
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.m_miniMapCameraFrame != null))
            {
                if (!theMinimapSys.m_miniMapCameraFrame.IsCameraFrameShow)
                {
                    theMinimapSys.m_miniMapCameraFrame.Show();
                    theMinimapSys.m_miniMapCameraFrame.ShowNormal();
                }
                theMinimapSys.m_miniMapCameraFrame.SetPos(position.x * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.x, position.z * Singleton<CBattleSystem>.instance.world_UI_Factor_Small.y);
            }
        }

        private void OnPanelCameraEndDrag(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.instance.enableCameraMovement = false;
            MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
            if ((theMinimapSys != null) && (theMinimapSys.m_miniMapCameraFrame != null))
            {
                theMinimapSys.m_miniMapCameraFrame.Hide();
            }
        }

        private void OnPanelCameraStartDrag(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.instance.enableCameraMovement = true;
        }

        public override void OnStateEnter()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnLeftAxisChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisReleased));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisPushed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraStartDrag));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraDraging, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraDraging));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnPanelCameraEndDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraEndDrag));
        }

        public override void OnStateLeave()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnLeftAxisChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisChanged, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisReleased, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisReleased));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCameraAxisPushed, new CUIEventManager.OnUIEventHandler(this.OnCameraAxisPushed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraStartDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraStartDrag));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraDraging, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraDraging));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnPanelCameraEndDrag, new CUIEventManager.OnUIEventHandler(this.OnPanelCameraEndDrag));
        }

        public override void Update()
        {
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                this.HandleUIInput();
            }
        }

        public enum enInputMode
        {
            None,
            UI,
            Joystick
        }

        public enum enJoystickButtonState
        {
            None,
            Down,
            Hold,
            Up
        }

        public enum enJoystickInput
        {
            LeftAxis,
            RightAxis,
            LeftTrigger,
            RightTrigger,
            DPad,
            ButtonX,
            ButtonY,
            ButtonA,
            ButtonB,
            ButtonL1,
            ButtonR1,
            ButtonSelect,
            ButtonStart,
            ButtonL3,
            ButtonR3
        }
    }
}

