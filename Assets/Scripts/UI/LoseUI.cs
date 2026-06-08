using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Plane.UI
{
    public class LoseUI : MonoBehaviour
    {
        public Text m_FinalCoinText; 
        public Text m_HighScoreText; 

        [Header("Scene Settings")]
        [Tooltip("Masukkan nama scene tujuan untuk tombol Exit (misal: MainMenu)")]
        public string m_ExitSceneName = "MainMenu"; // Bisa diubah langsung di Inspector

        public void BtnRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void BtnExit()
        {
            // Cek apakah nama scene sudah diisi di inspector
            if (!string.IsNullOrEmpty(m_ExitSceneName))
            {
                SceneManager.LoadScene(m_ExitSceneName);
            }
            else
            {
                Debug.LogWarning("Nama Scene untuk tombol Exit belum diatur di LoseUI Inspector!");
            }
        }

        public void ShowFinalScore(int coins, int highScore)
        {
            if (m_FinalCoinText != null)
            {
                m_FinalCoinText.text = "Score : " + coins.ToString();
            }

            if (m_HighScoreText != null)
            {
                m_HighScoreText.text = "High Score : " + highScore.ToString();
            }
        }
    }
}