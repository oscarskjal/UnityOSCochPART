using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Pendelum : MonoBehaviour
{
    public Transform pivot;             // The pivot of the pendulum (moving with crane)
    public float length = 5f;           // Längden av pendeln
    public float gravity = 9.81f;       // Gravitations konstant
    public float damping = 0.995f;      // Damping för saktande av objektet över tid
    public float initialAngleX = 45f;   // Initiala vinkeln
    public float initialAngleZ = 45f;   // Initial vinkeln

    private float angleInRadiansX;      // Vinkeln i radianer
    private float angleInRadiansZ;      // Vinklemn i radianer
    private float angularVelocityX = 0f; // Utsätta kraften i början
    private float angularVelocityZ = 0f; // Utsätta kraften i början
    private float angularAccelerationX; 
    private float angularAccelerationZ; 
    private Vector3 previousPivotPosition; // föregående positionen
    private Vector3 pivotVelocity;         // Pivotens hastighet
    private Vector3 previousPivotVelocity;  // Föregående hastighet
    private Vector3 pivotAcceleration;      // Accelerationen av punkten

    private LineRenderer lineRenderer;  // Linjen

    void Start()
    {
        // Convertering till radianer
        angleInRadiansX = initialAngleX * Mathf.Deg2Rad;
        angleInRadiansZ = initialAngleZ * Mathf.Deg2Rad;

        // Initialisering av previousPivotPosition
        previousPivotPosition = pivot.position;
        previousPivotVelocity = Vector3.zero;

        // Linjen
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // LineRenderer egenskaper
        lineRenderer.positionCount = 2; // Start och end points
        lineRenderer.startWidth = 0.05f; 
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); 
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // Pivot hastighet och acceleration
        pivotVelocity = (pivot.position - previousPivotPosition) / Time.deltaTime;
        pivotAcceleration = (pivotVelocity - previousPivotVelocity) / Time.deltaTime;

        // Updatera pendelns rörelse
        UpdatePendulumMotion(pivotAcceleration);

        // Updatera pendelns position 
        UpdatePendulumPosition();

        // Updatera linjen
        UpdateLineRenderer();

        // Spara position och hastighet för nästa frame
        previousPivotPosition = pivot.position;
        previousPivotVelocity = pivotVelocity;
    }

    void UpdatePendulumMotion(Vector3 pivotAcceleration)
    {
        // Calculering av acceleration
        angularAccelerationX = -(gravity / length) * Mathf.Sin(angleInRadiansX);
        angularAccelerationZ = -(gravity / length) * Mathf.Sin(angleInRadiansZ);

        // Punktens acceleration effect i motsat direction för correct pendel respons
        angularAccelerationX -= pivotAcceleration.x / length; 
        angularAccelerationZ -= pivotAcceleration.z / length; 

        // Vinkel acceleration samt hastighet
        angularVelocityX += angularAccelerationX * Time.deltaTime;
        angularVelocityZ += angularAccelerationZ * Time.deltaTime;

        // Updatera vinkeln baserat på vinkel-hastigheten
        angleInRadiansX += angularVelocityX * Time.deltaTime;
        angleInRadiansZ += angularVelocityZ * Time.deltaTime;

        // Damping
        angularVelocityX *= damping;
        angularVelocityZ *= damping;
    }

    void UpdatePendulumPosition()
    {
        if (pivot != null)
        {
            // Pendelns nya rörelse
            Vector3 pendulumOffset = new Vector3(
                Mathf.Sin(angleInRadiansX) * length,
                -Mathf.Cos(angleInRadiansX) * length,
                Mathf.Sin(angleInRadiansZ) * length
            );

            // Updatera pendelns position
            transform.position = pivot.position + pendulumOffset;
        }
    }

    void UpdateLineRenderer()
    {
        if (lineRenderer != null && pivot != null)
        {
            // Start position av pivot punkten samt ändan av bollen
            lineRenderer.SetPosition(0, pivot.position);
            lineRenderer.SetPosition(1, transform.position);
        }
    }
}
