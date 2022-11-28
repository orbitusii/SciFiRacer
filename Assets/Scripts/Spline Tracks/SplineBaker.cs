using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SplineBaker
{
    public static Mesh[] BakeSplines (Spline[] splines, SplineBakeArguments args)
    {
        List<Mesh> meshes = new List<Mesh>();
        
        foreach(Spline original in splines)
        {
            Mesh newMesh = new Mesh();

            Spline s = new Spline(original.Start, original.End, args.PointMultiplier);

            int vertices = (s.PointCount + 1) * (args.WidthSteps + 1);

            // If we're making A LOT of points, we'll have to trim it down to a manageable number
            if(vertices > Mathf.Pow(2, 16))
            {
                s.PointCount = Mathf.RoundToInt(Mathf.Pow(2, 16) / (args.WidthSteps +1)) -1;
            }

            Vector3[] points = s.AllSurfacePoints(s.PointCount, args.WidthSteps, true);
            //Debug.Log($"{points.Length}");

            newMesh.SetVertices(points);

            newMesh.SetTriangles(ComputeTris(s.PointCount + 1, args.WidthSteps + 1), 0);

            newMesh.RecalculateNormals(UnityEngine.Rendering.MeshUpdateFlags.Default);

            newMesh.Optimize();
            newMesh.UploadMeshData(false);
            meshes.Add(newMesh);
        }

        if(args.CombineMeshes)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            Mesh combined = new Mesh();
            CombineInstance[] CIs = meshes.Select(x => new CombineInstance() { mesh = x, subMeshIndex = 0, transform = mat }).ToArray();

            combined.CombineMeshes(CIs, true);

            combined.Optimize();
            combined.RecalculateNormals();
            combined.UploadMeshData(false);

            meshes.Insert(0, combined);
        }

        return meshes.ToArray();
    }

    private static int[] ComputeTris (int rows, int columns)
    {
        int[] triArray = new int[rows * columns * 6];
        //Debug.Log(rows * columns * 6);
        int index = 0;

        for(int i = 0; i < rows - 1; i++)
        {
            for(int j = 0; j < columns - 1; j++)
            {
                int BL = i * columns + j;
                int BR = BL + 1;
                int UL = (i + 1) * columns + j;
                int UR = UL + 1;

                // First tri for this quad
                triArray[index++] = BL;
                triArray[index++] = UL;
                triArray[index++] = BR;

                // Second tri for this quad
                triArray[index++] = UL;
                triArray[index++] = UR;
                triArray[index++] = BR;
            }
        }

        return triArray;
    }
}

[System.Serializable]
public struct SplineBakeArguments
{
    public float PointMultiplier;
    public int WidthSteps;
    public bool UniformSteps;
    public bool CombineMeshes;

    public SplineBakeArguments (float pointMult = 1, int wSteps = 16, bool uniform = true, bool combine = false)
    {
        PointMultiplier = pointMult;
        WidthSteps = wSteps;
        UniformSteps = uniform;

        CombineMeshes = combine;
    }
}