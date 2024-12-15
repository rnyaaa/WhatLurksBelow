using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOverOnCollision : MonoBehaviour
{
    //public CanvasGroup fadeCanvas; // Reference to a UI Canvas Group for fading
    //public Text deathText;         // Reference to a UI Text element
    //public float fadeDuration = 1.5f; // Time it takes to fade to black
    public GameObject enemy;
    private bool isGameOver = false;
    //private float fadeTimer = 0f;
    public GameObject player;

    void Start()
    {
        //if (fadeCanvas != null)
        //{
            //fadeCanvas.alpha = 0;
        //}
        //if (deathText != null)
        //{
            //deathText.enabled = false;
        //}
    }

    void Update()
    {
        if ((enemy.transform.position - transform.position).magnitude < 3f && !isGameOver)
        {
            isGameOver = true; // Prevent multiple triggers
            StartCoroutine(HandleGameOver());
        }
    }

    private IEnumerator HandleGameOver()
    {
        Debug.Log("GameOver started");

        yield return null; // Wait 1 frame

        Transform checkpoint = FindObjectOfType<CheckpointManager>()?.GetCurrentCheckpoint();
        if (checkpoint == null)
        {
            Debug.LogError("Checkpoint is NULL! Cannot teleport player.");
            yield break;
        }

        if (enemy != null)
        {
            enemy.transform.position = new Vector3(90f, 219f, -90f);
            Debug.Log("Enemy teleported.");
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        CharacterController controller = player.GetComponent<CharacterController>();
        if (rb != null)
        {
            rb.MovePosition(checkpoint.position);
            Debug.Log("Player teleported with Rigidbody.");
        }
        else
        {
            controller.enabled = false;
            player.transform.position = checkpoint.position;
            Debug.Log("Player teleported using transform.");
            controller.enabled = true;
        }

        yield return new WaitForSeconds(0.1f); // Give physics time to settle

        player.transform.position = checkpoint.position; // Just to be sure
        Debug.Log($"Player position verified: {player.transform.position}");
        yield return new WaitForSeconds(0.1f);
        isGameOver = false;
    }
}

