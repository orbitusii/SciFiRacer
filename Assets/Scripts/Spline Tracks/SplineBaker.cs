using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log($"{points.Length}");

            newMesh.SetVertices(points);

            newMesh.SetTriangles(ComputeTris(s.PointCount, args.WidthSteps), 0);

            newMesh.RecalculateNormals(UnityEngine.Rendering.MeshUpdateFlags.Default);

            newMesh.Optimize();
            newMesh.UploadMeshData(false);
            meshes.Add(newMesh);
        }

        return meshes.ToArray();
    }

    private static int[] ComputeTris (int rows, int columns)
    {
        int[] triArray = new int[rows * columns * 6];
        Debug.Log(rows * columns);
        int index = 0;

        for(int i = 0; i < rows-1; i++)
        {
            for(int j = 0; j < columns-1; j++)
            {

                // First tri for this quad
                triArray[index+0] = index;
                triArray[index+1] = index + columns + 1;
                triArray[index+2] = index + 1;

                // Second tri for this quad
                triArray[index+3] = index + columns + 2;
                triArray[index+4] = index + 1;
                triArray[index+5] = index + columns + 1;

                index += 6;
            }
        }

        return triArray;
    }
}

public struct SplineBakeArguments
{
    public float PointMultiplier;
    public int WidthSteps;
    public bool UniformSteps;

    public SplineBakeArguments (float pointMult = 1, int wSteps = 16, bool uniform = true)
    {
        PointMultiplier = pointMult;
        WidthSteps = wSteps;
        UniformSteps = uniform;
    }
}