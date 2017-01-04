namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class View : MonoBehaviour, IView
    {
        private AppFacade m_Facade;
        private GameManager m_GameMgr;
        private LuaScriptMgr m_LuaMgr;
        private PanelManager m_PanelMgr;
        private ResourceManager m_ResMgr;
        private TimerManager m_TimerMgr;

        public virtual void OnMessage(IMessage message)
        {
        }

        protected void RegisterMessage(IView view, List<string> messages)
        {
            if ((messages != null) && (messages.Count != 0))
            {
                Controller.Instance.RegisterViewCommand(view, messages.ToArray());
            }
        }

        protected void RemoveMessage(IView view, List<string> messages)
        {
            if ((messages != null) && (messages.Count != 0))
            {
                Controller.Instance.RemoveViewCommand(view, messages.ToArray());
            }
        }

        protected AppFacade facade
        {
            get
            {
                if (this.m_Facade == null)
                {
                    this.m_Facade = AppFacade.Instance;
                }
                return this.m_Facade;
            }
        }

        protected GameManager GameMgr
        {
            get
            {
                if (this.m_GameMgr == null)
                {
                    this.m_GameMgr = this.facade.GetManager<GameManager>("GameManager");
                }
                return this.m_GameMgr;
            }
        }

        protected LuaScriptMgr LuaManager
        {
            get
            {
                if (this.m_LuaMgr == null)
                {
                    this.m_LuaMgr = this.facade.GetManager<LuaScriptMgr>("LuaScriptMgr");
                }
                return this.m_LuaMgr;
            }
            set
            {
                this.m_LuaMgr = value;
            }
        }

        protected PanelManager PanelMgr
        {
            get
            {
                if (this.m_PanelMgr == null)
                {
                    this.m_PanelMgr = this.facade.GetManager<PanelManager>("PanelManager");
                }
                return this.m_PanelMgr;
            }
        }

        protected ResourceManager ResManager
        {
            get
            {
                if (this.m_ResMgr == null)
                {
                    this.m_ResMgr = this.facade.GetManager<ResourceManager>("ResourceManager");
                }
                return this.m_ResMgr;
            }
        }

        protected TimerManager TimerManger
        {
            get
            {
                if (this.m_TimerMgr == null)
                {
                    this.m_TimerMgr = this.facade.GetManager<TimerManager>("TimeManager");
                }
                return this.m_TimerMgr;
            }
        }
    }
}

