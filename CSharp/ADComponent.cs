using System;
using UnityEngine;
using UnityEngine.UI;

public class ADComponent : MonoBehaviour
{
    public Texture[] m_adTextures;
    public float m_fadeTime = 1f;
    private int m_index;
    private Material m_material;
    public float m_stableTime = 5f;
    private eADState m_state = eADState.eStable;
    public Image m_targetImage;
    private float m_timer;

    public void Awake()
    {
        if (this.m_adTextures.Length != 0)
        {
            this.m_targetImage.gameObject.SetActive(true);
            Shader shader = Shader.Find("UI/ImageSwitch");
            this.m_material = new Material(shader);
            this.m_targetImage.material = this.m_material;
            this.m_material.SetTexture("_Tex1", this.m_adTextures[0]);
            this.m_material.SetFloat("_Percent", 0f);
        }
    }

    private void Update()
    {
        if (this.m_adTextures.Length != 0)
        {
            this.m_timer += Time.deltaTime;
            if (this.m_state == eADState.eStable)
            {
                if (this.m_timer >= this.m_stableTime)
                {
                    this.m_timer = 0f;
                    this.m_state = eADState.eFade;
                    int index = this.m_index;
                    this.m_index = (this.m_index + 1) % this.m_adTextures.Length;
                    this.m_material.SetTexture("_Tex2", this.m_adTextures[index]);
                    this.m_material.SetTexture("_Tex1", this.m_adTextures[this.m_index]);
                    this.m_material.SetFloat("_Percent", 1f);
                }
            }
            else if (this.m_timer >= this.m_fadeTime)
            {
                this.m_timer = 0f;
                this.m_state = eADState.eStable;
                this.m_material.SetFloat("_Percent", 0f);
            }
            else
            {
                this.m_material.SetFloat("_Percent", 1f - (this.m_timer / this.m_fadeTime));
            }
        }
    }

    private enum eADState
    {
        eFade,
        eStable
    }
}

