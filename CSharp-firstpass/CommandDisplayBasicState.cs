using System;
using System.Runtime.InteropServices;
using UnityEngine;

internal abstract class CommandDisplayBasicState : BaseState
{
    protected ConsoleViewMobile ParentView;
    protected ConsoleWindow ParentWindow;
    public static int SpaceHeight = 3;

    public CommandDisplayBasicState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView)
    {
        this.ParentWindow = InParentWindow;
        this.ParentView = InParentView;
    }

    public bool DrawButton(string InButtonText, string InToolTip = "")
    {
        GUIContent content = new GUIContent(InButtonText, InToolTip);
        Vector2 vector = this.ParentView.CustomButtonStyle.CalcSize(content);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(vector.x + 20f) };
        return GUILayout.Button(content, this.ParentView.CustomButtonStyle, options);
    }

    public void DrawLabel(string InLabel)
    {
        GUIContent content = new GUIContent(InLabel);
        Vector2 vector = this.ParentView.CustomLabelStyle.CalcSize(content);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(vector.x + 15f) };
        GUILayout.Label(content, this.ParentView.CustomLabelStyle, options);
    }

    public abstract void OnGUI();
    protected virtual void OnResetSkipCount()
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        this.OnResetSkipCount();
    }

    public override void OnStateResume()
    {
        base.OnStateResume();
        this.OnResetSkipCount();
    }

    public virtual bool canScroll
    {
        get
        {
            return true;
        }
    }

    public IConsoleLogger logger
    {
        get
        {
            return this.ParentView.logger;
        }
    }
}

