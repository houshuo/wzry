using System;
using System.Collections.Generic;
using UnityEngine;

public class NcSpriteFactory : NcEffectBehaviour
{
    protected bool m_bbInstance;
    protected bool m_bEndSprite = true;
    public bool m_bNeedRebuild = true;
    public bool m_bSequenceMode;
    public bool m_bShowEffect = true;
    public bool m_bTestMode = true;
    public bool m_bTrimAlpha = true;
    public bool m_bTrimBlack = true;
    public GameObject m_CurrentEffect;
    public float m_fTextureRatio = 1f;
    public float m_fUvScale = 1f;
    public int m_nBuildStartIndex;
    public int m_nCurrentIndex;
    public int m_nMaxAtlasTextureSize = 0x800;
    public SHOW_TYPE m_ShowType = SHOW_TYPE.SPRITE;
    public List<NcSpriteNode> m_SpriteList;
    public SPRITE_TYPE m_SpriteType;

    public int AddSpriteNode()
    {
        NcSpriteNode item = new NcSpriteNode();
        if (this.m_SpriteList == null)
        {
            this.m_SpriteList = new List<NcSpriteNode>();
        }
        this.m_SpriteList.Add(item);
        return (this.m_SpriteList.Count - 1);
    }

    public int AddSpriteNode(NcSpriteNode addSpriteNode)
    {
        if (this.m_SpriteList == null)
        {
            this.m_SpriteList = new List<NcSpriteNode>();
        }
        this.m_SpriteList.Add(addSpriteNode.GetClone());
        this.m_bNeedRebuild = true;
        return (this.m_SpriteList.Count - 1);
    }

    private void Awake()
    {
        this.m_bbInstance = true;
    }

    public void ClearAllSpriteNode()
    {
        if (this.m_SpriteList != null)
        {
            this.m_bNeedRebuild = true;
            this.m_SpriteList.Clear();
        }
    }

