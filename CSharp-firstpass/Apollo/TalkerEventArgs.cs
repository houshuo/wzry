namespace Apollo
{
    using ApolloTdr;
    using System;

    internal class TalkerEventArgs : TalkerEventArgs<IUnpackable>
    {
        public TalkerEventArgs()
        {
        }

        public TalkerEventArgs(ApolloResult result)
        {
            base.Result = result;
        }

        public TalkerEventArgs(ApolloResult result, string errorMessage)
        {
            base.Result = result;
            base.ErrorMessage = errorMessage;
        }

        public TalkerEventArgs(IUnpackable response, object Context)
        {
            base.Result = ApolloResult.Success;
            base.Response = response;
            base.Context = Context;
        }
    }
}

