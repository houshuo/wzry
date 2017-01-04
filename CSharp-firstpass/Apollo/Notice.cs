namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class Notice : ApolloObject, INotice, IApolloServiceBase
    {
        public static readonly Notice Instance = new Notice();

        private Notice()
        {
        }

        public void GetNoticeData(APOLLO_NOTICETYPE type, string scene, ref ApolloNoticeInfo info)
        {
            StringBuilder builder = new StringBuilder(0x2800);
            GetNoticeData(base.ObjectId, type, scene, builder, 0x2800);
            string src = builder.ToString();
            if (src.Length > 0)
            {
                info.FromString(src);
            }
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void GetNoticeData(ulong objId, APOLLO_NOTICETYPE type, [MarshalAs(UnmanagedType.LPStr)] string scene, [MarshalAs(UnmanagedType.LPStr)] StringBuilder info, int size);
        public void HideNotice()
        {
            HideScrollNotice(base.ObjectId);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void HideScrollNotice(ulong objId);
        public void ShowNotice(APOLLO_NOTICETYPE type, string scene)
        {
            ShowNotice(base.ObjectId, type, scene);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void ShowNotice(ulong objId, APOLLO_NOTICETYPE type, [MarshalAs(UnmanagedType.LPStr)] string scene);
    }
}

