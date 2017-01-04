namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class ApolloObjectManager : MonoBehaviour
    {
        private ListView<ApolloObject> acceptUpdatedObjectList = new ListView<ApolloObject>();
        private static GameObject container;
        private DictionaryView<ulong, ApolloObject> dictObjectCollection = new DictionaryView<ulong, ApolloObject>();
        private static bool init;
        private static ApolloObjectManager instance;
        private ListView<ApolloObject> removedReflectibleList = new ListView<ApolloObject>();
        private ListView<ApolloObject> removedUpdatableList = new ListView<ApolloObject>();

        private ApolloObjectManager()
        {
            setApolloSendMessageCallback(new ApolloSendMessageDelegate(ApolloObjectManager.onSendMessage));
            setApolloSendStructCallback(new ApolloSendStructDelegate(ApolloObjectManager.onSendStruct));
            setApolloSendResultCallback(new ApolloSendResultDelegate(ApolloObjectManager.onSendResult));
            setApolloSendBufferCallback(new ApolloSendBufferDelegate(ApolloObjectManager.onSendBuffer));
            setApolloSendResultBufferCallback(new ApolloSendResultBufferDelegate(ApolloObjectManager.onSendResultBuffer));
        }

        public void AddAcceptUpdatedObject(ApolloObject obj)
        {
            if ((obj != null) && !this.acceptUpdatedObjectList.Contains(obj))
            {
                this.acceptUpdatedObjectList.Add(obj);
            }
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void addApolloObject(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string objName);
        public void AddObject(ApolloObject obj)
        {
            if ((obj != null) && !this.dictObjectCollection.ContainsKey(obj.ObjectId))
            {
                this.dictObjectCollection.Add(obj.ObjectId, obj);
                addApolloObject(obj.ObjectId, obj.GetType().FullName);
            }
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void apollo_quit();
        private void Awake()
        {
        }

        public void ClearObjects()
        {
            foreach (ulong num in this.dictObjectCollection.Keys)
            {
                ApolloObject obj2 = this.dictObjectCollection[num];
                removeApolloObject(obj2.ObjectId);
            }
            this.dictObjectCollection.Clear();
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            ADebug.Log("ObjectManager OnApplicationPause:" + pauseStatus);
            for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
            {
                this.acceptUpdatedObjectList[i].OnApplicationPause(pauseStatus);
            }
        }

        public void OnApplicationQuit()
        {
            ADebug.Log("ObjectManager OnApplicationQuit");
            for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
            {
                this.acceptUpdatedObjectList[i].OnApplicationQuit();
            }
            this.acceptUpdatedObjectList.Clear();
            this.ClearObjects();
            apollo_quit();
        }

        public void OnDisable()
        {
            ADebug.Log("ObjectManager OnDisable");
            for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
            {
                this.acceptUpdatedObjectList[i].OnDisable();
            }
            this.acceptUpdatedObjectList.Clear();
        }

        [MonoPInvokeCallback(typeof(ApolloSendBufferDelegate))]
        private static void onSendBuffer(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr buffer, int len)
        {
            ApolloObject obj2 = Instance.dictObjectCollection[objectId];
            if ((obj2 != null) && (function != null))
            {
                System.Type type = obj2.GetType();
                System.Type[] types = new System.Type[] { typeof(byte[]) };
                MethodInfo info = type.GetMethod(function, BindingFlags.IgnoreReturn | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                if (info != null)
                {
                    byte[] destination = new byte[len];
                    Marshal.Copy(buffer, destination, 0, len);
                    object[] parameters = new object[] { destination };
                    info.Invoke(obj2, parameters);
                }
                else
                {
                    ADebug.LogError("onSendBuffer not exist method:" + function + " " + type.FullName);
                }
            }
            else
            {
                ADebug.LogError("onSendBuffer:" + objectId + " do not exist");
            }
        }

        [MonoPInvokeCallback(typeof(ApolloSendMessageDelegate))]
        private static void onSendMessage(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string param)
        {
            if (!Instance.dictObjectCollection.ContainsKey(objectId))
            {
                ADebug.LogError(string.Concat(new object[] { "onSendMessage not exist: ", objectId, " function:", function, " param:", param }));
            }
            else
            {
                ApolloObject obj2 = Instance.dictObjectCollection[objectId];
                if ((obj2 != null) && (function != null))
                {
                    System.Type[] types = new System.Type[] { typeof(string) };
                    MethodInfo info = obj2.GetType().GetMethod(function, BindingFlags.IgnoreReturn | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                    if (info != null)
                    {
                        object[] parameters = new object[] { param };
                        info.Invoke(obj2, parameters);
                    }
                    else
                    {
                        ADebug.LogError("onSendMessage not exist method:" + function);
                    }
                }
                else
                {
                    ADebug.Log("onSendMessage:" + objectId + " do not exist");
                }
            }
        }

        [MonoPInvokeCallback(typeof(ApolloSendResultDelegate))]
        private static void onSendResult(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result)
        {
            ApolloObject obj2 = Instance.dictObjectCollection[objectId];
            if ((obj2 != null) && (function != null))
            {
                System.Type type = obj2.GetType();
                System.Type[] types = new System.Type[] { typeof(int) };
                MethodInfo info = type.GetMethod(function, BindingFlags.IgnoreReturn | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                if (info != null)
                {
                    object[] parameters = new object[] { result };
                    info.Invoke(obj2, parameters);
                }
                else
                {
                    ADebug.LogError("onSendResult not exist method:" + function + " " + type.FullName);
                }
            }
            else
            {
                ADebug.LogError("onSendResult:" + objectId + " do not exist");
            }
        }

        [MonoPInvokeCallback(typeof(ApolloSendResultBufferDelegate))]
        private static void onSendResultBuffer(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result, IntPtr buffer, int len)
        {
            ApolloObject obj2 = Instance.dictObjectCollection[objectId];
            if ((obj2 != null) && (function != null))
            {
                System.Type type = obj2.GetType();
                System.Type[] types = new System.Type[] { typeof(int), typeof(byte[]) };
                MethodInfo info = type.GetMethod(function, BindingFlags.IgnoreReturn | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                if (info != null)
                {
                    byte[] destination = new byte[len];
                    if ((buffer != IntPtr.Zero) && (len > 0))
                    {
                        Marshal.Copy(buffer, destination, 0, len);
                    }
                    object[] parameters = new object[] { result, destination };
                    info.Invoke(obj2, parameters);
                }
                else
                {
                    ADebug.LogError("onSendResultBuffer not exist method:" + function + " " + type.FullName);
                }
            }
            else
            {
                ADebug.LogError("onSendResultBuffer:" + objectId + " do not exist");
            }
        }

        [MonoPInvokeCallback(typeof(ApolloSendStructDelegate))]
        private static void onSendStruct(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr param)
        {
            ApolloObject obj2 = Instance.dictObjectCollection[objectId];
            if ((obj2 != null) && (function != null))
            {
                System.Type type = obj2.GetType();
                System.Type[] types = new System.Type[] { typeof(IntPtr) };
                MethodInfo info = type.GetMethod(function, BindingFlags.IgnoreReturn | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, types, null);
                if (info != null)
                {
                    object[] parameters = new object[] { param };
                    info.Invoke(obj2, parameters);
                }
                else
                {
                    ADebug.LogError("onSendStruct not exist method:" + function + " " + type.FullName);
                }
            }
            else
            {
                ADebug.LogError("onSendStruct:" + objectId + " do not exist");
            }
        }

        public void RemoveAcceptUpdatedObject(ApolloObject obj)
        {
            if ((obj != null) && this.acceptUpdatedObjectList.Contains(obj))
            {
                this.acceptUpdatedObjectList.Remove(obj);
            }
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void removeApolloObject(ulong objectId);
        public void RemoveObject(ApolloObject obj)
        {
            if ((obj != null) && this.dictObjectCollection.ContainsKey(obj.ObjectId))
            {
                this.dictObjectCollection.Remove(obj.ObjectId);
                removeApolloObject(obj.ObjectId);
            }
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void setApolloSendBufferCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendBufferDelegate callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void setApolloSendMessageCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendMessageDelegate callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void setApolloSendResultBufferCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendResultBufferDelegate callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void setApolloSendResultCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendResultDelegate callback);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void setApolloSendStructCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendStructDelegate callback);
        public void Update()
        {
            for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
            {
                ApolloObject item = this.acceptUpdatedObjectList[i];
                if (item.Removable)
                {
                    this.removedUpdatableList.Add(item);
                }
                else
                {
                    item.Update();
                }
            }
            for (int j = 0; j < this.removedUpdatableList.Count; j++)
            {
                ApolloObject obj3 = this.removedUpdatableList[j];
                if (obj3 != null)
                {
                    this.RemoveAcceptUpdatedObject(obj3);
                }
            }
            this.removedUpdatableList.Clear();
            DictionaryView<ulong, ApolloObject>.Enumerator enumerator = this.dictObjectCollection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, ApolloObject> current = enumerator.Current;
                ApolloObject obj4 = current.Value;
                if (obj4.Removable)
                {
                    this.removedReflectibleList.Add(obj4);
                }
            }
            for (int k = 0; k < this.removedReflectibleList.Count; k++)
            {
                ApolloObject obj5 = this.removedReflectibleList[k];
                if (obj5 != null)
                {
                    this.RemoveObject(obj5);
                }
            }
            this.removedReflectibleList.Clear();
        }

        public static ApolloObjectManager Instance
        {
            get
            {
                if (!init)
                {
                    init = true;
                    if (container == null)
                    {
                        container = new GameObject();
                        UnityEngine.Object.DontDestroyOnLoad(container);
                    }
                    instance = container.AddComponent(typeof(ApolloObjectManager)) as ApolloObjectManager;
                }
                return instance;
            }
        }
    }
}

