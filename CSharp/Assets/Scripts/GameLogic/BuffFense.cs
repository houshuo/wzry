namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class BuffFense
    {
        public PoolObjHandle<BuffSkill> buffPtr = new PoolObjHandle<BuffSkill>();

        public BuffFense(BuffSkill inBuff)
        {
            this.buffPtr = new PoolObjHandle<BuffSkill>(inBuff);
        }

        public void Stop()
        {
            if (this.buffPtr != 0)
            {
                this.buffPtr.handle.Stop();
                this.buffPtr.Release();
            }
        }
    }
}

