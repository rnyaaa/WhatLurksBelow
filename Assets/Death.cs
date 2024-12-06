using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverOnCollision : MonoBehaviour
{
    public CanvasGroup fadeCanvas; // Reference to a UI Canvas Group for fading
    public Text deathText;         // Reference to a UI Text element
    public float fadeDuration = 1.5f; // Time it takes to fade to black
    public GameObject enemy;
    private bool isGameOver = false;
    private float fadeTimer = 0f;

    void Start()
    {
        // Ensure UI elements are properly initialized
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0; // Start transparent
        }
        if (deathText != null)
        {
            deathText.enabled = false; // Hide the text initially
        }
    }

    void Update()
    {
        Debug.Log((enemy.transform.position - transform.position).magnitude);
        if ((enemy.transform.position - transform.position).magnitude < 3f)
        {
            // Fade to black
            if (fadeCanvas != null && fadeCanvas.alpha < 1)
            {
                fadeTimer += Time.deltaTime / fadeDuration;
                fadeCanvas.alpha = Mathf.Clamp01(fadeTimer);
            }

            AudioListener.volume = 1f-Mathf.Clamp01(fadeTimer);

            // Restart on Space key
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
