using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class ConsoleViewMobile : IConsoleView
{
    public GUIStyle CustomButtonStyle;
    public GUIStyle CustomLabelStyle;
    public GUIStyle CustomSmallLabelStyle;
    public GUIStyle CustomTextFieldStyle;
    private ConsoleDragHandler DragHandler = new ConsoleDragHandler();
    private MobileLogger Logger = new MobileLogger();
    private int MaxSkipCount;
    private ConsoleWindow ParentWindow;
    private float ScrollValue;
    private StateMachine States = new StateMachine();

    public ConsoleViewMobile(ConsoleWindow InParent)
    {
        this.ParentWindow = InParent;
        object[] args = new object[] { this.ParentWindow, this };
        this.States.RegisterStateByAttributes<CommandDisplayAttribute>(typeof(CommandDisplayAttribute).Assembly, args);
        this.States.ChangeState("CommandGroupDisplayState");
    }

    public void Awake()
    {
    }

    public static void DrawEmptyLine(int lineCount)
    {
        for (int i = 0; i < lineCount; i++)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(" ", null);
            GUILayout.EndHorizontal();
        }
    }

    protected void DrawPreviousButton()
    {
        GUI.contentColor = Color.cyan;
        try
        {
            if ((this.States.TopState() as CommandDisplayBasicState).DrawButton("上一步", string.Empty))
            {
                this.skipCount = 0;
                this.States.PopState();
            }
        }
        finally
        {
            GUI.contentColor = Color.white;
        }
    }

    protected void DrawScrollButton()
    {
        GUI.contentColor = Color.yellow;
        try
        {
            if ((this.States.TopState() as CommandDisplayBasicState).DrawButton("向上移", string.Empty))
            {
                this.skipCount++;
            }
            if ((this.States.TopState() as CommandDisplayBasicState).DrawButton("向下移", string.Empty))
            {
                this.skipCount = Math.Max(0, this.skipCount - 1);
            }
            if ((this.States.TopState() as CommandDisplayBasicState).DrawButton("还原滚动", string.Empty))
            {
                this.skipCount = 0;
            }
        }
        finally
        {
            GUI.contentColor = Color.white;
        }
    }

    public void OnConsole(int InWindowID)
    {
        try
        {
            if (this.CustomButtonStyle == null)
            {
                this.CustomButtonStyle = new GUIStyle(GUI.skin.button);
                this.CustomLabelStyle = new GUIStyle(GUI.skin.label);
                this.CustomSmallLabelStyle = new GUIStyle(GUI.skin.label);
                this.CustomTextFieldStyle = new GUIStyle(GUI.skin.textField);
            }
            int num = 60;
            int num2 = 0x19;
            float num3 = 0.75f;
            float height = Screen.height;
            float width = Screen.width;
            int num6 = (int) ((num * Math.Min((float) (height / 800f), (float) (width / 1280f))) * num3);
            int num7 = (int) ((num2 * Math.Min((float) (height / 800f), (float) (width / 1280f))) * num3);
            this.CustomButtonStyle.fontSize = num6;
            this.CustomLabelStyle.fontSize = num6;
            this.CustomSmallLabelStyle.fontSize = num7;
            this.CustomTextFieldStyle.fontSize = num6;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width((float) Screen.width), GUILayout.Height((float) Screen.height) };
            GUILayout.BeginScrollView(Vector2.zero, false, false, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (this.hasPreviousState)
            {
                this.DrawPreviousButton();
            }
            if (((CommandDisplayBasicState) this.States.TopState()).canScroll)
            {
                this.DrawScrollButton();
            }
            GUILayout.EndHorizontal();
            if (this.States.TopState() != null)
            {
                (this.States.TopState() as CommandDisplayBasicState).OnGUI();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        catch (Exception exception)
        {
            DebugHelper.Assert(false, string.Format("ConsoleViewMobile Exception: {0} , Stack: {1}", exception.Message, exception.StackTrace));
        }
    }

    public void OnDestory()
    {
    }

    public void OnDisable()
    {
    }

    public void OnEnable()
    {
    }

    public void OnEnter()
    {
    }

    public void OnToggleVisible(bool bVisible)
    {
    }

    public void OnUpdate()
    {
        if (((CommandDisplayBasicState) this.States.TopState()).canScroll)
        {
            this.DragHandler.OnUpdate();
            if (this.DragHandler.isDragging)
            {
                float num = 10f;
                float num2 = this.DragHandler.dragDelta * num;
                this.ScrollValue += num2;
                if (Math.Abs(this.ScrollValue) > 1f)
                {
                    int num3 = this.skipCount + ((int) this.ScrollValue);
                    this.ScrollValue -= (int) this.ScrollValue;
                    num3 = Math.Min(this.MaxSkipCount - 1, num3);
                    this.skipCount = Math.Max(num3, 0);
                }
            }
            else
            {
                this.ScrollValue = 0f;
            }
        }
        else
        {
            this.ScrollValue = 0f;
        }
    }

    public void SelectGroup(CheatCommandGroup InGroup)
    {
        DebugHelper.Assert(InGroup != null);
        this.States.Push(new CommandInGroupDisplayState(this.ParentWindow, this));
        (this.States.TopState() as CommandInGroupDisplayState).SetGroup(InGroup);
    }

    public void SelectionCommand(ICheatCommand InCommand)
    {
        DebugHelper.Assert(InCommand != null);
        this.States.Push(new CommandDisplayState(this.ParentWindow, this));
        (this.States.TopState() as CommandDisplayState).ResetCheatCommand(InCommand);
    }

    public Rect SelectWindowRect()
    {
        return new Rect(0f, 0f, (float) Screen.width, (float) Screen.height);
    }

    public void UpdateSkipCount(int InMaxCount)
    {
        this.MaxSkipCount = InMaxCount;
        this.skipCount = Math.Min(InMaxCount - 1, this.skipCount);
    }

    protected bool hasPreviousState
    {
        get
        {
            return (this.States.Count > 1);
        }
    }

    public IConsoleLogger logger
    {
        get
        {
            return this.Logger;
        }
    }

    public int skipCount { get; private set; }
}

