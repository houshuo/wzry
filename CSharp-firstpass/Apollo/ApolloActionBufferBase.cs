namespace Apollo
{
    using System;

    public abstract class ApolloActionBufferBase : ApolloBufferBase
    {
        private int action;

        protected ApolloActionBufferBase()
        {
        }

        protected ApolloActionBufferBase(int action)
        {
            this.action = action;
        }

        protected override void BeforeDecode(ApolloBufferReader reader)
        {
            reader.Read(ref this.action);
        }

        protected override void BeforeEncode(ApolloBufferWriter writer)
        {
            writer.Write(this.Action);
        }

        public int Action
        {
            get
            {
                return this.action;
            }
            protected set
            {
                this.action = value;
            }
        }
    }
}

