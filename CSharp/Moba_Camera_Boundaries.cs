using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Moba_Camera_Boundaries
{
    public static string boundaryLayer = "mobaCameraBoundaryLayer";
    private static bool boundaryLayerExists = true;
    private static ListView<Moba_Camera_Boundary> cube_boundaries = new ListView<Moba_Camera_Boundary>();
    private static ListView<Moba_Camera_Boundary> sphere_boundaries = new ListView<Moba_Camera_Boundary>();

    public static bool AddBoundary(Moba_Camera_Boundary boundary, BoundaryType type)
    {
        if (boundary != null)
        {
            if (type == BoundaryType.cube)
            {
                cube_boundaries.Add(boundary);
                return true;
            }
            if (type == BoundaryType.sphere)
            {
                sphere_boundaries.Add(boundary);
                return true;
            }
        }
        return false;
    }

    private static Vector3 calBoxRelations(BoxCollider box, Vector3 point, bool containedToBox, out bool isPointInBox)
    {
        Vector3 vector = box.transform.position + box.center;
        float num = (box.size.x / 2f) * box.transform.localScale.x;
        float num2 = (box.size.y / 2f) * box.transform.localScale.y;
        float num3 = (box.size.z / 2f) * box.transform.localScale.z;
        float num4 = Vector3.Dot(point - vector, box.transform.up);
        Vector3 vector2 = point + ((Vector3) (num4 * -box.transform.up));
        float num5 = Vector3.Dot(vector2 - vector, box.transform.right);
        Vector3 vector3 = vector2 + ((Vector3) (num5 * -box.transform.right));
        Vector3 rhs = vector3 - vector;
        Vector3 vector5 = point - vector2;
        Vector3 vector6 = vector2 - vector3;
        float magnitude = rhs.magnitude;
        float y = vector5.magnitude;
        float num8 = vector6.magnitude;
        isPointInBox = true;
        if (magnitude > num3)
        {
            if (containedToBox)
            {
                magnitude = num3;
            }
            isPointInBox = false;
        }
        if (y > num2)
        {
            if (containedToBox)
            {
                y = num2;
            }
            isPointInBox = false;
        }
        if (num8 > num)
        {
            if (containedToBox)
            {
                num8 = num;
            }
            isPointInBox = false;
        }
        magnitude *= (Vector3.Dot(box.transform.forward, rhs) < 0f) ? -1f : 1f;
        y *= (Vector3.Dot(box.transform.up, vector5) < 0f) ? -1f : 1f;
        return new Vector3(num8 * ((Vector3.Dot(box.transform.right, vector6) < 0f) ? -1f : 1f), y, magnitude);
    }

    public static Moba_Camera_Boundary GetClosestBoundary(Vector3 point)
    {
        Moba_Camera_Boundary boundary = null;
        float num = 999999f;
        ListView<Moba_Camera_Boundary>.Enumerator enumerator = cube_boundaries.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Moba_Camera_Boundary current = enumerator.Current;
            if ((current != null) && current.isActive)
            {
                Vector3 vector = getClosestPointOnSurfaceBox(current.GetComponent<BoxCollider>(), point);
                Vector3 vector5 = point - vector;
                float magnitude = vector5.magnitude;
                if (magnitude < num)
                {
                    boundary = current;
                    num = magnitude;
                }
            }
        }
        ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = sphere_boundaries.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Moba_Camera_Boundary boundary3 = enumerator2.Current;
            if (boundary3.isActive)
            {
                SphereCollider component = boundary3.GetComponent<SphereCollider>();
                Vector3 vector2 = boundary3.transform.position + component.center;
                float radius = component.radius;
                Vector3 vector3 = point - vector2;
                Vector3 vector4 = vector2 + ((Vector3) (vector3.normalized * radius));
                Vector3 vector6 = point - vector4;
                float num4 = vector6.magnitude;
                if (num4 < num)
                {
                    boundary = boundary3;
                    num = num4;
                }
            }
        }
        return boundary;
    }

    public static Vector3 GetClosestPointOnBoundary(Moba_Camera_Boundary boundary, Vector3 point)
    {
        Vector3 vector = point;
        if (boundary.type == BoundaryType.cube)
        {
            return getClosestPointOnSurfaceBox(boundary.GetComponent<BoxCollider>(), point);
        }
        if (boundary.type == BoundaryType.sphere)
        {
            SphereCollider component = boundary.GetComponent<SphereCollider>();
            Vector3 vector2 = boundary.transform.position + component.center;
            float radius = component.radius;
            Vector3 vector3 = point - vector2;
            vector = vector2 + ((Vector3) (vector3.normalized * radius));
        }
        return vector;
    }

    private static Vector3 getClosestPointOnSurfaceBox(BoxCollider box, Vector3 point)
    {
        bool flag;
        Vector3 vector = calBoxRelations(box, point, true, out flag);
        return (Vector3) (((box.transform.position + (box.transform.forward * vector.z)) + (box.transform.right * vector.x)) + (box.transform.up * vector.y));
    }

    public static int GetNumberOfBoundaries()
    {
        return (cube_boundaries.Count + sphere_boundaries.Count);
    }

    public static bool isPointInBoundary(Vector3 point)
    {
        bool flag = false;
        ListView<Moba_Camera_Boundary>.Enumerator enumerator = cube_boundaries.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Moba_Camera_Boundary current = enumerator.Current;
            if (current.isActive)
            {
                BoxCollider component = current.GetComponent<BoxCollider>();
                if (component != null)
                {
                    bool flag2;
                    calBoxRelations(component, point, false, out flag2);
                    if (flag2)
                    {
                        flag = true;
                    }
                }
            }
        }
        ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = sphere_boundaries.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Moba_Camera_Boundary boundary2 = enumerator2.Current;
            if (boundary2.isActive)
            {
                SphereCollider collider2 = boundary2.GetComponent<SphereCollider>();
                if (collider2 != null)
                {
                    Vector3 vector = (boundary2.transform.position + collider2.center) - point;
                    if (vector.magnitude < collider2.radius)
                    {
                        flag = true;
                    }
                }
            }
        }
        return flag;
    }

    public static bool RemoveBoundary(Moba_Camera_Boundary boundary, BoundaryType type)
    {
        if (type == BoundaryType.cube)
        {
            return cube_boundaries.Remove(boundary);
        }
        return ((type == BoundaryType.sphere) && cube_boundaries.Remove(boundary));
    }

    public static void SetBoundaryLayerExist(bool value)
    {
        if (boundaryLayerExists)
        {
            boundaryLayerExists = false;
        }
    }

    public enum BoundaryType
    {
        cube,
        sphere,
        none
    }
}

