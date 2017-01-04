namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;

    internal class LockModeUISelector : Singleton<LockModeUISelector>
    {
        public void OnClickBattleUI(uint _targetID)
        {
            OperateMode defaultMode = OperateMode.DefaultMode;
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (hostPlayer != null)
            {
                defaultMode = hostPlayer.GetOperateMode();
            }
            if ((defaultMode != OperateMode.DefaultMode) && (_targetID != 0))
            {
                Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(_targetID);
            }
        }
    }
}

