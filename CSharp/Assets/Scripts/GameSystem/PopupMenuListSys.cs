namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class PopupMenuListSys : Singleton<PopupMenuListSys>
    {
        public List<PopupMenuListItem> m_popupMenuList = new List<PopupMenuListItem>();

        public void AddItem(PopupMenuListItem item)
        {
            this.m_popupMenuList.Add(item);
        }

        public void ClearAll()
        {
            this.m_popupMenuList.Clear();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.MENU_PopupMenuFinish, new CUIEventManager.OnUIEventHandler(this.OnPopupMenuFinish));
        }

        public void OnPopupMenuFinish(CUIEvent evt)
        {
            if ((this.m_popupMenuList != null) && (this.m_popupMenuList.Count != 0))
            {
                this.m_popupMenuList.RemoveAt(0);
                this.PopupMenuListStart();
            }
        }

        public void PopupMenuListStart()
        {
            if ((this.m_popupMenuList != null) && (this.m_popupMenuList.Count != 0))
            {
                PopupMenuListItem item = this.m_popupMenuList[0];
                PopupMenuListItem item2 = this.m_popupMenuList[0];
                item.m_show(item2.content);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PopupMenuListItem
        {
            public Show m_show;
            public string content;
            public delegate void Show(string content);
        }
    }
}

