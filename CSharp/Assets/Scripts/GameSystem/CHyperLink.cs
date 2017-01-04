namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;

    public class CHyperLink
    {
        public const char CommandDelimiter = '|';
        public const char EndChar = ']';
        public const char ParamDelimiter = ',';
        public const char StartChar = '[';

        public static bool Bind(GameObject target, string hyperlink)
        {
            if (!string.IsNullOrEmpty(hyperlink))
            {
                int num;
                char[] separator = new char[] { '|' };
                string[] strArray = hyperlink.Split(separator);
                if (strArray.Length != 2)
                {
                    return false;
                }
                if (!int.TryParse(strArray[0], out num))
                {
                    return false;
                }
                char[] chArray2 = new char[] { ',' };
                string[] strArray2 = strArray[1].Split(chArray2);
                CUIEventScript component = target.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                switch (((COM_HYPERLINK_TYPE) num))
                {
                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_GUILD_INVITE:
                        if (strArray2.Length == 4)
                        {
                            ulong num3 = ulong.Parse(strArray2[0]);
                            int num4 = int.Parse(strArray2[1]);
                            ulong num5 = ulong.Parse(strArray2[2]);
                            int num6 = int.Parse(strArray2[3]);
                            eventParams.commonUInt64Param1 = num3;
                            eventParams.tag = num4;
                            eventParams.commonUInt64Param2 = num5;
                            eventParams.tag2 = num6;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Hyperlink_Search_Guild, eventParams);
                            Utility.GetComponetInChild<Text>(target, "Text").text = Singleton<CTextManager>.instance.GetText("Common_Check");
                            return true;
                        }
                        return false;

                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_PREGUILD_INVITE:
                        if (strArray2.Length == 1)
                        {
                            ulong num8 = ulong.Parse(strArray2[0]);
                            eventParams.commonUInt64Param1 = num8;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Hyperlink_Search_PrepareGuild, eventParams);
                            Utility.GetComponetInChild<Text>(target, "Text").text = Singleton<CTextManager>.instance.GetText("Common_Check");
                            return true;
                        }
                        return false;

                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_FORM_JUMP:
                    {
                        int result = 0;
                        if (!int.TryParse(strArray2[0], out result))
                        {
                            return false;
                        }
                        eventParams.tag = result;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Mail_JumpForm, eventParams);
                        if (strArray2.Length != 2)
                        {
                            Utility.GetComponetInChild<Text>(target, "Text").text = Singleton<CTextManager>.instance.GetText("Common_Check");
                        }
                        else
                        {
                            Utility.GetComponetInChild<Text>(target, "Text").text = strArray2[1];
                        }
                        return true;
                    }
                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_URL:
                        eventParams.tagStr = strArray2[0];
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Mail_JumpUrl, eventParams);
                        if (strArray2.Length != 2)
                        {
                            Utility.GetComponetInChild<Text>(target, "Text").text = Singleton<CTextManager>.instance.GetText("Common_Check");
                        }
                        else
                        {
                            Utility.GetComponetInChild<Text>(target, "Text").text = strArray2[1];
                        }
                        return true;
                }
            }
            return false;
        }

        public static bool IsStandCommond(string hyperlink)
        {
            if (!string.IsNullOrEmpty(hyperlink))
            {
                int num;
                char[] separator = new char[] { '|' };
                string[] strArray = hyperlink.Split(separator);
                if (strArray.Length != 2)
                {
                    return false;
                }
                if (!int.TryParse(strArray[0], out num))
                {
                    return false;
                }
                char[] chArray2 = new char[] { ',' };
                string[] strArray2 = strArray[1].Split(chArray2);
                switch (((COM_HYPERLINK_TYPE) num))
                {
                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_GUILD_INVITE:
                        return (strArray2.Length == 4);

                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_PREGUILD_INVITE:
                        return (strArray2.Length == 1);

                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_FORM_JUMP:
                    {
                        int result = 0;
                        if (!int.TryParse(strArray2[0], out result))
                        {
                            return false;
                        }
                        return true;
                    }
                    case COM_HYPERLINK_TYPE.COM_HYPERLINK_TYPE_URL:
                        return true;
                }
            }
            return false;
        }
    }
}

