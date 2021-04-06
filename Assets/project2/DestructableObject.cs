using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

//[ExecuteInEditMode]
public class DestructableObject : MonoBehaviour
{
    private Mesh cloneMesh;
    private MeshFilter meshFilter;

    //[HideInInspector]
    public Vector3[] vertices; //for debugging
    
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
    }
    
    
    private void CloneMesh(Vector3[] newVerts, int[] newTris, Vector2[] newUV)
    {
        cloneMesh = new Mesh();

        cloneMesh.name = "meshClone";
        cloneMesh.vertices = newVerts;
        cloneMesh.triangles = newTris;
        cloneMesh.uv = newUV;
        meshFilter.mesh = cloneMesh;
        cloneMesh.RecalculateNormals();

        vertices = cloneMesh.vertices;
    }

    //function obtained from
    //https://www.raywenderlich.com/3169311-runtime-mesh-manipulation-with-unity
    // returns List of int that is related to the targetPt.
    private List<int> FindRelatedVertices(Vector3 targetPt, bool findConnected)
    {
        int[] triangles = meshFilter.mesh.triangles;
        // list of int
        List<int> relatedVertices = new List<int>();

        int idx = 0;
        Vector3 pos;

        // loop through triangle array of indices
        for (int t = 0; t < triangles.Length; t++)
        {
            // current idx return from tris
            idx = triangles[t];
            // current pos of the vertex
            pos = vertices[idx];
            // if current pos is same as targetPt
            if (pos == targetPt)
            {
                // add to list
                relatedVertices.Add(idx);
                // if find connected vertices
                if (findConnected)
                {
                    // min
                    // - prevent running out of count
                    if (t == 0)
                    {
                        relatedVertices.Add(triangles[t + 1]);
                    }
                    // max 
                    // - prevent runnign out of count
                    if (t == triangles.Length - 1)
                    {
                        relatedVertices.Add(triangles[t - 1]);
                    }
                    // between 1 ~ max-1 
                    // - add idx from triangles before t and after t 
                    if (t > 0 && t < triangles.Length - 1)
                    {
                        relatedVertices.Add(triangles[t - 1]);
                        relatedVertices.Add(triangles[t + 1]);
                    }
                }
            }
        }
        // return compiled list of int
        return relatedVertices;
    }

    public void ChangeSimilarVertices(int index, Vector3 playerPos, float force)
    {
        Vector3 targetVertexPos = meshFilter.mesh.vertices[index];

        List<int> relatedVertices = FindRelatedVertices(targetVertexPos, false);
        foreach (int i in relatedVertices)
        {
            vertices[i] = targetVertexPos - (playerPos.normalized * force);
        }
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
    }
    
    public void ChangeSingleVertex(int index, Vector3 playerPos, float force)
    {
        vertices[index] = vertices[index] - (playerPos.normalized * force);
    }

    /// <summary>
    /// Gets the nearby vertex indice of the vertex closest to the point
    /// </summary>
    /// <param name="point">the position you are looking for nearby vertices from.</param>
    /// <param name="searchRadius">the radius you want to search around the point by</param>
    /// <returns>Returns an integer index on this object's vertex array, or it returns -1 as a null</returns>
    public int getNearbyVerticeIndex(Vector3 point, float searchRadius, int triangleIndex)
    {
        int closest = -1;
        float shortest_distance = searchRadius;
        int i = 0;

        int[] tvs = {triangleIndex + 0, triangleIndex + 1, triangleIndex + 2};
        foreach (var tv in tvs)
        {
            float our_distance = Vector2.Distance(meshFilter.sharedMesh.vertices[tv] + transform.position, point);
            if (our_distance < shortest_distance)
            {
                shortest_distance = our_distance;
                closest = tv;
            }
        }
        return closest;
    }

    /// <summary>
    /// Adds a vertex to the end of 
    /// </summary>
    /// <param name="point">the position you are looking for nearby vertices from.</param>
    /// <param name="searchRadius">the radius you want to search around the point by</param>
    /// <returns>Returns an integer index on this object's vertex array, or it returns -1 as a null</returns>
    public int addVertexToPoint(Vector3 point, int triangleIndex)
    {
    
        Mesh originalMesh = meshFilter.sharedMesh;
        //Vector3[] newVs = new Vector3[originalMesh.vertices.Length + 1];
        Vector3[] newVs = originalMesh.vertices;

        int returnIndex = -1;
        /*int offset = 0;
        for (int i = 0; i < newVs.Length; i++)
        {
            if (i == triangleIndex + 2) //the index after the second vertex of the hit triangle
            {
                Debug.Log("adding vertex at " + i);
                newVs[i] = point;
                returnIndex = i;
                offset = -1;
                i++;
            }
            newVs[i] = originalMesh.vertices[i+offset];
        }*/
        
        Vector2[] newUVs = new Vector2[newVs.Length];
        for (int i = 0; i < newUVs.Length; i++)
        {
            Vector3 v = newVs[i];
            newUVs[i] = new Vector2(v.x, v.y);
        }

        int[] newTriangles = new int[(newVs.Length + 2) * 3];
        for (int i = 0; i < newVs.Length-3; i+=3)
        {
            newTriangles[i] = i;
            newTriangles[i + 1] = i + 1;
            newTriangles[i + 2] = i + 2;

            newTriangles[i + 3] = i + 2;
            newTriangles[i + 4] = i + 1;
            newTriangles[i + 5] = i + 3;
        }
        
        CloneMesh(newVs, newTriangles, newUVs);

        return returnIndex;
    }
}
