using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class CodeContext : CodeBuilder
{
    public const string GLOBAL_NS = "_global_";
    public string nameSpace;
    public string savePath;
    public Dictionary<string, object> usingNss;

    public CodeContext(string nameSpace, string savePath) : base(0)
    {
        this.nameSpace = nameSpace;
        this.savePath = savePath;
        this.usingNss = new Dictionary<string, object>();
    }

    public void AddUseNs(string usedNameSpace)
    {
        if (!string.IsNullOrEmpty(usedNameSpace) && !this.usingNss.ContainsKey(usedNameSpace))
        {
            this.usingNss.Add(usedNameSpace, null);
        }
    }

    public void AddUseType(Type type)
    {
        this.AddUseNs(type.Namespace);
    }

    public void MergeChild(CodeContext cc)
    {
        Dictionary<string, object>.Enumerator enumerator = cc.usingNss.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, object> current = enumerator.Current;
            this.AddUseNs(current.Key);
        }
        cc.usingNss.Clear();
        base.AddChild(cc);
    }

    public void Save(string optFileName = null)
    {
        try
        {
            File.WriteAllText(this.savePath + "/" + (!string.IsNullOrEmpty(optFileName) ? optFileName : (this.nameSpace + ".cs")), this.FullText, Encoding.UTF8);
        }
        catch (Exception)
        {
        }
    }

    public override string FullText
    {
        get
        {
            StringBuilder builder = new StringBuilder(0x2800);
            Dictionary<string, object>.Enumerator enumerator = this.usingNss.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, object> current = enumerator.Current;
                builder.AppendLine("using " + current.Key + ";");
            }
            if (this.nameSpace != "_global_")
            {
                builder.AppendLine("namespace " + this.nameSpace + " {");
            }
            builder.Append(base.FullText);
            if (this.nameSpace != "_global_")
            {
                builder.AppendLine("}");
            }
            return builder.ToString();
        }
    }
}

