namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameInput : Singleton<GameInput>
    {
        private bool bMessageBoxIsOpen;
        private bool bSmartUse;
        private int ConfirmDirSndFrame = -1;
        public static float DoubleTouchDeltaTime = 0.25f;
        public static int enemyExploreOptRadius = 0x7d0;
        private int FixtimeDirSndFrame = -1;
        private StateMachine inputMode = new StateMachine();
        private DateTime lastClickEscape = DateTime.Now;
        public static float minCurveTrackDistance = 60f;
        public static float minGuestureCircleRadian = 20f;
        private byte nDirMoveSeq;
        private int PreMoveDirection = 0x7fffffff;
        public static float UseDirectionSkillDistance = 80f;

        private VInt3 CalcDirectionByTouchPosition(Vector2 InFirst, Vector2 InSecond)
        {
            if (Camera.main != null)
            {
                Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(InFirst.x, InFirst.y, Camera.main.nearClipPlane));
                Vector3 vector4 = Camera.main.ScreenToWorldPoint(new Vector3(InSecond.x, InSecond.y, Camera.main.nearClipPlane)) - vector;
                return new VInt3(Vector3.ProjectOnPlane(vector4.normalized, new Vector3(0f, 1f, 0f)).normalized);
            }
            DebugHelper.Assert(false, "CalcDirectionByTouchPosition, Main camera is null");
            return VInt3.forward;
        }

        public void ChangeBattleMode(bool bBriefness)
        {
            this.inputMode.ChangeState("JoystickMode");
        }

        public void ChangeLobbyMode()
        {
            this.inputMode.ChangeState("LobbyInputMode");
        }

        public override void Init()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, new RefAction<DefaultGameEventParam>(this.OnHostActorClearMove));
            this.inputMode.RegisterState<LobbyInputMode>(new LobbyInputMode(this), "LobbyInputMode");
            this.inputMode.RegisterState<JoystickMode>(new JoystickMode(this), "JoystickMode");
            this.inputMode.ChangeState("LobbyInputMode");
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnQuitGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Quit_GameCancel, new CUIEventManager.OnUIEventHandler(this.OnQuitCameCancel));
            this.bSmartUse = false;
        }

        public bool IsSmartUse()
        {
            return this.bSmartUse;
        }

        private void OnHostActorClearMove(ref DefaultGameEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                this.PreMoveDirection = -2147483648;
            }
        }

        public void OnHostActorRecvMove(int nDegree)
        {
            if (nDegree == this.PreMoveDirection)
            {
                this.ConfirmDirSndFrame = -1;
            }
        }

        private void OnQuitCameCancel(CUIEvent uiEvent)
        {
            this.bMessageBoxIsOpen = false;
        }

        private void OnQuitGame(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void SendMoveDirection(int moveDegree, uint playerId = 0)
        {
            byte num;
            if (playerId == 0)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer == null)
                {
                    return;
                }
                playerId = hostPlayer.PlayerId;
            }
            this.PreMoveDirection = moveDegree;
            this.ConfirmDirSndFrame = 0;
            this.FixtimeDirSndFrame = 0;
            FrameCommand<MoveDirectionCommand> command = FrameCommandFactory.CreateFrameCommand<MoveDirectionCommand>();
            command.cmdData.Degree = (short) moveDegree;
            this.nDirMoveSeq = (byte) ((num = this.nDirMoveSeq) + 1);
            command.cmdData.nSeq = num;
            command.Send();
        }

        public void SendMoveDirection(Vector2 start, Vector2 end)
        {
            this.FixtimeDirSndFrame++;
            VInt3 num = this.CalcDirectionByTouchPosition(start, end);
            if (num != VInt3.zero)
            {
                int moveDegree = (int) (((double) (IntMath.atan2(-num.z, num.x).single * 180f)) / 3.1416);
                DebugHelper.Assert((moveDegree < 0x7fff) && (moveDegree > -32768), "向量转换成2pi空间超过范围了");
                int num3 = moveDegree - this.PreMoveDirection;
                if (((num3 > 1) || (num3 < -1)) || (this.FixtimeDirSndFrame > 30))
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        this.SendMoveDirection(moveDegree, hostPlayer.PlayerId);
                    }
                }
            }
        }

        public void SendStopMove(bool force = false)
        {
            if ((this.PreMoveDirection != 0x7fffffff) || force)
            {
                this.PreMoveDirection = 0x7fffffff;
                this.ConfirmDirSndFrame = 0;
                this.FixtimeDirSndFrame = 0;
                FrameCommandFactory.CreateFrameCommand<StopMoveCommand>().Send();
            }
        }

        public void SetSmartUse(bool _bUse)
        {
            this.bSmartUse = _bUse;
        }

        public void StopInput()
        {
            ((GameInputMode) this.inputMode.tarState).StopInput();
        }

        public void UpdateEscape()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TimeSpan span = (TimeSpan) (DateTime.Now - this.lastClickEscape);
                if (span.TotalMilliseconds < 1500.0)
                {
                    if (!this.bMessageBoxIsOpen)
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Is_QuitGame"), enUIEventID.Quit_Game, enUIEventID.Quit_GameCancel, false);
                        this.bMessageBoxIsOpen = true;
                    }
                }
                else
                {
                    this.lastClickEscape = DateTime.Now;
                }
            }
        }

        public void UpdateFrame()
        {
            if (this.inputMode.tarState != null)
            {
                ((GameInputMode) this.inputMode.tarState).Update();
            }
            if (((this.ConfirmDirSndFrame > 0) && (++this.ConfirmDirSndFrame > 8)) && (this.PreMoveDirection != -2147483648))
            {
                if (this.PreMoveDirection != 0x7fffffff)
                {
                    this.SendMoveDirection(this.PreMoveDirection, 0);
                }
                else
                {
                    this.SendStopMove(true);
                }
                this.ConfirmDirSndFrame = 0;
            }
            this.UpdateEscape();
        }
    }
}

