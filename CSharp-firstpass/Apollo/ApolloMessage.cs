namespace Apollo
{
    using apollo_talker;
    using ApolloTdr;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class ApolloMessage
    {
        internal TalkerMessageHandler Handler;
        internal TalkerMessageWithoutReceiptHandler HandlerWithoutReceipt;
        internal TalkerMessageWithReceiptHandler HandlerWithReceipt;
        public float Life;
        internal Apollo.RawMessageHandler RawMessageHandler;

        public ApolloMessage()
        {
            this.Life = 30f;
            this.SeqNum = ApolloCommon.MsgSeq;
            this.IsRequest = true;
        }

        public ApolloMessage(TalkerCommand command)
        {
            this.Life = 30f;
            this.IsRequest = false;
            this.Command = command;
        }

        private int calculateHeadSize(TalkerHead head)
        {
            return 100;
        }

        public static TalkerMessageType GetMessageType(int flag)
        {
            return (((TalkerMessageType) flag) & ((TalkerMessageType) 7));
        }

        public void HandleMessage()
        {
            if (this.Handler != null)
            {
                TalkerEventArgs e = new TalkerEventArgs(this.Response, this.Context);
                this.Handler(this.Request, e);
            }
            else if (this.HandlerWithoutReceipt != null)
            {
                this.HandlerWithoutReceipt(this.Response);
            }
            else if (this.HandlerWithReceipt != null)
            {
                IPackable receipt = null;
                this.HandlerWithReceipt(this.Response, ref receipt);
                if (receipt != null)
                {
                    ADebug.Log("HandlerWithReceipt receipt:" + receipt);
                    if ((this.Talker != null) && (receipt != null))
                    {
                        this.Talker.SendReceipt<NullResponse>(receipt, this.AsyncFlag);
                    }
                    else
                    {
                        ADebug.Log("HandlerWithReceipt without receipt");
                    }
                }
            }
            else if (this.RawMessageHandler != null)
            {
                ADebug.Log((("RawMessageHandler raw data size:" + this.RawData) != null) ? this.RawData.Length : 0);
                this.RawMessageHandler(new RawMessageEventArgs(this.RawData));
            }
            if (this.IsRequest)
            {
                ApolloMessageManager.Instance.RemoveMessage(this);
            }
        }

        public bool PackRequestData(TalkerCommand command, IPackable request, out byte[] data, TalkerMessageType type)
        {
            if ((command == null) || (command.Command == null))
            {
                throw new Exception("Invalid Argument!");
            }
            data = null;
            byte[] buffer = new byte[ApolloCommon.ApolloInfo.MaxMessageBufferSize];
            int used = 0;
            if ((request.packTLV(ref buffer, buffer.Length, ref used, true) == TdrError.ErrorType.TDR_NO_ERROR) && this.PackRequestData(command, buffer, used, out data, type))
            {
                this.Request = request;
                return true;
            }
            ADebug.Log("PackRequestData request.pack error");
            return false;
        }

        public bool PackRequestData(TalkerCommand command, byte[] body, int usedBodySize, out byte[] data, TalkerMessageType type)
        {
            if ((command == null) || (command.Command == null))
            {
                throw new Exception("Invalid Argument!");
            }
            data = null;
            if ((body != null) && (usedBodySize > 0))
            {
                TalkerHead head = new TalkerHead();
                this.setFlag(ref head.bFlag, type, command.Command.Type);
                ADebug.Log(string.Concat(new object[] { "PackRequestData head flag:", head.bFlag, " commandType:", command.Command.Type }));
                this.Command = command;
                head.bDomain = (byte) command.Domain;
                head.bCmdFmt = (byte) command.Command.Type;
                head.stCommand.iIntCmd = (int) command.Command.IntegerValue;
                if (command.Command.StringValue != null)
                {
                    head.stCommand.szStrCmd = Encoding.UTF8.GetBytes(command.Command.StringValue);
                }
                if (type == TalkerMessageType.Response)
                {
                    head.dwAsync = this.AsyncFlag;
                }
                else
                {
                    head.dwAsync = this.SeqNum;
                }
                ADebug.Log(string.Concat(new object[] { "PackRequestData cmd: ", this.Command.ToString(), " type:", type, " async:", head.dwAsync }));
                byte[] buffer = new byte[this.calculateHeadSize(head)];
                int used = 0;
                TdrError.ErrorType type2 = head.packTLV(ref buffer, buffer.Length, ref used, true);
                if (type2 != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    ADebug.Log(string.Concat(new object[] { "PackRequestData head.pack error:", type2, " usedBodySize:", usedBodySize, " usedHeadSize:", used, " headBufflen:", buffer.Length }));
                    return false;
                }
                int num3 = usedBodySize + used;
                data = new byte[num3];
                Array.Copy(buffer, data, used);
                Array.Copy(body, 0, data, used, usedBodySize);
                this.Request = null;
                return true;
            }
            ADebug.Log("PackRequestData request.pack error");
            return false;
        }

        private void setFlag(ref byte flag, TalkerMessageType type, TalkerCommand.CommandValueType commandType)
        {
            flag = 0;
            flag = (byte) (flag | ((byte) (((byte) type) & 7)));
            if (commandType == TalkerCommand.CommandValueType.Raw)
            {
                flag = (byte) (flag | 0x10);
            }
        }

        public uint AsyncFlag { get; set; }

        public TalkerCommand Command { get; set; }

        public object Context { get; set; }

        public bool IsRequest { get; private set; }

        public byte[] RawData { get; set; }

        public Type ReceiptType { get; set; }

        public IPackable Request { get; set; }

        public IUnpackable Response { get; set; }

        public Type RespType { get; set; }

        public uint SeqNum { get; private set; }

        internal ApolloTalker Talker { get; set; }
    }
}

