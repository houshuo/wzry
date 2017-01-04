namespace TMPro
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpriteAsset : ScriptableObject
    {
        private List<Sprite> m_sprites;
        public Material material;
        public List<SpriteInfo> spriteInfoList;
        public Texture spriteSheet;

        public void AddSprites(string path)
        {
        }

        private void OnEnable()
        {
        }

        private void OnValidate()
        {
            TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, this);
        }
    }
}

