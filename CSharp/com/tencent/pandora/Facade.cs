namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Facade
    {
        protected IController m_controller;
        private static GameObject m_GameManager;
        private static Dictionary<string, object> m_Managers = new Dictionary<string, object>();

        protected Facade()
        {
            this.InitFramework();
        }

        public T AddManager<T>(string typeName) where T: Component
        {
            object obj2 = null;
            m_Managers.TryGetValue(typeName, out obj2);
            if (obj2 != null)
            {
                return (T) obj2;
            }
            Component component = this.AppGameManager.AddComponent<T>();
            m_Managers.Add(typeName, component);
            return null;
        }

        public void AddManager(string typeName, object obj)
        {
            if (!m_Managers.ContainsKey(typeName))
            {
                m_Managers.Add(typeName, obj);
            }
        }

        public T GetManager<T>(string typeName) where T: class
        {
            if (!m_Managers.ContainsKey(typeName))
            {
                return null;
            }
            object obj2 = null;
            m_Managers.TryGetValue(typeName, out obj2);
            return (T) obj2;
        }

        public virtual bool HasCommand(string commandName)
        {
            return this.m_controller.HasCommand(commandName);
        }

        protected virtual void InitFramework()
        {
            if (this.m_controller == null)
            {
                this.m_controller = Controller.Instance;
                com.tencent.pandora.Logger.d("  Controller.Instance;");
            }
        }

        public virtual void RegisterCommand(string commandName, System.Type commandType)
        {
            this.m_controller.RegisterCommand(commandName, commandType);
            com.tencent.pandora.Logger.d("RegisterCommand(commandName, commandType);");
        }

        public void RegisterMultiCommand(System.Type commandType, params string[] commandNames)
        {
            int length = commandNames.Length;
            for (int i = 0; i < length; i++)
            {
                this.RegisterCommand(commandNames[i], commandType);
            }
        }

        public virtual void RemoveCommand(string commandName)
        {
            this.m_controller.RemoveCommand(commandName);
        }

        public void RemoveManager(string typeName)
        {
            if (m_Managers.ContainsKey(typeName))
            {
                object obj2 = null;
                m_Managers.TryGetValue(typeName, out obj2);
                if (obj2.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    UnityEngine.Object.Destroy((Component) obj2);
                }
                m_Managers.Remove(typeName);
            }
        }

        public void RemoveMultiCommand(params string[] commandName)
        {
            int length = commandName.Length;
            for (int i = 0; i < length; i++)
            {
                this.RemoveCommand(commandName[i]);
            }
        }

        public void SendMessageCommand(string message, object body = null)
        {
            com.tencent.pandora.Logger.d("  send message :" + message);
            this.m_controller.ExecuteCommand(new Message(message, body));
            com.tencent.pandora.Logger.d("execute command finished " + message);
        }

        private GameObject AppGameManager
        {
            get
            {
                if (m_GameManager == null)
                {
                    m_GameManager = GameObject.Find("luaObj");
                }
                return m_GameManager;
            }
        }
    }
}

