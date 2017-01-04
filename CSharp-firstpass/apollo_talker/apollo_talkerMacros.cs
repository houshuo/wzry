namespace apollo_talker
{
    using System;

    public class apollo_talkerMacros
    {
        public const int APOLLO_MAX_CMD_LEN = 0x40;
        public const int CMD_FMT_INT = 2;
        public const int CMD_FMT_NIL = 0;
        public const int CMD_FMT_STR = 1;
        public const int DATA_FLOW_DOWN = 8;
        public const int DATA_FLOW_MASK = 8;
        public const int DATA_FLOW_UP = 0;
        public const int DATA_FMT_BIN = 0x10;
        public const int DATA_FMT_MASK = 0x10;
        public const int DATA_FMT_MSG = 0;
        public const int DATA_TYPE_MASK = 7;
        public const int DATA_TYPE_NOTICE = 0;
        public const int DATA_TYPE_REQUEST = 1;
        public const int DATA_TYPE_RESPONSE = 2;
        public const int DOMAIN_APP = 1;
        public const int DOMAIN_COUNT = 0x100;
        public const int DOMAIN_TSS = 0xff;
    }
}

