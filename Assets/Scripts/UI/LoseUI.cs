using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Plane.UI
{
    public class LoseUI : MonoBehaviour
    {
        public Text m_FinalCoinText; // Pastikan ini diisi di Inspector!
        public Text m_HighScoreText; // Pastikan ini juga diisi di Inspector!

        public void BtnRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        public void BtnExit()
        {
            Application.Quit();
        }

        public void ShowFinalScore(int coins, int highScore)
        {
            if (m_FinalCoinText != null)
            {
                // Ditambahkan label "Score : "
                m_FinalCoinText.text = "Score : " + coins.ToString();
            }

            if (m_HighScoreText != null)
            {
                m_HighScoreText.text = "High Score : " + highScore.ToString();
            }
        }
    }
}