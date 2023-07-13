using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public static class MeshUtils
{
    //Works, maybe
    public static Mesh SimplifyMesh(Mesh mesh, float threshold)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<int> visitedIndexes = new List<int>();
        //List<int> newTriangles = new List<int>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (visitedIndexes.Contains(i)) continue;
            visitedIndexes.Add(i);

            for (int j = i + 1; j < vertices.Length; j++)
            {
                if (visitedIndexes.Contains(j)) continue;

                if (Vector3.Distance(vertices[i], vertices[j]) <= threshold)
                {
                    //Change the triangle list so that the vertex of J becomes I

                    for (int k = 0; k < triangles.Length; k += 3)
                    {
                        if (triangles[k] == j)
                        {
                            triangles[k] = i;
                        }
                    }
                }
            }
        }

        //Now there are vertices that are not assigned to the triangles

        int[] newTriangles = new int[mesh.triangles.Length];
        List<Vector3> newVertices = new List<Vector3>();
        //Iterate through the triangles and create a new list of vertices
        for (int k = 0; k < triangles.Length; k++)
        {
            //Add the vertex
            newVertices.Add(vertices[triangles[k]]);
            newTriangles[k] = k;
        }


        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();

        return newMesh;
    }

    public static List<Mesh> CreateSubMeshes(Mesh mesh, float threshold)
    {
        List<Mesh> toReturn = new List<Mesh>();
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<int> visitedIndexes = new List<int>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (visitedIndexes.Contains(i)) continue;

            Mesh subMesh = new Mesh();
            HashSet<Vector3> subMeshVertices = new HashSet<Vector3>();
            HashSet<int> subMeshTriangles = new HashSet<int>();

            List<int> indexesToVisit = new List<int>();
            indexesToVisit.Add(i);

            int targetIndex = i;
            while (indexesToVisit.Count > 0)
            {
                //Search for all the connected vertices to the initial one

                //Get the indexesToVisit from the triangles that contain this vertex
                for (int k = 0; k < triangles.Length - 3; k += 3)
                {
                    if ((triangles[k] == targetIndex || triangles[k + 1] == targetIndex || triangles[k + 2] == targetIndex) == false)
                        continue;
                    //Debug.Log($"Adding indexes {} {} {}");
                    //Here we are part of a triangle so we save the vertex in the submesh
                    if (visitedIndexes.Contains(k) == false && indexesToVisit.Contains(k) == false)
                        indexesToVisit.Add(k);
                    if (visitedIndexes.Contains(k + 1) == false && indexesToVisit.Contains(k + 1) == false)
                        indexesToVisit.Add(k + 1);
                    if (visitedIndexes.Contains(k + 2) == false && indexesToVisit.Contains(k + 2) == false)
                        indexesToVisit.Add(k + 2);

                    //Add the vertices and triangles to the submesh
                    int lastSubMeshVertexIndex = subMeshVertices.Count;
                    subMeshVertices.Add(vertices[k]);
                    subMeshVertices.Add(vertices[k + 1]);
                    subMeshVertices.Add(vertices[k + 2]);
                    subMeshTriangles.Add(k);
                    subMeshTriangles.Add(k + 1);
                    subMeshTriangles.Add(k + 2);
                }

                //Remove the index we just visited
                indexesToVisit.RemoveAt(0);
                visitedIndexes.Add(targetIndex);

                //Set the next index to visit
                if (indexesToVisit.Count > 0)
                    targetIndex = indexesToVisit[0];
            }

            subMesh.vertices = subMeshVertices.ToArray();
            subMesh.triangles = subMeshTriangles.ToArray();
            subMesh.RecalculateNormals();
            subMesh.RecalculateBounds();
            toReturn.Add(subMesh);
        }
        return toReturn;
    }



    public static List<Vector3> GetUniqueVertices(Mesh mesh, float threshold)
    {
        List<Vector3> uniqueVertices = new List<Vector3>();
        HashSet<Vector3> visitedVertices = new HashSet<Vector3>();

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            if (visitedVertices.Contains(vertex))
                continue;

            visitedVertices.Add(vertex);
            uniqueVertices.Add(vertex);

            for (int j = i + 1; j < mesh.vertexCount; j++)
            {
                Vector3 otherVertex = mesh.vertices[j];
                if (!visitedVertices.Contains(otherVertex) && Vector3.Distance(vertex, otherVertex) < threshold)
                {
                    visitedVertices.Add(otherVertex);
                }
            }
        }

        return uniqueVertices;
    }

    public static List<Mesh> SeparateMeshByLooseParts(Mesh mesh, float threshold)
    {
        List<Mesh> subMeshes = new List<Mesh>();
        HashSet<int> visited = new HashSet<int>();
        Debug.Log($"Amount vertices {mesh.vertexCount}");
        for (int i = 0; i < mesh.vertexCount; i++)
        {


            if (visited.Contains(i))
                continue;

            List<int> indices = new List<int>();
            DFS(mesh, i, threshold, visited, indices);

            if (indices.Count > 0)
            {
                subMeshes.Add(CreateMeshFromIndices(mesh, indices));
            }
        }

        return subMeshes;
    }

    private static void DFS(Mesh mesh, int index, float threshold, HashSet<int> visited, List<int> indices)
    {
        if (visited.Contains(index))
            return;

        visited.Add(index);
        indices.Add(index);

        Vector3 vertex = mesh.vertices[index];

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (visited.Contains(i))
                continue;

            Vector3 otherVertex = mesh.vertices[i];
            float distance = Vector3.Distance(vertex, otherVertex);

            if (distance < threshold)
            {
                bool isConnected = false;

                int[] triangles = mesh.triangles;
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    if (triangles[j] == index && (triangles[j + 1] == i || triangles[j + 2] == i))
                    {
                        isConnected = true;
                        break;
                    }
                    else if (triangles[j + 1] == index && (triangles[j] == i || triangles[j + 2] == i))
                    {
                        isConnected = true;
                        break;
                    }
                    else if (triangles[j + 2] == index && (triangles[j] == i || triangles[j + 1] == i))
                    {
                        isConnected = true;
                        break;
                    }
                }

                if (isConnected)
                {
                    DFS(mesh, i, threshold, visited, indices);
                }
            }
        }
    }

    private static Mesh CreateMeshFromIndices(Mesh mesh, List<int> indices)
    {
        Mesh subMesh = new Mesh();
        subMesh.vertices = indices.ConvertAll(x => mesh.vertices[x]).ToArray();
        subMesh.triangles = mesh.triangles.Where(x => indices.Contains(x)).Select(x => indices.IndexOf(x)).ToArray();
        subMesh.RecalculateNormals();
        return subMesh;
    }

}
