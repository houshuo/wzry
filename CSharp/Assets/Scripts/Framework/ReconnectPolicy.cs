namespace Assets.Scripts.Framework
{
    using Apollo;
    using System;
    using UnityEngine;

    public class ReconnectPolicy
    {
        private tryReconnectDelegate callback;
        private BaseConnector connector;
        private uint connectTimeout = 10;
        private uint reconnectCount = 4;
        private float reconnectTime;
        private bool sessionStopped;
        public bool shouldReconnect;
        private uint tryCount;

        public void SetConnector(BaseConnector inConnector, tryReconnectDelegate inEvent, uint tryMax)
        {
            this.StopPolicy();
            this.connector = inConnector;
            this.callback = inEvent;
            this.reconnectCount = tryMax;
        }

        public void StartPolicy(ApolloResult result, int timeWait)
        {
            ApolloResult result2 = result;
            switch (result2)
            {
                case ApolloResult.Success:
                    this.shouldReconnect = false;
                    this.sessionStopped = false;
                    return;

                case ApolloResult.NetworkException:
                    this.shouldReconnect = true;
                    this.sessionStopped = false;
                    this.reconnectTime = (this.tryCount != 0) ? ((float) timeWait) : ((float) 0);
                    return;

                case ApolloResult.Timeout:
                case ApolloResult.GcpError:
                case ApolloResult.PeerStopSession:
                    break;

                default:
                    if ((result2 != ApolloResult.ConnectFailed) && (result2 != ApolloResult.TokenSvrError))
                    {
                        this.shouldReconnect = true;
                        this.sessionStopped = true;
                        this.reconnectTime = (this.tryCount != 0) ? ((float) timeWait) : ((float) 0);
                        return;
                    }
                    break;
            }
            this.shouldReconnect = true;
            this.sessionStopped = true;
            this.reconnectTime = (this.tryCount != 0) ? ((float) timeWait) : ((float) 0);
        }

        public void StopPolicy()
        {
            this.sessionStopped = false;
            this.shouldReconnect = false;
            this.reconnectTime = this.connectTimeout;
            this.tryCount = 0;
        }

        public void UpdatePolicy(bool bForce)
        {
            if ((this.connector != null) && !this.connector.connected)
            {
                if (bForce)
                {
                    this.reconnectTime = this.connectTimeout;
                    this.tryCount = this.reconnectCount;
                    if (this.sessionStopped)
                    {
                        this.connector.RestartConnector();
                    }
                    else
                    {
                        this.connector.RestartConnector();
                    }
                }
                else
                {
                    this.reconnectTime -= Time.unscaledDeltaTime;
                    if (this.reconnectTime < 0f)
                    {
                        this.tryCount++;
                        this.reconnectTime = this.connectTimeout;
                        uint tryCount = this.tryCount;
                        if (this.callback != null)
                        {
                            tryCount = this.callback(tryCount, this.reconnectCount);
                        }
                        if (tryCount <= this.reconnectCount)
                        {
                            this.tryCount = tryCount;
                            if (this.sessionStopped)
                            {
                                this.connector.RestartConnector();
                            }
                            else
                            {
                                this.connector.RestartConnector();
                            }
                        }
                    }
                }
            }
        }
    }
}

