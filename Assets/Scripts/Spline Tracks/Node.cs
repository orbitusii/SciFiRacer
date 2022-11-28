using System;
using UnityEngine;

[Serializable]
public class Node
{
    public Vector3 Point;

    public Vector3 Fwd_World
    {
        get => _fwd + Point;
        set
        {
            Fwd_Local = value - Point;
        }
    }
    public Vector3 Fwd_Local
    {
        get => _fwd;
        set
        {
            _fwd = value;
            MoveOppositeNode(Mode, ref _fwd, ref _back);
        }
    }
    [SerializeField]
    private Vector3 _fwd = Vector3.forward;

    public Vector3 Back_World
    {
        get => _back + Point;
        set
        {
            Back_Local = value - Point;
        }
    }
    public Vector3 Back_Local
    {
        get => _back;
        set
        {
            _back = value;
            MoveOppositeNode(Mode, ref _back, ref _fwd);
        }
    }
    [SerializeField]
    private Vector3 _back = Vector3.back;

    public NodeMode Mode = NodeMode.AutoWeightDir;

    /// <summary>
    /// The rotation of this Node's Normal and Tangent, in Radians
    /// </summary>
    public float Rotation = 0;
    public float Width = 1;
    [Range(- Mathf.PI, Mathf.PI)]
    public float Curvature = 0;

    public Matrix4x4 Matrix => Matrix4x4.TRS(Point, Quaternion.LookRotation(Fwd_Local - Back_Local, Vector3.up), Vector3.one);

    private Vector3 RotatedNormal => Mathf.Cos(Rotation) * Vector3.up + Mathf.Sin(Rotation) * Vector3.right;
    public Vector3 Normal_Between => Matrix.MultiplyVector(RotatedNormal);
    public Vector3 Tangent_Right => Vector3.Cross(Normal_Between, Fwd_Local.normalized);

    private void MoveOppositeNode(NodeMode mode, ref Vector3 active, ref Vector3 opp)
    {
        switch (mode)
        {
            case NodeMode.AutoWeightDir:
                opp = -active;
                return;

            case NodeMode.AutoDir:
                float mag = opp.magnitude;
                opp = (-active.normalized * mag);
                return;

            default:
                return;
        }
    }

    public void DrawGizmo()
    {
        var lastColor = Gizmos.color;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Point, 0.2f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Point, Fwd_World);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Point, Back_World);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Point, Normal_Between);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(Point, Tangent_Right * Width);

        Gizmos.color = lastColor;
    }
}