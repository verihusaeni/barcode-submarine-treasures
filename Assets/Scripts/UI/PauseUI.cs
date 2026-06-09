using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Plane.Gameplay; // <--- TAMBAHKAN BARIS INI AGAR PAUSEUI BISA MEMBACA GAMECONTROL

namespace Plane.UI
{
    public class PauseUI : MonoBehaviour
    {
        [Tooltip("Masukkan nama scene tujuan untuk tombol Exit (misal: MainMenu)")]
        public string m_ExitSceneName = "MainMenu";

        public void BtnResume()
        {
            GameControl.m_Current.ResumeGame();
        }

        public void BtnExit()
        {
            // WAJIB kembalikan timeScale ke 1 sebelum pindah scene, 
            // jika tidak, scene selanjutnya akan terkunci/beku!
            Time.timeScale = 1f; 
            
            if (!string.IsNullOrEmpty(m_ExitSceneName))
            {
                SceneManager.LoadScene(m_ExitSceneName);
            }
        }
    }
}