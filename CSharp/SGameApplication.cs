using System;
using System.Diagnostics;
using UnityEngine;

public static class SGameApplication
{
    public static void Quit()
    {
        try
        {
            Singleton<NetworkModule>.GetInstance().CloseAllServerConnect();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        catch (Exception)
        {
            Application.Quit();
        }
    }
}

