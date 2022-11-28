using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshPuller : MonoBehaviour
{
    public BakedSpline BakedSource;
    public bool ForceBakeOnStart = true;
    public int MeshIndex = 0;

    void Start()
    {
        if (ForceBakeOnStart) BakedSource?.Bake();

        (GetComponent(typeof(MeshFilter)) as MeshFilter).mesh = BakedSource?.Meshes[MeshIndex];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
