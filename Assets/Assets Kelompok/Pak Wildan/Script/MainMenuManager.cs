using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Exit Panel")]
    public GameObject exitPanel;

    void Start()
    {
        exitPanel.SetActive(false);
    }

    // dipanggil dari ButtonExit
    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);
    }

    // tombol NO
    public void CancelExit()
    {
        exitPanel.SetActive(false);
    }

    // tombol YES
    public void ExitGame()
    {
        Debug.Log("Keluar Game");

        Application.Quit();

        // untuk editor unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}