using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class CommandParser
{
    public static readonly string RegexPattern = " (?=(?:[^\\\"]*\\\"[^\\\"]*\\\")*[^\\\"]*$)";

    public CommandParser()
    {
        string str = string.Empty;
        this.baseCommand = str;
        this.text = str;
    }

    public void Parse(string InText)
    {
        this.text = InText;
        this.sections = Regex.Split(this.text, RegexPattern);
        for (int i = 0; i < this.sections.Length; i++)
        {
            char[] trimChars = new char[] { '"' };
            this.sections[i] = this.sections[i].Trim(trimChars);
        }
        if (this.sections.Length > 0)
        {
            this.baseCommand = this.sections[0];
        }
        else
        {
            this.baseCommand = string.Empty;
        }
        if (this.sections.Length > 1)
        {
            this.arguments = LinqS.Skip(this.sections, 1);
        }
        else
        {
            this.arguments = null;
        }
    }

    public string[] arguments { get; protected set; }

    public string baseCommand { get; protected set; }

    public string[] sections { get; protected set; }

    public string text { get; protected set; }
}

