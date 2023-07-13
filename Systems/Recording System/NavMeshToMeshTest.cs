using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;
using UnityEditor;
using System.IO;

namespace PropellerCap
{
    public class NavMeshToMeshTest : MonoBehaviour
    {
        [SerializeField] NavMeshData navMeshData;
        [SerializeField] Material material;
        [SerializeField] float _precision = 1f;
        [SerializeField] bool _subdivideMesh = true;
        [SerializeField] MeshFilter _targetMesh;
        [SerializeField] float _threshold = 0.1f;
        [ContextMenu("Generate Mesh")]
        private void GenerateMesh()
        {
            // Create a new Mesh object
            Mesh mesh = new Mesh();

            // Convert the NavMesh data to a Mesh
            NavMeshDataToMesh(navMeshData, out mesh);
            _CreateMeshObject(mesh);
        }

        private void _CreateMeshObject(Mesh mesh)
        {
            // Create a new GameObject and add a MeshFilter and MeshRenderer component
            GameObject meshObject = new GameObject("NavMesh Mesh");
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

            // Assign the Mesh to the MeshFilter and the Material to the MeshRenderer
            meshFilter.mesh = mesh;
            meshRenderer.material = material;
        }

        [ContextMenu("Subdivide Target")]
        void SubdivideTarget()
        {
            Mesh mesh = _targetMesh.sharedMesh;
            mesh = SubdivideMesh(mesh, _precision);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            _targetMesh.sharedMesh = mesh;
        }

        [ContextMenu("LogSubMeshCount")]
        void LogSubMeshCount()
        {
            Debug.Log($"Amount vertices: {_targetMesh.sharedMesh.vertices.Length}");
            Debug.Log($"Amount triangles: {_targetMesh.sharedMesh.triangles.Length}");
        }

        [ContextMenu("Simplify")]
        void CreateAsset()
        {
            _targetMesh.sharedMesh = MeshUtils.SimplifyMesh(_targetMesh.sharedMesh, 0.01f);
        }

        [ContextMenu("Divide mesh")]
        void DivideMesh()
        {
            Mesh mesh = MeshUtils.SimplifyMesh(_targetMesh.sharedMesh, 0.01f);
            //_CreateMeshObject(mesh);
            List<Mesh> subMeshes = MeshUtils.CreateSubMeshes(mesh, _threshold);
            foreach (var item in subMeshes)
            {
                _CreateMeshObject(item);
            }
            /*List<Mesh> submeshes = MeshUtils.SeparateLooseParts(_targetMesh.sharedMesh, _threshold);

            foreach (Mesh item in submeshes)
            {
                _CreateMeshObject(item);
            }*/

            /*List<Vector3> uniqueVertices = MeshUtils.GetUniqueVertices(_targetMesh.sharedMesh, 0.01f);

            foreach (Vector3 vertex in uniqueVertices)
            {
                Debug.Log(vertex);
            }*/
            /*List<Mesh> subMeshes = MeshUtils.SeparateMeshByLooseParts(mesh, _threshold);
            foreach (var item in subMeshes)
            {
                _CreateMeshObject(item);
            }*/
            /*List<List<int>> connectedComponents = FindConnectedComponents(mesh.vertices, mesh.triangles, _threshold);

            foreach (List<int> component in connectedComponents)
            {
                Mesh separatedMesh = ExtractMesh(mesh, component);
                _CreateMeshObject(separatedMesh);
            }*/
        }

