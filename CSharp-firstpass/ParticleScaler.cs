using System;
using UnityEngine;

[AddComponentMenu("SGame/Effect/ParticleScaler"), ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour, IPooledMonoBehaviour
{
    public bool alsoScaleGameobject = true;
    private bool m_gotten;
    public float particleScale = 1f;
    private float prevScale = 1f;
    [HideInInspector]
    public bool scriptGenerated;

    public void CheckAndApplyScale()
    {
        if ((this.prevScale != this.particleScale) && (this.particleScale > 0f))
        {
            if (this.alsoScaleGameobject)
            {
                base.transform.localScale = new Vector3(this.particleScale, this.particleScale, this.particleScale);
            }
            float scaleFactor = this.particleScale / this.prevScale;
            this.ScaleLegacySystems(scaleFactor);
            this.ScaleShurikenSystems(scaleFactor);
            this.ScaleTrailRenderers(scaleFactor);
            this.prevScale = this.particleScale;
        }
    }

    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (!this.m_gotten)
        {
            this.m_gotten = true;
            this.prevScale = this.particleScale;
            if (this.scriptGenerated && (this.particleScale != 1f))
            {
                this.prevScale = 1f;
                this.CheckAndApplyScale();
            }
        }
    }

    public void OnRecycle()
    {
        this.m_gotten = false;
    }

    private void ScaleLegacySystems(float scaleFactor)
    {
        ParticleEmitter[] componentsInChildren = base.GetComponentsInChildren<ParticleEmitter>(true);
        ParticleAnimator[] animatorArray = base.GetComponentsInChildren<ParticleAnimator>(true);
        foreach (ParticleEmitter emitter in componentsInChildren)
        {
            emitter.minSize *= scaleFactor;
            emitter.maxSize *= scaleFactor;
            emitter.worldVelocity = (Vector3) (emitter.worldVelocity * scaleFactor);
            emitter.localVelocity = (Vector3) (emitter.localVelocity * scaleFactor);
            emitter.rndVelocity = (Vector3) (emitter.rndVelocity * scaleFactor);
        }
        foreach (ParticleAnimator animator in animatorArray)
        {
            animator.force = (Vector3) (animator.force * scaleFactor);
            animator.rndForce = (Vector3) (animator.rndForce * scaleFactor);
        }
    }

    private void ScaleShurikenSystems(float scaleFactor)
    {
        foreach (ParticleSystem system in base.GetComponentsInChildren<ParticleSystem>(true))
        {
            system.startSpeed *= scaleFactor;
            system.startSize *= scaleFactor;
            system.gravityModifier *= scaleFactor;
        }
    }

    private void ScaleTrailRenderers(float scaleFactor)
    {
        foreach (TrailRenderer renderer in base.GetComponentsInChildren<TrailRenderer>(true))
        {
            renderer.startWidth *= scaleFactor;
            renderer.endWidth *= scaleFactor;
        }
    }

    private void Start()
    {
        this.OnGet();
    }

    private void Update()
    {
    }
}

