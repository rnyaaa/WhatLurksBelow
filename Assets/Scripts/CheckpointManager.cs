using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public Transform[] checkpoints; // List of all checkpoints
    private Transform currentCheckpoint; // Current active checkpoint

    void Start()
    {
        if (checkpoints.Length > 0)
        {
            currentCheckpoint = checkpoints[0]; // Start at the first checkpoint by default
        }
    }

    public Transform GetCurrentCheckpoint()
    {
        return currentCheckpoint != null ? currentCheckpoint : checkpoints[0];
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint != null)
        {
            currentCheckpoint = checkpoint;
            Debug.Log("Checkpoint set to: " + checkpoint.name);
        }
    }
}
