namespace Assets.Scripts.GameLogic
{
    using System;

    public class ConstantMotionControler : SpecialMotionControler
    {
        public override int GetMotionDeltaDistance(int _deltaTime)
        {
            return ((base.motionSpeed * _deltaTime) / 0x3e8);
        }

        public override int GetMotionDistance(int _allTime)
        {
            return ((base.motionSpeed * _allTime) / 0x3e8);
        }

        public void InitMotionControler(int _motionSpeed)
        {
            base.motionSpeed = _motionSpeed;
        }
    }
}

