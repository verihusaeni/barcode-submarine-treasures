using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plane.UI;
using UnityEngine.UI;

namespace Plane.Gameplay
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl m_Current;

        [HideInInspector]
        public int m_GameState = 0;
        public const int State_Start = 0;
        public const int State_Chase = 1;
        public const int State_Shoot = 2;
        public const int State_Win = 3;
        public const int State_Lose = 4;
        
        [HideInInspector]
        public float State_Timer = 0;

        public Transform m_SpeedParticle;
        
        [HideInInspector]
        public float m_GameSpeed = 0; 

        [HideInInspector]
        public int m_CoinCount = 0;

        [Header("BGM Settings")]
        public AudioSource m_BGMSource; 
        public float m_BGMFadeInDuration = 2f;
        public float m_BGMMaxVolume = 0.5f;

        [Header("Game Settings")]
        public float m_NormalGameSpeed = 100f;

        [Header("Countdown Sound Settings")]
        [Range(0f, 1f)] 
        public float m_CountdownVolume = 1f;
        public AudioClip m_Count3Sound;
        public AudioClip m_Count2Sound;
        public AudioClip m_Count1Sound;
        public AudioClip m_CountGoSound;

        void Awake()
        {
            m_Current = this;
        }
        
        void Start()
        {
            m_GameState = State_Start;
            m_CoinCount = 0; 
            UpdateCoinUI();

            StartCoroutine(StartCountdown());
        }

        void Update()
        {
            State_Timer += Time.deltaTime;
        }

        private IEnumerator StartCountdown()
        {
            m_GameSpeed = 0; 
            
            if (UIControl.Current != null && UIControl.Current.m_CountdownText != null)
            {
                UIControl.Current.m_CountdownText.gameObject.SetActive(true);

                if (m_Count3Sound != null) AudioSource.PlayClipAtPoint(m_Count3Sound, Vector3.zero, m_CountdownVolume);
                UIControl.Current.m_CountdownText.text = "3";
                yield return new WaitForSeconds(1f);

                if (m_Count2Sound != null) AudioSource.PlayClipAtPoint(m_Count2Sound, Vector3.zero, m_CountdownVolume);
                UIControl.Current.m_CountdownText.text = "2";
                yield return new WaitForSeconds(1f);

                if (m_Count1Sound != null) AudioSource.PlayClipAtPoint(m_Count1Sound, Vector3.zero, m_CountdownVolume);
                UIControl.Current.m_CountdownText.text = "1";
                yield return new WaitForSeconds(1f);

                if (m_CountGoSound != null) AudioSource.PlayClipAtPoint(m_CountGoSound, Vector3.zero, m_CountdownVolume);
                UIControl.Current.m_CountdownText.text = "GO!";
                yield return new WaitForSeconds(1f);

                UIControl.Current.m_CountdownText.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(4f);
            }

            m_GameSpeed = m_NormalGameSpeed; 
            m_GameState = State_Chase; 

            if (m_BGMSource != null)
            {
                StartCoroutine(FadeInBGM());
            }
        }

        public void AddCoin(int amount)
        {
            m_CoinCount += amount;
            UpdateCoinUI();
        }

        public void UpdateCoinUI()
        {
            if (UIControl.Current != null && UIControl.Current.m_CoinText != null)
            {
                UIControl.Current.m_CoinText.text = m_CoinCount.ToString();
            }
        }

        // === FUNGSI PAUSE & RESUME (UPDATE) ===
        public void PauseGame()
        {
            // Hanya bisa pause jika sedang bermain (State_Chase)
            if (m_GameState != State_Chase) return;

            Time.timeScale = 0f; // Bekukan waktu game
            
            if (m_BGMSource != null && m_BGMSource.isPlaying)
            {
                m_BGMSource.Pause();
            }

            if (UIControl.Current != null)
            {
                UIControl.Current.m_PauseButton.SetActive(false);
                UIControl.Current.m_PausePanel.SetActive(true);
            }
        }

        public void ResumeGame()
        {
            // Jangan langsung resume, tapi mulai hitungan mundur dulu
            StartCoroutine(ResumeCountdown());
        }

        private IEnumerator ResumeCountdown()
        {
            // Sembunyikan panel pause terlebih dahulu
            if (UIControl.Current != null)
            {
                UIControl.Current.m_PausePanel.SetActive(false);
                UIControl.Current.m_CountdownText.gameObject.SetActive(true);
            }

            // PENTING: Gunakan WaitForSecondsRealtime karena Time.timeScale saat ini = 0
            // Jika pakai WaitForSeconds biasa, hitungan mundur tidak akan jalan!

            if (m_Count3Sound != null) AudioSource.PlayClipAtPoint(m_Count3Sound, Vector3.zero, m_CountdownVolume);
            UIControl.Current.m_CountdownText.text = "3";
            yield return new WaitForSecondsRealtime(1f);

            if (m_Count2Sound != null) AudioSource.PlayClipAtPoint(m_Count2Sound, Vector3.zero, m_CountdownVolume);
            UIControl.Current.m_CountdownText.text = "2";
            yield return new WaitForSecondsRealtime(1f);

            if (m_Count1Sound != null) AudioSource.PlayClipAtPoint(m_Count1Sound, Vector3.zero, m_CountdownVolume);
            UIControl.Current.m_CountdownText.text = "1";
            yield return new WaitForSecondsRealtime(1f);

            if (m_CountGoSound != null) AudioSource.PlayClipAtPoint(m_CountGoSound, Vector3.zero, m_CountdownVolume);
            UIControl.Current.m_CountdownText.text = "GO!";
            yield return new WaitForSecondsRealtime(1f);

            // Setelah hitungan mundur selesai, baru kembalikan semuanya
            if (UIControl.Current != null)
            {
                UIControl.Current.m_CountdownText.gameObject.SetActive(false);
                UIControl.Current.m_PauseButton.SetActive(true); // Tampilkan tombol pause lagi
            }

            Time.timeScale = 1f; // Cairkan waktu game

            // Lanjutkan BGM
            if (m_BGMSource != null)
            {
                m_BGMSource.UnPause();
            }
        }
        // ======================================

        public void HandleGameOver()
        {
            m_GameSpeed = 0; 
            m_SpeedParticle.gameObject.SetActive(false);
            CameraControl.Current.m_ShakeEnabled = false;
            
            if (m_BGMSource != null)
            {
                m_BGMSource.Stop(); 
            }

            int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (m_CoinCount > currentHighScore)
            {
                currentHighScore = m_CoinCount;
                PlayerPrefs.SetInt("HighScore", currentHighScore);
                PlayerPrefs.Save();
            }

            if (UIControl.Current != null)
            {
                UIControl.Current.m_PauseButton.SetActive(false);
            }

            UIControl.Current.m_InGameUI.SetActive(false);
            UIControl.Current.UpdateLoseUI(m_CoinCount, currentHighScore);
            UIControl.Current.m_LoseUI.SetActive(true);
        }

        public void HandleWin()
        { 

        }

        private IEnumerator FadeInBGM()
        {
            m_BGMSource.volume = 0f;
            m_BGMSource.Play();

            float timer = 0f;

            while (timer < m_BGMFadeInDuration)
            {
                timer += Time.deltaTime;
                m_BGMSource.volume = Mathf.Lerp(0f, m_BGMMaxVolume, timer / m_BGMFadeInDuration);
                yield return null;
            }

            m_BGMSource.volume = m_BGMMaxVolume;
        }
    }
}