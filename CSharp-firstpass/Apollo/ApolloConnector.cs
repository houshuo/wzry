namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ApolloConnector : ApolloObject, IApolloServiceBase, IApolloConnector
    {
        private byte[] tempReadBuffer;

        public event ConnectEventHandler ConnectEvent;

        public event DisconnectEventHandler DisconnectEvent;

        public event ConnectorErrorEventHandler ErrorEvent;

        public event ReconnectEventHandler ReconnectEvent;

        public event RecvedDataHandler RecvedDataEvent;

        public event RecvedUdpDataHandler RecvedUdpDataEvent;

        public event RouteChangedEventHandler RouteChangedEvent;

        public ApolloConnector()
        {
            this.Connected = false;
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_connect(ulong objId, uint timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_disconnect(ulong objId);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_getstopreason(ulong objId, ref int result, ref int reason, ref int excode);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern int apollo_connector_Initialize(ulong objId, ApolloPlatform platform, uint permission, [MarshalAs(UnmanagedType.LPStr)] string pszSvrUrl);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_readData(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, ref int size);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_readUdpData(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, ref int size);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_reconnect(ulong objId, uint timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_report_accesstoken(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string atk, uint expire);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_set_clientType(ulong objId, ClientType type);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_set_protocol_version(ulong objId, int headVersion, int bodyVersion);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_setRouteInfo(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, int size);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_setSecurityInfo(ulong objId, ApolloEncryptMethod encyptMethod, ApolloKeyMaking keyMakingMethod, [MarshalAs(UnmanagedType.LPStr)] string pszDHP);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_writeData(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, int size);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_writeData_with_route_info(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, int size, [MarshalAs(UnmanagedType.LPArray)] byte[] routeData, int routeDataLen, bool allowLost);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern ApolloResult apollo_connector_writeUdpData(ulong objId, [MarshalAs(UnmanagedType.LPArray)] byte[] buff, int size);
        public ApolloResult Connect()
        {
            return this.Connect(30);
        }

        public ApolloResult Connect(uint timeout)
        {
            ADebug.Log("Connect");
            return apollo_connector_connect(base.ObjectId, timeout);
        }

        public ApolloResult Disconnect()
        {
            ADebug.Log("Disconnect");
            return apollo_connector_disconnect(base.ObjectId);
        }

        ~ApolloConnector()
        {
            ADebug.Log(" ~ApolloConnector()");
            this.Disconnect();
        }

        public ApolloResult GetSessionStopReason(ref int result, ref int reason, ref int excode)
        {
            return apollo_connector_getstopreason(base.ObjectId, ref result, ref reason, ref excode);
        }

        public ApolloResult Initialize(ApolloPlatform platform, uint permission, string url)
        {
            ADebug.Log(string.Concat(new object[] { "Connector Initialize:", platform, " url:", url }));
            if (platform == ApolloPlatform.WTLogin)
            {
            }
            return (ApolloResult) apollo_connector_Initialize(base.ObjectId, platform, permission, url);
        }

        private void OnConnectorErrorProc(string msg)
        {
            ApolloStringParser parser = new ApolloStringParser(msg);
            ApolloResult @int = (ApolloResult) parser.GetInt("Result", 6);
            ADebug.LogError("OnConnectorErrorProc:" + @int);
            this.Connected = false;
            if (this.ErrorEvent != null)
            {
                try
                {
                    this.ErrorEvent(@int);
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        private void OnConnectProc(string msg)
        {
            ADebug.Log("c#:OnConnectProc: " + msg);
            if (string.IsNullOrEmpty(msg))
            {
                ADebug.LogError("OnConnectProc msg is null");
            }
            else
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloResult @int = (ApolloResult) parser.GetInt("Result", 6);
                this.LoginInfo = parser.GetObject<ApolloLoginInfo>("LoginInfo");
                if (@int == ApolloResult.Success)
                {
                    this.Connected = true;
                }
                else
                {
                    this.Connected = false;
                }
                ADebug.Log(string.Concat(new object[] { "c#:OnConnectProc: ", @int, " loginfo:", this.LoginInfo }));
                if (((this.LoginInfo != null) && (this.LoginInfo.AccountInfo != null)) && (this.LoginInfo.AccountInfo.TokenList != null))
                {
                    ADebug.Log(string.Concat(new object[] { "C# logininfo| platform:", this.LoginInfo.AccountInfo.Platform, " openid:", this.LoginInfo.AccountInfo.OpenId, " tokensize:", this.LoginInfo.AccountInfo.TokenList.Count, " pf:", this.LoginInfo.AccountInfo.Pf, " pfkey:", this.LoginInfo.AccountInfo.PfKey }));
                }
                if (this.ConnectEvent != null)
                {
                    try
                    {
                        this.ConnectEvent(@int, this.LoginInfo);
                    }
                    catch (Exception exception)
                    {
                        ADebug.LogException(exception);
                    }
                }
                else
                {
                    ADebug.Log("OnConnectProc ConnectEvent is null");
                }
            }
        }

        private void OnDataRecvedProc(string msg)
        {
            this.Connected = true;
            if (this.RecvedDataEvent != null)
            {
                try
                {
                    this.RecvedDataEvent();
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        private void OnDisconnectProc(string msg)
        {
            ApolloStringParser parser = new ApolloStringParser(msg);
            ApolloResult @int = (ApolloResult) parser.GetInt("Result");
            if (@int == ApolloResult.Success)
            {
                this.Connected = false;
            }
            if (this.DisconnectEvent != null)
            {
                try
                {
                    this.DisconnectEvent(@int);
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        private void OnReconnectProc(string msg)
        {
            ADebug.Log("c#:OnReconnectProc: " + msg);
            ApolloStringParser parser = new ApolloStringParser(msg);
            ApolloResult @int = (ApolloResult) parser.GetInt("Result", 6);
            if (@int == ApolloResult.Success)
            {
                this.Connected = true;
            }
            else
            {
                this.Connected = false;
            }
            if (this.ReconnectEvent != null)
            {
                try
                {
                    this.ReconnectEvent(@int);
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
            else
            {
                ADebug.Log("OnReconnectProc ReconnectEvent is null");
            }
        }

        private void OnRouteChangedProc(string msg)
        {
            ulong serverId = new ApolloStringParser(msg).GetUInt64("serverId");
            if (this.RouteChangedEvent != null)
            {
                try
                {
                    this.RouteChangedEvent(serverId);
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        private void OnUdpDataRecvedProc(string msg)
        {
            if (this.RecvedUdpDataEvent != null)
            {
                try
                {
                    this.RecvedUdpDataEvent();
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        public ApolloResult ReadData(out byte[] buffer, out int realLength)
        {
            buffer = null;
            realLength = 0;
            if (!this.Connected)
            {
            }
            if (this.tempReadBuffer == null)
            {
                this.tempReadBuffer = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
            }
            int length = this.tempReadBuffer.Length;
            ApolloResult result = apollo_connector_readData(base.ObjectId, this.tempReadBuffer, ref length);
            if (result == ApolloResult.Success)
            {
                if (length == 0)
                {
                    ADebug.LogError("ReadData empty len==0");
                    return ApolloResult.Empty;
                }
                buffer = this.tempReadBuffer;
                realLength = length;
            }
            return result;
        }

        public ApolloResult ReadUdpData(out byte[] buffer, out int realLength)
        {
            buffer = null;
            realLength = 0;
            if (!this.Connected)
            {
            }
            if (this.tempReadBuffer == null)
            {
                this.tempReadBuffer = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
            }
            int length = this.tempReadBuffer.Length;
            ApolloResult result = apollo_connector_readUdpData(base.ObjectId, this.tempReadBuffer, ref length);
            if (result == ApolloResult.Success)
            {
                if (length == 0)
                {
                    ADebug.LogError("ReadUdpData empty len==0");
                    return ApolloResult.Empty;
                }
                buffer = this.tempReadBuffer;
                realLength = length;
            }
            return result;
        }

        public ApolloResult Reconnect()
        {
            ADebug.Log("Reconnect");
            return this.Reconnect(30);
        }

        public ApolloResult Reconnect(uint timeout)
        {
            ADebug.Log("Reconnect");
            return apollo_connector_reconnect(base.ObjectId, timeout);
        }

        public ApolloResult ReportAccessToken(string accessToken, ulong expire)
        {
            return apollo_connector_report_accesstoken(base.ObjectId, accessToken, (uint) expire);
        }

        public void SetClientType(ClientType type)
        {
            apollo_connector_set_clientType(base.ObjectId, type);
        }

        public void SetProtocolVersion(int headVersion, int bodyVersion)
        {
            apollo_connector_set_protocol_version(base.ObjectId, headVersion, bodyVersion);
        }

        public ApolloResult SetRouteInfo(ApolloRouteInfoBase routeInfo)
        {
            byte[] buffer;
            if (routeInfo == null)
            {
                return ApolloResult.InvalidArgument;
            }
            routeInfo.Encode(out buffer);
            if (buffer == null)
            {
                ADebug.LogError("WriteData Encode error!");
                return ApolloResult.InnerError;
            }
            return apollo_connector_setRouteInfo(base.ObjectId, buffer, buffer.Length);
        }

        public ApolloResult SetSecurityInfo(ApolloEncryptMethod encyptMethod, ApolloKeyMaking keyMakingMethod, string dhp)
        {
            ADebug.Log(string.Concat(new object[] { "SetSecurityInfo encyptMethod:", encyptMethod, " keyMakingMethod:", keyMakingMethod, " dh:", dhp }));
            return apollo_connector_setSecurityInfo(base.ObjectId, encyptMethod, keyMakingMethod, dhp);
        }

        public ApolloResult WriteData(byte[] data, int len = -1)
        {
            if (!this.Connected)
            {
                return ApolloResult.NoConnection;
            }
            if (len == -1)
            {
                len = data.Length;
            }
            return apollo_connector_writeData(base.ObjectId, data, len);
        }

        public ApolloResult WriteData(byte[] data, int len, ApolloRouteInfoBase routeInfo, bool allowLost = false)
        {
            byte[] buffer;
            if (routeInfo == null)
            {
                return ApolloResult.InvalidArgument;
            }
            if (!this.Connected)
            {
                return ApolloResult.NoConnection;
            }
            if (len == -1)
            {
                len = data.Length;
            }
            routeInfo.Encode(out buffer);
            if (buffer == null)
            {
                ADebug.LogError("WriteData Encode error!");
                return ApolloResult.InnerError;
            }
            return apollo_connector_writeData_with_route_info(base.ObjectId, data, len, buffer, buffer.Length, allowLost);
        }

        public ApolloResult WriteUdpData(byte[] data, int len = -1)
        {
            if (!this.Connected)
            {
                return ApolloResult.NoConnection;
            }
            if (len == -1)
            {
                len = data.Length;
            }
            return apollo_connector_writeUdpData(base.ObjectId, data, len);
        }

        public bool Connected { get; private set; }

        public ApolloLoginInfo LoginInfo { get; private set; }
    }
}

