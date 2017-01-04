namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class Lobby3DInteraction
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map4;
        public const float CAMERA_MAX = 7.5f;
        public const float CAMERA_MIN = 0.8f;
        private int InteractLayerMask = (((int) 1) << LayerMask.NameToLayer("Lobby_Interactable"));
        private Transform mouseDownObj;
        private const string NAME_ARENA = "Arena";
        private const string NAME_LOTTERY = "Lottery";
        private const string NAME_PROBE = "Probe";
        private const string NAME_PVE = "PVE";
        private const string NAME_PVP = "PVP";
        private const string NAME_SHOP = "Shop";
        private const string NAME_SOCIAL = "Social";
        public static string PATH_3DINTERACTION_FORM = "UGUI/Form/System/Lobby/Form_LobbyInteractable.prefab";
        private CUIEvent uiEvt;

        public void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
            this.uiEvt = new CUIEvent();
        }

        private void onDragEnd(CUIEvent uiEvent)
        {
        }

        private void onDragging(CUIEvent uiEvent)
        {
            Transform transform = Camera.main.transform;
            transform.position = new Vector3(transform.position.x - (uiEvent.m_pointerEventData.delta.x / 100f), transform.position.y, transform.position.z);
            if (transform.position.x < 0.8f)
            {
                transform.position = new Vector3(0.8f, transform.position.y, transform.position.z);
            }
            if (transform.position.x > 7.5f)
            {
                transform.position = new Vector3(7.5f, transform.position.y, transform.position.z);
            }
        }

        private void onDragStart(CUIEvent uiEvent)
        {
        }

        private void onMouseClick(CUIEvent uiEvent)
        {
            RaycastHit hit;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay((Vector3) uiEvent.m_pointerEventData.position), out hit, float.PositiveInfinity, this.InteractLayerMask))
            {
                return;
            }
            string name = hit.collider.gameObject.transform.gameObject.name;
            if (name != null)
            {
                int num;
                if (<>f__switch$map4 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
                    dictionary.Add("PVP", 0);
                    dictionary.Add("PVE", 1);
                    dictionary.Add("Probe", 2);
                    dictionary.Add("Lottery", 3);
                    dictionary.Add("Arena", 4);
                    dictionary.Add("Social", 5);
                    dictionary.Add("Shop", 6);
                    <>f__switch$map4 = dictionary;
                }
                if (<>f__switch$map4.TryGetValue(name, out num))
                {
                    switch (num)
                    {
                        case 0:
                            this.uiEvt.m_eventID = enUIEventID.Matching_OpenEntry;
                            goto Label_01AE;

                        case 1:
                            this.uiEvt.m_eventID = enUIEventID.Adv_OpenChapterForm;
                            goto Label_01AE;

                        case 2:
                            this.uiEvt.m_eventID = enUIEventID.Explore_OpenForm;
                            goto Label_01AE;

                        case 3:
                            this.uiEvt.m_eventID = enUIEventID.Lottery_OpenForm;
                            goto Label_01AE;

                        case 4:
                            this.uiEvt.m_eventID = enUIEventID.Arena_OpenForm;
                            goto Label_01AE;

                        case 5:
                            this.uiEvt.m_eventID = enUIEventID.Guild_OpenForm;
                            goto Label_01AE;

                        case 6:
                            this.uiEvt.m_eventID = enUIEventID.Shop_OpenForm;
                            goto Label_01AE;
                    }
                }
            }
            this.uiEvt.m_eventID = enUIEventID.None;
        Label_01AE:
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(this.uiEvt);
        }

        private void onMouseDown(CUIEvent uiEvent)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay((Vector3) uiEvent.m_pointerEventData.position), out hit, float.PositiveInfinity, this.InteractLayerMask))
            {
                this.mouseDownObj = hit.collider.gameObject.transform;
                this.mouseDownObj.position = new Vector3(this.mouseDownObj.position.x, this.mouseDownObj.position.y + 0.2f, this.mouseDownObj.position.z);
            }
        }

        private void onMouseUp(CUIEvent uiEvent)
        {
            if (this.mouseDownObj != null)
            {
                this.mouseDownObj.position = new Vector3(this.mouseDownObj.position.x, this.mouseDownObj.position.y - 0.2f, this.mouseDownObj.position.z);
                this.mouseDownObj = null;
            }
        }

        public void OpenForm()
        {
            Singleton<CUIManager>.GetInstance().OpenForm(PATH_3DINTERACTION_FORM, false, true);
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseDown, new CUIEventManager.OnUIEventHandler(this.onMouseDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseUp, new CUIEventManager.OnUIEventHandler(this.onMouseUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnMouseClick, new CUIEventManager.OnUIEventHandler(this.onMouseClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragStart, new CUIEventManager.OnUIEventHandler(this.onDragStart));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragging, new CUIEventManager.OnUIEventHandler(this.onDragging));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OnDragEnd, new CUIEventManager.OnUIEventHandler(this.onDragEnd));
            this.uiEvt = null;
        }
    }
}

