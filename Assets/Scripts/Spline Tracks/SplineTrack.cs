using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTrack : MonoBehaviour
{
    public bool Close = true;
    public bool UseUniformDistances = false;

    [Range(0.1f, 2f)]
    public float PointsMultiplier = 1;
    [Range(1, 16)]
    public int WidthSteps = 32;

    public Node[] Nodes;
    private int NodeCount;

    public Spline[] Splines = new Spline[0];

    public BakedSpline Target;

    [ContextMenu("Regenerate Splines")]
    public void RegenerateSplines()
    {
        NodeCount = Nodes.Length;

        Splines = new Spline[NodeCount - (Close ? 0 : 1)];

        int SL = Splines.Length;

        for (int i = 0; i < SL; i++)
        {
            Splines[i] = new Spline(Nodes[i], Nodes[(i + 1) % NodeCount], PointsMultiplier);
        }
    }

    [ContextMenu("Bake Surface Points")]
    public void BakePoints ()
    {
        RegenerateSplines();

        if (Target != null)
        {
            Target.Originals = (Spline[])Splines.Clone();
            Target.Bake();
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach(Node n in Nodes)
        {
            n.DrawGizmo();
        }

        foreach(Spline s in Splines)
        {
            Vector3 normAt025 = s.Normal(0.25f);
            Vector3 normAt050 = s.Normal(0.5f);
            Vector3 normAt075 = s.Normal(0.75f);

            Vector3 tanAt025 = s.Tangent(0.25f);
            Vector3 tanAt050 = s.Tangent(0.5f);
            Vector3 tanAt075 = s.Tangent(0.75f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(s.Evaluate(0.25f), normAt025);
            Gizmos.DrawRay(s.Evaluate(0.5f), normAt050);
            Gizmos.DrawRay(s.Evaluate(0.75f), normAt075);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(s.Evaluate(0.25f), tanAt025);
            Gizmos.DrawRay(s.Evaluate(0.5f), tanAt050);
            Gizmos.DrawRay(s.Evaluate(0.75f), tanAt075);

            Gizmos.color = Color.gray;
            Vector3[] allPoints = s.AllSurfacePoints(s.PointCount, WidthSteps, true);

            foreach (Vector3 pt in allPoints)
            {
                Gizmos.DrawSphere(pt, 0.1f);
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (Splines is null)
        {
            RegenerateSplines();
        }

        foreach (Spline s in Splines)
        {
            int sPoints = s.Points.Length;

            for (int i = 0; i < sPoints - 1; i++)
            {
                Gizmos.DrawLine(s.Points[i], s.Points[i + 1]);
            }
        }
    }
}
