using System;
using System.Collections.Generic;

public class SLogObj
{
    private string filePath;
    private string lastTargetPath;
    private List<string> sLogTxt = new List<string>();
    private StreamWriterProxy streamWriter;
    private string targetPath;

    public void Close()
    {
    }

    public void Flush()
    {
    }

    public void Log(string str)
    {
    }

    public string LastTargetPath
    {
        get
        {
            return this.lastTargetPath;
        }
    }

    public string TargetPath
    {
        get
        {
            return this.targetPath;
        }
        set
        {
            this.targetPath = value;
            if (!string.IsNullOrEmpty(this.targetPath))
            {
                this.lastTargetPath = this.targetPath;
            }
            this.filePath = null;
        }
    }
}

