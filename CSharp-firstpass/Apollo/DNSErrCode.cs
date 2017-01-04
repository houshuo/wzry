namespace Apollo
{
    using System;

    public enum DNSErrCode
    {
        DNS_ALL_IP_USED_ERROR = 0x456,
        DNS_HANDLE_FAILED = 0x459,
        DNS_HTTP_RESPONSE_ERROR = 0x453,
        DNS_INIT_FAILED = 0x3e9,
        DNS_INPUT_PARAM_ERROR = 0x44e,
        DNS_INPUT_USERDATA_ERROR = 0x44f,
        DNS_JSON_CONSTRUCT_ERROR = 0x451,
        DNS_JSON_PARSE_ERROR = 0x452,
        DNS_NETWORK_UNAVAILBALE = 0x44c,
        DNS_NO_ERROR = 0,
        DNS_READ_CONFIG_ERROR = 0x44d,
        DNS_RESPONSE_DATA_EMPTY = 0x454,
        DNS_SEND_REQUEST_ERROR = 0x455,
        DNS_TOOMANY_DOMAINNAMES = 0x450,
        DNS_UN_INIT = 0x3e8,
        DNS_UPDATE_IP_ERROR = 0x457,
        DNS_WAIT_RESPONSE_TIMEOUT = 0x458
    }
}

