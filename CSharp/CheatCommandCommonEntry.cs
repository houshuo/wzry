using Assets.Scripts.Framework;
using System;
using UnityEngine;

[CheatCommandEntry("工具")]
internal class CheatCommandCommonEntry
{
    public static bool CPU_CLOCK_ENABLE;
    private static string pauseSender = "pauseSender";

    [CheatCommandEntryMethod("清屏", false, true)]
    public static string Clean()
    {
        if (MonoSingleton<ConsoleWindow>.HasInstance())
        {
            MonoSingleton<ConsoleWindow>.instance.ClearLog();
        }
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("CPU频率显示/关闭", true, false)]
    public static string DisableCPUClockDisplay()
    {
        CPU_CLOCK_ENABLE = false;
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("CPU频率显示/打开", true, false)]
    public static string EnableCPUClockDisplay()
    {
        CPU_CLOCK_ENABLE = true;
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("退出(Exit)", false, true)]
    public static string Exit()
    {
        if (MonoSingleton<ConsoleWindow>.HasInstance())
        {
            MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        }
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("帧率/-5", true, false)]
    public static string FramerateMinus5()
    {
        GameFramework.c_renderFPS -= 5;
        GameFramework.c_targetFrameTime = 1000f / ((float) GameFramework.c_renderFPS);
        Application.targetFrameRate = GameFramework.c_renderFPS;
        return GameFramework.c_renderFPS.ToString();
    }

    [CheatCommandEntryMethod("帧率/+5", true, false)]
    public static string FrameratePlus5()
    {
        GameFramework.c_renderFPS += 5;
        GameFramework.c_targetFrameTime = 1000f / ((float) GameFramework.c_renderFPS);
        Application.targetFrameRate = GameFramework.c_renderFPS;
        return GameFramework.c_renderFPS.ToString();
    }

    [CheatCommandEntryMethod("暂停/暂停", true, false)]
    public static string PauseLogic()
    {
        Singleton<CBattleGuideManager>.GetInstance().PauseGame(pauseSender, true);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("退出(Quit)", false, true)]
    public static string Quit()
    {
        if (MonoSingleton<ConsoleWindow>.HasInstance())
        {
            MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        }
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/刷新活动状态", true, false)]
    public static string RefreshActivityStates()
    {
        Singleton<ActivitySys>.GetInstance().RequestRefresh(0);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/游戏恢复", true, false)]
    public static string RestoreGame()
    {
        Singleton<GameBuilder>.GetInstance().RestoreGame();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("暂停/恢复", true, false)]
    public static string ResumeLogic()
    {
        Singleton<CBattleGuideManager>.GetInstance().ResumeGame(pauseSender);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/设置漂移[1-10]", true, false)]
    public static string SetFactorDrift(int InFlag)
    {
        Singleton<FrameSynchr>.GetInstance().nDriftFactor = Mathf.Clamp(InFlag, 1, 10);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/帧冗余数", true, false)]
    public static string ShowCommandPing()
    {
        Singleton<FrameWindow>.GetInstance().ToggleShowFrameSpareChart();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/游戏存档", true, false)]
    public static string StoreGame()
    {
        Singleton<GameBuilder>.GetInstance().StoreGame();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("游戏/开关录像", true, false)]
    public static string ToggleReplay()
    {
        GameSettings.enableReplay = !GameSettings.enableReplay;
        return (!GameSettings.enableReplay ? "关闭" : "开启");
    }

    [CheatCommandEntryMethod("更换视图", false, true)]
    public static string ToggleView(EConsoleView InView)
    {
        if (MonoSingleton<ConsoleWindow>.HasInstance())
        {
            if (InView == EConsoleView.PC)
            {
                MonoSingleton<ConsoleWindow>.instance.ChangeToPCView();
            }
            else if (InView == EConsoleView.Mobile)
            {
                MonoSingleton<ConsoleWindow>.instance.ChangeToMobileView();
            }
        }
        return CheatCommandBase.Done;
    }

    public enum EConsoleView
    {
        PC,
        Mobile
    }
}

