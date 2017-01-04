using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SGameGraphicRaycaster : GraphicRaycaster
{
    [CompilerGenerated]
    private static Comparison<Graphic> <>f__am$cacheC;
    private Canvas canvas_;
    private Vector3[] corners = new Vector3[4];
    private ListView<Item> m_allItems = new ListView<Item>();
    [NonSerialized]
    private List<Graphic> m_RaycastResults = new List<Graphic>();
    private int m_screenHeight;
    private int m_screenWidth;
    private Tile[] m_tiles;
    private int m_tileSizeX;
    private int m_tileSizeY;
    private int raycast_mask = 4;
    public RaycastMode raycastMode = RaycastMode.Sgame;
    private const int TileCount = 4;
    [NonSerialized, HideInInspector]
    public bool tilesDirty;

    private void AddToTileList(Item item)
    {
        int num = item.m_coord.x + (item.m_coord.y * 4);
        for (int i = 0; i < item.m_coord.numX; i++)
        {
            for (int j = 0; j < item.m_coord.numY; j++)
            {
                int index = ((j * 4) + i) + num;
                this.m_tiles[index].items.Add(item);
            }
        }
    }

    private void AppendResultList(ref Ray ray, float hitDistance, List<RaycastResult> resultAppendList, List<Graphic> raycastResults)
    {
        for (int i = 0; i < raycastResults.Count; i++)
        {
            GameObject gameObject = raycastResults[i].gameObject;
            bool flag = true;
            if (base.ignoreReversedGraphics)
            {
                if (this.eventCamera == null)
                {
                    Vector3 rhs = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                    flag = Vector3.Dot(Vector3.forward, rhs) > 0f;
                }
                else
                {
                    Vector3 lhs = (Vector3) (this.eventCamera.transform.rotation * Vector3.forward);
                    Vector3 vector3 = (Vector3) (gameObject.transform.rotation * Vector3.forward);
                    flag = Vector3.Dot(lhs, vector3) > 0f;
                }
            }
            if (flag)
            {
                float num2 = 0f;
                if ((this.eventCamera == null) || (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay))
                {
                    num2 = 0f;
                }
                else
                {
                    num2 = Vector3.Dot(gameObject.transform.forward, gameObject.transform.position - ray.origin) / Vector3.Dot(gameObject.transform.forward, ray.direction);
                    if (num2 < 0f)
                    {
                        continue;
                    }
                }
                if (num2 < hitDistance)
                {
                    RaycastResult item = new RaycastResult {
                        gameObject = gameObject,
                        module = this,
                        distance = num2,
                        index = resultAppendList.Count,
                        depth = raycastResults[i].depth,
                        sortingLayer = this.canvas.sortingLayerID,
                        sortingOrder = this.canvas.sortingOrder
                    };
                    resultAppendList.Add(item);
                }
            }
        }
    }

    private void CalcItemCoord(ref Coord coord, Item item)
    {
        item.m_rectTransform.GetWorldCorners(this.corners);
        int b = 0x7fffffff;
        int num2 = -2147483648;
        int num3 = 0x7fffffff;
        int num4 = -2147483648;
        Camera worldCamera = this.canvas.worldCamera;
        for (int i = 0; i < this.corners.Length; i++)
        {
            Vector3 vector = (Vector3) CUIUtility.WorldToScreenPoint(worldCamera, this.corners[i]);
            b = Mathf.Min((int) vector.x, b);
            num2 = Mathf.Max((int) vector.x, num2);
            num3 = Mathf.Min((int) vector.y, num3);
            num4 = Mathf.Max((int) vector.y, num4);
        }
        coord.x = Mathf.Clamp(b / this.m_tileSizeX, 0, 3);
        coord.numX = (Mathf.Clamp(num2 / this.m_tileSizeX, 0, 3) - coord.x) + 1;
        coord.y = Mathf.Clamp(num3 / this.m_tileSizeY, 0, 3);
        coord.numY = (Mathf.Clamp(num4 / this.m_tileSizeY, 0, 3) - coord.y) + 1;
    }

    private void InitializeAllItems()
    {
        if ((this.raycastMode == RaycastMode.Sgame) || (this.raycastMode == RaycastMode.Sgame_tile))
        {
            this.m_allItems.Clear();
            Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
                {
                    Item item = Item.Create(componentsInChildren[i]);
                    if (item != null)
                    {
                        this.m_allItems.Add(item);
                    }
                }
            }
            if (this.raycastMode == RaycastMode.Sgame_tile)
            {
                this.InitTiles();
                for (int j = 0; j < this.m_allItems.Count; j++)
                {
                    Item item2 = this.m_allItems[j];
                    this.CalcItemCoord(ref item2.m_coord, item2);
                    this.AddToTileList(item2);
                }
            }
        }
    }

    public void InitTiles()
    {
        if (this.m_tiles == null)
        {
            this.m_tiles = new Tile[0x10];
            for (int i = 0; i < this.m_tiles.Length; i++)
            {
                this.m_tiles[i] = new Tile();
            }
            this.m_screenWidth = Screen.width;
            this.m_screenHeight = Screen.height;
            this.m_tileSizeX = this.m_screenWidth / 4;
            this.m_tileSizeY = this.m_screenHeight / 4;
        }
    }

    private bool IsGameObjectHandleInput(GameObject go)
    {
        return (((go.GetComponent<CUIEventScript>() != null) || (go.GetComponent<CUIMiniEventScript>() != null)) || (go.GetComponent<CUIJoystickScript>() != null));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.m_allItems.Clear();
        this.m_RaycastResults.Clear();
        if (this.m_tiles != null)
        {
            for (int i = 0; i < this.m_tiles.Length; i++)
            {
                this.m_tiles[i].items.Clear();
            }
            this.m_tiles = null;
        }
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        switch (this.raycastMode)
        {
            case RaycastMode.Unity:
                base.Raycast(eventData, resultAppendList);
                break;

            case RaycastMode.Sgame:
                this.Raycast2(eventData, resultAppendList, false);
                break;

            case RaycastMode.Sgame_tile:
                this.Raycast2(eventData, resultAppendList, true);
                break;
        }
    }

    private void Raycast2(PointerEventData eventData, List<RaycastResult> resultAppendList, bool useTiles)
    {
        if (this.canvas != null)
        {
            Vector2 vector;
            if (this.eventCamera == null)
            {
                vector = new Vector2(eventData.position.x / ((float) Screen.width), eventData.position.y / ((float) Screen.height));
            }
            else
            {
                vector = this.eventCamera.ScreenToViewportPoint((Vector3) eventData.position);
            }
            if (((vector.x >= 0f) && (vector.x <= 1f)) && ((vector.y >= 0f) && (vector.y <= 1f)))
            {
                float maxValue = float.MaxValue;
                Ray ray = new Ray();
                if (this.eventCamera != null)
                {
                    ray = this.eventCamera.ScreenPointToRay((Vector3) eventData.position);
                }
                this.m_RaycastResults.Clear();
                Vector2 position = eventData.position;
                ListView<Item> items = null;
                if (useTiles && (this.m_tiles != null))
                {
                    int num2 = Mathf.Clamp(((int) position.x) / this.m_tileSizeX, 0, 3);
                    int index = (Mathf.Clamp(((int) position.y) / this.m_tileSizeY, 0, 3) * 4) + num2;
                    items = this.m_tiles[index].items;
                }
                else
                {
                    items = this.m_allItems;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Raycast(this.m_RaycastResults, position, this.eventCamera);
                }
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = (g1, g2) => g2.depth.CompareTo(g1.depth);
                }
                this.m_RaycastResults.Sort(<>f__am$cacheC);
                this.AppendResultList(ref ray, maxValue, resultAppendList, this.m_RaycastResults);
            }
        }
    }

    public void RefreshGameObject(GameObject go)
    {
        if (((this.raycastMode == RaycastMode.Sgame) || (this.raycastMode == RaycastMode.Sgame_tile)) && ((go != null) && (this.m_allItems != null)))
        {
            Image[] componentsInChildren = go.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if (this.IsGameObjectHandleInput(componentsInChildren[i].gameObject))
                {
                    this.RefreshItem(componentsInChildren[i]);
                }
            }
        }
    }

    public void RefreshItem(Image image)
    {
        if ((image != null) && (this.m_allItems != null))
        {
            Item item = null;
            for (int i = 0; i < this.m_allItems.Count; i++)
            {
                if (this.m_allItems[i].m_image == image)
                {
                    item = this.m_allItems[i];
                    break;
                }
            }
            if (item != null)
            {
                if (this.raycastMode == RaycastMode.Sgame_tile)
                {
                    this.RemoveFromTileList(item);
                }
            }
            else
            {
                item = Item.Create(image);
                if (item == null)
                {
                    return;
                }
                this.m_allItems.Add(item);
            }
            if (this.raycastMode == RaycastMode.Sgame_tile)
            {
                this.CalcItemCoord(ref item.m_coord, item);
                this.AddToTileList(item);
            }
        }
    }

    private void RemoveFromTileList(Item item)
    {
        if (item.m_coord.IsValid())
        {
            int num = item.m_coord.x + (item.m_coord.y * 4);
            for (int i = 0; i < item.m_coord.numX; i++)
            {
                for (int j = 0; j < item.m_coord.numY; j++)
                {
                    int index = ((j * 4) + i) + num;
                    this.m_tiles[index].items.Remove(item);
                }
            }
            item.m_coord = Coord.Invalid;
        }
    }

    public void RemoveGameObject(GameObject go)
    {
        if ((go != null) && (this.m_allItems != null))
        {
            Image component = go.GetComponent<Image>();
            if ((component != null) && this.IsGameObjectHandleInput(go))
            {
                this.RemoveItem(component);
            }
        }
    }

    public void RemoveItem(Image image)
    {
        if ((image != null) && (this.m_allItems != null))
        {
            for (int i = 0; i < this.m_allItems.Count; i++)
            {
                if (this.m_allItems[i].m_image == image)
                {
                    if (this.raycastMode == RaycastMode.Sgame_tile)
                    {
                        this.RemoveFromTileList(this.m_allItems[i]);
                    }
                    this.m_allItems.RemoveAt(i);
                    break;
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        this.InitializeAllItems();
    }

    public void UpdateTiles()
    {
        if (this.raycastMode == RaycastMode.Sgame_tile)
        {
            Coord invalid = Coord.Invalid;
            for (int i = 0; i < this.m_allItems.Count; i++)
            {
                Item item = this.m_allItems[i];
                this.CalcItemCoord(ref invalid, item);
                if (!invalid.Equals(ref item.m_coord))
                {
                    this.RemoveFromTileList(item);
                    item.m_coord = invalid;
                    this.AddToTileList(item);
                }
            }
        }
    }

    private void UpdateTiles_Editor()
    {
        if (((this.m_screenWidth != Screen.width) || (this.m_screenHeight != Screen.height)) && ((this.m_tiles != null) && (this.raycastMode == RaycastMode.Sgame_tile)))
        {
            this.m_screenWidth = Screen.width;
            this.m_screenHeight = Screen.height;
            this.m_tileSizeX = this.m_screenWidth / 4;
            this.m_tileSizeY = this.m_screenHeight / 4;
            for (int i = 0; i < this.m_tiles.Length; i++)
            {
                this.m_tiles[i].items.Clear();
            }
            for (int j = 0; j < this.m_allItems.Count; j++)
            {
                Item item = this.m_allItems[j];
                this.CalcItemCoord(ref item.m_coord, item);
                this.AddToTileList(item);
            }
        }
    }

    private Canvas canvas
    {
        get
        {
            if (this.canvas_ == null)
            {
                this.canvas_ = base.GetComponent<Canvas>();
            }
            return this.canvas_;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Coord
    {
        public int x;
        public int y;
        public int numX;
        public int numY;
        public static SGameGraphicRaycaster.Coord Invalid;
        static Coord()
        {
            SGameGraphicRaycaster.Coord coord = new SGameGraphicRaycaster.Coord {
                x = -1,
                y = -1
            };
            Invalid = coord;
        }

        public bool IsValid()
        {
            return ((this.x >= 0) && (this.y >= 0));
        }

        public bool Equals(ref SGameGraphicRaycaster.Coord r)
        {
            return ((((r.x == this.x) && (r.y == this.y)) && (r.numX == this.numX)) && (r.numY == this.numY));
        }
    }

    private class Item
    {
        public SGameGraphicRaycaster.Coord m_coord = SGameGraphicRaycaster.Coord.Invalid;
        public Image m_image;
        public RectTransform m_rectTransform;

        public static SGameGraphicRaycaster.Item Create(Image image)
        {
            if (image == null)
            {
                return null;
            }
            return new SGameGraphicRaycaster.Item { m_image = image, m_rectTransform = image.gameObject.transform as RectTransform };
        }

        public void Raycast(List<Graphic> raycastResults, Vector2 pointerPosition, Camera eventCamera)
        {
            if ((this.m_image.enabled && this.m_rectTransform.gameObject.activeInHierarchy) && RectTransformUtility.RectangleContainsScreenPoint(this.m_rectTransform, pointerPosition, eventCamera))
            {
                raycastResults.Add(this.m_image);
            }
        }
    }

    public enum RaycastMode
    {
        Unity,
        Sgame,
        Sgame_tile
    }

    private class Tile
    {
        public ListView<SGameGraphicRaycaster.Item> items = new ListView<SGameGraphicRaycaster.Item>();
    }
}

