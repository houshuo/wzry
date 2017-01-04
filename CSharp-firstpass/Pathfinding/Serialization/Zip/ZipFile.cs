namespace Pathfinding.Serialization.Zip
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class ZipFile
    {
        public Encoding AlternateEncoding;
        public ZipOption AlternateEncodingUsage;
        private DictionaryView<string, ZipEntry> dict = new DictionaryView<string, ZipEntry>();

        public void AddEntry(string name, byte[] bytes)
        {
            this.dict[name] = new ZipEntry(name, bytes);
        }

        public bool ContainsEntry(string name)
        {
            return this.dict.ContainsKey(name);
        }

        public void Dispose()
        {
        }

        public static ZipFile Read(Stream stream)
        {
            ZipFile file = new ZipFile();
            BinaryReader reader = new BinaryReader(stream);
            int num = reader.ReadInt32();
            for (int i = 0; i < num; i++)
            {
                string name = reader.ReadString();
                int count = reader.ReadInt32();
                byte[] bytes = reader.ReadBytes(count);
                file.dict[name] = new ZipEntry(name, bytes);
            }
            return file;
        }

        public void Save(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(this.dict.Count);
            foreach (KeyValuePair<string, ZipEntry> pair in this.dict)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.bytes.Length);
                writer.Write(pair.Value.bytes);
            }
        }

        public ZipEntry this[string index]
        {
            get
            {
                ZipEntry entry;
                this.dict.TryGetValue(index, out entry);
                return entry;
            }
        }
    }
}

