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
        public float m_GameSpeed = 100;

        [HideInInspector]
        public int m_CoinCount = 0;

        void Awake()
        {
            m_Current = this;
        }
        
        void Start()
        {
            m_GameState = State_Start;
            m_CoinCount = 0; 
            UpdateCoinUI();
        }

        void Update()
        {
            State_Timer += Time.deltaTime;
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

        public void HandleGameOver()
        {
            m_GameSpeed = 0;
            m_SpeedParticle.gameObject.SetActive(false);
            CameraControl.Current.m_ShakeEnabled = false;
            
            // === CEK DAN SIMPAN HIGH SCORE ===
            int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (m_CoinCount > currentHighScore)
            {
                currentHighScore = m_CoinCount;
                PlayerPrefs.SetInt("HighScore", currentHighScore);
                PlayerPrefs.Save(); // Pastikan tersimpan
            }
            // =================================

            UIControl.Current.m_InGameUI.SetActive(false);
            
            // Kirim skor akhir dan high score ke UI
            UIControl.Current.UpdateLoseUI(m_CoinCount, currentHighScore);
            
            UIControl.Current.m_LoseUI.SetActive(true);
        }

        public void HandleWin()
        { 

        }
    }
}