using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIEventListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    public event VoidDelegate onClick;

    public event VoidDelegate onDown;

    public event VoidDelegate onEnter;

    public event VoidDelegate onExit;

    public event VoidDelegate onSelect;

    public event VoidDelegate onUp;

    public static GUIEventListener Get(GameObject go)
    {
        GUIEventListener component = go.GetComponent<GUIEventListener>();
        if (component == null)
        {
            component = go.AddComponent<GUIEventListener>();
        }
        return component;
    }

    public object GetUserData()
    {
        return this.userData;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.onClick != null)
        {
            this.onClick(base.gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.onDown != null)
        {
            this.onDown(base.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.onEnter != null)
        {
            this.onEnter(base.gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.onExit != null)
        {
            this.onExit(base.gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (this.onUp != null)
        {
            this.onUp(base.gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (this.onSelect != null)
        {
            this.onSelect(base.gameObject);
        }
    }

    public void SetUserData(object data)
    {
        this.userData = data;
    }

    public object userData { get; set; }

    public delegate void VoidDelegate(GameObject go);
}

