using UnityEngine;
using UnityEngine.UI; // Ditambahkan jika nanti ingin memanipulasi komponen UI lainnya

public class Infomanager : MonoBehaviour
{
    [Header("Panel Info")]
    public GameObject infoPanel;

    void Start()
    {
        // Memastikan panel info tertutup saat game pertama kali dimulai
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    // Dipanggil saat tombol INFO ditekan
    public void ShowInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    // Dipanggil saat tombol BACK / CLOSE ditekan
    public void BackPressed()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}