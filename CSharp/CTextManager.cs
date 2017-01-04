using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

public class CTextManager : Singleton<CTextManager>
{
    private Dictionary<string, string> m_textMap;

    public string GetText(string key)
    {
        string str = string.Empty;
        this.m_textMap.TryGetValue(key, out str);
        if (string.IsNullOrEmpty(str))
        {
            str = key;
        }
        return str;
    }

    public string GetText(string key, params string[] args)
    {
        string text = Singleton<CTextManager>.GetInstance().GetText(key);
        if (text == null)
        {
            return ("text with tag [" + key + "] was not found!");
        }
        return string.Format(text, (object[]) args);
    }

    public override void Init()
    {
    }

    public bool IsTextLoaded()
    {
        return (this.m_textMap != null);
    }

    public void LoadLocalText()
    {
        DatabinTable<ResText, ushort> textDataBin = new DatabinTable<ResText, ushort>("Databin/Client/Text/Text.bytes", "wID");
        this.LoadText(textDataBin);
    }

    public void LoadText(DatabinTable<ResText, ushort> textDataBin)
    {
        if (textDataBin != null)
        {
            this.m_textMap = new Dictionary<string, string>();
            Dictionary<long, object>.Enumerator enumerator = textDataBin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResText text = (ResText) current.Value;
                this.m_textMap.Add(StringHelper.UTF8BytesToString(ref text.szKey), StringHelper.UTF8BytesToString(ref text.szValue));
            }
        }
    }
}

