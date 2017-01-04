using Assets.Scripts.GameSystem;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class CChatParser
{
    public static int[] ascii_width = new int[] { 
        7, 6, 7, 15, 12, 20, 15, 4, 9, 9, 10, 0x11, 6, 6, 6, 6, 
        12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 6, 6, 0x11, 0x11, 0x11, 11, 
        20, 15, 13, 15, 0x10, 12, 11, 0x12, 0x10, 7, 10, 14, 10, 20, 0x11, 0x12, 
        13, 0x12, 14, 13, 11, 0x10, 14, 20, 14, 13, 14, 8, 6, 8, 20, 10, 
        10, 14, 14, 11, 14, 13, 7, 14, 13, 6, 7, 13, 6, 0x13, 13, 13, 
        14, 14, 9, 10, 8, 13, 11, 0x12, 11, 12, 11, 10, 10, 10, 0x11
     };
    public bool bProc_ChatEntry;
    public static int chat_entry_channel_img_width = 0x2f;
    public static int chat_entry_lineHeight = 0x27;
    public static int chat_list_max_width = 0x163;
    public int chatCell_DeltaHeight = 30;
    public int chatCell_InitHeight = 80;
    public int chatCell_patchHeight = 10;
    public static int chatFaceHeight = 0x22;
    public static int chatFaceWidth = 0x1a;
    private CChatEntity curEntNode;
    public static float element_height = 86f;
    public static float element_width = 547.4f;
    public static int lineHeight = 0x22;
    public int maxWidth;
    private ListView<LabelType> mList = new ListView<LabelType>();
    private int mOriginalPositionX = 5;
    private int mPositionX;
    private int mPositionY;
    private int mWidth;
    private const string SPEAKER_REP_STR = "{0}{1}";
    private const string SPEAKER_STR = "        ";
    public static int start_x = 0x35;
    public int viewFontSize = 0x12;

    private void CreateText(string value)
    {
        string str = string.Empty;
        string str2 = string.Empty;
        int index = value.IndexOf(@"\n");
        if (index != -1)
        {
            str = value.Substring(0, index);
            str2 = value.Substring(index + 2);
            if (!string.IsNullOrEmpty(str))
            {
                this.CreateText(str);
            }
            if (this.bProc_ChatEntry)
            {
                this.mPositionX = this.mOriginalPositionX;
                this.mPositionY -= lineHeight;
                if (!string.IsNullOrEmpty(str))
                {
                    this.CreateText(str2);
                }
            }
        }
        else
        {
            string finalText = string.Empty;
            float length = 0f;
            if (this.WrapText(value, (float) this.mPositionX, out finalText, out length))
            {
                int num3 = finalText.IndexOf(@"\n");
                str = finalText.Substring(0, num3);
                str2 = finalText.Substring(num3 + 2);
                if (!string.IsNullOrEmpty(str))
                {
                    this.CreateText(str);
                }
                if (!this.bProc_ChatEntry)
                {
                    this.mPositionX = this.mOriginalPositionX;
                    this.mPositionY -= lineHeight;
                    if (!string.IsNullOrEmpty(str))
                    {
                        this.CreateText(str2);
                    }
                }
            }
            else
            {
                length += 2.5f;
                if (this.curEntNode != null)
                {
                    this.curEntNode.TextObjList.Add(new CTextImageNode(value, true, length, 0f, (float) this.mPositionX, (float) this.mPositionY));
                    if (this.mPositionY < this.curEntNode.final_height)
                    {
                        this.curEntNode.final_height = this.mPositionY;
                    }
                    if (this.curEntNode.final_height < (-lineHeight * 2))
                    {
                        this.curEntNode.numLine = 3;
                    }
                    else if (this.curEntNode.final_height < -lineHeight)
                    {
                        this.curEntNode.numLine = 2;
                    }
                    else
                    {
                        this.curEntNode.numLine = 1;
                    }
                    this.mPositionX += (int) length;
                    if (this.mPositionX > this.curEntNode.final_width)
                    {
                        this.curEntNode.final_width = this.mPositionX;
                    }
                }
            }
        }
    }

    private void CreateTextFace(string value)
    {
        if (this.curEntNode != null)
        {
            bool flag = false;
            if ((chatFaceWidth + this.mPositionX) > this.maxWidth)
            {
                if (this.bProc_ChatEntry)
                {
                    return;
                }
                this.mPositionX = this.mOriginalPositionX;
                this.mPositionY -= chatFaceHeight;
                this.curEntNode.numLine = 2;
                flag = true;
            }
            this.curEntNode.TextObjList.Add(new CTextImageNode(value.Substring(1), false, (float) chatFaceWidth, 0f, (float) this.mPositionX, (float) -chatFaceHeight));
            this.mPositionX += chatFaceWidth;
            this.mWidth = Mathf.Max(this.mPositionX, this.mWidth);
            if (!flag)
            {
                this.curEntNode.final_width += chatFaceWidth;
            }
        }
    }

    private float GetCharacterWidth(char ch)
    {
        int num = ch;
        if ((num >= 0x20) && (num <= 0x7e))
        {
            return (float) ascii_width[num - 0x20];
        }
        return (float) this.viewFontSize;
    }

    public void Parse(string value, int startX, CChatEntity entNode)
    {
        if (!string.IsNullOrEmpty(value))
        {
            this.curEntNode = entNode;
            this.mList.Clear();
            this.mWidth = 0;
            this.mPositionY = -chatFaceHeight;
            this.mPositionX = this.mOriginalPositionX = 8;
            this.ParseText(value);
            this.ShowText();
        }
    }

    private void ParseText(string value)
    {
        if ((this.curEntNode != null) && ((this.curEntNode.type == EChaterType.Speaker) || (this.curEntNode.type == EChaterType.LoudSpeaker)))
        {
            value = string.Format("{0}{1}", "        ", value);
        }
        int length = 0;
        int startIndex = 0;
        string str2 = value;
        string pattern = @"(%\d+)";
        MatchCollection matchs = null;
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                matchs = Regex.Matches(value, pattern);
            }
            catch (Exception)
            {
            }
        }
        if ((matchs != null) && (matchs.Count > 0))
        {
            int count = matchs.Count;
            for (int i = 0; i < count; i++)
            {
                Match match = matchs[i];
                string str4 = match.Value;
                if (!string.IsNullOrEmpty(str4))
                {
                    length = str2.IndexOf(str4);
                    startIndex = length + str4.Length;
                    if (length > -1)
                    {
                        string str = str2.Substring(0, length);
                        if (!string.IsNullOrEmpty(str))
                        {
                            this.mList.Add(new LabelType(str, InfoType.Text));
                        }
                        if (!string.IsNullOrEmpty(str4))
                        {
                            this.mList.Add(new LabelType(str4, InfoType.Face));
                        }
                        str2 = str2.Substring(startIndex);
                    }
                }
            }
            if (!string.IsNullOrEmpty(str2))
            {
                this.mList.Add(new LabelType(str2, InfoType.Text));
            }
        }
        else
        {
            this.mList.Add(new LabelType(str2, InfoType.Text));
        }
    }

    private void ShowText()
    {
        int count = this.mList.Count;
        for (int i = 0; i < count; i++)
        {
            LabelType type = this.mList[i];
            switch (type.type)
            {
                case InfoType.Text:
                    this.CreateText(type.info);
                    break;

                case InfoType.Face:
                    this.CreateTextFace(type.info);
                    break;
            }
        }
        if (this.curEntNode != null)
        {
            this.curEntNode.final_width += 8f;
        }
    }

    private bool WrapText(string text, float curPosX, out string finalText, out float length)
    {
        float num = curPosX;
        float num2 = 0f;
        float characterWidth = 0f;
        int startIndex = -1;
        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];
            characterWidth = this.GetCharacterWidth(ch);
            num += characterWidth;
            num2 += characterWidth;
            if (num > this.maxWidth)
            {
                startIndex = i;
                break;
            }
        }
        length = num2;
        if (startIndex != -1)
        {
            text = text.Insert(startIndex, @"\n");
        }
        finalText = text;
        return (startIndex != -1);
    }

    public enum InfoType
    {
        Text,
        Face
    }

    public class LabelType
    {
        public string info;
        public CChatParser.InfoType type;

        public LabelType(string text, CChatParser.InfoType tp)
        {
            this.info = text;
            this.type = tp;
        }
    }
}

