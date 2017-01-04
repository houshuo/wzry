using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public abstract class TemplateMarkerBase : MonoBehaviour
{
    public bool m_isUnique = true;
    public bool m_mainMark;
    public MarkerType m_markType;

    protected TemplateMarkerBase()
    {
    }

    public abstract bool Check(GameObject targetObject, out string errorInfo);
    protected bool isWildCardMatch(string targetString, string wildCard)
    {
        string pattern = this.wildcard2Regex(wildCard);
        return Regex.IsMatch(targetString, pattern);
    }

    protected string wildcard2Regex(string wildCard)
    {
        return ("^" + Regex.Escape(wildCard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$");
    }

    public enum MarkerType
    {
        eEssential,
        eOptional
    }
}

