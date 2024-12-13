using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public Transform linkedCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager checkpointManager = other.GetComponent<CheckpointManager>();
            if (checkpointManager != null)
            {
                checkpointManager.SetCheckpoint(linkedCheckpoint);
                Debug.Log("Checkpoint set to: " + linkedCheckpoint.name);
            }
        }
    }
}
