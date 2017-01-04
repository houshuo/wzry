namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class Tx : ApolloObject
    {
        public static Tx Instance = new Tx();

        public event NetworkStateChangedNotify TXNetworkChangedEvent;

        private Tx() : base(false, true)
        {
        }

        public NetworkState GetNetworkState()
        {
            return tx_network_GetNetworkState();
        }

        public void Initialize()
        {
            ADebug.Log("TX Initialize");
            tx_object_unity_enable_ui_update();
            this.setNetworkChangedCallback();
        }

        [MonoPInvokeCallback(typeof(NetworkStateChangedNotify))]
        private static void onNetworkChanged(NetworkState state)
        {
            if (Instance.TXNetworkChangedEvent != null)
            {
                Instance.TXNetworkChangedEvent(state);
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            tx_object_unity_update();
        }

        private void setNetworkChangedCallback()
        {
            tx_network_SetNetworkChangedCallback(new NetworkStateChangedNotify(Tx.onNetworkChanged));
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern NetworkState tx_network_GetNetworkState();
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void tx_network_SetNetworkChangedCallback([MarshalAs(UnmanagedType.FunctionPtr)] NetworkStateChangedNotify callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void tx_object_unity_enable_ui_update();
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void tx_object_unity_update();
    }
}

