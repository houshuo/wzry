namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class Json
    {
        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }
            return Parser.Parse(json);
        }

        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        private sealed class Parser : IDisposable
        {
            private StringReader json;
            private const string WORD_BREAK = "{}[],:\"";

            private Parser(string jsonString)
            {
                this.json = new StringReader(jsonString);
            }

            public void Dispose()
            {
                this.json.Dispose();
                this.json = null;
            }

            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(this.PeekChar))
                {
                    this.json.Read();
                    if (this.json.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            public static bool IsWordBreak(char c)
            {
                return (char.IsWhiteSpace(c) || ("{}[],:\"".IndexOf(c) != -1));
            }

            public static object Parse(string jsonString)
            {
                using (Json.Parser parser = new Json.Parser(jsonString))
                {
                    return parser.ParseValue();
                }
            }

            private List<object> ParseArray()
            {
                List<object> list = new List<object>();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    TOKEN nextToken = this.NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.SQUARED_CLOSE:
                        {
                            flag = false;
                            continue;
                        }
                        case TOKEN.COMMA:
                        {
                            continue;
                        }
                        case TOKEN.NONE:
                            return null;
                    }
                    object item = this.ParseByToken(nextToken);
                    list.Add(item);
                }
                return list;
            }

            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.CURLY_OPEN:
                        return this.ParseObject();

                    case TOKEN.SQUARED_OPEN:
                        return this.ParseArray();

                    case TOKEN.STRING:
                        return this.ParseString();

                    case TOKEN.NUMBER:
                        return this.ParseNumber();

                    case TOKEN.TRUE:
                        return true;

                    case TOKEN.FALSE:
                        return false;

                    case TOKEN.NULL:
                        return null;
                }
                return null;
            }

            private object ParseNumber()
            {
                float num2;
                string nextWord = this.NextWord;
                if (nextWord.IndexOf('.') == -1)
                {
                    int num;
                    int.TryParse(nextWord, out num);
                    return num;
                }
                float.TryParse(nextWord, out num2);
                return num2;
            }

            private Dictionary<string, object> ParseObject()
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                this.json.Read();
            Label_0012:
                switch (this.NextToken)
                {
                    case TOKEN.NONE:
                        return null;

                    case TOKEN.CURLY_CLOSE:
                        return dictionary;

                    case TOKEN.COMMA:
                        goto Label_0012;
                }
                string str = this.ParseString();
                if (str == null)
                {
                    return null;
                }
                if (this.NextToken != TOKEN.COLON)
                {
                    return null;
                }
                this.json.Read();
                dictionary[str] = this.ParseValue();
                goto Label_0012;
            }

            private string ParseString()
            {
                StringBuilder builder = new StringBuilder();
                this.json.Read();
                bool flag = true;
                while (flag)
                {
                    char[] chArray;
                    int num;
                    if (this.json.Peek() == -1)
                    {
                        break;
                    }
                    char nextChar = this.NextChar;
                    char ch2 = nextChar;
                    if (ch2 == '"')
                    {
                        goto Label_0173;
                    }
                    if (ch2 != '\\')
                    {
                        builder.Append(nextChar);
                    }
                    else if (this.json.Peek() == -1)
                    {
                        flag = false;
                    }
                    else
                    {
                        nextChar = this.NextChar;
                        switch (nextChar)
                        {
                            case '"':
                            case '/':
                            case '\\':
                                builder.Append(nextChar);
                                break;

                            case 'b':
                                builder.Append('\b');
                                break;

                            case 'f':
                                builder.Append('\f');
                                break;

                            case 'r':
                                builder.Append('\r');
                                break;

                            case 't':
                                builder.Append('\t');
                                break;

                            case 'u':
                                chArray = new char[4];
                                num = 0;
                                goto Label_013D;

                            case 'n':
                                builder.Append('\n');
                                break;
                        }
                    }
                    continue;
                Label_012C:
                    chArray[num] = this.NextChar;
                    num++;
                Label_013D:
                    if (num < 4)
                    {
                        goto Label_012C;
                    }
                    builder.Append((char) Convert.ToInt32(new string(chArray), 0x10));
                    continue;
                Label_0173:
                    flag = false;
                }
                return builder.ToString();
            }

            private object ParseValue()
            {
                TOKEN nextToken = this.NextToken;
                return this.ParseByToken(nextToken);
            }

            private char NextChar
            {
                get
                {
                    return Convert.ToChar(this.json.Read());
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    this.EatWhitespace();
                    if (this.json.Peek() != -1)
                    {
                        switch (this.PeekChar)
                        {
                            case '"':
                                return TOKEN.STRING;

                            case ',':
                                this.json.Read();
                                return TOKEN.COMMA;

                            case '-':
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                return TOKEN.NUMBER;

                            case ':':
                                return TOKEN.COLON;

                            case '[':
                                return TOKEN.SQUARED_OPEN;

                            case ']':
                                this.json.Read();
                                return TOKEN.SQUARED_CLOSE;

                            case '{':
                                return TOKEN.CURLY_OPEN;

                            case '}':
                                this.json.Read();
                                return TOKEN.CURLY_CLOSE;
                        }
                        switch (this.NextWord)
                        {
                            case "false":
                                return TOKEN.FALSE;

                            case "true":
                                return TOKEN.TRUE;

                            case "null":
                                return TOKEN.NULL;
                        }
                    }
                    return TOKEN.NONE;
                }
            }

            private string NextWord
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    while (!IsWordBreak(this.PeekChar))
                    {
                        builder.Append(this.NextChar);
                        if (this.json.Peek() == -1)
                        {
                            break;
                        }
                    }
                    return builder.ToString();
                }
            }

            private char PeekChar
            {
                get
                {
                    return Convert.ToChar(this.json.Peek());
                }
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            }
        }

        private sealed class Serializer
        {
            private StringBuilder builder = new StringBuilder();

            private Serializer()
            {
            }

            public static string Serialize(object obj)
            {
                Json.Serializer serializer = new Json.Serializer();
                serializer.SerializeValue(obj);
                return serializer.builder.ToString();
            }

            private void SerializeArray(IList anArray)
            {
                this.builder.Append('[');
                bool flag = true;
                IEnumerator enumerator = anArray.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeValue(current);
                        flag = false;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                this.builder.Append(']');
            }

            private void SerializeObject(IDictionary obj)
            {
                bool flag = true;
                this.builder.Append('{');
                IEnumerator enumerator = obj.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (!flag)
                        {
                            this.builder.Append(',');
                        }
                        this.SerializeString(current.ToString());
                        this.builder.Append(':');
                        this.SerializeValue(obj[current]);
                        flag = false;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                this.builder.Append('}');
            }

            private void SerializeOther(object value)
            {
                if (value is float)
                {
                    this.builder.Append(((float) value).ToString("R"));
                }
                else if ((((value is int) || (value is uint)) || ((value is long) || (value is sbyte))) || (((value is byte) || (value is short)) || ((value is ushort) || (value is ulong))))
                {
                    this.builder.Append(value);
                }
                else if ((value is double) || (value is decimal))
                {
                    this.builder.Append(Convert.ToDouble(value).ToString("R"));
                }
                else
                {
                    this.SerializeString(value.ToString());
                }
            }

            private void SerializeString(string str)
            {
                this.builder.Append('"');
                char[] chArray2 = str.ToCharArray();
                int index = 0;
                while (index < chArray2.Length)
                {
                    int num2;
                    char ch = chArray2[index];
                    char ch2 = ch;
                    switch (ch2)
                    {
                        case '\b':
                            this.builder.Append(@"\b");
                            break;

                        case '\t':
                            this.builder.Append(@"\t");
                            break;

                        case '\n':
                            this.builder.Append(@"\n");
                            break;

                        case '\v':
                            goto Label_010E;

                        case '\f':
                            this.builder.Append(@"\f");
                            break;

                        case '\r':
                            this.builder.Append(@"\r");
                            break;

                        default:
                            if (ch2 != '"')
                            {
                                if (ch2 != '\\')
                                {
                                    goto Label_010E;
                                }
                                this.builder.Append(@"\\");
                            }
                            else
                            {
                                this.builder.Append("\\\"");
                            }
                            break;
                    }
                Label_0105:
                    index++;
                    continue;
                Label_010E:
                    num2 = Convert.ToInt32(ch);
                    if ((num2 >= 0x20) && (num2 <= 0x7e))
                    {
                        this.builder.Append(ch);
                    }
                    else
                    {
                        this.builder.Append(@"\u");
                        this.builder.Append(num2.ToString("x4"));
                    }
                    goto Label_0105;
                }
                this.builder.Append('"');
            }

            private void SerializeValue(object value)
            {
                if (value == null)
                {
                    this.builder.Append("null");
                }
                else
                {
                    string str = value as string;
                    if (str != null)
                    {
                        this.SerializeString(str);
                    }
                    else if (value is bool)
                    {
                        this.builder.Append(!((bool) value) ? "false" : "true");
                    }
                    else
                    {
                        IList anArray = value as IList;
                        if (anArray != null)
                        {
                            this.SerializeArray(anArray);
                        }
                        else
                        {
                            IDictionary dictionary = value as IDictionary;
                            if (dictionary != null)
                            {
                                this.SerializeObject(dictionary);
                            }
                            else if (value is char)
                            {
                                this.SerializeString(new string((char) value, 1));
                            }
                            else
                            {
                                this.SerializeOther(value);
                            }
                        }
                    }
                }
            }
        }
    }
}

