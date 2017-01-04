using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MMGame_Math
{
    public static float Dot3(this Vector3 a, Vector3 b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static float Dot3(this Vector3 a, Vector4 b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static float Dot3(this Vector3 a, ref Vector3 b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static float Dot3(this Vector3 a, ref Vector4 b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static float DotXZ(this Vector3 a, Vector3 b)
    {
        return ((a.x * b.x) + (a.z * b.z));
    }

    public static float DotXZ(this Vector3 a, ref Vector3 b)
    {
        return ((a.x * b.x) + (a.z * b.z));
    }

    public static MeshRenderer GetMeshRendererInChildren(this GameObject go)
    {
        MeshRenderer component = go.GetComponent<Renderer>() as MeshRenderer;
        if (component != null)
        {
            return component;
        }
        int childCount = go.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            if ((child != null) && (child.gameObject != null))
            {
                component = child.gameObject.GetMeshRendererInChildren();
                if (component != null)
                {
                    return component;
                }
            }
        }
        return null;
    }

    public static Renderer GetRendererInChildren(this GameObject go)
    {
        if (go.GetComponent<Renderer>() != null)
        {
            return go.GetComponent<Renderer>();
        }
        int childCount = go.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            if ((child != null) && (child.gameObject != null))
            {
                Renderer rendererInChildren = child.gameObject.GetRendererInChildren();
                if (rendererInChildren != null)
                {
                    return rendererInChildren;
                }
            }
        }
        return null;
    }

    public static SkinnedMeshRenderer GetSkinnedMeshRendererInChildren(this GameObject go)
    {
        SkinnedMeshRenderer component = go.GetComponent<Renderer>() as SkinnedMeshRenderer;
        if (component != null)
        {
            return component;
        }
        int childCount = go.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            if ((child != null) && (child.gameObject != null))
            {
                component = child.gameObject.GetSkinnedMeshRendererInChildren();
                if (component != null)
                {
                    return component;
                }
            }
        }
        return null;
    }

    public static bool isMirror(Matrix4x4 m)
    {
        Vector3 lhs = m.GetColumn(0).toVec3();
        Vector3 rhs = m.GetColumn(1).toVec3();
        Vector3 a = m.GetColumn(2).toVec3();
        Vector3 b = Vector3.Cross(lhs, rhs);
        a.Normalize();
        b.Normalize();
        return (a.Dot3(ref b) < 0f);
    }

    public static Vector2 Lerp(this Vector2 left, Vector2 right, float lerp)
    {
        return new Vector2(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp));
    }

    public static Vector3 Lerp(this Vector3 left, Vector3 right, float lerp)
    {
        return new Vector3(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp));
    }

    public static Vector4 Lerp(this Vector4 left, Vector4 right, float lerp)
    {
        return new Vector4(Mathf.Lerp(left.x, right.x, lerp), Mathf.Lerp(left.y, right.y, lerp), Mathf.Lerp(left.z, right.z, lerp), Mathf.Lerp(left.w, right.w, lerp));
    }

    public static Vector3 Mul(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Mul(this Vector3 a, ref Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Mul(this Vector3 a, Vector3 b, float f)
    {
        return new Vector3((a.x * b.x) * f, (a.y * b.y) * f, (a.z * b.z) * f);
    }

    public static Vector3 Mul(this Vector3 a, ref Vector3 b, float f)
    {
        return new Vector3((a.x * b.x) * f, (a.y * b.y) * f, (a.z * b.z) * f);
    }

    public static void SetLayer(this GameObject go, int layer, bool bFileSkillIndicator)
    {
        SetLayerRecursively(go, layer, bFileSkillIndicator);
    }

    public static void SetLayer(this GameObject go, string layerName, bool bFileSkillIndicator = false)
    {
        int layer = LayerMask.NameToLayer(layerName);
        SetLayerRecursively(go, layer, bFileSkillIndicator);
    }

    public static void SetLayer(this GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
    {
        SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
    }

    public static void SetLayer(this GameObject go, string layerName, string layerNameParticles, bool bFileSkillIndicator = false)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int layerParticles = LayerMask.NameToLayer(layerNameParticles);
        SetLayerRecursively(go, layer, layerParticles, bFileSkillIndicator);
    }

    public static void SetLayerRecursively(GameObject go, int layer, bool bFileSkillIndicator)
    {
        if (!bFileSkillIndicator || !go.CompareTag("SCI"))
        {
            go.layer = layer;
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }
    }

    public static void SetLayerRecursively(GameObject go, int layer, int layerParticles, bool bFileSkillIndicator)
    {
        if (!bFileSkillIndicator || !go.CompareTag("SCI"))
        {
            if (go.GetComponent<ParticleSystem>() != null)
            {
                go.layer = layerParticles;
            }
            else
            {
                go.layer = layer;
            }
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer, bFileSkillIndicator);
            }
        }
    }

    public static void SetOffsetX(this Camera camera, float offsetX)
    {
        float num = (2f * Mathf.Tan((0.01745329f * camera.fieldOfView) * 0.5f)) * camera.nearClipPlane;
        float num2 = num * camera.aspect;
        float num3 = -Mathf.Clamp(offsetX, -1f, 1f) * num2;
        float right = (num2 + num3) * 0.5f;
        float left = right - num2;
        camera.SetPerspectiveOffCenter(left, right, -num * 0.5f, num * 0.5f, camera.nearClipPlane, camera.farClipPlane);
    }

    public static void SetPerspectiveOffCenter(this Camera camera, float left, float right, float bottom, float top, float near, float far)
    {
        float num = 1f / (right - left);
        float num2 = 1f / (top - bottom);
        float num3 = 1f / (near - far);
        Matrix4x4 matrixx = new Matrix4x4 {
            m00 = (2f * near) * num,
            m10 = 0f,
            m20 = 0f,
            m30 = 0f,
            m01 = 0f,
            m11 = (2f * near) * num2,
            m21 = 0f,
            m31 = 0f,
            m02 = (right + left) * num,
            m12 = (top + bottom) * num2,
            m22 = far * num3,
            m32 = -1f,
            m03 = 0f,
            m13 = 0f,
            m23 = ((2f * far) * near) * num3,
            m33 = 0f
        };
        camera.projectionMatrix = matrixx;
    }

    public static string ToString2(this Vector3 a)
    {
        return string.Format("({0},{1},{2})", a.x, a.y, a.z);
    }

    public static Vector3 toVec3(this Vector4 a)
    {
        return new Vector3(a.x, a.y, a.z);
    }

    public static Vector4 toVec4(this Vector3 v, float a)
    {
        return new Vector4(v.x, v.y, v.z, a);
    }

    public static Vector2 xz(this Vector3 a)
    {
        return new Vector2(a.x, a.z);
    }

    public static float XZSqrMagnitude(this Vector3 a, Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.z - b.z;
        return ((num * num) + (num2 * num2));
    }

    public static float XZSqrMagnitude(this Vector3 a, ref Vector3 b)
    {
        float num = a.x - b.x;
        float num2 = a.z - b.z;
        return ((num * num) + (num2 * num2));
    }
}

