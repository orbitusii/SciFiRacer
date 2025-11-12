using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
public class SplineColliderData : MonoBehaviour
{
    public SplineMeshPuller Parent;
    public Spline Spline;
    public Spline Next;
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
        Vector2 surfPos = GetSurfacePosition(triangle, point);

        return GetMatrixAtPoint(surfPos);
    }

    public Matrix4x4 GetMatrixAtPoint(Vector2 surfPos)
    {
        return Spline.GetMatrixAtPoint(surfPos);
    }

    public Vector2 GetSurfacePosition (int triangle, Vector3 point)
    {
        Color col = GetSplineValuesOnTriangle(triangle, point);
        float dist = col.r * Spline.Length;
        float t = Spline.TimeLUT.Evaluate(dist);

        float wT = col.g;

        return new Vector2(t, wT);
    }

    private Color GetSplineValuesOnTriangle (int triangle, Vector3 point)
    {
        int index0 = mesh.triangles[triangle * 3 + 0];
        int index1 = mesh.triangles[triangle * 3 + 1];
        int index2 = mesh.triangles[triangle * 3 + 2];

        var pt0 = GetVertexValues(index0);
        var pt1 = GetVertexValues(index1);
        var pt2 = GetVertexValues(index2);

        Debug.DrawLine(pt0.point, pt1.point);
        Debug.DrawLine(pt1.point, pt2.point);
        Debug.DrawLine(pt2.point, pt0.point);
        Debug.DrawRay(pt0.point, Vector3.up);

        Vector3 vecDeriv = pt1.point - pt0.point;
        Vector3 vecTang = pt2.point - pt0.point;
        Vector3 ptRelative = point - pt0.point;

        float dotDeriv = Vector3.Dot(vecDeriv.normalized, ptRelative) / vecDeriv.magnitude;
        float dotTang = Vector3.Dot(vecTang.normalized, ptRelative) / vecTang.magnitude;

        //Debug.DrawRay(pt0.point, vecDeriv * dotDeriv, Color.red);
        //Debug.DrawRay(pt0.point, vecTang * dotTang, Color.green);

        Debug.DrawRay(pt0.point+ vecTang * dotTang, vecDeriv * dotDeriv, Color.red);
        Debug.DrawRay(pt0.point+ vecDeriv * dotDeriv, vecTang * dotTang, Color.green);

        float r = Mathf.Lerp(pt0.color.r, pt1.color.r, dotDeriv);
        float g = Mathf.Lerp(pt0.color.g, pt2.color.g, dotTang);
        float b = Mathf.Lerp(pt0.color.b, pt1.color.b, dotDeriv);

        return new Color(r, g, b);
    }

    public (Vector3 point, Color color) GetVertexValues (int vertex)
    {
        Vector3 point = mesh.vertices[vertex];
        Color color = colors[vertex];

        return (point, color);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        int sPoints = Spline.Points.Length;

        for (int i = 0; i < sPoints - 1; i++)
        {
            Gizmos.DrawLine(Spline.Points[i], Spline.Points[i + 1]);
        }

        Gizmos.color = Color.gray;

        int nPoints = Next.GeneratePoints().Length;

        for (int i = 0; i < nPoints - 1; i++)
        {
            Gizmos.DrawLine(Next.Points[i], Next.Points[i + 1]);
        }
    }
}