namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public abstract class TssTransferBase
    {
        internal ITssService tssService;

        protected TssTransferBase()
        {
        }

        public abstract bool IsConnected();
        public abstract void OnTssDataCollected(byte[] data);
        public void SetRecvedTssData(byte[] data, int len = 0)
        {
            TssService tssService = this.tssService as TssService;
            if (tssService != null)
            {
                tssService.SetAntiData(data, len);
            }
        }
    }
}

