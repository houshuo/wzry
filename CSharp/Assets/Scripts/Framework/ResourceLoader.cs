namespace Assets.Scripts.Framework
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ResourceLoader : Singleton<ResourceLoader>
    {
        public static string GetDatabinPath(string name)
        {
            return string.Format("jar:file://{0}!/assets/databin/{1}", Application.dataPath, name);
        }

        public void LoadDatabin(string name, BinLoadCompletedDelegate finishDelegate)
        {
            CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource(name, typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
            object[] inParameters = new object[] { name };
            DebugHelper.Assert(content != null, "load databin fail {0}", inParameters);
            byte[] data = content.m_data;
            if (finishDelegate != null)
            {
                finishDelegate(ref data);
            }
            Singleton<CResourceManager>.GetInstance().RemoveCachedResource(name);
        }

        public void LoadScene(string name, LoadCompletedDelegate finishDelegate)
        {
            Application.LoadLevel(name);
            if (finishDelegate != null)
            {
                finishDelegate();
            }
        }

        public delegate void BinLoadCompletedDelegate(ref byte[] rawData);

        public delegate void LoadCompletedDelegate();
    }
}

