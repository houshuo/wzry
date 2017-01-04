namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    internal class IIPSMobileErrorCodeCheck
    {
        private int m_nLastCheckErrorCode;

        public ErrorCodeInfo CheckIIPSErrorCode(int nErrorCode)
        {
            ErrorCodeInfo info;
            this.m_nLastCheckErrorCode = nErrorCode;
            int firstModuleType = this.GetFirstModuleType();
            int secondModuleType = this.GetSecondModuleType();
            int errorCodeType = this.GetErrorCodeType();
            int realErrorCode = this.GetRealErrorCode();
            int secondModuleNoticeErrorType = 0;
            if (errorCodeType != 1)
            {
                switch (errorCodeType)
                {
                    case 2:
                        switch (realErrorCode)
                        {
                            case 0x70:
                            case 0x27:
                            case 0x1c:
                                secondModuleNoticeErrorType = 3;
                                goto Label_0144;
                        }
                        secondModuleNoticeErrorType = 4;
                        goto Label_0144;

                    case 3:
                        switch (firstModuleType)
                        {
                            case 1:
                                secondModuleNoticeErrorType = 5;
                                goto Label_0144;

                            case 2:
                                secondModuleNoticeErrorType = this.GetSecondModuleNoticeErrorType(secondModuleType, realErrorCode);
                                goto Label_0144;
                        }
                        secondModuleNoticeErrorType = 5;
                        goto Label_0144;

                    case 4:
                        switch (realErrorCode)
                        {
                            case 0x70:
                            case 0x27:
                            case 0x1c:
                                secondModuleNoticeErrorType = 3;
                                goto Label_0144;
                        }
                        secondModuleNoticeErrorType = 4;
                        goto Label_0144;

                    case 5:
                        secondModuleNoticeErrorType = 5;
                        goto Label_0144;
                }
                if (errorCodeType == 5)
                {
                    secondModuleNoticeErrorType = 5;
                }
                else
                {
                    secondModuleNoticeErrorType = 5;
                }
                goto Label_0144;
            }
            int downloadErrorType = this.GetDownloadErrorType(realErrorCode);
            int readDownloadErrorCode = this.GetReadDownloadErrorCode(realErrorCode);
            switch (downloadErrorType)
            {
                case 1:
                    secondModuleNoticeErrorType = 2;
                    readDownloadErrorCode = realErrorCode;
                    break;

                case 2:
                    secondModuleNoticeErrorType = 1;
                    break;

                case 5:
                    switch (readDownloadErrorCode)
                    {
                        case 0x70:
                        case 0x27:
                        case 0x1c:
                            secondModuleNoticeErrorType = 3;
                            goto Label_0098;
                    }
                    secondModuleNoticeErrorType = 4;
                    break;

                default:
                    secondModuleNoticeErrorType = 5;
                    break;
            }
        Label_0098:
            realErrorCode = readDownloadErrorCode;
        Label_0144:
            info = new ErrorCodeInfo(false);
            info.m_bCheckOk = true;
            info.m_nErrorType = secondModuleNoticeErrorType;
            info.m_nFirstModule = firstModuleType;
            info.m_nSecondModule = secondModuleType;
            info.m_nInsideErrorType = errorCodeType;
            info.m_nErrorCode = realErrorCode;
            info.m_nLastCheckError = this.m_nLastCheckErrorCode;
            return info;
        }

        private int GetDownloadErrorType(int downloaderror)
        {
            int num = downloaderror >> 0x10;
            return (num & 15);
        }

        private int GetErrorCodeType()
        {
            int num = this.m_nLastCheckErrorCode >> 20;
            return (num & 7);
        }

        private int GetFirstModuleType()
        {
            int num = this.m_nLastCheckErrorCode >> 0x17;
            return (num & 7);
        }

        private int GetReadDownloadErrorCode(int downloaderror)
        {
            return (downloaderror & 0xffff);
        }

        private int GetRealErrorCode()
        {
            return (this.m_nLastCheckErrorCode & 0xfffff);
        }

        private int GetSecondModuleNoticeErrorType(int secondModule, int errorcode)
        {
            if (secondModule != 1)
            {
                if (secondModule == 2)
                {
                    if (((errorcode == 15) || (errorcode == 0x10)) || ((errorcode == 0x11) || (errorcode == 0x16)))
                    {
                        return 6;
                    }
                    return 1;
                }
                if (secondModule == 5)
                {
                    if (errorcode == 4)
                    {
                        return 4;
                    }
                    if (errorcode == 0xfa6)
                    {
                        return 8;
                    }
                    return 5;
                }
                if (secondModule == 6)
                {
                    return 7;
                }
            }
            return 5;
        }

        private int GetSecondModuleType()
        {
            int num = this.m_nLastCheckErrorCode >> 0x1a;
            return (num & 15);
        }

        public enum error_type
        {
            error_type_init,
            error_type_network,
            error_type_net_timeout,
            error_type_device_hasno_space,
            error_type_other_system_error,
            error_type_other_error,
            error_type_cur_version_not_support_update,
            error_type_can_not_sure,
            error_type_cur_net_not_support_update
        }

        public enum error_type_inside
        {
            error_type_inside_init,
            error_type_inside_download_error,
            error_type_inside_system_error,
            error_type_inside_module_error,
            error_type_inside_ifs_error,
            error_type_inside_should_not_happen
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ErrorCodeInfo
        {
            public bool m_bCheckOk;
            public int m_nErrorType;
            public int m_nFirstModule;
            public int m_nSecondModule;
            public int m_nInsideErrorType;
            public int m_nErrorCode;
            public int m_nLastCheckError;
            public ErrorCodeInfo(bool bOk)
            {
                this.m_bCheckOk = bOk;
                this.m_nFirstModule = 0;
                this.m_nSecondModule = 0;
                this.m_nErrorType = 0;
                this.m_nInsideErrorType = 0;
                this.m_nErrorCode = 0;
                this.m_nLastCheckError = 0;
            }
        }

        public enum first_module
        {
            first_module_init,
            first_module_data,
            first_module_version
        }

        public enum second_module
        {
            second_module_apkupdate_action = 5,
            second_module_datadownloader = 2,
            second_module_datamanager = 1,
            second_module_dataqueryer = 3,
            second_module_datareader = 4,
            second_module_extract_action = 4,
            second_module_init = 0,
            second_module_srcupdate_cures_action = 8,
            second_module_srcupdate_filediff_action = 9,
            second_module_srcupdate_fulldiff_action = 6,
            second_module_srcupdate_mergeifs_action = 7,
            second_module_update_action = 3,
            second_module_version_action = 2,
            second_module_versionmanager = 1
        }
    }
}

