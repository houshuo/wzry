using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class CodeBuilder
{
    private List<CodeBuilder> _bodies;
    private StringBuilder _footer;
    private StringBuilder _header;
    protected int _lineIndent;

    public CodeBuilder(int indent)
    {
        this._lineIndent = indent;
        this._header = new StringBuilder(0x400);
        this._bodies = new List<CodeBuilder>();
        this._footer = new StringBuilder(0x100);
    }

    public void AddChild(CodeBuilder cb)
    {
        this._bodies.Add(cb);
    }

    public void Append(string codeFraq, bool headerOrFooter = true)
    {
        (!headerOrFooter ? this._footer : this._header).Append(codeFraq);
    }

    public void AppendIndent(bool headerOrFooter = true)
    {
        StringBuilder builder = !headerOrFooter ? this._footer : this._header;
        string str = string.Empty;
        for (int i = 0; i < this._lineIndent; i++)
        {
            str = str + "\t";
        }
        builder.Append(str);
    }

    public void AppendLine(int moveIndentBefore, string codeFraq, bool headerOrFooter = true)
    {
        this.MoveIndent(moveIndentBefore);
        this.AppendIndent(headerOrFooter);
        StringBuilder builder = !headerOrFooter ? this._footer : this._header;
        builder.Append(codeFraq);
        builder.Append("\n");
    }

    public void AppendLine(string codeFraq, int moveIndentAfter = 0, bool headerOrFooter = true)
    {
        StringBuilder builder = !headerOrFooter ? this._footer : this._header;
        this.AppendIndent(headerOrFooter);
        builder.Append(codeFraq);
        builder.Append("\n");
        this.MoveIndent(moveIndentAfter);
    }

    public CodeBuilder CreateChild(int indentOffset = 1)
    {
        CodeBuilder item = new CodeBuilder(this._lineIndent + indentOffset);
        this._bodies.Add(item);
        return item;
    }

    public int MoveIndent(int delta)
    {
        return (this._lineIndent += delta);
    }

    public void RemoveChild(CodeBuilder cb)
    {
        this._bodies.Remove(cb);
    }

    public virtual string FullText
    {
        get
        {
            StringBuilder builder = new StringBuilder(0x2800);
            builder.Append(this._header);
            for (int i = 0; i < this._bodies.Count; i++)
            {
                builder.Append(this._bodies[i].FullText);
            }
            builder.Append(this._footer);
            return builder.ToString();
        }
    }
}

