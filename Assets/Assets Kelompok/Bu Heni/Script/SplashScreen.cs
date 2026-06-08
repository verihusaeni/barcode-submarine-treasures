using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public float spashDuration = 3f; // Durasi splash screen dalam detik
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      Invoke("LoadMenu", spashDuration); // Memanggil fungsi LoadMenu setelah durasi splash screen selesai  
    }
    void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Ganti "Menu" dengan nama scene menu Anda
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
