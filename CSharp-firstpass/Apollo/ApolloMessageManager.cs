namespace Apollo
{
    using apollo_talker;
    using ApolloTdr;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class ApolloMessageManager : ApolloObject
    {
        private DictionaryObjectView<TalkerCommand, ApolloMessage> cmdMessageCollection;
        public static ApolloMessageManager Instance = new ApolloMessageManager();
        private DictionaryView<uint, ApolloMessage> seqMessageCollection;

        private ApolloMessageManager() : base(false, true)
        {
            this.seqMessageCollection = new DictionaryView<uint, ApolloMessage>();
            this.cmdMessageCollection = new DictionaryObjectView<TalkerCommand, ApolloMessage>();
        }

        public void Add(ApolloMessage message)
        {
            if (message != null)
            {
                if (message.IsRequest)
                {
                    this.SeqMessageCollection.Add(message.SeqNum, message);
                }
                else if (this.CmdMessageCollection.ContainsKey(message.Command))
                {
                    this.CmdMessageCollection[message.Command] = message;
                }
                else
                {
                    this.CmdMessageCollection.Add(message.Command, message);
                }
            }
        }

        public bool Exist(TalkerCommand command)
        {
            if (command == null)
            {
                return false;
            }
            return this.CmdMessageCollection.ContainsKey(command);
        }

        public bool Exist(uint seq)
        {
            return this.SeqMessageCollection.ContainsKey(seq);
        }

        public ApolloMessage Get(TalkerCommand command)
        {
            if (this.CmdMessageCollection.ContainsKey(command))
            {
                return this.CmdMessageCollection[command];
            }
            return null;
        }

        public ApolloMessage Get(uint seq)
        {
            if (this.SeqMessageCollection.ContainsKey(seq))
            {
                return this.SeqMessageCollection[seq];
            }
            return null;
        }

        protected override void OnUpdate(float delta)
        {
            this.TestLife(delta);
        }

        public void Remove(TalkerCommand command)
        {
            if (this.CmdMessageCollection.ContainsKey(command))
            {
                this.CmdMessageCollection.Remove(command);
            }
        }

        public void Remove(uint seq)
        {
            if (this.SeqMessageCollection.ContainsKey(seq))
            {
                this.SeqMessageCollection.Remove(seq);
            }
        }

        public void RemoveMessage(ApolloMessage msg)
        {
            if (Instance.SeqMessageCollection.ContainsKey(msg.SeqNum))
            {
                Instance.Remove(msg.SeqNum);
            }
            else if (Instance.Exist(msg.Command))
            {
                Instance.Remove(msg.Command);
            }
        }

        public void TestLife(float deltaTime)
        {
            List<uint> list = new List<uint>();
            foreach (uint num in this.seqMessageCollection.Keys)
            {
                ApolloMessage message = this.seqMessageCollection[num];
                if (message != null)
                {
                    message.Life -= deltaTime;
                    if (message.Life <= 0f)
                    {
                        list.Add(num);
                        if (message.Handler != null)
                        {
                            TalkerEventArgs e = new TalkerEventArgs(ApolloResult.Timeout) {
                                Context = message.Context,
                                Response = null
                            };
                            message.Handler(message.Request, e);
                        }
                    }
                }
            }
            foreach (uint num2 in list)
            {
                this.seqMessageCollection.Remove(num2);
            }
        }

        public ApolloMessage UnpackResponseData(byte[] data, int realSize)
        {
            int num2;
            TalkerHead head = new TalkerHead();
            int used = 0;
            TdrError.ErrorType type = head.unpackTLV(ref data, realSize, ref used);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                ADebug.LogError("UnpackResponseData head.unpack error:" + type);
                return null;
            }
            TalkerCommand.CommandDomain bDomain = (TalkerCommand.CommandDomain) head.bDomain;
            TalkerCommand command = null;
            switch (((CMD_FMT) head.bCmdFmt))
            {
                case CMD_FMT.CMD_FMT_INT:
                    if (head.stCommand != null)
                    {
                        command = new TalkerCommand(bDomain, (uint) head.stCommand.iIntCmd);
                    }
                    goto Label_0103;

                case CMD_FMT.CMD_FMT_NIL:
                    command = new TalkerCommand(bDomain, TalkerCommand.CommandValueType.Raw);
                    goto Label_0103;

                default:
                    if ((head.stCommand == null) || (head.stCommand.szStrCmd == null))
                    {
                        goto Label_0103;
                    }
                    num2 = 0;
                    for (int i = 0; i < head.stCommand.szStrCmd.Length; i++)
                    {
                        num2 = i;
                        if (head.stCommand.szStrCmd[i] == 0)
                        {
                            break;
                        }
                    }
                    break;
            }
            string str = Encoding.UTF8.GetString(head.stCommand.szStrCmd, 0, num2);
            command = new TalkerCommand(bDomain, str);
        Label_0103:
            if (command == null)
            {
                ADebug.LogError("With command is null");
                return null;
            }
            ApolloMessage message = null;
            if (ApolloMessage.GetMessageType(head.bFlag) == TalkerMessageType.Response)
            {
                if (Instance.SeqMessageCollection.ContainsKey(head.dwAsync))
                {
                    message = Instance.SeqMessageCollection[head.dwAsync];
                }
                if (message != null)
                {
                    message.AsyncFlag = head.dwAsync;
                    if (message.IsRequest && (head.dwAsync != message.SeqNum))
                    {
                        if (head.dwAsync != message.SeqNum)
                        {
                            ADebug.Log(string.Format("UnpackResponseData error: if(head.dwSeqNum({0}) != seqNum({1})", head.dwAsync, message.SeqNum));
                        }
                        else
                        {
                            ADebug.Log(string.Concat(new object[] { "UnpackResponseData error compare result:", command.Equals(message.Command), " msg.command:", message.Command, " cmd:", command }));
                        }
                        return null;
                    }
                }
            }
            else
            {
                message = this.Get(command);
                if (message != null)
                {
                    ADebug.Log(string.Concat(new object[] { "cmd:", command, " msg receipt handler:", message.HandlerWithReceipt != null, " without receipt handler:", message.HandlerWithoutReceipt != null }));
                }
            }
            if (message == null)
            {
                ADebug.LogError(string.Concat(new object[] { "UnpackResponseData error: msg == null while seq:", head.dwAsync, " cmd:", command, " type:", ApolloMessage.GetMessageType(head.bFlag) }));
                return null;
            }
            ADebug.Log(string.Concat(new object[] { "UnpackResponseData msg.Command:", message.Command, " type:", ApolloMessage.GetMessageType(head.bFlag) }));
            if (realSize < used)
            {
                ADebug.LogError(string.Format("realSize{0} < usedSize({1})", realSize, used));
                return null;
            }
            byte[] destinationArray = new byte[realSize - used];
            Array.Copy(data, used, destinationArray, 0, destinationArray.Length);
            if (message.RespType != null)
            {
                message.Response = Activator.CreateInstance(message.RespType) as IUnpackable;
                if (message.Response != null)
                {
                    type = message.Response.unpackTLV(ref destinationArray, destinationArray.Length, ref used);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        ADebug.Log("UnpackResponseData resp.unpack error:" + type);
                        return null;
                    }
                }
                return message;
            }
            message.RawData = destinationArray;
            return message;
        }

        public DictionaryObjectView<TalkerCommand, ApolloMessage> CmdMessageCollection
        {
            get
            {
                return this.cmdMessageCollection;
            }
        }

        public DictionaryView<uint, ApolloMessage> SeqMessageCollection
        {
            get
            {
                return this.seqMessageCollection;
            }
        }
    }
}

