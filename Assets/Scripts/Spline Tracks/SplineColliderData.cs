using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
public class SplineColliderData : MonoBehaviour
{
    public SplineMeshPuller Parent;
    public Spline Spline;
    private MeshCollider mc;
    private Mesh mesh;
    private List<Color> colors = new List<Color>();

    public void Start()
    {
        mc = GetComponent<MeshCollider>();
        mesh = mc.sharedMesh;
        mesh.GetColors(colors);
    }

    public Matrix4x4 GetMatrixAtPoint(int triangle, Vector3 point)
    {
        Color col = GetSplineValuesOnTriangle(triangle, point);
        float dist = col.r * Spline.Length;
        float t = Spline.TimeLUT.Evaluate(dist);

        float wT = col.g * 2 - 1;

        Vector3 surfPoint = Spline.SurfacePoint(t, wT);
        Vector3 deriv = Spline.Derivative(t);
        Vector3 normal = Spline.SurfaceNormal(t, wT);

        return Matrix4x4.TRS(surfPoint, Quaternion.LookRotation(deriv, normal), Vector3.one);
    }

    public Color GetSplineValuesOnTriangle (int triangle, Vector3 point)
    {
        int index0 = mesh.triangles[triangle * 3 + 0];
        int index1 = mesh.triangles[triangle * 3 + 1];
        int index2 = mesh.triangles[triangle * 3 + 2];

        var pt0 = GetVertexValues(index0);
        var pt1 = GetVertexValues(index1);
        var pt2 = GetVertexValues(index2);

        Vector3 vecDeriv = pt1.point - pt0.point;
        Vector3 vecTang = pt2.point - pt0.point;

        float dot01 = Vector3.Dot(vecDeriv, point - pt0.point);
        float dot02 = Vector3.Dot(vecTang, point - pt0.point);
        float dotHypot = Mathf.Sqrt(dot01 * dot01 + dot02 * dot02);

        Color col = Color.Lerp(pt1.color, pt2.color, dotHypot);
        return col;
    }

    public (Vector3 point, Color color) GetVertexValues (int vertex)
    {
        Vector3 point = mesh.vertices[vertex];
        Color color = colors[vertex];

        return (point, color);
    }
}