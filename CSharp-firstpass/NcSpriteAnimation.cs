using System;
using UnityEngine;

public class NcSpriteAnimation : NcEffectAniBehaviour, IPooledMonoBehaviour
{
    public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;
    public bool m_bAutoDestruct;
    protected bool m_bBreakLoop;
    [HideInInspector]
    public bool m_bBuildSpriteObj;
    protected bool m_bCreateBuiltInPlane;
    protected bool m_bInPartLoop;
    public bool m_bLoop = true;
    [HideInInspector]
    public bool m_bNeedRebuildAlphaChannel;
    public bool m_bTrimCenterAlign;
    [HideInInspector]
    public AnimationCurve m_curveAlphaWeight;
    private bool m_enabled = true;
    public float m_fDelayTime;
    public float m_fFps = 10f;
    protected float m_fStartTime;
    public float m_fUvScale = 1f;
    private bool m_gotten;
    private bool m_inited;
    public NcSpriteFactory.MESH_TYPE m_MeshType;
    protected Vector2[] m_MeshUVsByTileTexture;
    protected NcSpriteFactory m_NcSpriteFactoryCom;
    protected GameObject m_NcSpriteFactoryPrefab;
    private Renderer m_NcSpriteFactoryPrefabRenderer;
    public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;
    public int m_nFrameCount;
    protected int m_nLastIndex = -999;
    protected int m_nLastSeqIndex = -1;
    public int m_nLoopFrameCount;
    public int m_nLoopingCount;
    public int m_nLoopStartFrame;
    public int m_nSelectFrame;
    public int m_nSpriteFactoryIndex;
    public int m_nStartFrame;
    public int m_nTilingX = 2;
    public int m_nTilingY = 2;
    public PLAYMODE m_PlayMode;
    protected Renderer m_Renderer;
    protected Vector2 m_size;
    public TEXTURE_TYPE m_TextureType;

    private void Awake()
    {
        this.OnCreate();
    }

    private int CalcPartLoopInfo(int nSeqIndex, ref int nLoopCount)
    {
        if (this.m_nLoopFrameCount <= 0)
        {
            return 0;
        }
        if (this.m_bBreakLoop)
        {
            nLoopCount = this.GetPartLoopCount(nSeqIndex);
            this.UpdateEndAnimation();
            this.m_bBreakLoop = false;
            return (this.m_nLoopStartFrame + this.m_nLoopFrameCount);
        }
        if (nSeqIndex < this.m_nLoopStartFrame)
        {
            return nSeqIndex;
        }
        this.m_bInPartLoop = true;
        int partLoopCount = this.GetPartLoopCount(this.m_nLastSeqIndex);
        int partLoopFrameIndex = this.GetPartLoopFrameIndex(nSeqIndex);
        nLoopCount = this.GetPartLoopCount(nSeqIndex);
        int num3 = 0;
        int num4 = partLoopCount;
        while (num4 < Mathf.Min(nLoopCount, this.m_nLoopFrameCount - 1))
        {
            if (base.transform.parent != null)
            {
                base.transform.parent.SendMessage("OnSpriteAnimationLoopStart", (int) nLoopCount, SendMessageOptions.DontRequireReceiver);
            }
            num4++;
            num3++;
        }
        if ((0 < this.m_nLoopingCount) && (this.m_nLoopingCount <= nLoopCount))
        {
            this.m_bInPartLoop = false;
            if (base.transform.parent != null)
            {
                base.transform.parent.SendMessage("OnSpriteAnimationLoopEnd", (int) nLoopCount, SendMessageOptions.DontRequireReceiver);
            }
            if (this.m_nFrameCount <= partLoopFrameIndex)
            {
                partLoopFrameIndex = this.m_nFrameCount - 1;
                this.UpdateEndAnimation();
            }
        }
        return partLoopFrameIndex;
    }

