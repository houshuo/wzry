using Assets.Scripts.UI;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FPSForm : CUIFormScript
{
    private Color color = Color.white;
    public static string extMsg = string.Empty;
    public Text m_fpsText;
    private string revision = string.Empty;
    public static string sFPS = string.Empty;

    private void Start()
    {
        CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource("Revision.txt", typeof(TextAsset), enResourceType.Numeric, false, false).m_content as CBinaryObject;
        if (null != content)
        {
            this.revision = Encoding.UTF8.GetString(content.m_data);
        }
        Singleton<CResourceManager>.GetInstance().RemoveCachedResource("Revision.txt");
    }
}