    private void CreateEffectObject()
    {
        if (this.m_bbInstance && this.m_bShowEffect)
        {
            this.DestroyEffectObject();
            if (base.transform.parent != null)
            {
                base.transform.parent.SendMessage("OnSpriteListEffectFrame", this.m_SpriteList[this.m_nCurrentIndex], SendMessageOptions.DontRequireReceiver);
            }
            if (this.m_SpriteList[this.m_nCurrentIndex].m_bEffectInstantiate)
            {
                this.m_CurrentEffect = this.CreateSpriteEffect(this.m_nCurrentIndex, base.transform);
                if (base.transform.parent != null)
                {
                    base.transform.parent.SendMessage("OnSpriteListEffectInstance", this.m_CurrentEffect, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    public static void CreatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType, MESH_TYPE m_MeshType)
    {
        int[] numArray;
        Vector2 vector = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
        float num = (alignType != ALIGN_TYPE.BOTTOM) ? ((alignType != ALIGN_TYPE.TOP) ? 0f : (-1f * vector.y)) : (1f * vector.y);
        Rect frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
        if (bTrimCenterAlign)
        {
            frameUvOffset.center = Vector2.zero;
        }
        Vector3[] vectorArray = new Vector3[] { new Vector3(frameUvOffset.xMax * vector.x, (frameUvOffset.yMax * vector.y) + num), new Vector3(frameUvOffset.xMax * vector.x, (frameUvOffset.yMin * vector.y) + num), new Vector3(frameUvOffset.xMin * vector.x, (frameUvOffset.yMin * vector.y) + num), new Vector3(frameUvOffset.xMin * vector.x, (frameUvOffset.yMax * vector.y) + num) };
        Color[] colorArray = new Color[] { Color.white, Color.white, Color.white, Color.white };
        Vector3[] vectorArray2 = new Vector3[] { new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, -1f) };
        Vector4[] vectorArray3 = new Vector4[] { new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f), new Vector4(1f, 0f, 0f, -1f) };
        if (m_MeshType == MESH_TYPE.BuiltIn_Plane)
        {
            numArray = new int[] { 1, 2, 0, 0, 2, 3 };
        }
        else
        {
            numArray = new int[] { 1, 2, 0, 0, 2, 3, 1, 0, 3, 3, 2, 1 };
        }
        Vector2[] vectorArray4 = new Vector2[] { new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f) };
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vectorArray;
        meshFilter.mesh.colors = colorArray;
        meshFilter.mesh.normals = vectorArray2;
        meshFilter.mesh.tangents = vectorArray3;
        meshFilter.mesh.triangles = numArray;
        meshFilter.mesh.uv = vectorArray4;
        meshFilter.mesh.RecalculateBounds();
    }

    private void CreateSoundObject(NcSpriteNode ncSpriteNode)
    {
        if (this.m_bShowEffect && (ncSpriteNode.m_AudioClip != null))
        {
        }
    }

    public GameObject CreateSpriteEffect(int nSrcSpriteIndex, Transform parentTrans)
    {
        GameObject obj2 = null;
        if (this.m_SpriteList[nSrcSpriteIndex].m_EffectPrefab != null)
        {
            obj2 = base.CreateGameObject("Effect_" + this.m_SpriteList[nSrcSpriteIndex].m_EffectPrefab.name);
            if (obj2 == null)
            {
                return null;
            }
            base.ChangeParent(parentTrans, obj2.transform, true, null);
            Transform transform = obj2.transform;
            transform.localScale = (Vector3) (transform.localScale * this.m_SpriteList[nSrcSpriteIndex].m_fEffectScale);
            Transform transform2 = obj2.transform;
            transform2.localPosition += this.m_SpriteList[nSrcSpriteIndex].m_EffectPos;
            Transform transform3 = obj2.transform;
            transform3.localRotation *= Quaternion.Euler(this.m_SpriteList[nSrcSpriteIndex].m_EffectRot);
        }
        return obj2;
    }

    public void DeleteSpriteNode(int nIndex)
    {
        if (((this.m_SpriteList != null) && (nIndex >= 0)) && (this.m_SpriteList.Count > nIndex))
        {
            this.m_bNeedRebuild = true;
            this.m_SpriteList.Remove(this.m_SpriteList[nIndex]);
        }
    }

    private void DestroyEffectObject()
    {
        if (this.m_CurrentEffect != null)
        {
            UnityEngine.Object.Destroy(this.m_CurrentEffect);
        }
        this.m_CurrentEffect = null;
    }

    public int GetCurrentSpriteIndex()
    {
        return this.m_nCurrentIndex;
    }

    public NcSpriteNode GetCurrentSpriteNode()
    {
        if ((this.m_SpriteList != null) && (this.m_SpriteList.Count > this.m_nCurrentIndex))
        {
            return this.m_SpriteList[this.m_nCurrentIndex];
        }
        return null;
    }

    public NcSpriteNode GetSpriteNode(int nIndex)
    {
        if (((this.m_SpriteList != null) && (nIndex >= 0)) && (this.m_SpriteList.Count > nIndex))
        {
            return this.m_SpriteList[nIndex];
        }
        return null;
    }

    public NcSpriteNode GetSpriteNode(string spriteName)
    {
        if (this.m_SpriteList != null)
        {
            foreach (NcSpriteNode node in this.m_SpriteList)
            {
                if (node.m_SpriteName == spriteName)
                {
                    return node;
                }
            }
        }
        return null;
    }

    public int GetSpriteNodeCount()
    {
        if (this.m_SpriteList == null)
        {
            return 0;
        }
        return this.m_SpriteList.Count;
    }

    public int GetSpriteNodeIndex(string spriteName)
    {
        if (this.m_SpriteList != null)
        {
            for (int i = 0; i < this.m_SpriteList.Count; i++)
            {
                if (this.m_SpriteList[i].m_SpriteName == spriteName)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public Rect GetSpriteUvRect(int nStriteIndex, int nFrameIndex)
    {
        if (((this.m_SpriteList.Count > nStriteIndex) && (this.m_SpriteList[nStriteIndex].m_FrameInfos != null)) && (this.m_SpriteList[nStriteIndex].m_FrameInfos.Length > nFrameIndex))
        {
            return this.m_SpriteList[nStriteIndex].m_FrameInfos[nFrameIndex].m_TextureUvOffset;
        }
        return new Rect(0f, 0f, 0f, 0f);
    }

    public bool IsEndSprite()
    {
        if ((((this.m_SpriteList != null) && (this.m_nCurrentIndex >= 0)) && (this.m_SpriteList.Count > this.m_nCurrentIndex)) && (!this.IsUnused(this.m_nCurrentIndex) && !this.m_SpriteList[this.m_nCurrentIndex].IsEmptyTexture()))
        {
            return this.m_bEndSprite;
        }
        return true;
    }

    public bool IsUnused(int nNodeIndex)
    {
        return (this.m_SpriteList[nNodeIndex].IsUnused() || (nNodeIndex < this.m_nBuildStartIndex));
    }

    public bool IsValidFactory()
    {
        if (this.m_bNeedRebuild)
        {
            return false;
        }
        return true;
    }

    public void MoveSpriteNode(int nSrcIndex, int nTarIndex)
    {
        NcSpriteNode item = this.m_SpriteList[nSrcIndex];
        this.m_SpriteList.Remove(item);
        this.m_SpriteList.Insert(nTarIndex, item);
    }

    public void OnAnimationChangingFrame(NcSpriteAnimation spriteCom, int nOldIndex, int nNewIndex, int nLoopCount)
    {
        if (this.m_SpriteList.Count > this.m_nCurrentIndex)
        {
            if (((this.m_SpriteList[this.m_nCurrentIndex].m_EffectPrefab != null) && ((nOldIndex < this.m_SpriteList[this.m_nCurrentIndex].m_nEffectFrame) || (nNewIndex <= nOldIndex))) && ((this.m_SpriteList[this.m_nCurrentIndex].m_nEffectFrame <= nNewIndex) && ((nLoopCount == 0) || !this.m_SpriteList[this.m_nCurrentIndex].m_bEffectOnlyFirst)))
            {
                this.CreateEffectObject();
            }
            if (((this.m_SpriteList[this.m_nCurrentIndex].m_AudioClip != null) && ((nOldIndex < this.m_SpriteList[this.m_nCurrentIndex].m_nSoundFrame) || (nNewIndex <= nOldIndex))) && ((this.m_SpriteList[this.m_nCurrentIndex].m_nSoundFrame <= nNewIndex) && ((nLoopCount == 0) || !this.m_SpriteList[this.m_nCurrentIndex].m_bSoundOnlyFirst)))
            {
                this.CreateSoundObject(this.m_SpriteList[this.m_nCurrentIndex]);
            }
        }
    }

    public bool OnAnimationLastFrame(NcSpriteAnimation spriteCom, int nLoopCount)
    {
        if (this.m_SpriteList.Count > this.m_nCurrentIndex)
        {
            this.m_bEndSprite = true;
            if (this.m_bSequenceMode)
            {
                if (this.m_nCurrentIndex < (this.GetSpriteNodeCount() - 1))
                {
                    if ((!this.m_SpriteList[this.m_nCurrentIndex].m_bLoop ? 1 : 3) == nLoopCount)
                    {
                        this.SetSprite((int) (this.m_nCurrentIndex + 1));
                        return true;
                    }
                }
                else
                {
                    this.SetSprite(0);
                }
            }
            else
            {
                NcSpriteAnimation animation = this.SetSprite(this.m_SpriteList[this.m_nCurrentIndex].m_nNextSpriteIndex) as NcSpriteAnimation;
                if (animation != null)
                {
                    animation.ResetAnimation();
                    return true;
                }
            }
        }
        return false;
    }

    public void OnAnimationStartFrame(NcSpriteAnimation spriteCom)
    {
    }

    public void OnChangingSprite(int nOldNodeIndex, int nNewNodeIndex)
    {
        this.m_bEndSprite = false;
        this.DestroyEffectObject();
    }

    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
    }

    public NcEffectBehaviour SetSprite(int nNodeIndex)
    {
        return this.SetSprite(nNodeIndex, true);
    }

    public NcEffectBehaviour SetSprite(string spriteName)
    {
        if (this.m_SpriteList != null)
        {
            int nNodeIndex = 0;
            foreach (NcSpriteNode node in this.m_SpriteList)
            {
                if (node.m_SpriteName == spriteName)
                {
                    return this.SetSprite(nNodeIndex, true);
                }
                nNodeIndex++;
            }
        }
        return null;
    }

    public NcEffectBehaviour SetSprite(int nNodeIndex, bool bRunImmediate)
    {
        if (((this.m_SpriteList != null) && (nNodeIndex >= 0)) && (this.m_SpriteList.Count > nNodeIndex))
        {
            if (bRunImmediate)
            {
                this.OnChangingSprite(this.m_nCurrentIndex, nNodeIndex);
            }
            this.m_nCurrentIndex = nNodeIndex;
            NcSpriteAnimation component = base.GetComponent<NcSpriteAnimation>();
            if (component != null)
            {
                component.SetSpriteFactoryIndex(nNodeIndex, false);
                if (bRunImmediate)
                {
                    component.ResetAnimation();
                }
            }
            NcSpriteTexture texture = base.GetComponent<NcSpriteTexture>();
            if (texture != null)
            {
                texture.SetSpriteFactoryIndex(nNodeIndex, -1, false);
                if (bRunImmediate)
                {
                    this.CreateEffectObject();
                }
            }
            if (component != null)
            {
                return component;
            }
            if (component != null)
            {
                return texture;
            }
        }
        return null;
    }

    public NcSpriteNode SetSpriteNode(int nIndex, NcSpriteNode newInfo)
    {
        if (((this.m_SpriteList == null) || (nIndex < 0)) || (this.m_SpriteList.Count <= nIndex))
        {
            return null;
        }
        NcSpriteNode node = this.m_SpriteList[nIndex];
        this.m_SpriteList[nIndex] = newInfo;
        return node;
    }

    public static void UpdateMeshUVs(MeshFilter meshFilter, Rect uv)
    {
        meshFilter.mesh.uv = new Vector2[] { new Vector2(uv.x + uv.width, uv.y + uv.height), new Vector2(uv.x + uv.width, uv.y), new Vector2(uv.x, uv.y), new Vector2(uv.x, uv.y + uv.height) };
    }

    public static void UpdatePlane(MeshFilter meshFilter, float fUvScale, NcFrameInfo ncSpriteFrameInfo, bool bTrimCenterAlign, ALIGN_TYPE alignType)
    {
        Vector2 vector = new Vector2(fUvScale * ncSpriteFrameInfo.m_FrameScale.x, fUvScale * ncSpriteFrameInfo.m_FrameScale.y);
        float num = (alignType != ALIGN_TYPE.BOTTOM) ? ((alignType != ALIGN_TYPE.TOP) ? 0f : (-1f * vector.y)) : (1f * vector.y);
        Rect frameUvOffset = ncSpriteFrameInfo.m_FrameUvOffset;
        if (bTrimCenterAlign)
        {
            frameUvOffset.center = Vector2.zero;
        }
        meshFilter.mesh.vertices = new Vector3[] { new Vector3(frameUvOffset.xMax * vector.x, (frameUvOffset.yMax * vector.y) + num), new Vector3(frameUvOffset.xMax * vector.x, (frameUvOffset.yMin * vector.y) + num), new Vector3(frameUvOffset.xMin * vector.x, (frameUvOffset.yMin * vector.y) + num), new Vector3(frameUvOffset.xMin * vector.x, (frameUvOffset.yMax * vector.y) + num) };
        meshFilter.mesh.RecalculateBounds();
    }

    public enum ALIGN_TYPE
    {
        TOP,
        CENTER,
        BOTTOM
    }

    [SerializeField]
    public enum MESH_TYPE
    {
        BuiltIn_Plane,
        BuiltIn_TwosidePlane
    }

    [Serializable]
    public class NcFrameInfo
    {
        public bool m_bEmptyFrame;
        public Vector2 m_FrameScale;
        public Rect m_FrameUvOffset;
        public int m_nFrameIndex;
        public int m_nTexHeight;
        public int m_nTexWidth;
        public Vector2 m_scaleFactor;
        public Rect m_TextureUvOffset;
    }

    [Serializable, SerializeField]
    public class NcSpriteNode
    {
        public AudioClip m_AudioClip;
        public bool m_bEffectDetach = true;
        public bool m_bEffectInstantiate = true;
        public bool m_bEffectOnlyFirst = true;
        public bool m_bIncludedAtlas = true;
        public bool m_bLoop;
        public bool m_bSoundLoop;
        public bool m_bSoundOnlyFirst = true;
        public Vector3 m_EffectPos = Vector3.zero;
        public GameObject m_EffectPrefab;
        public Vector3 m_EffectRot = Vector3.zero;
        public float m_fEffectScale = 1f;
        public float m_fEffectSpeed = 1f;
        public float m_fFps = 20f;
        public float m_fMaxTextureAlpha = 1f;
        public NcSpriteFactory.NcFrameInfo[] m_FrameInfos;
        public float m_fSoundPitch = 1f;
        public float m_fSoundVolume = 1f;
        public float m_fTestSpeed = 1f;
        public float m_fTime;
        public int m_nEffectFrame;
        public int m_nFrameCount = 1;
        public int m_nLoopFrameCount;
        public int m_nLoopingCount;
        public int m_nLoopStartFrame;
        public int m_nNextSpriteIndex = -1;
        public int m_nSoundFrame;
        public int m_nStartFrame;
        public int m_nTestMode;
        public int m_nTilingX = 1;
        public int m_nTilingY = 1;
        public string m_SpriteName = string.Empty;
        public string m_TextureGUID = string.Empty;
        public string m_TextureName = string.Empty;

        public NcSpriteFactory.NcSpriteNode GetClone()
        {
            return null;
        }

        public int GetStartFrame()
        {
            if ((this.m_FrameInfos != null) && (this.m_FrameInfos.Length != 0))
            {
                return this.m_FrameInfos[0].m_nFrameIndex;
            }
            return 0;
        }

        public bool IsEmptyTexture()
        {
            return (this.m_TextureGUID == string.Empty);
        }

        public bool IsUnused()
        {
            return !this.m_bIncludedAtlas;
        }

        public void SetEmpty()
        {
            this.m_FrameInfos = null;
            this.m_TextureGUID = string.Empty;
        }
    }

    public enum SHOW_TYPE
    {
        NONE,
        ALL,
        SPRITE,
        ANIMATION,
        EFFECT
    }

    public enum SPRITE_TYPE
    {
        NcSpriteTexture,
        NcSpriteAnimation
    }
}

