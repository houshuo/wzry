namespace Apollo
{
    using System;

    public interface IApolloPayService : IApolloServiceBase
    {
        event OnApolloPaySvrEvenHandle PayEvent;

        void Action(ApolloActionBufferBase info, ApolloActionDelegate callback);
        [Obsolete("Obsolete since V1.1.6, use Pay instead", true)]
        bool ApolloPay(ApolloPayInfoBase payInfo);
        [Obsolete("Obsolete since V1.1.6, use Initialize instead", true)]
        bool ApolloPaySvrInit(ApolloBufferBase registerInfo);
        [Obsolete("Obsolete since V1.1.6, use Dipose instead", true)]
        bool ApolloPaySvrUninit();
        bool Dipose();
        IApolloExtendPayServiceBase GetExtendService();
        bool Initialize(ApolloBufferBase registerInfo);
        bool Pay(ApolloActionBufferBase payInfo);
    }
}

