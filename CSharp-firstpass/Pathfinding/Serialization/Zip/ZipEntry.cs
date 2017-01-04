namespace Pathfinding.Serialization.Zip
{
    using System;
    using System.IO;

    public class ZipEntry
    {
        internal byte[] bytes;
        internal string name;

        public ZipEntry(string name, byte[] bytes)
        {
            this.name = name;
            this.bytes = bytes;
        }

        public void Extract(Stream stream)
        {
            stream.Write(this.bytes, 0, this.bytes.Length);
        }
    }
}

