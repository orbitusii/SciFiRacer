using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu()]
public class BakedSpline: ScriptableObject
{
    public Spline[] Originals;

    [SerializeField, HideInInspector]
    private List<Vector3[]> pointArrays;

    public Mesh[] Meshes;

    [ContextMenu("Bake!")]
    public void Bake ()
    {
        var args = new SplineBakeArguments(1, 16, true);
        Meshes = SplineBaker.BakeSplines(Originals, args);
    }
}
