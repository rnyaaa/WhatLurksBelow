using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValveInteraction : MonoBehaviour
{
    public GameObject player;
    public float range;
    [SerializeField] private Text interactionText;
    public bool finished = false;
    public float rotationSpeed = 2f;

    void Update()
    {
        if ((player.transform.position - transform.position).magnitude < range)
        {
            if (!finished)
            {
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown("e"))
                {
                    interactionText.gameObject.SetActive(false);
                    finished = true;
                    FixValve();
                }
            }
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }


    void FixValve()
    {
        StartCoroutine(RotateValve());
    }

    private IEnumerator RotateValve()   
    {
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
    }
}
