namespace Apollo
{
    using System;

    public interface IApolloLbsService : IApolloServiceBase
    {
        event OnLocationNotifyHandle onLocationEvent;

        event OnLocationGotNotifyHandle onLocationGotEvent;

        bool CleanLocation();
        bool GetLocationInfo();
        void GetNearbyPersonInfo();
    }
}

