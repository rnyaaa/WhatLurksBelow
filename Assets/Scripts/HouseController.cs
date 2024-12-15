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
    }

    // Update is called once per frame
    void Update()
    {
        if(valve1.finished && valve2.finished)
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
        player_script.walkSpeed = 0f;
        float fadeStart = 0;
        Color imageColor = image.color;
        while (fadeStart < fadeDuration)
        {
            fadeStart += Time.deltaTime;
            imageColor.a = Mathf.Clamp01(fadeStart / fadeDuration);
            image.color = imageColor;
            yield return null;
        }
    }
}