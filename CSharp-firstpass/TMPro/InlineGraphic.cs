namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class InlineGraphic : MaskableGraphic
    {
        private InlineGraphicManager m_manager;
        public Texture texture;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.m_manager = base.GetComponentInParent<InlineGraphicManager>();
            if ((this.m_manager != null) && (this.m_manager.spriteAsset != null))
            {
                this.texture = this.m_manager.spriteAsset.spriteSheet;
            }
        }

        protected override void OnFillVBO(List<UIVertex> vbo)
        {
            base.OnFillVBO(vbo);
        }

        protected override void UpdateGeometry()
        {
        }

        public void UpdateMaterial()
        {
            base.UpdateMaterial();
        }

        public override Texture mainTexture
        {
            get
            {
                if (this.texture == null)
                {
                    return Graphic.s_WhiteTexture;
                }
                return this.texture;
            }
        }
    }
}

