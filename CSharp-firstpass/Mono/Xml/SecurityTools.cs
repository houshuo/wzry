namespace Mono.Xml
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    public static class SecurityTools
    {
        public static bool ConvertSecurityElementToBinaryFile(SecurityElement InRoot, string InBinaryPath)
        {
            using (FileStream stream = File.OpenWrite(InBinaryPath))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    Write(writer, InRoot, 0);
                }
            }
            return true;
        }

        public static bool ConvertXmlFileToBinaryFile(string InXmlPath, string InBinaryPath)
        {
            string str = File.ReadAllText(InXmlPath, Encoding.UTF8);
            if (string.IsNullOrEmpty(str))
            {
                object[] inParameters = new object[] { InXmlPath };
                DebugHelper.Assert(false, "{0} is not an valid xml file.", inParameters);
                return false;
            }
            return ConvertXmlTextToBinaryFile(str, InBinaryPath);
        }

        public static bool ConvertXmlTextToBinaryFile(string InXmlText, string InBinaryPath)
        {
            Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
            parser.LoadXml(InXmlText);
            return ConvertSecurityElementToBinaryFile(parser.root, InBinaryPath);
        }

        private static SecurityElement LoadRootSecurityElement(BinaryReader InReader)
        {
            try
            {
                return LoadSecurityElementChecked(InReader, 0, null);
            }
            catch (Exception exception)
            {
                DebugHelper.Assert(false, exception.Message);
                return null;
            }
        }

        private static SecurityElement LoadSecurityElementChecked(BinaryReader InReader, byte InType, SecurityElement InParent)
        {
            if (InReader.ReadByte() != InType)
            {
                return null;
            }
            string tag = InReader.ReadString();
            string text = InReader.ReadString();
            SecurityElement inParent = new SecurityElement(tag, text);
            int num2 = InReader.ReadInt32();
            DebugHelper.Assert(num2 < 0x200, "too many attributes.");
            for (int i = 0; i < num2; i++)
            {
                string name = InReader.ReadString();
                string str4 = InReader.ReadString();
                inParent.AddAttribute(name, str4);
            }
            int num4 = InReader.ReadInt32();
            DebugHelper.Assert(num4 < 0x203, "too many children");
            for (int j = 0; j < num4; j++)
            {
                DebugHelper.Assert(LoadSecurityElementChecked(InReader, 1, inParent) != null, "invalid child element");
            }
            if (InParent != null)
            {
                InParent.AddChild(inParent);
            }
            return inParent;
        }

        public static bool LoadXmlFromBinaryBuffer(Mono.Xml.SecurityParser InParser, byte[] InFileBytes, string InPath = "")
        {
            if ((InFileBytes != null) && (InFileBytes.Length >= 4))
            {
                using (MemoryStream stream = new MemoryStream(InFileBytes))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        SecurityElement element = LoadRootSecurityElement(reader);
                        object[] inParameters = new object[] { InPath };
                        DebugHelper.Assert(element != null, "Failed load root Security Element in file: {0}", inParameters);
                        if (element != null)
                        {
                            InParser.root = element;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Mono.Xml.SecurityParser LoadXmlFromBinaryFile(string InPath)
        {
            Mono.Xml.SecurityParser inParser = new Mono.Xml.SecurityParser();
            if (!LoadXmlFromBinaryFile(inParser, InPath))
            {
                return null;
            }
            return inParser;
        }

        public static bool LoadXmlFromBinaryFile(Mono.Xml.SecurityParser InParser, string InPath)
        {
            byte[] inFileBytes = File.ReadAllBytes(InPath);
            return LoadXmlFromBinaryBuffer(InParser, inFileBytes, InPath);
        }

        private static void Write(BinaryWriter InWriterRef, SecurityElement InElement, byte InType)
        {
            InWriterRef.Write(InType);
            InWriterRef.WriteString(InElement.Tag);
            InWriterRef.WriteString(InElement.Text);
            Hashtable attributes = InElement.Attributes;
            int num = (attributes == null) ? 0 : attributes.Count;
            InWriterRef.Write(num);
            if (attributes != null)
            {
                IDictionaryEnumerator enumerator = attributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string key = enumerator.Key as string;
                    string inText = enumerator.Value as string;
                    DebugHelper.Assert((key != null) && (inText != null), "Invalid Attributes");
                    InWriterRef.WriteString(key);
                    InWriterRef.WriteString(inText);
                }
            }
            ArrayList children = InElement.Children;
            int num2 = (children == null) ? 0 : children.Count;
            InWriterRef.Write(num2);
            if (children != null)
            {
                IEnumerator enumerator2 = children.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SecurityElement current = enumerator2.Current as SecurityElement;
                    DebugHelper.Assert(current != null, "Invalid Security Element.");
                    Write(InWriterRef, current, 1);
                }
            }
        }

        private static void WriteString(this BinaryWriter InWriter, string InText)
        {
            if (InText != null)
            {
                InWriter.Write(InText);
            }
            else
            {
                InWriter.Write(string.Empty);
            }
        }

        private enum ESecurityElementType
        {
            Root,
            Children
        }
    }
}

