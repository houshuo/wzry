namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Material")]
    public class MaterialFadeDuration : DurationEvent
    {
        public bool FadeIn;
        private ListView<Material> materials;
        [ObjectTemplate(new System.Type[] {  })]
        public int triggerId;

        public override BaseEvent Clone()
        {
            MaterialFadeDuration duration = ClassObjPool<MaterialFadeDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            MaterialFadeDuration duration = src as MaterialFadeDuration;
            this.triggerId = duration.triggerId;
            this.FadeIn = duration.FadeIn;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.triggerId);
            this.materials = FadeMaterialUtility.GetFadeMaterials(gameObject);
            if (this.materials != null)
            {
                float num = !this.FadeIn ? 1f : 0f;
                for (int i = 0; i < this.materials.Count; i++)
                {
                    this.materials[i].SetFloat("_FadeFactor", num);
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.materials != null)
            {
                float num = !this.FadeIn ? 0f : 1f;
                num = Mathf.Clamp01(num);
                for (int i = 0; i < this.materials.Count; i++)
                {
                    Material material = this.materials[i];
                    material.SetFloat("_FadeFactor", num);
                    if (this.FadeIn)
                    {
                        Shader fadeShader = FadeMaterialUtility.GetFadeShader(material.shader, false);
                        if ((fadeShader != null) && (fadeShader != material.shader))
                        {
                            material.shader = fadeShader;
                        }
                    }
                }
                this.materials = null;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.materials = null;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if ((base.length != 0) && (this.materials != null))
            {
                float num = ActionUtility.MsToSec(_localTime) / base.lengthSec;
                if (!this.FadeIn)
                {
                    num = 1f - num;
                }
                num = Mathf.Clamp01(num);
                for (int i = 0; i < this.materials.Count; i++)
                {
                    this.materials[i].SetFloat("_FadeFactor", num);
                }
            }
        }
    }
}

