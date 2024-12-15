using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseController : MonoBehaviour
{
    public GameObject house;
    public GameObject jellyfish;
    public ValveInteraction valve1;
    public ValveInteraction valve2;
    public GameObject newTerrain;  // Assign the new terrain in the Inspector
    public GameObject oldTerrain; // Assign the old terrain in the Inspector
    public GameObject Player;
    private AudioSource ambience;
    private Player player_script;
    private AudioSource footstepsSound;
    public Text endingText;
    public Image image;
    public Canvas creditsCanvas; // Add reference to the credits canvas
    private Animator creditsAnimator; // Add reference to the Animator on the credits canvas
    public float fadeDistance = 5;
    public float fadeDuration = 2;

    // Start is called before the first frame update
    void Start()
    {
        house.SetActive(false);
        jellyfish.SetActive(false);
        endingText.enabled = false;
        ambience = Player.GetComponent<AudioSource>();
        player_script = Player.GetComponent<Player>();
        footstepsSound = Player.transform.Find("Footsteps").GetComponent<AudioSource>();
        newTerrain.SetActive(false);
        creditsCanvas.gameObject.SetActive(false); // Ensure credits canvas is hidden initially
        creditsAnimator = creditsCanvas.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (valve1.finished && valve2.finished)
        {
            ShowEnd();
            float distance = Vector3.Distance(Player.transform.position, house.transform.position);
            if (distance <= fadeDistance)
                StartCoroutine(EndGame());
        }
    }

    void ShowEnd()
    {
        house.SetActive(true);  
        jellyfish.SetActive(true);
        oldTerrain.SetActive(false);
        newTerrain.SetActive(true);
        endingText.enabled = true;
        ambience.mute = true;
        footstepsSound.mute = true;
        player_script.walkSpeed = 1f;
    }

    private IEnumerator EndGame()   
    {
        // Disable player movement
        player_script.walkSpeed = 0f;

        // Fade out the current image
        float fadeStart = 0;
        Color imageColor = image.color;
        while (fadeStart < fadeDuration)
        {
            fadeStart += Time.deltaTime;
            imageColor.a = Mathf.Clamp01(fadeStart / fadeDuration);
            image.color = imageColor;
            yield return null;
        }

        // Show credits canvas and start its fade-in
        creditsCanvas.gameObject.SetActive(true);
        fadeStart = 0; // Reset fade start
        CanvasGroup canvasGroup = creditsCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = creditsCanvas.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; // Ensure it starts invisible
        float canvasfadeStart = 0;
        while (canvasfadeStart < fadeDuration)
        {
            fadeStart += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(fadeStart / fadeDuration);
            yield return null;
        }

        // Start the credits animation
        if (creditsAnimator != null)
        {
            creditsAnimator.enabled = true; // Assumes an animator trigger called "StartCredits"
        }
    }
}
