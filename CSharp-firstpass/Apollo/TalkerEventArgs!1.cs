namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;

    public class TalkerEventArgs<TResp>
    {
        public TalkerEventArgs()
        {
        }

        public TalkerEventArgs(ApolloResult result)
        {
            this.Result = result;
        }

        public TalkerEventArgs(TResp response, object Context)
        {
            this.Result = ApolloResult.Success;
            this.Response = response;
        }

        public TalkerEventArgs(ApolloResult result, string errorMessage)
        {
            this.Result = result;
            this.ErrorMessage = errorMessage;
        }

        public object Context { get; set; }

        public string ErrorMessage { get; set; }

        public TResp Response { get; set; }

        public ApolloResult Result { get; set; }
    }
}

