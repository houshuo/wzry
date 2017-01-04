namespace Mono.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class SmallXmlParser
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map3;
        private AttrListImpl attributes = new AttrListImpl();
        private StringBuilder buffer = new StringBuilder(200);
        private int column;
        private Stack elementNames = new Stack();
        private IContentHandler handler;
        private bool isWhitespace;
        private int line = 1;
        private char[] nameBuffer = new char[30];
        private TextReader reader;
        private bool resetColumn;
        private string xmlSpace;
        private Stack xmlSpaces = new Stack();

        private void Cleanup()
        {
            this.line = 1;
            this.column = 0;
            this.handler = null;
            this.reader = null;
            this.elementNames.Clear();
            this.xmlSpaces.Clear();
            this.attributes.Clear();
            this.buffer.Length = 0;
            this.xmlSpace = null;
            this.isWhitespace = false;
        }

        private Exception Error(string msg)
        {
            return new Mono.Xml.SmallXmlParserException(msg, this.line, this.column);
        }

        public void Expect(int c)
        {
            int num = this.Read();
            if (num < 0)
            {
                throw this.UnexpectedEndError();
            }
            if (num != c)
            {
                throw this.Error(string.Format("Expected '{0}' but got {1}", (char) c, (char) num));
            }
        }

        private void HandleBufferedContent()
        {
            if (this.buffer.Length != 0)
            {
                if (this.isWhitespace)
                {
                    this.handler.OnIgnorableWhitespace(this.buffer.ToString());
                }
                else
                {
                    this.handler.OnChars(this.buffer.ToString());
                }
                this.buffer.Length = 0;
                this.isWhitespace = false;
            }
        }

        private void HandleWhitespaces()
        {
            while (this.IsWhitespace(this.Peek()))
            {
                this.buffer.Append((char) this.Read());
            }
            if ((this.Peek() != 60) && (this.Peek() >= 0))
            {
                this.isWhitespace = false;
            }
        }

        private bool IsNameChar(char c, bool start)
        {
            switch (c)
            {
                case '-':
                case '.':
                    return !start;

                case ':':
                case '_':
                    return true;
            }
            if (c > 'Ā')
            {
                switch (c)
                {
                    case 'ۥ':
                    case 'ۦ':
                    case 'ՙ':
                        return true;
                }
                if (('ʻ' <= c) && (c <= 'ˁ'))
                {
                    return true;
                }
            }
            switch (char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.LetterNumber:
                    return true;

                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.EnclosingMark:
                case UnicodeCategory.DecimalDigitNumber:
                    return !start;
            }
            return false;
        }

        private bool IsWhitespace(int c)
        {
            switch (c)
            {
                case 9:
                case 10:
                case 13:
                case 0x20:
                    return true;
            }
            return false;
        }

        public void Parse(TextReader input, IContentHandler handler)
        {
            this.reader = input;
            this.handler = handler;
            handler.OnStartParsing(this);
            while (this.Peek() >= 0)
            {
                this.ReadContent();
            }
            this.HandleBufferedContent();
            if (this.elementNames.Count > 0)
            {
                throw this.Error(string.Format("Insufficient close tag: {0}", this.elementNames.Peek()));
            }
            handler.OnEndParsing(this);
            this.Cleanup();
        }

        private int Peek()
        {
            return this.reader.Peek();
        }

        private int Read()
        {
            int num = this.reader.Read();
            if (num == 10)
            {
                this.resetColumn = true;
            }
            if (this.resetColumn)
            {
                this.line++;
                this.resetColumn = false;
                this.column = 1;
                return num;
            }
            this.column++;
            return num;
        }

        private void ReadAttribute(AttrListImpl a)
        {
            this.SkipWhitespaces(true);
            if ((this.Peek() != 0x2f) && (this.Peek() != 0x3e))
            {
                string str2;
                string name = this.ReadName();
                this.SkipWhitespaces();
                this.Expect(0x3d);
                this.SkipWhitespaces();
                int num = this.Read();
                if (num != 0x22)
                {
                    if (num != 0x27)
                    {
                        throw this.Error("Invalid attribute value markup.");
                    }
                    str2 = this.ReadUntil('\'', true);
                }
                else
                {
                    str2 = this.ReadUntil('"', true);
                }
                if (name == "xml:space")
                {
                    this.xmlSpace = str2;
                }
                a.Add(name, str2);
            }
        }

        private void ReadCDATASection()
        {
            int num = 0;
        Label_0002:
            if (this.Peek() < 0)
            {
                throw this.UnexpectedEndError();
            }
            char ch = (char) this.Read();
            if (ch == ']')
            {
                num++;
                goto Label_0002;
            }
            if ((ch == '>') && (num > 1))
            {
                for (int i = num; i > 2; i--)
                {
                    this.buffer.Append(']');
                }
            }
            else
            {
                for (int j = 0; j < num; j++)
                {
                    this.buffer.Append(']');
                }
                num = 0;
                this.buffer.Append(ch);
                goto Label_0002;
            }
        }

        private int ReadCharacterReference()
        {
            int num = 0;
            if (this.Peek() == 120)
            {
                this.Read();
                for (int j = this.Peek(); j >= 0; j = this.Peek())
                {
                    if ((0x30 <= j) && (j <= 0x39))
                    {
                        num = num << ((4 + j) - 0x30);
                    }
                    else if ((0x41 <= j) && (j <= 70))
                    {
                        num = num << (((4 + j) - 0x41) + 10);
                    }
                    else
                    {
                        if ((0x61 > j) || (j > 0x66))
                        {
                            return num;
                        }
                        num = num << (((4 + j) - 0x61) + 10);
                    }
                    this.Read();
                }
                return num;
            }
            for (int i = this.Peek(); i >= 0; i = this.Peek())
            {
                if ((0x30 > i) || (i > 0x39))
                {
                    return num;
                }
                num = num << ((4 + i) - 0x30);
                this.Read();
            }
            return num;
        }

        private void ReadCharacters()
        {
            this.isWhitespace = false;
        Label_0007:
            switch (this.Peek())
            {
                case -1:
                    return;

                case 0x26:
                    this.Read();
                    this.ReadReference();
                    goto Label_0007;

                case 60:
                    return;
            }
            this.buffer.Append((char) this.Read());
            goto Label_0007;
        }

        private void ReadComment()
        {
            this.Expect(0x2d);
            this.Expect(0x2d);
            while ((this.Read() != 0x2d) || (this.Read() != 0x2d))
            {
            }
            if (this.Read() != 0x3e)
            {
                throw this.Error("'--' is not allowed inside comment markup.");
            }
        }

        public void ReadContent()
        {
            if (this.IsWhitespace(this.Peek()))
            {
                if (this.buffer.Length == 0)
                {
                    this.isWhitespace = true;
                }
                this.HandleWhitespaces();
            }
            if (this.Peek() != 60)
            {
                this.ReadCharacters();
            }
            else
            {
                string str;
                string str2;
                this.Read();
                switch (this.Peek())
                {
                    case 0x21:
                        this.Read();
                        if (this.Peek() != 0x5b)
                        {
                            if (this.Peek() == 0x2d)
                            {
                                this.ReadComment();
                                return;
                            }
                            if (this.ReadName() != "DOCTYPE")
                            {
                                throw this.Error("Invalid declaration markup.");
                            }
                            throw this.Error("This parser does not support document type.");
                        }
                        this.Read();
                        if (this.ReadName() != "CDATA")
                        {
                            throw this.Error("Invalid declaration markup");
                        }
                        this.Expect(0x5b);
                        this.ReadCDATASection();
                        return;

                    case 0x2f:
                    {
                        this.HandleBufferedContent();
                        if (this.elementNames.Count == 0)
                        {
                            throw this.UnexpectedEndError();
                        }
                        this.Read();
                        str = this.ReadName();
                        this.SkipWhitespaces();
                        string str3 = (string) this.elementNames.Pop();
                        this.xmlSpaces.Pop();
                        if (this.xmlSpaces.Count > 0)
                        {
                            this.xmlSpace = (string) this.xmlSpaces.Peek();
                        }
                        else
                        {
                            this.xmlSpace = null;
                        }
                        if (str != str3)
                        {
                            throw this.Error(string.Format("End tag mismatch: expected {0} but found {1}", str3, str));
                        }
                        this.handler.OnEndElement(str);
                        this.Expect(0x3e);
                        return;
                    }
                    case 0x3f:
                        this.HandleBufferedContent();
                        this.Read();
                        str = this.ReadName();
                        this.SkipWhitespaces();
                        str2 = string.Empty;
                        if (this.Peek() != 0x3f)
                        {
                            while (true)
                            {
                                str2 = str2 + this.ReadUntil('?', false);
                                if (this.Peek() == 0x3e)
                                {
                                    break;
                                }
                                str2 = str2 + "?";
                            }
                        }
                        break;

                    default:
                        this.HandleBufferedContent();
                        str = this.ReadName();
                        while ((this.Peek() != 0x3e) && (this.Peek() != 0x2f))
                        {
                            this.ReadAttribute(this.attributes);
                        }
                        this.handler.OnStartElement(str, this.attributes);
                        this.attributes.Clear();
                        this.SkipWhitespaces();
                        if (this.Peek() == 0x2f)
                        {
                            this.Read();
                            this.handler.OnEndElement(str);
                        }
                        else
                        {
                            this.elementNames.Push(str);
                            this.xmlSpaces.Push(this.xmlSpace);
                        }
                        this.Expect(0x3e);
                        return;
                }
                this.handler.OnProcessingInstruction(str, str2);
                this.Expect(0x3e);
            }
        }

        public string ReadName()
        {
            int length = 0;
            if ((this.Peek() < 0) || !this.IsNameChar((char) this.Peek(), true))
            {
                throw this.Error("XML name start character is expected.");
            }
            for (int i = this.Peek(); i >= 0; i = this.Peek())
            {
                char c = (char) i;
                if (!this.IsNameChar(c, false))
                {
                    break;
                }
                if (length == this.nameBuffer.Length)
                {
                    char[] destinationArray = new char[length * 2];
                    Array.Copy(this.nameBuffer, 0, destinationArray, 0, length);
                    this.nameBuffer = destinationArray;
                }
                this.nameBuffer[length++] = c;
                this.Read();
            }
            if (length == 0)
            {
                throw this.Error("Valid XML name is expected.");
            }
            return new string(this.nameBuffer, 0, length);
        }

        private void ReadReference()
        {
            if (this.Peek() == 0x23)
            {
                this.Read();
                this.ReadCharacterReference();
            }
            else
            {
                string str = this.ReadName();
                this.Expect(0x3b);
                string key = str;
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map3 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                        dictionary.Add("amp", 0);
                        dictionary.Add("quot", 1);
                        dictionary.Add("apos", 2);
                        dictionary.Add("lt", 3);
                        dictionary.Add("gt", 4);
                        <>f__switch$map3 = dictionary;
                    }
                    if (<>f__switch$map3.TryGetValue(key, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                this.buffer.Append('&');
                                return;

                            case 1:
                                this.buffer.Append('"');
                                return;

                            case 2:
                                this.buffer.Append('\'');
                                return;

                            case 3:
                                this.buffer.Append('<');
                                return;

                            case 4:
                                this.buffer.Append('>');
                                return;
                        }
                    }
                }
                throw this.Error("General non-predefined entity reference is not supported in this parser.");
            }
        }

        private string ReadUntil(char until, bool handleReferences)
        {
        Label_0000:
            if (this.Peek() < 0)
            {
                throw this.UnexpectedEndError();
            }
            char ch = (char) this.Read();
            if (ch != until)
            {
                if (handleReferences && (ch == '&'))
                {
                    this.ReadReference();
                }
                else
                {
                    this.buffer.Append(ch);
                }
                goto Label_0000;
            }
            string str = this.buffer.ToString();
            this.buffer.Length = 0;
            return str;
        }

        public void SkipWhitespaces()
        {
            this.SkipWhitespaces(false);
        }

        public void SkipWhitespaces(bool expected)
        {
        Label_0000:
            switch (this.Peek())
            {
                case 9:
                case 10:
                case 13:
                case 0x20:
                    this.Read();
                    if (expected)
                    {
                        expected = false;
                    }
                    goto Label_0000;
            }
            if (expected)
            {
                throw this.Error("Whitespace is expected.");
            }
        }

        private Exception UnexpectedEndError()
        {
            string[] array = new string[this.elementNames.Count];
            this.elementNames.CopyTo(array, 0);
            return this.Error(string.Format("Unexpected end of stream. Element stack content is {0}", string.Join(",", array)));
        }

        private class AttrListImpl : Mono.Xml.SmallXmlParser.IAttrList
        {
            private ArrayList attrNames = new ArrayList();
            private ArrayList attrValues = new ArrayList();

            internal void Add(string name, string value)
            {
                this.attrNames.Add(name);
                this.attrValues.Add(value);
            }

            internal void Clear()
            {
                this.attrNames.Clear();
                this.attrValues.Clear();
            }

            public string GetName(int i)
            {
                return (string) this.attrNames[i];
            }

            public string GetValue(int i)
            {
                return (string) this.attrValues[i];
            }

            public string GetValue(string name)
            {
                for (int i = 0; i < this.attrNames.Count; i++)
                {
                    if (((string) this.attrNames[i]) == name)
                    {
                        return (string) this.attrValues[i];
                    }
                }
                return null;
            }

            public bool IsEmpty
            {
                get
                {
                    return (this.attrNames.Count == 0);
                }
            }

            public int Length
            {
                get
                {
                    return this.attrNames.Count;
                }
            }

            public string[] Names
            {
                get
                {
                    return (string[]) this.attrNames.ToArray(typeof(string));
                }
            }

            public string[] Values
            {
                get
                {
                    return (string[]) this.attrValues.ToArray(typeof(string));
                }
            }
        }

        public interface IAttrList
        {
            string GetName(int i);
            string GetValue(int i);
            string GetValue(string name);

            bool IsEmpty { get; }

            int Length { get; }

            string[] Names { get; }

            string[] Values { get; }
        }

        public interface IContentHandler
        {
            void OnChars(string text);
            void OnEndElement(string name);
            void OnEndParsing(Mono.Xml.SmallXmlParser parser);
            void OnIgnorableWhitespace(string text);
            void OnProcessingInstruction(string name, string text);
            void OnStartElement(string name, Mono.Xml.SmallXmlParser.IAttrList attrs);
            void OnStartParsing(Mono.Xml.SmallXmlParser parser);
        }
    }
}

