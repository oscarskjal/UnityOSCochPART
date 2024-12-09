using System.Collections.Generic;
using UnityEngine;

public class ElasticTrampoline : MonoBehaviour
{
    public float springConstant = 50f;          // Hooke's constant för stiffness
    public float damping = 0.98f;               // Damping factor för att reducera oscillation
    public float mass = 0.1f;                   // Massan av varje vertex
    public float gravity = -9.81f;              // Gravity 
    public float restLength = 0.5f;             // Spring length
    public float maxForce = 100f;               // Maximala kraften 

    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private Vector3[] velocities;
    private List<int>[] neighborIndices;

    void Start()
    {
        // Meshen och initialisera meshen
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        velocities = new Vector3[originalVertices.Length];

        originalVertices.CopyTo(displacedVertices, 0);

        // Hitta grannen för fjädern
        neighborIndices = new List<int>[originalVertices.Length];
        FindNeighbors();
    }

    void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            // Skippa fixade vertexerna
            if (IsFixedVertex(i))
                continue;

            Vector3 force = Vector3.zero;

            // Hooke's Law
            foreach (int neighborIndex in neighborIndices[i])
            {
                Vector3 displacement = displacedVertices[i] - displacedVertices[neighborIndex];
                float distance = displacement.magnitude;

                Vector3 direction = displacement.normalized;

                // Mera Hooke's Law: F = -k * (distance - restLength)
                float springForceMagnitude = -springConstant * (distance - restLength);
                Vector3 springForce = direction * springForceMagnitude;

                force += springForce; // Spring force
            }

            // Laga till restorative force till original positionen
            Vector3 restorativeForce = -springConstant * (displacedVertices[i] - originalVertices[i]);
            force += restorativeForce;

            // Gravivtation
            force += new Vector3(0, gravity * mass, 0);

            // Clampa totala kraften
            force = Vector3.ClampMagnitude(force, maxForce);

            // Accelertion
            Vector3 acceleration = force / mass;

            // Damping
            velocities[i] = (velocities[i] + acceleration * Time.deltaTime) * damping;

            // Updatera positionen
            displacedVertices[i] += velocities[i] * Time.deltaTime;
        }

        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds(); // Uppdatera bounds for kollision
        GetComponent<MeshCollider>().sharedMesh = mesh; // Updatera mesh collidern
    }

    void FindNeighbors()
    {
        // Initialisera grannens listan
        for (int i = 0; i < originalVertices.Length; i++)
        {
            neighborIndices[i] = new List<int>();
        }

        float threshold = 0.6f * restLength;
        for (int i = 0; i < originalVertices.Length; i++)
        {
            for (int j = 0; j < originalVertices.Length; j++)
            {
                if (i != j && Vector3.Distance(originalVertices[i], originalVertices[j]) <= threshold)
                {
                    neighborIndices[i].Add(j);
                }
            }
        }
    }

   bool IsFixedVertex(int index)
{
    Bounds bounds = mesh.bounds;
    Vector3 vertex = originalVertices[index];
    return Mathf.Abs(vertex.x - bounds.min.x) < 0.01f ||
           Mathf.Abs(vertex.x - bounds.max.x) < 0.01f ||
           Mathf.Abs(vertex.z - bounds.min.z) < 0.01f ||
           Mathf.Abs(vertex.z - bounds.max.z) < 0.01f;
}


    void OnDrawGizmos()
    {
        if (mesh == null) return;

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            foreach (int neighborIndex in neighborIndices[i])
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(displacedVertices[i], displacedVertices[neighborIndex]);
            }
        }
    }
}
