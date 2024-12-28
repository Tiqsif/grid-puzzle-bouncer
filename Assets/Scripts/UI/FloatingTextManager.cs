using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingTextManager : MonoBehaviour
{
    // Singleton instance
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField] private GameObject textPrefab; // Prefab for the TextMeshPro element
    [SerializeField] private Canvas canvas;         // Reference to the Canvas

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance != null && Instance != this) // If another instance already exists, destroy this instance and return out
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // Ensure references are set
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas not found on the parent hierarchy.");
            }
        }
    }

    /// <summary>
    /// Creates a floating text on the screen.
    /// </summary>
    /// <param name="message">The text to display.</param>
    /// <param name="fadeInDuration">Duration for the fade-in effect.</param>
    /// <param name="displayDuration">How long the text stays fully visible.</param>
    /// <param name="fadeOutDuration">Duration for the fade-out effect.</param>
    public void CreateFloatingText(string message, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        if (textPrefab == null || canvas == null)
        {
            Debug.LogError("Text prefab or canvas reference is missing.");
            return;
        }

        // Instantiate text object and set parent to Canvas
        GameObject newText = Instantiate(textPrefab, canvas.transform);
        TextMeshProUGUI tmp = newText.GetComponent<TextMeshProUGUI>();

        if (tmp != null)
        {
            tmp.text = message;
            StartCoroutine(FadeTextRoutine(tmp, fadeInDuration, displayDuration, fadeOutDuration));
        }
    }

    private IEnumerator FadeTextRoutine(TextMeshProUGUI tmp, float fadeInDuration, float displayDuration, float fadeOutDuration)
    {
        // Fade in
        float timer = 0f;
        Color initialColor = tmp.color;
        initialColor.a = 0;
        tmp.color = initialColor;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeInDuration);
            tmp.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Full display
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeOutDuration);
            tmp.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // Destroy the text object after fading out
        Destroy(tmp.gameObject);
    }
}
