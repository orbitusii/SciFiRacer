using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu()]
public class BakedSpline: ScriptableObject
{
    public SplineBakeArguments Args = new SplineBakeArguments(1, 16, true, true);

    public Spline[] Originals;

    [SerializeField, HideInInspector]
    private List<Vector3[]> pointArrays;

    public Mesh[] Meshes;

    [ContextMenu("Bake!")]
    public void Bake ()
    {
        Meshes = SplineBaker.BakeSplines(Originals, Args);
    }
}
