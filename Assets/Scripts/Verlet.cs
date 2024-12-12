using UnityEngine;
using System.Collections.Generic;




[RequireComponent(typeof(LineRenderer))]
public class Kelp : MonoBehaviour
{
    public int numSegments = 10; // Number of nodes in the kelp
    public float segmentLength = 0.5f; // Distance between nodes
    public KelpSegment[] segments;
    private LineRenderer lineRenderer;
    [System.Serializable]
public class KelpSegment
{
    public Vector3 position;
    public Vector3 previousPosition;
    public Vector3 acceleration;

    public KelpSegment()
    {
        this.position = Vector3.zero;
        this.previousPosition = position;
        this.acceleration = Vector3.zero;
    }
}

    void Start()
    {
        // Ensure the LineRenderer component is attached
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component missing! Please attach one to the GameObject.");
            return;
        }

        // Initialize segments
        segments = new KelpSegment[numSegments];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < numSegments; i++)
        {
            segments[i] = new KelpSegment
            {
                position = startPosition + Vector3.down * segmentLength * i,
                previousPosition = startPosition + Vector3.down * segmentLength * i,
                acceleration = Vector3.zero
            };
        }

        // Configure LineRenderer
        lineRenderer.positionCount = numSegments;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        if (segments == null || segments.Length == 0 || lineRenderer == null)
            return;

        ApplyForces();
        ApplyPhysics();
        ApplyConstraints();
        UpdateLineRenderer();
    }

    void ApplyForces()
    {
        foreach (var segment in segments)
        {
            segment.acceleration += new Vector3(0, -9.8f, 0) * Time.deltaTime; // Gravity
            segment.acceleration += new Vector3(Mathf.Sin(Time.time), 0, 0) * 2.0f; // Example water current
        }
    }

    void ApplyPhysics()
    {
        foreach (var segment in segments)
        {
            Vector3 velocity = segment.position - segment.previousPosition;
            segment.previousPosition = segment.position;
            segment.position += velocity; // Move based on velocity
            segment.position += segment.acceleration * Time.deltaTime * Time.deltaTime; // Apply forces
            segment.acceleration = Vector3.zero; // Reset acceleration for the next frame
        }
    }

    void ApplyConstraints()
    {
        for (int i = 1; i < segments.Length; i++)
        {
            KelpSegment current = segments[i];
            KelpSegment previous = segments[i - 1];

            Vector3 direction = current.position - previous.position;
            float distance = direction.magnitude;
            float error = distance - segmentLength;

            // Correct positions
            Vector3 correction = direction.normalized * (error * 0.5f);
            current.position -= correction;
            previous.position += correction;
        }
    }

    void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            positions[i] = segments[i].position;
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
}
