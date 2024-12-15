using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValveInteraction : MonoBehaviour
{
    public GameObject player;
    public float range;
    [SerializeField] private Text interactionText;
    [SerializeField] private Light valvelight;
    public bool finished = false;
    public bool complete = false;
    public float rotationSpeed = 2f;
    public AudioSource valveturning;

    void Update()
    {
        if ((player.transform.position - transform.position).magnitude < range)
        {
            if (!complete)
            {
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown("e"))
                {
                    interactionText.gameObject.SetActive(false);
                    complete = true;
                    StartCoroutine(RotateValve());
                }
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private IEnumerator RotateValve()   
    {
        Debug.Log("FIXING");
        if (valveturning != null)
        {
            valveturning.Play();
        }
        else
        {
            Debug.LogError("AudioSource 'valveturning' is not assigned in the Inspector!");
        }

        float targetAngle = transform.localEulerAngles.z + 90f;
        float currentAngle = transform.localEulerAngles.z;
        float elapsedTime = 0f;
        float duration = Mathf.Abs(90f / rotationSpeed);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float angle = Mathf.Lerp(currentAngle, targetAngle, elapsedTime / duration);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, targetAngle);

        valvelight.color = Color.green;
        finished = true;
    }
}
