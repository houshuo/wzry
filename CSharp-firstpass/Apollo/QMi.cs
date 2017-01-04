namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    internal class QMi : ApolloObject, IQMi, IApolloServiceBase
    {
        public static readonly QMi Instance = new QMi();

        private QMi()
        {
        }

        public void HideQMi()
        {
            WGHideQMi(base.ObjectId);
        }

        public void SetGameEngineType(string gameEngineInfo)
        {
            WGSetGameEngineType(base.ObjectId, gameEngineInfo);
        }

        public void ShowQMi()
        {
            WGShowQMi(base.ObjectId);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WGHideQMi(ulong objId);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WGSetGameEngineType(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string gameEngineType);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void WGShowQMi(ulong objId);
    }
}

