namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using System;

    internal class NetLockAttackTarget : Singleton<NetLockAttackTarget>
    {
        public void SendLockAttackTarget(uint _targetID)
        {
            FrameCommand<LockAttackTargetCommand> command = FrameCommandFactory.CreateFrameCommand<LockAttackTargetCommand>();
            command.cmdData.LockAttackTarget = _targetID;
            command.Send();
        }
    }
}

