namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Controller : IController
    {
        protected IDictionary<string, System.Type> m_commandMap;
        protected static volatile IController m_instance;
        protected static readonly object m_staticSyncRoot = new object();
        protected readonly object m_syncRoot = new object();
        protected IDictionary<IView, List<string>> m_viewCmdMap;

        protected Controller()
        {
            this.InitializeController();
        }

        public virtual void ExecuteCommand(IMessage note)
        {
            System.Type type = null;
            List<IView> list = null;
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                if (this.m_commandMap.ContainsKey(note.Name))
                {
                    type = this.m_commandMap[note.Name];
                }
                else
                {
                    list = new List<IView>();
                    IEnumerator<KeyValuePair<IView, List<string>>> enumerator = this.m_viewCmdMap.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<IView, List<string>> current = enumerator.Current;
                            if (current.Value.Contains(note.Name))
                            {
                                list.Add(current.Key);
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator == null)
                        {
                        }
                        enumerator.Dispose();
                    }
                }
            }
            if (type != null)
            {
                object obj3 = Activator.CreateInstance(type);
                if (obj3 is ICommand)
                {
                    ((ICommand) obj3).Execute(note);
                }
            }
            if ((list != null) && (list.Count > 0))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].OnMessage(note);
                }
                list = null;
            }
        }

        public virtual bool HasCommand(string commandName)
        {
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                return this.m_commandMap.ContainsKey(commandName);
            }
        }

        protected virtual void InitializeController()
        {
            this.m_commandMap = new Dictionary<string, System.Type>();
            this.m_viewCmdMap = new Dictionary<IView, List<string>>();
        }

        public virtual void RegisterCommand(string commandName, System.Type commandType)
        {
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                this.m_commandMap[commandName] = commandType;
            }
        }

        public virtual void RegisterViewCommand(IView view, string[] commandNames)
        {
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                if (this.m_viewCmdMap.ContainsKey(view))
                {
                    List<string> list = null;
                    if (this.m_viewCmdMap.TryGetValue(view, out list))
                    {
                        for (int i = 0; i < commandNames.Length; i++)
                        {
                            if (!list.Contains(commandNames[i]))
                            {
                                list.Add(commandNames[i]);
                            }
                        }
                    }
                }
                else
                {
                    this.m_viewCmdMap.Add(view, new List<string>(commandNames));
                }
            }
        }

        public virtual void RemoveCommand(string commandName)
        {
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                if (this.m_commandMap.ContainsKey(commandName))
                {
                    this.m_commandMap.Remove(commandName);
                }
            }
        }

        public virtual void RemoveViewCommand(IView view, string[] commandNames)
        {
            object syncRoot = this.m_syncRoot;
            lock (syncRoot)
            {
                if (this.m_viewCmdMap.ContainsKey(view))
                {
                    List<string> list = null;
                    if (this.m_viewCmdMap.TryGetValue(view, out list))
                    {
                        for (int i = 0; i < commandNames.Length; i++)
                        {
                            if (list.Contains(commandNames[i]))
                            {
                                list.Remove(commandNames[i]);
                            }
                        }
                    }
                }
            }
        }

        public static IController Instance
        {
            get
            {
                if (m_instance == null)
                {
                    object staticSyncRoot = m_staticSyncRoot;
                    lock (staticSyncRoot)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new Controller();
                        }
                    }
                }
                return m_instance;
            }
        }
    }
}

