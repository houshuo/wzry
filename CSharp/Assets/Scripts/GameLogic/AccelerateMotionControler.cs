namespace Assets.Scripts.GameLogic
{
    using System;

    public class AccelerateMotionControler : SpecialMotionControler
    {
        private int accelerateSpeed;
        private int curTime;
        private int lastDistance;
        private int lastLerpDistance;
        private int lerpCurTime;

        public override int GetMotionDeltaDistance(int _deltaTime)
        {
            int num2 = 0;
            this.curTime += _deltaTime;
            int motionDistance = this.GetMotionDistance(this.curTime);
            num2 = motionDistance - this.lastDistance;
            this.lastDistance = motionDistance;
            return num2;
        }

        public override int GetMotionDistance(int _allTime)
        {
            long a = ((base.motionSpeed * _allTime) << 1) + (this.accelerateSpeed * (_allTime * _allTime));
            return (int) IntMath.Divide(a, 0x7d0L);
        }

        public override int GetMotionLerpDistance(int _deltaTime)
        {
            int num2 = 0;
            this.lerpCurTime += _deltaTime;
            int motionDistance = this.GetMotionDistance(this.lerpCurTime);
            num2 = motionDistance - this.lastLerpDistance;
            this.lastLerpDistance = motionDistance;
            return num2;
        }

        public void InitMotionControler(int _motionSpeed, int _accelerateSpeed)
        {
            this.curTime = 0;
            this.lerpCurTime = 0;
            this.lastDistance = 0;
            this.lastLerpDistance = 0;
            base.motionSpeed = _motionSpeed;
            this.accelerateSpeed = _accelerateSpeed;
        }

        public void InitMotionControler(int _time, int _distance, int _accelerateSpeed)
        {
            this.curTime = 0;
            this.lerpCurTime = 0;
            this.lastDistance = 0;
            this.lastLerpDistance = 0;
            this.accelerateSpeed = _accelerateSpeed;
            long a = (_distance * 0x7d0L) - (this.accelerateSpeed * (_time * _time));
            base.motionSpeed = (int) IntMath.Divide(a, (long) (_time << 1));
        }

        public void Reset()
        {
            this.curTime = 0;
            this.lerpCurTime = 0;
            this.lastDistance = 0;
            this.lastLerpDistance = 0;
        }

        public void ResetLerpTime()
        {
            this.lerpCurTime = 0;
            this.lastLerpDistance = 0;
        }

        public void ResetTime()
        {
            this.curTime = 0;
            this.lastDistance = 0;
        }
    }
}

