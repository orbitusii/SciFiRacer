using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshPuller : MonoBehaviour
{
    public BakedSpline BakedSource;
    public bool ForceBakeOnStart = true;
    public int MeshIndex = 0;

    public bool GenerateColliders;
    public int[] ColliderIndices;

    void Start()
    {
        if (ForceBakeOnStart) BakedSource?.Bake();

        (GetComponent(typeof(MeshFilter)) as MeshFilter).mesh = BakedSource?.Meshes[MeshIndex];

        if(GenerateColliders)
        {
            Transform parent = this.transform;

            foreach(int i in ColliderIndices)
            {
                Mesh m = BakedSource.Meshes[i];
                GameObject newCol = new GameObject($"Collider {i}");
                newCol.transform.parent = parent;

                var meshCol = (MeshCollider)newCol.AddComponent(typeof(MeshCollider));
                meshCol.sharedMesh = m;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
