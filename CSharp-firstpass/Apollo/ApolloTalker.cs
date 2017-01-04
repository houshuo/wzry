namespace Apollo
{
    using ApolloTdr;
    using System;
    using System.Runtime.CompilerServices;

    internal class ApolloTalker : ApolloObject, IApolloTalker
    {
        private CircularCollection<ApolloMessage> apolloMessageCollections;
        private bool autoUpdate;
        private RecvedDataHandler recvDataHandler;

        public ApolloTalker(IApolloConnector connector) : base(false, true)
        {
            this.apolloMessageCollections = new CircularCollection<ApolloMessage>(100);
            this.connector = connector;
            if (connector == null)
            {
                throw new Exception("Invalid Argument");
            }
            this.AutoUpdate = true;
        }

        private void onRecvedData()
        {
            ADebug.Log("onRecvedData OnDataRecvedProc");
            while (true)
            {
                int num;
                byte[] buffer = null;
                if (this.connector.ReadData(out buffer, out num) != ApolloResult.Success)
                {
                    return;
                }
                try
                {
                    ApolloMessage message = ApolloMessageManager.Instance.UnpackResponseData(buffer, num);
                    if (message != null)
                    {
                        ADebug.Log(string.Concat(new object[] { "Recved:", message.Command, " and resp is:", message.Response }));
                        ADebug.Log(string.Concat(new object[] { "OnDataRecvedProc: apolloMessage.Handler != null?: ", message.Handler != null, " apolloMessage.HandlerWithReceipt != null?: ", message.HandlerWithReceipt != null, " apolloMessage.HandlerWithoutReceipt != null?: ", message.HandlerWithoutReceipt != null }));
                        message.HandleMessage();
                    }
                    else
                    {
                        ADebug.LogError("OnDataRecvedProc UnpackResponseData error");
                    }
                }
                catch (Exception exception)
                {
                    ADebug.LogException(exception);
                }
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            this.popAndHandleApolloMessage();
        }

        private bool popAndHandleApolloMessage()
        {
            if (this.apolloMessageCollections.Count == 0)
            {
                return false;
            }
            ADebug.Log("popAndHandleApolloMessage: " + this.apolloMessageCollections.Count);
            ApolloMessage next = this.apolloMessageCollections.Next;
            if (next != null)
            {
                if (next.Handler != null)
                {
                    TalkerEventArgs e = new TalkerEventArgs(next.Response, next.Context);
                    next.Handler(next.Request, e);
                }
                if (next.HandlerWithoutReceipt != null)
                {
                    next.HandlerWithoutReceipt(next.Response);
                }
                if (next.HandlerWithReceipt != null)
                {
                    IPackable receipt = null;
                    next.HandlerWithReceipt(next.Response, ref receipt);
                    if (receipt != null)
                    {
                        ADebug.Log("HandlerWithReceipt receipt:" + receipt);
                        this.SendReceipt<NullResponse>(receipt, next.AsyncFlag);
                    }
                }
                if (next.IsRequest)
                {
                    ApolloMessageManager.Instance.RemoveMessage(next);
                }
            }
            return true;
        }

        private void pushApolloMessage(ApolloMessage msg)
        {
            if (msg != null)
            {
                this.apolloMessageCollections.Add(msg);
            }
        }

        public ApolloResult Register(RawMessageHandler handler)
        {
            return this.Register(new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw), handler);
        }

        public ApolloResult Register<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable
        {
            Type type = typeof(TResp);
            string fullName = type.FullName;
            return this.Register<TResp>(fullName, handler);
        }

        public ApolloResult Register<TResp, TReceipt>(TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp: IUnpackable where TReceipt: IPackable
        {
            Type type = typeof(TResp);
            string fullName = type.FullName;
            return this.Register<TResp, TReceipt>(fullName, handler);
        }

        public ApolloResult Register(TalkerCommand command, RawMessageHandler handler)
        {
            if ((command == null) || (handler == null))
            {
                return ApolloResult.InvalidArgument;
            }
            if (ApolloMessageManager.Instance.Exist(command))
            {
            }
            ApolloMessage message = new ApolloMessage(command) {
                RawMessageHandler = handler
            };
            ApolloMessageManager.Instance.Add(message);
            return ApolloResult.Success;
        }

        public ApolloResult Register<TResp>(TalkerCommand command, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable
        {
            <Register>c__AnonStorey16<TResp> storey = new <Register>c__AnonStorey16<TResp> {
                handler = handler
            };
            if ((command == null) || (storey.handler == null))
            {
                return ApolloResult.InvalidArgument;
            }
            ADebug.Log("Register:" + command);
            if (ApolloMessageManager.Instance.Exist(command))
            {
            }
            ApolloMessage message = new ApolloMessage(command) {
                RespType = typeof(TResp),
                HandlerWithoutReceipt = new TalkerMessageWithoutReceiptHandler(storey.<>m__3)
            };
            ApolloMessageManager.Instance.Add(message);
            return ApolloResult.Success;
        }

        internal ApolloResult Register<TResp, TReceipt>(TalkerCommand command, TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp: IUnpackable where TReceipt: IPackable
        {
            <Register>c__AnonStorey17<TResp, TReceipt> storey = new <Register>c__AnonStorey17<TResp, TReceipt> {
                handler = handler
            };
            if ((command == null) || (storey.handler == null))
            {
                return ApolloResult.InvalidArgument;
            }
            if (ApolloMessageManager.Instance.Exist(command))
            {
            }
            ApolloMessage message = new ApolloMessage(command) {
                RespType = typeof(TResp),
                ReceiptType = typeof(TReceipt),
                Talker = this,
                HandlerWithReceipt = new TalkerMessageWithReceiptHandler(storey.<>m__4)
            };
            ApolloMessageManager.Instance.Add(message);
            return ApolloResult.Success;
        }

        public ApolloResult Register<TResp>(TalkerCommand.CommandDomain domain, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable
        {
            Type type = typeof(TResp);
            string fullName = type.FullName;
            return this.Register<TResp>(new TalkerCommand(domain, fullName), handler);
        }

        public ApolloResult Register<TResp>(string command, TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable
        {
            return this.Register<TResp>(new TalkerCommand(TalkerCommand.CommandDomain.App, command), handler);
        }

        public ApolloResult Register<TResp, TReceipt>(string command, TalkerMessageWithReceiptHandler<TResp, TReceipt> handler) where TResp: IUnpackable where TReceipt: IPackable
        {
            return this.Register<TResp, TReceipt>(new TalkerCommand(TalkerCommand.CommandDomain.App, command), handler);
        }

        public ApolloResult RegisterMessage<TResp>(TalkerMessageWithoutReceiptHandler<TResp> handler) where TResp: IUnpackable
        {
            return this.Register<TResp>(handler);
        }

        private void resetRecvedDataHandler()
        {
            if ((this.connector != null) && (this.recvDataHandler != null))
            {
                this.connector.RecvedDataEvent -= this.recvDataHandler;
            }
        }

        public ApolloResult Send(IPackable request)
        {
            return this.SendNotice(request);
        }

        internal ApolloResult Send(TalkerCommand.CommandDomain domain, IPackable request)
        {
            return this.SendNotice(new TalkerCommand(domain, request), request);
        }

        public ApolloResult Send(byte[] data, int usedSize)
        {
            return this.Send(TalkerMessageType.Notice, new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw), data, usedSize, null);
        }

        public ApolloResult Send(TalkerMessageType type, TalkerCommand command, IPackable request, ApolloMessage message)
        {
            if ((command == null) || (request == null))
            {
                return ApolloResult.InvalidArgument;
            }
            byte[] data = null;
            if (message == null)
            {
                message = new ApolloMessage();
            }
            if (message.PackRequestData(command, request, out data, type))
            {
                ADebug.Log(string.Concat(new object[] { "Sending:", command, " data size:", data.Length }));
                ApolloResult result = this.connector.WriteData(data, -1);
                if ((result == ApolloResult.Success) && (message.Handler != null))
                {
                    ApolloMessageManager.Instance.SeqMessageCollection.Add(message.SeqNum, message);
                }
                return result;
            }
            ADebug.LogError("Send: " + command + " msg.PackRequestDat error");
            return ApolloResult.InnerError;
        }

        public ApolloResult Send<TResp>(IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp: IUnpackable
        {
            return this.Send<TResp>(new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, handler, context, timeout);
        }

        public ApolloResult Send<TResp>(TalkerCommand command, IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp: IUnpackable
        {
            return this.Send<TResp>(TalkerMessageType.Request, command, request, handler, context, timeout);
        }

        public ApolloResult Send(TalkerMessageType type, TalkerCommand command, byte[] bodyData, int usedSize, ApolloMessage message)
        {
            if (((command == null) || (bodyData == null)) || (usedSize <= 0))
            {
                return ApolloResult.InvalidArgument;
            }
            byte[] data = null;
            if (message == null)
            {
                message = new ApolloMessage();
            }
            if (message.PackRequestData(command, bodyData, usedSize, out data, type))
            {
                ADebug.Log(string.Concat(new object[] { "Sending:", command, " data size:", data.Length }));
                return this.connector.WriteData(data, -1);
            }
            ADebug.LogError("Send: " + command + " msg.PackRequestData error");
            return ApolloResult.InnerError;
        }

        public ApolloResult Send<TResp>(TalkerMessageType type, TalkerCommand command, IPackable request, TalkerMessageHandler<TResp> handler, object context, float timeout) where TResp: IUnpackable
        {
            <Send>c__AnonStorey15<TResp> storey = new <Send>c__AnonStorey15<TResp> {
                handler = handler
            };
            ApolloMessage message = new ApolloMessage {
                RespType = typeof(TResp),
                Context = context,
                Life = timeout
            };
            if (storey.handler != null)
            {
                message.Handler = new TalkerMessageHandler(storey.<>m__2);
            }
            return this.Send(type, command, request, message);
        }

        public ApolloResult SendMessage(IPackable request)
        {
            return this.Send(request);
        }

        private ApolloResult SendNotice(IPackable request)
        {
            ADebug.Log("SendNotice:" + request);
            ApolloMessage message = new ApolloMessage();
            return this.Send(TalkerMessageType.Notice, new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, message);
        }

        internal ApolloResult SendNotice(TalkerCommand command, IPackable request)
        {
            ADebug.Log("SendNotice:" + request);
            ApolloMessage message = new ApolloMessage();
            return this.Send(TalkerMessageType.Notice, command, request, message);
        }

        internal ApolloResult SendReceipt<TResp>(IPackable request, uint asyncFlag) where TResp: IUnpackable
        {
            ApolloMessage message = new ApolloMessage {
                AsyncFlag = asyncFlag
            };
            return this.Send(TalkerMessageType.Response, new TalkerCommand(TalkerCommand.CommandDomain.App, request), request, message);
        }

        private ApolloResult SendReceipt<TResp>(TalkerCommand command, IPackable request, uint asyncFlag) where TResp: IUnpackable
        {
            ApolloMessage message = new ApolloMessage {
                AsyncFlag = asyncFlag
            };
            return this.Send(TalkerMessageType.Response, command, request, message);
        }

        private void setRecvedDataHandler()
        {
            if (this.connector != null)
            {
                if (this.recvDataHandler == null)
                {
                    this.recvDataHandler = new RecvedDataHandler(this.onRecvedData);
                }
                this.connector.RecvedDataEvent += this.recvDataHandler;
            }
        }

        public void Unregister<TResp>()
        {
            Type type = typeof(TResp);
            string fullName = type.FullName;
            this.Unregister(fullName);
        }

        public void Unregister(TalkerCommand command)
        {
            if (command != null)
            {
                ApolloMessageManager.Instance.Remove(command);
            }
        }

        public void Unregister(string command)
        {
            this.Unregister(new TalkerCommand(TalkerCommand.CommandDomain.App, command));
        }

        public void UnregisterMessage<TResp>()
        {
            this.Unregister<TResp>();
        }

        public void UnregisterRawMessageHandler()
        {
            this.Unregister(new TalkerCommand(TalkerCommand.CommandDomain.App, TalkerCommand.CommandValueType.Raw));
        }

        public void Update(int num)
        {
            if (!this.autoUpdate && (num >= 0))
            {
                this.onRecvedData();
                for (int i = 0; i < num; i++)
                {
                    if (!this.popAndHandleApolloMessage())
                    {
                        return;
                    }
                }
            }
        }

        public bool AutoUpdate
        {
            get
            {
                return this.autoUpdate;
            }
            set
            {
                if (this.autoUpdate != value)
                {
                    this.autoUpdate = value;
                    if (this.autoUpdate)
                    {
                        this.setRecvedDataHandler();
                    }
                    else
                    {
                        this.resetRecvedDataHandler();
                    }
                }
            }
        }

        public IApolloConnector connector { get; private set; }

        [CompilerGenerated]
        private sealed class <Register>c__AnonStorey16<TResp> where TResp: IUnpackable
        {
            internal TalkerMessageWithoutReceiptHandler<TResp> handler;

            internal void <>m__3(IUnpackable resp)
            {
                this.handler((TResp) resp);
            }
        }

        [CompilerGenerated]
        private sealed class <Register>c__AnonStorey17<TResp, TReceipt> where TResp: IUnpackable where TReceipt: IPackable
        {
            internal TalkerMessageWithReceiptHandler<TResp, TReceipt> handler;

            internal void <>m__4(IUnpackable resp, ref IPackable receipt)
            {
                TReceipt local = default(TReceipt);
                this.handler((TResp) resp, ref local);
                receipt = local;
                ADebug.Log("Register receipt:" + (receipt != null));
            }
        }

        [CompilerGenerated]
        private sealed class <Send>c__AnonStorey15<TResp> where TResp: IUnpackable
        {
            internal TalkerMessageHandler<TResp> handler;

            internal void <>m__2(object req, TalkerEventArgs loginInfo)
            {
                if (this.handler != null)
                {
                    TalkerEventArgs<TResp> e = new TalkerEventArgs<TResp>(loginInfo.Result, loginInfo.ErrorMessage) {
                        Response = loginInfo.Response,
                        Context = loginInfo.Context
                    };
                    this.handler(req, e);
                }
            }
        }
    }
}

