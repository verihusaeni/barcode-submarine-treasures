using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashLoader : MonoBehaviour
{
    [Header("UI")]
    public Slider loadingSlider;

    [Header("Loading Time")]
    public float loadTime = 3f;

    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        float timer = 0f;

        while (timer < loadTime)
        {
            timer += Time.deltaTime;

            float progress = timer / loadTime;
            loadingSlider.value = progress;

            yield return null;
        }

        // pindah ke MainMenu
        SceneManager.LoadScene("MainMenu");
    }
}