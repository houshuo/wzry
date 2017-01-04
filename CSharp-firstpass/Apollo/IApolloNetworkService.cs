namespace Apollo
{
    using System;

    public interface IApolloNetworkService : IApolloServiceBase
    {
        event NetworkStateChanged NetworkChangedEvent;

        NetworkState GetNetworkState();
    }
}

