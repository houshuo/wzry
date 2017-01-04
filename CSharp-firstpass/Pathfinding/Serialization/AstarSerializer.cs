namespace Pathfinding.Serialization
{
    using Pathfinding;
    using Pathfinding.Serialization.Zip;
    using Pathfinding.Util;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    public class AstarSerializer
    {
        private static StringBuilder _stringBuilder = new StringBuilder();
        private const string binaryExt = ".binary";
        private uint checksum;
        private AstarData data;
        public NavGraph[] graphs;
        private const string jsonExt = ".binary";
        private GraphMeta meta;
        private SerializeSettings settings;
        private MemoryStream str;
        private ZipFile zip;

        public AstarSerializer(AstarData data)
        {
            this.checksum = uint.MaxValue;
            this.data = data;
            this.settings = SerializeSettings.Settings;
        }

        public AstarSerializer(AstarData data, SerializeSettings settings)
        {
            this.checksum = uint.MaxValue;
            this.data = data;
            this.settings = settings;
        }

        public void AddChecksum(byte[] bytes)
        {
            this.checksum = Checksum.GetChecksum(bytes, this.checksum);
        }

        public void CloseDeserialize()
        {
            this.str.Dispose();
            this.zip.Dispose();
            this.zip = null;
            this.str = null;
        }

        public byte[] CloseSerialize()
        {
            byte[] bytes = this.SerializeMeta();
            this.AddChecksum(bytes);
            this.zip.AddEntry("meta.binary", bytes);
            MemoryStream stream = new MemoryStream();
            this.zip.Save(stream);
            bytes = stream.ToArray();
            stream.Dispose();
            this.zip.Dispose();
            this.zip = null;
            return bytes;
        }

        public void DeserializeEditorSettings(GraphEditorBase[] graphEditors)
        {
        }

        public void DeserializeExtraInfo()
        {
            <DeserializeExtraInfo>c__AnonStorey29 storey = new <DeserializeExtraInfo>c__AnonStorey29();
            bool flag = false;
            for (int i = 0; i < this.graphs.Length; i++)
            {
                ZipEntry entry = this.zip["graph" + i + "_extra.binary"];
                if (entry != null)
                {
                    flag = true;
                    MemoryStream stream = new MemoryStream();
                    entry.Extract(stream);
                    stream.Seek(0L, SeekOrigin.Begin);
                    BinaryReader reader = new BinaryReader(stream);
                    GraphSerializationContext ctx = new GraphSerializationContext(reader, null, i);
                    this.graphs[i].DeserializeExtraInfo(ctx);
                }
            }
            if (flag)
            {
                storey.totCount = 0;
                for (int j = 0; j < this.graphs.Length; j++)
                {
                    if (this.graphs[j] != null)
                    {
                        this.graphs[j].GetNodes(new GraphNodeDelegateCancelable(storey.<>m__1B));
                    }
                }
                <DeserializeExtraInfo>c__AnonStorey2A storeya = new <DeserializeExtraInfo>c__AnonStorey2A();
                ZipEntry entry2 = this.zip["graph_references.binary"];
                if (entry2 == null)
                {
                    throw new Exception("Node references not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
                }
                MemoryStream stream2 = new MemoryStream();
                entry2.Extract(stream2);
                stream2.Seek(0L, SeekOrigin.Begin);
                storeya.reader = new BinaryReader(stream2);
                int num3 = storeya.reader.ReadInt32();
                storeya.int2Node = new GraphNode[num3 + 1];
                try
                {
                    for (int m = 0; m < this.graphs.Length; m++)
                    {
                        if (this.graphs[m] != null)
                        {
                            this.graphs[m].GetNodes(new GraphNodeDelegateCancelable(storeya.<>m__1C));
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("Some graph(s) has thrown an exception during GetNodes, or some graph(s) have deserialized more or fewer nodes than were serialized", exception);
                }
                storeya.reader.Close();
                for (int k = 0; k < this.graphs.Length; k++)
                {
                    <DeserializeExtraInfo>c__AnonStorey2B storeyb = new <DeserializeExtraInfo>c__AnonStorey2B();
                    if (this.graphs[k] != null)
                    {
                        entry2 = this.zip["graph" + k + "_references.binary"];
                        if (entry2 == null)
                        {
                            throw new Exception("Node references for graph " + k + " not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
                        }
                        stream2 = new MemoryStream();
                        entry2.Extract(stream2);
                        stream2.Seek(0L, SeekOrigin.Begin);
                        storeya.reader = new BinaryReader(stream2);
                        storeyb.ctx = new GraphSerializationContext(storeya.reader, storeya.int2Node, k);
                        this.graphs[k].GetNodes(new GraphNodeDelegateCancelable(storeyb.<>m__1D));
                    }
                }
            }
        }

        public NavGraph[] DeserializeGraphs()
        {
            this.graphs = new NavGraph[this.meta.graphs];
            int index = 0;
            for (int i = 0; i < this.meta.graphs; i++)
            {
                System.Type graphType = this.meta.GetGraphType(i);
                if (!object.Equals(graphType, null))
                {
                    index++;
                    ZipEntry entry = this.zip["graph" + i + ".binary"];
                    if (entry == null)
                    {
                        object[] objArray1 = new object[] { "Could not find data for graph ", i, " in zip. Entry 'graph+", i, ".binary' does not exist" };
                        throw new FileNotFoundException(string.Concat(objArray1));
                    }
                    NavGraph graph = this.data.CreateGraph(graphType);
                    MemoryStream stream = new MemoryStream();
                    entry.Extract(stream);
                    stream.Position = 0L;
                    BinaryReader reader = new BinaryReader(stream);
                    GraphSerializationContext ctx = new GraphSerializationContext(reader, null, i);
                    graph.DeserializeSettings(ctx);
                    this.graphs[i] = graph;
                    if (this.graphs[i].guid.ToString() != this.meta.guids[i])
                    {
                        throw new Exception("Guid in graph file not equal to guid defined in meta file. Have you edited the data manually?\n" + this.graphs[i].guid.ToString() + " != " + this.meta.guids[i]);
                    }
                }
            }
            NavGraph[] graphArray = new NavGraph[index];
            index = 0;
            for (int j = 0; j < this.graphs.Length; j++)
            {
                if (this.graphs[j] != null)
                {
                    graphArray[index] = this.graphs[j];
                    index++;
                }
            }
            this.graphs = graphArray;
            return this.graphs;
        }

        private GraphMeta DeserializeMeta(ZipEntry entry)
        {
            if (entry == null)
            {
                throw new Exception("No metadata found in serialized data.");
            }
            GraphMeta meta = new GraphMeta();
            MemoryStream stream = new MemoryStream();
            entry.Extract(stream);
            stream.Position = 0L;
            BinaryReader reader = new BinaryReader(stream);
            if (reader.ReadString() != "A*")
            {
                throw new Exception("Invalid magic number in saved data");
            }
            int major = reader.ReadInt32();
            int minor = reader.ReadInt32();
            int build = reader.ReadInt32();
            int revision = reader.ReadInt32();
            if (major < 0)
            {
                meta.version = new Version(0, 0);
            }
            else if (minor < 0)
            {
                meta.version = new Version(major, 0);
            }
            else if (build < 0)
            {
                meta.version = new Version(major, minor);
            }
            else if (revision < 0)
            {
                meta.version = new Version(major, minor, build);
            }
            else
            {
                meta.version = new Version(major, minor, build, revision);
            }
            meta.graphs = reader.ReadInt32();
            meta.guids = new string[reader.ReadInt32()];
            for (int i = 0; i < meta.guids.Length; i++)
            {
                meta.guids[i] = reader.ReadString();
            }
            meta.typeNames = new string[reader.ReadInt32()];
            for (int j = 0; j < meta.typeNames.Length; j++)
            {
                meta.typeNames[j] = reader.ReadString();
            }
            meta.nodeCounts = new int[reader.ReadInt32()];
            for (int k = 0; k < meta.nodeCounts.Length; k++)
            {
                meta.nodeCounts[k] = reader.ReadInt32();
            }
            return meta;
        }

        private void DeserializeNodeConnections(int index, BinaryReader reader)
        {
        }

        public void DeserializeNodes()
        {
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if ((this.graphs[i] != null) && this.zip.ContainsEntry("graph" + i + "_nodes.binary"))
                {
                }
            }
            for (int j = 0; j < this.graphs.Length; j++)
            {
                if (this.graphs[j] != null)
                {
                    ZipEntry entry = this.zip["graph" + j + "_nodes.binary"];
                    if (entry != null)
                    {
                        MemoryStream stream = new MemoryStream();
                        entry.Extract(stream);
                        stream.Position = 0L;
                        BinaryReader reader = new BinaryReader(stream);
                        this.DeserializeNodes(j, reader);
                    }
                }
            }
            for (int k = 0; k < this.graphs.Length; k++)
            {
                if (this.graphs[k] != null)
                {
                    ZipEntry entry2 = this.zip["graph" + k + "_conns.binary"];
                    if (entry2 != null)
                    {
                        MemoryStream stream2 = new MemoryStream();
                        entry2.Extract(stream2);
                        stream2.Position = 0L;
                        BinaryReader reader2 = new BinaryReader(stream2);
                        this.DeserializeNodeConnections(k, reader2);
                    }
                }
            }
        }

        private void DeserializeNodes(int index, BinaryReader reader)
        {
        }

        public UserConnection[] DeserializeUserConnections()
        {
            return new UserConnection[0];
        }

        public uint GetChecksum()
        {
            return this.checksum;
        }

        private string GetString(ZipEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            entry.Extract(stream);
            stream.Position = 0L;
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            stream.Position = 0L;
            reader.Dispose();
            return str;
        }

        private static StringBuilder GetStringBuilder()
        {
            _stringBuilder.Length = 0;
            return _stringBuilder;
        }

        public static byte[] LoadFromFile(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                byte[] array = new byte[(int) stream.Length];
                stream.Read(array, 0, (int) stream.Length);
                return array;
            }
        }

        public bool OpenDeserialize(byte[] bytes)
        {
            this.str = new MemoryStream();
            this.str.Write(bytes, 0, bytes.Length);
            this.str.Position = 0L;
            try
            {
                this.zip = ZipFile.Read(this.str);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Caught exception when loading from zip\n" + exception);
                this.str.Dispose();
                return false;
            }
            this.meta = this.DeserializeMeta(this.zip["meta.binary"]);
            if (this.meta.version > AstarPath.Version)
            {
                Debug.LogWarning(string.Concat(new object[] { "Trying to load data from a newer version of the A* Pathfinding Project\nCurrent version: ", AstarPath.Version, " Data version: ", this.meta.version }));
            }
            else if (this.meta.version < AstarPath.Version)
            {
                Debug.LogWarning(string.Concat(new object[] { "Trying to load data from an older version of the A* Pathfinding Project\nCurrent version: ", AstarPath.Version, " Data version: ", this.meta.version, "\nThis is usually fine, it just means you have upgraded to a new version.\nHowever node data (not settings) can get corrupted between versions, so it is recommendedto recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n" }));
            }
            return true;
        }

        public void OpenSerialize()
        {
            this.zip = new ZipFile();
            this.zip.AlternateEncoding = Encoding.UTF8;
            this.zip.AlternateEncodingUsage = ZipOption.Always;
            this.meta = new GraphMeta();
        }

        public void PostDeserialization()
        {
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if (this.graphs[i] != null)
                {
                    this.graphs[i].PostDeserialization();
                }
            }
        }

        public static void SaveToFile(string path, byte[] data)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(data, 0, data.Length);
            }
        }

        public byte[] Serialize(NavGraph graph)
        {
            MemoryStream output = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(output);
            GraphSerializationContext ctx = new GraphSerializationContext(writer);
            graph.SerializeSettings(ctx);
            return output.ToArray();
        }

        public void SerializeEditorSettings(GraphEditorBase[] editors)
        {
            if ((editors != null) && this.settings.editorSettings)
            {
            }
        }

        public void SerializeExtraInfo()
        {
            <SerializeExtraInfo>c__AnonStorey26 storey = new <SerializeExtraInfo>c__AnonStorey26();
            if (this.settings.nodes)
            {
                storey.totCount = 0;
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if (this.graphs[i] != null)
                    {
                        this.graphs[i].GetNodes(new GraphNodeDelegateCancelable(storey.<>m__18));
                    }
                }
                <SerializeExtraInfo>c__AnonStorey27 storey2 = new <SerializeExtraInfo>c__AnonStorey27();
                MemoryStream output = new MemoryStream();
                storey2.wr = new BinaryWriter(output);
                storey2.wr.Write(storey.totCount);
                storey2.c = 0;
                for (int j = 0; j < this.graphs.Length; j++)
                {
                    if (this.graphs[j] != null)
                    {
                        this.graphs[j].GetNodes(new GraphNodeDelegateCancelable(storey2.<>m__19));
                    }
                }
                if (storey2.c != storey.totCount)
                {
                    throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
                }
                byte[] bytes = output.ToArray();
                storey2.wr.Close();
                this.AddChecksum(bytes);
                this.zip.AddEntry("graph_references.binary", bytes);
                for (int k = 0; k < this.graphs.Length; k++)
                {
                    <SerializeExtraInfo>c__AnonStorey28 storey3 = new <SerializeExtraInfo>c__AnonStorey28();
                    if (this.graphs[k] != null)
                    {
                        MemoryStream stream2 = new MemoryStream();
                        BinaryWriter writer = new BinaryWriter(stream2);
                        storey3.ctx = new GraphSerializationContext(writer);
                        this.graphs[k].SerializeExtraInfo(storey3.ctx);
                        byte[] buffer2 = stream2.ToArray();
                        writer.Close();
                        this.AddChecksum(buffer2);
                        this.zip.AddEntry("graph" + k + "_extra.binary", buffer2);
                        stream2 = new MemoryStream();
                        writer = new BinaryWriter(stream2);
                        storey3.ctx = new GraphSerializationContext(writer);
                        this.graphs[k].GetNodes(new GraphNodeDelegateCancelable(storey3.<>m__1A));
                        writer.Close();
                        buffer2 = stream2.ToArray();
                        this.AddChecksum(buffer2);
                        this.zip.AddEntry("graph" + k + "_references.binary", buffer2);
                    }
                }
            }
        }

        public byte[] SerializeExtraInfoBytes()
        {
            <SerializeExtraInfoBytes>c__AnonStorey23 storey = new <SerializeExtraInfoBytes>c__AnonStorey23();
            if (!this.settings.nodes)
            {
                return null;
            }
            storey.totCount = 0;
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if (this.graphs[i] != null)
                {
                    this.graphs[i].GetNodes(new GraphNodeDelegateCancelable(storey.<>m__15));
                }
            }
            MemoryStream output = new MemoryStream();
            storey.wr = new BinaryWriter(output);
            <SerializeExtraInfoBytes>c__AnonStorey24 storey2 = new <SerializeExtraInfoBytes>c__AnonStorey24 {
                <>f__ref$35 = storey
            };
            storey.wr.Write(storey.totCount);
            storey2.c = 0;
            for (int j = 0; j < this.graphs.Length; j++)
            {
                if (this.graphs[j] != null)
                {
                    this.graphs[j].GetNodes(new GraphNodeDelegateCancelable(storey2.<>m__16));
                }
            }
            if (storey2.c != storey.totCount)
            {
                throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
            }
            for (int k = 0; k < this.graphs.Length; k++)
            {
                <SerializeExtraInfoBytes>c__AnonStorey25 storey3 = new <SerializeExtraInfoBytes>c__AnonStorey25();
                if (this.graphs[k] != null)
                {
                    storey3.ctx = new GraphSerializationContext(storey.wr);
                    this.graphs[k].SerializeExtraInfo(storey3.ctx);
                    storey3.ctx = new GraphSerializationContext(storey.wr);
                    this.graphs[k].GetNodes(new GraphNodeDelegateCancelable(storey3.<>m__17));
                }
            }
            storey.wr.Close();
            return output.ToArray();
        }

        public void SerializeGraphs(NavGraph[] _graphs)
        {
            if (this.graphs != null)
            {
                throw new InvalidOperationException("Cannot serialize graphs multiple times.");
            }
            this.graphs = _graphs;
            if (this.zip == null)
            {
                throw new NullReferenceException("You must not call CloseSerialize before a call to this function");
            }
            if (this.graphs == null)
            {
                this.graphs = new NavGraph[0];
            }
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if (this.graphs[i] != null)
                {
                    byte[] bytes = this.Serialize(this.graphs[i]);
                    this.AddChecksum(bytes);
                    this.zip.AddEntry("graph" + i + ".binary", bytes);
                }
            }
        }

        private byte[] SerializeMeta()
        {
            this.meta.version = AstarPath.Version;
            this.meta.graphs = this.data.graphs.Length;
            this.meta.guids = new string[this.data.graphs.Length];
            this.meta.typeNames = new string[this.data.graphs.Length];
            this.meta.nodeCounts = new int[this.data.graphs.Length];
            for (int i = 0; i < this.data.graphs.Length; i++)
            {
                if (this.data.graphs[i] != null)
                {
                    this.meta.guids[i] = this.data.graphs[i].guid.ToString();
                    this.meta.typeNames[i] = this.data.graphs[i].GetType().FullName;
                }
            }
            MemoryStream output = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write("A*");
            writer.Write(this.meta.version.Major);
            writer.Write(this.meta.version.Minor);
            writer.Write(this.meta.version.Build);
            writer.Write(this.meta.version.Revision);
            writer.Write(this.meta.graphs);
            writer.Write(this.meta.guids.Length);
            for (int j = 0; j < this.meta.guids.Length; j++)
            {
                writer.Write(this.meta.guids[j]);
            }
            writer.Write(this.meta.typeNames.Length);
            for (int k = 0; k < this.meta.typeNames.Length; k++)
            {
                writer.Write(this.meta.typeNames[k]);
            }
            writer.Write(this.meta.nodeCounts.Length);
            for (int m = 0; m < this.meta.nodeCounts.Length; m++)
            {
                writer.Write(this.meta.nodeCounts[m]);
            }
            return output.ToArray();
        }

        private byte[] SerializeNodeConnections(int index)
        {
            return new byte[0];
        }

        public void SerializeNodes()
        {
            if (this.settings.nodes)
            {
                if (this.graphs == null)
                {
                    throw new InvalidOperationException("Cannot serialize nodes with no serialized graphs (call SerializeGraphs first)");
                }
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    byte[] bytes = this.SerializeNodes(i);
                    this.AddChecksum(bytes);
                    this.zip.AddEntry("graph" + i + "_nodes.binary", bytes);
                }
                for (int j = 0; j < this.graphs.Length; j++)
                {
                    byte[] buffer2 = this.SerializeNodeConnections(j);
                    this.AddChecksum(buffer2);
                    this.zip.AddEntry("graph" + j + "_conns.binary", buffer2);
                }
            }
        }

        private byte[] SerializeNodes(int index)
        {
            return new byte[0];
        }

        public void SerializeUserConnections(UserConnection[] conns)
        {
        }

        [CompilerGenerated]
        private sealed class <DeserializeExtraInfo>c__AnonStorey29
        {
            internal int totCount;

            internal bool <>m__1B(GraphNode node)
            {
                this.totCount = Math.Max(node.NodeIndex, this.totCount);
                if (node.NodeIndex == -1)
                {
                    Debug.LogError("Graph contains destroyed nodes. This is a bug.");
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <DeserializeExtraInfo>c__AnonStorey2A
        {
            internal GraphNode[] int2Node;
            internal BinaryReader reader;

            internal bool <>m__1C(GraphNode node)
            {
                this.int2Node[this.reader.ReadInt32()] = node;
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <DeserializeExtraInfo>c__AnonStorey2B
        {
            internal GraphSerializationContext ctx;

            internal bool <>m__1D(GraphNode node)
            {
                node.DeserializeReferences(this.ctx);
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfo>c__AnonStorey26
        {
            internal int totCount;

            internal bool <>m__18(GraphNode node)
            {
                this.totCount = Math.Max(node.NodeIndex, this.totCount);
                if (node.NodeIndex == -1)
                {
                    Debug.LogError("Graph contains destroyed nodes. This is a bug.");
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfo>c__AnonStorey27
        {
            internal int c;
            internal BinaryWriter wr;

            internal bool <>m__19(GraphNode node)
            {
                this.c = Math.Max(node.NodeIndex, this.c);
                this.wr.Write(node.NodeIndex);
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfo>c__AnonStorey28
        {
            internal GraphSerializationContext ctx;

            internal bool <>m__1A(GraphNode node)
            {
                node.SerializeReferences(this.ctx);
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfoBytes>c__AnonStorey23
        {
            internal int totCount;
            internal BinaryWriter wr;

            internal bool <>m__15(GraphNode node)
            {
                this.totCount = Math.Max(node.NodeIndex, this.totCount);
                if (node.NodeIndex == -1)
                {
                    Debug.LogError("Graph contains destroyed nodes. This is a bug.");
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfoBytes>c__AnonStorey24
        {
            internal AstarSerializer.<SerializeExtraInfoBytes>c__AnonStorey23 <>f__ref$35;
            internal int c;

            internal bool <>m__16(GraphNode node)
            {
                this.c = Math.Max(node.NodeIndex, this.c);
                this.<>f__ref$35.wr.Write(node.NodeIndex);
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <SerializeExtraInfoBytes>c__AnonStorey25
        {
            internal GraphSerializationContext ctx;

            internal bool <>m__17(GraphNode node)
            {
                node.SerializeReferences(this.ctx);
                return true;
            }
        }
    }
}

