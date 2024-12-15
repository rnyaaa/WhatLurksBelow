using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float respawnDelay = 2f;
    public Text youDiedText;
    private CheckpointManager checkpointManager;

    private void Start()
    {
        checkpointManager = FindObjectOfType<CheckpointManager>();
        youDiedText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Die();
        }
    }

    private void Die()
    {
        youDiedText.gameObject.SetActive(true);
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        youDiedText.gameObject.SetActive(false);
        Transform checkpoint = checkpointManager.GetCurrentCheckpoint();
        transform.position = checkpoint.position;
    }
}
