namespace Assets.Scripts.GameSystem
{
    using System;

    public enum enVersionUpdateState
    {
        None,
        StartCheckPathPermission,
        CheckPathPermission,
        StartCheckAppVersion,
        CheckAppVersion,
        DownloadApp,
        InstallApp,
        FinishUpdateApp,
        DownloadYYB,
        StartCheckFirstExtractResource,
        CheckFirstExtractResource,
        FirstExtractResource,
        FinishFirstExtractResouce,
        StartCheckResourceVersion,
        CheckResourceVersion,
        DownloadResource,
        FinishUpdateResource,
        Complete,
        End
    }
}

