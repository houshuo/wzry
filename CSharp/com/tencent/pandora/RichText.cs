namespace com.tencent.pandora
{
    using System;
    using System.Text.RegularExpressions;

    internal class RichText
    {
        private static void addXhx(ref string strText)
        {
            string[] strArray = Regex.Split(strText, "url=", RegexOptions.IgnoreCase);
            string str = string.Empty;
            if (strArray.Length > 0)
            {
                str = strArray[0];
            }
            for (int i = 1; i < strArray.Length; i++)
            {
                int index = strArray[i].IndexOf("]");
                if (index >= 0)
                {
                    str = (str + "url=" + strArray[i].Substring(0, index + 1)) + "[u]" + strArray[i].Substring(index + 1);
                }
            }
            strText = str;
        }

        public static void convertToRichText(ref string strText)
        {
            if (strText.IndexOf("[p]") == 0)
            {
                strText = strText.Substring(3);
            }
            strText = strText.Replace("<p>", string.Empty);
            strText = strText.Replace("</p>", "\n");
            strText = strText.Replace("[p]", string.Empty);
            strText = strText.Replace("[/p]", "\n");
            strText = strText.Replace("<strong>", "[b]");
            strText = strText.Replace("</strong>", "[/b]");
            strText = strText.Replace("<u>", "[u]");
            strText = strText.Replace("</u>", "[/u]");
            strText = strText.Replace("[color=#", "[");
            strText = strText.Replace("[/color]", "[-]");
            strText = strText.Replace("[hilitecolor=#", "[");
            strText = strText.Replace("[/hilitecolor]", "[-]");
            addXhx(ref strText);
            strText = strText.Replace("[/url]", "[/u][/url]");
            strText = strText.Replace("[br]", "\n");
            pureText(ref strText);
        }

        public static void pureText(ref string strText)
        {
            strText = strText.Replace("&#39;", "'");
            strText = strText.Replace("&nbsp;", " ");
            strText = strText.Replace("&ldquo;", "“");
            strText = strText.Replace("&rdquo;", "”");
            strText = Regex.Replace(strText, "<[^>]*>", string.Empty);
            char[] trimChars = new char[] { '\n' };
            strText.Trim(trimChars);
        }
    }
}

