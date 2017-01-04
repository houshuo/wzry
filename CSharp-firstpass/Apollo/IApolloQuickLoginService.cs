namespace Apollo
{
    using System;

    public interface IApolloQuickLoginService : IApolloServiceBase
    {
        void SetQuickLoginNotify(ApolloQuickLoginNotify callback);
        void SwitchUser(bool useExternalAccount);
    }
}

