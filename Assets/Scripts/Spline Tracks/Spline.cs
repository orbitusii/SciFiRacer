using System;
using UnityEngine;

[Serializable]
public class Spline
{
    [HideInInspector]
    public Node Start;
    [HideInInspector]
    public Node End;

    public float Length = 0;
    [Min(1)]
    public int PointCount = 0;

    public Vector3[] Points;

    /// <summary>
    /// A lookup table for time according to distance along the curve
    /// </summary>
    public AnimationCurve TimeLUT = new AnimationCurve();

    public Spline(Node s, Node e, float pointCountMultiplier = 1)
    {
        Start = s;
        End = e;

        float RPD = RawPointDistance(Start, End);

        (Length, TimeLUT) = CalculateLength(Mathf.RoundToInt(RPD));

        PointCount = Mathf.RoundToInt(Length * pointCountMultiplier);
        Points = GeneratePoints();
    }

    /// <summary>
    /// Approximates the length of this Spline and stores those length values to a LUT
    /// </summary>
    /// <returns>A tuple (float, AnimationCurve) with the length and LUT of t-values by Distance</returns>
    public (float length, AnimationCurve LUT) CalculateLength (int increments)
    {
        float _length = 0;
        AnimationCurve _lut = new AnimationCurve();

        Vector3 lastPoint = Start.Point;

        for (int i = 0; i <= increments; i++)
        {
            float t = (float)i / increments;

            Vector3 nextPoint = Evaluate(t);

            _length += Vector3.Distance(lastPoint, nextPoint);

            _lut.AddKey(_length, t);

            lastPoint = nextPoint;
        }

        return (_length, _lut);
    }

    public Vector3[] GeneratePoints ()
    {
        Vector3[] _points = new Vector3[PointCount+1];

        for (int i = 0; i <= PointCount; i++)
        {
            float t = (float)i / PointCount;

            _points[i] = Evaluate(t);
        }

        return _points;
    }

    public Vector3 Evaluate (float time)
    {
        return EvaluateNodes(Start, End, time);
    }

    public Vector3 Derivative (float time)
    {
        return EvaluateDeriv(Start, End, time);
    }

    public Vector3 Normal (float time)
    {
        Vector3 pos = Evaluate(time);
        Vector3 deriv = Derivative(time);
        Matrix4x4 mat = Matrix4x4.TRS(pos, Quaternion.LookRotation(deriv), Vector3.one);

        float lerpedRotation = Mathf.Lerp(Start.Rotation, End.Rotation, time);

        Vector3 rotatedNormal = Mathf.Cos(lerpedRotation) * Vector3.up + Mathf.Sin(lerpedRotation) * Vector3.right;
        return mat.MultiplyVector(rotatedNormal);
    }

    public Vector3 Tangent (float time)
    {
        float lerpedWidth = Mathf.Lerp(Start.Width, End.Width, time);

        return Vector3.Cross(Normal(time), Derivative(time).normalized);
    }

    public Vector3 SurfacePoint (float time, float width)
    {
        float lerpedWidth = Mathf.Lerp(Start.Width, End.Width, time);
        float clampedWidth = Mathf.Clamp(width, -lerpedWidth, lerpedWidth);

        float lerpedCurvature = Mathf.Lerp(Start.Curvature, End.Curvature, time);
        Vector3 point = Evaluate(time);
        Vector3 norm = Normal(time);
        Vector3 tang = Tangent(time);

        Vector3 onNorm = Vector3.zero;
        Vector3 onTang = clampedWidth * tang;

        if (lerpedCurvature != 0)
        {
            float radius = 1 / lerpedCurvature;

            float arcAngle = clampedWidth / radius;

            onNorm += (1 - radius * Mathf.Cos(arcAngle)) * norm;
            onTang *= radius * Mathf.Sin(arcAngle);
        }

        return point + onNorm + onTang;
    }

    public Vector3[] AllSurfacePoints (int timeSteps, int widthSteps, bool useUniformDistance = false)
    {
        Vector3[] allPoints = new Vector3[(timeSteps+1) * (widthSteps+1)];
        int index = 0;

        for(int i = 0; i <= timeSteps; i++)
        {
            // If we want Uniform Distance steps along the path, we need to evaluate the Time LUT
            // in order to get the time values for specific increments of distance
            float time = useUniformDistance ? TimeLUT.Evaluate((float)i / timeSteps * Length) : (float)i / timeSteps;

            float lerpedWidth = Mathf.Lerp(Start.Width, End.Width, time);

            float lerpedCurvature = Mathf.Lerp(Start.Curvature, End.Curvature, time) / lerpedWidth;
            float radiusOffset = lerpedCurvature * lerpedWidth / Mathf.PI;
            Vector3 point = Evaluate(time);
            Vector3 norm = Normal(time);
            Vector3 tang = Tangent(time);

            bool flat = lerpedCurvature == 0;
            float radius = flat ? 0 : 1 / lerpedCurvature;

            for (int j = 0; j <= widthSteps; j++)
            {
                float wTime = 2 * ((float)j / widthSteps) - 1;
                float widthPos = wTime * lerpedWidth;
                float arcAngle = widthPos / radius;

                Vector3 onNorm = flat ? Vector3.zero : (radius * (Mathf.Cos(arcAngle) - 1) + radiusOffset) * norm;
                Vector3 onTan = (flat ? widthPos : radius * Mathf.Sin(arcAngle)) * tang;

                allPoints[index] = point + onNorm + onTan;
                
                index++;
            }
        }

        return allPoints;
    }

    public static Vector3 EvaluateNodes (Node from, Node to, float time, bool clamp = true)
    {
        float t = clamp ? Mathf.Clamp01(time) : time;

        float tCube = Mathf.Pow(t, 3);
        float tSqr = Mathf.Pow(t, 2);

        Vector3 P0 = from.Point * (-tCube + 3 * tSqr - 3 * t + 1);
        Vector3 P1 = from.Fwd_World * (3 * tCube - 6 * tSqr + 3 * t);
        Vector3 P2 = to.Back_World * (-3 * tCube + 3 * tSqr);
        Vector3 P3 = to.Point * tCube;

        return P0 + P1 + P2 + P3;
    }

    public static Vector3 EvaluateDeriv (Node from, Node to, float time, bool clamp = true)
    {
        float t = clamp ? Mathf.Clamp01(time) : time;

        float tSqr = Mathf.Pow(t, 2);

        Vector3 P0 = from.Point * (-3 * tSqr + 6 * t - 3);
        Vector3 P1 = from.Fwd_World * (9 * tSqr - 12 * t + 3);
        Vector3 P2 = to.Back_World * (-9 * tSqr + 6 * t);
        Vector3 P3 = to.Point * 3 * tSqr;

        return P0 + P1 + P2 + P3;
    }

    public static float RawPointDistance (Node from, Node to)
    {
        Vector3 P0 = from.Point;
        Vector3 P1 = from.Fwd_World;
        Vector3 P2 = to.Back_World;
        Vector3 P3 = to.Point;

        return Vector3.Distance(P0, P1) + Vector3.Distance(P1, P2) + Vector3.Distance(P2, P3);
    }
}
