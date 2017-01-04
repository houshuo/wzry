using System;
using UnityEngine;

public class NcUvAnimation : NcEffectAniBehaviour, IPooledMonoBehaviour
{
    public bool m_bAutoDestruct;
    public bool m_bFixedTileSize;
    public bool m_bRepeat = true;
    public bool m_bUseSmoothDeltaTime;
    protected Vector2 m_EndOffset = new Vector2();
    public float m_fOffsetX;
    public float m_fOffsetY;
    public float m_fScrollSpeedX = 1f;
    public float m_fScrollSpeedY;
    public float m_fTilingX = 1f;
    public float m_fTilingY = 1f;
    private bool m_gotten;
    private bool m_inited;
    protected Vector3 m_OriginalScale = new Vector3();
    protected Vector2 m_OriginalTiling = new Vector2();
    protected Renderer m_Renderer;
    protected Vector2 m_RepeatOffset = new Vector2();

    public override int GetAnimationState()
    {
        if (!this.m_bRepeat)
        {
            int num;
            if ((base.enabled && NcEffectBehaviour.IsActive(base.gameObject)) && !base.IsEndAnimation())
            {
                num = 1;
            }
            num = 0;
        }
        return -1;
    }

    public void OnCreate()
    {
        if (!this.m_inited)
        {
            this.m_inited = true;
            this.m_Renderer = base.GetRenderer();
            if (((this.m_Renderer == null) || (this.m_Renderer.sharedMaterial == null)) || (this.m_Renderer.sharedMaterial.mainTexture == null))
            {
                base.enabled = false;
            }
            else
            {
                base.GetRenderer().material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
                base.AddRuntimeMaterial(base.GetRenderer().material);
            }
        }
    }

    public void OnGet()
    {
        if (!this.m_gotten)
        {
            this.m_gotten = true;
            float num = this.m_fOffsetX + this.m_fTilingX;
            this.m_RepeatOffset.x = num - ((int) num);
            if (this.m_RepeatOffset.x < 0f)
            {
                this.m_RepeatOffset.x++;
            }
            num = this.m_fOffsetY + this.m_fTilingY;
            this.m_RepeatOffset.y = num - ((int) num);
            if (this.m_RepeatOffset.y < 0f)
            {
                this.m_RepeatOffset.y++;
            }
            this.m_EndOffset.x = 1f - ((this.m_fTilingX - ((int) this.m_fTilingX)) + (((this.m_fTilingX - ((int) this.m_fTilingX)) >= 0f) ? ((float) 0) : ((float) 1)));
            this.m_EndOffset.y = 1f - ((this.m_fTilingY - ((int) this.m_fTilingY)) + (((this.m_fTilingY - ((int) this.m_fTilingY)) >= 0f) ? ((float) 0) : ((float) 1)));
            base.InitAnimationTimer();
        }
    }

    public void OnRecycle()
    {
        this.m_gotten = false;
    }

    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
        this.m_fScrollSpeedX *= fSpeedRate;
        this.m_fScrollSpeedY *= fSpeedRate;
    }

    public override void OnUpdateToolData()
    {
        this.m_OriginalScale = base.transform.lossyScale;
        this.m_OriginalTiling.x = this.m_fTilingX;
        this.m_OriginalTiling.y = this.m_fTilingY;
    }

    public override void ResetAnimation()
    {
        if (!base.enabled)
        {
            base.enabled = true;
        }
        this.Start();
    }

    public void SetFixedTileSize(bool bFixedTileSize)
    {
        this.m_bFixedTileSize = bFixedTileSize;
    }

    private void Start()
    {
        this.OnCreate();
        this.OnGet();
    }

    private void Update()
    {
        if (((this.m_Renderer != null) && (this.m_Renderer.sharedMaterial != null)) && (this.m_Renderer.sharedMaterial.mainTexture != null))
        {
            if (this.m_bFixedTileSize)
            {
                if ((this.m_fScrollSpeedX != 0f) && (this.m_OriginalScale.x != 0f))
                {
                    this.m_fTilingX = this.m_OriginalTiling.x * (base.transform.lossyScale.x / this.m_OriginalScale.x);
                }
                if ((this.m_fScrollSpeedY != 0f) && (this.m_OriginalScale.y != 0f))
                {
                    this.m_fTilingY = this.m_OriginalTiling.y * (base.transform.lossyScale.y / this.m_OriginalScale.y);
                }
                base.GetRenderer().material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
            }
            if (this.m_bUseSmoothDeltaTime)
            {
                this.m_fOffsetX += base.m_Timer.GetSmoothDeltaTime() * this.m_fScrollSpeedX;
                this.m_fOffsetY += base.m_Timer.GetSmoothDeltaTime() * this.m_fScrollSpeedY;
            }
            else
            {
                this.m_fOffsetX += base.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX;
                this.m_fOffsetY += base.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY;
            }
            bool flag = false;
            if (!this.m_bRepeat)
            {
                this.m_RepeatOffset.x += base.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX;
                if ((this.m_RepeatOffset.x < 0f) || (1f < this.m_RepeatOffset.x))
                {
                    this.m_fOffsetX = this.m_EndOffset.x;
                    base.enabled = false;
                    flag = true;
                }
                this.m_RepeatOffset.y += base.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY;
                if ((this.m_RepeatOffset.y < 0f) || (1f < this.m_RepeatOffset.y))
                {
                    this.m_fOffsetY = this.m_EndOffset.y;
                    base.enabled = false;
                    flag = true;
                }
            }
            this.m_Renderer.material.mainTextureOffset = new Vector2(this.m_fOffsetX, this.m_fOffsetY);
            if (flag)
            {
                base.OnEndAnimation();
                if (this.m_bAutoDestruct)
                {
                    UnityEngine.Object.DestroyObject(base.gameObject);
                }
            }
        }
    }
}

