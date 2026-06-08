using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashController : MonoBehaviour
{
    [Header("UI")]
    public Image logo;
    public Slider loadingSlider;

    [Header("Timing")]
    public float fadeDuration = 1.5f;
    public float loadTime = 3f;

    void Start()
    {
        StartCoroutine(SplashRoutine());
    }

    IEnumerator SplashRoutine()
    {
        // awal transparan
        logo.canvasRenderer.SetAlpha(0f);

        // =====================
        // FADE IN LOGO
        // =====================
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
            logo.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        logo.canvasRenderer.SetAlpha(1f);

        // =====================
        // LOADING BAR
        // =====================
        float timer = 0f;

        while (timer < loadTime)
        {
            timer += Time.deltaTime;

            float progress = timer / loadTime;
            loadingSlider.value = progress;

            yield return null;
        }

        // =====================
        // FADE OUT LOGO
        // =====================
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / fadeDuration);
            logo.canvasRenderer.SetAlpha(alpha);
            yield return null;
        }

        logo.canvasRenderer.SetAlpha(0f);

        // =====================
        // GO TO MAIN MENU
        // =====================
        SceneManager.LoadScene("MainMenu");
    }
}