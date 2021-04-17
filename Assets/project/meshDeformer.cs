using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class meshDeformer : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] currentVertices, displacedVertices, initialVertices;
    private bool[] wasDisplaced;

    private Vector3[] vertexVelocities;

    public float springForce = 20f;
    public float damping = 5f;
    public float maxDisplacement = 1f;
    private float uniformScale = 1f;

    private void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        currentVertices = deformingMesh.vertices;
        initialVertices = currentVertices;
        wasDisplaced = new bool[currentVertices.Length];
        displacedVertices = new Vector3[currentVertices.Length];
        uniformScale = transform.localScale.x;
        for (int i = 0; i < currentVertices.Length; i++)
        {
            displacedVertices[i] = currentVertices[i];
            wasDisplaced[i] = false;
        }
        vertexVelocities = new Vector3[currentVertices.Length];
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - currentVertices[i];
        displacement *= uniformScale;
        velocity -= displacement; //* (springForce * Time.deltaTime);
        //velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        if (Vector3.Distance(displacement, initialVertices[i]) >
            maxDisplacement && wasDisplaced[i]) return;
        
        wasDisplaced[i] = true;
        displacedVertices[i] += velocity * uniformScale;
        currentVertices[i] = displacedVertices[i];
    }

    private void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        float reducedForce = force / (1f + pointToVertex.sqrMagnitude) /1000;
        vertexVelocities[i] += pointToVertex.normalized * reducedForce;
    }

    public void AddDeformingForce(Vector3 point, float force, float radius_from_point)
    {
        point = transform.InverseTransformPoint(point);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            if ((Vector3.Distance(displacedVertices[i], point)) >= radius_from_point)
            {
                continue;
            }
            AddForceToVertex(i, point, force);
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    }

    /*
    public float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }
    
    **code was used for testing**
    public float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;
        return 1.0f / 6.0f * (-v321 + v231 + v312 - v132 - v213 + v123);
    }

    public void TellMeVolume()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        float volume = VolumeOfMesh(mesh);
        string msg = "The volume of the mesh is " + volume + " cube units.";
        Debug.Log(msg);
    }*/

    //returns the array position of the nearest vertex to the given point
    //give the array, then the point
    public int NearestVertexTo(Vector3[] arr, Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        
        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        int i = 0;
        int closesti = 0;
        foreach (Vector3 vertex in arr)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closesti = i;
            }

            i++;
        }

        // convert nearest vertex back to world space
        return closesti;
    }

    //the point to add, then array to add it to, then the position you are adding it at
    public Vector3[] AddToArrayAtIndex(Vector3 x ,Vector3[] arr, int pos)
    {
        int i;

        // create a new array of size n+1 
        Vector3[] newarr = new Vector3[arr.Length + 1]; 
  
        // insert the elements from the  
        // old array into the new array 
        // insert all elements till pos 
        // then insert x at pos 
        // then insert rest of the elements 
        for (i = 0; i < arr.Length + 1; i++) { 
            if (i < pos - 1) 
                newarr[i] = arr[i]; 
            else if (i == pos - 1) 
                newarr[i] = x; 
            else
                newarr[i] = arr[i - 1]; 
        }

        return newarr;
    }
}
