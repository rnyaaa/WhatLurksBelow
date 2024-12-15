using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOverOnCollision : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject enemy;
    private bool isGameOver = false;
    //private float fadeTimer = 0f;
    public GameObject player;

    void Start()
    {
        deathScreen.SetActive(false);
    }

    void Update()
    {
        if ((enemy.transform.position - transform.position).magnitude < 3f && !isGameOver)
        {
            isGameOver = true;
            StartCoroutine(HandleGameOver());
        }
        if (isGameOver && Input.GetKeyDown("space"))
            StartCoroutine(ResetPosition());
    }

    private IEnumerator HandleGameOver()
    {
        deathScreen.SetActive(true);

        yield break;
    }
    private IEnumerator ResetPosition()
    {
        Transform checkpoint = FindObjectOfType<CheckpointManager>()?.GetCurrentCheckpoint();
        if (checkpoint == null)
        {
            Debug.LogError("Checkpoint is NULL! Cannot teleport player.");
            yield break;
        }

        if (enemy != null)
        {
            enemy.transform.position = new Vector3(90f, 219f, -90f);
        }

        CharacterController controller = player.GetComponent<CharacterController>();

        controller.enabled = false;
        player.transform.position = checkpoint.position;
        controller.enabled = true;


        yield return new WaitForSeconds(0.1f);

        // player.transform.position = checkpoint.position;
        // yield return new WaitForSeconds(0.1f);
        isGameOver = false;

        deathScreen.SetActive(false);
    }
}