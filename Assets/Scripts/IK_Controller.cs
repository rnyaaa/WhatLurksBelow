using UnityEngine;

public class SpiderIndividualizedLocomotion : MonoBehaviour
{
    [System.Serializable]
    public class LegConfig
    {
        public Transform target;
        public Transform hint;
        public bool isMoving;
        public Vector3 startPosition;
        public Vector3 targetPosition;
        public float movementProgress;
        public float movementDelay; // Individual movement delay
    }

    public LegConfig[] legs;
    
    [Header("Movement Parameters")]
    public float legMoveThreshold = 0.5f;
    public float legLiftHeight = 0.2f;
    public float legMoveSpeed = 2f;

    [Header("Randomization")]
    public float maxIndividualDelay = 0.5f;

    private void Start()
    {
        // Assign random initial delays
        foreach (var leg in legs)
        {
            leg.movementDelay = Random.Range(0f, maxIndividualDelay);
        }
    }

    private void Update()
    {
        UpdateLegMovement();
    }

    void UpdateLegMovement()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            var leg = legs[i];
            
            // Null checks
            if (leg.target == null || leg.hint == null) continue;

            // Find counterpart leg
            int counterpartIndex = CalculateCounterpartIndex(i);
            var counterpartLeg = legs[counterpartIndex];

            // Skip if counterpart is moving
            if (counterpartLeg.isMoving) continue;

            // Decrement movement delay
            if (leg.movementDelay > 0)
            {
                leg.movementDelay -= Time.deltaTime;
                continue;
            }

            // Calculate world space distance
            float distanceToHint = Vector3.Distance(
                new Vector3(leg.target.position.x, 0, leg.target.position.z), 
                new Vector3(leg.hint.position.x, 0, leg.hint.position.z)
            );

            // Trigger movement
            if (!leg.isMoving && distanceToHint > legMoveThreshold)
            {
                // Initialize movement
                leg.isMoving = true;
                leg.startPosition = leg.target.position;
                leg.targetPosition = new Vector3(leg.hint.position.x, 0, leg.hint.position.z);
                leg.movementProgress = 0;
                
                // Reset delay for next movement
                leg.movementDelay = Random.Range(0f, maxIndividualDelay);

                Debug.Log($"Leg {i} started moving. Distance: {distanceToHint}");
            }

            // Handle ongoing movement
            if (leg.isMoving)
            {
                // Increment progress
                leg.movementProgress += Time.deltaTime * legMoveSpeed;

                // Calculate arc movement
                float t = leg.movementProgress;
                Vector3 currentPos = Vector3.Lerp(leg.startPosition, leg.targetPosition, t);
                
                // Add vertical lift
                currentPos.y += Mathf.Sin(t * Mathf.PI) * legLiftHeight;

                // Update target position
                leg.target.position = currentPos;

                // Check if movement complete
                if (t >= 1f)
                {
                    leg.target.position = leg.targetPosition;
                    leg.isMoving = false;
                    Debug.Log($"Leg {i} finished moving");
                }
            }
        }
    }

    int CalculateCounterpartIndex(int currentIndex)
    {
        return (currentIndex + legs.Length / 2) % legs.Length;
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (legs == null) return;

        for (int i = 0; i < legs.Length; i++)
        {
            var leg = legs[i];
            if (leg.target == null || leg.hint == null) continue;

            // Color code: moving legs in red, stationary in green
            Gizmos.color = leg.isMoving ? Color.red : Color.green;
            Gizmos.DrawSphere(leg.target.position, 0.1f);
            Gizmos.DrawWireSphere(leg.hint.position, 0.1f);
            Gizmos.DrawLine(leg.target.position, leg.hint.position);
        }
    }
}