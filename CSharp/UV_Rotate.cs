using System;
using UnityEngine;

public class UV_Rotate : MonoBehaviour
{
    public int rotateSpeed = 30;
    public Vector2 rotationCenter = Vector2.zero;
    public Texture texture;

    private void Start()
    {
        Material material = new Material(Shader.Find("Rotating Texture")) {
            mainTexture = this.texture
        };
        base.GetComponent<Renderer>().material = material;
    }

    private void Update()
    {
        Quaternion q = Quaternion.Euler(0f, 0f, Time.time * this.rotateSpeed);
        Matrix4x4 matrixx = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
        Matrix4x4 matrixx2 = Matrix4x4.TRS((Vector3) -this.rotationCenter, Quaternion.identity, new Vector3(1f, 1f, 1f));
        Matrix4x4 matrixx3 = Matrix4x4.TRS((Vector3) this.rotationCenter, Quaternion.identity, new Vector3(1f, 1f, 1f));
        base.GetComponent<Renderer>().material.SetMatrix("_Rotation", (matrixx3 * matrixx) * matrixx2);
    }
}

