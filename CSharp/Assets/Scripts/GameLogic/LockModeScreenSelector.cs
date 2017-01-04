namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;
    using UnityEngine;

    internal class LockModeScreenSelector : Singleton<LockModeScreenSelector>
    {
        private Plane curPlane;
        private Ray screenRay;

        public override void Init()
        {
            this.curPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
        }

        public void OnClickBattleScene(Vector2 _screenPos)
        {
            OperateMode defaultMode = OperateMode.DefaultMode;
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (hostPlayer != null)
            {
                defaultMode = hostPlayer.GetOperateMode();
            }
            if (defaultMode != OperateMode.DefaultMode)
            {
                float num;
                uint num2 = 0;
                Ray ray = Camera.main.ScreenPointToRay((Vector3) _screenPos);
                Player player2 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (this.curPlane.Raycast(ray, out num))
                {
                    Vector3 point = ray.GetPoint(num);
                    if (player2 != null)
                    {
                        num2 = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref player2.Captain, (VInt3) point, 0xbb8);
                    }
                    if (num2 != 0)
                    {
                        Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(num2);
                    }
                }
            }
        }
    }
}

