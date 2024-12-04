using UnityEngine;

public class PipePlacement : MonoBehaviour
{
    public GameObject startEndPipe;
    public GameObject straightPipe;
    public GameObject cornerPipe;
    public GameObject end;
    private Vector3 forward;
    private Vector3 currentPosition;


    public void Start()
    {
        // start at current point
        currentPosition = transform.position;

        // place startPipe
        Vector3 endPosition   = end.transform.position;
        endPosition.y += 4f;

        Vector3 strongestDirection = getNewForward(endPosition);
        forward = strongestDirection;
        PosStart(strongestDirection);

        // routine:
        bool finished = false;
        int maxIterations = 1000; // Failsafe to prevent infinite loop
        int iterationCount = 0;
        while(!finished)
        {

            if (iterationCount++ > maxIterations)
            {
                Debug.LogError("Pipe placement exceeded maximum iterations. Exiting to prevent freeze.");
                break;
            }
            strongestDirection = getNewForward(endPosition);
            Debug.Log($"Current Position: {currentPosition}, Distance to End: {(endPosition - currentPosition).magnitude}");
            placePipe(strongestDirection);

            if((endPosition - currentPosition).magnitude < 6f)
            {
                finished = true;
            }
        }
        
        // Place the end pipe
        Vector3 invForward = forward * -1f;
        Vector3 endPipeOrigin = currentPosition;
        endPipeOrigin.y -= 4f;
        endPipeOrigin = endPipeOrigin - invForward * 2f;
        Instantiate(startEndPipe, endPipeOrigin, Quaternion.LookRotation(invForward));
    }

    Vector3 getNewForward(Vector3 endPosition)
    {
        Vector3 toEnd = endPosition - currentPosition;

        Vector3 absToEnd = new Vector3(Mathf.Abs(toEnd.x), Mathf.Abs(toEnd.y), Mathf.Abs(toEnd.z));
        Vector3 strongestDirection = Vector3.zero;

        if (absToEnd.x >= absToEnd.y && absToEnd.x >= absToEnd.z)
        {
            // X
            strongestDirection = new Vector3(toEnd.x > 0 ? 1 : -1, 0, 0);

        }
        else if (absToEnd.y >= absToEnd.x && absToEnd.y >= absToEnd.z)
        {
            // Y
            strongestDirection = new Vector3(0, toEnd.y > 0 ? 1 : -1, 0);
        }
        else
        {
            // Z
            strongestDirection = new Vector3(0, 0, toEnd.z > 0 ? 1 : -1);
        }

        return strongestDirection;
    }

    // Y + 4
    // X or Z +/- 2
    void PosStart(Vector3 newForward)
    {
        Instantiate(straightPipe, currentPosition, Quaternion.LookRotation(newForward));
        currentPosition.y += 4;
        currentPosition += newForward * 2;
        forward = newForward;
    }
    
    // forward + 4
    void incPosSeg()
    {
        currentPosition += forward * 4;
    }

    // forward + 2 
    // X / Y / Z + 2
    void incPosCurve(Vector3 newForward)
    {
        currentPosition += (forward * 2) + (newForward * 2);
        forward = newForward;
    }

    // FORWARD is the forward unit vector of the last placed pipe (direction the pipe continues)
    // toEnd is the direction we want to move in (may be forward or curve left/right/down/up)
    // both will be in this format: Vector3(1, 0, 0) or Vector3(0, -1, 0)
    void placePipe(Vector3 toEnd)
    {

        float dot = Vector3.Dot(forward, toEnd);
        // GOING FORWARD
        if(dot == 0)
        {
            Debug.Log("Forward");
            Instantiate(straightPipe, currentPosition, Quaternion.LookRotation(forward));
            incPosSeg();
        } else {
            Vector3 cross = Vector3.Cross(forward, toEnd);
            Vector3 newForward = Vector3.zero;
            // RIGHT (z-aligned)
            if (cross.z > 0)
            {
                Debug.Log("Curve to the right (z-aligned)");
                newForward = new Vector3(0, 0, 1);

            }
            // LEFT (z-aligned)
            else if (cross.z < 0)
            {
                Debug.Log("Curve to the left (z-aligned)");
                newForward = new Vector3(0, 0, -1);
            }
            // RIGHT (x-aligned)
            if (cross.x > 0)
            {
                Debug.Log("Curve to the right (x-aligned)");
                newForward = new Vector3(1, 0, 0);

            }
            // LEFT (x-aligned)
            else if (cross.x < 0)
            {
                Debug.Log("Curve to the left (x-aligned)");
                newForward = new Vector3(-1, 0, 0);
            }
            // UP
            if (cross.y > 0)
            {
                Debug.Log("Curve upward");
                newForward = new Vector3(0, 1, 0);
            }
            // DOWN
            else if (cross.y < 0)
            {
                Debug.Log("Curve downward");
                newForward = new Vector3(0, -1, 0);
            }
            Instantiate(cornerPipe, currentPosition, Quaternion.LookRotation(newForward));
            incPosCurve(newForward);
        }
    }
}

