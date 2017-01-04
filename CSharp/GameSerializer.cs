using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameSerializer
{
    public const string DOM_ATTR_DISABLE = "DIS";
    public const string DOM_ATTR_IS_NULL = "NULL";
    private const string DOM_ATTR_JUDGETYPE_ARRAY = "Arr";
    private const string DOM_ATTR_JUDGETYPE_COMMON = "Com";
    private const string DOM_ATTR_JUDGETYPE_CUSTOM = "Cus";
    private const string DOM_ATTR_JUDGETYPE_ENUM = "Enum";
    private const string DOM_ATTR_JUDGETYPE_PRIMITIVE = "Pri";
    private const string DOM_ATTR_JUDGETYPE_REF = "Ref";
    public const string DOM_ATTR_LIGHTMAP_IDX = "I";
    public const string DOM_ATTR_LIGHTMAP_TILEOFFSET = "TO";
    public const string DOM_ATTR_NAME_ARRAY_SIZE = "Size";
    public const string DOM_ATTR_NAME_JUDGETYPE = "JT";
    public const string DOM_ATTR_NAME_OBJECT_TYPE = "Type";
    public const string DOM_ATTR_NAME_PREFAB = "PFB";
    public const string DOM_ATTR_NAME_TRANSFORM_ACTIVE = "A";
    public const string DOM_ATTR_NAME_TRANSFORM_LAYER = "L";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_POS = "P";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_ROT = "R";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_SCL = "S";
    public const string DOM_ATTR_NAME_TRANSFORM_TAG = "Tag";
    public const string DOM_ATTR_NAME_VALUE = "V";
    public const string DOM_LIGHTMAP_INFO = "LMI";
    public const string DOM_NODE_NAME_CHILDNODE = "CHD";
    public const string DOM_NODE_NAME_COMPONENT = "Cop";
    public const string DOM_NODE_NAME_OBJNAME = "ON";
    public const string DOM_NODE_NAME_TRANSFORM = "T";
    public const string DOM_ROOT_TAG = "root";
    public const string K_EXPORT_PATH = "/Resources/SceneExport";
    public const string K_EXPORT_PATH_IN_RESOURCES_FOLDER = "SceneExport";
    public string m_domPath = Application.dataPath;
    private ArrayList m_storedObjs = new ArrayList();
    public const string PREFABREF_ASSET_DIR = "PrefabAssets";
    private const string RESOURCES_DIR = "Assets/Resources";
    private const BindingFlags s_bindFlag = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static System.Type[] s_ComponentsCannotSerialize = new System.Type[] { typeof(Rigidbody), typeof(Rigidbody2D), typeof(MeshFilter), typeof(Renderer), typeof(PhysicMaterial), typeof(Avatar), typeof(Material), typeof(Animator), typeof(MeshCollider) };
    private static DictionaryView<System.Type, ICustomizedComponentSerializer> s_componentSerializerTypeCache = null;
    private static System.Type[] s_FieldsCannotSerialize = new System.Type[] { typeof(Mesh) };
    private static GameObject s_gameObjectRoot4Read = null;
    private static DictionaryView<System.Type, ICustomizedObjectSerializer> s_objectSerializerTypeCache = null;
    private static string s_saveFailStr = string.Empty;
    private static int s_saveRecurTimes = 0;
    private static Dictionary<string, System.Type> s_typeCache = new Dictionary<string, System.Type>();

    private static bool CheckObjectLegal(GameObject go, Component[] cpnt, GameObject prefab)
    {
        if (prefab == null)
        {
            for (int i = 0; i < cpnt.Length; i++)
            {
                for (int j = 0; j < s_ComponentsCannotSerialize.Length; j++)
                {
                    if ((cpnt[i] != null) && (cpnt[i].GetType() == s_ComponentsCannotSerialize[j]))
                    {
                        Debug.LogWarning("忽略保存对象:" + GetObjectHierachy(go) + "，因为其有" + GetPureType(s_ComponentsCannotSerialize[j].ToString()) + "组件却不是Prefab");
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static object CreateInstance(string typeStr)
    {
        return CreateInstance(GetType(typeStr));
    }

    public static object CreateInstance(System.Type type)
    {
        object obj2 = null;
        try
        {
            obj2 = Activator.CreateInstance(type);
        }
        catch (Exception exception)
        {
            object[] inParameters = new object[] { (type == null) ? "UnkownType" : type.ToString(), exception };
            DebugHelper.Assert(obj2 != null, "{0} create failed. due to exception: {1}", inParameters);
        }
        return obj2;
    }

    private static void DeserializeObject(BinaryNode objInfoNode, GameObject go)
    {
        BinaryNode node = objInfoNode.SelectSingleNode("T");
        if (node != null)
        {
            try
            {
                byte[] binaryAttribute = GetBinaryAttribute(node, "P");
                if (binaryAttribute != null)
                {
                    go.transform.position = UnityBasetypeSerializer.BytesToVector3(binaryAttribute);
                }
                byte[] data = GetBinaryAttribute(node, "R");
                if (data != null)
                {
                    go.transform.rotation = UnityBasetypeSerializer.BytesToQuaternion(data);
                }
                byte[] buffer3 = GetBinaryAttribute(node, "S");
                if (buffer3 != null)
                {
                    go.transform.localScale = UnityBasetypeSerializer.BytesToVector3(buffer3);
                }
                string attribute = GetAttribute(node, "L");
                if (attribute != null)
                {
                    go.layer = Convert.ToInt32(attribute);
                }
                string str2 = GetAttribute(node, "Tag");
                if (str2 != null)
                {
                    go.tag = str2;
                }
                string str3 = GetAttribute(node, "A");
                if (str3 != null)
                {
                    go.SetActive(str3.Equals("True"));
                }
            }
            catch (Exception)
            {
                object[] inParameters = new object[] { go.name };
                DebugHelper.Assert(false, "Gameobject {0} transform load failed!", inParameters);
            }
        }
        BinaryNode node2 = objInfoNode.SelectSingleNode("LMI");
        if (node2 != null)
        {
            Renderer component = go.GetComponent<Renderer>();
            if (component != null)
            {
                string str4 = GetAttribute(node2, "I");
                if (str4 != null)
                {
                    component.lightmapIndex = Convert.ToInt32(str4);
                }
                else
                {
                    component.lightmapIndex = -1;
                }
                byte[] buffer4 = GetBinaryAttribute(node2, "TO");
                if (buffer4 != null)
                {
                    component.set_lightmapTilingOffset(UnityBasetypeSerializer.BytesToVector4(buffer4));
                }
            }
        }
    }

    public static GameObject FindRootGameObject(GameObject anyGo)
    {
        GameObject gameObject = anyGo;
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
        }
        return gameObject;
    }

    public static string GetAttribute(BinaryNode node, string attName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attName)
            {
                return attr.GetValueString();
            }
        }
        return null;
    }

    public static byte[] GetBinaryAttribute(BinaryNode node, string attName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attName)
            {
                return attr.GetValue();
            }
        }
        return null;
    }

    private static ICustomizedComponentSerializer GetComponentSerlizer(System.Type type)
    {
        ICustomizedComponentSerializer serializer = null;
        if (componentSerializerTypeCache.TryGetValue(type, out serializer))
        {
            return serializer;
        }
        return null;
    }

    public static UnityEngine.Object GetGameObjectFromPath(string pathname, string typeName = null)
    {
        if (string.IsNullOrEmpty(pathname))
        {
            return null;
        }
        UnityEngine.Object resource = null;
        System.Type type = typeof(GameObject);
        if (typeName != null)
        {
            type = GetType(typeName);
        }
        resource = (UnityEngine.Object) GetResource(pathname, type);
        if (resource != null)
        {
            return resource;
        }
        if (s_gameObjectRoot4Read != null)
        {
            if (pathname == string.Format("/{0}", s_gameObjectRoot4Read.name))
            {
                return s_gameObjectRoot4Read;
            }
            string name = pathname.Replace(string.Format("/{0}/", s_gameObjectRoot4Read.name), string.Empty);
            Transform transform = s_gameObjectRoot4Read.transform.Find(name);
            if (transform == null)
            {
                return resource;
            }
            return transform.gameObject;
        }
        return GameObject.Find(pathname);
    }

    public static string GetGameObjectPathName(GameObject go)
    {
        string str = string.Empty;
        if (go == null)
        {
            return "null";
        }
        if ((str == null) || (str.Length == 0))
        {
            try
            {
                str = "/" + go.name;
                for (GameObject obj2 = go; obj2.transform.parent != null; obj2 = obj2.transform.parent.gameObject)
                {
                    str = "/" + obj2.transform.parent.name + str;
                }
            }
            catch (Exception exception)
            {
                Debug.Log("Get gameobject " + go.name + " path failed!");
                Debug.LogError(exception);
                str = string.Empty;
            }
        }
        return str;
    }

    private static System.Type GetMIType(MemberInfo mi)
    {
        if (mi != null)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                {
                    FieldInfo info = (FieldInfo) mi;
                    return info.FieldType;
                }
                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    return info2.PropertyType;
                }
            }
        }
        return null;
    }

    private static string GetMITypeStr(MemberInfo mi)
    {
        if (mi != null)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                {
                    FieldInfo info = (FieldInfo) mi;
                    return info.FieldType.ToString();
                }
                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    return info2.PropertyType.ToString();
                }
            }
        }
        return string.Empty;
    }

    private static object GetMIValue(MemberInfo mi, object owner)
    {
        try
        {
            PropertyInfo info2;
            if ((owner == null) || (mi == null))
            {
                goto Label_00C9;
            }
            MemberTypes memberType = mi.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType == MemberTypes.Property)
                {
                    goto Label_003C;
                }
                goto Label_00C9;
            }
            FieldInfo info = (FieldInfo) mi;
            return info.GetValue(owner);
        Label_003C:
            info2 = (PropertyInfo) mi;
            return info2.GetValue(owner, null);
        }
        catch (Exception exception)
        {
            DebugHelper.Assert(mi != null);
            if (mi != null)
            {
                Debug.Log(string.Concat(new object[] { GetMITypeStr(mi), mi.MemberType, " ", mi.ReflectedType.ToString(), " ", mi.Name, " seems not support !!!", exception }));
            }
        }
    Label_00C9:
        return null;
    }

    public static string GetNodeAttr(BinaryNode node, string attrName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attrName)
            {
                return attr.GetValueString();
            }
        }
        return null;
    }

    public static object GetObject(BinaryNode currNode)
    {
        string nodeAttr = GetNodeAttr(currNode, "NULL");
        object o = null;
        if (nodeAttr == null)
        {
            string typeStr = GetNodeAttr(currNode, "Type");
            string s = GetNodeAttr(currNode, "V");
            string str4 = GetNodeAttr(currNode, "JT");
            if ("Arr".Equals(str4))
            {
                if (typeStr == null)
                {
                    return o;
                }
                System.Type elementType = GetType(typeStr.Replace("[]", string.Empty));
                if (elementType == null)
                {
                    Debug.LogError("Array type " + typeStr + " create failed!");
                    return null;
                }
                Array array = Array.CreateInstance(elementType, currNode.GetChildNum());
                for (int i = 0; i < array.Length; i++)
                {
                    array.SetValue(GetObject(currNode.GetChild(i)), i);
                }
                return array;
            }
            if ("Cus".Equals(str4))
            {
                if (typeStr != null)
                {
                    System.Type type = GetType(typeStr);
                    ICustomizedObjectSerializer objectSerlizer = GetObjectSerlizer(type);
                    if ((objectSerlizer != null) && (objectSerlizer is ICustomInstantiate))
                    {
                        o = ((ICustomInstantiate) objectSerlizer).Instantiate(currNode);
                    }
                    else
                    {
                        o = CreateInstance(type);
                    }
                    if (o == null)
                    {
                        return null;
                    }
                    if (objectSerlizer != null)
                    {
                        objectSerlizer.ObjectDeserialize(ref o, currNode);
                    }
                }
                return o;
            }
            if ("Enum".Equals(str4))
            {
                if (typeStr != null)
                {
                    o = Enum.ToObject(GetType(typeStr), int.Parse(s));
                }
                return o;
            }
            if ("Pri".Equals(str4))
            {
                if (typeStr != null)
                {
                    o = Convert.ChangeType(s, GetType(typeStr));
                }
                return o;
            }
            if ("Ref".Equals(str4))
            {
                UnityEngine.Object gameObjectFromPath = GetGameObjectFromPath(s, typeStr);
                if (gameObjectFromPath != null)
                {
                    if (gameObjectFromPath is GameObject)
                    {
                        if (typeStr == null)
                        {
                            return o;
                        }
                        string pureType = GetPureType(typeStr);
                        if (!"GameObject".Equals(pureType))
                        {
                            o = (gameObjectFromPath as GameObject).GetComponent(pureType);
                            if (o == null)
                            {
                                Debug.LogError("No " + pureType + " component found in " + s);
                            }
                            return o;
                        }
                    }
                    return gameObjectFromPath;
                }
                o = null;
                Debug.LogError("Load gameobject " + s + " failed!");
                return o;
            }
            if ("Com".Equals(str4))
            {
                o = CreateInstance(typeStr);
                if (o == null)
                {
                    return null;
                }
                MemberInfo[] members = o.GetType().GetMembers();
                for (int j = 0; j < members.Length; j++)
                {
                    if (IsMINeedExport(members[j]))
                    {
                        BinaryNode node = currNode.SelectSingleNode(members[j].Name);
                        if (node != null)
                        {
                            try
                            {
                                object obj4 = GetObject(node);
                                if ((node != null) && (obj4 != null))
                                {
                                    SetMIValue(members[j], o, obj4);
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.LogError(string.Concat(new object[] { "Set field value failed! Field ", members[j].Name, " in ", o.GetType(), "e:", exception }));
                            }
                        }
                    }
                }
            }
        }
        return o;
    }

    public static string GetObjectHierachy(GameObject go)
    {
        string str = "/" + go.name;
        if (go.transform.parent != null)
        {
            str = GetObjectHierachy(go.transform.parent.gameObject) + str;
        }
        return str;
    }

    private static ICustomizedObjectSerializer GetObjectSerlizer(System.Type type)
    {
        ICustomizedObjectSerializer serializer = null;
        if (type.IsGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }
        if (objectSerializerTypeCache.TryGetValue(type, out serializer))
        {
            return serializer;
        }
        return null;
    }

    private string GetPathFromGameObj(GameObject obj, string org_path)
    {
        string str = org_path;
        if (str.Contains(obj.transform.name))
        {
            int startIndex = (str.IndexOf(obj.name) + obj.name.Length) + 1;
            str = str.Substring(startIndex, (str.Length - startIndex) - 1);
        }
        return str;
    }

    public static string GetPureType(object o)
    {
        if (o is Component)
        {
            return GetPureType(o.GetType().ToString());
        }
        return o.GetType().ToString();
    }

    public static string GetPureType(string str)
    {
        string str2 = str;
        if (str2.Contains("."))
        {
            str2 = str2.Substring(str2.LastIndexOf(".") + 1, (str2.Length - str2.LastIndexOf(".")) - 1);
        }
        return str2;
    }

    public static object GetResource(string pathName, System.Type type)
    {
        object prefabObject = null;
        string fullPathInResources = pathName.Replace(@"\", "/");
        if (fullPathInResources.Contains("PrefabAssets/"))
        {
            PrefabRefAsset content = (PrefabRefAsset) Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(PrefabRefAsset), enResourceType.BattleScene, false, false).m_content;
            if (content != null)
            {
                prefabObject = content.m_prefabObject;
            }
            return prefabObject;
        }
        string str2 = pathName;
        string str3 = "Assets/Resources";
        int index = pathName.IndexOf(str3);
        if (index >= 0)
        {
            str2 = pathName.Substring((index + str3.Length) + 1);
        }
        return Singleton<CResourceManager>.GetInstance().GetResource(str2, type, enResourceType.BattleScene, false, false).m_content;
    }

    public static System.Type GetType(string typeStr)
    {
        System.Type type = null;
        if (s_typeCache.TryGetValue(typeStr, out type))
        {
            return type;
        }
        if ((typeStr.Contains("`") && typeStr.Contains("[")) && typeStr.Contains("]"))
        {
            string str = typeStr.Substring(0, typeStr.IndexOf("["));
            int index = typeStr.IndexOf("[");
            int num2 = typeStr.LastIndexOf("]");
            char[] separator = new char[] { ',' };
            string[] strArray = typeStr.Substring(index + 1, (num2 - index) - 1).Split(separator);
            System.Type type2 = GetType(str);
            System.Type[] typeArguments = new System.Type[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                typeArguments[i] = GetType(strArray[i]);
            }
            System.Type type3 = type2.MakeGenericType(typeArguments);
            s_typeCache.Add(typeStr, type3);
            return type3;
        }
        System.Type type4 = Utility.GetType(typeStr);
        if (type4 == null)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int j = 0; j < assemblies.Length; j++)
            {
                if (assemblies[j] != null)
                {
                    System.Type[] types = assemblies[j].GetTypes();
                    for (int k = 0; k < types.Length; k++)
                    {
                        if (typeStr == types[k].Name)
                        {
                            type4 = types[k];
                            s_typeCache.Add(typeStr, type4);
                            return type4;
                        }
                    }
                }
            }
        }
        s_typeCache.Add(typeStr, type4);
        return type4;
    }

    private static void InitComponets(BinaryNode domNode, GameObject go)
    {
        Component[] components = go.GetComponents(typeof(Component));
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "Cop")
            {
                string nodeAttr = GetNodeAttr(child, "Type");
                Component component = null;
                for (int j = 0; j < components.Length; j++)
                {
                    if (!isNull(components[j]) && GetPureType(components[j].GetType().ToString()).Equals(nodeAttr))
                    {
                        component = components[j];
                        break;
                    }
                }
                if (component == null)
                {
                    component = go.AddComponent(GetType(nodeAttr));
                }
            }
        }
    }

    private static bool IsComplexObject(object o)
    {
        if (o == null)
        {
            return false;
        }
        if ((o is GameObject) || (o is Component))
        {
            return false;
        }
        return ((!o.GetType().IsValueType && !(o is string)) && (o.GetType() != typeof(decimal)));
    }

    public static bool IsEqual(object o1, object o2)
    {
        if ((o1 != null) || (o2 != null))
        {
            if ((o1 == null) || (o2 == null))
            {
                return false;
            }
            if (o1.GetType() != o2.GetType())
            {
                return false;
            }
            if (IsComplexObject(o1) || IsComplexObject(o2))
            {
                return false;
            }
            if (IsGameObject(o1) && IsGameObject(o2))
            {
                GameObject go = (GameObject) o1;
                GameObject obj3 = (GameObject) o2;
                string gameObjectPathName = GetGameObjectPathName(go);
                string str2 = GetGameObjectPathName(obj3);
                return (((gameObjectPathName != null) && (str2 != null)) && gameObjectPathName.Equals(str2));
            }
            if (!object.ReferenceEquals(o1.GetType(), o2.GetType()))
            {
                return false;
            }
            if (!o1.GetType().ToString().EndsWith("[]"))
            {
                return o1.Equals(o2);
            }
            Array array = (Array) o1;
            Array array2 = (Array) o2;
            if (array.GetLength(0) != array2.GetLength(0))
            {
                return false;
            }
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array.GetValue(i) != array2.GetValue(i))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsGameObject(object obj)
    {
        return (obj is GameObject);
    }

    private static bool IsInherit(System.Type type, System.Type BaseType)
    {
        if (type == BaseType)
        {
            return true;
        }
        if (BaseType.IsInterface)
        {
            return BaseType.IsAssignableFrom(type);
        }
        if (BaseType.IsValueType)
        {
            return false;
        }
        if (BaseType == typeof(Enum))
        {
            return type.IsEnum;
        }
        return type.IsSubclassOf(BaseType);
    }

    private static bool IsMINeedExport(MemberInfo mi)
    {
        if (mi.MemberType == MemberTypes.Field)
        {
            FieldInfo info = (FieldInfo) mi;
            if (info.IsLiteral && !info.IsInitOnly)
            {
                return false;
            }
            if (info.IsStatic)
            {
                return false;
            }
            return true;
        }
        if (mi.MemberType != MemberTypes.Property)
        {
            return false;
        }
        PropertyInfo info2 = (PropertyInfo) mi;
        return ((info2.GetSetMethod() != null) && info2.GetSetMethod().IsPublic);
    }

    private static bool IsNeedNotSave(object o, bool storeAsObject)
    {
        return IsNeedNotSaveByType(o.GetType(), storeAsObject);
    }

    private static bool IsNeedNotSaveByType(System.Type tp, bool storeAsObject)
    {
        if (storeAsObject)
        {
            for (int j = 0; j < s_FieldsCannotSerialize.Length; j++)
            {
                if (IsInherit(tp, s_FieldsCannotSerialize[j]))
                {
                    return true;
                }
            }
            return false;
        }
        for (int i = 0; i < s_ComponentsCannotSerialize.Length; i++)
        {
            if (IsInherit(tp, s_ComponentsCannotSerialize[i]))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsNeedNotSaveMemberInfo(object o, MemberInfo mi, bool storeAsObject)
    {
        if (!IsNeedNotSaveByType(GetMIType(mi), storeAsObject))
        {
            if (mi is PropertyInfo)
            {
                PropertyInfo info = (PropertyInfo) mi;
                if (!info.CanWrite)
                {
                    return true;
                }
                if (info.GetIndexParameters().Length != 0)
                {
                    return true;
                }
            }
            if ((((o == null) || IsUnityReferenceType(o.GetType())) || (GetMITypeStr(mi).EndsWith("[]") || (GetMIValue(mi, o) != o))) && (((o == null) || !(o is Component)) || ((!mi.Name.Equals("tag") && !mi.Name.Equals("name")) && !mi.Name.Equals("active"))))
            {
                return false;
            }
        }
        return true;
    }

    private static bool isNull(object obj)
    {
        return (object.ReferenceEquals(obj, null) || obj.Equals(null));
    }

    private static bool IsPrimitive(System.Type type)
    {
        return ((type.IsPrimitive || (type == typeof(string))) || (type == typeof(decimal)));
    }

    public static bool isStringEqual(string s1, string s2)
    {
        return (((s1 != null) && (s2 != null)) && s1.Equals(s2));
    }

    private static bool IsUnityReferenceType(System.Type type)
    {
        return (((IsInherit(type, typeof(GameObject)) || IsInherit(type, typeof(Component))) || (IsInherit(type, typeof(ScriptableObject)) || IsInherit(type, typeof(Mesh)))) || IsInherit(type, typeof(Material)));
    }

    private static GameObject Load(BinaryDomDocument document)
    {
        GameObject go = null;
        s_gameObjectRoot4Read = null;
        BinaryNode root = document.Root;
        if (root != null)
        {
            go = LoadRecursionOnce(null, root);
            s_gameObjectRoot4Read = go;
            LoadRecursionTwice(null, root, go);
        }
        if (Camera.main != null)
        {
            Camera.SetupCurrent(Camera.main);
        }
        s_gameObjectRoot4Read = null;
        return go;
    }

    public GameObject Load(LevelResAsset lightmapAsset)
    {
        GameObject obj2 = null;
        BinaryDomDocument document = null;
        if (lightmapAsset != null)
        {
            if (lightmapAsset.levelDom != null)
            {
                document = new BinaryDomDocument(lightmapAsset.levelDom.bytes);
                obj2 = Load(document);
            }
            else
            {
                return null;
            }
            int length = lightmapAsset.lightmapFar.Length;
            LightmapData[] dataArray = new LightmapData[length];
            for (int i = 0; i < length; i++)
            {
                dataArray[i] = new LightmapData { lightmapFar = lightmapAsset.lightmapFar[i], lightmapNear = lightmapAsset.lightmapNear[i] };
            }
            LightmapSettings.lightmaps = dataArray;
        }
        return obj2;
    }

    public GameObject Load(byte[] data)
    {
        BinaryDomDocument document = new BinaryDomDocument(data);
        return Load(document);
    }

    private static void LoadComponets(BinaryNode domNode, GameObject go)
    {
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "Cop")
            {
                string nodeAttr = GetNodeAttr(child, "Type");
                Component cmp = go.GetComponent(nodeAttr);
                if (cmp != null)
                {
                    if ((GetNodeAttr(child, "DIS") != null) && (cmp is Behaviour))
                    {
                        Behaviour behaviour = (Behaviour) cmp;
                        behaviour.enabled = false;
                    }
                    ICustomizedComponentSerializer componentSerlizer = GetComponentSerlizer(cmp.GetType());
                    if (componentSerlizer != null)
                    {
                        componentSerlizer.ComponentDeserialize(cmp, child);
                    }
                    else
                    {
                        MemberInfo[] members = cmp.GetType().GetMembers();
                        for (int j = 0; j < members.Length; j++)
                        {
                            if (IsMINeedExport(members[j]))
                            {
                                BinaryNode currNode = child.SelectSingleNode(members[j].Name);
                                if (currNode != null)
                                {
                                    object obj2 = GetObject(currNode);
                                    try
                                    {
                                        if (obj2 != null)
                                        {
                                            SetMIValue(members[j], cmp, obj2);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private static GameObject LoadRecursionOnce(GameObject parentGo, BinaryNode domNode)
    {
        GameObject go = null;
        string nodeAttr = GetNodeAttr(domNode, "ON");
        if (parentGo != null)
        {
            for (int j = 0; j < parentGo.transform.childCount; j++)
            {
                if (parentGo.transform.GetChild(j).name.Equals(nodeAttr))
                {
                    go = parentGo.transform.GetChild(j).gameObject;
                    break;
                }
            }
        }
        if (go == null)
        {
            string pathName = GetNodeAttr(domNode, "PFB");
            if ((pathName != null) && (pathName.Length != 0))
            {
                object resource = GetResource(pathName, typeof(GameObject));
                if ((resource == null) || !(resource is GameObject))
                {
                    Debug.LogError(pathName + " 不存在或者类型错误，请重新导出该场景");
                    go = new GameObject();
                }
                else
                {
                    GameObject original = resource as GameObject;
                    bool activeSelf = original.activeSelf;
                    original.SetActive(false);
                    go = (GameObject) UnityEngine.Object.Instantiate(original);
                    original.SetActive(activeSelf);
                }
            }
            else
            {
                go = new GameObject();
            }
        }
        Vector3 localScale = go.transform.localScale;
        go.name = GetNodeAttr(domNode, "ON");
        if (parentGo != null)
        {
            go.transform.parent = parentGo.transform;
        }
        go.transform.localScale = localScale;
        DeserializeObject(domNode, go);
        go.SetActive(false);
        InitComponets(domNode, go);
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "CHD")
            {
                LoadRecursionOnce(go, child);
            }
        }
        return go;
    }

    private static void LoadRecursionTwice(GameObject parentGo, BinaryNode domNode, GameObject go)
    {
        if (domNode == domNode.OwnerDocument.Root)
        {
            LoadComponets(domNode, go);
            int index = -1;
            for (int i = 0; i < domNode.GetChildNum(); i++)
            {
                BinaryNode child = domNode.GetChild(i);
                if (child.GetName() == "CHD")
                {
                    index++;
                    GameObject gameObject = go.transform.GetChild(index).gameObject;
                    if (gameObject != null)
                    {
                        LoadRecursionTwice(null, child, gameObject);
                    }
                }
            }
        }
        else
        {
            BinaryNode parentNode = domNode.ParentNode;
            for (int j = 0; j < parentNode.GetChildNum(); j++)
            {
                BinaryNode node = parentNode.GetChild(j);
                if ((node.GetName() == "CHD") && (GetAttribute(node, "ON") == go.name))
                {
                    LoadComponets(node, go);
                    if ((node.GetChildNum() > 0) && (go.transform.childCount > 0))
                    {
                        BinaryNode node4 = node.GetChild(0);
                        for (int k = 0; k < go.transform.childCount; k++)
                        {
                            GameObject obj3 = go.transform.GetChild(k).gameObject;
                            LoadRecursionTwice(null, node4, obj3);
                        }
                    }
                    domNode = node;
                }
            }
        }
        if (GetNodeAttr(domNode, "DIS") != null)
        {
            go.SetActive(false);
        }
        else
        {
            go.SetActive(true);
        }
    }

    private static void SetMIValue(MemberInfo mi, object owner, object value)
    {
        if (((owner != null) && (mi != null)) && !owner.ToString().Equals("null"))
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo) mi).SetValue(owner, value);
                    break;

                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    if (GetMIType(mi).ToString().EndsWith("[]"))
                    {
                        object[] objArray = (object[]) value;
                        IList list = (IList) info2.GetValue(owner, null);
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            list[i] = objArray[i];
                        }
                    }
                    else
                    {
                        try
                        {
                            info2.SetValue(owner, value, null);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                }
            }
        }
    }

    private static DictionaryView<System.Type, ICustomizedComponentSerializer> componentSerializerTypeCache
    {
        get
        {
            if (s_componentSerializerTypeCache == null)
            {
                s_componentSerializerTypeCache = new DictionaryView<System.Type, ICustomizedComponentSerializer>();
                ClassEnumerator enumerator = new ClassEnumerator(typeof(ComponentTypeSerializerAttribute), typeof(ICustomizedComponentSerializer), typeof(ComponentTypeSerializerAttribute).Assembly, true, false, false);
                foreach (System.Type type in enumerator.results)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(ComponentTypeSerializerAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        s_componentSerializerTypeCache.Add((customAttributes[0] as ComponentTypeSerializerAttribute).type, Activator.CreateInstance(type) as ICustomizedComponentSerializer);
                    }
                }
            }
            return s_componentSerializerTypeCache;
        }
    }

    private static DictionaryView<System.Type, ICustomizedObjectSerializer> objectSerializerTypeCache
    {
        get
        {
            if (s_objectSerializerTypeCache == null)
            {
                s_objectSerializerTypeCache = new DictionaryView<System.Type, ICustomizedObjectSerializer>();
                ClassEnumerator enumerator = new ClassEnumerator(typeof(ObjectTypeSerializerAttribute), typeof(ICustomizedObjectSerializer), typeof(ObjectTypeSerializerAttribute).Assembly, true, false, false);
                foreach (System.Type type in enumerator.results)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(ObjectTypeSerializerAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        System.Type key = (customAttributes[0] as ObjectTypeSerializerAttribute).type;
                        if (key.IsGenericType)
                        {
                            key = key.GetGenericTypeDefinition();
                        }
                        s_objectSerializerTypeCache.Add(key, Activator.CreateInstance(type) as ICustomizedObjectSerializer);
                    }
                }
            }
            return s_objectSerializerTypeCache;
        }
    }
}

