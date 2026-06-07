using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}