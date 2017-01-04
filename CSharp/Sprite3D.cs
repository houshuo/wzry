using System;
using UnityEngine;

public class Sprite3D : MonoBehaviour
{
    [SerializeField]
    private EnumHoriontal m_alignHoriontal = EnumHoriontal.Center;
    [SerializeField]
    private EnumVertical m_alignVertical = EnumVertical.Middle;
    [SerializeField]
    private AtlasInfo m_atlas;
    [SerializeField]
    private Color m_color = Color.white;
    [SerializeField]
    private float m_depth = 1f;
    [SerializeField]
    private float m_fillAmount = 1f;
    [SerializeField]
    private EnumFillType m_fillType;
    [SerializeField]
    private float m_height = 1f;
    [NonSerialized]
    private string m_lastAtlasName;
    [NonSerialized]
    private Mesh m_mesh;
    [NonSerialized]
    private bool m_propchanged = true;
    [SerializeField]
    private uint m_segments = 50;
    [SerializeField]
    private string m_spriteName;
    [NonSerialized]
    private AtlasInfo.UVDetail m_uv;
    [SerializeField]
    private float m_width = 1f;
    public static readonly int TRANSPARENT_RENDER_QUEUE = 0xbb8;

    private void Awake()
    {
        this.m_lastAtlasName = null;
        this.m_propchanged = true;
        this.m_depth = Mathf.Max(1f, this.m_depth);
        this.m_mesh = null;
        this.PrepareMesh();
    }

