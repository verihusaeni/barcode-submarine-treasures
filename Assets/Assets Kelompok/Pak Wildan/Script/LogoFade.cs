using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogoFade : MonoBehaviour
{
    public Image logo;
    public float fadeDuration = 2f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        // mulai transparan
        logo.canvasRenderer.SetAlpha(0f);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = t / fadeDuration;
            logo.canvasRenderer.SetAlpha(alpha);

            yield return null;
        }

        logo.canvasRenderer.SetAlpha(1f);
    }
}