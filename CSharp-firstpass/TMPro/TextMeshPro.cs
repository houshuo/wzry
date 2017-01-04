namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;

    [ExecuteInEditMode, AddComponentMenu("Mesh/TextMesh Pro"), RequireComponent(typeof(TextContainer)), RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class TextMeshPro : MonoBehaviour
    {
        [SerializeField]
        private bool checkPaddingRequired;
        [SerializeField]
        private bool hasFontAssetChanged;
        [SerializeField]
        private bool haveColorChanged;
        [SerializeField]
        private bool haveFastScaleChanged;
        [SerializeField]
        private bool havePropertiesChanged;
        private bool isDebugOutputDone;
        [SerializeField]
        private bool isInputParsingRequired;
        private bool isMaskUpdateRequired;
        private Vector2 k_InfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        private readonly float[] k_Power = new float[] { 0.5f, 0.05f, 0.005f, 0.0005f, 5E-05f, 5E-06f, 5E-07f, 5E-08f, 5E-09f, 5E-10f };
        private int loopCountA;
        private int loopCountB;
        private int loopCountC;
        private int loopCountD;
        private int loopCountE;
        private Vector4 m_alignmentPadding;
        [SerializeField]
        private TMP_Compatibility.AnchorPositions m_anchor;
        private bool m_anchorDampening;
        private Vector3 m_anchorOffset;
        private float m_baseDampeningWidth;
        private float m_baselineOffset;
        private GlyphInfo m_cached_GlyphInfo;
        private GlyphInfo m_cached_Underline_GlyphInfo;
        private int[] m_char_buffer;
        private int m_characterCount;
        [SerializeField]
        private float m_characterSpacing;
        private int m_charArray_Length;
        [SerializeField]
        private float m_charSpacingMax;
        private Color32[] m_colorStack = new Color32[0x20];
        private int m_colorStackIndex;
        private float m_cSpacing;
        private float m_currentFontSize;
        private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 0f));
        [SerializeField]
        private bool m_enableAutoSizing;
        [SerializeField]
        private bool m_enableExtraPadding;
        [SerializeField]
        private bool m_enableKerning;
        [SerializeField]
        private bool m_enableVertexGradient;
        [SerializeField]
        private bool m_enableWordWrapping;
        private Matrix4x4 m_EnvMapMatrix = new Matrix4x4();
        [SerializeField]
        private Color32 m_faceColor = Color.white;
        [SerializeField]
        private Vector3 m_fastScale = Vector3.one;
        private Vector3 m_fastScalePreviews = Vector3.one;
        private int m_firstVisibleCharacterOfLine;
        [SerializeField]
        private TextMeshProFont m_fontAsset;
        [SerializeField]
        private TextMeshProFont[] m_fontAssetArray;
        [SerializeField]
        private Color m_fontColor = Color.white;
        [SerializeField, FormerlySerializedAs("m_fontColor")]
        private Color32 m_fontColor32 = Color.white;
        [SerializeField]
        private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);
        private Color m_fontColorPreviews = Color.white;
        private int m_fontIndex;
        private Material m_fontMaterial;
        private float m_fontScale;
        [SerializeField]
        private float m_fontSize = 36f;
        [SerializeField]
        private float m_fontSizeBase = 36f;
        [SerializeField]
        private float m_fontSizeMax;
        [SerializeField]
        private float m_fontSizeMin;
        [SerializeField]
        private FontStyles m_fontStyle;
        [SerializeField]
        private TextureMappingOptions m_horizontalMapping;
        private Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);
        private char[] m_htmlTag = new char[0x80];
        [SerializeField]
        private bool m_ignoreCulling = true;
        private float m_indent;
        private char[] m_input_CharArray = new char[0x100];
        [SerializeField]
        private TextInputSources m_inputSource;
        private bool m_isCharacterWrappingEnabled;
        [SerializeField]
        private bool m_isCullingEnabled;
        private bool m_isFirstAllocation;
        private bool m_isMaskingEnabled;
        private bool m_isMaterialBlockSet;
        private bool m_isNewPage;
        [SerializeField]
        private bool m_isNewTextObject;
        [SerializeField]
        private bool m_isOrthographic;
        [SerializeField]
        private bool m_isOverlay;
        private bool m_isRecalculateScaleRequired;
        [SerializeField]
        private bool m_isRichText = true;
        private bool m_isTextTruncated;
        private bool m_isUsingBold;
        private int m_lastVisibleCharacterOfLine;
        private float m_lineHeight;
        private TextAlignmentOptions m_lineJustification;
        [SerializeField]
        private float m_lineLength;
        private int m_lineNumber;
        private float m_lineOffset;
        [SerializeField]
        private float m_lineSpacing;
        private float m_lineSpacingDelta;
        [SerializeField]
        private float m_lineSpacingMax;
        private float m_marginWidth;
        private MaterialPropertyBlock m_maskingPropertyBlock;
        [SerializeField]
        private MaskingTypes m_maskType;
        private int m_max_characters = 8;
        private int m_max_numberOfLines = 4;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        private float m_maxFontSize;
        private int m_maxVisibleCharacters = -1;
        private int m_maxVisibleLines = -1;
        private float m_maxXAdvance;
        private Mesh m_mesh;
        private Extents m_meshExtents;
        private MeshFilter m_meshFilter;
        private float m_minFontSize;
        private float m_monoSpacing;
        private Vector3[] m_normals;
        [SerializeField]
        private Color32 m_outlineColor = Color.black;
        private float m_outlineWidth;
        [SerializeField]
        private TextOverflowModes m_overflowMode;
        [SerializeField]
        private bool m_overrideHtmlColors;
        private float m_padding;
        private int m_pageNumber;
        [SerializeField]
        private int m_pageToDisplay;
        [SerializeField]
        private float m_paragraphSpacing;
        private GameObject m_prefabParent;
        private float m_preferredHeight;
        private float m_preferredWidth;
        private Vector3 m_previousLossyScale;
        private int m_recursiveCount;
        [SerializeField]
        private Renderer m_renderer;
        private TextRenderFlags m_renderMode;
        private WordWrapState m_SavedLineState = new WordWrapState();
        private WordWrapState m_SavedWordWrapState = new WordWrapState();
        private int m_selectedFontAsset;
        private Material m_sharedMaterial;
        private List<Material> m_sharedMaterials = new List<Material>(0x10);
        private int m_sortingLayerID;
        private Vector2[] m_spacePositions = new Vector2[8];
        private float m_spacing;
        private Stopwatch m_StopWatch;
        private FontStyles m_style;
        private float m_tabSpacing;
        private Vector4[] m_tangents;
        [SerializeField]
        private string m_text;
        [SerializeField, FormerlySerializedAs("m_lineJustification")]
        private TextAlignmentOptions m_textAlignment;
        private TextContainer m_textContainer;
        private TMP_TextInfo m_textInfo;
        [SerializeField]
        private Vector4 m_textRectangle;
        private Transform m_transform;
        private int[] m_triangles;
        private Vector2[] m_uv2s;
        [SerializeField]
        private float m_uvLineOffset;
        [SerializeField]
        private Vector2 m_uvOffset = Vector2.zero;
        private Vector2[] m_uvs;
        private Color32[] m_vertColors;
        [SerializeField]
        private TextureMappingOptions m_verticalMapping;
        private Vector3[] m_vertices;
        private int m_visibleCharacterCount;
        private List<char> m_VisibleCharacters = new List<char>();
        [SerializeField]
        private float m_wordWrappingRatios = 0.4f;
        private float m_xAdvance;
        private float old_arg0;
        private float old_arg1;
        private float old_arg2;
        private string old_text;

        private void AddFloatToCharArray(float number, ref int index, int precision)
        {
            if (number < 0f)
            {
                this.m_input_CharArray[index++] = '-';
                number = -number;
            }
            number += this.k_Power[Mathf.Min(9, precision)];
            int num = (int) number;
            this.AddIntToCharArray(num, ref index, precision);
            if (precision > 0)
            {
                this.m_input_CharArray[index++] = '.';
                number -= num;
                for (int i = 0; i < precision; i++)
                {
                    number *= 10f;
                    int num3 = (int) number;
                    this.m_input_CharArray[index++] = (char) (num3 + 0x30);
                    number -= num3;
                }
            }
        }

        private void AddIntToCharArray(int number, ref int index, int precision)
        {
            if (number < 0)
            {
                this.m_input_CharArray[index++] = '-';
                number = -number;
            }
            int num = index;
            do
            {
                this.m_input_CharArray[num++] = (char) ((number % 10) + 0x30);
                number /= 10;
            }
            while (number > 0);
            int num2 = num;
            while ((index + 1) < num)
            {
                num--;
                char ch = this.m_input_CharArray[index];
                this.m_input_CharArray[index] = this.m_input_CharArray[num];
                this.m_input_CharArray[num] = ch;
                index++;
            }
            index = num2;
        }

        private void AdjustLineOffset(int startIndex, int endIndex, float offset)
        {
            Vector3 vector = new Vector3(0f, offset, 0f);
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (this.m_textInfo.characterInfo[i].isVisible)
                {
                    int vertexIndex = this.m_textInfo.characterInfo[i].vertexIndex;
                    this.m_textInfo.characterInfo[i].bottomLeft -= vector;
                    this.m_textInfo.characterInfo[i].topRight -= vector;
                    this.m_textInfo.characterInfo[i].bottomLine -= vector.y;
                    this.m_textInfo.characterInfo[i].topLine -= vector.y;
                    this.m_vertices[0 + vertexIndex] -= vector;
                    this.m_vertices[1 + vertexIndex] -= vector;
                    this.m_vertices[2 + vertexIndex] -= vector;
                    this.m_vertices[3 + vertexIndex] -= vector;
                }
            }
        }

        private void Awake()
        {
            if ((this.m_fontColor == Color.white) && (this.m_fontColor32 != Color.white))
            {
                Debug.LogWarning("Converting Vertex Colors from Color32 to Color.");
                this.m_fontColor = (Color) this.m_fontColor32;
            }
            this.m_textContainer = base.GetComponent<TextContainer>();
            if (this.m_textContainer == null)
            {
                this.m_textContainer = base.gameObject.AddComponent<TextContainer>();
            }
            this.m_renderer = base.GetComponent<Renderer>();
            if (this.m_renderer == null)
            {
                this.m_renderer = base.gameObject.AddComponent<Renderer>();
            }
            this.m_transform = base.gameObject.transform;
            this.m_meshFilter = base.GetComponent<MeshFilter>();
            if (this.m_meshFilter == null)
            {
                this.m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
            }
            if (this.m_mesh == null)
            {
                this.m_mesh = new Mesh();
                this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
                this.m_meshFilter.mesh = this.m_mesh;
            }
            this.m_meshFilter.hideFlags = HideFlags.HideInInspector;
            this.LoadFontAsset();
            this.m_char_buffer = new int[this.m_max_characters];
            this.m_cached_GlyphInfo = new GlyphInfo();
            this.m_vertices = new Vector3[0];
            this.m_isFirstAllocation = true;
            this.m_textInfo = new TMP_TextInfo();
            this.m_textInfo.wordInfo = new List<TMP_WordInfo>();
            this.m_textInfo.lineInfo = new TMP_LineInfo[this.m_max_numberOfLines];
            this.m_textInfo.pageInfo = new TMP_PageInfo[0x10];
            this.m_textInfo.meshInfo = new TMP_MeshInfo();
            this.m_fontAssetArray = new TextMeshProFont[0x10];
            if (this.m_fontAsset == null)
            {
                Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.");
            }
            else
            {
                if (this.m_fontSizeMin == 0f)
                {
                    this.m_fontSizeMin = this.m_fontSize / 2f;
                }
                if (this.m_fontSizeMax == 0f)
                {
                    this.m_fontSizeMax = this.m_fontSize * 2f;
                }
                this.isInputParsingRequired = true;
                this.havePropertiesChanged = true;
                this.haveColorChanged = false;
                if (this.m_fastScale != Vector3.one)
                {
                    this.haveFastScaleChanged = true;
                }
                else
                {
                    this.haveFastScaleChanged = false;
                }
                this.ForceMeshUpdate();
            }
        }

        private float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
        {
            float num = 0f;
            float num2 = 1f;
            decimalPointIndex = (decimalPointIndex <= 0) ? (endIndex + 1) : decimalPointIndex;
            if (chars[startIndex] == '-')
            {
                startIndex++;
                num2 = -1f;
            }
            if ((chars[startIndex] == '+') || (chars[startIndex] == '%'))
            {
                startIndex++;
            }
            for (int i = startIndex; i < (endIndex + 1); i++)
            {
                int num4 = decimalPointIndex - i;
                switch ((num4 + 3))
                {
                    case 0:
                        num += (chars[i] - '0') * 0.001f;
                        break;

                    case 1:
                        num += (chars[i] - '0') * 0.01f;
                        break;

                    case 2:
                        num += (chars[i] - '0') * 0.1f;
                        break;

                    case 4:
                        num += chars[i] - '0';
                        break;

                    case 5:
                        num += (chars[i] - '0') * 10;
                        break;

                    case 6:
                        num += (chars[i] - '0') * 100;
                        break;

                    case 7:
                        num += (chars[i] - '0') * 0x3e8;
                        break;
                }
            }
            return (num * num2);
        }

        private void CreateMaterialInstance()
        {
            Material material;
            material = new Material(this.m_sharedMaterial) {
                shaderKeywords = this.m_sharedMaterial.shaderKeywords,
                name = material.name + " Instance"
            };
            this.m_fontMaterial = material;
        }

        private void DisableMasking()
        {
            if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                this.m_sharedMaterial.EnableKeyword("MASK_OFF");
                this.m_sharedMaterial.DisableKeyword("MASK_HARD");
                this.m_sharedMaterial.DisableKeyword("MASK_SOFT");
                this.m_isMaskingEnabled = false;
                this.UpdateMask();
            }
        }

        private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
        {
            int num = index + 12;
            if (num > this.m_vertices.Length)
            {
                this.ResizeMeshBuffers((num / 4) + 12);
            }
            start.y = Mathf.Min(start.y, end.y);
            end.y = Mathf.Min(start.y, end.y);
            float x = (this.m_cached_Underline_GlyphInfo.width / 2f) * this.m_fontScale;
            if ((end.x - start.x) < (this.m_cached_Underline_GlyphInfo.width * this.m_fontScale))
            {
                x = (end.x - start.x) / 2f;
            }
            float height = this.m_cached_Underline_GlyphInfo.height;
            this.m_vertices[index] = start + new Vector3(0f, 0f - ((height + this.m_padding) * this.m_fontScale), 0f);
            this.m_vertices[index + 1] = start + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
            this.m_vertices[index + 2] = this.m_vertices[index] + new Vector3(x, 0f, 0f);
            this.m_vertices[index + 3] = start + new Vector3(x, this.m_padding * this.m_fontScale, 0f);
            this.m_vertices[index + 4] = this.m_vertices[index + 2];
            this.m_vertices[index + 5] = this.m_vertices[index + 3];
            this.m_vertices[index + 6] = end + new Vector3(-x, -(height + this.m_padding) * this.m_fontScale, 0f);
            this.m_vertices[index + 7] = end + new Vector3(-x, this.m_padding * this.m_fontScale, 0f);
            this.m_vertices[index + 8] = this.m_vertices[index + 6];
            this.m_vertices[index + 9] = this.m_vertices[index + 7];
            this.m_vertices[index + 10] = end + new Vector3(0f, -(height + this.m_padding) * this.m_fontScale, 0f);
            this.m_vertices[index + 11] = end + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
            Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - this.m_padding) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (((this.m_cached_Underline_GlyphInfo.y + this.m_padding) + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight));
            Vector2 vector2 = new Vector2(vector.x, 1f - ((this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight));
            Vector2 vector3 = new Vector2(((this.m_cached_Underline_GlyphInfo.x + this.m_padding) + (this.m_cached_Underline_GlyphInfo.width / 2f)) / this.m_fontAsset.fontInfo.AtlasWidth, vector.y);
            Vector2 vector4 = new Vector2(vector3.x, vector2.y);
            Vector2 vector5 = new Vector2(((this.m_cached_Underline_GlyphInfo.x + this.m_padding) + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector.y);
            Vector2 vector6 = new Vector2(vector5.x, vector2.y);
            this.m_uvs[0 + index] = vector;
            this.m_uvs[1 + index] = vector2;
            this.m_uvs[2 + index] = vector3;
            this.m_uvs[3 + index] = vector4;
            this.m_uvs[4 + index] = new Vector2(vector3.x - (vector3.x * 0.001f), vector.y);
            this.m_uvs[5 + index] = new Vector2(vector3.x - (vector3.x * 0.001f), vector2.y);
            this.m_uvs[6 + index] = new Vector2(vector3.x + (vector3.x * 0.001f), vector.y);
            this.m_uvs[7 + index] = new Vector2(vector3.x + (vector3.x * 0.001f), vector2.y);
            this.m_uvs[8 + index] = vector3;
            this.m_uvs[9 + index] = vector4;
            this.m_uvs[10 + index] = vector5;
            this.m_uvs[11 + index] = vector6;
            float num4 = 0f;
            float num5 = (this.m_vertices[index + 2].x - start.x) / (end.x - start.x);
            float scale = this.m_fontScale * this.m_transform.lossyScale.z;
            float num7 = scale;
            this.m_uv2s[0 + index] = this.PackUV(0f, 0f, scale);
            this.m_uv2s[1 + index] = this.PackUV(0f, 1f, scale);
            this.m_uv2s[2 + index] = this.PackUV(num5, 0f, scale);
            this.m_uv2s[3 + index] = this.PackUV(num5, 1f, scale);
            num4 = (this.m_vertices[index + 4].x - start.x) / (end.x - start.x);
            num5 = (this.m_vertices[index + 6].x - start.x) / (end.x - start.x);
            this.m_uv2s[4 + index] = this.PackUV(num4, 0f, num7);
            this.m_uv2s[5 + index] = this.PackUV(num4, 1f, num7);
            this.m_uv2s[6 + index] = this.PackUV(num5, 0f, num7);
            this.m_uv2s[7 + index] = this.PackUV(num5, 1f, num7);
            num4 = (this.m_vertices[index + 8].x - start.x) / (end.x - start.x);
            num5 = (this.m_vertices[index + 6].x - start.x) / (end.x - start.x);
            this.m_uv2s[8 + index] = this.PackUV(num4, 0f, scale);
            this.m_uv2s[9 + index] = this.PackUV(num4, 1f, scale);
            this.m_uv2s[10 + index] = this.PackUV(1f, 0f, scale);
            this.m_uv2s[11 + index] = this.PackUV(1f, 1f, scale);
            this.m_vertColors[0 + index] = underlineColor;
            this.m_vertColors[1 + index] = underlineColor;
            this.m_vertColors[2 + index] = underlineColor;
            this.m_vertColors[3 + index] = underlineColor;
            this.m_vertColors[4 + index] = underlineColor;
            this.m_vertColors[5 + index] = underlineColor;
            this.m_vertColors[6 + index] = underlineColor;
            this.m_vertColors[7 + index] = underlineColor;
            this.m_vertColors[8 + index] = underlineColor;
            this.m_vertColors[9 + index] = underlineColor;
            this.m_vertColors[10 + index] = underlineColor;
            this.m_vertColors[11 + index] = underlineColor;
            index += 12;
        }

        private void EnableMasking()
        {
            if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                this.m_sharedMaterial.EnableKeyword("MASK_SOFT");
                this.m_sharedMaterial.DisableKeyword("MASK_HARD");
                this.m_sharedMaterial.DisableKeyword("MASK_OFF");
                this.m_isMaskingEnabled = true;
                this.UpdateMask();
            }
        }

        public void ForceMeshUpdate()
        {
            this.havePropertiesChanged = true;
            this.OnWillRenderObject();
        }

        private void GenerateFastScaleAndColor()
        {
            if (this.m_renderMode == TextRenderFlags.Render)
            {
                this.m_mesh.MarkDynamic();
                if (this.haveColorChanged)
                {
                    Color[] colorArray = new Color[this.m_vertColors.Length];
                    for (int i = 0; i < this.m_vertColors.Length; i++)
                    {
                        colorArray[i] = this.m_fontColor;
                    }
                    this.m_mesh.colors = colorArray;
                    this.haveColorChanged = false;
                }
                if (this.haveFastScaleChanged)
                {
                    Vector3[] array = new Vector3[this.m_vertices.Length];
                    this.m_vertices.CopyTo(array, 0);
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j].Set(array[j].x * this.m_fastScale.x, array[j].y * this.m_fastScale.y, array[j].z * this.m_fastScale.z);
                    }
                    this.m_mesh.vertices = array;
                    this.haveFastScaleChanged = false;
                }
            }
        }

        private void GenerateTextMesh()
        {
            if (this.m_fontAsset.characterDictionary == null)
            {
                Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID());
            }
            else
            {
                if (this.m_textInfo != null)
                {
                    this.m_textInfo.Clear();
                }
                else
                {
                    this.m_textInfo = new TMP_TextInfo();
                }
                if ((this.m_char_buffer == null) || (this.m_char_buffer[0] == 0))
                {
                    if (this.m_vertices != null)
                    {
                        Array.Clear(this.m_vertices, 0, this.m_vertices.Length);
                        this.m_mesh.vertices = this.m_vertices;
                    }
                    this.m_preferredWidth = 0f;
                    this.m_preferredHeight = 0f;
                }
                else
                {
                    int num = this.SetArraySizes(this.m_char_buffer);
                    this.m_fontIndex = 0;
                    this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
                    this.m_fontScale = (this.m_fontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                    float fontScale = this.m_fontScale;
                    this.m_maxFontScale = 0f;
                    float num3 = 0f;
                    this.m_currentFontSize = this.m_fontSize;
                    float num4 = 0f;
                    int num5 = 0;
                    this.m_style = this.m_fontStyle;
                    this.m_lineJustification = this.m_textAlignment;
                    if (this.checkPaddingRequired)
                    {
                        this.checkPaddingRequired = false;
                        this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
                        this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
                        this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
                    }
                    float num6 = 0f;
                    float num7 = 1f;
                    this.m_baselineOffset = 0f;
                    bool flag2 = false;
                    Vector3 zero = Vector3.zero;
                    Vector3 end = Vector3.zero;
                    bool flag3 = false;
                    Vector3 start = Vector3.zero;
                    Vector3 vector4 = Vector3.zero;
                    this.m_fontColor32 = this.m_fontColor;
                    this.m_htmlColor = this.m_fontColor32;
                    this.m_colorStackIndex = 0;
                    Array.Clear(this.m_colorStack, 0, this.m_colorStack.Length);
                    this.m_lineOffset = 0f;
                    this.m_lineHeight = 0f;
                    this.m_cSpacing = 0f;
                    this.m_monoSpacing = 0f;
                    float num8 = 0f;
                    this.m_xAdvance = 0f;
                    this.m_indent = 0f;
                    this.m_maxXAdvance = 0f;
                    this.m_lineNumber = 0;
                    this.m_pageNumber = 0;
                    this.m_characterCount = 0;
                    this.m_visibleCharacterCount = 0;
                    this.m_firstVisibleCharacterOfLine = 0;
                    this.m_lastVisibleCharacterOfLine = 0;
                    int index = 0;
                    Vector3[] corners = this.m_textContainer.corners;
                    Vector4 margins = this.m_textContainer.margins;
                    this.m_marginWidth = (this.m_textContainer.rect.width - margins.z) - margins.x;
                    float num10 = (this.m_textContainer.rect.height - margins.y) - margins.w;
                    float z = this.m_transform.lossyScale.z;
                    this.m_preferredWidth = 0f;
                    this.m_preferredHeight = 0f;
                    bool flag4 = true;
                    bool flag5 = false;
                    this.m_SavedWordWrapState = new WordWrapState();
                    this.m_SavedLineState = new WordWrapState();
                    int num12 = 0;
                    this.m_meshExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
                    if (this.m_textInfo.lineInfo == null)
                    {
                        this.m_textInfo.lineInfo = new TMP_LineInfo[2];
                    }
                    for (int i = 0; i < this.m_textInfo.lineInfo.Length; i++)
                    {
                        this.m_textInfo.lineInfo[i] = new TMP_LineInfo();
                        this.m_textInfo.lineInfo[i].lineExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
                        this.m_textInfo.lineInfo[i].ascender = -this.k_InfinityVector.x;
                        this.m_textInfo.lineInfo[i].descender = this.k_InfinityVector.x;
                    }
                    this.m_maxAscender = 0f;
                    this.m_maxDescender = 0f;
                    this.m_isNewPage = false;
                    float num14 = 0f;
                    this.loopCountA++;
                    int endIndex = 0;
                    for (int j = 0; this.m_char_buffer[j] != 0; j++)
                    {
                        Color32 red;
                        int num21;
                        num5 = this.m_char_buffer[j];
                        this.loopCountE++;
                        if ((this.m_isRichText && (num5 == 60)) && this.ValidateHtmlTag(this.m_char_buffer, j + 1, out endIndex))
                        {
                            j = endIndex;
                            if (this.m_isRecalculateScaleRequired)
                            {
                                this.m_fontScale = (this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                                this.m_isRecalculateScaleRequired = false;
                            }
                            continue;
                        }
                        bool flag = false;
                        if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
                        {
                            if (char.IsLower((char) num5))
                            {
                                num5 -= 0x20;
                            }
                        }
                        else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
                        {
                            if (char.IsUpper((char) num5))
                            {
                                num5 += 0x20;
                            }
                        }
                        else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps) || ((this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps))
                        {
                            if (char.IsLower((char) num5))
                            {
                                this.m_fontScale = ((this.m_currentFontSize * 0.8f) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                                num5 -= 0x20;
                            }
                            else
                            {
                                this.m_fontScale = (this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                            }
                        }
                        this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num5, out this.m_cached_GlyphInfo);
                        if (this.m_cached_GlyphInfo == null)
                        {
                            if (char.IsLower((char) num5))
                            {
                                if (this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num5 - 0x20, out this.m_cached_GlyphInfo))
                                {
                                    num5 -= 0x20;
                                }
                            }
                            else if (char.IsUpper((char) num5) && this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num5 + 0x20, out this.m_cached_GlyphInfo))
                            {
                                num5 += 0x20;
                            }
                            if (this.m_cached_GlyphInfo == null)
                            {
                                this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(0x58, out this.m_cached_GlyphInfo);
                                if (this.m_cached_GlyphInfo == null)
                                {
                                    continue;
                                }
                                num5 = 0x58;
                                flag = true;
                            }
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].character = (char) num5;
                        this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
                        this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_style;
                        this.m_textInfo.characterInfo[this.m_characterCount].index = (short) j;
                        if (this.m_enableKerning && (this.m_characterCount >= 1))
                        {
                            KerningPair pair;
                            int character = this.m_textInfo.characterInfo[this.m_characterCount - 1].character;
                            KerningPairKey key = new KerningPairKey(character, num5);
                            this.m_fontAssetArray[this.m_fontIndex].kerningDictionary.TryGetValue(key.key, out pair);
                            if (pair != null)
                            {
                                this.m_xAdvance += pair.XadvanceOffset * this.m_fontScale;
                            }
                        }
                        if ((this.m_monoSpacing != 0f) && (this.m_xAdvance != 0f))
                        {
                            this.m_xAdvance -= ((this.m_cached_GlyphInfo.width / 2f) + this.m_cached_GlyphInfo.xOffset) * this.m_fontScale;
                        }
                        if (((this.m_style & FontStyles.Bold) == FontStyles.Bold) || ((this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
                        {
                            num6 = this.m_fontAssetArray[this.m_fontIndex].BoldStyle * 2f;
                            num7 = 1.07f;
                        }
                        else
                        {
                            num6 = this.m_fontAssetArray[this.m_fontIndex].NormalStyle * 2f;
                            num7 = 1f;
                        }
                        Vector3 vector6 = new Vector3((0f + this.m_xAdvance) + (((this.m_cached_GlyphInfo.xOffset - this.m_padding) - num6) * this.m_fontScale), (((this.m_cached_GlyphInfo.yOffset + this.m_padding) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset, 0f);
                        Vector3 vector7 = new Vector3(vector6.x, vector6.y - ((this.m_cached_GlyphInfo.height + (this.m_padding * 2f)) * this.m_fontScale), 0f);
                        Vector3 vector8 = new Vector3(vector7.x + (((this.m_cached_GlyphInfo.width + (this.m_padding * 2f)) + (num6 * 2f)) * this.m_fontScale), vector6.y, 0f);
                        Vector3 vector9 = new Vector3(vector8.x, vector7.y, 0f);
                        if (((this.m_style & FontStyles.Italic) == FontStyles.Italic) || ((this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic))
                        {
                            float num18 = this.m_fontAssetArray[this.m_fontIndex].ItalicStyle * 0.01f;
                            Vector3 vector10 = new Vector3(num18 * (((this.m_cached_GlyphInfo.yOffset + this.m_padding) + num6) * this.m_fontScale), 0f, 0f);
                            Vector3 vector11 = new Vector3(num18 * ((((this.m_cached_GlyphInfo.yOffset - this.m_cached_GlyphInfo.height) - this.m_padding) - num6) * this.m_fontScale), 0f, 0f);
                            vector6 += vector10;
                            vector7 += vector11;
                            vector8 += vector10;
                            vector9 += vector11;
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector6;
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector7;
                        this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector8;
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector9;
                        float num19 = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) + this.m_baselineOffset;
                        if (this.m_lineNumber == 0)
                        {
                            this.m_maxAscender = (this.m_maxAscender <= num19) ? num19 : this.m_maxAscender;
                        }
                        if (this.m_lineOffset == 0f)
                        {
                            num14 = (num14 <= num19) ? num19 : num14;
                        }
                        float num20 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                        this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
                        switch (num5)
                        {
                            case 0x20:
                            case 9:
                            case 10:
                            case 13:
                                goto Label_19A4;

                            default:
                                num21 = this.m_visibleCharacterCount * 4;
                                this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
                                this.m_textInfo.characterInfo[this.m_characterCount].vertexIndex = (short) (0 + num21);
                                if (this.m_vertices != null)
                                {
                                    this.m_vertices[0 + num21] = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
                                    this.m_vertices[1 + num21] = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
                                    this.m_vertices[2 + num21] = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
                                    this.m_vertices[3 + num21] = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
                                }
                                if (this.m_baselineOffset == 0f)
                                {
                                    this.m_maxFontScale = Mathf.Max(this.m_maxFontScale, this.m_fontScale);
                                }
                                if (((this.m_xAdvance + (this.m_cached_GlyphInfo.xAdvance * this.m_fontScale)) > (this.m_marginWidth + 0.0001f)) && !this.m_textContainer.isDefaultWidth)
                                {
                                    index = this.m_characterCount - 1;
                                    if (this.enableWordWrapping && (this.m_characterCount != this.m_firstVisibleCharacterOfLine))
                                    {
                                        if ((num12 == this.m_SavedWordWrapState.previous_WordBreak) || flag4)
                                        {
                                            if (this.m_enableAutoSizing && (this.m_fontSize > this.m_fontSizeMin))
                                            {
                                                this.m_maxFontSize = this.m_fontSize;
                                                this.m_fontSize -= Mathf.Max((float) ((this.m_fontSize - this.m_minFontSize) / 2f), (float) 0.05f);
                                                this.m_fontSize = ((float) ((int) ((Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f) + 0.5f))) / 20f;
                                                if (this.loopCountA <= 20)
                                                {
                                                    this.GenerateTextMesh();
                                                }
                                                return;
                                            }
                                            if (!this.m_isCharacterWrappingEnabled)
                                            {
                                                this.m_isCharacterWrappingEnabled = true;
                                            }
                                            else
                                            {
                                                flag5 = true;
                                            }
                                            this.m_recursiveCount++;
                                            if (this.m_recursiveCount > 20)
                                            {
                                                continue;
                                            }
                                        }
                                        j = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState);
                                        num12 = j;
                                        if ((((this.m_lineNumber > 0) && (this.m_maxFontScale != 0f)) && ((this.m_lineHeight == 0f) && (this.m_maxFontScale != num3))) && !this.m_isNewPage)
                                        {
                                            float num22 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
                                            float num23 = ((((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + num22) + this.m_lineSpacing) + this.m_lineSpacingDelta) * this.m_maxFontScale) - ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num22) * num3);
                                            this.m_lineOffset += num23 - num8;
                                            this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount - 1, num23 - num8);
                                        }
                                        this.m_isNewPage = false;
                                        float y = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale) - this.m_lineOffset;
                                        float num25 = (((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                                        y = (y <= num25) ? num25 : y;
                                        float num26 = ((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale) - this.m_lineOffset;
                                        float num27 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                                        num26 = (num26 >= num27) ? num27 : num26;
                                        if (this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].isVisible)
                                        {
                                            this.m_maxDescender = (this.m_maxDescender >= num26) ? num26 : this.m_maxDescender;
                                        }
                                        this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
                                        this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = ((this.m_characterCount - 1) <= 0) ? 1 : (this.m_characterCount - 1);
                                        this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
                                        this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = (this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex) + 1;
                                        this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num26);
                                        this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, y);
                                        this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - (this.m_padding * this.m_maxFontScale);
                                        this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing * this.m_fontScale);
                                        this.m_firstVisibleCharacterOfLine = this.m_characterCount;
                                        this.m_preferredWidth += this.m_xAdvance;
                                        if (this.m_enableWordWrapping)
                                        {
                                            this.m_preferredHeight = this.m_maxAscender - this.m_maxDescender;
                                        }
                                        else
                                        {
                                            this.m_preferredHeight = Mathf.Max(this.m_preferredHeight, y - num26);
                                        }
                                        this.SaveWordWrappingState(ref this.m_SavedLineState, j, this.m_characterCount - 1);
                                        this.m_lineNumber++;
                                        if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
                                        {
                                            this.ResizeLineExtents(this.m_lineNumber);
                                        }
                                        if (this.m_lineHeight == 0f)
                                        {
                                            num8 = ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_lineSpacing) + this.m_lineSpacingDelta) * this.m_fontScale;
                                            this.m_lineOffset += num8;
                                        }
                                        else
                                        {
                                            this.m_lineOffset += (this.m_lineHeight + this.m_lineSpacing) * fontScale;
                                        }
                                        num3 = this.m_fontScale;
                                        this.m_xAdvance = 0f + this.m_indent;
                                        this.m_maxFontScale = 0f;
                                        continue;
                                    }
                                    if (this.m_enableAutoSizing && (this.m_fontSize > this.m_fontSizeMin))
                                    {
                                        this.m_maxFontSize = this.m_fontSize;
                                        this.m_fontSize -= Mathf.Max((float) ((this.m_fontSize - this.m_minFontSize) / 2f), (float) 0.05f);
                                        this.m_fontSize = ((float) ((int) ((Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f) + 0.5f))) / 20f;
                                        this.m_recursiveCount = 0;
                                        if (this.loopCountA <= 20)
                                        {
                                            this.GenerateTextMesh();
                                        }
                                        return;
                                    }
                                    switch (this.m_overflowMode)
                                    {
                                        case TextOverflowModes.Overflow:
                                            if (this.m_isMaskingEnabled)
                                            {
                                                this.DisableMasking();
                                            }
                                            goto Label_1592;

                                        case TextOverflowModes.Ellipsis:
                                            if (this.m_isMaskingEnabled)
                                            {
                                                this.DisableMasking();
                                            }
                                            this.m_isTextTruncated = true;
                                            if (this.m_characterCount < 1)
                                            {
                                                this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
                                                this.m_visibleCharacterCount--;
                                                goto Label_1592;
                                            }
                                            this.m_char_buffer[j - 1] = 0x2026;
                                            this.m_char_buffer[j] = 0;
                                            this.GenerateTextMesh();
                                            return;

                                        case TextOverflowModes.Masking:
                                            if (!this.m_isMaskingEnabled)
                                            {
                                                this.EnableMasking();
                                            }
                                            goto Label_1592;

                                        case TextOverflowModes.Truncate:
                                            if (this.m_isMaskingEnabled)
                                            {
                                                this.DisableMasking();
                                            }
                                            this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
                                            goto Label_1592;

                                        case TextOverflowModes.ScrollRect:
                                            if (!this.m_isMaskingEnabled)
                                            {
                                                this.EnableMasking();
                                            }
                                            goto Label_1592;
                                    }
                                }
                                break;
                        }
                    Label_1592:
                        if (flag)
                        {
                            red = Color.red;
                        }
                        else if (this.m_overrideHtmlColors)
                        {
                            red = this.m_fontColor32;
                        }
                        else
                        {
                            red = this.m_htmlColor;
                        }
                        red.a = (this.m_fontColor32.a >= red.a) ? red.a : this.m_fontColor32.a;
                        if (!this.m_enableVertexGradient)
                        {
                            this.m_vertColors[0 + num21] = red;
                            this.m_vertColors[1 + num21] = red;
                            this.m_vertColors[2 + num21] = red;
                            this.m_vertColors[3 + num21] = red;
                        }
                        else
                        {
                            if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
                            {
                                this.m_vertColors[0 + num21] = red;
                                this.m_vertColors[1 + num21] = red;
                                this.m_vertColors[2 + num21] = red;
                                this.m_vertColors[3 + num21] = red;
                            }
                            else
                            {
                                this.m_vertColors[0 + num21] = this.m_fontColorGradient.bottomLeft;
                                this.m_vertColors[1 + num21] = this.m_fontColorGradient.topLeft;
                                this.m_vertColors[2 + num21] = this.m_fontColorGradient.bottomRight;
                                this.m_vertColors[3 + num21] = this.m_fontColorGradient.topRight;
                            }
                            this.m_vertColors[0 + num21].a = red.a;
                            this.m_vertColors[1 + num21].a = red.a;
                            this.m_vertColors[2 + num21].a = red.a;
                            this.m_vertColors[3 + num21].a = red.a;
                        }
                        if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
                        {
                            num6 = 0f;
                        }
                        Vector2 vector12 = new Vector2(((this.m_cached_GlyphInfo.x - this.m_padding) - num6) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, 1f - ((((this.m_cached_GlyphInfo.y + this.m_padding) + num6) + this.m_cached_GlyphInfo.height) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight));
                        Vector2 vector13 = new Vector2(vector12.x, 1f - (((this.m_cached_GlyphInfo.y - this.m_padding) - num6) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight));
                        Vector2 vector14 = new Vector2((((this.m_cached_GlyphInfo.x + this.m_padding) + num6) + this.m_cached_GlyphInfo.width) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, vector12.y);
                        Vector2 vector15 = new Vector2(vector14.x, vector13.y);
                        this.m_uvs[0 + num21] = vector12;
                        this.m_uvs[1 + num21] = vector13;
                        this.m_uvs[2 + num21] = vector14;
                        this.m_uvs[3 + num21] = vector15;
                        this.m_visibleCharacterCount++;
                        if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
                        {
                            this.m_lastVisibleCharacterOfLine = this.m_characterCount;
                        }
                        goto Label_19EC;
                    Label_19A4:
                        switch (num5)
                        {
                            case 9:
                            case 0x20:
                                this.m_textInfo.lineInfo[this.m_lineNumber].spaceCount++;
                                this.m_textInfo.spaceCount++;
                                break;
                        }
                    Label_19EC:
                        this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = (short) this.m_lineNumber;
                        this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = (short) this.m_pageNumber;
                        if ((((num5 != 10) && (num5 != 13)) && (num5 != 0x2026)) || (this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1))
                        {
                            this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
                        }
                        if ((((this.m_maxAscender - num20) + ((this.m_alignmentPadding.w * 2f) * this.m_fontScale)) > num10) && !this.m_textContainer.isDefaultHeight)
                        {
                            if (this.m_enableAutoSizing && (this.m_lineSpacingDelta > this.m_lineSpacingMax))
                            {
                                this.m_lineSpacingDelta--;
                                this.GenerateTextMesh();
                                return;
                            }
                            if (this.m_enableAutoSizing && (this.m_fontSize > this.m_fontSizeMin))
                            {
                                this.m_maxFontSize = this.m_fontSize;
                                this.m_fontSize -= Mathf.Max((float) ((this.m_fontSize - this.m_minFontSize) / 2f), (float) 0.05f);
                                this.m_fontSize = ((float) ((int) ((Mathf.Max(this.m_fontSize, this.m_fontSizeMin) * 20f) + 0.5f))) / 20f;
                                this.m_recursiveCount = 0;
                                if (this.loopCountA <= 20)
                                {
                                    this.GenerateTextMesh();
                                }
                                return;
                            }
                            switch (this.m_overflowMode)
                            {
                                case TextOverflowModes.Overflow:
                                    if (this.m_isMaskingEnabled)
                                    {
                                        this.DisableMasking();
                                    }
                                    break;

                                case TextOverflowModes.Ellipsis:
                                    if (this.m_isMaskingEnabled)
                                    {
                                        this.DisableMasking();
                                    }
                                    if (this.m_lineNumber > 0)
                                    {
                                        this.m_char_buffer[this.m_textInfo.characterInfo[index].index] = 0x2026;
                                        this.m_char_buffer[this.m_textInfo.characterInfo[index].index + 1] = 0;
                                        this.GenerateTextMesh();
                                        this.m_isTextTruncated = true;
                                        return;
                                    }
                                    this.m_char_buffer[0] = 0;
                                    this.GenerateTextMesh();
                                    this.m_isTextTruncated = true;
                                    return;

                                case TextOverflowModes.Masking:
                                    if (!this.m_isMaskingEnabled)
                                    {
                                        this.EnableMasking();
                                    }
                                    break;

                                case TextOverflowModes.Truncate:
                                    if (this.m_isMaskingEnabled)
                                    {
                                        this.DisableMasking();
                                    }
                                    if (this.m_lineNumber > 0)
                                    {
                                        this.m_char_buffer[this.m_textInfo.characterInfo[index].index + 1] = 0;
                                        this.GenerateTextMesh();
                                        this.m_isTextTruncated = true;
                                        return;
                                    }
                                    this.m_char_buffer[0] = 0;
                                    this.GenerateTextMesh();
                                    this.m_isTextTruncated = true;
                                    return;

                                case TextOverflowModes.ScrollRect:
                                    if (!this.m_isMaskingEnabled)
                                    {
                                        this.EnableMasking();
                                    }
                                    break;

                                case TextOverflowModes.Page:
                                {
                                    if (this.m_isMaskingEnabled)
                                    {
                                        this.DisableMasking();
                                    }
                                    j = this.RestoreWordWrappingState(ref this.m_SavedLineState);
                                    if (j == 0)
                                    {
                                        this.m_char_buffer[0] = 0;
                                        this.GenerateTextMesh();
                                        this.m_isTextTruncated = true;
                                        return;
                                    }
                                    this.m_isNewPage = true;
                                    this.m_xAdvance = 0f + this.m_indent;
                                    this.m_lineOffset = 0f;
                                    this.m_lineNumber++;
                                    this.m_pageNumber++;
                                    continue;
                                }
                            }
                        }
                        if (num5 == 9)
                        {
                            this.m_xAdvance += (this.m_fontAsset.fontInfo.TabWidth * this.m_fontScale) * this.m_fontAsset.TabSize;
                        }
                        else if (this.m_monoSpacing != 0f)
                        {
                            this.m_xAdvance += ((((this.m_monoSpacing + (this.m_cached_GlyphInfo.width / 2f)) + this.m_cached_GlyphInfo.xOffset) + this.m_characterSpacing) + this.m_cSpacing) * this.m_fontScale;
                        }
                        else
                        {
                            this.m_xAdvance += ((this.m_cached_GlyphInfo.xAdvance * num7) * this.m_fontScale) + ((this.m_characterSpacing + this.m_cSpacing) * this.m_fontScale);
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].xAdvance = this.m_xAdvance;
                        if (num5 == 13)
                        {
                            this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, (this.m_preferredWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                            this.m_preferredWidth = 0f;
                            this.m_xAdvance = 0f + this.m_indent;
                        }
                        if ((num5 == 10) || (this.m_characterCount == (num - 1)))
                        {
                            if ((((this.m_lineNumber > 0) && (this.m_maxFontScale != 0f)) && ((this.m_lineHeight == 0f) && (this.m_maxFontScale != num3))) && !this.m_isNewPage)
                            {
                                float num28 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
                                float num29 = (((((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + num28) + this.m_lineSpacing) + this.m_paragraphSpacing) + this.m_lineSpacingDelta) * this.m_maxFontScale) - ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num28) * num3);
                                this.m_lineOffset += num29 - num8;
                                this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount, num29 - num8);
                            }
                            this.m_isNewPage = false;
                            float num30 = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale) - this.m_lineOffset;
                            float num31 = (((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                            num30 = (num30 <= num31) ? num31 : num30;
                            float num32 = ((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale) - this.m_lineOffset;
                            float num33 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                            num32 = (num32 >= num33) ? num33 : num32;
                            this.m_maxDescender = (this.m_maxDescender >= num32) ? num32 : this.m_maxDescender;
                            this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = this.m_characterCount;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
                            this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = (this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex) + 1;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num32);
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num30);
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - (this.m_padding * this.m_maxFontScale);
                            this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing * this.m_fontScale);
                            this.m_firstVisibleCharacterOfLine = this.m_characterCount + 1;
                            if ((num5 == 10) && (this.m_characterCount != (num - 1)))
                            {
                                this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, (this.m_preferredWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                                this.m_preferredWidth = 0f;
                            }
                            else
                            {
                                this.m_preferredWidth = Mathf.Max(this.m_maxXAdvance, (this.m_preferredWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                            }
                            this.m_preferredHeight = this.m_maxAscender - this.m_maxDescender;
                            if (num5 == 10)
                            {
                                this.SaveWordWrappingState(ref this.m_SavedLineState, j, this.m_characterCount);
                                this.SaveWordWrappingState(ref this.m_SavedWordWrapState, j, this.m_characterCount);
                                this.m_lineNumber++;
                                if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
                                {
                                    this.ResizeLineExtents(this.m_lineNumber);
                                }
                                if (this.m_lineHeight == 0f)
                                {
                                    num8 = (((this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_paragraphSpacing) + this.m_lineSpacing) + this.m_lineSpacingDelta) * this.m_fontScale;
                                    this.m_lineOffset += num8;
                                }
                                else
                                {
                                    this.m_lineOffset += ((this.m_lineHeight + this.m_paragraphSpacing) + this.m_lineSpacing) * fontScale;
                                }
                                num3 = this.m_fontScale;
                                this.m_maxFontScale = 0f;
                                this.m_xAdvance = 0f + this.m_indent;
                                index = this.m_characterCount - 1;
                            }
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].baseLine = this.m_textInfo.characterInfo[this.m_characterCount].topRight.y - ((this.m_cached_GlyphInfo.yOffset + this.m_padding) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].topLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - this.m_alignmentPadding.w) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].padding = this.m_padding * this.m_fontScale;
                        this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = this.m_cached_GlyphInfo.width / this.m_cached_GlyphInfo.height;
                        this.m_textInfo.characterInfo[this.m_characterCount].scale = this.m_fontScale;
                        if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
                        {
                            float x = Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.x);
                            this.m_meshExtents.min = new Vector2(x, Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.y));
                            float introduced104 = Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].topRight.x);
                            this.m_meshExtents.max = new Vector2(introduced104, Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].topLeft.y));
                        }
                        if (((num5 != 13) && (num5 != 10)) && (this.m_pageNumber < 0x10))
                        {
                            this.m_textInfo.pageInfo[this.m_pageNumber].ascender = num14;
                            this.m_textInfo.pageInfo[this.m_pageNumber].descender = (num20 >= this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? this.m_textInfo.pageInfo[this.m_pageNumber].descender : num20;
                            if ((this.m_pageNumber == 0) && (this.m_characterCount == 0))
                            {
                                this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
                            }
                            else if ((this.m_characterCount > 0) && (this.m_pageNumber != this.m_textInfo.characterInfo[this.m_characterCount - 1].pageNumber))
                            {
                                this.m_textInfo.pageInfo[this.m_pageNumber - 1].lastCharacterIndex = this.m_characterCount - 1;
                                this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
                            }
                            else if (this.m_characterCount == (num - 1))
                            {
                                this.m_textInfo.pageInfo[this.m_pageNumber].lastCharacterIndex = this.m_characterCount;
                            }
                        }
                        if ((this.m_enableWordWrapping || (this.m_overflowMode == TextOverflowModes.Truncate)) || (this.m_overflowMode == TextOverflowModes.Ellipsis))
                        {
                            if ((num5 == 9) || (num5 == 0x20))
                            {
                                this.SaveWordWrappingState(ref this.m_SavedWordWrapState, j, this.m_characterCount);
                                this.m_isCharacterWrappingEnabled = false;
                                flag4 = false;
                            }
                            else if (((flag4 || this.m_isCharacterWrappingEnabled) && (((this.m_characterCount < (num - 1)) && !this.m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num5)) && !this.m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(this.m_VisibleCharacters[this.m_characterCount + 1]))) || flag5)
                            {
                                this.SaveWordWrappingState(ref this.m_SavedWordWrapState, j, this.m_characterCount);
                            }
                        }
                        this.m_characterCount++;
                    }
                    num4 = this.m_maxFontSize - this.m_minFontSize;
                    if (((!this.m_textContainer.isDefaultWidth || !this.m_textContainer.isDefaultHeight) && (!this.m_isCharacterWrappingEnabled && this.m_enableAutoSizing)) && ((num4 > 0.051f) && (this.m_fontSize < this.m_fontSizeMax)))
                    {
                        this.m_minFontSize = this.m_fontSize;
                        this.m_fontSize += Mathf.Max((float) ((this.m_maxFontSize - this.m_fontSize) / 2f), (float) 0.05f);
                        this.m_fontSize = ((float) ((int) ((Mathf.Min(this.m_fontSize, this.m_fontSizeMax) * 20f) + 0.5f))) / 20f;
                        if (this.loopCountA <= 20)
                        {
                            this.GenerateTextMesh();
                        }
                    }
                    else
                    {
                        if (this.m_characterCount < this.m_textInfo.characterInfo.Length)
                        {
                            this.m_textInfo.characterInfo[this.m_characterCount].character = '\0';
                        }
                        this.m_isCharacterWrappingEnabled = false;
                        if (this.m_renderMode != TextRenderFlags.GetPreferredSizes)
                        {
                            if (this.m_visibleCharacterCount == 0)
                            {
                                if (this.m_vertices != null)
                                {
                                    Array.Clear(this.m_vertices, 0, this.m_vertices.Length);
                                    this.m_mesh.vertices = this.m_vertices;
                                }
                            }
                            else
                            {
                                int num34 = this.m_visibleCharacterCount * 4;
                                Array.Clear(this.m_vertices, num34, this.m_vertices.Length - num34);
                                switch (this.m_textAlignment)
                                {
                                    case TextAlignmentOptions.TopLeft:
                                    case TextAlignmentOptions.Top:
                                    case TextAlignmentOptions.TopRight:
                                    case TextAlignmentOptions.TopJustified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = corners[1] + new Vector3(0f + margins.x, (0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender) - margins.y, 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = corners[1] + new Vector3(0f + margins.x, (0f - this.m_maxAscender) - margins.y, 0f);
                                        break;

                                    case TextAlignmentOptions.Left:
                                    case TextAlignmentOptions.Center:
                                    case TextAlignmentOptions.Right:
                                    case TextAlignmentOptions.Justified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = ((Vector3) ((corners[0] + corners[1]) / 2f)) + new Vector3(0f + margins.x, 0f - ((((this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender + margins.y) + this.m_textInfo.pageInfo[this.m_pageToDisplay].descender) - margins.w) / 2f), 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = ((Vector3) ((corners[0] + corners[1]) / 2f)) + new Vector3(0f + margins.x, 0f - ((((this.m_maxAscender + margins.y) + this.m_maxDescender) - margins.w) / 2f), 0f);
                                        break;

                                    case TextAlignmentOptions.BottomLeft:
                                    case TextAlignmentOptions.Bottom:
                                    case TextAlignmentOptions.BottomRight:
                                    case TextAlignmentOptions.BottomJustified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = corners[0] + new Vector3(0f + margins.x, (0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].descender) + margins.w, 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = corners[0] + new Vector3(0f + margins.x, (0f - this.m_maxDescender) + margins.w, 0f);
                                        break;

                                    case TextAlignmentOptions.BaselineLeft:
                                    case TextAlignmentOptions.Baseline:
                                    case TextAlignmentOptions.BaselineRight:
                                        this.m_anchorOffset = ((Vector3) ((corners[0] + corners[1]) / 2f)) + new Vector3(0f + margins.x, 0f, 0f);
                                        break;

                                    case TextAlignmentOptions.MidlineLeft:
                                    case TextAlignmentOptions.Midline:
                                    case TextAlignmentOptions.MidlineRight:
                                    case TextAlignmentOptions.MidlineJustified:
                                        this.m_anchorOffset = ((Vector3) ((corners[0] + corners[1]) / 2f)) + new Vector3(0f + margins.x, 0f - ((((this.m_meshExtents.max.y + margins.y) + this.m_meshExtents.min.y) - margins.w) / 2f), 0f);
                                        break;
                                }
                                Vector3 vector16 = Vector3.zero;
                                Vector3 vector17 = Vector3.zero;
                                int num35 = 0;
                                int num36 = 0;
                                int num37 = 0;
                                int num38 = 0;
                                Color32 underlineColor = new Color32(0xff, 0xff, 0xff, 0x7f);
                                bool flag6 = false;
                                int num39 = 0;
                                int num40 = 0;
                                for (int k = 0; k < this.m_characterCount; k++)
                                {
                                    float num43;
                                    float num45;
                                    int lineNumber = this.m_textInfo.characterInfo[k].lineNumber;
                                    char c = this.m_textInfo.characterInfo[k].character;
                                    TMP_LineInfo info = this.m_textInfo.lineInfo[lineNumber];
                                    TextAlignmentOptions alignment = info.alignment;
                                    num37 = lineNumber + 1;
                                    switch (alignment)
                                    {
                                        case TextAlignmentOptions.TopLeft:
                                        case TextAlignmentOptions.Left:
                                        case TextAlignmentOptions.BottomLeft:
                                        case TextAlignmentOptions.BaselineLeft:
                                        case TextAlignmentOptions.MidlineLeft:
                                            vector16 = Vector3.zero;
                                            goto Label_3199;

                                        case TextAlignmentOptions.Top:
                                        case TextAlignmentOptions.Center:
                                        case TextAlignmentOptions.Bottom:
                                        case TextAlignmentOptions.Baseline:
                                        case TextAlignmentOptions.Midline:
                                            vector16 = new Vector3((this.m_marginWidth / 2f) - (info.maxAdvance / 2f), 0f, 0f);
                                            goto Label_3199;

                                        case TextAlignmentOptions.TopRight:
                                        case TextAlignmentOptions.Right:
                                        case TextAlignmentOptions.BottomRight:
                                        case TextAlignmentOptions.BaselineRight:
                                        case TextAlignmentOptions.MidlineRight:
                                            vector16 = new Vector3(this.m_marginWidth - info.maxAdvance, 0f, 0f);
                                            goto Label_3199;

                                        case TextAlignmentOptions.TopJustified:
                                        case TextAlignmentOptions.Justified:
                                        case TextAlignmentOptions.BottomJustified:
                                        case TextAlignmentOptions.MidlineJustified:
                                        {
                                            num5 = this.m_textInfo.characterInfo[k].character;
                                            char ch2 = this.m_textInfo.characterInfo[info.lastCharacterIndex].character;
                                            if ((!char.IsWhiteSpace(ch2) || char.IsControl(ch2)) || (lineNumber >= this.m_lineNumber))
                                            {
                                                goto Label_318D;
                                            }
                                            num43 = ((corners[3].x - margins.z) - (corners[0].x + margins.x)) - info.maxAdvance;
                                            if ((lineNumber == num38) && (k != 0))
                                            {
                                                break;
                                            }
                                            vector16 = Vector3.zero;
                                            goto Label_3199;
                                        }
                                        default:
                                            goto Label_3199;
                                    }
                                    switch (num5)
                                    {
                                        case 9:
                                        case 0x20:
                                            vector16 += new Vector3((num43 * (1f - this.m_wordWrappingRatios)) / ((float) (info.spaceCount - 1)), 0f, 0f);
                                            goto Label_3199;

                                        default:
                                            vector16 += new Vector3((num43 * this.m_wordWrappingRatios) / ((float) ((info.characterCount - info.spaceCount) - 1)), 0f, 0f);
                                            goto Label_3199;
                                    }
                                Label_318D:
                                    vector16 = Vector3.zero;
                                Label_3199:
                                    vector17 = this.m_anchorOffset + vector16;
                                    if (!this.m_textInfo.characterInfo[k].isVisible)
                                    {
                                        goto Label_41F4;
                                    }
                                    Extents lineExtents = info.lineExtents;
                                    float num44 = ((this.m_uvLineOffset * lineNumber) % 1f) + this.m_uvOffset.x;
                                    switch (this.m_horizontalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            this.m_uv2s[num35].x = 0f + this.m_uvOffset.x;
                                            this.m_uv2s[num35 + 1].x = 0f + this.m_uvOffset.x;
                                            this.m_uv2s[num35 + 2].x = 1f + this.m_uvOffset.x;
                                            this.m_uv2s[num35 + 3].x = 1f + this.m_uvOffset.x;
                                            goto Label_3B1D;

                                        case TextureMappingOptions.Line:
                                            if (this.m_textAlignment == TextAlignmentOptions.Justified)
                                            {
                                                break;
                                            }
                                            this.m_uv2s[num35].x = ((this.m_vertices[num35].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 1].x = ((this.m_vertices[num35 + 1].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 2].x = ((this.m_vertices[num35 + 2].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 3].x = ((this.m_vertices[num35 + 3].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num44;
                                            goto Label_3B1D;

                                        case TextureMappingOptions.Paragraph:
                                            this.m_uv2s[num35].x = (((this.m_vertices[num35].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 1].x = (((this.m_vertices[num35 + 1].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 2].x = (((this.m_vertices[num35 + 2].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                            this.m_uv2s[num35 + 3].x = (((this.m_vertices[num35 + 3].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                            goto Label_3B1D;

                                        case TextureMappingOptions.MatchAspect:
                                            switch (this.m_verticalMapping)
                                            {
                                                case TextureMappingOptions.Character:
                                                    goto Label_3751;

                                                case TextureMappingOptions.Line:
                                                    goto Label_37E8;

                                                case TextureMappingOptions.Paragraph:
                                                    goto Label_38DB;

                                                case TextureMappingOptions.MatchAspect:
                                                    goto Label_39E6;
                                            }
                                            goto Label_39F5;

                                        default:
                                            goto Label_3B1D;
                                    }
                                    this.m_uv2s[num35].x = (((this.m_vertices[num35].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                    this.m_uv2s[num35 + 1].x = (((this.m_vertices[num35 + 1].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                    this.m_uv2s[num35 + 2].x = (((this.m_vertices[num35 + 2].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                    this.m_uv2s[num35 + 3].x = (((this.m_vertices[num35 + 3].x + vector16.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num44;
                                    goto Label_3B1D;
                                Label_3751:
                                    this.m_uv2s[num35].y = 0f + this.m_uvOffset.y;
                                    this.m_uv2s[num35 + 1].y = 1f + this.m_uvOffset.y;
                                    this.m_uv2s[num35 + 2].y = 0f + this.m_uvOffset.y;
                                    this.m_uv2s[num35 + 3].y = 1f + this.m_uvOffset.y;
                                    goto Label_39F5;
                                Label_37E8:
                                    this.m_uv2s[num35].y = ((this.m_vertices[num35].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + num44;
                                    this.m_uv2s[num35 + 1].y = ((this.m_vertices[num35 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + num44;
                                    this.m_uv2s[num35 + 2].y = this.m_uv2s[num35].y;
                                    this.m_uv2s[num35 + 3].y = this.m_uv2s[num35 + 1].y;
                                    goto Label_39F5;
                                Label_38DB:
                                    this.m_uv2s[num35].y = ((this.m_vertices[num35].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + num44;
                                    this.m_uv2s[num35 + 1].y = ((this.m_vertices[num35 + 1].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + num44;
                                    this.m_uv2s[num35 + 2].y = this.m_uv2s[num35].y;
                                    this.m_uv2s[num35 + 3].y = this.m_uv2s[num35 + 1].y;
                                    goto Label_39F5;
                                Label_39E6:
                                    Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                Label_39F5:
                                    num45 = (1f - ((this.m_uv2s[num35].y + this.m_uv2s[num35 + 1].y) * this.m_textInfo.characterInfo[k].aspectRatio)) / 2f;
                                    this.m_uv2s[num35].x = ((this.m_uv2s[num35].y * this.m_textInfo.characterInfo[k].aspectRatio) + num45) + num44;
                                    this.m_uv2s[num35 + 1].x = this.m_uv2s[num35].x;
                                    this.m_uv2s[num35 + 2].x = ((this.m_uv2s[num35 + 1].y * this.m_textInfo.characterInfo[k].aspectRatio) + num45) + num44;
                                    this.m_uv2s[num35 + 3].x = this.m_uv2s[num35 + 2].x;
                                Label_3B1D:
                                    switch (this.m_verticalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            this.m_uv2s[num35].y = 0f + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 1].y = 1f + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 2].y = 0f + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 3].y = 1f + this.m_uvOffset.y;
                                            break;

                                        case TextureMappingOptions.Line:
                                            this.m_uv2s[num35].y = ((this.m_vertices[num35].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 1].y = ((this.m_vertices[num35 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 2].y = this.m_uv2s[num35].y;
                                            this.m_uv2s[num35 + 3].y = this.m_uv2s[num35 + 1].y;
                                            break;

                                        case TextureMappingOptions.Paragraph:
                                            this.m_uv2s[num35].y = ((this.m_vertices[num35].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 1].y = ((this.m_vertices[num35 + 1].y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 2].y = this.m_uv2s[num35].y;
                                            this.m_uv2s[num35 + 3].y = this.m_uv2s[num35 + 1].y;
                                            break;

                                        case TextureMappingOptions.MatchAspect:
                                        {
                                            float num46 = (1f - ((this.m_uv2s[num35].x + this.m_uv2s[num35 + 2].x) / this.m_textInfo.characterInfo[k].aspectRatio)) / 2f;
                                            this.m_uv2s[num35].y = (num46 + (this.m_uv2s[num35].x / this.m_textInfo.characterInfo[k].aspectRatio)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 1].y = (num46 + (this.m_uv2s[num35 + 2].x / this.m_textInfo.characterInfo[k].aspectRatio)) + this.m_uvOffset.y;
                                            this.m_uv2s[num35 + 2].y = this.m_uv2s[num35].y;
                                            this.m_uv2s[num35 + 3].y = this.m_uv2s[num35 + 1].y;
                                            break;
                                        }
                                    }
                                    float scale = this.m_textInfo.characterInfo[k].scale * z;
                                    if ((this.m_textInfo.characterInfo[k].style & FontStyles.Bold) == FontStyles.Bold)
                                    {
                                        scale *= -1f;
                                    }
                                    float f = this.m_uv2s[num35].x;
                                    float num49 = this.m_uv2s[num35].y;
                                    float num50 = this.m_uv2s[num35 + 3].x;
                                    float num51 = this.m_uv2s[num35 + 3].y;
                                    float num52 = Mathf.Floor(f);
                                    float num53 = Mathf.Floor(num49);
                                    f -= num52;
                                    num50 -= num52;
                                    num49 -= num53;
                                    num51 -= num53;
                                    this.m_uv2s[num35] = this.PackUV(f, num49, scale);
                                    this.m_uv2s[num35 + 1] = this.PackUV(f, num51, scale);
                                    this.m_uv2s[num35 + 2] = this.PackUV(num50, num49, scale);
                                    this.m_uv2s[num35 + 3] = this.PackUV(num50, num51, scale);
                                    if ((((this.m_maxVisibleCharacters != -1) && (k >= this.m_maxVisibleCharacters)) || ((this.m_maxVisibleLines != -1) && (lineNumber >= this.m_maxVisibleLines))) || ((this.m_overflowMode == TextOverflowModes.Page) && (this.m_textInfo.characterInfo[k].pageNumber != this.m_pageToDisplay)))
                                    {
                                        this.m_vertices[num35] = (Vector3) (this.m_vertices[num35] * 0f);
                                        this.m_vertices[num35 + 1] = (Vector3) (this.m_vertices[num35 + 1] * 0f);
                                        this.m_vertices[num35 + 2] = (Vector3) (this.m_vertices[num35 + 2] * 0f);
                                        this.m_vertices[num35 + 3] = (Vector3) (this.m_vertices[num35 + 3] * 0f);
                                    }
                                    else
                                    {
                                        this.m_vertices[num35] += vector17;
                                        this.m_vertices[num35 + 1] += vector17;
                                        this.m_vertices[num35 + 2] += vector17;
                                        this.m_vertices[num35 + 3] += vector17;
                                    }
                                    num35 += 4;
                                Label_41F4:
                                    this.m_textInfo.characterInfo[k].bottomLeft += vector17;
                                    this.m_textInfo.characterInfo[k].topRight += vector17;
                                    this.m_textInfo.characterInfo[k].topLine += vector17.y;
                                    this.m_textInfo.characterInfo[k].bottomLine += vector17.y;
                                    this.m_textInfo.characterInfo[k].baseLine += vector17.y;
                                    this.m_textInfo.lineInfo[lineNumber].ascender = (this.m_textInfo.characterInfo[k].topLine <= this.m_textInfo.lineInfo[lineNumber].ascender) ? this.m_textInfo.lineInfo[lineNumber].ascender : this.m_textInfo.characterInfo[k].topLine;
                                    this.m_textInfo.lineInfo[lineNumber].descender = (this.m_textInfo.characterInfo[k].bottomLine >= this.m_textInfo.lineInfo[lineNumber].descender) ? this.m_textInfo.lineInfo[lineNumber].descender : this.m_textInfo.characterInfo[k].bottomLine;
                                    if ((lineNumber != num38) || (k == (this.m_characterCount - 1)))
                                    {
                                        if (lineNumber != num38)
                                        {
                                            this.m_textInfo.lineInfo[num38].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num38].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num38].descender);
                                            this.m_textInfo.lineInfo[num38].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num38].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num38].ascender);
                                        }
                                        if (k == (this.m_characterCount - 1))
                                        {
                                            this.m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[lineNumber].descender);
                                            this.m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[lineNumber].ascender);
                                        }
                                    }
                                    if (char.IsLetterOrDigit(c) && (k < (this.m_characterCount - 1)))
                                    {
                                        if (!flag6)
                                        {
                                            flag6 = true;
                                            num39 = k;
                                        }
                                    }
                                    else if ((((char.IsPunctuation(c) || char.IsWhiteSpace(c)) || (k == (this.m_characterCount - 1))) && flag6) || (k == 0))
                                    {
                                        num40 = ((k != (this.m_characterCount - 1)) || !char.IsLetterOrDigit(c)) ? (k - 1) : k;
                                        flag6 = false;
                                        num36++;
                                        this.m_textInfo.lineInfo[lineNumber].wordCount++;
                                        TMP_WordInfo item = new TMP_WordInfo {
                                            firstCharacterIndex = num39,
                                            lastCharacterIndex = num40,
                                            characterCount = (num40 - num39) + 1
                                        };
                                        this.m_textInfo.wordInfo.Add(item);
                                    }
                                    if ((this.m_textInfo.characterInfo[k].style & FontStyles.Underline) == FontStyles.Underline)
                                    {
                                        if ((!flag2 && (c != ' ')) && ((c != '\n') && (c != '\r')))
                                        {
                                            flag2 = true;
                                            zero = new Vector3(this.m_textInfo.characterInfo[k].bottomLeft.x, this.m_textInfo.characterInfo[k].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                            underlineColor = this.m_textInfo.characterInfo[k].color;
                                        }
                                        if (this.m_characterCount == 1)
                                        {
                                            flag2 = false;
                                            end = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                            this.DrawUnderlineMesh(zero, end, ref num34, underlineColor);
                                        }
                                        else if (k == info.lastCharacterIndex)
                                        {
                                            switch (c)
                                            {
                                                case ' ':
                                                case '\n':
                                                case '\r':
                                                {
                                                    int lastVisibleCharacterIndex = info.lastVisibleCharacterIndex;
                                                    end = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                                    break;
                                                }
                                                default:
                                                    end = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                                    break;
                                            }
                                            flag2 = false;
                                            this.DrawUnderlineMesh(zero, end, ref num34, underlineColor);
                                        }
                                    }
                                    else if (flag2)
                                    {
                                        flag2 = false;
                                        end = new Vector3(this.m_textInfo.characterInfo[k - 1].topRight.x, this.m_textInfo.characterInfo[k - 1].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                        this.DrawUnderlineMesh(zero, end, ref num34, underlineColor);
                                    }
                                    if ((this.m_textInfo.characterInfo[k].style & FontStyles.Strikethrough) == FontStyles.Strikethrough)
                                    {
                                        if ((!flag3 && (c != ' ')) && ((c != '\n') && (c != '\r')))
                                        {
                                            flag3 = true;
                                            start = new Vector3(this.m_textInfo.characterInfo[k].bottomLeft.x, this.m_textInfo.characterInfo[k].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                            underlineColor = this.m_textInfo.characterInfo[k].color;
                                        }
                                        if (this.m_characterCount == 1)
                                        {
                                            flag3 = false;
                                            vector4 = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                            this.DrawUnderlineMesh(start, vector4, ref num34, underlineColor);
                                        }
                                        else if (k == info.lastCharacterIndex)
                                        {
                                            switch (c)
                                            {
                                                case ' ':
                                                case '\n':
                                                case '\r':
                                                {
                                                    int num55 = info.lastVisibleCharacterIndex;
                                                    vector4 = new Vector3(this.m_textInfo.characterInfo[num55].topRight.x, this.m_textInfo.characterInfo[num55].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                                    break;
                                                }
                                                default:
                                                    vector4 = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                                    break;
                                            }
                                            flag3 = false;
                                            this.DrawUnderlineMesh(start, vector4, ref num34, underlineColor);
                                        }
                                    }
                                    else if (flag3)
                                    {
                                        flag3 = false;
                                        vector4 = new Vector3(this.m_textInfo.characterInfo[k - 1].topRight.x, this.m_textInfo.characterInfo[k - 1].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                        this.DrawUnderlineMesh(start, vector4, ref num34, underlineColor);
                                    }
                                    num38 = lineNumber;
                                }
                                this.m_textInfo.characterCount = (short) this.m_characterCount;
                                this.m_textInfo.lineCount = (short) num37;
                                this.m_textInfo.wordCount = ((num36 == 0) || (this.m_characterCount <= 0)) ? 1 : ((short) num36);
                                this.m_textInfo.pageCount = this.m_pageNumber;
                                this.m_textInfo.meshInfo.vertices = this.m_vertices;
                                this.m_textInfo.meshInfo.uv0s = this.m_uvs;
                                this.m_textInfo.meshInfo.uv2s = this.m_uv2s;
                                this.m_textInfo.meshInfo.vertexColors = this.m_vertColors;
                                if (this.m_renderMode == TextRenderFlags.Render)
                                {
                                    this.m_mesh.MarkDynamic();
                                    if (!this.haveFastScaleChanged)
                                    {
                                        this.m_mesh.vertices = this.m_vertices;
                                    }
                                    else
                                    {
                                        Vector3[] array = new Vector3[this.m_vertices.Length];
                                        this.m_vertices.CopyTo(array, 0);
                                        for (int m = 0; m < array.Length; m++)
                                        {
                                            array[m].Set(array[m].x * this.m_fastScale.x, array[m].y * this.m_fastScale.y, array[m].z * this.m_fastScale.z);
                                        }
                                        this.m_mesh.vertices = array;
                                    }
                                    this.m_mesh.uv = this.m_uvs;
                                    this.m_mesh.uv2 = this.m_uv2s;
                                    this.m_mesh.colors32 = this.m_vertColors;
                                }
                                this.m_mesh.RecalculateBounds();
                                if ((this.m_textContainer.isDefaultWidth || this.m_textContainer.isDefaultHeight) && this.m_textContainer.isAutoFitting)
                                {
                                    if (this.m_textContainer.isDefaultWidth)
                                    {
                                        this.m_textContainer.width = (this.m_preferredWidth + margins.x) + margins.z;
                                    }
                                    if (this.m_textContainer.isDefaultHeight)
                                    {
                                        this.m_textContainer.height = (this.m_preferredHeight + margins.y) + margins.w;
                                    }
                                    if (this.m_isMaskingEnabled)
                                    {
                                        this.isMaskUpdateRequired = true;
                                    }
                                    this.GenerateTextMesh();
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetArraySizes(int[] chars)
        {
            int num = 0;
            int num2 = 0;
            int endIndex = 0;
            this.m_isUsingBold = false;
            this.m_VisibleCharacters.Clear();
            for (int i = 0; chars[i] != 0; i++)
            {
                int num5 = chars[i];
                if ((this.m_isRichText && (num5 == 60)) && this.ValidateHtmlTag(chars, i + 1, out endIndex))
                {
                    i = endIndex;
                    if ((this.m_style & FontStyles.Underline) == FontStyles.Underline)
                    {
                        num += 3;
                    }
                    if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
                    {
                        this.m_isUsingBold = true;
                    }
                }
                else
                {
                    if (((num5 != 0x20) && (num5 != 9)) && ((num5 != 10) && (num5 != 13)))
                    {
                        num++;
                    }
                    this.m_VisibleCharacters.Add((char) ((ushort) num5));
                    num2++;
                }
            }
            return num2;
        }

        public TMP_TextInfo GetTextInfo(string text)
        {
            this.StringToCharArray(text, ref this.m_char_buffer);
            this.m_renderMode = TextRenderFlags.DontRender;
            this.GenerateTextMesh();
            this.m_renderMode = TextRenderFlags.Render;
            return this.textInfo;
        }

        private Color32 HexCharsToColor(char[] hexChars, int tagCount)
        {
            if (tagCount == 7)
            {
                byte r = (byte) ((this.HexToInt(hexChars[1]) * 0x10) + this.HexToInt(hexChars[2]));
                byte g = (byte) ((this.HexToInt(hexChars[3]) * 0x10) + this.HexToInt(hexChars[4]));
                return new Color32(r, g, (byte) ((this.HexToInt(hexChars[5]) * 0x10) + this.HexToInt(hexChars[6])), 0xff);
            }
            if (tagCount == 9)
            {
                byte num4 = (byte) ((this.HexToInt(hexChars[1]) * 0x10) + this.HexToInt(hexChars[2]));
                byte num5 = (byte) ((this.HexToInt(hexChars[3]) * 0x10) + this.HexToInt(hexChars[4]));
                byte b = (byte) ((this.HexToInt(hexChars[5]) * 0x10) + this.HexToInt(hexChars[6]));
                return new Color32(num4, num5, b, (byte) ((this.HexToInt(hexChars[7]) * 0x10) + this.HexToInt(hexChars[8])));
            }
            if (tagCount == 13)
            {
                byte num8 = (byte) ((this.HexToInt(hexChars[7]) * 0x10) + this.HexToInt(hexChars[8]));
                byte num9 = (byte) ((this.HexToInt(hexChars[9]) * 0x10) + this.HexToInt(hexChars[10]));
                return new Color32(num8, num9, (byte) ((this.HexToInt(hexChars[11]) * 0x10) + this.HexToInt(hexChars[12])), 0xff);
            }
            if (tagCount == 15)
            {
                byte num11 = (byte) ((this.HexToInt(hexChars[7]) * 0x10) + this.HexToInt(hexChars[8]));
                byte num12 = (byte) ((this.HexToInt(hexChars[9]) * 0x10) + this.HexToInt(hexChars[10]));
                byte num13 = (byte) ((this.HexToInt(hexChars[11]) * 0x10) + this.HexToInt(hexChars[12]));
                return new Color32(num11, num12, num13, (byte) ((this.HexToInt(hexChars[13]) * 0x10) + this.HexToInt(hexChars[14])));
            }
            return new Color32(0xff, 0xff, 0xff, 0xff);
        }

        private int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0':
                    return 0;

                case '1':
                    return 1;

                case '2':
                    return 2;

                case '3':
                    return 3;

                case '4':
                    return 4;

                case '5':
                    return 5;

                case '6':
                    return 6;

                case '7':
                    return 7;

                case '8':
                    return 8;

                case '9':
                    return 9;

                case 'A':
                    return 10;

                case 'B':
                    return 11;

                case 'C':
                    return 12;

                case 'D':
                    return 13;

                case 'E':
                    return 14;

                case 'F':
                    return 15;

                case 'a':
                    return 10;

                case 'b':
                    return 11;

                case 'c':
                    return 12;

                case 'd':
                    return 13;

                case 'e':
                    return 14;

                case 'f':
                    return 15;
            }
            return 15;
        }

        private void LateUpdate()
        {
            if (!this.haveFastScaleChanged && !this.m_fastScale.Compare(this.m_fastScalePreviews, 0x2710))
            {
                this.haveFastScaleChanged = true;
                this.m_fastScalePreviews = this.m_fastScale;
            }
            if (!this.haveColorChanged && !this.m_fontColor.Compare(this.m_fontColorPreviews))
            {
                this.haveColorChanged = true;
                this.m_fontColorPreviews = this.m_fontColor;
            }
        }

        private void LoadFontAsset()
        {
            ShaderUtilities.GetShaderPropertyIDs();
            if (this.m_fontAsset == null)
            {
                this.m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                if (this.m_fontAsset == null)
                {
                    Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".");
                    return;
                }
                if (this.m_fontAsset.characterDictionary == null)
                {
                    Debug.Log("Dictionary is Null!");
                }
                this.m_renderer.sharedMaterial = this.m_fontAsset.material;
                this.m_sharedMaterial = this.m_fontAsset.material;
                this.m_sharedMaterial.SetFloat("_CullMode", 0f);
                this.m_sharedMaterial.SetFloat("_ZTestMode", 4f);
                this.m_renderer.receiveShadows = false;
                this.m_renderer.castShadows = false;
            }
            else
            {
                if (this.m_fontAsset.characterDictionary == null)
                {
                    this.m_fontAsset.ReadFontDefinition();
                }
                if (((this.m_renderer.sharedMaterial == null) || (this.m_renderer.sharedMaterial.mainTexture == null)) || (this.m_fontAsset.atlas.GetInstanceID() != this.m_renderer.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID()))
                {
                    this.m_renderer.sharedMaterial = this.m_fontAsset.material;
                    this.m_sharedMaterial = this.m_fontAsset.material;
                }
                else
                {
                    this.m_sharedMaterial = this.m_renderer.sharedMaterial;
                }
                this.m_sharedMaterial.SetFloat("_ZTestMode", 4f);
                if (this.m_sharedMaterial.passCount > 1)
                {
                    this.m_renderer.receiveShadows = false;
                    this.m_renderer.castShadows = true;
                }
                else
                {
                    this.m_renderer.receiveShadows = false;
                    this.m_renderer.castShadows = false;
                }
            }
            this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
            this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
            this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
            this.m_sharedMaterials.Add(this.m_sharedMaterial);
        }

        private void OnDestroy()
        {
            if (this.m_mesh != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_mesh);
            }
        }

        private void OnDidApplyAnimationProperties()
        {
            this.havePropertiesChanged = true;
            this.isMaskUpdateRequired = true;
        }

        private void OnDisable()
        {
            TMPro_EventManager.OnPreRenderObject_Event -= new TMPro_EventManager.OnPreRenderObject_Event_Handler(this.OnPreRenderObject);
        }

        private void OnEnable()
        {
            if (this.m_meshFilter.sharedMesh == null)
            {
                this.m_meshFilter.mesh = this.m_mesh;
            }
            TMPro_EventManager.OnPreRenderObject_Event += new TMPro_EventManager.OnPreRenderObject_Event_Handler(this.OnPreRenderObject);
            this.haveFastScaleChanged = true;
            this.haveColorChanged = true;
        }

        private void OnPreRenderObject()
        {
        }

        private void OnWillRenderObject()
        {
            this.loopCountA = 0;
            if (this.m_fontAsset != null)
            {
                if (this.havePropertiesChanged || this.m_fontAsset.propertiesChanged)
                {
                    if (this.hasFontAssetChanged || this.m_fontAsset.propertiesChanged)
                    {
                        this.LoadFontAsset();
                        this.hasFontAssetChanged = false;
                        if ((this.m_fontAsset == null) || (this.m_renderer.sharedMaterial == null))
                        {
                            return;
                        }
                        this.m_fontAsset.propertiesChanged = false;
                    }
                    if (this.isMaskUpdateRequired)
                    {
                        this.UpdateMask();
                        this.isMaskUpdateRequired = false;
                    }
                    if (this.isInputParsingRequired || this.m_isTextTruncated)
                    {
                        this.ParseInputText();
                    }
                    if (this.m_enableAutoSizing)
                    {
                        this.m_fontSize = Mathf.Clamp(this.m_fontSize, this.m_fontSizeMin, this.m_fontSizeMax);
                    }
                    this.m_maxFontSize = this.m_fontSizeMax;
                    this.m_minFontSize = this.m_fontSizeMin;
                    this.m_lineSpacingDelta = 0f;
                    this.m_recursiveCount = 0;
                    this.m_isCharacterWrappingEnabled = false;
                    this.m_isTextTruncated = false;
                    this.GenerateTextMesh();
                    this.havePropertiesChanged = false;
                }
                else if ((this.haveColorChanged && !this.enableVertexGradient) || this.haveFastScaleChanged)
                {
                    this.GenerateFastScaleAndColor();
                }
            }
        }

        private Vector2 PackUV(float x, float y, float scale)
        {
            x = (x % 5f) / 5f;
            y = (y % 5f) / 5f;
            return new Vector2(Mathf.Round(x * 4096f) + y, scale);
        }

        private void ParseInputText()
        {
            this.isInputParsingRequired = false;
            switch (this.m_inputSource)
            {
                case TextInputSources.Text:
                    this.StringToCharArray(this.m_text, ref this.m_char_buffer);
                    break;

                case TextInputSources.SetText:
                    this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
                    break;
            }
        }

        private void Reset()
        {
            if (this.m_mesh != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_mesh);
            }
            this.Awake();
        }

        private void ResizeLineExtents(int size)
        {
            size = (size <= 0x400) ? Mathf.NextPowerOfTwo(size + 1) : (size + 0x100);
            TMP_LineInfo[] infoArray = new TMP_LineInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < this.m_textInfo.lineInfo.Length)
                {
                    infoArray[i] = this.m_textInfo.lineInfo[i];
                }
                else
                {
                    infoArray[i].lineExtents = new Extents(this.k_InfinityVector, -this.k_InfinityVector);
                    infoArray[i].ascender = -this.k_InfinityVector.x;
                    infoArray[i].descender = this.k_InfinityVector.x;
                }
            }
            this.m_textInfo.lineInfo = infoArray;
        }

        private void ResizeMeshBuffers(int size)
        {
            int newSize = size * 4;
            int num2 = size * 6;
            int num3 = this.m_vertices.Length / 4;
            Array.Resize<Vector3>(ref this.m_vertices, newSize);
            Array.Resize<Vector3>(ref this.m_normals, newSize);
            Array.Resize<Vector4>(ref this.m_tangents, newSize);
            Array.Resize<Color32>(ref this.m_vertColors, newSize);
            Array.Resize<Vector2>(ref this.m_uvs, newSize);
            Array.Resize<Vector2>(ref this.m_uv2s, newSize);
            Array.Resize<int>(ref this.m_triangles, num2);
            for (int i = num3; i < size; i++)
            {
                int num5 = i * 4;
                int num6 = i * 6;
                this.m_normals[0 + num5] = new Vector3(0f, 0f, -1f);
                this.m_normals[1 + num5] = new Vector3(0f, 0f, -1f);
                this.m_normals[2 + num5] = new Vector3(0f, 0f, -1f);
                this.m_normals[3 + num5] = new Vector3(0f, 0f, -1f);
                this.m_tangents[0 + num5] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[1 + num5] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[2 + num5] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[3 + num5] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_triangles[0 + num6] = 0 + num5;
                this.m_triangles[1 + num6] = 1 + num5;
                this.m_triangles[2 + num6] = 2 + num5;
                this.m_triangles[3 + num6] = 3 + num5;
                this.m_triangles[4 + num6] = 2 + num5;
                this.m_triangles[5 + num6] = 1 + num5;
            }
            this.m_mesh.vertices = this.m_vertices;
            this.m_mesh.normals = this.m_normals;
            this.m_mesh.tangents = this.m_tangents;
            this.m_mesh.triangles = this.m_triangles;
        }

        private int RestoreWordWrappingState(ref WordWrapState state)
        {
            this.m_textInfo.lineInfo[this.m_lineNumber] = state.lineInfo;
            this.m_textInfo = (state.textInfo == null) ? this.m_textInfo : state.textInfo;
            this.m_currentFontSize = state.currentFontSize;
            this.m_fontScale = state.fontScale;
            this.m_baselineOffset = state.baselineOffset;
            this.m_style = state.fontStyle;
            this.m_htmlColor = state.vertexColor;
            this.m_colorStackIndex = state.colorStackIndex;
            this.m_characterCount = state.total_CharacterCount + 1;
            this.m_visibleCharacterCount = state.visible_CharacterCount;
            this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
            this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
            this.m_meshExtents = state.meshExtents;
            this.m_xAdvance = state.xAdvance;
            this.m_maxAscender = state.maxAscender;
            this.m_maxDescender = state.maxDescender;
            this.m_preferredWidth = state.preferredWidth;
            this.m_preferredHeight = state.preferredHeight;
            this.m_lineNumber = state.lineNumber;
            this.m_lineOffset = state.lineOffset;
            this.m_maxFontScale = state.maxFontScale;
            return state.previous_WordBreak;
        }

        private void SaveWordWrappingState(ref WordWrapState state, int index, int count)
        {
            state.previous_WordBreak = index;
            state.total_CharacterCount = count;
            state.visible_CharacterCount = this.m_visibleCharacterCount;
            state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
            state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
            state.xAdvance = this.m_xAdvance;
            state.maxAscender = this.m_maxAscender;
            state.maxDescender = this.m_maxDescender;
            state.preferredWidth = this.m_preferredWidth;
            state.preferredHeight = this.m_preferredHeight;
            state.fontScale = this.m_fontScale;
            state.maxFontScale = this.m_maxFontScale;
            state.currentFontSize = this.m_currentFontSize;
            state.lineNumber = this.m_lineNumber;
            state.lineOffset = this.m_lineOffset;
            state.baselineOffset = this.m_baselineOffset;
            state.fontStyle = this.m_style;
            state.vertexColor = this.m_htmlColor;
            state.colorStackIndex = this.m_colorStackIndex;
            state.meshExtents = this.m_meshExtents;
            state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
            state.textInfo = this.m_textInfo;
        }

        private void ScheduleUpdate()
        {
        }

        private int SetArraySizes(int[] chars)
        {
            int size = 0;
            int num2 = 0;
            int endIndex = 0;
            this.m_isUsingBold = false;
            this.m_VisibleCharacters.Clear();
            for (int i = 0; chars[i] != 0; i++)
            {
                int num5 = chars[i];
                if ((this.m_isRichText && (num5 == 60)) && this.ValidateHtmlTag(chars, i + 1, out endIndex))
                {
                    i = endIndex;
                    if ((this.m_style & FontStyles.Underline) == FontStyles.Underline)
                    {
                        size += 3;
                    }
                    if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
                    {
                        this.m_isUsingBold = true;
                    }
                }
                else
                {
                    if (((num5 != 0x20) && (num5 != 9)) && ((num5 != 10) && (num5 != 13)))
                    {
                        size++;
                    }
                    this.m_VisibleCharacters.Add((char) ((ushort) num5));
                    num2++;
                }
            }
            if ((this.m_textInfo.characterInfo == null) || (num2 > this.m_textInfo.characterInfo.Length))
            {
                this.m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 <= 0x400) ? Mathf.NextPowerOfTwo(num2) : (num2 + 0x100)];
            }
            if ((size * 4) > this.m_vertices.Length)
            {
                if (this.m_isFirstAllocation)
                {
                    this.SetMeshArrays(size);
                    this.m_isFirstAllocation = false;
                    return num2;
                }
                this.SetMeshArrays((size <= 0x400) ? Mathf.NextPowerOfTwo(size) : (size + 0x100));
            }
            return num2;
        }

        public void SetCharArray(char[] charArray)
        {
            if ((charArray != null) && (charArray.Length != 0))
            {
                if (this.m_char_buffer.Length <= charArray.Length)
                {
                    int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
                    this.m_char_buffer = new int[num];
                }
                int index = 0;
                for (int i = 0; i < charArray.Length; i++)
                {
                    if ((charArray[i] == '\\') && (i < (charArray.Length - 1)))
                    {
                        int num4 = charArray[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                            {
                                this.m_char_buffer[index] = 13;
                                i++;
                                index++;
                                continue;
                            }
                            case 0x74:
                            {
                                this.m_char_buffer[index] = 9;
                                i++;
                                index++;
                                continue;
                            }
                        }
                        if (num4 == 110)
                        {
                            this.m_char_buffer[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    this.m_char_buffer[index] = charArray[i];
                    index++;
                }
                this.m_char_buffer[index] = 0;
                this.m_inputSource = TextInputSources.SetCharArray;
                this.havePropertiesChanged = true;
                this.isInputParsingRequired = true;
            }
        }

        private void SetCulling()
        {
            if (this.m_isCullingEnabled)
            {
                this.m_renderer.material.SetFloat("_CullMode", 2f);
            }
            else
            {
                this.m_renderer.material.SetFloat("_CullMode", 0f);
            }
        }

        private void SetFaceColor(Color32 color)
        {
            this.m_renderer.material.SetColor(ShaderUtilities.ID_FaceColor, (Color) color);
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.m_renderer.material;
            }
        }

        private void SetFontMaterial(Material mat)
        {
            if (this.m_renderer == null)
            {
                this.m_renderer = base.GetComponent<Renderer>();
            }
            this.m_renderer.material = mat;
            this.m_fontMaterial = this.m_renderer.material;
            this.m_sharedMaterial = this.m_fontMaterial;
            this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
            this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
        }

        private void SetMeshArrays(int size)
        {
            int num = size * 4;
            int num2 = size * 6;
            this.m_vertices = new Vector3[num];
            this.m_normals = new Vector3[num];
            this.m_tangents = new Vector4[num];
            this.m_uvs = new Vector2[num];
            this.m_uv2s = new Vector2[num];
            this.m_vertColors = new Color32[num];
            this.m_triangles = new int[num2];
            for (int i = 0; i < size; i++)
            {
                int num4 = i * 4;
                int num5 = i * 6;
                this.m_vertices[0 + num4] = Vector3.zero;
                this.m_vertices[1 + num4] = Vector3.zero;
                this.m_vertices[2 + num4] = Vector3.zero;
                this.m_vertices[3 + num4] = Vector3.zero;
                this.m_uvs[0 + num4] = Vector2.zero;
                this.m_uvs[1 + num4] = Vector2.zero;
                this.m_uvs[2 + num4] = Vector2.zero;
                this.m_uvs[3 + num4] = Vector2.zero;
                this.m_normals[0 + num4] = new Vector3(0f, 0f, -1f);
                this.m_normals[1 + num4] = new Vector3(0f, 0f, -1f);
                this.m_normals[2 + num4] = new Vector3(0f, 0f, -1f);
                this.m_normals[3 + num4] = new Vector3(0f, 0f, -1f);
                this.m_tangents[0 + num4] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[1 + num4] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[2 + num4] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_tangents[3 + num4] = new Vector4(-1f, 0f, 0f, 1f);
                this.m_triangles[0 + num5] = 0 + num4;
                this.m_triangles[1 + num5] = 1 + num4;
                this.m_triangles[2 + num5] = 2 + num4;
                this.m_triangles[3 + num5] = 3 + num4;
                this.m_triangles[4 + num5] = 2 + num4;
                this.m_triangles[5 + num5] = 1 + num4;
            }
            this.m_mesh.vertices = this.m_vertices;
            this.m_mesh.uv = this.m_uvs;
            this.m_mesh.normals = this.m_normals;
            this.m_mesh.tangents = this.m_tangents;
            this.m_mesh.triangles = this.m_triangles;
            this.m_mesh.bounds = this.m_default_bounds;
        }

        private void SetOutlineColor(Color32 color)
        {
            this.m_renderer.material.SetColor(ShaderUtilities.ID_OutlineColor, (Color) color);
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.m_renderer.material;
            }
        }

        private void SetOutlineThickness(float thickness)
        {
            thickness = Mathf.Clamp01(thickness);
            this.m_renderer.material.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.m_renderer.material;
            }
            this.m_fontMaterial = this.m_renderer.material;
        }

        private void SetPerspectiveCorrection()
        {
            if (this.m_isOrthographic)
            {
                this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
            }
            else
            {
                this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
            }
        }

        private void SetShaderType()
        {
            if (this.m_isOverlay)
            {
                this.m_renderer.material.SetFloat("_ZTestMode", 8f);
                this.m_renderer.material.renderQueue = 0xfa0;
                this.m_sharedMaterial = this.m_renderer.material;
            }
            else
            {
                this.m_renderer.material.SetFloat("_ZTestMode", 4f);
                this.m_renderer.material.renderQueue = -1;
                this.m_sharedMaterial = this.m_renderer.material;
            }
        }

        private void SetSharedFontMaterial(Material mat)
        {
            if (this.m_renderer == null)
            {
                this.m_renderer = base.GetComponent<Renderer>();
            }
            this.m_renderer.sharedMaterial = mat;
            this.m_sharedMaterial = this.m_renderer.sharedMaterial;
            this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
            this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
        }

        public void SetText(string text, float arg0)
        {
            this.SetText(text, arg0, 255f, 255f);
        }

        public void SetText(string text, float arg0, float arg1)
        {
            this.SetText(text, arg0, arg1, 255f);
        }

        public void SetText(string text, float arg0, float arg1, float arg2)
        {
            if (((text != this.old_text) || (arg0 != this.old_arg0)) || ((arg1 != this.old_arg1) || (arg2 != this.old_arg2)))
            {
                if (this.m_input_CharArray.Length < text.Length)
                {
                    this.m_input_CharArray = new char[Mathf.NextPowerOfTwo(text.Length + 1)];
                }
                this.old_text = text;
                this.old_arg1 = 255f;
                this.old_arg2 = 255f;
                int precision = 0;
                int index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    char ch = text[i];
                    if (ch == '{')
                    {
                        if (text[i + 2] == ':')
                        {
                            precision = text[i + 3] - '0';
                        }
                        switch ((text[i + 1] - '0'))
                        {
                            case 0:
                                this.old_arg0 = arg0;
                                this.AddFloatToCharArray(arg0, ref index, precision);
                                break;

                            case 1:
                                this.old_arg1 = arg1;
                                this.AddFloatToCharArray(arg1, ref index, precision);
                                break;

                            case 2:
                                this.old_arg2 = arg2;
                                this.AddFloatToCharArray(arg2, ref index, precision);
                                break;
                        }
                        if (text[i + 2] == ':')
                        {
                            i += 4;
                        }
                        else
                        {
                            i += 2;
                        }
                        continue;
                    }
                    this.m_input_CharArray[index] = ch;
                    index++;
                }
                this.m_input_CharArray[index] = '\0';
                this.m_charArray_Length = index;
                this.m_inputSource = TextInputSources.SetText;
                this.isInputParsingRequired = true;
                this.havePropertiesChanged = true;
            }
        }

        private void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
        {
            if ((charArray != null) && (this.m_charArray_Length != 0))
            {
                if (charBuffer.Length <= this.m_charArray_Length)
                {
                    int num = (this.m_charArray_Length <= 0x400) ? Mathf.NextPowerOfTwo(this.m_charArray_Length + 1) : (this.m_charArray_Length + 0x100);
                    charBuffer = new int[num];
                }
                int index = 0;
                for (int i = 0; i < this.m_charArray_Length; i++)
                {
                    if ((charArray[i] == '\\') && (i < (this.m_charArray_Length - 1)))
                    {
                        int num4 = charArray[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                            {
                                charBuffer[index] = 13;
                                i++;
                                index++;
                                continue;
                            }
                            case 0x74:
                            {
                                charBuffer[index] = 9;
                                i++;
                                index++;
                                continue;
                            }
                        }
                        if (num4 == 110)
                        {
                            charBuffer[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    charBuffer[index] = charArray[i];
                    index++;
                }
                charBuffer[index] = 0;
            }
        }

        private void StringToCharArray(string text, ref int[] chars)
        {
            if (text != null)
            {
                if (chars.Length <= text.Length)
                {
                    int num = (text.Length <= 0x400) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 0x100);
                    chars = new int[num];
                }
                int index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if ((text[i] == '\\') && (i < (text.Length - 1)))
                    {
                        int num4 = text[i + 1];
                        switch (num4)
                        {
                            case 0x72:
                            {
                                chars[index] = 13;
                                i++;
                                index++;
                                continue;
                            }
                            case 0x74:
                            {
                                chars[index] = 9;
                                i++;
                                index++;
                                continue;
                            }
                        }
                        if (num4 == 110)
                        {
                            chars[index] = 10;
                            i++;
                            index++;
                            continue;
                        }
                    }
                    chars[index] = text[i];
                    index++;
                }
                chars[index] = 0;
            }
        }

        private void UpdateEnvMapMatrix()
        {
            if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && (this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) != null))
            {
                Vector3 euler = (Vector3) this.m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
                this.m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
                this.m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, this.m_EnvMapMatrix);
            }
        }

        public void UpdateFontAsset()
        {
            this.LoadFontAsset();
        }

        private void UpdateMask()
        {
            if (this.m_isMaskingEnabled)
            {
                if (this.m_isMaskingEnabled && (this.m_fontMaterial == null))
                {
                    this.CreateMaterialInstance();
                }
                float num = Mathf.Min(Mathf.Min(this.m_textContainer.margins.x, this.m_textContainer.margins.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
                float num2 = Mathf.Min(Mathf.Min(this.m_textContainer.margins.y, this.m_textContainer.margins.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
                num = (num <= 0f) ? 0f : num;
                num2 = (num2 <= 0f) ? 0f : num2;
                float z = (((this.m_textContainer.width - Mathf.Max(this.m_textContainer.margins.x, 0f)) - Mathf.Max(this.m_textContainer.margins.z, 0f)) / 2f) + num;
                float w = (((this.m_textContainer.height - Mathf.Max(this.m_textContainer.margins.y, 0f)) - Mathf.Max(this.m_textContainer.margins.w, 0f)) / 2f) + num2;
                Vector2 vector = new Vector2(((0.5f - this.m_textContainer.pivot.x) * this.m_textContainer.width) + ((Mathf.Max(this.m_textContainer.margins.x, 0f) - Mathf.Max(this.m_textContainer.margins.z, 0f)) / 2f), ((0.5f - this.m_textContainer.pivot.y) * this.m_textContainer.height) + ((-Mathf.Max(this.m_textContainer.margins.y, 0f) + Mathf.Max(this.m_textContainer.margins.w, 0f)) / 2f));
                Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
                this.m_fontMaterial.SetVector(ShaderUtilities.ID_MaskCoord, vector2);
                this.m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessX, num);
                this.m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessY, num2);
            }
        }

        private void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
        {
        }

        public void UpdateMeshPadding()
        {
            this.m_padding = ShaderUtilities.GetPadding(this.m_renderer.sharedMaterials, this.m_enableExtraPadding, this.m_isUsingBold);
            this.havePropertiesChanged = true;
        }

        private void UpdateSDFScale(float prevScale, float newScale)
        {
            for (int i = 0; i < this.m_uv2s.Length; i++)
            {
                this.m_uv2s[i].y = (this.m_uv2s[i].y / prevScale) * newScale;
            }
            this.m_mesh.uv2 = this.m_uv2s;
        }

        private bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
        {
            Array.Clear(this.m_htmlTag, 0, this.m_htmlTag.Length);
            int index = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int decimalPointIndex = 0;
            endIndex = startIndex;
            bool flag = false;
            int num7 = 1;
            for (int i = startIndex; ((chars[i] != 0) && (index < this.m_htmlTag.Length)) && (chars[i] != 60); i++)
            {
                if (chars[i] == 0x3e)
                {
                    flag = true;
                    endIndex = i;
                    this.m_htmlTag[index] = '\0';
                    break;
                }
                this.m_htmlTag[index] = (char) chars[i];
                index++;
                if (chars[i] == 0x3d)
                {
                    num7 = 0;
                }
                num2 += (chars[i] * index) * num7;
                num3 += (chars[i] * index) * (1 - num7);
                switch (chars[i])
                {
                    case 0x2e:
                        decimalPointIndex = index - 1;
                        break;

                    case 0x3d:
                        num4 = index;
                        break;
                }
            }
            if (flag)
            {
                float num9;
                if ((this.m_htmlTag[0] == '#') && (index == 7))
                {
                    this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                    this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                    this.m_colorStackIndex++;
                    return true;
                }
                if ((this.m_htmlTag[0] == '#') && (index == 9))
                {
                    this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                    this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                    this.m_colorStackIndex++;
                    return true;
                }
                switch (num2)
                {
                    case 0x73:
                        this.m_style |= FontStyles.Strikethrough;
                        return true;

                    case 0x75:
                        this.m_style |= FontStyles.Underline;
                        return true;

                    case 0xf1:
                        return true;

                    case 0xf3:
                        if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
                        {
                            this.m_style &= ~FontStyles.Bold;
                        }
                        return true;

                    case 0x3fb:
                        if (this.m_overflowMode == TextOverflowModes.Page)
                        {
                            this.m_xAdvance = 0f + this.m_indent;
                            this.m_lineOffset = 0f;
                            this.m_pageNumber++;
                            this.m_isNewPage = true;
                        }
                        return true;

                    case 0x3fc:
                        this.m_currentFontSize /= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_baselineOffset = 0f;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        return true;

                    case 0x62:
                        this.m_style |= FontStyles.Bold;
                        return true;

                    case 0x69:
                        this.m_style |= FontStyles.Italic;
                        return true;

                    case 0x101:
                        this.m_style &= ~FontStyles.Italic;
                        return true;

                    case 0x115:
                        if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
                        {
                            this.m_style &= ~FontStyles.Strikethrough;
                        }
                        return true;

                    case 0x119:
                        if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
                        {
                            this.m_style &= ~FontStyles.Underline;
                        }
                        return true;

                    case 0x283:
                        this.m_currentFontSize *= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SubscriptOffset * this.m_fontScale;
                        return true;

                    case 0x2a7:
                        num9 = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_xAdvance = (num9 * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        return true;

                    case 0x2ad:
                        this.m_currentFontSize *= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SuperscriptOffset * this.m_fontScale;
                        return true;

                    case 0x434:
                        this.m_currentFontSize /= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_baselineOffset = 0f;
                        this.m_fontScale = (this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize) * (!this.m_isOrthographic ? 0.1f : 1f);
                        return true;

                    case 0x447:
                    {
                        num5 = index - 1;
                        float num10 = 0f;
                        if (this.m_htmlTag[5] == '%')
                        {
                            num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                            this.m_currentFontSize = (this.m_fontSize * num10) / 100f;
                            this.m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        if (this.m_htmlTag[5] == '+')
                        {
                            num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                            this.m_currentFontSize = this.m_fontSize + num10;
                            this.m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        if (this.m_htmlTag[5] == '-')
                        {
                            num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                            this.m_currentFontSize = this.m_fontSize + num10;
                            this.m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        num10 = this.ConvertToFloat(this.m_htmlTag, num4, num5, decimalPointIndex);
                        if (num10 == 73493f)
                        {
                            return false;
                        }
                        this.m_currentFontSize = num10;
                        this.m_isRecalculateScaleRequired = true;
                        return true;
                    }
                    case 0x45e:
                        Debug.Log("Font Tag used.");
                        return true;

                    case 0x5fb:
                        num9 = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_xAdvance += (num9 * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        return true;

                    case 0x60e:
                        this.m_htmlColor.a = (byte) ((this.HexToInt(this.m_htmlTag[7]) * 0x10) + this.HexToInt(this.m_htmlTag[8]));
                        return true;

                    case 0x631:
                        this.m_currentFontSize = this.m_fontSize;
                        this.m_isRecalculateScaleRequired = true;
                        return true;

                    case 0x636:
                        switch (num3)
                        {
                            case 0xfa8:
                                this.m_lineJustification = TextAlignmentOptions.Left;
                                return true;

                            case 0x147f:
                                this.m_lineJustification = TextAlignmentOptions.Right;
                                return true;

                            case 0x1960:
                                this.m_lineJustification = TextAlignmentOptions.Center;
                                return true;

                            case 0x2a91:
                                this.m_lineJustification = TextAlignmentOptions.Justified;
                                return true;
                        }
                        return false;

                    case 0x67b:
                        if ((this.m_htmlTag[6] == '#') && (index == 13))
                        {
                            this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                            this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                            this.m_colorStackIndex++;
                            return true;
                        }
                        if ((this.m_htmlTag[6] == '#') && (index == 15))
                        {
                            this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, index);
                            this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                            this.m_colorStackIndex++;
                            return true;
                        }
                        switch (num3)
                        {
                            case 0xb38:
                                this.m_htmlColor = Color.red;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0xf8b:
                                this.m_htmlColor = Color.blue;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x135c:
                                this.m_htmlColor = Color.black;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x1408:
                                this.m_htmlColor = Color.green;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x147f:
                                this.m_htmlColor = Color.white;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x18e5:
                                this.m_htmlColor = new Color32(0xff, 0x80, 0, 0xff);
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x19e8:
                                this.m_htmlColor = new Color32(160, 0x20, 240, 0xff);
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;

                            case 0x1a42:
                                this.m_htmlColor = Color.yellow;
                                this.m_colorStack[this.m_colorStackIndex] = this.m_htmlColor;
                                this.m_colorStackIndex++;
                                return true;
                        }
                        return false;

                    case 0x7ee:
                        return true;

                    case 0x86a:
                        this.m_cSpacing = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x870:
                        this.m_lineJustification = this.m_textAlignment;
                        return true;

                    case 0x874:
                        this.m_monoSpacing = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x8c9:
                        this.m_colorStackIndex--;
                        if (this.m_colorStackIndex <= 0)
                        {
                            this.m_htmlColor = this.m_fontColor32;
                            this.m_colorStackIndex = 0;
                        }
                        else
                        {
                            this.m_htmlColor = this.m_colorStack[this.m_colorStackIndex - 1];
                        }
                        return true;

                    case 0x8e3:
                        this.m_indent = (this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex) * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        this.m_xAdvance = this.m_indent;
                        return true;

                    case 0x8ef:
                        Debug.Log("Sprite Tag used.");
                        return true;

                    case 0xb08:
                        this.m_cSpacing = 0f;
                        return true;

                    case 0xb1c:
                        this.m_monoSpacing = 0f;
                        return true;

                    case 0xb94:
                        this.m_indent = 0f;
                        return true;

                    case 0xbb3:
                        this.m_style |= FontStyles.UpperCase;
                        return true;

                    case 0xec2:
                        this.m_style &= ~FontStyles.UpperCase;
                        return true;

                    case 0x12c0:
                        this.m_style |= FontStyles.SmallCaps;
                        return true;

                    case 0x16af:
                        this.m_currentFontSize = this.m_fontSize;
                        this.m_style &= ~FontStyles.SmallCaps;
                        this.m_isRecalculateScaleRequired = true;
                        return true;

                    case 0x1a23:
                        this.m_lineHeight = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

                    case 0x1ea0:
                        this.m_lineHeight = 0f;
                        return true;
                }
            }
            return false;
        }

        public TextAlignmentOptions alignment
        {
            get
            {
                return this.m_textAlignment;
            }
            set
            {
                if (this.m_textAlignment != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_textAlignment = value;
                }
            }
        }

        [Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
        public TMP_Compatibility.AnchorPositions anchor
        {
            get
            {
                return this.m_anchor;
            }
        }

        public bool anchorDampening
        {
            get
            {
                return this.m_anchorDampening;
            }
            set
            {
                if (this.m_anchorDampening != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_anchorDampening = value;
                }
            }
        }

        public Bounds bounds
        {
            get
            {
                if (this.m_mesh != null)
                {
                    return this.m_mesh.bounds;
                }
                return new Bounds();
            }
        }

        public float characterSpacing
        {
            get
            {
                return this.m_characterSpacing;
            }
            set
            {
                if (this.m_characterSpacing != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_characterSpacing = value;
                }
            }
        }

        public Color color
        {
            get
            {
                return this.m_fontColor;
            }
            set
            {
                if (!this.m_fontColor.Compare(value))
                {
                    this.haveColorChanged = true;
                    this.m_fontColor = value;
                }
            }
        }

        public VertexGradient colorGradient
        {
            get
            {
                return this.m_fontColorGradient;
            }
            set
            {
                this.havePropertiesChanged = true;
                this.m_fontColorGradient = value;
            }
        }

        public bool enableAutoSizing
        {
            get
            {
                return this.m_enableAutoSizing;
            }
            set
            {
                this.m_enableAutoSizing = value;
            }
        }

        public bool enableCulling
        {
            get
            {
                return this.m_isCullingEnabled;
            }
            set
            {
                this.m_isCullingEnabled = value;
                this.SetCulling();
                this.havePropertiesChanged = true;
            }
        }

        public bool enableKerning
        {
            get
            {
                return this.m_enableKerning;
            }
            set
            {
                if (this.m_enableKerning != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_enableKerning = value;
                }
            }
        }

        public bool enableVertexGradient
        {
            get
            {
                return this.m_enableVertexGradient;
            }
            set
            {
                this.havePropertiesChanged = true;
                this.m_enableVertexGradient = value;
            }
        }

        public bool enableWordWrapping
        {
            get
            {
                return this.m_enableWordWrapping;
            }
            set
            {
                if (this.m_enableWordWrapping != value)
                {
                    this.havePropertiesChanged = true;
                    this.isInputParsingRequired = true;
                    this.m_enableWordWrapping = value;
                }
            }
        }

        public bool extraPadding
        {
            get
            {
                return this.m_enableExtraPadding;
            }
            set
            {
                if (this.m_enableExtraPadding != value)
                {
                    this.havePropertiesChanged = true;
                    this.checkPaddingRequired = true;
                    this.m_enableExtraPadding = value;
                }
            }
        }

        public Color32 faceColor
        {
            get
            {
                return this.m_faceColor;
            }
            set
            {
                if (!this.m_faceColor.Compare(value))
                {
                    this.SetFaceColor(value);
                    this.havePropertiesChanged = true;
                    this.m_faceColor = value;
                }
            }
        }

        public Vector3 fastScale
        {
            get
            {
                return this.m_fastScale;
            }
            set
            {
                if (!this.m_fastScale.Compare(value, 0x2710))
                {
                    this.haveFastScaleChanged = true;
                    this.m_fastScale = value;
                }
            }
        }

        public TextMeshProFont font
        {
            get
            {
                return this.m_fontAsset;
            }
            set
            {
                if (this.m_fontAsset != value)
                {
                    this.m_fontAsset = value;
                    this.LoadFontAsset();
                    this.havePropertiesChanged = true;
                }
            }
        }

        public Material fontMaterial
        {
            get
            {
                if (this.m_fontMaterial == null)
                {
                    this.SetFontMaterial(this.m_sharedMaterial);
                    return this.m_sharedMaterial;
                }
                return this.m_sharedMaterial;
            }
            set
            {
                this.SetFontMaterial(value);
                this.havePropertiesChanged = true;
            }
        }

        public float fontScale
        {
            get
            {
                return this.m_fontScale;
            }
        }

        public Material fontSharedMaterial
        {
            get
            {
                return this.m_renderer.sharedMaterial;
            }
            set
            {
                if (this.m_sharedMaterial != value)
                {
                    this.SetSharedFontMaterial(value);
                    this.havePropertiesChanged = true;
                }
            }
        }

        public float fontSize
        {
            get
            {
                return this.m_fontSize;
            }
            set
            {
                if (this.m_fontSize != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_fontSize = value;
                }
            }
        }

        public float fontSizeMax
        {
            get
            {
                return this.m_fontSizeMax;
            }
            set
            {
                this.m_fontSizeMax = value;
            }
        }

        public float fontSizeMin
        {
            get
            {
                return this.m_fontSizeMin;
            }
            set
            {
                this.m_fontSizeMin = value;
            }
        }

        public FontStyles fontStyle
        {
            get
            {
                return this.m_fontStyle;
            }
            set
            {
                this.m_fontStyle = value;
                this.havePropertiesChanged = true;
                this.checkPaddingRequired = true;
            }
        }

        public bool hasChanged
        {
            get
            {
                return this.havePropertiesChanged;
            }
            set
            {
                this.havePropertiesChanged = value;
            }
        }

        public TextureMappingOptions horizontalMapping
        {
            get
            {
                return this.m_horizontalMapping;
            }
            set
            {
                if (this.m_horizontalMapping != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_horizontalMapping = value;
                }
            }
        }

        public bool ignoreVisibility
        {
            get
            {
                return this.m_ignoreCulling;
            }
            set
            {
                if (this.m_ignoreCulling != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_ignoreCulling = value;
                }
            }
        }

        public bool isOrthographic
        {
            get
            {
                return this.m_isOrthographic;
            }
            set
            {
                this.havePropertiesChanged = true;
                this.m_isOrthographic = value;
            }
        }

        public bool isOverlay
        {
            get
            {
                return this.m_isOverlay;
            }
            set
            {
                this.m_isOverlay = value;
                this.SetShaderType();
                this.havePropertiesChanged = true;
            }
        }

        [Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
        public float lineLength
        {
            get
            {
                return this.m_lineLength;
            }
            set
            {
                Debug.Log("lineLength set called.");
            }
        }

        public float lineSpacing
        {
            get
            {
                return this.m_lineSpacing;
            }
            set
            {
                if (this.m_lineSpacing != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_lineSpacing = value;
                }
            }
        }

        public MaskingTypes maskType
        {
            get
            {
                return this.m_maskType;
            }
            set
            {
                this.m_maskType = value;
                this.havePropertiesChanged = true;
                this.isMaskUpdateRequired = true;
            }
        }

        public int maxVisibleCharacters
        {
            get
            {
                return this.m_maxVisibleCharacters;
            }
            set
            {
                if (this.m_maxVisibleCharacters != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_maxVisibleCharacters = value;
                }
            }
        }

        public int maxVisibleLines
        {
            get
            {
                return this.m_maxVisibleLines;
            }
            set
            {
                if (this.m_maxVisibleLines != value)
                {
                    this.havePropertiesChanged = true;
                    this.isInputParsingRequired = true;
                    this.m_maxVisibleLines = value;
                }
            }
        }

        public Mesh mesh
        {
            get
            {
                return this.m_mesh;
            }
        }

        public Color32 outlineColor
        {
            get
            {
                return this.m_outlineColor;
            }
            set
            {
                if (!this.m_outlineColor.Compare(value))
                {
                    this.SetOutlineColor(value);
                    this.havePropertiesChanged = true;
                    this.m_outlineColor = value;
                }
            }
        }

        public float outlineWidth
        {
            get
            {
                return this.m_outlineWidth;
            }
            set
            {
                this.SetOutlineThickness(value);
                this.havePropertiesChanged = true;
                this.checkPaddingRequired = true;
                this.m_outlineWidth = value;
            }
        }

        public TextOverflowModes OverflowMode
        {
            get
            {
                return this.m_overflowMode;
            }
            set
            {
                this.m_overflowMode = value;
                this.havePropertiesChanged = true;
            }
        }

        public bool overrideColorTags
        {
            get
            {
                return this.m_overrideHtmlColors;
            }
            set
            {
                if (this.m_overrideHtmlColors != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_overrideHtmlColors = value;
                }
            }
        }

        public int pageToDisplay
        {
            get
            {
                return this.m_pageToDisplay;
            }
            set
            {
                this.havePropertiesChanged = true;
                this.m_pageToDisplay = value;
            }
        }

        public float paragraphSpacing
        {
            get
            {
                return this.m_paragraphSpacing;
            }
            set
            {
                if (this.m_paragraphSpacing != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_paragraphSpacing = value;
                }
            }
        }

        public float preferredWidth
        {
            get
            {
                return this.m_preferredWidth;
            }
        }

        public TextRenderFlags renderMode
        {
            get
            {
                return this.m_renderMode;
            }
            set
            {
                this.m_renderMode = value;
                this.havePropertiesChanged = true;
            }
        }

        public bool richText
        {
            get
            {
                return this.m_isRichText;
            }
            set
            {
                this.m_isRichText = value;
                this.havePropertiesChanged = true;
                this.isInputParsingRequired = true;
            }
        }

        public int sortingLayerID
        {
            get
            {
                return this.m_renderer.sortingLayerID;
            }
            set
            {
                this.m_renderer.sortingLayerID = value;
            }
        }

        public int sortingOrder
        {
            get
            {
                return this.m_renderer.sortingOrder;
            }
            set
            {
                this.m_renderer.sortingOrder = value;
            }
        }

        public Vector2[] spacePositions
        {
            get
            {
                return this.m_spacePositions;
            }
        }

        public string text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_inputSource = TextInputSources.Text;
                this.havePropertiesChanged = true;
                this.isInputParsingRequired = true;
                this.m_text = value;
            }
        }

        public TextContainer textContainer
        {
            get
            {
                if (this.m_textContainer == null)
                {
                    this.m_textContainer = base.GetComponent<TextContainer>();
                }
                return this.m_textContainer;
            }
        }

        public TMP_TextInfo textInfo
        {
            get
            {
                return this.m_textInfo;
            }
        }

        public TextureMappingOptions verticalMapping
        {
            get
            {
                return this.m_verticalMapping;
            }
            set
            {
                if (this.m_verticalMapping != value)
                {
                    this.havePropertiesChanged = true;
                    this.m_verticalMapping = value;
                }
            }
        }

        private enum TextInputSources
        {
            Text,
            SetText,
            SetCharArray
        }
    }
}

