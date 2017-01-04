namespace Apollo
{
    using System;

    public interface INotice : IApolloServiceBase
    {
        void GetNoticeData(APOLLO_NOTICETYPE type, string scene, ref ApolloNoticeInfo info);
        void HideNotice();
        void ShowNotice(APOLLO_NOTICETYPE type, string scene);
    }
}

