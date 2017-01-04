namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ApolloStringParser
    {
        private Dictionary<string, string> objectCollection;

        public ApolloStringParser()
        {
            this.objectCollection = new Dictionary<string, string>();
        }

        public ApolloStringParser(string src)
        {
            this.objectCollection = new Dictionary<string, string>();
            this.Source = src;
            this.parse(src, this.objectCollection);
        }

        public int GetInt(string objName)
        {
            return this.GetInt(objName, 0);
        }

        public int GetInt(string objName, int defaultValue)
        {
            if (((objName != null) && (this.objectCollection.Count != 0)) && this.objectCollection.ContainsKey(objName))
            {
                int num;
                string s = this.objectCollection[objName];
                if (int.TryParse(s, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        public T GetObject<T>(string objName) where T: ApolloStruct<T>
        {
            if ((objName == null) || (this.objectCollection.Count == 0))
            {
                return null;
            }
            T local = null;
            if (this.objectCollection.ContainsKey(objName))
            {
                local = (T) Activator.CreateInstance(typeof(T));
                string src = this.objectCollection[objName];
                src = ReplaceApolloString(src);
                local = local.FromString(src);
            }
            return local;
        }

        public string GetString(string objName)
        {
            if (((objName != null) && (this.objectCollection.Count != 0)) && this.objectCollection.ContainsKey(objName))
            {
                return this.objectCollection[objName];
            }
            return null;
        }

        public uint GetUInt32(string objName)
        {
            return this.GetUInt32(objName, 0);
        }

        public uint GetUInt32(string objName, uint defaultValue)
        {
            if (((objName != null) && (this.objectCollection.Count != 0)) && this.objectCollection.ContainsKey(objName))
            {
                uint num;
                string s = this.objectCollection[objName];
                if (uint.TryParse(s, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        public ulong GetUInt64(string objName)
        {
            return this.GetUInt64(objName, 0L);
        }

        public ulong GetUInt64(string objName, ulong defaultValue)
        {
            if (((objName != null) && (this.objectCollection.Count != 0)) && this.objectCollection.ContainsKey(objName))
            {
                ulong num;
                string s = this.objectCollection[objName];
                if (ulong.TryParse(s, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        private void parse(string src, Dictionary<string, string> collection)
        {
            if ((src == null) || (src.Length == 0))
            {
                ADebug.LogError("ApolloStringParser src is null");
            }
            else
            {
                char[] separator = new char[] { '&' };
                foreach (string str in src.Split(separator))
                {
                    char[] chArray2 = new char[] { '=' };
                    string[] strArray3 = str.Split(chArray2);
                    if (strArray3.Length > 1)
                    {
                        if (collection.ContainsKey(strArray3[0]))
                        {
                            collection[strArray3[0]] = strArray3[1];
                        }
                        else
                        {
                            collection.Add(strArray3[0], strArray3[1]);
                        }
                    }
                }
            }
        }

        public static string ReplaceApolloString(string src)
        {
            if (src == null)
            {
                return null;
            }
            return src.Replace("%26", "&").Replace("%3d", "=").Replace("%25", "%");
        }

        public static string ReplaceApolloStringQuto(string src)
        {
            if (src == null)
            {
                return null;
            }
            return src.Replace("%26", "&").Replace("%3d", "=").Replace("%25", "%").Replace("%2c", ",");
        }

        public string Source { get; set; }
    }
}

