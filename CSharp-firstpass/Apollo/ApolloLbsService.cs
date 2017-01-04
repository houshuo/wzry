namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ApolloLbsService : ApolloObject, IApolloLbsService, IApolloServiceBase
    {
        public static readonly ApolloLbsService Instance = new ApolloLbsService();

        public event OnLocationNotifyHandle onLocationEvent;

        public event OnLocationGotNotifyHandle onLocationGotEvent;

        private ApolloLbsService()
        {
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Lbs_CleanLocation(ulong objId);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Lbs_GetLocationInfo(ulong objId);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Lbs_GetNearbyPersonInfo(ulong objId);
        public bool CleanLocation()
        {
            return Apollo_Lbs_CleanLocation(base.ObjectId);
        }

        public bool GetLocationInfo()
        {
            return Apollo_Lbs_GetLocationInfo(base.ObjectId);
        }

        public void GetNearbyPersonInfo()
        {
            Apollo_Lbs_GetNearbyPersonInfo(base.ObjectId);
        }

        private void OnLocationGotNotify(string msg)
        {
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloLocation aRelation = null;
                aRelation = parser.GetObject<ApolloLocation>("Location");
                if (this.onLocationGotEvent != null)
                {
                    try
                    {
                        this.onLocationGotEvent(aRelation);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("onLocationGotEvent:" + exception);
                    }
                }
            }
        }

        private void OnLocationNotify(string msg)
        {
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloRelation aRelation = null;
                aRelation = parser.GetObject<ApolloRelation>("Relation");
                if (this.onLocationEvent != null)
                {
                    try
                    {
                        this.onLocationEvent(aRelation);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("onLocationEvent:" + exception);
                    }
                }
            }
        }
    }
}

