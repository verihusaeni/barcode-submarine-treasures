
using System.Collections;
using UnityEngine;

public class LogoFade : MonoBehaviour
{
    public CanvasGroup logo;
    public float fadeInDuration = 1f;
    public float stayDuration = 2f;
    public float fadeOutDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        logo.alpha = 0f;

        // Fade In
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            logo.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }

        yield return new WaitForSeconds(stayDuration);

        // Fade Out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            logo.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
    }
}
