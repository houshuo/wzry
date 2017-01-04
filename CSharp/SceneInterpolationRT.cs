using System;
using UnityEngine;

internal class SceneInterpolationRT : MonoBehaviour
{
    [NonSerialized]
    public float factor;
    private Material material;
    [NonSerialized]
    public RenderTexture rt0;
    [NonSerialized]
    public RenderTexture rt1;

    private void Awake()
    {
        string name = "SGame_Post/SceneInterpolation";
        Shader shader = Shader.Find(name);
        if (shader == null)
        {
            string str2 = "Load shader failed, filename= " + name;
        }
        else
        {
            this.material = new Material(shader);
            this.material.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy;
        }
    }

    private void OnPostRender()
    {
        if (this.material != null)
        {
            Camera component = base.gameObject.GetComponent<Camera>();
            if (component != null)
            {
                this.material.SetTexture("_Tex1", this.rt0);
                this.material.SetTexture("_Tex2", this.rt1);
                this.material.SetFloat("_Factor", this.factor);
                GL.PushMatrix();
                this.material.SetPass(0);
                GL.LoadOrtho();
                GL.Viewport(component.pixelRect);
                GL.Begin(7);
                GL.TexCoord2(0f, 0f);
                GL.Vertex3(0f, 0f, 0f);
                GL.TexCoord2(0f, 1f);
                GL.Vertex3(0f, 1f, 0f);
                GL.TexCoord2(1f, 1f);
                GL.Vertex3(1f, 1f, 0f);
                GL.TexCoord2(1f, 0f);
                GL.Vertex3(1f, 0f, 0f);
                GL.End();
                GL.PopMatrix();
                this.material.SetTexture("_Tex1", null);
                this.material.SetTexture("_Tex2", null);
            }
        }
    }
}

