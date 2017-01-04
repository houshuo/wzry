namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public class ApolloAndroidResTools
    {
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern int apollo_utils_get_res_id([MarshalAs(UnmanagedType.LPStr)] string resName, [MarshalAs(UnmanagedType.LPStr)] string typeName);
        public static int GetResID(string resName, string typeName)
        {
            if (!ApolloAndroidResType.Valied(typeName))
            {
                return 0;
            }
            return apollo_utils_get_res_id(resName, typeName);
        }
    }
}

