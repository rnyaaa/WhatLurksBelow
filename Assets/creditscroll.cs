using UnityEngine;
using UnityEngine.UI;

public class RollingCredits : MonoBehaviour
{
    public float scrollSpeed = 50f; // Speed of the rolling credits
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Move the credits upward over time
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }
}