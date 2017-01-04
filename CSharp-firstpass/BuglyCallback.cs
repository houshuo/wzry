using System;
using UnityEngine;

public abstract class BuglyCallback
{
    protected BuglyCallback()
    {
    }

    public abstract void OnApplicationLogCallbackHandler(string condition, string stackTrace, LogType type);
}

