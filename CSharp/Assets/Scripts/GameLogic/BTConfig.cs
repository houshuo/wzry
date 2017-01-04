namespace Assets.Scripts.GameLogic
{
    using behaviac;
    using System;
    using UnityEngine;

    public class BTConfig
    {
        public static bool s_bBlock;
        public static Workspace.EFileFormat s_FileFormat = Workspace.EFileFormat.EFF_cs;
        public static bool s_isMakeMeta;
        public static string s_MetaPath = "BTWorkspace/xmlmeta/metas.xml";

        public static void SetBTConfig()
        {
            Workspace.SetWorkspaceSettings(WorkspaceExportedPath, s_FileFormat);
            if (s_isMakeMeta)
            {
                Workspace.ExportMetas(s_MetaPath);
            }
        }

        public static void StopBTConnect()
        {
        }

        public static string WorkspaceExportedPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return (Application.dataPath + "/Resources/BTData");
                }
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return (Application.dataPath + "/Resources/BTData");
                }
                return "Assets/Resources/BTData";
            }
        }
    }
}

