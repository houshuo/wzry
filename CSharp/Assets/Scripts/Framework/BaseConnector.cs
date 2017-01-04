namespace Assets.Scripts.Framework
{
    using Apollo;
    using System;

    public class BaseConnector
    {
        public bool connected;
        protected IApolloConnector connector;
        public static uint connectTimeout = 10;
        public ConnectorParam initParam;

        public bool CreateConnector(ConnectorParam param)
        {
            this.DestroyConnector();
            if (param == null)
            {
                return false;
            }
            this.initParam = param;
            this.connected = false;
            this.connector = IApollo.Instance.CreateApolloConnection(ApolloConfig.platform, (uint) 0xffffff, param.url);
            if (this.connector == null)
            {
                return false;
            }
            Console.WriteLine("Create Connect Entered!{0}  {1}", ApolloConfig.platform, param.url);
            this.connector.ConnectEvent += new ConnectEventHandler(this.onConnectEvent);
            this.connector.DisconnectEvent += new DisconnectEventHandler(this.onDisconnectEvent);
            this.connector.ReconnectEvent += new ReconnectEventHandler(this.onReconnectEvent);
            this.connector.ErrorEvent += new ConnectorErrorEventHandler(this.onConnectError);
            this.connector.SetSecurityInfo(param.enc, param.keyMaking, ConnectorParam.DH);
            if (this.connector.Connect(connectTimeout) != ApolloResult.Success)
            {
                return false;
            }
            return true;
        }

        protected virtual void DealConnectClose(ApolloResult result)
        {
        }

        protected virtual void DealConnectError(ApolloResult result)
        {
        }

        protected virtual void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
        {
        }

        protected virtual void DealConnectSucc()
        {
        }

        public void DestroyConnector()
        {
            if (this.connector != null)
            {
                this.connector.ConnectEvent -= new ConnectEventHandler(this.onConnectEvent);
                this.connector.DisconnectEvent -= new DisconnectEventHandler(this.onDisconnectEvent);
                this.connector.ReconnectEvent -= new ReconnectEventHandler(this.onReconnectEvent);
                this.connector.ErrorEvent -= new ConnectorErrorEventHandler(this.onConnectError);
                this.connector.Disconnect();
                IApollo.Instance.DestroyApolloConnector(this.connector);
                this.connector = null;
                this.connected = false;
            }
        }

        public ConnectorParam GetConnectionParam()
        {
            return this.initParam;
        }

        private void onConnectError(ApolloResult result)
        {
            this.connected = false;
            this.DealConnectError(result);
        }

        private void onConnectEvent(ApolloResult result, ApolloLoginInfo loginInfo)
        {
            if (this.connector != null)
            {
                if (result == ApolloResult.Success)
                {
                    this.connected = true;
                    this.DealConnectSucc();
                }
                else
                {
                    this.DealConnectFail(result, loginInfo);
                }
            }
        }

        private void onDisconnectEvent(ApolloResult result)
        {
            if (result == ApolloResult.Success)
            {
                this.connected = false;
                this.DealConnectClose(result);
            }
        }

        private void onReconnectEvent(ApolloResult result)
        {
            if (this.connector != null)
            {
                if (result == ApolloResult.Success)
                {
                    this.connected = true;
                    this.DealConnectSucc();
                }
                else
                {
                    this.DealConnectFail(result, null);
                }
            }
        }

        public void RestartConnector()
        {
            this.DestroyConnector();
            this.CreateConnector(this.initParam);
        }
    }
}

