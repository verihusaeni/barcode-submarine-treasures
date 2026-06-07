using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float delay = 3f;   // waktu tampil splash

    void Start()
    {
        Invoke("GoToMainMenu", delay);
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}