namespace Apollo
{
    using System;

    public interface IQMi : IApolloServiceBase
    {
        void HideQMi();
        void SetGameEngineType(string gameEngineInfo);
        void ShowQMi();
    }
}

