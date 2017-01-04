using System;
using UnityEngine;

public class AtlasInfo : ScriptableObject
{
    [NonSerialized]
    private Material m_material;
    public Texture2D texture;
    public UVDetail[] uvDetails;

    public UVDetail GetUV(string atlasName)
    {
        if (!string.IsNullOrEmpty(atlasName))
        {
            for (int i = 0; i < this.uvDetails.Length; i++)
            {
                if (this.uvDetails[i].Name == atlasName)
                {
                    return this.uvDetails[i];
                }
            }
            Debug.LogError("no atlas \"" + atlasName + "\" was found in texture:" + this.texture.name);
        }
        return null;
    }

    public Material material
    {
        get
        {
            if (null == this.m_material)
            {
                Shader content = Singleton<CResourceManager>.GetInstance().GetResource("Shaders/UI/UI3D.shader", typeof(Shader), enResourceType.BattleScene, true, true).m_content as Shader;
                this.m_material = new Material(content);
                this.m_material.SetTexture("_MainTex", this.texture);
            }
            return this.m_material;
        }
    }

    [Serializable]
    public class UVDetail
    {
        public int height;
        public string Name;
        public bool rotate;
        public Vector2 uvBL;
        public Vector2 uvBR;
        public Vector2 uvTL;
        public Vector2 uvTR;
        public int width;
    }
}