    private void GenerateHoriontalFillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 localPosition = base.transform.localPosition;
            this.m_mesh.MarkDynamic();
            float x = 0f;
            float y = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                x = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                x = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                y = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                y = -this.m_height;
            }
            Vector3[] vectorArray = new Vector3[] { new Vector3(x, y + this.m_height, 0f), new Vector3(x + (this.m_width * this.m_fillAmount), y + this.m_height, 0f), new Vector3(x, y, 0f), new Vector3(x + (this.m_width * this.m_fillAmount), y, 0f) };
            Vector2[] vectorArray2 = new Vector2[] { this.m_uv.uvTL, this.m_uv.uvTL.Lerp(this.m_uv.uvTR, this.m_fillAmount), this.m_uv.uvBL, this.m_uv.uvBL.Lerp(this.m_uv.uvBR, this.m_fillAmount) };
            Color[] colorArray = new Color[] { this.m_color, this.m_color, this.m_color, this.m_color };
            int[] numArray = new int[] { 0, 1, 2, 3, 2, 1 };
            this.m_mesh.vertices = vectorArray;
            this.m_mesh.uv = vectorArray2;
            this.m_mesh.colors = colorArray;
            this.m_mesh.triangles = numArray;
            this.RecaculateDepth();
        }
    }

    public void GenerateMesh()
    {
        this.RefreshUVDetail();
        if (this.m_uv != null)
        {
            this.PrepareMesh();
            this.m_mesh.Clear();
            if (this.m_fillType == EnumFillType.Horiontal)
            {
                this.GenerateHoriontalFillMesh();
            }
            else if (this.m_fillType == EnumFillType.Vertical)
            {
                this.GenerateVerticalFillMesh();
            }
            else if (this.m_fillType == EnumFillType.Radial360)
            {
                this.GenerateRadial360FillMesh();
            }
        }
    }

    private void GenerateRadial360FillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 localPosition = base.transform.localPosition;
            this.m_mesh.MarkDynamic();
            float x = 0f;
            float y = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                x = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                x = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                y = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                y = -this.m_height;
            }
            int num3 = ((int) (this.m_segments * this.m_fillAmount)) + 1;
            float num4 = (2f * (this.width + this.height)) / ((float) this.m_segments);
            Vector3[] vectorArray = new Vector3[num3 + 1];
            Vector2[] vectorArray2 = new Vector2[num3 + 1];
            vectorArray[0] = new Vector3(x + (0.5f * this.m_width), y + (0.5f * this.height), 0f);
            vectorArray2[0] = this.m_uv.uvTL.Lerp(this.m_uv.uvBR, 0.5f);
            int num5 = 0;
            int num6 = 0;
            float num7 = 0f;
            for (int i = 0; i < num3; i++)
            {
                switch (num5)
                {
                    case 0:
                    {
                        float num9 = (x + (0.5f * this.width)) + (num6 * num4);
                        if (num9 >= (x + this.width))
                        {
                            num7 = (num9 - x) - this.width;
                            num9 = x + this.width;
                            num5 = 1;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num9, y + this.height, 0f);
                        break;
                    }
                    case 1:
                    {
                        float num10 = ((y + this.height) - (num6 * num4)) - num7;
                        if (num10 <= y)
                        {
                            num7 = y - num10;
                            num10 = y;
                            num5 = 2;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(x + this.width, num10, 0f);
                        break;
                    }
                    case 2:
                    {
                        float num11 = ((x + this.width) - (num6 * num4)) - num7;
                        if (num11 <= x)
                        {
                            num7 = x - num11;
                            num11 = x;
                            num5 = 3;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(num11, y, 0f);
                        break;
                    }
                    case 3:
                    {
                        float num12 = (y + (num6 * num4)) + num7;
                        if (num12 >= (y + this.height))
                        {
                            num7 = (num12 - y) - this.height;
                            num12 = y + this.height;
                            num5 = 4;
                            num6 = 1;
                        }
                        else
                        {
                            num6++;
                        }
                        vectorArray[i + 1] = new Vector3(x, num12);
                        break;
                    }
                    case 4:
                    {
                        float num13 = (x + (num6 * num4)) + num7;
                        if (num13 > (x + (0.5f * this.width)))
                        {
                            num13 = x + (0.5f * this.width);
                        }
                        num6++;
                        vectorArray[i + 1] = new Vector3(num13, y + this.height, 0f);
                        break;
                    }
                }
                float num14 = vectorArray[i + 1].x;
                float num15 = vectorArray[i + 1].y;
                vectorArray2[i + 1] = new Vector2(Mathf.Lerp(this.m_uv.uvTL.x, this.m_uv.uvTR.x, (num14 - x) / this.width), Mathf.Lerp(this.m_uv.uvBL.y, this.m_uv.uvTL.y, (num15 - y) / this.height));
            }
            Color[] colorArray = new Color[num3 + 1];
            for (int j = 0; j < colorArray.Length; j++)
            {
                colorArray[j] = this.m_color;
            }
            int[] numArray = new int[(num3 - 1) * 3];
            for (int k = 0; k < (num3 - 1); k++)
            {
                numArray[k * 3] = 0;
                numArray[(k * 3) + 1] = k + 1;
                numArray[(k * 3) + 2] = k + 2;
            }
            this.m_mesh.vertices = vectorArray;
            this.m_mesh.uv = vectorArray2;
            this.m_mesh.colors = colorArray;
            this.m_mesh.triangles = numArray;
            this.RecaculateDepth();
        }
    }

    private void GenerateVerticalFillMesh()
    {
        this.m_fillAmount = Mathf.Clamp01(this.m_fillAmount);
        if (this.m_fillAmount > 0f)
        {
            Vector3 localPosition = base.transform.localPosition;
            this.m_mesh.MarkDynamic();
            float x = 0f;
            float y = 0f;
            if (this.m_alignHoriontal == EnumHoriontal.Center)
            {
                x = -0.5f * this.m_width;
            }
            else if (this.m_alignHoriontal == EnumHoriontal.Right)
            {
                x = -this.m_width;
            }
            if (this.m_alignVertical == EnumVertical.Middle)
            {
                y = -0.5f * this.m_height;
            }
            else if (this.m_alignVertical == EnumVertical.Top)
            {
                y = -this.m_height;
            }
            Vector3[] vectorArray = new Vector3[] { new Vector3(x, y + (this.m_height * this.m_fillAmount), 0f), new Vector3(x + this.m_width, y + (this.m_height * this.m_fillAmount), 0f), new Vector3(x, y, 0f), new Vector3(x + this.m_width, y, 0f) };
            Vector2[] vectorArray2 = new Vector2[] { this.m_uv.uvBL.Lerp(this.m_uv.uvTL, this.m_fillAmount), this.m_uv.uvBR.Lerp(this.m_uv.uvTR, this.m_fillAmount), this.m_uv.uvBL, this.m_uv.uvBR };
            Color[] colorArray = new Color[] { this.m_color, this.m_color, this.m_color, this.m_color };
            int[] numArray = new int[] { 0, 1, 2, 3, 2, 1 };
            this.m_mesh.vertices = vectorArray;
            this.m_mesh.uv = vectorArray2;
            this.m_mesh.colors = colorArray;
            this.m_mesh.triangles = numArray;
            this.RecaculateDepth();
        }
    }

    private void LateUpdate()
    {
        if (this.m_propchanged)
        {
            this.GenerateMesh();
            this.m_propchanged = false;
        }
    }

    private void PrepareMesh()
    {
        if (this.m_mesh == null)
        {
            MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
            if (null == component)
            {
                component = base.gameObject.AddComponent<MeshFilter>();
            }
            this.m_mesh = new Mesh();
            component.mesh = this.m_mesh;
            MeshRenderer renderer = base.gameObject.GetComponent<MeshRenderer>();
            if (null == renderer)
            {
                renderer = base.gameObject.AddComponent<MeshRenderer>();
            }
            renderer.sharedMaterial = this.m_atlas.material;
        }
    }

    private void RecaculateDepth()
    {
        Bounds bounds = new Bounds {
            center = new Vector3(0.5f * this.m_width, 0.5f * this.m_height, this.m_depth / 4f),
            size = new Vector3(this.m_width, this.m_height, (this.m_depth / 4f) * 2f)
        };
        this.m_mesh.bounds = bounds;
    }

    private void RefreshUVDetail()
    {
        if ((null != this.m_atlas) && (this.m_lastAtlasName != this.m_spriteName))
        {
            this.m_uv = this.m_atlas.GetUV(this.m_spriteName);
            this.m_lastAtlasName = this.m_spriteName;
        }
    }

    public void SetNativeSize(Camera camera, float depth)
    {
        int num = 0x500;
        int num2 = Mathf.Max(Screen.width, Screen.height);
        float num3 = Mathf.Max((float) 1f, (float) (((float) num2) / ((float) num)));
        this.RefreshUVDetail();
        if (camera != null)
        {
            Vector3 position = camera.transform.TransformPoint(0f, 0f, depth);
            Vector3 vector2 = camera.WorldToScreenPoint(position);
            Vector3 vector3 = new Vector3(vector2.x + (this.m_uv.width * num3), vector2.y, vector2.z);
            Vector3 vector4 = new Vector3(vector2.x, vector2.y + (this.m_uv.height * num3), vector2.z);
            Vector3 b = camera.ScreenToWorldPoint(vector3);
            Vector3 vector6 = camera.ScreenToWorldPoint(vector4);
            this.m_width = Vector3.Distance(position, b);
            this.m_height = Vector3.Distance(position, vector6);
        }
    }

    private void Update()
    {
    }

    public EnumHoriontal alignHoriontal
    {
        get
        {
            return this.m_alignHoriontal;
        }
        set
        {
            if (this.m_alignHoriontal != value)
            {
                this.m_alignHoriontal = value;
                this.m_propchanged = true;
            }
        }
    }

    public EnumVertical alignVertical
    {
        get
        {
            return this.m_alignVertical;
        }
        set
        {
            if (this.m_alignVertical != value)
            {
                this.m_alignVertical = value;
                this.m_propchanged = true;
            }
        }
    }

    public AtlasInfo atlas
    {
        get
        {
            return this.m_atlas;
        }
        set
        {
            if (this.m_atlas != value)
            {
                this.m_atlas = value;
                this.m_propchanged = true;
            }
        }
    }

    public Color color
    {
        get
        {
            return this.m_color;
        }
        set
        {
            if (this.m_color != value)
            {
                this.m_color = value;
                this.m_propchanged = true;
            }
        }
    }

    public float depth
    {
        get
        {
            return this.m_depth;
        }
        set
        {
            this.m_depth = value;
            this.RecaculateDepth();
        }
    }

    public float fillAmount
    {
        get
        {
            return this.m_fillAmount;
        }
        set
        {
            if (this.m_fillAmount != value)
            {
                this.m_fillAmount = value;
                this.m_propchanged = true;
            }
        }
    }

    public EnumFillType fillType
    {
        get
        {
            return this.m_fillType;
        }
        set
        {
            if (this.m_fillType != value)
            {
                this.m_fillType = value;
                this.m_propchanged = true;
            }
        }
    }

    public float height
    {
        get
        {
            return this.m_height;
        }
        set
        {
            if (this.m_height != value)
            {
                this.m_height = value;
                this.m_propchanged = true;
            }
        }
    }

    public uint segments
    {
        get
        {
            return this.m_segments;
        }
        set
        {
            if (this.m_segments != value)
            {
                this.m_segments = value;
                this.m_propchanged = true;
            }
        }
    }

    public string spriteName
    {
        get
        {
            return this.m_spriteName;
        }
        set
        {
            if (this.m_spriteName != value)
            {
                this.m_spriteName = value;
                this.m_propchanged = true;
            }
        }
    }

    public float width
    {
        get
        {
            return this.m_width;
        }
        set
        {
            if (this.m_width != value)
            {
                this.m_width = value;
                this.m_propchanged = true;
            }
        }
    }

    [Serializable]
    public enum EnumFillType
    {
        Horiontal,
        Vertical,
        Radial360
    }

    [Serializable]
    public enum EnumHoriontal
    {
        Left,
        Center,
        Right
    }

    [Serializable]
    public enum EnumVertical
    {
        Top,
        Middle,
        Bottom
    }
}

