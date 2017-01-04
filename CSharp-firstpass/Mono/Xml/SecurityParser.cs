namespace Mono.Xml
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security;

    public class SecurityParser : Mono.Xml.SmallXmlParser, Mono.Xml.SmallXmlParser.IContentHandler
    {
        private SecurityElement current;
        public SecurityElement root;
        private Stack stack;

        public SecurityParser()
        {
            this.stack = new Stack();
        }

        public SecurityParser(int NoInit)
        {
        }

        public void LoadXml(string xml)
        {
            this.root = null;
            this.stack.Clear();
            base.Parse(new StringReader(xml), this);
        }

        public void OnChars(string ch)
        {
            this.current.Text = ch;
        }

        public void OnEndElement(string name)
        {
            this.current = (SecurityElement) this.stack.Pop();
        }

        public void OnEndParsing(Mono.Xml.SmallXmlParser parser)
        {
        }

        public void OnIgnorableWhitespace(string s)
        {
        }

        public void OnProcessingInstruction(string name, string text)
        {
        }

        public void OnStartElement(string name, Mono.Xml.SmallXmlParser.IAttrList attrs)
        {
            SecurityElement child = new SecurityElement(name);
            if (this.root == null)
            {
                this.root = child;
                this.current = child;
            }
            else
            {
                ((SecurityElement) this.stack.Peek()).AddChild(child);
            }
            this.stack.Push(child);
            this.current = child;
            int length = attrs.Length;
            for (int i = 0; i < length; i++)
            {
                string str = SecurityElement.Escape(attrs.GetName(i));
                string str2 = SecurityElement.Escape(attrs.GetValue(i));
                this.current.AddAttribute(str, str2);
            }
        }

        public void OnStartParsing(Mono.Xml.SmallXmlParser parser)
        {
        }

        public SecurityElement SelectSingleNode(string InPath)
        {
            if (this.root.Tag == InPath)
            {
                return this.root;
            }
            DebugHelper.Assert(false, "invalid call function SelectSingleNode in SecurityParser.");
            return null;
        }

        public SecurityElement ToXml()
        {
            return this.root;
        }
    }
}

