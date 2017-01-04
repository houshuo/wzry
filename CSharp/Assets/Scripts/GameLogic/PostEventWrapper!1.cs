namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct PostEventWrapper<T> : IPostEventWrapper
    {
        public T prm;
        public uint frameNum;
        public RefAction<T> callback;
        public PostEventWrapper(RefAction<T> _call, T _prm, uint _delayFrame = 1)
        {
            this.prm = _prm;
            this.frameNum = Singleton<FrameSynchr>.instance.CurFrameNum + _delayFrame;
            this.callback = _call;
        }

        public uint GetFrameNum()
        {
            return this.frameNum;
        }

        public void ExecCommand()
        {
            if (this.callback != null)
            {
                this.callback(ref this.prm);
            }
        }
    }
}

