namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public interface ITssService : IApolloServiceBase
    {
        void Intialize(uint gameId);
        void ReportUserInfo();
        void ReportUserInfo(uint wordId, string roleId);
        void StartWithTalker(IApolloTalker talker, float intervalBetweenCollections = 2);
        void StartWithTransfer(TssTransferBase transfer, float intervalBetweenCollections = 2);
    }
}

