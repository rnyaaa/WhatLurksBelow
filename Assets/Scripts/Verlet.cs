using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class Kelp : MonoBehaviour
{
    public int numSegments = 10; // Number of nodes in the kelp
    public float segmentLength = 0.5f; // Distance between nodes
    public float leafSpawnChance = 0.3f; // Chance a segment spawns a leaf
    public float leafLength = 0.3f; // Length of the leaf
    public float gravity;
    public KelpSegment[] segments;
    private LineRenderer lineRenderer;
    private LineRenderer leafRenderer; // New LineRenderer for leaves

    [System.Serializable]
    public class KelpSegment
    {
        public Vector3 position;
        public Vector3 previousPosition;
        public Vector3 acceleration;
        public Leaf leaf; // Optional leaf attached to this segment

        public KelpSegment()
        {
            this.position = Vector3.zero;
            this.previousPosition = position;
            this.acceleration = Vector3.zero;
            this.leaf = null;
        }
    }

    [System.Serializable]
    public class Leaf
    {
        public Vector3 position;
        public Vector3 previousPosition;
        public Vector3 acceleration;
        public KelpSegment parentSegment;

        public Leaf(KelpSegment parent, float length)
        {
            this.parentSegment = parent;
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * length;
            this.position = parent.position + offset;
            this.previousPosition = this.position;
            this.acceleration = Vector3.zero;
        }
    }

    public Material kelpMaterial; // Material for the kelp line
    public Material leafMaterial; // Material for the leaves

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component missing! Please attach one to the GameObject.");
            return;
        }

        // Assign materials
        if (kelpMaterial != null)
        {
            lineRenderer.material = kelpMaterial;
        }

        // Configure the leaf LineRenderer
        GameObject leafLineRendererObject = new GameObject("LeafLineRenderer");
        leafLineRendererObject.transform.SetParent(transform);
        leafRenderer = leafLineRendererObject.AddComponent<LineRenderer>();
        leafRenderer.useWorldSpace = true;
        leafRenderer.startWidth = 0.05f;
        leafRenderer.endWidth = 0.02f;
        leafRenderer.positionCount = 0;

        if (leafMaterial != null)
        {
            leafRenderer.material = leafMaterial; // Assign leaf material if provided
        }

        // Initialize segments
        segments = new KelpSegment[numSegments];
        Vector3 startPosition = transform.position;

        for (int i = 0; i < numSegments; i++)
        {
            segments[i] = new KelpSegment
            {
                position = startPosition + Vector3.down * segmentLength * i,
                previousPosition = startPosition + Vector3.down * segmentLength * (i + 0.1f), // Slight offset
                acceleration = Vector3.zero
            };

            // Random chance to spawn a leaf
            if (Random.value < leafSpawnChance)
            {
                segments[i].leaf = new Leaf(segments[i], leafLength);
            }
        }

        // Configure LineRenderer for the kelp stem
        lineRenderer.positionCount = numSegments;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.05f;
        UpdateLineRenderer();
    }

    void FixedUpdate()
    {
        ApplyForces();
        ApplyPhysics();
        ApplyConstraints();
        UpdateLineRenderer();
    }

    void ApplyForces()
    {
        for (int i = 1; i < segments.Length; i++)
        {
            KelpSegment segment = segments[i];
            Vector3 direction = segment.position - segments[i - 1].position;
            float distance = direction.magnitude;
            float stretchForce = (distance - segmentLength) * 0.5f;

            segment.acceleration -= direction.normalized * stretchForce;
            segments[i - 1].acceleration += direction.normalized * stretchForce;

            // Apply forces to leaves
            if (segment.leaf != null)
            {
                Vector3 leafDirection = segment.leaf.position - segment.position;
                float leafDistance = leafDirection.magnitude;
                float leafStretchForce = (leafDistance - leafLength) * 0.5f;

                segment.leaf.acceleration -= leafDirection.normalized * leafStretchForce;
                segment.acceleration += leafDirection.normalized * leafStretchForce;

                // Maintain a rigid orientation
                Vector3 targetOffset = (segment.leaf.position - segment.position).normalized * leafLength;
                Vector3 targetPosition = segment.position + targetOffset;
                Vector3 correctionForce = (targetPosition - segment.leaf.position) * 0.2f; // Adjust 0.2f for stiffness
                segment.leaf.acceleration += correctionForce;
            }
        }

        foreach (var segment in segments)
        {
            // Add buoyancy-like upward force to counteract gravity
            segment.acceleration += new Vector3(Mathf.Sin(Time.time), gravity, 0); // Reduced gravity for kelp buoyancy
            if (segment.leaf != null)
            {
                // Leaves are less affected by gravity but still tethered to the segment
                segment.leaf.acceleration += new Vector3(0, gravity / 2f, 0);
            }
        }
    }


    void ApplyPhysics()
    {
        foreach (var segment in segments)
        {
            Vector3 velocity = segment.position - segment.previousPosition;
            segment.previousPosition = segment.position;

            velocity *= 0.98f; // Damping
            segment.position += velocity + segment.acceleration * Time.deltaTime * Time.deltaTime;
            segment.acceleration = Vector3.zero;

            // Update leaf physics
            if (segment.leaf != null)
            {
                Vector3 leafVelocity = segment.leaf.position - segment.leaf.previousPosition;
                segment.leaf.previousPosition = segment.leaf.position;

                leafVelocity *= 0.98f; // Damping for leaves
                segment.leaf.position += leafVelocity + segment.leaf.acceleration * Time.deltaTime * Time.deltaTime;
                segment.leaf.acceleration = Vector3.zero;
            }
        }
    }

    void ApplyConstraints()
    {
        // Anchor the root segment to the GameObject's position
        segments[0].position = transform.position;

        // Handle the first segment's leaf
        if (segments[0].leaf != null)
        {
            Vector3 leafDirection = segments[0].leaf.position - segments[0].position;
            float leafDistance = leafDirection.magnitude;
            float leafError = leafDistance - leafLength;

            // Maintain a rigid angle
            Vector3 targetOffset = leafDirection.normalized * leafLength;
            Vector3 targetPosition = segments[0].position + targetOffset;
            Vector3 leafCorrection = targetPosition - segments[0].leaf.position;

            segments[0].leaf.position += leafCorrection * 0.5f; // Adjust for rigidity
        }

        // Constrain the rest of the segments
        for (int i = 1; i < segments.Length; i++)
        {
            KelpSegment current = segments[i];
            KelpSegment previous = segments[i - 1];

            Vector3 direction = current.position - previous.position;
            float distance = direction.magnitude;
            float error = distance - segmentLength;

            Vector3 correction = direction.normalized * (error * 0.5f);
            current.position -= correction;
            previous.position += correction;

            // Constrain leaves
            if (current.leaf != null)
            {
                Vector3 leafDirection = current.leaf.position - current.position;
                float leafDistance = leafDirection.magnitude;
                float leafError = leafDistance - leafLength;

                // Maintain a rigid angle
                Vector3 targetOffset = (current.leaf.position - current.position).normalized * leafLength;
                Vector3 targetPosition = current.position + targetOffset;
                Vector3 leafCorrection = targetPosition - current.leaf.position;

                current.leaf.position += leafCorrection * 0.5f; // Adjust for rigidity
            }
        }
    }

    void UpdateLineRenderer()
    {
        // Update main segments
        Vector3[] segmentPositions = new Vector3[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            segmentPositions[i] = segments[i].position;
        }
        lineRenderer.positionCount = segmentPositions.Length;
        lineRenderer.SetPositions(segmentPositions);

        // Update leaves
        List<Vector3> leafPositions = new List<Vector3>();
        foreach (var segment in segments)
        {
            if (segment.leaf != null)
            {
                leafPositions.Add(segment.position); // Add parent segment position
                leafPositions.Add(segment.leaf.position); // Add leaf position
                leafPositions.Add(segment.position); // Add parent segment position
            }
        }

        leafRenderer.positionCount = leafPositions.Count;
        leafRenderer.SetPositions(leafPositions.ToArray());
    }
}
