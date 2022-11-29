using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshPuller : MonoBehaviour
{
    public SplineBakeArguments BakeArgs = new SplineBakeArguments(1, 16, true, true);
    public SplineTrack SourceSplines;
    private Mesh[] bakedMeshes;
    public bool ForceBakeOnStart = true;
    public int MeshIndex = 0;

    public bool GenerateColliders;
    public int[] ColliderIndices;

    void Start()
    {
        if (SourceSplines is null) return;

        bakedMeshes = SplineBaker.BakeSplines(SourceSplines.Splines, BakeArgs);

        (GetComponent(typeof(MeshFilter)) as MeshFilter).mesh = bakedMeshes[MeshIndex];

        if(GenerateColliders)
        {
            Transform parent = this.transform;

            foreach(int i in ColliderIndices)
            {
                Mesh m = bakedMeshes[i];
                GameObject newCol = new GameObject($"Collider {i}");
                newCol.transform.parent = parent;

                var meshCol = (MeshCollider)newCol.AddComponent(typeof(MeshCollider));
                meshCol.sharedMesh = m;
            }
        }
    }


}
