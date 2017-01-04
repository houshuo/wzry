namespace Apollo
{
    using System;

    public interface IApolloHttpClient
    {
        IApolloHttpRequest CreateRequest(IApolloConnector connector);
        ApolloResult ReleaseRequest(IApolloHttpRequest request);
        void Update();
    }
}

