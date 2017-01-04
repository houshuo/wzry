using System;
using UnityEngine;

public class NcSpriteTexture : NcEffectBehaviour
{
    public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;
    protected GameObject m_EffectObject;
    public float m_fUvScale = 1f;
    public NcSpriteFactory.MESH_TYPE m_MeshType;
    protected NcSpriteFactory m_NcSpriteFactoryCom;
    public GameObject m_NcSpriteFactoryPrefab;
    public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;
    public int m_nFrameIndex;
    public int m_nSpriteFactoryIndex;

    private void Awake()
    {
        if ((this.m_NcSpriteFactoryPrefab == null) && (base.gameObject.GetComponent<NcSpriteFactory>() != null))
        {
            this.m_NcSpriteFactoryPrefab = base.gameObject;
        }
        if ((this.m_NcSpriteFactoryPrefab != null) && (this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
        {
            this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
        }
    }

    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
    }

    public override void OnUpdateToolData()
    {
    }

    public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, int nFrameIndex, bool bRunImmediate)
    {
        if (this.m_NcSpriteFactoryCom == null)
        {
            if ((this.m_NcSpriteFactoryPrefab == null) || (this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() == null))
            {
                return;
            }
            this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
        }
        this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
        if (this.m_NcSpriteFactoryCom.IsValidFactory())
        {
            this.m_NcSpriteFrameInfos = this.m_NcSpriteFactoryCom.GetSpriteNode(nSpriteFactoryIndex).m_FrameInfos;
            this.m_nFrameIndex = (0 > nFrameIndex) ? this.m_nFrameIndex : nFrameIndex;
            this.m_nFrameIndex = ((this.m_NcSpriteFrameInfos.Length != 0) && (this.m_NcSpriteFrameInfos.Length > this.m_nFrameIndex)) ? this.m_nFrameIndex : 0;
            this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
            if (bRunImmediate)
            {
                this.UpdateSpriteTexture(bRunImmediate);
            }
        }
    }

    private void Start()
    {
        this.UpdateSpriteTexture(true);
    }

    public bool UpdateSpriteMaterial()
    {
        if (this.m_NcSpriteFactoryPrefab == null)
        {
            return false;
        }
        if (((this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>() == null) || (this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial == null)) || (this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial.mainTexture == null))
        {
            return false;
        }
        if (base.GetComponent<Renderer>() == null)
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
        if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture)
        {
            return false;
        }
        base.GetComponent<Renderer>().sharedMaterial = this.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
        return true;
    }

    private void UpdateSpriteTexture(bool bShowEffect)
    {
        if (this.UpdateSpriteMaterial() && this.m_NcSpriteFactoryCom.IsValidFactory())
        {
            if (this.m_NcSpriteFrameInfos.Length == 0)
            {
                this.SetSpriteFactoryIndex(this.m_nSpriteFactoryIndex, this.m_nFrameIndex, false);
            }
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
            NcSpriteFactory.CreatePlane(base.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[this.m_nFrameIndex], false, this.m_AlignType, this.m_MeshType);
            NcSpriteFactory.UpdateMeshUVs(base.m_MeshFilter, this.m_NcSpriteFrameInfos[this.m_nFrameIndex].m_TextureUvOffset);
            if (bShowEffect)
            {
                this.m_EffectObject = this.m_NcSpriteFactoryCom.CreateSpriteEffect(this.m_nSpriteFactoryIndex, base.transform);
            }
        }
    }
}

