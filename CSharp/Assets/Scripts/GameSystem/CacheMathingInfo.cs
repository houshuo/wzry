namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;

    public class CacheMathingInfo
    {
        public COM_AI_LEVEL AILevel = COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE;
        public bool CanGameAgain;
        public uint mapId;
        public byte mapType;
        public enUIEventID uiEventId;
    }
}

