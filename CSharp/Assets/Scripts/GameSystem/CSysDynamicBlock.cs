namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.CompilerServices;

    [MessageHandlerClass]
    internal class CSysDynamicBlock
    {
        private static bool _bNewbieBlocked;

        [MessageHandler(0x100e)]
        public static void OnSysBlock(CSPkg msg)
        {
            uint[] numArray = msg.stPkgData.stFunctionSwitchNtf.Switch;
            int num = numArray.Length * 0x20;
            bool[] flagArray = new bool[num];
            for (int i = 0; i < num; i++)
            {
                int index = i / 0x20;
                int num4 = i % 0x20;
                flagArray[i] = (numArray[index] & (((int) 1) << num4)) > 0;
            }
            bNewbieBlocked = flagArray[0];
            bLobbyEntryBlocked = flagArray[1];
            bFriendBlocked = flagArray[2];
            bSocialBlocked = flagArray[3];
            bOperationBlock = flagArray[4];
            bDialogBlock = flagArray[5];
            bUnfinishBlock = flagArray[7];
            bVipBlock = flagArray[6];
            bChatPayBlock = flagArray[10];
            bJifenHallBlock = flagArray[11];
        }

        public static bool bChatPayBlock
        {
            [CompilerGenerated]
            get
            {
                return <bChatPayBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bChatPayBlock>k__BackingField = value;
            }
        }

        public static bool bDialogBlock
        {
            [CompilerGenerated]
            get
            {
                return <bDialogBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bDialogBlock>k__BackingField = value;
            }
        }

        public static bool bFriendBlocked
        {
            [CompilerGenerated]
            get
            {
                return <bFriendBlocked>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bFriendBlocked>k__BackingField = value;
            }
        }

        public static bool bJifenHallBlock
        {
            [CompilerGenerated]
            get
            {
                return <bJifenHallBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bJifenHallBlock>k__BackingField = value;
            }
        }

        public static bool bLobbyEntryBlocked
        {
            [CompilerGenerated]
            get
            {
                return <bLobbyEntryBlocked>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bLobbyEntryBlocked>k__BackingField = value;
            }
        }

        public static bool bNewbieBlocked
        {
            get
            {
                return _bNewbieBlocked;
            }
            private set
            {
                _bNewbieBlocked = value;
                if (_bNewbieBlocked)
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().newbieGuideEnable = false;
                }
            }
        }

        public static bool bOperationBlock
        {
            [CompilerGenerated]
            get
            {
                return <bOperationBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bOperationBlock>k__BackingField = value;
            }
        }

        public static bool bSocialBlocked
        {
            [CompilerGenerated]
            get
            {
                return <bSocialBlocked>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bSocialBlocked>k__BackingField = value;
            }
        }

        public static bool bUnfinishBlock
        {
            [CompilerGenerated]
            get
            {
                return <bUnfinishBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bUnfinishBlock>k__BackingField = value;
            }
        }

        public static bool bVipBlock
        {
            [CompilerGenerated]
            get
            {
                return <bVipBlock>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <bVipBlock>k__BackingField = value;
            }
        }
    }
}

