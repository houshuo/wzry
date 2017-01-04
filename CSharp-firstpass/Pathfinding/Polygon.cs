namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Polygon
    {
        public static List<Vector3> hullCache = new List<Vector3>();

        public static Vector3 ClosestPointOnTriangle(Vector3[] triangle, Vector3 point)
        {
            return ClosestPointOnTriangle(triangle[0], triangle[1], triangle[2], point);
        }

        public static Vector3 ClosestPointOnTriangle(Vector3 tr0, Vector3 tr1, Vector3 tr2, Vector3 point)
        {
            Vector3 lhs = tr0 - point;
            Vector3 vector2 = tr1 - tr0;
            Vector3 rhs = tr2 - tr0;
            float sqrMagnitude = vector2.sqrMagnitude;
            float num2 = Vector3.Dot(vector2, rhs);
            float num3 = rhs.sqrMagnitude;
            float num4 = Vector3.Dot(lhs, vector2);
            float num5 = Vector3.Dot(lhs, rhs);
            float num6 = (sqrMagnitude * num3) - (num2 * num2);
            float num7 = (num2 * num5) - (num3 * num4);
            float num8 = (num2 * num4) - (sqrMagnitude * num5);
            if ((num7 + num8) <= num6)
            {
                if (num7 < 0f)
                {
                    if (num8 < 0f)
                    {
                        if (num4 < 0f)
                        {
                            num8 = 0f;
                            if (-num4 >= sqrMagnitude)
                            {
                                num7 = 1f;
                            }
                            else
                            {
                                num7 = -num4 / sqrMagnitude;
                            }
                        }
                        else
                        {
                            num7 = 0f;
                            if (num5 >= 0f)
                            {
                                num8 = 0f;
                            }
                            else if (-num5 >= num3)
                            {
                                num8 = 1f;
                            }
                            else
                            {
                                num8 = -num5 / num3;
                            }
                        }
                    }
                    else
                    {
                        num7 = 0f;
                        if (num5 >= 0f)
                        {
                            num8 = 0f;
                        }
                        else if (-num5 >= num3)
                        {
                            num8 = 1f;
                        }
                        else
                        {
                            num8 = -num5 / num3;
                        }
                    }
                }
                else if (num8 < 0f)
                {
                    num8 = 0f;
                    if (num4 >= 0f)
                    {
                        num7 = 0f;
                    }
                    else if (-num4 >= sqrMagnitude)
                    {
                        num7 = 1f;
                    }
                    else
                    {
                        num7 = -num4 / sqrMagnitude;
                    }
                }
                else
                {
                    float num9 = 1f / num6;
                    num7 *= num9;
                    num8 *= num9;
                }
            }
            else
            {
                float num10;
                float num11;
                float num12;
                float num13;
                if (num7 < 0f)
                {
                    num10 = num2 + num4;
                    num11 = num3 + num5;
                    if (num11 > num10)
                    {
                        num12 = num11 - num10;
                        num13 = (sqrMagnitude - (2f * num2)) + num3;
                        if (num12 >= num13)
                        {
                            num7 = 1f;
                            num8 = 0f;
                        }
                        else
                        {
                            num7 = num12 / num13;
                            num8 = 1f - num7;
                        }
                    }
                    else
                    {
                        num7 = 0f;
                        if (num11 <= 0f)
                        {
                            num8 = 1f;
                        }
                        else if (num5 >= 0f)
                        {
                            num8 = 0f;
                        }
                        else
                        {
                            num8 = -num5 / num3;
                        }
                    }
                }
                else if (num8 < 0f)
                {
                    num10 = num2 + num5;
                    num11 = sqrMagnitude + num4;
                    if (num11 > num10)
                    {
                        num12 = num11 - num10;
                        num13 = (sqrMagnitude - (2f * num2)) + num3;
                        if (num12 >= num13)
                        {
                            num8 = 1f;
                            num7 = 0f;
                        }
                        else
                        {
                            num8 = num12 / num13;
                            num7 = 1f - num8;
                        }
                    }
                    else
                    {
                        num8 = 0f;
                        if (num11 <= 0f)
                        {
                            num7 = 1f;
                        }
                        else if (num4 >= 0f)
                        {
                            num7 = 0f;
                        }
                        else
                        {
                            num7 = -num4 / sqrMagnitude;
                        }
                    }
                }
                else
                {
                    num12 = ((num3 + num5) - num2) - num4;
                    if (num12 <= 0f)
                    {
                        num7 = 0f;
                        num8 = 1f;
                    }
                    else
                    {
                        num13 = (sqrMagnitude - (2f * num2)) + num3;
                        if (num12 >= num13)
                        {
                            num7 = 1f;
                            num8 = 0f;
                        }
                        else
                        {
                            num7 = num12 / num13;
                            num8 = 1f - num7;
                        }
                    }
                }
            }
            return (Vector3) ((tr0 + (num7 * vector2)) + (num8 * rhs));
        }

        public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
        {
            int index = polyPoints.Length - 1;
            bool flag = false;
            int num2 = 0;
            while (num2 < polyPoints.Length)
            {
                if ((((polyPoints[num2].y <= p.y) && (p.y < polyPoints[index].y)) || ((polyPoints[index].y <= p.y) && (p.y < polyPoints[num2].y))) && (p.x < ((((polyPoints[index].x - polyPoints[num2].x) * (p.y - polyPoints[num2].y)) / (polyPoints[index].y - polyPoints[num2].y)) + polyPoints[num2].x)))
                {
                    flag = !flag;
                }
                index = num2++;
            }
            return flag;
        }

        public static bool ContainsPoint(Vector3[] polyPoints, Vector3 p)
        {
            int index = polyPoints.Length - 1;
            bool flag = false;
            int num2 = 0;
            while (num2 < polyPoints.Length)
            {
                if ((((polyPoints[num2].z <= p.z) && (p.z < polyPoints[index].z)) || ((polyPoints[index].z <= p.z) && (p.z < polyPoints[num2].z))) && (p.x < ((((polyPoints[index].x - polyPoints[num2].x) * (p.z - polyPoints[num2].z)) / (polyPoints[index].z - polyPoints[num2].z)) + polyPoints[num2].x)))
                {
                    flag = !flag;
                }
                index = num2++;
            }
            return flag;
        }

        public static bool ContainsPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
        {
            return ((IsClockwiseMargin(a, b, p) && IsClockwiseMargin(b, c, p)) && IsClockwiseMargin(c, a, p));
        }

        public static bool ContainsPoint(VInt2 a, VInt2 b, VInt2 c, VInt2 p)
        {
            return ((IsClockwiseMargin(a, b, p) && IsClockwiseMargin(b, c, p)) && IsClockwiseMargin(c, a, p));
        }

        public static bool ContainsPoint(VInt3 a, VInt3 b, VInt3 c, VInt3 p)
        {
            return ((IsClockwiseMargin(a, b, p) && IsClockwiseMargin(b, c, p)) && IsClockwiseMargin(c, a, p));
        }

        public static Vector3[] ConvexHull(Vector3[] points)
        {
            if (points.Length == 0)
            {
                return new Vector3[0];
            }
            List<Vector3> hullCache = Polygon.hullCache;
            hullCache.Clear();
            int index = 0;
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i].x < points[index].x)
                {
                    index = i;
                }
            }
            int num3 = index;
            int num4 = 0;
            do
            {
                hullCache.Add(points[index]);
                int num5 = 0;
                for (int j = 0; j < points.Length; j++)
                {
                    if ((num5 == index) || !Left(points[index], points[num5], points[j]))
                    {
                        num5 = j;
                    }
                }
                index = num5;
                num4++;
                if (num4 > 0x2710)
                {
                    Debug.LogWarning("Infinite Loop in Convex Hull Calculation");
                    break;
                }
            }
            while (index != num3);
            return hullCache.ToArray();
        }

        public static float DistanceSegmentSegment3D(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2)
        {
            float num8;
            float num11;
            Vector3 lhs = e1 - s1;
            Vector3 rhs = e2 - s2;
            Vector3 vector3 = s1 - s2;
            float num = Vector3.Dot(lhs, lhs);
            float num2 = Vector3.Dot(lhs, rhs);
            float num3 = Vector3.Dot(rhs, rhs);
            float num4 = Vector3.Dot(lhs, vector3);
            float num5 = Vector3.Dot(rhs, vector3);
            float num6 = (num * num3) - (num2 * num2);
            float num9 = num6;
            float num12 = num6;
            if (num6 < 1E-06f)
            {
                num8 = 0f;
                num9 = 1f;
                num11 = num5;
                num12 = num3;
            }
            else
            {
                num8 = (num2 * num5) - (num3 * num4);
                num11 = (num * num5) - (num2 * num4);
                if (num8 < 0f)
                {
                    num8 = 0f;
                    num11 = num5;
                    num12 = num3;
                }
                else if (num8 > num9)
                {
                    num8 = num9;
                    num11 = num5 + num2;
                    num12 = num3;
                }
            }
            if (num11 < 0f)
            {
                num11 = 0f;
                if (-num4 < 0f)
                {
                    num8 = 0f;
                }
                else if (-num4 > num)
                {
                    num8 = num9;
                }
                else
                {
                    num8 = -num4;
                    num9 = num;
                }
            }
            else if (num11 > num12)
            {
                num11 = num12;
                if ((-num4 + num2) < 0f)
                {
                    num8 = 0f;
                }
                else if ((-num4 + num2) > num)
                {
                    num8 = num9;
                }
                else
                {
                    num8 = -num4 + num2;
                    num9 = num;
                }
            }
            float num7 = (Math.Abs(num8) >= 1E-06f) ? (num8 / num9) : 0f;
            float num10 = (Math.Abs(num11) >= 1E-06f) ? (num11 / num12) : 0f;
            Vector3 vector4 = (Vector3) ((vector3 + (num7 * lhs)) - (num10 * rhs));
            return vector4.sqrMagnitude;
        }

        public static float IntersectionFactor(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
        {
            Vector3 vector = end1 - start1;
            Vector3 vector2 = end2 - start2;
            float num = (vector2.z * vector.x) - (vector2.x * vector.z);
            if (num == 0f)
            {
                return -1f;
            }
            float num2 = (vector2.x * (start1.z - start2.z)) - (vector2.z * (start1.x - start2.x));
            return (num2 / num);
        }

        public static bool IntersectionFactor(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out float factor1, out float factor2)
        {
            Vector3 vector = end1 - start1;
            Vector3 vector2 = end2 - start2;
            float num = (vector2.z * vector.x) - (vector2.x * vector.z);
            if ((num <= 1E-05f) && (num >= -1E-05f))
            {
                factor1 = 0f;
                factor2 = 0f;
                return false;
            }
            float num2 = (vector2.x * (start1.z - start2.z)) - (vector2.z * (start1.x - start2.x));
            float num3 = (vector.x * (start1.z - start2.z)) - (vector.z * (start1.x - start2.x));
            float num4 = num2 / num;
            float num5 = num3 / num;
            factor1 = num4;
            factor2 = num5;
            return true;
        }

        public static bool IntersectionFactor(VInt3 start1, VInt3 end1, VInt3 start2, VInt3 end2, out float factor1, out float factor2)
        {
            VInt3 num = end1 - start1;
            VInt3 num2 = end2 - start2;
            long num3 = (num2.z * num.x) - (num2.x * num.z);
            if (num3 == 0)
            {
                factor1 = 0f;
                factor2 = 0f;
                return false;
            }
            long num4 = (num2.x * (start1.z - start2.z)) - (num2.z * (start1.x - start2.x));
            long num5 = (num.x * (start1.z - start2.z)) - (num.z * (start1.x - start2.x));
            factor1 = ((float) num4) / ((float) num3);
            factor2 = ((float) num5) / ((float) num3);
            return true;
        }

        public static bool IntersectionFactor(VInt3 start1, VInt3 end1, VInt3 start2, VInt3 end2, out VFactor factor1, out VFactor factor2)
        {
            VInt3 num = end1 - start1;
            VInt3 num2 = end2 - start2;
            long num3 = (num2.z * num.x) - (num2.x * num.z);
            if (num3 == 0)
            {
                factor1 = VFactor.zero;
                factor2 = VFactor.zero;
                return false;
            }
            long num4 = (num2.x * (start1.z - start2.z)) - (num2.z * (start1.x - start2.x));
            long num5 = (num.x * (start1.z - start2.z)) - (num.z * (start1.x - start2.x));
            factor1 = new VFactor();
            VFactor factor = factor1;
            factor.nom = num4;
            factor.den = num3;
            factor1 = factor;
            factor2 = new VFactor();
            factor = factor2;
            factor.nom = num5;
            factor.den = num3;
            factor2 = factor;
            return true;
        }

        public static float IntersectionFactorRay(VInt3 start1, VInt3 end1, VInt3 start2, VInt3 end2)
        {
            VInt3 num = end1 - start1;
            VInt3 num2 = end2 - start2;
            int num3 = (num2.z * num.x) - (num2.x * num.z);
            if (num3 == 0)
            {
                return float.NaN;
            }
            int num4 = (num2.x * (start1.z - start2.z)) - (num2.z * (start1.x - start2.x));
            int num5 = (num.x * (start1.z - start2.z)) - (num.z * (start1.x - start2.x));
            if ((((float) num5) / ((float) num3)) < 0f)
            {
                return float.NaN;
            }
            return (((float) num4) / ((float) num3));
        }

        public static bool IntersectionFactorRaySegment(VInt3 start1, VInt3 end1, VInt3 start2, VInt3 end2)
        {
            VInt3 num = end1 - start1;
            VInt3 num2 = end2 - start2;
            long num3 = (num2.z * num.x) - (num2.x * num.z);
            if (num3 == 0)
            {
                return false;
            }
            long num4 = (num2.x * (start1.z - start2.z)) - (num2.z * (start1.x - start2.x));
            long num5 = (num.x * (start1.z - start2.z)) - (num.z * (start1.x - start2.x));
            if (!((num4 < 0L) ^ (num3 < 0L)))
            {
                return false;
            }
            if (!((num5 < 0L) ^ (num3 < 0L)))
            {
                return false;
            }
            return (((num3 < 0L) || (num5 <= num3)) && ((num3 >= 0L) || (num5 > num3)));
        }

        public static Vector2 IntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            bool flag;
            return IntersectionPoint(start1, end1, start2, end2, out flag);
        }

        public static Vector3 IntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
        {
            bool flag;
            return IntersectionPoint(start1, end1, start2, end2, out flag);
        }

        public static Vector2 IntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out bool intersects)
        {
            Vector2 vector = end1 - start1;
            Vector2 vector2 = end2 - start2;
            float num = (vector2.y * vector.x) - (vector2.x * vector.y);
            if (num == 0f)
            {
                intersects = false;
                return start1;
            }
            float num2 = (vector2.x * (start1.y - start2.y)) - (vector2.y * (start1.x - start2.x));
            float num3 = num2 / num;
            intersects = true;
            return (start1 + ((Vector2) (vector * num3)));
        }

        public static Vector3 IntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
        {
            Vector3 vector = end1 - start1;
            Vector3 vector2 = end2 - start2;
            float num = (vector2.z * vector.x) - (vector2.x * vector.z);
            if (num == 0f)
            {
                intersects = false;
                return start1;
            }
            float num2 = (vector2.x * (start1.z - start2.z)) - (vector2.z * (start1.x - start2.x));
            float num3 = num2 / num;
            intersects = true;
            return (start1 + ((Vector3) (vector * num3)));
        }

        public static VInt3 IntersectionPoint(ref VInt3 start1, ref VInt3 end1, ref VInt3 start2, ref VInt3 end2, out bool intersects)
        {
            VInt3 a = end1 - start1;
            VInt3 num2 = end2 - start2;
            long b = (num2.z * a.x) - (num2.x * a.z);
            if (b == 0)
            {
                intersects = false;
                return start1;
            }
            long m = (num2.x * (start1.z - start2.z)) - (num2.z * (start1.x - start2.x));
            intersects = true;
            return (IntMath.Divide(a, m, b) + start1);
        }

        public static Vector3 IntersectionPointOptimized(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2)
        {
            float num = (dir2.z * dir1.x) - (dir2.x * dir1.z);
            if (num == 0f)
            {
                return start1;
            }
            float num2 = (dir2.x * (start1.z - start2.z)) - (dir2.z * (start1.x - start2.x));
            float num3 = num2 / num;
            return (start1 + ((Vector3) (dir1 * num3)));
        }

        public static Vector3 IntersectionPointOptimized(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2, out bool intersects)
        {
            float num = (dir2.z * dir1.x) - (dir2.x * dir1.z);
            if (num == 0f)
            {
                intersects = false;
                return start1;
            }
            float num2 = (dir2.x * (start1.z - start2.z)) - (dir2.z * (start1.x - start2.x));
            float num3 = num2 / num;
            intersects = true;
            return (start1 + ((Vector3) (dir1 * num3)));
        }

        public static bool Intersects(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
        {
            Vector3 vector = end1 - start1;
            Vector3 vector2 = end2 - start2;
            float num = (vector2.z * vector.x) - (vector2.x * vector.z);
            if (num == 0f)
            {
                return false;
            }
            float num2 = (vector2.x * (start1.z - start2.z)) - (vector2.z * (start1.x - start2.x));
            float num3 = (vector.x * (start1.z - start2.z)) - (vector.z * (start1.x - start2.x));
            float num4 = num2 / num;
            float num5 = num3 / num;
            return (((num4 >= 0f) && (num4 <= 1f)) && ((num5 >= 0f) && (num5 <= 1f)));
        }

        public static bool Intersects(VInt2 a, VInt2 b, VInt2 a2, VInt2 b2)
        {
            return ((Left(a, b, a2) != Left(a, b, b2)) && (Left(a2, b2, a) != Left(a2, b2, b)));
        }

        public static bool Intersects(VInt3 a, VInt3 b, VInt3 a2, VInt3 b2)
        {
            return ((Left(a, b, a2) != Left(a, b, b2)) && (Left(a2, b2, a) != Left(a2, b2, b)));
        }

        public static bool IntersectsUnclamped(Vector3 a, Vector3 b, Vector3 a2, Vector3 b2)
        {
            return (Left(a, b, a2) != Left(a, b, b2));
        }

        public static bool IsClockwise(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) < 0f);
        }

        public static bool IsClockwise(VInt3 a, VInt3 b, VInt3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) < 0L);
        }

        public static bool IsClockwiseMargin(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) <= float.Epsilon);
        }

        public static bool IsClockwiseMargin(VInt2 a, VInt2 b, VInt2 c)
        {
            return ((((b.x - a.x) * (c.y - a.y)) - ((c.x - a.x) * (b.y - a.y))) <= 0L);
        }

        public static bool IsClockwiseMargin(VInt3 a, VInt3 b, VInt3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) <= 0L);
        }

        public static bool IsColinear(Vector3 a, Vector3 b, Vector3 c)
        {
            float num = ((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z));
            return ((num <= 1E-07f) && (num >= -1E-07f));
        }

        public static bool IsColinear(VInt3 a, VInt3 b, VInt3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) == 0L);
        }

        public static bool IsColinearAlmost(VInt3 a, VInt3 b, VInt3 c)
        {
            long num = ((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z));
            return ((num > -1L) && (num < 1L));
        }

        public static bool Left(Vector2 a, Vector2 b, Vector2 p)
        {
            return ((((b.x - a.x) * (p.y - a.y)) - ((p.x - a.x) * (b.y - a.y))) <= 0f);
        }

        public static bool Left(Vector3 a, Vector3 b, Vector3 p)
        {
            return ((((b.x - a.x) * (p.z - a.z)) - ((p.x - a.x) * (b.z - a.z))) <= 0f);
        }

        public static bool Left(VInt2 a, VInt2 b, VInt2 c)
        {
            return ((((b.x - a.x) * (c.y - a.y)) - ((c.x - a.x) * (b.y - a.y))) <= 0L);
        }

        public static bool Left(VInt3 a, VInt3 b, VInt3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) <= 0L);
        }

        public static bool LeftNotColinear(Vector3 a, Vector3 b, Vector3 p)
        {
            return ((((b.x - a.x) * (p.z - a.z)) - ((p.x - a.x) * (b.z - a.z))) < -1.401298E-45f);
        }

        public static bool LeftNotColinear(VInt3 a, VInt3 b, VInt3 c)
        {
            return ((((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z))) < 0L);
        }

        public static bool LineIntersectsBounds(Bounds bounds, Vector3 a, Vector3 b)
        {
            a -= bounds.center;
            b -= bounds.center;
            Vector3 vector = (Vector3) ((a + b) * 0.5f);
            Vector3 vector2 = a - vector;
            float x = Math.Abs(vector2.x);
            float y = Math.Abs(vector2.y);
            Vector3 vector3 = new Vector3(x, y, Math.Abs(vector2.z));
            Vector3 extents = bounds.extents;
            if (Math.Abs(vector.x) > (extents.x + vector3.x))
            {
                return false;
            }
            if (Math.Abs(vector.y) > (extents.y + vector3.y))
            {
                return false;
            }
            if (Math.Abs(vector.z) > (extents.z + vector3.z))
            {
                return false;
            }
            if (Math.Abs((float) ((vector.y * vector2.z) - (vector.z * vector2.y))) > ((extents.y * vector3.z) + (extents.z * vector3.y)))
            {
                return false;
            }
            if (Math.Abs((float) ((vector.x * vector2.z) - (vector.z * vector2.x))) > ((extents.x * vector3.z) + (extents.z * vector3.x)))
            {
                return false;
            }
            if (Math.Abs((float) ((vector.x * vector2.y) - (vector.y * vector2.x))) > ((extents.x * vector3.y) + (extents.y * vector3.x)))
            {
                return false;
            }
            return true;
        }

        public static Vector3 SegmentIntersectionPoint(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
        {
            Vector3 vector = end1 - start1;
            Vector3 vector2 = end2 - start2;
            float num = (vector2.z * vector.x) - (vector2.x * vector.z);
            if (num == 0f)
            {
                intersects = false;
                return start1;
            }
            float num2 = (vector2.x * (start1.z - start2.z)) - (vector2.z * (start1.x - start2.x));
            float num3 = (vector.x * (start1.z - start2.z)) - (vector.z * (start1.x - start2.x));
            float num4 = num2 / num;
            float num5 = num3 / num;
            if (((num4 < 0f) || (num4 > 1f)) || ((num5 < 0f) || (num5 > 1f)))
            {
                intersects = false;
                return start1;
            }
            intersects = true;
            return (start1 + ((Vector3) (vector * num4)));
        }

        public static Vector3[] Subdivide(Vector3[] path, int subdivisions)
        {
            subdivisions = (subdivisions >= 0) ? subdivisions : 0;
            if (subdivisions == 0)
            {
                return path;
            }
            Vector3[] vectorArray = new Vector3[((path.Length - 1) * ((int) Mathf.Pow(2f, (float) subdivisions))) + 1];
            int index = 0;
            for (int i = 0; i < (path.Length - 1); i++)
            {
                float num3 = 1f / Mathf.Pow(2f, (float) subdivisions);
                for (float j = 0f; j < 1f; j += num3)
                {
                    vectorArray[index] = Vector3.Lerp(path[i], path[i + 1], Mathf.SmoothStep(0f, 1f, j));
                    index++;
                }
            }
            vectorArray[index] = path[path.Length - 1];
            return vectorArray;
        }

        public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            return (((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z)));
        }

        public static long TriangleArea(VInt3 a, VInt3 b, VInt3 c)
        {
            return (((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z)));
        }

        public static float TriangleArea2(Vector3 a, Vector3 b, Vector3 c)
        {
            return (((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z)));
        }

        public static long TriangleArea2(VInt3 a, VInt3 b, VInt3 c)
        {
            return (((b.x - a.x) * (c.z - a.z)) - ((c.x - a.x) * (b.z - a.z)));
        }
    }
}

