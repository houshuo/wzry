namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ApolloPayService : ApolloObject, IApolloServiceBase, IApolloPayService
    {
        private DictionaryView<int, ApolloActionDelegate> actionCallbackCollection = new DictionaryView<int, ApolloActionDelegate>();
        private bool inited;
        public static readonly ApolloPayService Instance = new ApolloPayService();

        public event OnApolloPaySvrEvenHandle PayEvent;

        private ApolloPayService()
        {
            Console.WriteLine("ApolloPayService Create!{0}", base.ObjectId);
        }

        public void Action(ApolloActionBufferBase info, ApolloActionDelegate callback)
        {
            if (info == null)
            {
                ADebug.LogError("PayService Action Info == null");
            }
            else
            {
                byte[] buffer;
                if (!info.Encode(out buffer))
                {
                    ADebug.LogError("Action Encode error!");
                }
                else
                {
                    if (this.actionCallbackCollection.ContainsKey(info.Action))
                    {
                        this.actionCallbackCollection[info.Action] = callback;
                    }
                    else
                    {
                        this.actionCallbackCollection.Add(info.Action, callback);
                    }
                    apollo_pay_action(buffer, buffer.Length);
                }
            }
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_pay_action([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_pay_Dipose();
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_pay_Initialize([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_pay_Pay([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool apollo_pay_Pay4Mounth([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
        [Obsolete("Obsolete since V1.1.6", true)]
        public bool ApolloPay(ApolloPayInfoBase payInfo)
        {
            return this.Pay(payInfo);
        }

        [Obsolete("Obsolete since V1.1.6", true)]
        public bool ApolloPaySvrInit(ApolloBufferBase registerInfo)
        {
            return this.Initialize(registerInfo);
        }

        [Obsolete("Obsolete since V1.1.6", true)]
        public bool ApolloPaySvrUninit()
        {
            return this.Dipose();
        }

        public bool Dipose()
        {
            this.inited = false;
            return apollo_pay_Dipose();
        }

        public IApolloExtendPayServiceBase GetExtendService()
        {
            PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
            if (currentPlugin == null)
            {
                return null;
            }
            return currentPlugin.GetPayExtendService();
        }

        public bool Initialize(ApolloBufferBase registerInfo)
        {
            byte[] buffer;
            ADebug.Log("ApolloPayService Initialize!");
            this.inited = true;
            registerInfo.Encode(out buffer);
            return apollo_pay_Initialize(buffer, buffer.Length);
        }

        public void OnApolloPayActionProc(int ret, byte[] data)
        {
            ADebug.Log("OnApolloPayActionProc!");
            PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
            if (currentPlugin == null)
            {
                ADebug.LogError("OnApolloPayActionProc plugin is null");
            }
            else
            {
                ApolloAction action = new ApolloAction();
                if (!action.Decode(data))
                {
                    ADebug.LogError("OnApolloPayActionProc Action Decode failed");
                }
                else if (this.actionCallbackCollection.ContainsKey(action.Action))
                {
                    ApolloActionBufferBase base3 = currentPlugin.CreatePayResponseAction(action.Action);
                    if (base3 != null)
                    {
                        if (!base3.Decode(data))
                        {
                            ADebug.LogError("OnApolloPayActionProc Decode failed");
                        }
                        else
                        {
                            ApolloActionDelegate delegate2 = this.actionCallbackCollection[action.Action];
                            if (delegate2 != null)
                            {
                                try
                                {
                                    delegate2((ApolloResult) ret, base3);
                                }
                                catch (Exception exception)
                                {
                                    ADebug.LogError("OnApolloPayActionProc exception:" + exception);
                                }
                            }
                            else
                            {
                                ADebug.LogError("OnApolloPayActionProc callback is null while action == " + action.Action);
                            }
                        }
                    }
                    else
                    {
                        ADebug.LogError("OnApolloPayActionProc info is null");
                    }
                }
                else
                {
                    ADebug.LogError("OnApolloPayActionProc not exist action:" + action.Action);
                }
            }
        }

        public void OnApolloPaySvrNotify(byte[] data)
        {
            ADebug.Log("ApolloPay OnApolloPaySvrNotify!");
            if (this.PayEvent != null)
            {
                PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
                if (currentPlugin == null)
                {
                    ADebug.LogError("OnApolloPaySvrNotify plugin is null");
                }
                else
                {
                    ApolloAction action = new ApolloAction();
                    if (!action.Decode(data))
                    {
                        ADebug.LogError("OnApolloPaySvrNotify Action Decode failed");
                    }
                    else
                    {
                        ApolloBufferBase payResponseInfo = currentPlugin.CreatePayResponseInfo(action.Action);
                        if (payResponseInfo != null)
                        {
                            if (!payResponseInfo.Decode(data))
                            {
                                ADebug.LogError("OnApolloPaySvrNotify Decode failed");
                            }
                            else
                            {
                                this.PayEvent(payResponseInfo);
                            }
                        }
                        else
                        {
                            ADebug.LogError("OnApolloPaySvrNotify info is null");
                        }
                    }
                }
            }
            else
            {
                ADebug.Log("PayEvent is null");
            }
        }

        public bool Pay(ApolloActionBufferBase payInfo)
        {
            byte[] buffer;
            payInfo.Encode(out buffer);
            return (this.inited && apollo_pay_Pay(buffer, buffer.Length));
        }

        public bool Pay4Mounth(ApolloBufferBase pay4MountInfo)
        {
            byte[] buffer;
            pay4MountInfo.Encode(out buffer);
            return (this.inited && apollo_pay_Pay4Mounth(buffer, buffer.Length));
        }
    }
}