    private void CreateBuiltInPlane(int nSelIndex)
    {
        if (!this.m_bCreateBuiltInPlane)
        {
            this.m_bCreateBuiltInPlane = true;
            if (base.m_MeshFilter == null)
            {
                if (base.gameObject.GetComponent<MeshFilter>() != null)
                {
                    base.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
                }
                else
                {
                    base.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
                }
            }
            NcSpriteFactory.CreatePlane(base.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], (this.m_TextureType != TEXTURE_TYPE.TileTexture) ? this.m_bTrimCenterAlign : false, this.m_AlignType, this.m_MeshType);
        }
    }

    public override int GetAnimationState()
    {
        if (!this.m_enabled || !NcEffectBehaviour.IsActive(base.gameObject))
        {
            return -1;
        }
        if ((this.m_fStartTime != 0f) && base.IsEndAnimation())
        {
            return 0;
        }
        return 1;
    }

    public float GetDurationTime()
    {
        return (((this.m_PlayMode != PLAYMODE.PINGPONG) ? ((float) this.m_nFrameCount) : ((float) ((this.m_nFrameCount * 2) - 1))) / this.m_fFps);
    }

    public int GetMaxFrameCount()
    {
        if (this.m_TextureType == TEXTURE_TYPE.TileTexture)
        {
            return (this.m_nTilingX * this.m_nTilingY);
        }
        return this.m_NcSpriteFrameInfos.Length;
    }

    private int GetPartLoopCount(int nSeqIndex)
    {
        if (nSeqIndex < 0)
        {
            return -1;
        }
        int num = nSeqIndex - this.m_nLoopStartFrame;
        if (num < 0)
        {
            return -1;
        }
        int num2 = num / this.m_nLoopFrameCount;
        if ((this.m_nLoopingCount != 0) && (num2 >= this.m_nLoopingCount))
        {
            return this.m_nLoopingCount;
        }
        return num2;
    }

    private int GetPartLoopFrameIndex(int nSeqIndex)
    {
        if (nSeqIndex < 0)
        {
            return -1;
        }
        int num = nSeqIndex - this.m_nLoopStartFrame;
        if (num < 0)
        {
            return nSeqIndex;
        }
        int num2 = num / this.m_nLoopFrameCount;
        if ((this.m_nLoopingCount != 0) && (num2 >= this.m_nLoopingCount))
        {
            return ((num - (this.m_nLoopFrameCount * (this.m_nLoopingCount - 1))) + this.m_nLoopStartFrame);
        }
        return ((num % this.m_nLoopFrameCount) + this.m_nLoopStartFrame);
    }

    public int GetShowIndex()
    {
        return (this.m_nLastIndex + this.m_nStartFrame);
    }

    public int GetValidFrameCount()
    {
        if (this.m_TextureType == TEXTURE_TYPE.TileTexture)
        {
            return ((this.m_nTilingX * this.m_nTilingY) - this.m_nStartFrame);
        }
        return (this.m_NcSpriteFrameInfos.Length - this.m_nStartFrame);
    }

    public bool IsEmptyFrame()
    {
        return this.m_NcSpriteFrameInfos[this.m_nSelectFrame].m_bEmptyFrame;
    }

    public bool IsInPartLoop()
    {
        return this.m_bInPartLoop;
    }

    public void OnCreate()
    {
        if (!this.m_inited)
        {
            this.m_inited = true;
            if ((this.m_NcSpriteFactoryPrefab == null) && (base.gameObject.GetComponent<NcSpriteFactory>() != null))
            {
                this.NcSpriteFactoryPrefab = base.gameObject;
            }
            if ((this.m_NcSpriteFactoryPrefab != null) && (this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
            {
                this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
            }
            if ((base.m_MeshFilter == null) && (base.gameObject.GetComponent<MeshFilter>() != null))
            {
                base.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
            }
            if (this.m_nLoopFrameCount == 0)
            {
                this.m_nLoopFrameCount = this.m_nFrameCount - this.m_nLoopStartFrame;
            }
        }
    }

    public void OnGet()
    {
        if (!this.m_gotten)
        {
            this.m_gotten = true;
            this.ResetLocalValue();
            if (this.m_Renderer == null)
            {
                this.SetEnable(false);
            }
            else
            {
                this.SetEnable(true);
                if (this.m_PlayMode == PLAYMODE.SELECT)
                {
                    this.SetIndex(this.m_nSelectFrame);
                }
                else if (0f < this.m_fDelayTime)
                {
                    this.m_Renderer.enabled = false;
                }
                else if (this.m_PlayMode == PLAYMODE.RANDOM)
                {
                    this.SetIndex(UnityEngine.Random.Range(0, this.m_nFrameCount - 1));
                }
                else
                {
                    base.InitAnimationTimer();
                    this.SetIndex(0);
                }
            }
        }
    }

    public void OnRecycle()
    {
        this.m_gotten = false;
    }

    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
        this.m_fDelayTime *= fSpeedRate;
        this.m_fFps *= fSpeedRate;
    }

    public override void ResetAnimation()
    {
        this.m_nLastIndex = -1;
        this.m_nLastSeqIndex = -1;
        if (!this.m_enabled)
        {
            this.SetEnable(true);
        }
        this.Start();
    }

    private void ResetLocalValue()
    {
        this.m_size = new Vector2(1f / ((float) this.m_nTilingX), 1f / ((float) this.m_nTilingY));
        this.m_Renderer = base.GetComponent<Renderer>();
        this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
        this.m_nFrameCount = (this.m_nFrameCount > 0) ? this.m_nFrameCount : (this.m_nTilingX * this.m_nTilingY);
        this.m_nLastIndex = -999;
        this.m_nLastSeqIndex = -1;
        this.m_bInPartLoop = false;
        this.m_bBreakLoop = false;
    }

    public void SetBreakLoop()
    {
        this.m_bBreakLoop = true;
    }

    private void SetEnable(bool bEnable)
    {
        this.m_enabled = bEnable;
        if (null != this.m_Renderer)
        {
            this.m_Renderer.enabled = bEnable;
        }
    }

    private void SetIndex(int nSeqIndex)
    {
        if (this.m_Renderer != null)
        {
            int nSelIndex = this.m_nLastSeqIndex = nSeqIndex;
            int nLoopCount = nSeqIndex / this.m_nFrameCount;
            switch (this.m_PlayMode)
            {
                case PLAYMODE.DEFAULT:
                    if (!this.m_bLoop)
                    {
                        nSelIndex = nSeqIndex % this.m_nFrameCount;
                        break;
                    }
                    nSelIndex = this.CalcPartLoopInfo(nSeqIndex, ref nLoopCount) % this.m_nFrameCount;
                    break;

                case PLAYMODE.INVERSE:
                    nSelIndex = (this.m_nFrameCount - (nSelIndex % this.m_nFrameCount)) - 1;
                    break;

                case PLAYMODE.PINGPONG:
                    nLoopCount = nSelIndex / ((this.m_nFrameCount * 2) - ((nSelIndex != 0) ? 2 : 1));
                    nSelIndex = nSelIndex % ((this.m_nFrameCount * 2) - ((nSelIndex != 0) ? 2 : 1));
                    if (this.m_nFrameCount <= nSelIndex)
                    {
                        nSelIndex = (this.m_nFrameCount - (nSelIndex % this.m_nFrameCount)) - 2;
                    }
                    break;

                case PLAYMODE.SELECT:
                    nSelIndex = nSeqIndex % this.m_nFrameCount;
                    break;
            }
            if (nSelIndex != this.m_nLastIndex)
            {
                if (this.m_TextureType == TEXTURE_TYPE.TileTexture)
                {
                    int num3 = (nSelIndex + this.m_nStartFrame) % this.m_nTilingX;
                    int num4 = (nSelIndex + this.m_nStartFrame) / this.m_nTilingX;
                    Vector2 vector = new Vector2(num3 * this.m_size.x, (1f - this.m_size.y) - (num4 * this.m_size.y));
                    if (!this.UpdateMeshUVsByTileTexture(new Rect(vector.x, vector.y, this.m_size.x, this.m_size.y)))
                    {
                        this.m_Renderer.material.mainTextureOffset = vector;
                        this.m_Renderer.material.mainTextureScale = this.m_size;
                        base.AddRuntimeMaterial(this.m_Renderer.material);
                    }
                }
                else if (this.m_TextureType == TEXTURE_TYPE.TrimTexture)
                {
                    this.UpdateSpriteTexture(nSelIndex, true);
                }
                else if (this.m_TextureType == TEXTURE_TYPE.SpriteFactory)
                {
                    this.UpdateFactoryTexture(nSelIndex, true);
                }
                if (this.m_NcSpriteFactoryCom != null)
                {
                    this.m_NcSpriteFactoryCom.OnAnimationChangingFrame(this, this.m_nLastIndex, nSelIndex, nLoopCount);
                }
                this.m_nLastIndex = nSelIndex;
            }
        }
    }

    public void SetSelectFrame(int nSelFrame)
    {
        this.m_nSelectFrame = nSelFrame;
        this.SetIndex(this.m_nSelectFrame);
    }

    public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, bool bRunImmediate)
    {
        if (this.m_NcSpriteFactoryCom == null)
        {
            if ((this.m_NcSpriteFactoryPrefab == null) || (this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() == null))
            {
                return;
            }
            this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
        }
        NcSpriteFactory.NcSpriteNode spriteNode = this.m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex);
        this.m_bBuildSpriteObj = false;
        this.m_bAutoDestruct = false;
        this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
        this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
        this.m_nStartFrame = 0;
        DebugHelper.Assert(spriteNode != null);
        if (spriteNode != null)
        {
            this.m_nFrameCount = spriteNode.m_nFrameCount;
            this.m_fFps = spriteNode.m_fFps;
            this.m_bLoop = spriteNode.m_bLoop;
            this.m_nLoopStartFrame = spriteNode.m_nLoopStartFrame;
            this.m_nLoopFrameCount = spriteNode.m_nLoopFrameCount;
            this.m_nLoopingCount = spriteNode.m_nLoopingCount;
            this.m_NcSpriteFrameInfos = spriteNode.m_FrameInfos;
        }
        this.ResetLocalValue();
    }

    private void Start()
    {
        this.OnGet();
    }

    private void Update()
    {
        if ((this.m_enabled && (this.m_PlayMode != PLAYMODE.SELECT)) && ((this.m_Renderer != null) && ((this.m_nTilingX * this.m_nTilingY) != 0)))
        {
            if (this.m_fDelayTime != 0f)
            {
                if (NcEffectBehaviour.GetEngineTime() < (this.m_fStartTime + this.m_fDelayTime))
                {
                    return;
                }
                this.m_fDelayTime = 0f;
                base.InitAnimationTimer();
                this.m_Renderer.enabled = true;
            }
            if (this.m_PlayMode != PLAYMODE.RANDOM)
            {
                int nSeqIndex = (int) (base.m_Timer.GetTime() * this.m_fFps);
                if ((nSeqIndex == 0) && (this.m_NcSpriteFactoryCom != null))
                {
                    this.m_NcSpriteFactoryCom.OnAnimationStartFrame(this);
                }
                if ((this.m_NcSpriteFactoryCom != null) && (this.m_nFrameCount <= 0))
                {
                    this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 0);
                }
                else
                {
                    if (((this.m_PlayMode != PLAYMODE.PINGPONG) ? this.m_nFrameCount : ((this.m_nFrameCount * 2) - 1)) <= nSeqIndex)
                    {
                        if (!this.m_bLoop)
                        {
                            if ((this.m_NcSpriteFactoryCom == null) || !this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, 1))
                            {
                                this.UpdateEndAnimation();
                            }
                            return;
                        }
                        if (this.m_PlayMode == PLAYMODE.PINGPONG)
                        {
                            if (((this.m_NcSpriteFactoryCom != null) && ((nSeqIndex % ((this.m_nFrameCount * 2) - 2)) == 1)) && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, nSeqIndex / ((this.m_nFrameCount * 2) - 1)))
                            {
                                return;
                            }
                        }
                        else if (((this.m_NcSpriteFactoryCom != null) && ((nSeqIndex % this.m_nFrameCount) == 0)) && this.m_NcSpriteFactoryCom.OnAnimationLastFrame(this, nSeqIndex / this.m_nFrameCount))
                        {
                            return;
                        }
                    }
                    this.SetIndex(nSeqIndex);
                }
            }
        }
    }

    private void UpdateBuiltInPlane(int nSelIndex)
    {
        NcSpriteFactory.UpdatePlane(base.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[nSelIndex], (this.m_TextureType != TEXTURE_TYPE.TileTexture) ? this.m_bTrimCenterAlign : false, this.m_AlignType);
        NcSpriteFactory.UpdateMeshUVs(base.m_MeshFilter, this.m_NcSpriteFrameInfos[nSelIndex].m_TextureUvOffset);
    }

    private void UpdateEndAnimation()
    {
        this.SetEnable(false);
        base.OnEndAnimation();
        if (this.m_bAutoDestruct)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    public bool UpdateFactoryMaterial()
    {
        if (this.m_NcSpriteFactoryPrefab == null)
        {
            return false;
        }
        if (((this.m_NcSpriteFactoryPrefabRenderer == null) || (this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial == null)) || (this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial.mainTexture == null))
        {
            return false;
        }
        if (base.GetRenderer() == null)
        {
            return false;
        }
        if (this.m_NcSpriteFactoryCom == null)
        {
            return false;
        }
        if ((this.m_nSpriteFactoryIndex < 0) || (this.m_NcSpriteFactoryCom.GetSpriteNodeCount() <= this.m_nSpriteFactoryIndex))
        {
            return false;
        }
        if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation)
        {
            return false;
        }
        base.GetRenderer().sharedMaterial = this.m_NcSpriteFactoryPrefabRenderer.sharedMaterial;
        return true;
    }

    private void UpdateFactoryTexture(int nSelIndex, bool bShowEffect)
    {
        nSelIndex += this.m_nStartFrame;
        if (((((this.m_NcSpriteFrameInfos != null) && (nSelIndex >= 0)) && (this.m_NcSpriteFrameInfos.Length > nSelIndex)) && this.UpdateFactoryMaterial()) && this.m_NcSpriteFactoryCom.IsValidFactory())
        {
            this.CreateBuiltInPlane(nSelIndex);
            this.UpdateBuiltInPlane(nSelIndex);
        }
    }

    private bool UpdateMeshUVsByTileTexture(Rect uv)
    {
        if ((base.m_MeshFilter != null) && (this.m_MeshUVsByTileTexture == null))
        {
            return false;
        }
        if (base.m_MeshFilter == null)
        {
            base.m_MeshFilter = (MeshFilter) base.gameObject.GetComponent(typeof(MeshFilter));
        }
        if ((base.m_MeshFilter == null) || (base.m_MeshFilter.sharedMesh == null))
        {
            return false;
        }
        if (this.m_MeshUVsByTileTexture == null)
        {
            for (int j = 0; j < base.m_MeshFilter.sharedMesh.uv.Length; j++)
            {
                if ((base.m_MeshFilter.sharedMesh.uv[j].x != 0f) && (base.m_MeshFilter.sharedMesh.uv[j].x != 1f))
                {
                    return false;
                }
                if ((base.m_MeshFilter.sharedMesh.uv[j].y != 0f) && (base.m_MeshFilter.sharedMesh.uv[j].y != 1f))
                {
                    return false;
                }
            }
            this.m_MeshUVsByTileTexture = base.m_MeshFilter.sharedMesh.uv;
        }
        Vector2[] vectorArray = new Vector2[this.m_MeshUVsByTileTexture.Length];
        for (int i = 0; i < this.m_MeshUVsByTileTexture.Length; i++)
        {
            if (this.m_MeshUVsByTileTexture[i].x == 0f)
            {
                vectorArray[i].x = uv.x;
            }
            if (this.m_MeshUVsByTileTexture[i].y == 0f)
            {
                vectorArray[i].y = uv.y;
            }
            if (this.m_MeshUVsByTileTexture[i].x == 1f)
            {
                vectorArray[i].x = uv.x + uv.width;
            }
            if (this.m_MeshUVsByTileTexture[i].y == 1f)
            {
                vectorArray[i].y = uv.y + uv.height;
            }
        }
        base.m_MeshFilter.mesh.uv = vectorArray;
        return true;
    }

    private void UpdateSpriteTexture(int nSelIndex, bool bShowEffect)
    {
        nSelIndex += this.m_nStartFrame;
        if (((this.m_NcSpriteFrameInfos != null) && (nSelIndex >= 0)) && (this.m_NcSpriteFrameInfos.Length > nSelIndex))
        {
            this.CreateBuiltInPlane(nSelIndex);
            this.UpdateBuiltInPlane(nSelIndex);
        }
    }

    public GameObject NcSpriteFactoryPrefab
    {
        get
        {
            return this.m_NcSpriteFactoryPrefab;
        }
        set
        {
            this.m_NcSpriteFactoryPrefab = value;
            if (null != this.m_NcSpriteFactoryPrefab)
            {
                this.m_NcSpriteFactoryPrefabRenderer = this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>();
            }
        }
    }

    public enum PLAYMODE
    {
        DEFAULT,
        INVERSE,
        PINGPONG,
        RANDOM,
        SELECT
    }

    public enum TEXTURE_TYPE
    {
        TileTexture,
        TrimTexture,
        SpriteFactory
    }
}

