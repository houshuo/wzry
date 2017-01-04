namespace Assets.Scripts.GameLogic
{
    using System;

    public interface IPostEventWrapper
    {
        void ExecCommand();
        uint GetFrameNum();
    }
}

