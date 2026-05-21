using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeImageScript : MonoBehaviour
{
    public static FadeImageScript Instance { get; private set; }

    [SerializeField] private Image image;
    public int duration = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(FadeFromBlack());
    }

    public IEnumerator FadeFromBlack()
    {
            // Stop immediately if no Image reference is assigned.
        if (image == null)
        {
                // Exit the coroutine when image is missing.
            yield break;
        }

            // Cache the current image color so we can modify alpha over time.
        Color color = image.color;
            // Tracks elapsed fade time.
        float elapsed = 0f;

            // Fade until elapsed time reaches the duration.
        while (elapsed < duration)
        {
                // Advance elapsed time by the frame delta time.
            elapsed += Time.deltaTime;
                // Interpolate alpha from fully opaque (1) to fully transparent (0).
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
                // Apply the updated color to the image.
            image.color = color;
                // Wait until the next frame.
            yield return null;
        }

            // Ensure the image ends fully transparent.
        color.a = 0f;
            // Apply the final transparent color.
        image.color = color;
    }

    public IEnumerator FadeToBlack()
    {
            // Stop immediately if no Image reference is assigned.
        if (image == null)
        {
                // Exit the coroutine when image is missing.
            yield break;
        }

            // Cache the current image color so we can modify alpha over time.
        Color color = image.color;
            // Tracks elapsed fade time.
        float elapsed = 0f;

            // Fade until elapsed time reaches the duration.
        while (elapsed < duration)
        {
                // Advance elapsed time by the frame delta time.
            elapsed += Time.deltaTime;
                // Interpolate alpha from fully transparent (0) to fully opaque (1).
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                // Apply the updated color to the image.
            image.color = color;
                // Wait until the next frame.
            yield return null;
        }

            // Ensure the image ends fully opaque.
        color.a = 1f;
            // Apply the final opaque color.
        image.color = color;
    }
}