        private void NavMeshDataToMesh(NavMeshData navMeshData, out Mesh mesh)
        {
            NavMesh.RemoveAllNavMeshData();
            NavMesh.AddNavMeshData(navMeshData);
            mesh = new Mesh();
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            mesh.vertices = triangles.vertices;
            mesh.triangles = triangles.indices;

            if (_subdivideMesh)
                mesh = SubdivideMesh(mesh, _precision);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        //----------------------------------------------------------------------------------------------------------------

        public Mesh SubdivideMesh(Mesh mesh, float precision)
        {
            Mesh newMesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>(mesh.vertices);
            List<int> triangles = new List<int>(mesh.triangles);

            while (true)
            {
                bool didSubdivide = false;

                for (int i = 0; i < triangles.Count; i += 3)
                {
                    int i1 = triangles[i];
                    int i2 = triangles[i + 1];
                    int i3 = triangles[i + 2];

                    Vector3 v1 = vertices[i1];
                    Vector3 v2 = vertices[i2];
                    Vector3 v3 = vertices[i3];

                    float d1 = (v2 - v1).magnitude;
                    float d2 = (v3 - v2).magnitude;
                    float d3 = (v1 - v3).magnitude;

                    if (d1 > precision && d2 > precision && d3 > precision)
                    {
                        Vector3 v12 = (v1 + v2) / 2f;
                        Vector3 v23 = (v2 + v3) / 2f;
                        Vector3 v31 = (v3 + v1) / 2f;

                        int i12 = vertices.Count;
                        vertices.Add(v12);
                        int i23 = vertices.Count;
                        vertices.Add(v23);
                        int i31 = vertices.Count;
                        vertices.Add(v31);

                        triangles[i] = i1;
                        triangles[i + 1] = i12;
                        triangles[i + 2] = i31;
                        triangles.Add(i12);
                        triangles.Add(i2);
                        triangles.Add(i23);
                        triangles.Add(i23);
                        triangles.Add(i3);
                        triangles.Add(i31);
                        triangles.Add(i12);
                        triangles.Add(i23);
                        triangles.Add(i31);

                        didSubdivide = true;
                    }
                }

                if (!didSubdivide)
                {
                    break;
                }
            }

            newMesh.vertices = vertices.ToArray();
            newMesh.triangles = triangles.ToArray();
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();

            return newMesh;
        }

        //----------------------------------------------------------------------------------------------------------------

        List<List<int>> FindConnectedComponents(Vector3[] vertices, int[] triangles, float threshold)
        {
            List<List<int>> components = new List<List<int>>();
            HashSet<int> unvisited = new HashSet<int>();

            for (int i = 0; i < vertices.Length; i++)
            {
                unvisited.Add(i);
            }

            while (unvisited.Count > 0)
            {
                int startVertex = unvisited.First();
                List<int> component = new List<int>();
                Queue<int> queue = new Queue<int>();
                queue.Enqueue(startVertex);
                unvisited.Remove(startVertex);

                while (queue.Count > 0)
                {
                    int vertex = queue.Dequeue();
                    component.Add(vertex);

                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        if (triangles[i] == vertex || triangles[i + 1] == vertex || triangles[i + 2] == vertex)
                        {
                            int v1 = triangles[i];
                            int v2 = triangles[i + 1];
                            int v3 = triangles[i + 2];

                            if (v1 != vertex && unvisited.Contains(v1) && Vector3.Distance(vertices[vertex], vertices[v1]) < threshold)
                            {
                                queue.Enqueue(v1);
                                unvisited.Remove(v1);
                            }

                            if (v2 != vertex && unvisited.Contains(v2) && Vector3.Distance(vertices[vertex], vertices[v2]) < threshold)
                            {
                                queue.Enqueue(v2);
                                unvisited.Remove(v2);
                            }

                            if (v3 != vertex && unvisited.Contains(v3) && Vector3.Distance(vertices[vertex], vertices[v3]) < threshold)
                            {
                                queue.Enqueue(v3);
                                unvisited.Remove(v3);
                            }
                        }
                    }
                }

                components.Add(component);
            }

            return components;
        }

        Mesh ExtractMesh(Mesh mesh, List<int> vertices)
        {
            Mesh newMesh = new Mesh();
            newMesh.vertices = vertices.ConvertAll(x => mesh.vertices[x]).ToArray();
            newMesh.triangles = mesh.triangles.Where(x => vertices.Contains(x)).Select(x => vertices.IndexOf(x)).ToArray();
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            return newMesh;
        }

        //----------------------------------------------------------------------------------------------------------------



    }
}
