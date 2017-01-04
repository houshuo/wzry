using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CommandDisplay]
internal class CommandInGroupDisplayState : CommandDisplayBasicState
{
    private int Count;

    public CommandInGroupDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
    {
    }

    protected void DrawCommands()
    {
        DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.currentGroup.Commands.GetEnumerator();
        int num = 0;
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, ICheatCommand> current = enumerator.Current;
            if (!current.Value.isHiddenInMobile)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                try
                {
                    KeyValuePair<string, ICheatCommand> pair2 = enumerator.Current;
                    string comment = pair2.Value.comment;
                    if (this.Count++ >= base.ParentView.skipCount)
                    {
                        KeyValuePair<string, ICheatCommand> pair3 = enumerator.Current;
                        if (base.DrawButton(comment, pair3.Value.fullyHelper))
                        {
                            KeyValuePair<string, ICheatCommand> pair4 = enumerator.Current;
                            ICheatCommand command = pair4.Value;
                            if ((command.argumentsTypes == null) || (command.argumentsTypes.Length == 0))
                            {
                                string[] inArguments = new string[] { string.Empty };
                                base.logger.AddMessage(command.StartProcess(inArguments));
                            }
                            else
                            {
                                base.logger.Clear();
                                KeyValuePair<string, ICheatCommand> pair5 = enumerator.Current;
                                base.ParentView.SelectionCommand(pair5.Value);
                                break;
                            }
                        }
                        GUILayout.Label(GUI.tooltip, base.ParentView.CustomLabelStyle, new GUILayoutOption[0]);
                        GUI.tooltip = string.Empty;
                    }
                    num++;
                }
                finally
                {
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space((float) CommandDisplayBasicState.SpaceHeight);
            }
        }
    }

    protected void DrawGroups()
    {
        DictionaryView<string, CheatCommandGroup>.Enumerator enumerator = this.currentGroup.ChildrenGroups.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, CheatCommandGroup> current = enumerator.Current;
            string key = current.Key;
            if ((this.Count++ >= base.ParentView.skipCount) && base.DrawButton(key, string.Empty))
            {
                KeyValuePair<string, CheatCommandGroup> pair2 = enumerator.Current;
                base.ParentView.SelectGroup(pair2.Value);
            }
            GUILayout.Space((float) CommandDisplayBasicState.SpaceHeight);
        }
    }

    public override void OnGUI()
    {
        if (this.currentGroup != null)
        {
            if (!string.IsNullOrEmpty(base.logger.message))
            {
                GUI.contentColor = Color.yellow;
                GUILayout.Label(base.logger.message, base.ParentView.CustomLabelStyle, new GUILayoutOption[0]);
            }
            this.Count = 0;
            GUI.contentColor = Color.green;
            try
            {
                this.DrawGroups();
            }
            finally
            {
                GUI.contentColor = Color.white;
            }
            this.DrawCommands();
        }
    }

    protected override void OnResetSkipCount()
    {
        if (this.currentGroup != null)
        {
            int num = 0;
            DictionaryView<string, ICheatCommand>.Enumerator enumerator = this.currentGroup.Commands.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, ICheatCommand> current = enumerator.Current;
                if (!current.Value.isHiddenInMobile)
                {
                    num++;
                }
            }
            base.ParentView.UpdateSkipCount(this.currentGroup.ChildrenGroups.Count + num);
        }
    }

    public void SetGroup(CheatCommandGroup InGroup)
    {
        this.currentGroup = InGroup;
        this.OnResetSkipCount();
    }

    public CheatCommandGroup currentGroup { get; protected set; }
}

