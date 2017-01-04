namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;

    public class FrameCommandFactory
    {
        public static CreatorDelegate[] s_CommandCreator = null;
        public static Dictionary<System.Type, FRAMECMD_ID_DEF> s_CommandTypeDef = new Dictionary<System.Type, FRAMECMD_ID_DEF>();
        public static CreatorCSSyncDelegate[] s_CSSyncCommandCreator = null;
        public static Dictionary<System.Type, CSSYNC_TYPE_DEF> s_CSSyncCommandTypeDef = new Dictionary<System.Type, CSSYNC_TYPE_DEF>();

        public static FRAME_CMD_PKG CreateCommandPKG(IFrameCommand cmd)
        {
            FRAME_CMD_PKG frame_cmd_pkg = FRAME_CMD_PKG.New();
            frame_cmd_pkg.bCmdType = cmd.cmdType;
            frame_cmd_pkg.stCmdInfo.construct((long) frame_cmd_pkg.bCmdType);
            return frame_cmd_pkg;
        }

        public static FrameCommand<T> CreateCSSyncFrameCommand<T>() where T: struct, ICommandImplement
        {
            return new FrameCommand<T> { cmdType = (byte) s_CSSyncCommandTypeDef[typeof(T)], cmdData = default(T) };
        }

        public static FrameCommand<T> CreateFrameCommand<T>() where T: struct, ICommandImplement
        {
            return new FrameCommand<T> { cmdType = (byte) s_CommandTypeDef[typeof(T)], cmdData = default(T) };
        }

        public static IFrameCommand CreateFrameCommand(ref FRAME_CMD_PKG msg)
        {
            if ((msg.bCmdType >= 0) && (msg.bCmdType < s_CommandCreator.Length))
            {
                CreatorDelegate delegate2 = s_CommandCreator[msg.bCmdType];
                object[] objArray1 = new object[] { msg.bCmdType };
                DebugHelper.Assert(delegate2 != null, "Creator is null at index {0}", objArray1);
                return delegate2(ref msg);
            }
            object[] inParameters = new object[] { msg.bCmdType };
            DebugHelper.Assert(false, "not register framec ommand creator {0}", inParameters);
            return null;
        }

        public static IFrameCommand CreateFrameCommandByCSSyncInfo(ref CSDT_GAMING_CSSYNCINFO msg)
        {
            if ((msg.bSyncType >= 0) && (msg.bSyncType < s_CommandCreator.Length))
            {
                CreatorCSSyncDelegate delegate2 = s_CSSyncCommandCreator[msg.bSyncType];
                object[] objArray1 = new object[] { msg.bSyncType };
                DebugHelper.Assert(delegate2 != null, "Creator is null at index {0}", objArray1);
                return delegate2(ref msg);
            }
            object[] inParameters = new object[] { msg.bSyncType };
            DebugHelper.Assert(false, "not register framec ommand creator {0}", inParameters);
            return null;
        }

        public static FRAMECMD_ID_DEF GetCommandType(System.Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(FrameCommandClassAttribute), false);
            if (customAttributes.Length > 0)
            {
                return ((FrameCommandClassAttribute) customAttributes[0]).ID;
            }
            return FRAMECMD_ID_DEF.FRAME_CMD_INVALID;
        }

        public static void PrepareRegisterCommand()
        {
            Array values = Enum.GetValues(typeof(FRAMECMD_ID_DEF));
            int num = 0;
            for (int i = 0; i < values.Length; i++)
            {
                int num3 = Convert.ToInt32(values.GetValue(i));
                if (num3 > num)
                {
                    num = num3;
                }
            }
            s_CommandCreator = new CreatorDelegate[num + 1];
            values = Enum.GetValues(typeof(CSSYNC_TYPE_DEF));
            num = 0;
            for (int j = 0; j < values.Length; j++)
            {
                int num5 = Convert.ToInt32(values.GetValue(j));
                if (num5 > num)
                {
                    num = num5;
                }
            }
            s_CSSyncCommandCreator = new CreatorCSSyncDelegate[num + 1];
        }

        public static void RegisterCommandCreator(FRAMECMD_ID_DEF CmdID, System.Type CmdType, CreatorDelegate Creator)
        {
            s_CommandCreator[(int) CmdID] = Creator;
            s_CommandTypeDef.Add(CmdType, CmdID);
        }

        public static void RegisterCSSyncCommandCreator(CSSYNC_TYPE_DEF CmdID, System.Type CmdType, CreatorCSSyncDelegate Creator)
        {
            s_CSSyncCommandCreator[(int) CmdID] = Creator;
            s_CSSyncCommandTypeDef.Add(CmdType, CmdID);
        }
    }
}

