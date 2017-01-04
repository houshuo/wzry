namespace Pathfinding.Serialization
{
    using Pathfinding;
    using System;
    using System.IO;
    using UnityEngine;

    public class GraphSerializationContext
    {
        public readonly int graphIndex;
        private readonly GraphNode[] id2NodeMapping;
        public readonly BinaryReader reader;
        public readonly BinaryWriter writer;

        public GraphSerializationContext(BinaryWriter writer)
        {
            this.writer = writer;
        }

        public GraphSerializationContext(BinaryReader reader, GraphNode[] id2NodeMapping, int graphIndex)
        {
            this.reader = reader;
            this.id2NodeMapping = id2NodeMapping;
            this.graphIndex = graphIndex;
        }

        public UnityEngine.Object DeserializeUnityObject()
        {
            if (this.reader.ReadInt32() != 0x7fffffff)
            {
                string path = this.reader.ReadString();
                string typeName = this.reader.ReadString();
                string str3 = this.reader.ReadString();
                System.Type type = UtilityPlugin.GetType(typeName);
                if (type == null)
                {
                    Debug.LogError("Could not find type '" + typeName + "'. Cannot deserialize Unity reference");
                    return null;
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    UnityReferenceHelper[] helperArray = UnityEngine.Object.FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[];
                    for (int j = 0; j < helperArray.Length; j++)
                    {
                        if (helperArray[j].GetGUID() == str3)
                        {
                            if (type == typeof(GameObject))
                            {
                                return helperArray[j].gameObject;
                            }
                            return helperArray[j].GetComponent(type);
                        }
                    }
                }
                UnityEngine.Object[] objArray = Resources.LoadAll(path, type);
                for (int i = 0; i < objArray.Length; i++)
                {
                    if ((objArray[i].name == path) || (objArray.Length == 1))
                    {
                        return objArray[i];
                    }
                }
            }
            return null;
        }

        public Vector3 DeserializeVector3()
        {
            return new Vector3(this.reader.ReadSingle(), this.reader.ReadSingle(), this.reader.ReadSingle());
        }

        public GraphNode GetNodeFromIdentifier(int id)
        {
            if (this.id2NodeMapping == null)
            {
                throw new Exception("Calling GetNodeFromIdentifier when serializing");
            }
            if (id == -1)
            {
                return null;
            }
            GraphNode node = this.id2NodeMapping[id];
            if (node == null)
            {
                throw new Exception("Invalid id");
            }
            return node;
        }

        public int GetNodeIdentifier(GraphNode node)
        {
            return ((node != null) ? node.NodeIndex : -1);
        }

        public void SerializeUnityObject(UnityEngine.Object ob)
        {
            if (ob == null)
            {
                this.writer.Write(0x7fffffff);
            }
            else
            {
                int instanceID = ob.GetInstanceID();
                string name = ob.name;
                string assemblyQualifiedName = ob.GetType().AssemblyQualifiedName;
                string gUID = string.Empty;
                Component component = ob as Component;
                GameObject gameObject = ob as GameObject;
                if ((component != null) || (gameObject != null))
                {
                    if ((component != null) && (gameObject == null))
                    {
                        gameObject = component.gameObject;
                    }
                    UnityReferenceHelper helper = gameObject.GetComponent<UnityReferenceHelper>();
                    if (helper == null)
                    {
                        Debug.Log("Adding UnityReferenceHelper to Unity Reference '" + ob.name + "'");
                        helper = gameObject.AddComponent<UnityReferenceHelper>();
                    }
                    helper.Reset();
                    gUID = helper.GetGUID();
                }
                this.writer.Write(instanceID);
                this.writer.Write(name);
                this.writer.Write(assemblyQualifiedName);
                this.writer.Write(gUID);
            }
        }

        public void SerializeVector3(Vector3 v)
        {
            this.writer.Write(v.x);
            this.writer.Write(v.y);
            this.writer.Write(v.z);
        }
    }
}

