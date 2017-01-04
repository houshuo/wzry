namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ObjImporter
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map2;

        private static meshStruct createMeshStruct(string filename)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            meshStruct struct2 = new meshStruct {
                fileName = filename
            };
            StreamReader reader = File.OpenText(filename);
            string s = reader.ReadToEnd();
            reader.Dispose();
            using (StringReader reader2 = new StringReader(s))
            {
                string str2 = reader2.ReadLine();
                char[] separator = new char[] { ' ' };
                while (str2 != null)
                {
                    if ((!str2.StartsWith("f ") && !str2.StartsWith("v ")) && (!str2.StartsWith("vt ") && !str2.StartsWith("vn ")))
                    {
                        str2 = reader2.ReadLine();
                        if (str2 != null)
                        {
                            str2 = str2.Replace("  ", " ");
                        }
                        continue;
                    }
                    string[] strArray = str2.Trim().Split(separator, 50);
                    string key = strArray[0];
                    if (key != null)
                    {
                        int num6;
                        if (<>f__switch$map1 == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                            dictionary.Add("v", 0);
                            dictionary.Add("vt", 1);
                            dictionary.Add("vn", 2);
                            dictionary.Add("f", 3);
                            <>f__switch$map1 = dictionary;
                        }
                        if (<>f__switch$map1.TryGetValue(key, out num6))
                        {
                            switch (num6)
                            {
                                case 0:
                                    num2++;
                                    break;

                                case 1:
                                    num3++;
                                    break;

                                case 2:
                                    num4++;
                                    break;

                                case 3:
                                    num5 = (num5 + strArray.Length) - 1;
                                    num += 3 * (strArray.Length - 2);
                                    break;
                            }
                        }
                    }
                    str2 = reader2.ReadLine();
                    if (str2 != null)
                    {
                        str2 = str2.Replace("  ", " ");
                    }
                }
            }
            struct2.triangles = new int[num];
            struct2.vertices = new Vector3[num2];
            struct2.uv = new Vector2[num3];
            struct2.normals = new Vector3[num4];
            struct2.faceData = new Vector3[num5];
            return struct2;
        }

        public static Mesh ImportFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("No file was found at '" + filePath + "'");
                return null;
            }
            meshStruct struct2 = createMeshStruct(filePath);
            populateMeshStruct(ref struct2);
            Vector3[] vectorArray = new Vector3[struct2.faceData.Length];
            Vector2[] vectorArray2 = new Vector2[struct2.faceData.Length];
            Vector3[] vectorArray3 = new Vector3[struct2.faceData.Length];
            int index = 0;
            foreach (Vector3 vector in struct2.faceData)
            {
                vectorArray[index] = struct2.vertices[((int) vector.x) - 1];
                if (vector.y >= 1f)
                {
                    vectorArray2[index] = struct2.uv[((int) vector.y) - 1];
                }
                if (vector.z >= 1f)
                {
                    vectorArray3[index] = struct2.normals[((int) vector.z) - 1];
                }
                index++;
            }
            Mesh mesh = new Mesh {
                vertices = vectorArray,
                uv = vectorArray2,
                normals = vectorArray3,
                triangles = struct2.triangles
            };
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void populateMeshStruct(ref meshStruct mesh)
        {
            StreamReader reader = File.OpenText(mesh.fileName);
            string s = reader.ReadToEnd();
            reader.Close();
            using (StringReader reader2 = new StringReader(s))
            {
                string str2 = reader2.ReadLine();
                char[] separator = new char[] { ' ' };
                char[] chArray2 = new char[] { '/' };
                int index = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                while (str2 != null)
                {
                    int num8;
                    if ((((!str2.StartsWith("f ") && !str2.StartsWith("v ")) && (!str2.StartsWith("vt ") && !str2.StartsWith("vn "))) && ((!str2.StartsWith("g ") && !str2.StartsWith("usemtl ")) && (!str2.StartsWith("mtllib ") && !str2.StartsWith("vt1 ")))) && ((!str2.StartsWith("vt2 ") && !str2.StartsWith("vc ")) && !str2.StartsWith("usemap ")))
                    {
                        str2 = reader2.ReadLine();
                        if (str2 != null)
                        {
                            str2 = str2.Replace("  ", " ");
                        }
                        continue;
                    }
                    string[] strArray = str2.Trim().Split(separator, 50);
                    string key = strArray[0];
                    if (key != null)
                    {
                        int num9;
                        if (<>f__switch$map2 == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
                            dictionary.Add("g", 0);
                            dictionary.Add("usemtl", 1);
                            dictionary.Add("usemap", 2);
                            dictionary.Add("mtllib", 3);
                            dictionary.Add("v", 4);
                            dictionary.Add("vt", 5);
                            dictionary.Add("vt1", 6);
                            dictionary.Add("vt2", 7);
                            dictionary.Add("vn", 8);
                            dictionary.Add("vc", 9);
                            dictionary.Add("f", 10);
                            <>f__switch$map2 = dictionary;
                        }
                        if (<>f__switch$map2.TryGetValue(key, out num9))
                        {
                            switch (num9)
                            {
                                case 4:
                                    goto Label_0258;

                                case 5:
                                    goto Label_0295;

                                case 6:
                                    goto Label_02C9;

                                case 7:
                                    goto Label_02FD;

                                case 8:
                                    goto Label_0331;

                                case 10:
                                    goto Label_0373;
                            }
                        }
                    }
                    goto Label_04A0;
                Label_0258:
                    mesh.vertices[num3] = new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]));
                    num3++;
                    goto Label_04A0;
                Label_0295:
                    mesh.uv[num5] = new Vector2(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
                    num5++;
                    goto Label_04A0;
                Label_02C9:
                    mesh.uv[num6] = new Vector2(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
                    num6++;
                    goto Label_04A0;
                Label_02FD:
                    mesh.uv[num7] = new Vector2(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]));
                    num7++;
                    goto Label_04A0;
                Label_0331:
                    mesh.normals[num4] = new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]));
                    num4++;
                    goto Label_04A0;
                Label_0373:
                    num8 = 1;
                    List<int> list = new List<int>();
                    while ((num8 < strArray.Length) && ((string.Empty + strArray[num8]).Length > 0))
                    {
                        Vector3 vector = new Vector3();
                        string[] strArray2 = strArray[num8].Split(chArray2, 3);
                        vector.x = Convert.ToInt32(strArray2[0]);
                        if (strArray2.Length > 1)
                        {
                            if (strArray2[1] != string.Empty)
                            {
                                vector.y = Convert.ToInt32(strArray2[1]);
                            }
                            vector.z = Convert.ToInt32(strArray2[2]);
                        }
                        num8++;
                        mesh.faceData[num2] = vector;
                        list.Add(num2);
                        num2++;
                    }
                    for (num8 = 1; (num8 + 2) < strArray.Length; num8++)
                    {
                        mesh.triangles[index] = list[0];
                        index++;
                        mesh.triangles[index] = list[num8];
                        index++;
                        mesh.triangles[index] = list[num8 + 1];
                        index++;
                    }
                Label_04A0:
                    str2 = reader2.ReadLine();
                    if (str2 != null)
                    {
                        str2 = str2.Replace("  ", " ");
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct meshStruct
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector2[] uv;
            public Vector2[] uv1;
            public Vector2[] uv2;
            public int[] triangles;
            public int[] faceVerts;
            public int[] faceUVs;
            public Vector3[] faceData;
            public string name;
            public string fileName;
        }
    }
}

