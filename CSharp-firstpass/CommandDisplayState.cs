using System;
using System.Collections.Generic;
using UnityEngine;

[CommandDisplay]
internal class CommandDisplayState : CommandDisplayBasicState
{
    private ICheatCommand CheatCommand;

    public CommandDisplayState(ConsoleWindow InParentWindow, ConsoleViewMobile InParentView) : base(InParentWindow, InParentView)
    {
    }

    private bool DrawArgument(ArgumentDescriptionAttribute InArgAttr, int InIndex, ArgumentDescriptionAttribute[] InArgTypes, ref string[] OutValues, ref string OutValue)
    {
        if (InArgAttr.isOptional && this.ShouldSkip(InArgAttr, ref OutValues))
        {
            return false;
        }
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        base.DrawLabel(InArgAttr.name);
        string name = string.Format("Argument_{0}", GUIUtility.GetControlID(FocusType.Keyboard));
        GUI.SetNextControlName(name);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(200f) };
        OutValue = GUILayout.TextField(OutValue, base.ParentView.CustomTextFieldStyle, options);
        if (GUI.GetNameOfFocusedControl() == name)
        {
            IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(InArgAttr.argumentType);
            DebugHelper.Assert(description != null);
            List<string> list = description.FilteredCandinates(InArgAttr.argumentType, OutValue);
            if ((list != null) && (list.Count > 0))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string inButtonText = list[i];
                    if (!inButtonText.Equals(OutValue, StringComparison.InvariantCultureIgnoreCase) && base.DrawButton(inButtonText, string.Empty))
                    {
                        OutValue = inButtonText;
                        break;
                    }
                }
            }
        }
        GUILayout.EndHorizontal();
        return true;
    }

    protected void DrawArugments()
    {
        ArgumentDescriptionAttribute[] argumentsTypes = this.CheatCommand.argumentsTypes;
        string[] arguments = this.CheatCommand.arguments;
        int num = 0;
        if ((argumentsTypes != null) && (argumentsTypes.Length > 0))
        {
            DebugHelper.Assert(argumentsTypes.Length == arguments.Length);
            for (int i = 0; i < argumentsTypes.Length; i++)
            {
                ArgumentDescriptionAttribute inArgAttr = argumentsTypes[i];
                if (!this.DrawArgument(inArgAttr, i, argumentsTypes, ref arguments, ref arguments[i]))
                {
                    break;
                }
                num++;
            }
        }
        if (base.DrawButton(this.CheatCommand.comment, string.Empty))
        {
            base.logger.AddMessage(this.CheatCommand.StartProcess(arguments));
        }
    }

    public override void OnGUI()
    {
        if (this.CheatCommand != null)
        {
            GUI.contentColor = Color.green;
            GUILayout.Label(this.CheatCommand.fullyHelper, base.ParentView.CustomSmallLabelStyle, new GUILayoutOption[0]);
            GUI.contentColor = Color.yellow;
            GUILayout.Label(base.logger.message, base.ParentView.CustomSmallLabelStyle, new GUILayoutOption[0]);
            GUI.contentColor = Color.white;
            this.DrawArugments();
        }
    }

    public override void OnStateEnter()
    {
        base.logger.Clear();
    }

    public void ResetCheatCommand(ICheatCommand InCommand)
    {
        this.CheatCommand = InCommand;
    }

    private bool ShouldSkip(ArgumentDescriptionAttribute InArgAttr, ref string[] ExistsValues)
    {
        DebugHelper.Assert(InArgAttr.isOptional);
        foreach (DependencyDescription description in InArgAttr.depends)
        {
            string inArgument = ExistsValues[description.dependsIndex];
            System.Type argumentType = this.CheatCommand.argumentsTypes[description.dependsIndex].argumentType;
            IArgumentDescription description2 = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(argumentType);
            DebugHelper.Assert(description2 != null);
            inArgument = description2.GetValue(argumentType, inArgument);
            if (description.ShouldBackOff(inArgument))
            {
                return false;
            }
        }
        return true;
    }

    public override bool canScroll
    {
        get
        {
            return false;
        }
    }
}

