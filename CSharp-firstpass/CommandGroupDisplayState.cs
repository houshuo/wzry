using System;
using System.Collections.Generic;
using UnityEngine;

[CommandDisplay]
internal class CommandGroupDisplayState : CommandDisplayBasicState
{
    public CommandGroupDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
    {
    }

    public override void OnGUI()
    {
        GUI.contentColor = Color.green;
        try
        {
            DictionaryView<string, CheatCommandGroup> repositories = Singleton<CheatCommandsRepository>.instance.repositories;
            DebugHelper.Assert(repositories != null);
            DictionaryView<string, CheatCommandGroup>.Enumerator enumerator = repositories.GetEnumerator();
            int num = 0;
            int num2 = 0;
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, CheatCommandGroup> current = enumerator.Current;
                string key = current.Key;
                if ((num2++ >= base.ParentView.skipCount) && base.DrawButton(key, string.Empty))
                {
                    KeyValuePair<string, CheatCommandGroup> pair2 = enumerator.Current;
                    base.ParentView.SelectGroup(pair2.Value);
                    return;
                }
                GUILayout.Space((float) CommandDisplayBasicState.SpaceHeight);
                num++;
            }
        }
        finally
        {
            GUI.contentColor = Color.white;
        }
    }

    protected override void OnResetSkipCount()
    {
        base.ParentView.UpdateSkipCount(Singleton<CheatCommandsRepository>.instance.repositories.Count);
    }

    public override bool canScroll
    {
        get
        {
            return true;
        }
    }
}

