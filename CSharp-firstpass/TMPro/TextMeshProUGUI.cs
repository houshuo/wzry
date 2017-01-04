namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [AddComponentMenu("UI/TextMeshPro Text", 12), ExecuteInEditMode, RequireComponent(typeof(CanvasRenderer)), RequireComponent(typeof(RectTransform)), DisallowMultipleComponent]
    public class TextMeshProUGUI : Graphic, ILayoutElement, IMaskable
    {
        [SerializeField]
        private bool checkPaddingRequired;
        [SerializeField]
        private bool hasFontAssetChanged;
        [SerializeField]
        private bool havePropertiesChanged;
        [SerializeField]
        private bool isInputParsingRequired;
        private bool IsRectTransformDriven;
        private Vector2 k_InfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        private readonly float[] k_Power = new float[] { 0.5f, 0.05f, 0.005f, 0.0005f, 5E-05f, 5E-06f, 5E-07f, 5E-08f, 5E-09f, 5E-10f };
        private int loopCountA;
        private Vector4 m_alignmentPadding;
        private Vector3 m_anchorOffset;
        private float m_baselineOffset;
        [SerializeField]
        private Material m_baseMaterial;
        private Bounds m_bounds;
        private GlyphInfo m_cached_GlyphInfo;
        private GlyphInfo m_cached_Underline_GlyphInfo;
        private Canvas m_canvas;
        private int[] m_char_buffer;
        private int m_characterCount;
        private List<TMP_CharacterInfo> m_characterInfoList = new List<TMP_CharacterInfo>(0x100);
        [SerializeField]
        private float m_characterSpacing;
        private int m_charArray_Length;
        [SerializeField]
        private float m_charSpacingMax;
        private Color32[] m_colorStack = new Color32[0x20];
        private int m_colorStackIndex;
        private float m_cSpacing;
        private bool m_currentAutoSizeMode;
        private float m_currentFontSize;
        private TextOverflowModes m_currentOverflowMode;
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
        private int m_firstVisibleCharacterOfLine;
        private float m_flexibleHeight;
        private float m_flexibleWidth;
        [SerializeField]
        private TextMeshProFont m_fontAsset;
        [SerializeField]
        private TextMeshProFont[] m_fontAssetArray;
        [SerializeField]
        private Color m_fontColor = Color.white;
        [SerializeField]
        private Color32 m_fontColor32 = Color.white;
        [SerializeField]
        private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);
        private int m_fontIndex;
        [SerializeField]
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
        private char[] m_htmlTag = new char[0x10];
        [SerializeField]
        private bool m_ignoreCulling = true;
        private float m_indent;
        private InlineGraphicManager m_inlineGraphics;
        private char[] m_input_CharArray = new char[0x100];
        [SerializeField]
        private TextInputSources m_inputSource;
        private bool m_isAwake;
        private bool m_isCalculateSizeRequired;
        private bool m_isCharacterWrappingEnabled;
        [SerializeField]
        private bool m_isCullingEnabled;
        private bool m_isEnabled;
        private bool m_isFirstAllocation;
        private bool m_isLayoutDirty;
        private bool m_isMaskingEnabled;
        [SerializeField]
        private bool m_isNewBaseMaterial;
        private bool m_isNewPage;
        [SerializeField]
        private bool m_isOrthographic;
        private bool m_isOverlay;
        private bool m_isRebuildingLayout;
        private bool m_isRecalculateScaleRequired;
        [SerializeField]
        private bool m_isRichText = true;
        private bool m_isScrollRegionSet;
        private bool m_isSprite;
        private bool m_isStencilUpdateRequired;
        private bool m_isTextTruncated;
        private bool m_isUsingBold;
        private Material m_lastBaseMaterial;
        private int m_lastVisibleCharacterOfLine;
        private ILayoutController m_layoutController;
        private AutoLayoutPhase m_LayoutPhase;
        private int m_layoutPriority;
        private float m_lineHeight;
        private TextAlignmentOptions m_lineJustification;
        private int m_lineNumber;
        private float m_lineOffset;
        [SerializeField]
        private float m_lineSpacing;
        private float m_lineSpacingDelta;
        [SerializeField]
        private float m_lineSpacingMax;
        [SerializeField]
        private Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);
        private float m_marginHeight;
        private bool m_marginsHaveChanged;
        private float m_marginWidth;
        private Material m_maskingMaterial;
        [SerializeField]
        private Vector4 m_maskOffset;
        private int m_max_characters = 8;
        private int m_max_numberOfLines = 4;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        private float m_maxFontSize;
        private int m_maxVisibleCharacters = -1;
        private int m_maxVisibleLines = -1;
        private float m_maxXAdvance;
        private int[] m_meshAllocCount = new int[0x11];
        private Extents m_meshExtents;
        private float m_minFontSize;
        private float m_minHeight;
        private float m_minWidth;
        private float m_monoSpacing;
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
        private float m_preferredHeight = 9999f;
        private float m_preferredWidth = 9999f;
        private Vector3 m_previousLossyScale;
        private Vector3[] m_rectCorners = new Vector3[4];
        private RectTransform m_rectTransform;
        [SerializeField]
        private bool m_rectTransformDimensionsChanged;
        private int m_recursiveCount;
        private float m_renderedHeight;
        private float m_renderedWidth;
        private TextRenderFlags m_renderMode;
        private WordWrapState m_SavedLineState;
        private WordWrapState m_SavedWordWrapState;
        private int m_selectedFontAsset;
        [SerializeField]
        private Material m_sharedMaterial;
        private List<Material> m_sharedMaterials = new List<Material>(0x10);
        [SerializeField]
        private int m_sortingLayerID;
        [SerializeField]
        private int m_sortingOrder;
        private Vector2[] m_spacePositions = new Vector2[8];
        private float m_spacing;
        private int m_spriteCount;
        private int m_spriteIndex;
        private int m_stencilID;
        private FontStyles m_style;
        private float m_tabSpacing;
        [SerializeField]
        private string m_text;
        [FormerlySerializedAs("m_lineJustification"), SerializeField]
        private TextAlignmentOptions m_textAlignment;
        private TMP_TextInfo m_textInfo;
        private CanvasRenderer m_uiRenderer;
        [SerializeField]
        private UIVertex[] m_uiVertices;
        [SerializeField]
        private float m_uvLineOffset;
        [SerializeField]
        private Vector2 m_uvOffset = Vector2.zero;
        [SerializeField]
        private TextureMappingOptions m_verticalMapping;
        private int m_visibleCharacterCount;
        private List<char> m_VisibleCharacters = new List<char>();
        private int m_visibleSpriteCount;
        private float m_width;
        [SerializeField]
        private float m_wordWrappingRatios = 0.4f;
        private float m_xAdvance;
        private float old_arg0;
        private float old_arg1;
        private float old_arg2;
        private string old_text;
        private GameObject[] subObjects = new GameObject[0x11];

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
                    this.m_textInfo.characterInfo[i].vertex_BL.position = this.m_textInfo.characterInfo[i].bottomLeft -= vector;
                    this.m_textInfo.characterInfo[i].vertex_TL.position = this.m_textInfo.characterInfo[i].topLeft -= vector;
                    this.m_textInfo.characterInfo[i].vertex_TR.position = this.m_textInfo.characterInfo[i].topRight -= vector;
                    this.m_textInfo.characterInfo[i].vertex_BR.position = this.m_textInfo.characterInfo[i].bottomRight -= vector;
                    this.m_textInfo.characterInfo[i].bottomLine -= vector.y;
                    this.m_textInfo.characterInfo[i].topLine -= vector.y;
                }
            }
        }

        protected override void Awake()
        {
            this.m_isAwake = true;
            this.m_canvas = base.GetComponentInParent(typeof(Canvas)) as Canvas;
            this.m_rectTransform = base.gameObject.GetComponent<RectTransform>();
            if (this.m_rectTransform == null)
            {
                this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
            }
            this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
            if (this.m_uiRenderer == null)
            {
                this.m_uiRenderer = base.gameObject.AddComponent<CanvasRenderer>();
            }
            ILayoutController component = base.GetComponent(typeof(ILayoutController)) as ILayoutController;
            if (component == null)
            {
            }
            this.m_layoutController = (base.transform.parent == null) ? null : (base.transform.parent.GetComponent(typeof(ILayoutController)) as ILayoutController);
            if (this.m_layoutController != null)
            {
                this.IsRectTransformDriven = true;
            }
            this.LoadFontAsset();
            this.m_char_buffer = new int[this.m_max_characters];
            this.m_cached_GlyphInfo = new GlyphInfo();
            this.m_uiVertices = new UIVertex[0];
            this.m_isFirstAllocation = true;
            this.m_textInfo = new TMP_TextInfo();
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
                this.m_rectTransformDimensionsChanged = true;
                this.ForceMeshUpdate();
            }
        }

        public void CalculateLayoutInputHorizontal()
        {
            if (base.gameObject.activeInHierarchy)
            {
                this.m_currentAutoSizeMode = this.m_enableAutoSizing;
                this.m_LayoutPhase = AutoLayoutPhase.Horizontal;
                this.m_isRebuildingLayout = true;
                this.m_minWidth = 0f;
                this.m_flexibleWidth = 0f;
                this.m_renderMode = TextRenderFlags.GetPreferredSizes;
                if (this.m_enableAutoSizing)
                {
                    this.m_fontSize = this.m_fontSizeMax;
                }
                this.m_marginWidth = float.PositiveInfinity;
                this.m_marginHeight = float.PositiveInfinity;
                if (this.isInputParsingRequired || this.m_isTextTruncated)
                {
                    this.ParseInputText();
                }
                this.GenerateTextMesh();
                this.m_renderMode = TextRenderFlags.Render;
                this.m_preferredWidth = this.m_renderedWidth;
                this.ComputeMarginSize();
                this.m_isLayoutDirty = true;
            }
        }

        public void CalculateLayoutInputVertical()
        {
            if (base.gameObject.activeInHierarchy)
            {
                this.m_LayoutPhase = AutoLayoutPhase.Vertical;
                this.m_isRebuildingLayout = true;
                this.m_minHeight = 0f;
                this.m_flexibleHeight = 0f;
                this.m_renderMode = TextRenderFlags.GetPreferredSizes;
                if (this.m_enableAutoSizing)
                {
                    this.m_currentAutoSizeMode = true;
                    this.m_enableAutoSizing = false;
                }
                this.m_marginHeight = float.PositiveInfinity;
                this.GenerateTextMesh();
                this.m_enableAutoSizing = this.m_currentAutoSizeMode;
                this.m_renderMode = TextRenderFlags.Render;
                this.ComputeMarginSize();
                this.m_preferredHeight = this.m_renderedHeight;
                this.m_isLayoutDirty = true;
                this.m_isCalculateSizeRequired = false;
            }
        }

        private void ComputeMarginSize()
        {
            if (this.m_rectTransform != null)
            {
                if (this.m_rectTransform.rect.width == 0f)
                {
                    this.m_marginWidth = float.PositiveInfinity;
                }
                else
                {
                    this.m_marginWidth = (this.m_rectTransform.rect.width - this.m_margin.x) - this.m_margin.z;
                }
                if (this.m_rectTransform.rect.height == 0f)
                {
                    this.m_marginHeight = float.PositiveInfinity;
                }
                else
                {
                    this.m_marginHeight = (this.m_rectTransform.rect.height - this.m_margin.y) - this.m_margin.w;
                }
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

        private Material CreateMaterialInstance(Material source)
        {
            Material material;
            return new Material(source) { shaderKeywords = source.shaderKeywords, hideFlags = HideFlags.DontSave, name = material.name + " (Instance)" };
        }

        private void DisableMasking()
        {
            if (this.m_fontMaterial != null)
            {
                if (this.m_stencilID > 0)
                {
                    this.m_sharedMaterial = this.m_maskingMaterial;
                }
                else
                {
                    this.m_sharedMaterial = this.m_baseMaterial;
                }
                this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
                UnityEngine.Object.DestroyImmediate(this.m_fontMaterial);
            }
            this.m_isMaskingEnabled = false;
        }

        private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
        {
            int num = index + 12;
            if (num > this.m_uiVertices.Length)
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
            this.m_uiVertices[index].position = start + new Vector3(0f, 0f - ((height + this.m_padding) * this.m_fontScale), 0f);
            this.m_uiVertices[index + 1].position = start + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
            this.m_uiVertices[index + 2].position = start + new Vector3(x, this.m_padding * this.m_fontScale, 0f);
            this.m_uiVertices[index + 3].position = this.m_uiVertices[index].position + new Vector3(x, 0f, 0f);
            this.m_uiVertices[index + 4].position = this.m_uiVertices[index + 3].position;
            this.m_uiVertices[index + 5].position = this.m_uiVertices[index + 2].position;
            this.m_uiVertices[index + 6].position = end + new Vector3(-x, this.m_padding * this.m_fontScale, 0f);
            this.m_uiVertices[index + 7].position = end + new Vector3(-x, -(height + this.m_padding) * this.m_fontScale, 0f);
            this.m_uiVertices[index + 8].position = this.m_uiVertices[index + 7].position;
            this.m_uiVertices[index + 9].position = this.m_uiVertices[index + 6].position;
            this.m_uiVertices[index + 10].position = end + new Vector3(0f, this.m_padding * this.m_fontScale, 0f);
            this.m_uiVertices[index + 11].position = end + new Vector3(0f, -(height + this.m_padding) * this.m_fontScale, 0f);
            Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - this.m_padding) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (((this.m_cached_Underline_GlyphInfo.y + this.m_padding) + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight));
            Vector2 vector2 = new Vector2(vector.x, 1f - ((this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight));
            Vector2 vector3 = new Vector2(((this.m_cached_Underline_GlyphInfo.x + this.m_padding) + (this.m_cached_Underline_GlyphInfo.width / 2f)) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
            Vector2 vector4 = new Vector2(vector3.x, vector.y);
            Vector2 vector5 = new Vector2(((this.m_cached_Underline_GlyphInfo.x + this.m_padding) + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
            Vector2 vector6 = new Vector2(vector5.x, vector.y);
            this.m_uiVertices[0 + index].uv0 = vector;
            this.m_uiVertices[1 + index].uv0 = vector2;
            this.m_uiVertices[2 + index].uv0 = vector3;
            this.m_uiVertices[3 + index].uv0 = vector4;
            this.m_uiVertices[4 + index].uv0 = new Vector2(vector3.x - (vector3.x * 0.001f), vector.y);
            this.m_uiVertices[5 + index].uv0 = new Vector2(vector3.x - (vector3.x * 0.001f), vector2.y);
            this.m_uiVertices[6 + index].uv0 = new Vector2(vector3.x + (vector3.x * 0.001f), vector2.y);
            this.m_uiVertices[7 + index].uv0 = new Vector2(vector3.x + (vector3.x * 0.001f), vector.y);
            this.m_uiVertices[8 + index].uv0 = vector4;
            this.m_uiVertices[9 + index].uv0 = vector3;
            this.m_uiVertices[10 + index].uv0 = vector5;
            this.m_uiVertices[11 + index].uv0 = vector6;
            float num4 = 0f;
            float num5 = (this.m_uiVertices[index + 2].position.x - start.x) / (end.x - start.x);
            float scale = this.m_fontScale * this.m_rectTransform.lossyScale.z;
            float num7 = scale;
            this.m_uiVertices[0 + index].uv1 = this.PackUV(0f, 0f, scale);
            this.m_uiVertices[1 + index].uv1 = this.PackUV(0f, 1f, scale);
            this.m_uiVertices[2 + index].uv1 = this.PackUV(num5, 1f, scale);
            this.m_uiVertices[3 + index].uv1 = this.PackUV(num5, 0f, scale);
            num4 = (this.m_uiVertices[index + 4].position.x - start.x) / (end.x - start.x);
            num5 = (this.m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);
            this.m_uiVertices[4 + index].uv1 = this.PackUV(num4, 0f, num7);
            this.m_uiVertices[5 + index].uv1 = this.PackUV(num4, 1f, num7);
            this.m_uiVertices[6 + index].uv1 = this.PackUV(num5, 1f, num7);
            this.m_uiVertices[7 + index].uv1 = this.PackUV(num5, 0f, num7);
            num4 = (this.m_uiVertices[index + 8].position.x - start.x) / (end.x - start.x);
            num5 = (this.m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);
            this.m_uiVertices[8 + index].uv1 = this.PackUV(num4, 0f, scale);
            this.m_uiVertices[9 + index].uv1 = this.PackUV(num4, 1f, scale);
            this.m_uiVertices[10 + index].uv1 = this.PackUV(1f, 1f, scale);
            this.m_uiVertices[11 + index].uv1 = this.PackUV(1f, 0f, scale);
            this.m_uiVertices[0 + index].color = underlineColor;
            this.m_uiVertices[1 + index].color = underlineColor;
            this.m_uiVertices[2 + index].color = underlineColor;
            this.m_uiVertices[3 + index].color = underlineColor;
            this.m_uiVertices[4 + index].color = underlineColor;
            this.m_uiVertices[5 + index].color = underlineColor;
            this.m_uiVertices[6 + index].color = underlineColor;
            this.m_uiVertices[7 + index].color = underlineColor;
            this.m_uiVertices[8 + index].color = underlineColor;
            this.m_uiVertices[9 + index].color = underlineColor;
            this.m_uiVertices[10 + index].color = underlineColor;
            this.m_uiVertices[11 + index].color = underlineColor;
            index += 12;
        }

        private void EnableMasking()
        {
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
                this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
            }
            this.m_sharedMaterial = this.m_fontMaterial;
            if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                this.m_sharedMaterial.EnableKeyword("MASK_SOFT");
                this.m_sharedMaterial.DisableKeyword("MASK_HARD");
                this.m_sharedMaterial.DisableKeyword("MASK_OFF");
                this.UpdateMask();
            }
            this.m_isMaskingEnabled = true;
        }

        private void FillCharacterVertexBuffers(int i, int index_X4)
        {
            TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
            this.m_textInfo.characterInfo[i].vertexIndex = (short) index_X4;
            UIVertex vertex = new UIVertex {
                position = characterInfo[i].vertex_BL.position,
                uv0 = characterInfo[i].vertex_BL.uv,
                uv1 = characterInfo[i].vertex_BL.uv2,
                color = characterInfo[i].vertex_BL.color,
                normal = characterInfo[i].vertex_BL.normal,
                tangent = characterInfo[i].vertex_BL.tangent
            };
            this.m_uiVertices[0 + index_X4] = vertex;
            UIVertex vertex2 = new UIVertex {
                position = characterInfo[i].vertex_TL.position,
                uv0 = characterInfo[i].vertex_TL.uv,
                uv1 = characterInfo[i].vertex_TL.uv2,
                color = characterInfo[i].vertex_TL.color,
                normal = characterInfo[i].vertex_TL.normal,
                tangent = characterInfo[i].vertex_TL.tangent
            };
            this.m_uiVertices[1 + index_X4] = vertex2;
            UIVertex vertex3 = new UIVertex {
                position = characterInfo[i].vertex_TR.position,
                uv0 = characterInfo[i].vertex_TR.uv,
                uv1 = characterInfo[i].vertex_TR.uv2,
                color = characterInfo[i].vertex_TR.color,
                normal = characterInfo[i].vertex_TR.normal,
                tangent = characterInfo[i].vertex_TR.tangent
            };
            this.m_uiVertices[2 + index_X4] = vertex3;
            UIVertex vertex4 = new UIVertex {
                position = characterInfo[i].vertex_BR.position,
                uv0 = characterInfo[i].vertex_BR.uv,
                uv1 = characterInfo[i].vertex_BR.uv2,
                color = characterInfo[i].vertex_BR.color,
                normal = characterInfo[i].vertex_BR.normal,
                tangent = characterInfo[i].vertex_BR.tangent
            };
            this.m_uiVertices[3 + index_X4] = vertex4;
        }

        private void FillSpriteVertexBuffers(int i, int spriteIndex_X4)
        {
            this.m_textInfo.characterInfo[i].vertexIndex = (short) spriteIndex_X4;
            TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
            UIVertex[] uiVertex = this.m_inlineGraphics.uiVertex;
            UIVertex vertex = new UIVertex {
                position = characterInfo[i].vertex_BL.position,
                uv0 = characterInfo[i].vertex_BL.uv,
                color = characterInfo[i].vertex_BL.color
            };
            uiVertex[spriteIndex_X4] = vertex;
            vertex.position = characterInfo[i].vertex_TL.position;
            vertex.uv0 = characterInfo[i].vertex_TL.uv;
            vertex.color = characterInfo[i].vertex_TL.color;
            uiVertex[spriteIndex_X4 + 1] = vertex;
            vertex.position = characterInfo[i].vertex_TR.position;
            vertex.uv0 = characterInfo[i].vertex_TR.uv;
            vertex.color = characterInfo[i].vertex_TR.color;
            uiVertex[spriteIndex_X4 + 2] = vertex;
            vertex.position = characterInfo[i].vertex_BR.position;
            vertex.uv0 = characterInfo[i].vertex_BR.uv;
            vertex.color = characterInfo[i].vertex_BR.color;
            uiVertex[spriteIndex_X4 + 3] = vertex;
            this.m_inlineGraphics.SetUIVertex(uiVertex);
        }

        public void ForceMeshUpdate()
        {
            this.OnPreRenderCanvas();
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
                if (((this.m_char_buffer == null) || (this.m_char_buffer.Length == 0)) || (this.m_char_buffer[0] == 0))
                {
                    if (this.m_uiVertices != null)
                    {
                        this.m_uiRenderer.SetVertices(this.m_uiVertices, 0);
                        if (this.m_inlineGraphics != null)
                        {
                            this.m_inlineGraphics.ClearUIVertex();
                        }
                    }
                    this.m_preferredWidth = 0f;
                    this.m_preferredHeight = 0f;
                    this.m_renderedWidth = 0f;
                    this.m_renderedHeight = 0f;
                    LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
                }
                else
                {
                    int num = this.SetArraySizes(this.m_char_buffer);
                    this.m_fontIndex = 0;
                    this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
                    this.m_fontScale = this.m_fontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
                    float fontScale = this.m_fontScale;
                    this.m_maxFontScale = 0f;
                    float num3 = 0f;
                    float num4 = 1f;
                    this.m_currentFontSize = this.m_fontSize;
                    float num5 = 0f;
                    int num6 = 0;
                    this.m_style = this.m_fontStyle;
                    this.m_lineJustification = this.m_textAlignment;
                    if (this.checkPaddingRequired)
                    {
                        this.checkPaddingRequired = false;
                        this.m_padding = ShaderUtilities.GetPadding(this.m_uiRenderer.GetMaterial(), this.m_enableExtraPadding, this.m_isUsingBold);
                        this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
                        this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
                    }
                    float num7 = 0f;
                    float num8 = 1f;
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
                    float num9 = 0f;
                    this.m_xAdvance = 0f;
                    this.m_indent = 0f;
                    this.m_maxXAdvance = 0f;
                    this.m_lineNumber = 0;
                    this.m_pageNumber = 0;
                    this.m_characterCount = 0;
                    this.m_visibleCharacterCount = 0;
                    this.m_visibleSpriteCount = 0;
                    this.m_firstVisibleCharacterOfLine = 0;
                    this.m_lastVisibleCharacterOfLine = 0;
                    int index = 0;
                    this.m_rectTransform.GetLocalCorners(this.m_rectCorners);
                    Vector4 margin = this.m_margin;
                    float marginWidth = this.m_marginWidth;
                    float marginHeight = this.m_marginHeight;
                    this.m_width = 0f;
                    this.m_renderedWidth = 0f;
                    this.m_renderedHeight = 0f;
                    bool flag4 = true;
                    bool flag5 = false;
                    this.m_SavedLineState = new WordWrapState();
                    this.m_SavedWordWrapState = new WordWrapState();
                    int num13 = 0;
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
                    float num15 = 0f;
                    this.m_isNewPage = false;
                    this.loopCountA++;
                    int endIndex = 0;
                    for (int j = 0; this.m_char_buffer[j] != 0; j++)
                    {
                        num6 = this.m_char_buffer[j];
                        this.m_isSprite = false;
                        num4 = 1f;
                        if ((this.m_isRichText && (num6 == 60)) && this.ValidateHtmlTag(this.m_char_buffer, j + 1, out endIndex))
                        {
                            j = endIndex;
                            if (this.m_isRecalculateScaleRequired)
                            {
                                this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
                                this.m_isRecalculateScaleRequired = false;
                            }
                            if (!this.m_isSprite)
                            {
                                continue;
                            }
                        }
                        bool flag = false;
                        if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
                        {
                            if (char.IsLower((char) num6))
                            {
                                num6 -= 0x20;
                            }
                        }
                        else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
                        {
                            if (char.IsUpper((char) num6))
                            {
                                num6 += 0x20;
                            }
                        }
                        else if (((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps) || ((this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps))
                        {
                            if (char.IsLower((char) num6))
                            {
                                this.m_fontScale = (this.m_currentFontSize * 0.8f) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
                                num6 -= 0x20;
                            }
                            else
                            {
                                this.m_fontScale = this.m_currentFontSize / this.m_fontAssetArray[this.m_fontIndex].fontInfo.PointSize;
                            }
                        }
                        if (this.m_isSprite)
                        {
                            SpriteInfo sprite = this.m_inlineGraphics.GetSprite(this.m_spriteIndex);
                            if (sprite == null)
                            {
                                continue;
                            }
                            num6 = 0xe000 + this.m_spriteIndex;
                            this.m_cached_GlyphInfo = new GlyphInfo();
                            this.m_cached_GlyphInfo.x = sprite.x;
                            this.m_cached_GlyphInfo.y = sprite.y;
                            this.m_cached_GlyphInfo.width = sprite.width;
                            this.m_cached_GlyphInfo.height = sprite.height;
                            this.m_cached_GlyphInfo.xOffset = sprite.pivot.x + sprite.xOffset;
                            this.m_cached_GlyphInfo.yOffset = sprite.pivot.y + sprite.yOffset;
                            num4 = (this.m_fontAsset.fontInfo.Ascender / sprite.height) * sprite.scale;
                            this.m_cached_GlyphInfo.xAdvance = sprite.xAdvance * num4;
                            this.m_visibleSpriteCount++;
                            this.m_textInfo.characterInfo[this.m_characterCount].type = TMP_CharacterType.Sprite;
                        }
                        else
                        {
                            this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num6, out this.m_cached_GlyphInfo);
                            if (this.m_cached_GlyphInfo == null)
                            {
                                if (char.IsLower((char) num6))
                                {
                                    if (this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num6 - 0x20, out this.m_cached_GlyphInfo))
                                    {
                                        num6 -= 0x20;
                                    }
                                }
                                else if (char.IsUpper((char) num6) && this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(num6 + 0x20, out this.m_cached_GlyphInfo))
                                {
                                    num6 += 0x20;
                                }
                                if (this.m_cached_GlyphInfo == null)
                                {
                                    this.m_fontAssetArray[this.m_fontIndex].characterDictionary.TryGetValue(0x58, out this.m_cached_GlyphInfo);
                                    if (this.m_cached_GlyphInfo != null)
                                    {
                                        Debug.LogWarning("Character with ASCII value of " + num6 + " was not found in the Font Asset Glyph Table.");
                                        num6 = 0x58;
                                        flag = true;
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Character with ASCII value of " + num6 + " was not found in the Font Asset Glyph Table.");
                                        continue;
                                    }
                                }
                            }
                            this.m_textInfo.characterInfo[this.m_characterCount].type = TMP_CharacterType.Character;
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].character = (char) num6;
                        this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
                        this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_style;
                        this.m_textInfo.characterInfo[this.m_characterCount].index = (short) j;
                        if (this.m_enableKerning && (this.m_characterCount >= 1))
                        {
                            KerningPair pair;
                            int character = this.m_textInfo.characterInfo[this.m_characterCount - 1].character;
                            KerningPairKey key = new KerningPairKey(character, num6);
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
                            num7 = this.m_fontAssetArray[this.m_fontIndex].BoldStyle * 2f;
                            num8 = 1.07f;
                        }
                        else
                        {
                            num7 = this.m_fontAssetArray[this.m_fontIndex].NormalStyle * 2f;
                            num8 = 1f;
                        }
                        float num19 = !this.m_isSprite ? this.m_padding : (!this.m_enableExtraPadding ? ((float) 0) : ((float) 4));
                        Vector3 vector6 = new Vector3((0f + this.m_xAdvance) + ((((this.m_cached_GlyphInfo.xOffset - num19) - num7) * this.m_fontScale) * num4), ((((this.m_cached_GlyphInfo.yOffset + num19) * this.m_fontScale) * num4) - this.m_lineOffset) + this.m_baselineOffset, 0f);
                        Vector3 vector7 = new Vector3(vector6.x, vector6.y - (((this.m_cached_GlyphInfo.height + (num19 * 2f)) * this.m_fontScale) * num4), 0f);
                        Vector3 vector8 = new Vector3(vector7.x + ((((this.m_cached_GlyphInfo.width + (num19 * 2f)) + (num7 * 2f)) * this.m_fontScale) * num4), vector6.y, 0f);
                        Vector3 vector9 = new Vector3(vector8.x, vector7.y, 0f);
                        if (((this.m_style & FontStyles.Italic) == FontStyles.Italic) || ((this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic))
                        {
                            float num20 = this.m_fontAssetArray[this.m_fontIndex].ItalicStyle * 0.01f;
                            Vector3 vector10 = new Vector3(num20 * ((((this.m_cached_GlyphInfo.yOffset + num19) + num7) * this.m_fontScale) * num4), 0f, 0f);
                            Vector3 vector11 = new Vector3(num20 * (((((this.m_cached_GlyphInfo.yOffset - this.m_cached_GlyphInfo.height) - num19) - num7) * this.m_fontScale) * num4), 0f, 0f);
                            vector6 += vector10;
                            vector7 += vector11;
                            vector8 += vector10;
                            vector9 += vector11;
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector7;
                        this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector6;
                        this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector8;
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector9;
                        float a = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) + this.m_baselineOffset;
                        float num22 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                        if (this.m_isSprite)
                        {
                            a = Mathf.Max(a, vector6.y - ((num19 * this.m_fontScale) * num4));
                            num22 = Mathf.Min(num22, vector9.y - ((num19 * this.m_fontScale) * num4));
                        }
                        if (this.m_lineNumber == 0)
                        {
                            this.m_maxAscender = (this.m_maxAscender <= a) ? a : this.m_maxAscender;
                        }
                        if (this.m_lineOffset == 0f)
                        {
                            num15 = (num15 <= a) ? a : num15;
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
                        if ((((num6 != 0x20) && (num6 != 9)) && ((num6 != 10) && (num6 != 13))) || this.m_isSprite)
                        {
                            Color32 red;
                            this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
                            if (this.m_baselineOffset == 0f)
                            {
                                this.m_maxFontScale = Mathf.Max(this.m_maxFontScale, this.m_fontScale);
                            }
                            float num23 = marginWidth + 0.0001f;
                            if ((this.m_xAdvance + (this.m_cached_GlyphInfo.xAdvance * this.m_fontScale)) > num23)
                            {
                                index = this.m_characterCount - 1;
                                if (this.enableWordWrapping && (this.m_characterCount != this.m_firstVisibleCharacterOfLine))
                                {
                                    if ((num13 == this.m_SavedWordWrapState.previous_WordBreak) || flag4)
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
                                            Debug.Log("Recursive count exceeded!");
                                            continue;
                                        }
                                    }
                                    j = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState);
                                    num13 = j;
                                    if ((((this.m_lineNumber > 0) && (this.m_maxFontScale != 0f)) && ((this.m_lineHeight == 0f) && (this.m_maxFontScale != num3))) && !this.m_isNewPage)
                                    {
                                        float num24 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
                                        float num25 = (((((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_lineSpacing) + this.m_paragraphSpacing) + num24) + this.m_lineSpacingDelta) * this.m_maxFontScale) - ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num24) * num3);
                                        this.m_lineOffset += num25 - num9;
                                        this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount - 1, num25 - num9);
                                    }
                                    this.m_isNewPage = false;
                                    float y = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale) - this.m_lineOffset;
                                    float num27 = (((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                                    y = (y <= num27) ? num27 : y;
                                    float num28 = ((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale) - this.m_lineOffset;
                                    float num29 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                                    num28 = (num28 >= num29) ? num29 : num28;
                                    if (this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].isVisible)
                                    {
                                        this.m_maxDescender = (this.m_maxDescender >= num28) ? num28 : this.m_maxDescender;
                                    }
                                    this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
                                    this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = ((this.m_characterCount - 1) <= 0) ? 1 : (this.m_characterCount - 1);
                                    this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
                                    this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = (this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex) + 1;
                                    this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num28);
                                    this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, y);
                                    this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - (num19 * this.m_maxFontScale);
                                    this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing * this.m_fontScale);
                                    this.m_firstVisibleCharacterOfLine = this.m_characterCount;
                                    this.m_renderedWidth += this.m_xAdvance;
                                    if (this.m_enableWordWrapping)
                                    {
                                        this.m_renderedHeight = this.m_maxAscender - this.m_maxDescender;
                                    }
                                    else
                                    {
                                        this.m_renderedHeight = Mathf.Max(this.m_renderedHeight, y - num28);
                                    }
                                    this.SaveWordWrappingState(ref this.m_SavedLineState, j, this.m_characterCount - 1);
                                    this.m_lineNumber++;
                                    if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
                                    {
                                        this.ResizeLineExtents(this.m_lineNumber);
                                    }
                                    if (this.m_lineHeight == 0f)
                                    {
                                        num9 = ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_lineSpacing) + this.m_lineSpacingDelta) * this.m_fontScale;
                                        this.m_lineOffset += num9;
                                    }
                                    else
                                    {
                                        this.m_lineOffset += (this.m_lineHeight + this.m_lineSpacing) * fontScale;
                                    }
                                    num3 = this.m_fontScale;
                                    this.m_xAdvance = 0f + this.m_indent;
                                    num4 = 1f;
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
                                        break;

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
                                            break;
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
                                        break;

                                    case TextOverflowModes.Truncate:
                                        if (this.m_isMaskingEnabled)
                                        {
                                            this.DisableMasking();
                                        }
                                        this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
                                        break;

                                    case TextOverflowModes.ScrollRect:
                                        if (!this.m_isMaskingEnabled)
                                        {
                                            this.EnableMasking();
                                        }
                                        break;
                                }
                            }
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
                            if (!this.m_isSprite)
                            {
                                this.SaveGlyphVertexInfo(num7, red);
                            }
                            else
                            {
                                this.SaveSpriteVertexInfo(red);
                            }
                            if (!this.m_isSprite)
                            {
                                this.m_visibleCharacterCount++;
                            }
                            if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible || this.m_isSprite)
                            {
                                this.m_lastVisibleCharacterOfLine = this.m_characterCount;
                            }
                        }
                        else
                        {
                            switch (num6)
                            {
                                case 9:
                                case 0x20:
                                    this.m_textInfo.lineInfo[this.m_lineNumber].spaceCount++;
                                    this.m_textInfo.spaceCount++;
                                    break;
                            }
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = (short) this.m_lineNumber;
                        this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = (short) this.m_pageNumber;
                        if ((((num6 != 10) && (num6 != 13)) && (num6 != 0x2026)) || (this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1))
                        {
                            this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
                        }
                        if (((this.m_maxAscender - num22) + ((this.m_alignmentPadding.w * 2f) * this.m_fontScale)) > marginHeight)
                        {
                            if ((this.m_enableAutoSizing && (this.m_lineSpacingDelta > this.m_lineSpacingMax)) && (this.m_lineNumber > 0))
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
                                    this.m_pageNumber++;
                                    this.m_lineNumber++;
                                    continue;
                                }
                            }
                        }
                        if (num6 == 9)
                        {
                            this.m_xAdvance += (this.m_fontAsset.fontInfo.TabWidth * this.m_fontScale) * this.m_fontAsset.TabSize;
                        }
                        else if (this.m_monoSpacing != 0f)
                        {
                            this.m_xAdvance += ((((this.m_monoSpacing + (this.m_cached_GlyphInfo.width / 2f)) + this.m_cached_GlyphInfo.xOffset) + this.m_characterSpacing) + this.m_cSpacing) * this.m_fontScale;
                        }
                        else
                        {
                            this.m_xAdvance += ((this.m_cached_GlyphInfo.xAdvance * num8) * this.m_fontScale) + ((this.m_characterSpacing + this.m_cSpacing) * this.m_fontScale);
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].xAdvance = this.m_xAdvance;
                        if (num6 == 13)
                        {
                            this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, (this.m_renderedWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                            this.m_renderedWidth = 0f;
                            this.m_xAdvance = 0f + this.m_indent;
                        }
                        if ((num6 == 10) || (this.m_characterCount == (num - 1)))
                        {
                            if ((((this.m_lineNumber > 0) && (this.m_maxFontScale != 0f)) && ((this.m_lineHeight == 0f) && (this.m_maxFontScale != num3))) && !this.m_isNewPage)
                            {
                                float num30 = this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight - (this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender - this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender);
                                float num31 = (((((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_lineSpacing) + this.m_paragraphSpacing) + num30) + this.m_lineSpacingDelta) * this.m_maxFontScale) - ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - num30) * num3);
                                this.m_lineOffset += num31 - num9;
                                this.AdjustLineOffset(this.m_firstVisibleCharacterOfLine, this.m_characterCount, num31 - num9);
                            }
                            this.m_isNewPage = false;
                            float num32 = ((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_maxFontScale) - this.m_lineOffset;
                            float num33 = (((this.m_fontAsset.fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                            num32 = (num32 <= num33) ? num33 : num32;
                            float num34 = ((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_maxFontScale) - this.m_lineOffset;
                            float num35 = (((this.m_fontAsset.fontInfo.Descender + this.m_alignmentPadding.w) * this.m_fontScale) - this.m_lineOffset) + this.m_baselineOffset;
                            num34 = (num34 >= num35) ? num35 : num34;
                            this.m_maxDescender = (this.m_maxDescender >= num34) ? num34 : this.m_maxDescender;
                            this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstVisibleCharacterOfLine;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = this.m_characterCount;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = this.m_lastVisibleCharacterOfLine;
                            this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = (this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex) + 1;
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num34);
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num32);
                            this.m_textInfo.lineInfo[this.m_lineNumber].lineLength = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - (num19 * this.m_maxFontScale);
                            this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance - (this.m_characterSpacing * this.m_fontScale);
                            this.m_firstVisibleCharacterOfLine = this.m_characterCount + 1;
                            if ((num6 == 10) && (this.m_characterCount != (num - 1)))
                            {
                                this.m_maxXAdvance = Mathf.Max(this.m_maxXAdvance, (this.m_renderedWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                                this.m_renderedWidth = 0f;
                            }
                            else
                            {
                                this.m_renderedWidth = Mathf.Max(this.m_maxXAdvance, (this.m_renderedWidth + this.m_xAdvance) + (this.m_alignmentPadding.z * this.m_fontScale));
                            }
                            this.m_renderedHeight = this.m_maxAscender - this.m_maxDescender;
                            if (num6 == 10)
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
                                    num9 = (((this.m_fontAssetArray[this.m_fontIndex].fontInfo.LineHeight + this.m_paragraphSpacing) + this.m_lineSpacing) + this.m_lineSpacingDelta) * this.m_fontScale;
                                    this.m_lineOffset += num9;
                                }
                                else
                                {
                                    this.m_lineOffset += ((this.m_lineHeight + this.m_lineSpacing) + this.m_paragraphSpacing) * fontScale;
                                }
                                num3 = this.m_fontScale;
                                this.m_maxFontScale = 0f;
                                num4 = 1f;
                                this.m_xAdvance = 0f + this.m_indent;
                                index = this.m_characterCount - 1;
                            }
                        }
                        this.m_textInfo.characterInfo[this.m_characterCount].baseLine = this.m_textInfo.characterInfo[this.m_characterCount].topRight.y - ((this.m_cached_GlyphInfo.yOffset + num19) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].topLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Ascender + this.m_alignmentPadding.y) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].bottomLine = this.m_textInfo.characterInfo[this.m_characterCount].baseLine + ((this.m_fontAssetArray[this.m_fontIndex].fontInfo.Descender - this.m_alignmentPadding.w) * this.m_fontScale);
                        this.m_textInfo.characterInfo[this.m_characterCount].padding = num19 * this.m_fontScale;
                        this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = this.m_cached_GlyphInfo.width / this.m_cached_GlyphInfo.height;
                        this.m_textInfo.characterInfo[this.m_characterCount].scale = this.m_fontScale;
                        this.m_textInfo.characterInfo[this.m_characterCount].meshIndex = this.m_fontIndex;
                        if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
                        {
                            float x = Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position.x);
                            this.m_meshExtents.min = new Vector2(x, Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position.y));
                            float introduced112 = Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position.x);
                            this.m_meshExtents.max = new Vector2(introduced112, Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position.y));
                        }
                        if (((num6 != 13) && (num6 != 10)) && (this.m_pageNumber < 0x10))
                        {
                            this.m_textInfo.pageInfo[this.m_pageNumber].ascender = num15;
                            this.m_textInfo.pageInfo[this.m_pageNumber].descender = (num22 >= this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? this.m_textInfo.pageInfo[this.m_pageNumber].descender : num22;
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
                            if ((num6 == 9) || (num6 == 0x20))
                            {
                                this.SaveWordWrappingState(ref this.m_SavedWordWrapState, j, this.m_characterCount);
                                this.m_isCharacterWrappingEnabled = false;
                                flag4 = false;
                            }
                            else if (((flag4 || this.m_isCharacterWrappingEnabled) && (((this.m_characterCount < (num - 1)) && !this.m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num6)) && !this.m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(this.m_VisibleCharacters[this.m_characterCount + 1]))) || flag5)
                            {
                                this.SaveWordWrappingState(ref this.m_SavedWordWrapState, j, this.m_characterCount);
                            }
                        }
                        this.m_characterCount++;
                    }
                    num5 = this.m_maxFontSize - this.m_minFontSize;
                    if ((!this.m_isCharacterWrappingEnabled && this.m_enableAutoSizing) && ((num5 > 0.051f) && (this.m_fontSize < this.m_fontSizeMax)))
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
                        this.m_isCharacterWrappingEnabled = false;
                        this.m_renderedHeight += (this.m_margin.y <= 0f) ? 0f : this.m_margin.y;
                        if (this.m_renderMode != TextRenderFlags.GetPreferredSizes)
                        {
                            if (!this.IsRectTransformDriven)
                            {
                                this.m_preferredWidth = this.m_renderedWidth;
                                this.m_preferredHeight = this.m_renderedHeight;
                            }
                            if ((this.m_visibleCharacterCount == 0) && (this.m_visibleSpriteCount == 0))
                            {
                                if (this.m_uiVertices != null)
                                {
                                    this.m_uiRenderer.SetVertices(this.m_uiVertices, 0);
                                }
                            }
                            else
                            {
                                int num36 = this.m_visibleCharacterCount * 4;
                                Array.Clear(this.m_uiVertices, num36, this.m_uiVertices.Length - num36);
                                switch (this.m_textAlignment)
                                {
                                    case TextAlignmentOptions.TopLeft:
                                    case TextAlignmentOptions.Top:
                                    case TextAlignmentOptions.TopRight:
                                    case TextAlignmentOptions.TopJustified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = this.m_rectCorners[1] + new Vector3(0f + margin.x, (0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender) - margin.y, 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = this.m_rectCorners[1] + new Vector3(0f + margin.x, (0f - this.m_maxAscender) - margin.y, 0f);
                                        break;

                                    case TextAlignmentOptions.Left:
                                    case TextAlignmentOptions.Center:
                                    case TextAlignmentOptions.Right:
                                    case TextAlignmentOptions.Justified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = ((Vector3) ((this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f)) + new Vector3(0f + margin.x, 0f - ((((this.m_textInfo.pageInfo[this.m_pageToDisplay].ascender + margin.y) + this.m_textInfo.pageInfo[this.m_pageToDisplay].descender) - margin.w) / 2f), 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = ((Vector3) ((this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f)) + new Vector3(0f + margin.x, 0f - ((((this.m_maxAscender + margin.y) + this.m_maxDescender) - margin.w) / 2f), 0f);
                                        break;

                                    case TextAlignmentOptions.BottomLeft:
                                    case TextAlignmentOptions.Bottom:
                                    case TextAlignmentOptions.BottomRight:
                                    case TextAlignmentOptions.BottomJustified:
                                        if (this.m_overflowMode == TextOverflowModes.Page)
                                        {
                                            this.m_anchorOffset = this.m_rectCorners[0] + new Vector3(0f + margin.x, (0f - this.m_textInfo.pageInfo[this.m_pageToDisplay].descender) + margin.w, 0f);
                                            break;
                                        }
                                        this.m_anchorOffset = this.m_rectCorners[0] + new Vector3(0f + margin.x, (0f - this.m_maxDescender) + margin.w, 0f);
                                        break;

                                    case TextAlignmentOptions.BaselineLeft:
                                    case TextAlignmentOptions.Baseline:
                                    case TextAlignmentOptions.BaselineRight:
                                        this.m_anchorOffset = ((Vector3) ((this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f)) + new Vector3(0f + margin.x, 0f, 0f);
                                        break;

                                    case TextAlignmentOptions.MidlineLeft:
                                    case TextAlignmentOptions.Midline:
                                    case TextAlignmentOptions.MidlineRight:
                                    case TextAlignmentOptions.MidlineJustified:
                                        this.m_anchorOffset = ((Vector3) ((this.m_rectCorners[0] + this.m_rectCorners[1]) / 2f)) + new Vector3(0f + margin.x, 0f - ((((this.m_meshExtents.max.y + margin.y) + this.m_meshExtents.min.y) - margin.w) / 2f), 0f);
                                        break;
                                }
                                Vector3 vector12 = Vector3.zero;
                                Vector3 vector13 = Vector3.zero;
                                int num37 = 0;
                                int num38 = 0;
                                Array.Clear(this.m_meshAllocCount, 0, 0x11);
                                int num39 = 0;
                                Color32 underlineColor = new Color32(0xff, 0xff, 0xff, 0x7f);
                                int num40 = 0;
                                int num41 = 0;
                                int num42 = 0;
                                bool flag6 = false;
                                int num43 = 0;
                                int num44 = 0;
                                float z = this.m_rectTransform.lossyScale.z;
                                RenderMode renderMode = this.m_canvas.renderMode;
                                float scaleFactor = this.m_canvas.scaleFactor;
                                bool flag7 = this.m_canvas.worldCamera != null;
                                for (int k = 0; k < this.m_characterCount; k++)
                                {
                                    float num49;
                                    float num51;
                                    TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
                                    int lineNumber = characterInfo[k].lineNumber;
                                    char c = characterInfo[k].character;
                                    TMP_LineInfo info2 = this.m_textInfo.lineInfo[lineNumber];
                                    TextAlignmentOptions alignment = info2.alignment;
                                    num41 = lineNumber + 1;
                                    switch (alignment)
                                    {
                                        case TextAlignmentOptions.TopLeft:
                                        case TextAlignmentOptions.Left:
                                        case TextAlignmentOptions.BottomLeft:
                                        case TextAlignmentOptions.BaselineLeft:
                                        case TextAlignmentOptions.MidlineLeft:
                                            vector12 = Vector3.zero;
                                            goto Label_2F74;

                                        case TextAlignmentOptions.Top:
                                        case TextAlignmentOptions.Center:
                                        case TextAlignmentOptions.Bottom:
                                        case TextAlignmentOptions.Baseline:
                                        case TextAlignmentOptions.Midline:
                                            vector12 = new Vector3((marginWidth / 2f) - (info2.maxAdvance / 2f), 0f, 0f);
                                            goto Label_2F74;

                                        case TextAlignmentOptions.TopRight:
                                        case TextAlignmentOptions.Right:
                                        case TextAlignmentOptions.BottomRight:
                                        case TextAlignmentOptions.BaselineRight:
                                        case TextAlignmentOptions.MidlineRight:
                                            vector12 = new Vector3(marginWidth - info2.maxAdvance, 0f, 0f);
                                            goto Label_2F74;

                                        case TextAlignmentOptions.TopJustified:
                                        case TextAlignmentOptions.Justified:
                                        case TextAlignmentOptions.BottomJustified:
                                        case TextAlignmentOptions.BaselineJustified:
                                        case TextAlignmentOptions.MidlineJustified:
                                        {
                                            num6 = this.m_textInfo.characterInfo[k].character;
                                            char ch2 = this.m_textInfo.characterInfo[info2.lastCharacterIndex].character;
                                            if ((!char.IsWhiteSpace(ch2) || char.IsControl(ch2)) || (lineNumber >= this.m_lineNumber))
                                            {
                                                goto Label_2F68;
                                            }
                                            num49 = ((this.m_rectCorners[3].x - margin.z) - (this.m_rectCorners[0].x + margin.x)) - info2.maxAdvance;
                                            if ((lineNumber == num42) && (k != 0))
                                            {
                                                break;
                                            }
                                            vector12 = Vector3.zero;
                                            goto Label_2F74;
                                        }
                                        default:
                                            goto Label_2F74;
                                    }
                                    switch (num6)
                                    {
                                        case 9:
                                        case 0x20:
                                            vector12 += new Vector3((num49 * (1f - this.m_wordWrappingRatios)) / ((float) (info2.spaceCount - 1)), 0f, 0f);
                                            goto Label_2F74;

                                        default:
                                            vector12 += new Vector3((num49 * this.m_wordWrappingRatios) / ((float) ((info2.characterCount - info2.spaceCount) - 1)), 0f, 0f);
                                            goto Label_2F74;
                                    }
                                Label_2F68:
                                    vector12 = Vector3.zero;
                                Label_2F74:
                                    vector13 = this.m_anchorOffset + vector12;
                                    if (!characterInfo[k].isVisible)
                                    {
                                        goto Label_43D4;
                                    }
                                    TMP_CharacterType type = characterInfo[k].type;
                                    TMP_CharacterType type2 = type;
                                    if (type2 != TMP_CharacterType.Character)
                                    {
                                        if (type2 == TMP_CharacterType.Sprite)
                                        {
                                        }
                                        goto Label_4238;
                                    }
                                    Extents lineExtents = info2.lineExtents;
                                    float num50 = ((this.m_uvLineOffset * lineNumber) % 1f) + this.m_uvOffset.x;
                                    switch (this.m_horizontalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            characterInfo[k].vertex_BL.uv2.x = 0f + this.m_uvOffset.x;
                                            characterInfo[k].vertex_TL.uv2.x = 0f + this.m_uvOffset.x;
                                            characterInfo[k].vertex_TR.uv2.x = 1f + this.m_uvOffset.x;
                                            characterInfo[k].vertex_BR.uv2.x = 1f + this.m_uvOffset.x;
                                            goto Label_3C0E;

                                        case TextureMappingOptions.Line:
                                            if (this.m_textAlignment == TextAlignmentOptions.Justified)
                                            {
                                                break;
                                            }
                                            characterInfo[k].vertex_BL.uv2.x = ((characterInfo[k].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num50;
                                            characterInfo[k].vertex_TL.uv2.x = ((characterInfo[k].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num50;
                                            characterInfo[k].vertex_TR.uv2.x = ((characterInfo[k].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num50;
                                            characterInfo[k].vertex_BR.uv2.x = ((characterInfo[k].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x)) + num50;
                                            goto Label_3C0E;

                                        case TextureMappingOptions.Paragraph:
                                        {
                                            characterInfo[k].vertex_BL.uv2.x = (((characterInfo[k].vertex_BL.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                            characterInfo[k].vertex_TL.uv2.x = (((characterInfo[k].vertex_TL.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                            characterInfo[k].vertex_TR.uv2.x = (((characterInfo[k].vertex_TR.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                            characterInfo[k].vertex_BR.uv2.x = (((characterInfo[k].vertex_BR.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                            Vector3 vector14 = characterInfo[k].vertex_BL.position + vector12;
                                            Vector3 vector15 = characterInfo[k].vertex_TL.position + vector12;
                                            Vector3 vector16 = characterInfo[k].vertex_TR.position + vector12;
                                            Vector3 vector17 = characterInfo[k].vertex_BR.position + vector12;
                                            Debug.DrawLine(vector14, vector15, Color.green, 60f);
                                            Debug.DrawLine(vector15, vector16, Color.green, 60f);
                                            Debug.DrawLine(vector16, vector17, Color.green, 60f);
                                            Debug.DrawLine(vector17, vector14, Color.green, 60f);
                                            vector14 = (Vector3) (this.m_meshExtents.min + new Vector2(vector12.x * 0f, vector12.y));
                                            vector15 = new Vector3(this.m_meshExtents.min.x, this.m_meshExtents.max.y, 0f) + new Vector3(vector12.x * 0f, vector12.y, 0f);
                                            vector16 = (Vector3) (this.m_meshExtents.max + new Vector2(vector12.x * 0f, vector12.y));
                                            vector17 = new Vector3(this.m_meshExtents.max.x, this.m_meshExtents.min.y, 0f) + new Vector3(vector12.x * 0f, vector12.y, 0f);
                                            Debug.DrawLine(vector14, vector15, Color.red, 60f);
                                            Debug.DrawLine(vector15, vector16, Color.red, 60f);
                                            Debug.DrawLine(vector16, vector17, Color.red, 60f);
                                            Debug.DrawLine(vector17, vector14, Color.red, 60f);
                                            goto Label_3C0E;
                                        }
                                        case TextureMappingOptions.MatchAspect:
                                            switch (this.m_verticalMapping)
                                            {
                                                case TextureMappingOptions.Character:
                                                    goto Label_37CF;

                                                case TextureMappingOptions.Line:
                                                    goto Label_3878;

                                                case TextureMappingOptions.Paragraph:
                                                    goto Label_3991;

                                                case TextureMappingOptions.MatchAspect:
                                                    goto Label_3AC2;
                                            }
                                            goto Label_3AD1;

                                        default:
                                            goto Label_3C0E;
                                    }
                                    characterInfo[k].vertex_BL.uv2.x = (((characterInfo[k].vertex_BL.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                    characterInfo[k].vertex_TL.uv2.x = (((characterInfo[k].vertex_TL.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                    characterInfo[k].vertex_TR.uv2.x = (((characterInfo[k].vertex_TR.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                    characterInfo[k].vertex_BR.uv2.x = (((characterInfo[k].vertex_BR.position.x + vector12.x) - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x)) + num50;
                                    goto Label_3C0E;
                                Label_37CF:
                                    characterInfo[k].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
                                    characterInfo[k].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
                                    characterInfo[k].vertex_TR.uv2.y = 0f + this.m_uvOffset.y;
                                    characterInfo[k].vertex_BR.uv2.y = 1f + this.m_uvOffset.y;
                                    goto Label_3AD1;
                                Label_3878:
                                    characterInfo[k].vertex_BL.uv2.y = ((characterInfo[k].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + num50;
                                    characterInfo[k].vertex_TL.uv2.y = ((characterInfo[k].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + num50;
                                    characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
                                    characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
                                    goto Label_3AD1;
                                Label_3991:
                                    characterInfo[k].vertex_BL.uv2.y = ((characterInfo[k].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + num50;
                                    characterInfo[k].vertex_TL.uv2.y = ((characterInfo[k].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + num50;
                                    characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
                                    characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
                                    goto Label_3AD1;
                                Label_3AC2:
                                    Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                Label_3AD1:
                                    num51 = (1f - ((characterInfo[k].vertex_BL.uv2.y + characterInfo[k].vertex_TL.uv2.y) * characterInfo[k].aspectRatio)) / 2f;
                                    characterInfo[k].vertex_BL.uv2.x = ((characterInfo[k].vertex_BL.uv2.y * characterInfo[k].aspectRatio) + num51) + num50;
                                    characterInfo[k].vertex_TL.uv2.x = characterInfo[k].vertex_BL.uv2.x;
                                    characterInfo[k].vertex_TR.uv2.x = ((characterInfo[k].vertex_TL.uv2.y * characterInfo[k].aspectRatio) + num51) + num50;
                                    characterInfo[k].vertex_BR.uv2.x = characterInfo[k].vertex_TR.uv2.x;
                                Label_3C0E:
                                    switch (this.m_verticalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            characterInfo[k].vertex_BL.uv2.y = 0f + this.m_uvOffset.y;
                                            characterInfo[k].vertex_TL.uv2.y = 1f + this.m_uvOffset.y;
                                            characterInfo[k].vertex_TR.uv2.y = 1f + this.m_uvOffset.y;
                                            characterInfo[k].vertex_BR.uv2.y = 0f + this.m_uvOffset.y;
                                            break;

                                        case TextureMappingOptions.Line:
                                            characterInfo[k].vertex_BL.uv2.y = ((characterInfo[k].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_TL.uv2.y = ((characterInfo[k].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
                                            characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.Paragraph:
                                            characterInfo[k].vertex_BL.uv2.y = ((characterInfo[k].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_TL.uv2.y = ((characterInfo[k].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
                                            characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.MatchAspect:
                                        {
                                            float num52 = (1f - ((characterInfo[k].vertex_BL.uv2.x + characterInfo[k].vertex_TR.uv2.x) / characterInfo[k].aspectRatio)) / 2f;
                                            characterInfo[k].vertex_BL.uv2.y = (num52 + (characterInfo[k].vertex_BL.uv2.x / characterInfo[k].aspectRatio)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_TL.uv2.y = (num52 + (characterInfo[k].vertex_TR.uv2.x / characterInfo[k].aspectRatio)) + this.m_uvOffset.y;
                                            characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
                                            characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
                                            break;
                                        }
                                    }
                                    float scale = characterInfo[k].scale;
                                    if ((characterInfo[k].style & FontStyles.Bold) == FontStyles.Bold)
                                    {
                                        scale *= -1f;
                                    }
                                    switch (renderMode)
                                    {
                                        case RenderMode.ScreenSpaceOverlay:
                                            scale *= z / scaleFactor;
                                            break;

                                        case RenderMode.ScreenSpaceCamera:
                                            scale *= !flag7 ? 1f : z;
                                            break;

                                        case RenderMode.WorldSpace:
                                            scale *= z;
                                            break;
                                    }
                                    float f = characterInfo[k].vertex_BL.uv2.x;
                                    float num55 = characterInfo[k].vertex_BL.uv2.y;
                                    float num56 = characterInfo[k].vertex_TR.uv2.x;
                                    float num57 = characterInfo[k].vertex_TR.uv2.y;
                                    float num58 = Mathf.Floor(f);
                                    float num59 = Mathf.Floor(num55);
                                    f -= num58;
                                    num56 -= num58;
                                    num55 -= num59;
                                    num57 -= num59;
                                    characterInfo[k].vertex_BL.uv2 = this.PackUV(f, num55, scale);
                                    characterInfo[k].vertex_TL.uv2 = this.PackUV(f, num57, scale);
                                    characterInfo[k].vertex_TR.uv2 = this.PackUV(num56, num57, scale);
                                    characterInfo[k].vertex_BR.uv2 = this.PackUV(num56, num55, scale);
                                Label_4238:
                                    if ((((this.m_maxVisibleCharacters != -1) && (k >= this.m_maxVisibleCharacters)) || ((this.m_maxVisibleLines != -1) && (lineNumber >= this.m_maxVisibleLines))) || ((this.m_overflowMode == TextOverflowModes.Page) && (characterInfo[k].pageNumber != this.m_pageToDisplay)))
                                    {
                                        characterInfo[k].vertex_BL.position = (Vector3) (characterInfo[k].vertex_BL.position * 0f);
                                        characterInfo[k].vertex_TL.position = (Vector3) (characterInfo[k].vertex_TL.position * 0f);
                                        characterInfo[k].vertex_TR.position = (Vector3) (characterInfo[k].vertex_TR.position * 0f);
                                        characterInfo[k].vertex_BR.position = (Vector3) (characterInfo[k].vertex_BR.position * 0f);
                                    }
                                    else
                                    {
                                        characterInfo[k].vertex_BL.position += vector13;
                                        characterInfo[k].vertex_TL.position += vector13;
                                        characterInfo[k].vertex_TR.position += vector13;
                                        characterInfo[k].vertex_BR.position += vector13;
                                    }
                                    switch (type)
                                    {
                                        case TMP_CharacterType.Character:
                                            this.FillCharacterVertexBuffers(k, num37);
                                            num37 += 4;
                                            break;

                                        case TMP_CharacterType.Sprite:
                                            this.FillSpriteVertexBuffers(k, num38);
                                            num38 += 4;
                                            break;
                                    }
                                Label_43D4:
                                    this.m_textInfo.characterInfo[k].bottomLeft += vector13;
                                    this.m_textInfo.characterInfo[k].topRight += vector13;
                                    this.m_textInfo.characterInfo[k].topLine += vector13.y;
                                    this.m_textInfo.characterInfo[k].bottomLine += vector13.y;
                                    this.m_textInfo.characterInfo[k].baseLine += vector13.y;
                                    this.m_textInfo.lineInfo[lineNumber].ascender = (this.m_textInfo.characterInfo[k].topLine <= this.m_textInfo.lineInfo[lineNumber].ascender) ? this.m_textInfo.lineInfo[lineNumber].ascender : this.m_textInfo.characterInfo[k].topLine;
                                    this.m_textInfo.lineInfo[lineNumber].descender = (this.m_textInfo.characterInfo[k].bottomLine >= this.m_textInfo.lineInfo[lineNumber].descender) ? this.m_textInfo.lineInfo[lineNumber].descender : this.m_textInfo.characterInfo[k].bottomLine;
                                    if ((lineNumber != num42) || (k == (this.m_characterCount - 1)))
                                    {
                                        if (lineNumber != num42)
                                        {
                                            this.m_textInfo.lineInfo[num42].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num42].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num42].descender);
                                            this.m_textInfo.lineInfo[num42].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num42].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num42].ascender);
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
                                            num43 = k;
                                        }
                                    }
                                    else if ((((char.IsPunctuation(c) || char.IsWhiteSpace(c)) || (k == (this.m_characterCount - 1))) && flag6) || (k == 0))
                                    {
                                        num44 = ((k != (this.m_characterCount - 1)) || !char.IsLetterOrDigit(c)) ? (k - 1) : k;
                                        flag6 = false;
                                        num40++;
                                        this.m_textInfo.lineInfo[lineNumber].wordCount++;
                                        TMP_WordInfo item = new TMP_WordInfo {
                                            firstCharacterIndex = num43,
                                            lastCharacterIndex = num44,
                                            characterCount = (num44 - num43) + 1
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
                                            this.DrawUnderlineMesh(zero, end, ref num36, underlineColor);
                                            num39++;
                                        }
                                        else if (k == info2.lastCharacterIndex)
                                        {
                                            switch (c)
                                            {
                                                case ' ':
                                                case '\n':
                                                case '\r':
                                                {
                                                    int lastVisibleCharacterIndex = info2.lastVisibleCharacterIndex;
                                                    end = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                                    break;
                                                }
                                                default:
                                                    end = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                                    break;
                                            }
                                            flag2 = false;
                                            this.DrawUnderlineMesh(zero, end, ref num36, underlineColor);
                                            num39++;
                                        }
                                    }
                                    else if (flag2)
                                    {
                                        flag2 = false;
                                        end = new Vector3(this.m_textInfo.characterInfo[k - 1].topRight.x, this.m_textInfo.characterInfo[k - 1].baseLine + (this.font.fontInfo.Underline * this.m_fontScale), 0f);
                                        this.DrawUnderlineMesh(zero, end, ref num36, underlineColor);
                                        num39++;
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
                                            this.DrawUnderlineMesh(start, vector4, ref num36, underlineColor);
                                            num39++;
                                        }
                                        else if (k == info2.lastCharacterIndex)
                                        {
                                            switch (c)
                                            {
                                                case ' ':
                                                case '\n':
                                                case '\r':
                                                {
                                                    int num61 = info2.lastVisibleCharacterIndex;
                                                    vector4 = new Vector3(this.m_textInfo.characterInfo[num61].topRight.x, this.m_textInfo.characterInfo[num61].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                                    break;
                                                }
                                                default:
                                                    vector4 = new Vector3(this.m_textInfo.characterInfo[k].topRight.x, this.m_textInfo.characterInfo[k].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                                    break;
                                            }
                                            flag3 = false;
                                            this.DrawUnderlineMesh(start, vector4, ref num36, underlineColor);
                                            num39++;
                                        }
                                    }
                                    else if (flag3)
                                    {
                                        flag3 = false;
                                        vector4 = new Vector3(this.m_textInfo.characterInfo[k - 1].topRight.x, this.m_textInfo.characterInfo[k - 1].baseLine + (((this.font.fontInfo.Ascender + this.font.fontInfo.Descender) / 2f) * this.m_fontScale), 0f);
                                        this.DrawUnderlineMesh(start, vector4, ref num36, underlineColor);
                                        num39++;
                                    }
                                    num42 = lineNumber;
                                }
                                this.m_textInfo.characterCount = (short) this.m_characterCount;
                                this.m_textInfo.spriteCount = this.m_spriteCount;
                                this.m_textInfo.lineCount = (short) num41;
                                this.m_textInfo.wordCount = ((num40 == 0) || (this.m_characterCount <= 0)) ? 1 : ((short) num40);
                                this.m_textInfo.pageCount = this.m_pageNumber;
                                this.m_textInfo.meshInfo.uiVertices = this.m_uiVertices;
                                if (this.m_renderMode == TextRenderFlags.Render)
                                {
                                    this.m_uiRenderer.SetVertices(this.m_uiVertices, num37 + (num39 * 12));
                                    if ((this.m_spriteCount > 0) && (this.m_inlineGraphics != null))
                                    {
                                        this.m_inlineGraphics.DrawSprite(this.m_inlineGraphics.uiVertex, this.m_spriteCount);
                                    }
                                }
                                this.m_bounds = new Bounds(new Vector3((this.m_meshExtents.max.x + this.m_meshExtents.min.x) / 2f, (this.m_meshExtents.max.y + this.m_meshExtents.min.y) / 2f, 0f) + vector13, new Vector3(this.m_meshExtents.max.x - this.m_meshExtents.min.x, this.m_meshExtents.max.y - this.m_meshExtents.min.y, 0f));
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

        private float GetPreferredHeight()
        {
            TextOverflowModes overflowMode = this.m_overflowMode;
            this.m_overflowMode = TextOverflowModes.Overflow;
            this.m_renderMode = TextRenderFlags.GetPreferredSizes;
            this.GenerateTextMesh();
            this.m_renderMode = TextRenderFlags.Render;
            this.m_overflowMode = overflowMode;
            Debug.Log("GetPreferredHeight() Called. Returning height of " + this.m_preferredHeight);
            return this.m_preferredHeight;
        }

        private float GetPreferredWidth()
        {
            TextOverflowModes overflowMode = this.m_overflowMode;
            this.m_overflowMode = TextOverflowModes.Overflow;
            this.m_renderMode = TextRenderFlags.GetPreferredSizes;
            this.GenerateTextMesh();
            this.m_renderMode = TextRenderFlags.Render;
            this.m_overflowMode = overflowMode;
            Debug.Log("GetPreferredWidth() Called. Returning width of " + this.m_preferredWidth);
            return this.m_preferredWidth;
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
                this.m_baseMaterial = this.m_fontAsset.material;
                this.m_sharedMaterial = this.m_baseMaterial;
                this.m_isNewBaseMaterial = true;
            }
            else
            {
                if (this.m_fontAsset.characterDictionary == null)
                {
                    this.m_fontAsset.ReadFontDefinition();
                }
                this.m_sharedMaterial = this.m_baseMaterial;
                this.m_isNewBaseMaterial = true;
                if (((this.m_sharedMaterial == null) || (this.m_sharedMaterial.mainTexture == null)) || (this.m_fontAsset.atlas.GetInstanceID() != this.m_sharedMaterial.mainTexture.GetInstanceID()))
                {
                    this.m_sharedMaterial = this.m_fontAsset.material;
                    this.m_baseMaterial = this.m_sharedMaterial;
                    this.m_isNewBaseMaterial = true;
                }
            }
            if (!this.m_fontAsset.characterDictionary.TryGetValue(0x5f, out this.m_cached_Underline_GlyphInfo))
            {
                Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");
            }
            this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
            if (this.m_stencilID == 0)
            {
                if (this.m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                    this.m_maskingMaterial = null;
                }
                this.m_sharedMaterial = this.m_baseMaterial;
            }
            else
            {
                if (this.m_maskingMaterial == null)
                {
                    this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
                }
                else if ((this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID) || this.m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                    this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
                }
                this.m_sharedMaterial = this.m_maskingMaterial;
            }
            this.m_isNewBaseMaterial = false;
            this.m_sharedMaterials.Add(this.m_sharedMaterial);
            this.SetShaderDepth();
            if (this.m_uiRenderer == null)
            {
                this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
            }
            this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
            this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
            this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
        }

        private void MarkLayoutForRebuild()
        {
            if (this.m_rectTransform == null)
            {
                this.m_rectTransform = base.GetComponent<RectTransform>();
            }
            LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
        }

        protected override void OnDestroy()
        {
            if (this.m_maskingMaterial != null)
            {
                MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                this.m_maskingMaterial = null;
            }
            if (this.m_fontMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_fontMaterial);
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            this.havePropertiesChanged = true;
        }

        protected override void OnDisable()
        {
            this.m_isEnabled = false;
            Canvas.willRenderCanvases -= new Canvas.WillRenderCanvases(this.OnPreRenderCanvas);
            this.m_uiRenderer.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
        }

        protected override void OnEnable()
        {
            this.m_isEnabled = true;
            Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.OnPreRenderCanvas);
            if (this.m_canvas == null)
            {
                this.m_canvas = base.GetComponentInParent(typeof(Canvas)) as Canvas;
            }
            if (this.m_uiRenderer.GetMaterial() == null)
            {
                if (this.m_sharedMaterial != null)
                {
                    this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
                }
                else
                {
                    this.m_isNewBaseMaterial = true;
                    this.fontSharedMaterial = this.m_baseMaterial;
                    this.ParentMaskStateChanged();
                }
                this.havePropertiesChanged = true;
                this.m_rectTransformDimensionsChanged = true;
            }
            LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
        }

        private void OnPreRenderCanvas()
        {
            this.loopCountA = 0;
            if (this.m_fontAsset != null)
            {
                if (this.m_rectTransform.hasChanged || this.m_marginsHaveChanged)
                {
                    if (this.m_inlineGraphics != null)
                    {
                        this.m_inlineGraphics.UpdatePivot(this.m_rectTransform.pivot);
                    }
                    if (this.m_rectTransformDimensionsChanged || this.m_marginsHaveChanged)
                    {
                        this.ComputeMarginSize();
                        if (this.m_marginsHaveChanged)
                        {
                            this.m_isScrollRegionSet = false;
                        }
                        this.m_rectTransformDimensionsChanged = false;
                        this.m_marginsHaveChanged = false;
                        this.m_isCalculateSizeRequired = true;
                        this.havePropertiesChanged = true;
                    }
                    if (this.m_isMaskingEnabled)
                    {
                        this.UpdateMask();
                    }
                    this.m_rectTransform.hasChanged = false;
                    Vector3 lossyScale = this.m_rectTransform.lossyScale;
                    if (lossyScale != this.m_previousLossyScale)
                    {
                        if ((!this.havePropertiesChanged && (this.m_previousLossyScale.z != 0f)) && (this.m_text != string.Empty))
                        {
                            this.UpdateSDFScale(this.m_previousLossyScale.z, lossyScale.z);
                        }
                        else
                        {
                            this.havePropertiesChanged = true;
                        }
                        this.m_previousLossyScale = lossyScale;
                    }
                }
                if ((this.havePropertiesChanged || this.m_fontAsset.propertiesChanged) || this.m_isLayoutDirty)
                {
                    if (this.m_canvas == null)
                    {
                        this.m_canvas = base.GetComponentInParent<Canvas>();
                    }
                    if (this.m_canvas != null)
                    {
                        if (this.hasFontAssetChanged || this.m_fontAsset.propertiesChanged)
                        {
                            this.LoadFontAsset();
                            this.hasFontAssetChanged = false;
                            if ((this.m_fontAsset == null) || (this.m_uiRenderer.GetMaterial() == null))
                            {
                                return;
                            }
                            this.m_fontAsset.propertiesChanged = false;
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
                        this.m_isLayoutDirty = false;
                        this.GenerateTextMesh();
                        this.havePropertiesChanged = false;
                    }
                }
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (base.gameObject.activeInHierarchy)
            {
                this.ComputeMarginSize();
                if (this.m_rectTransform != null)
                {
                    this.m_rectTransform.hasChanged = true;
                }
                else
                {
                    this.m_rectTransform = base.GetComponent<RectTransform>();
                    this.m_rectTransform.hasChanged = true;
                }
                if (this.m_isRebuildingLayout)
                {
                    this.m_isLayoutDirty = true;
                }
                else
                {
                    this.havePropertiesChanged = true;
                }
            }
        }

        protected override void OnTransformParentChanged()
        {
            int stencilID = this.m_stencilID;
            this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
            if (stencilID != this.m_stencilID)
            {
                this.ParentMaskStateChanged();
            }
            LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
        }

        private Vector2 PackUV(float x, float y, float scale)
        {
            x = (x % 5f) / 5f;
            y = (y % 5f) / 5f;
            return new Vector2(Mathf.Round(x * 4096f) + y, scale);
        }

        public void ParentMaskStateChanged()
        {
            if (this.m_fontAsset != null)
            {
                this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
                if (this.m_isAwake)
                {
                    if (this.m_stencilID == 0)
                    {
                        if (this.m_maskingMaterial != null)
                        {
                            MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                            this.m_maskingMaterial = null;
                            this.m_sharedMaterial = this.m_baseMaterial;
                        }
                        else if (this.m_fontMaterial != null)
                        {
                            this.m_sharedMaterial = MaterialManager.SetStencil(this.m_fontMaterial, 0);
                        }
                    }
                    else
                    {
                        ShaderUtilities.GetShaderPropertyIDs();
                        if (this.m_fontMaterial != null)
                        {
                            this.m_sharedMaterial = MaterialManager.SetStencil(this.m_fontMaterial, this.m_stencilID);
                        }
                        else if (this.m_maskingMaterial == null)
                        {
                            this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
                            this.m_sharedMaterial = this.m_maskingMaterial;
                        }
                        else if ((this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID) || this.m_isNewBaseMaterial)
                        {
                            MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                            this.m_maskingMaterial = MaterialManager.GetStencilMaterial(this.m_baseMaterial, this.m_stencilID);
                            this.m_sharedMaterial = this.m_maskingMaterial;
                        }
                        if (this.m_isMaskingEnabled)
                        {
                            this.EnableMasking();
                        }
                    }
                    this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
                    this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
                    this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
                }
            }
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

        protected void Reset()
        {
            this.isInputParsingRequired = true;
            this.havePropertiesChanged = true;
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
            int num2 = this.m_uiVertices.Length / 4;
            Array.Resize<UIVertex>(ref this.m_uiVertices, newSize);
            for (int i = num2; i < size; i++)
            {
                int num4 = i * 4;
                this.m_uiVertices[0 + num4].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[1 + num4].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[2 + num4].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[3 + num4].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[0 + num4].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[1 + num4].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[2 + num4].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[3 + num4].tangent = new Vector4(-1f, 0f, 0f, 1f);
            }
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
            this.m_visibleSpriteCount = state.visible_SpriteCount;
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

        private void SaveGlyphVertexInfo(float style_padding, Color32 vertexColor)
        {
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
            vertexColor.a = (this.m_fontColor32.a >= vertexColor.a) ? vertexColor.a : this.m_fontColor32.a;
            if (!this.m_enableVertexGradient)
            {
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
            }
            else
            {
                if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
                {
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
                }
                else
                {
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight;
                    this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight;
                }
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color.a = vertexColor.a;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color.a = vertexColor.a;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color.a = vertexColor.a;
                this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color.a = vertexColor.a;
            }
            if (!this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
            {
                style_padding = 0f;
            }
            Vector2 vector = new Vector2(((this.m_cached_GlyphInfo.x - this.m_padding) - style_padding) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, 1f - ((((this.m_cached_GlyphInfo.y + this.m_padding) + style_padding) + this.m_cached_GlyphInfo.height) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight));
            Vector2 vector2 = new Vector2(vector.x, 1f - (((this.m_cached_GlyphInfo.y - this.m_padding) - style_padding) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasHeight));
            Vector2 vector3 = new Vector2((((this.m_cached_GlyphInfo.x + this.m_padding) + style_padding) + this.m_cached_GlyphInfo.width) / this.m_fontAssetArray[this.m_fontIndex].fontInfo.AtlasWidth, vector.y);
            Vector2 vector4 = new Vector2(vector3.x, vector2.y);
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector4;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = vector3;
            Vector3 vector5 = new Vector3(0f, 0f, -1f);
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.normal = vector5;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.normal = vector5;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.normal = vector5;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.normal = vector5;
            Vector4 vector6 = new Vector4(-1f, 0f, 0f, 1f);
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.tangent = vector6;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.tangent = vector6;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.tangent = vector6;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.tangent = vector6;
        }

        private void SaveSpriteVertexInfo(Color32 vertexColor)
        {
            int num = !this.m_enableExtraPadding ? 0 : 4;
            Vector2 vector = new Vector2((this.m_cached_GlyphInfo.x - num) / ((float) this.m_inlineGraphics.spriteAsset.spriteSheet.width), (this.m_cached_GlyphInfo.y - num) / ((float) this.m_inlineGraphics.spriteAsset.spriteSheet.height));
            Vector2 vector2 = new Vector2(vector.x, ((this.m_cached_GlyphInfo.y + num) + this.m_cached_GlyphInfo.height) / ((float) this.m_inlineGraphics.spriteAsset.spriteSheet.height));
            Vector2 vector3 = new Vector2(((this.m_cached_GlyphInfo.x + num) + this.m_cached_GlyphInfo.width) / ((float) this.m_inlineGraphics.spriteAsset.spriteSheet.width), vector.y);
            Vector2 vector4 = new Vector2(vector3.x, vector2.y);
            vertexColor.a = (this.m_fontColor32.a >= vertexColor.a) ? vertexColor.a : this.m_fontColor32.a;
            TMP_Vertex vertex = new TMP_Vertex {
                position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft,
                uv = vector,
                color = vertexColor
            };
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL = vertex;
            vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
            vertex.uv = vector2;
            vertex.color = vertexColor;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL = vertex;
            vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
            vertex.uv = vector4;
            vertex.color = vertexColor;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR = vertex;
            vertex.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
            vertex.uv = vector3;
            vertex.color = vertexColor;
            this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR = vertex;
        }

        private void SaveWordWrappingState(ref WordWrapState state, int index, int count)
        {
            state.previous_WordBreak = index;
            state.total_CharacterCount = count;
            state.visible_CharacterCount = this.m_visibleCharacterCount;
            state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
            state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
            state.visible_SpriteCount = this.m_visibleSpriteCount;
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
            int num4 = 0;
            this.m_isUsingBold = false;
            this.m_isSprite = false;
            this.m_fontIndex = 0;
            this.m_VisibleCharacters.Clear();
            Array.Clear(this.m_meshAllocCount, 0, 0x11);
            for (int i = 0; chars[i] != 0; i++)
            {
                int num6 = chars[i];
                if ((this.m_isRichText && (num6 == 60)) && this.ValidateHtmlTag(chars, i + 1, out endIndex))
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
                    if (this.m_isSprite)
                    {
                        num4++;
                        num2++;
                        this.m_VisibleCharacters.Add((char) ((ushort) (0xe000 + this.m_spriteIndex)));
                        this.m_isSprite = false;
                    }
                }
                else
                {
                    if (((num6 != 0x20) && (num6 != 9)) && ((num6 != 10) && (num6 != 13)))
                    {
                        size++;
                        this.m_meshAllocCount[this.m_fontIndex]++;
                    }
                    this.m_VisibleCharacters.Add((char) ((ushort) num6));
                    num2++;
                }
            }
            if (num4 > 0)
            {
                if (this.m_inlineGraphics == null)
                {
                    InlineGraphicManager component = base.GetComponent<InlineGraphicManager>();
                    if (component == null)
                    {
                    }
                    this.m_inlineGraphics = base.gameObject.AddComponent<InlineGraphicManager>();
                }
                this.m_inlineGraphics.AllocatedVertexBuffers(num4);
            }
            else if (this.m_inlineGraphics != null)
            {
                this.m_inlineGraphics.ClearUIVertex();
            }
            this.m_spriteCount = num4;
            if ((this.m_textInfo.characterInfo == null) || (num2 > this.m_textInfo.characterInfo.Length))
            {
                this.m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 <= 0x400) ? Mathf.NextPowerOfTwo(num2) : (num2 + 0x100)];
            }
            if (this.m_uiVertices == null)
            {
                this.m_uiVertices = new UIVertex[0];
            }
            if ((size * 4) > this.m_uiVertices.Length)
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
                this.m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2f);
            }
            else
            {
                this.m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0f);
            }
        }

        private void SetFaceColor(Color32 color)
        {
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
                this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
            }
            this.m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_FaceColor, (Color) color);
        }

        private void SetFontBaseMaterial(Material mat)
        {
            Debug.Log("Changing Base Material from [" + ((this.m_lastBaseMaterial != null) ? this.m_lastBaseMaterial.name : "Null") + "] to [" + mat.name + "].");
            this.m_baseMaterial = mat;
            this.m_lastBaseMaterial = mat;
        }

        private void SetFontMaterial(Material mat)
        {
            ShaderUtilities.GetShaderPropertyIDs();
            if (this.m_uiRenderer == null)
            {
                this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
            }
            if (this.m_fontMaterial != null)
            {
                UnityEngine.Object.DestroyImmediate(this.m_fontMaterial);
            }
            if (this.m_maskingMaterial != null)
            {
                MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                this.m_maskingMaterial = null;
            }
            this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
            this.m_fontMaterial = this.CreateMaterialInstance(mat);
            if (this.m_stencilID > 0)
            {
                this.m_fontMaterial = MaterialManager.SetStencil(this.m_fontMaterial, this.m_stencilID);
            }
            this.m_sharedMaterial = this.m_fontMaterial;
            this.SetShaderDepth();
            this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
            this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
            this.m_alignmentPadding = ShaderUtilities.GetFontExtent(this.m_sharedMaterial);
        }

        private void SetMeshArrays(int size)
        {
            int num = size * 4;
            this.m_uiVertices = new UIVertex[num];
            for (int i = 0; i < size; i++)
            {
                int num3 = i * 4;
                this.m_uiVertices[0 + num3].position = Vector3.zero;
                this.m_uiVertices[1 + num3].position = Vector3.zero;
                this.m_uiVertices[2 + num3].position = Vector3.zero;
                this.m_uiVertices[3 + num3].position = Vector3.zero;
                this.m_uiVertices[0 + num3].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[1 + num3].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[2 + num3].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[3 + num3].normal = new Vector3(0f, 0f, -1f);
                this.m_uiVertices[0 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[1 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[2 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
                this.m_uiVertices[3 + num3].tangent = new Vector4(-1f, 0f, 0f, 1f);
            }
            this.m_uiRenderer.SetVertices(this.m_uiVertices, num);
        }

        private void SetOutlineColor(Color32 color)
        {
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
                this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
            }
            this.m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_OutlineColor, (Color) color);
        }

        private void SetOutlineThickness(float thickness)
        {
            if (this.m_fontMaterial == null)
            {
                this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
                this.m_uiRenderer.SetMaterial(this.m_fontMaterial, null);
            }
            thickness = Mathf.Clamp01(thickness);
            this.m_uiRenderer.GetMaterial().SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
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

        private void SetShaderDepth()
        {
            if (this.m_canvas != null)
            {
                if ((this.m_canvas.renderMode == RenderMode.ScreenSpaceOverlay) || this.m_isOverlay)
                {
                    this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
                }
                else
                {
                    this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
                }
            }
        }

        private void SetSharedFontMaterial(Material mat)
        {
            ShaderUtilities.GetShaderPropertyIDs();
            if (this.m_uiRenderer == null)
            {
                this.m_uiRenderer = base.GetComponent<CanvasRenderer>();
            }
            if (mat == null)
            {
                mat = this.m_baseMaterial;
                this.m_isNewBaseMaterial = true;
            }
            this.m_stencilID = MaterialManager.GetStencilID(base.gameObject);
            if (this.m_stencilID == 0)
            {
                if (this.m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                    this.m_maskingMaterial = null;
                }
                this.m_baseMaterial = mat;
            }
            else
            {
                if (this.m_maskingMaterial == null)
                {
                    this.m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, this.m_stencilID);
                }
                else if ((this.m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != this.m_stencilID) || this.m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(this.m_maskingMaterial);
                    this.m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, this.m_stencilID);
                }
                mat = this.m_maskingMaterial;
            }
            this.m_isNewBaseMaterial = false;
            this.m_sharedMaterial = mat;
            this.SetShaderDepth();
            this.m_uiRenderer.SetMaterial(this.m_sharedMaterial, null);
            this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
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
                Debug.Log("Updating Env Matrix...");
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
            if (this.m_rectTransform != null)
            {
                if (!ShaderUtilities.isInitialized)
                {
                    ShaderUtilities.GetShaderPropertyIDs();
                }
                this.m_isScrollRegionSet = true;
                float num = Mathf.Min(Mathf.Min(this.m_margin.x, this.m_margin.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
                float num2 = Mathf.Min(Mathf.Min(this.m_margin.y, this.m_margin.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
                num = (num <= 0f) ? 0f : num;
                num2 = (num2 <= 0f) ? 0f : num2;
                float z = (((this.m_rectTransform.rect.width - Mathf.Max(this.m_margin.x, 0f)) - Mathf.Max(this.m_margin.z, 0f)) / 2f) + num;
                float w = (((this.m_rectTransform.rect.height - Mathf.Max(this.m_margin.y, 0f)) - Mathf.Max(this.m_margin.w, 0f)) / 2f) + num2;
                float introduced12 = Mathf.Max(this.m_margin.x, 0f);
                Vector2 vector = this.m_rectTransform.localPosition + new Vector3(((0.5f - this.m_rectTransform.pivot.x) * this.m_rectTransform.rect.width) + ((introduced12 - Mathf.Max(this.m_margin.z, 0f)) / 2f), ((0.5f - this.m_rectTransform.pivot.y) * this.m_rectTransform.rect.height) + ((-Mathf.Max(this.m_margin.y, 0f) + Mathf.Max(this.m_margin.w, 0f)) / 2f));
                Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
                this.m_sharedMaterial.SetVector(ShaderUtilities.ID_MaskCoord, vector2);
            }
        }

        private void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
        {
            this.m_textInfo.characterInfo = characterInfo;
            this.m_textInfo.characterCount = (short) characterCount;
        }

        public void UpdateMeshPadding()
        {
            Material[] materials = new Material[] { this.m_uiRenderer.GetMaterial() };
            this.m_padding = ShaderUtilities.GetPadding(materials, this.m_enableExtraPadding, this.m_isUsingBold);
            this.havePropertiesChanged = true;
        }

        private void UpdateSDFScale(float prevScale, float newScale)
        {
            for (int i = 0; i < this.m_uiVertices.Length; i++)
            {
                this.m_uiVertices[i].uv1.y = (this.m_uiVertices[i].uv1.y / prevScale) * newScale;
            }
            this.m_uiRenderer.SetVertices(this.m_uiVertices, this.m_uiVertices.Length);
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
                        this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
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
                        this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SubscriptOffset * this.m_fontScale;
                        return true;

                    case 0x2a7:
                        num9 = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_xAdvance = (num9 * this.m_fontScale) * this.m_fontAsset.fontInfo.TabWidth;
                        return true;

                    case 0x2ad:
                        this.m_currentFontSize *= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
                        this.m_baselineOffset = this.m_fontAsset.fontInfo.SuperscriptOffset * this.m_fontScale;
                        return true;

                    case 0x434:
                        this.m_currentFontSize /= (this.m_fontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_fontAsset.fontInfo.SubSize;
                        this.m_baselineOffset = 0f;
                        this.m_fontScale = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize;
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
                        this.m_fontIndex = (int) this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        if (this.m_fontAssetArray[this.m_fontIndex] == null)
                        {
                            this.m_fontAssetArray[this.m_fontIndex] = this.m_fontAsset;
                        }
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

                    case 0x64d:
                        this.m_width = this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        return true;

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

                    case 0x89c:
                        this.m_width = 0f;
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
                        this.m_spriteIndex = (int) this.ConvertToFloat(this.m_htmlTag, num4, index - 1, decimalPointIndex);
                        this.m_isSprite = true;
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

        public Bounds bounds
        {
            get
            {
                if (this.m_uiVertices != null)
                {
                    return this.m_bounds;
                }
                return new Bounds();
            }
        }

        public CanvasRenderer canvasRenderer
        {
            get
            {
                return this.m_uiRenderer;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
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
                if (this.m_fontColor != value)
                {
                    this.havePropertiesChanged = true;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
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
                    this.m_isRecalculateScaleRequired = true;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
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

        public float flexibleHeight
        {
            get
            {
                return this.m_flexibleHeight;
            }
        }

        public float flexibleWidth
        {
            get
            {
                return this.m_flexibleWidth;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
                }
            }
        }

        protected Material fontBaseMaterial
        {
            get
            {
                return this.m_baseMaterial;
            }
            set
            {
                if (this.m_baseMaterial != value)
                {
                    this.SetFontBaseMaterial(value);
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

        public Material fontSharedMaterial
        {
            get
            {
                return this.m_uiRenderer.GetMaterial();
            }
            set
            {
                if (this.m_uiRenderer.GetMaterial() != value)
                {
                    this.m_isNewBaseMaterial = true;
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
                this.havePropertiesChanged = true;
                this.m_isCalculateSizeRequired = true;
                this.MarkLayoutForRebuild();
                this.m_fontSize = value;
                if (!this.m_enableAutoSizing)
                {
                    this.m_fontSizeBase = this.m_fontSize;
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
                if (this.m_isOverlay != value)
                {
                    this.m_isOverlay = value;
                    this.SetShaderDepth();
                    this.havePropertiesChanged = true;
                }
            }
        }

        public int layoutPriority
        {
            get
            {
                return this.m_layoutPriority;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
                    this.m_lineSpacing = value;
                }
            }
        }

        public Vector4 margin
        {
            get
            {
                return this.m_margin;
            }
            set
            {
                if (this.m_margin != value)
                {
                    this.m_margin = value;
                    this.havePropertiesChanged = true;
                    this.m_marginsHaveChanged = true;
                }
            }
        }

        public Vector4 maskOffset
        {
            get
            {
                return this.m_maskOffset;
            }
            set
            {
                this.m_maskOffset = value;
                this.UpdateMask();
                this.havePropertiesChanged = true;
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

        public UIVertex[] mesh
        {
            get
            {
                return this.m_uiVertices;
            }
        }

        public float minHeight
        {
            get
            {
                return this.m_minHeight;
            }
        }

        public float minWidth
        {
            get
            {
                return this.m_minWidth;
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
                this.m_isRecalculateScaleRequired = true;
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
                    this.m_isCalculateSizeRequired = true;
                    this.MarkLayoutForRebuild();
                    this.m_paragraphSpacing = value;
                }
            }
        }

        public float preferredHeight
        {
            get
            {
                return ((this.m_preferredHeight != 9999f) ? this.m_preferredHeight : this.m_renderedHeight);
            }
        }

        public float preferredWidth
        {
            get
            {
                return ((this.m_preferredWidth != 9999f) ? this.m_preferredWidth : this.m_renderedWidth);
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (this.m_rectTransform == null)
                {
                    this.m_rectTransform = base.GetComponent<RectTransform>();
                }
                return this.m_rectTransform;
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
                this.m_isCalculateSizeRequired = true;
                this.MarkLayoutForRebuild();
                this.isInputParsingRequired = true;
            }
        }

        public int sortingLayerID
        {
            get
            {
                return this.m_sortingLayerID;
            }
            set
            {
                this.m_sortingLayerID = value;
            }
        }

        public int sortingOrder
        {
            get
            {
                return this.m_sortingOrder;
            }
            set
            {
                this.m_sortingOrder = value;
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
                this.m_isCalculateSizeRequired = true;
                this.isInputParsingRequired = true;
                this.MarkLayoutForRebuild();
                this.m_text = value;
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

        private enum AutoLayoutPhase
        {
            Horizontal,
            Vertical
        }

        private enum TextInputSources
        {
            Text,
            SetText,
            SetCharArray
        }
    }
}

