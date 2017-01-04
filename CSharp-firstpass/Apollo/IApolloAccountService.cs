namespace Apollo
{
    using System;

    public interface IApolloAccountService : IApolloServiceBase
    {
        event AccountInitializeHandle InitializeEvent;

        event AccountLoginHandle LoginEvent;

        event AccountLogoutHandle LogoutEvent;

        event RefreshAccessTokenHandler RefreshAtkEvent;

        ApolloResult GetRecord(ref ApolloAccountInfo accountInfo);
        bool Initialize(ApolloBufferBase initInfo);
        bool IsPlatformInstalled(ApolloPlatform platform);
        bool IsPlatformSupportApi(ApolloPlatform platform);
        void Login(ApolloPlatform platform);
        void Logout();
        void RefreshAccessToken();
        void Reset();
        [Obsolete("Obsolete since 1.1.6, use Initialize instead")]
        void SetPermission(uint permission);
    }
}

