namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public interface IApolloConnector : IApolloServiceBase
    {
        event ConnectEventHandler ConnectEvent;

        event DisconnectEventHandler DisconnectEvent;

        event ConnectorErrorEventHandler ErrorEvent;

        event ReconnectEventHandler ReconnectEvent;

        event RecvedDataHandler RecvedDataEvent;

        event RecvedUdpDataHandler RecvedUdpDataEvent;

        event RouteChangedEventHandler RouteChangedEvent;

        ApolloResult Connect();
        ApolloResult Connect(uint timeout);
        ApolloResult Disconnect();
        ApolloResult GetSessionStopReason(ref int result, ref int reason, ref int excode);
        ApolloResult ReadData(out byte[] buffer, out int realLength);
        ApolloResult ReadUdpData(out byte[] buffer, out int realLength);
        ApolloResult Reconnect();
        ApolloResult Reconnect(uint timeout);
        ApolloResult ReportAccessToken(string accessToken, ulong expire);
        void SetClientType(ClientType type);
        void SetProtocolVersion(int headVersion, int bodyVersion);
        ApolloResult SetRouteInfo(ApolloRouteInfoBase routeInfo);
        ApolloResult SetSecurityInfo(ApolloEncryptMethod EncyptMethod, ApolloKeyMaking KeyMakingMethod, string DHP);
        ApolloResult WriteData(byte[] data, int len = -1);
        ApolloResult WriteData(byte[] data, int len, ApolloRouteInfoBase routeInfo, bool allowLost = false);
        ApolloResult WriteUdpData(byte[] data, int len = -1);

        bool Connected { get; }

        ApolloLoginInfo LoginInfo { get; }
    }
}

