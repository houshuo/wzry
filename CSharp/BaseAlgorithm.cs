using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BaseAlgorithm
{
    public const int KINDA_SMALL_NUMBER = 2;
    public const float KINDA_SMALL_NUMBER_F = 0.001f;
    public const int SMALL_NUMBER = 1;
    public const float SMALL_NUMBER_F = 0.0001f;

    public static bool AddUniqueItem<T>(List<T> inList, T inPoint)
    {
        if (!inList.Contains(inPoint))
        {
            inList.Add(inPoint);
            return true;
        }
        return false;
    }

    public static void CalcBlendPctByFunc(EViewTargetBlendFunction inIndirectViewSightFunc, float inIndirectViewSightExp, float DurationPct, out float BlendPct)
    {
        BlendPct = 0f;
        switch (inIndirectViewSightFunc)
        {
            case EViewTargetBlendFunction.VTBlend_Linear:
                BlendPct = Lerp(0f, 1f, DurationPct);
                break;

            case EViewTargetBlendFunction.VTBlend_Cubic:
                BlendPct = CubicInterp(0f, 0f, 1f, 0f, DurationPct);
                break;

            case EViewTargetBlendFunction.VTBlend_EaseIn:
                BlendPct = FInterpEaseIn(0f, 1f, DurationPct, inIndirectViewSightExp);
                break;

            case EViewTargetBlendFunction.VTBlend_EaseOut:
                BlendPct = FInterpEaseOut(0f, 1f, DurationPct, inIndirectViewSightExp);
                break;

            case EViewTargetBlendFunction.VTBlend_EaseInOut:
                BlendPct = FInterpEaseInOut(0f, 1f, DurationPct, inIndirectViewSightExp);
                break;
        }
    }

    public static float CubicInterp(float P0, float T0, float P1, float T1, float A)
    {
        float num = A * A;
        float num2 = num * A;
        return (((((((2f * num2) - (3f * num)) + 1f) * P0) + (((num2 - (2f * num)) + A) * T0)) + ((num2 - num) * T1)) + (((-2f * num2) + (3f * num)) * P1));
    }

    public static float FInterpEaseIn(float A, float B, float Alpha, float Exp)
    {
        return Lerp(A, B, Mathf.Pow(Alpha, Exp));
    }

    public static float FInterpEaseInOut(float A, float B, float Alpha, float Exp)
    {
        float num;
        if (Alpha < 0.5f)
        {
            num = 0.5f * Mathf.Pow(2f * Alpha, Exp);
        }
        else
        {
            num = 1f - (0.5f * Mathf.Pow(2f * (1f - Alpha), Exp));
        }
        return Lerp(A, B, num);
    }

    public static float FInterpEaseOut(float A, float B, float Alpha, float Exp)
    {
        return Lerp(A, B, Mathf.Pow(Alpha, 1f / Exp));
    }

    public static bool IsNearlyZero(int inPoint, int Tolerance = 2)
    {
        return (Math.Abs(inPoint) < Tolerance);
    }

    public static bool IsNearlyZero(VInt2 inPoint, int Tolerance = 2)
    {
        return ((Math.Abs(inPoint.x) < Tolerance) && (Math.Abs(inPoint.y) < Tolerance));
    }

    public static bool IsNearlyZero(VInt3 inPoint, int Tolerance = 2)
    {
        return (((Math.Abs(inPoint.x) < Tolerance) && (Math.Abs(inPoint.y) < Tolerance)) && (Math.Abs(inPoint.z) < Tolerance));
    }

    public static float Lerp(float A, float B, float Alpha)
    {
        return (A + (Alpha * (B - A)));
    }

    public static Vector3 VLerp(Vector3 A, Vector3 B, float Alpha)
    {
        return (A + ((Vector3) (Alpha * (B - A))));
    }

    public enum EViewTargetBlendFunction
    {
        VTBlend_Linear,
        VTBlend_Cubic,
        VTBlend_EaseIn,
        VTBlend_EaseOut,
        VTBlend_EaseInOut,
        VTBlend_MAX
    }
}

