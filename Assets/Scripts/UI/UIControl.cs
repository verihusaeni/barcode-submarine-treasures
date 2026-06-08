using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Plane.UI
{
    public class UIControl : MonoBehaviour
    {
        private static UIControl m_Current;
        public static UIControl Current
        { get { return m_Current; } }

        public GameObject m_LoseUI;
        public GameObject m_WinUI;
        public GameObject m_InGameUI;

        [SerializeField]
        public Camera m_EventCamera;

        public Text m_CoinText; 

        void Awake()
        {
            m_Current = this;
        }

        void Start()
        {
            Canvas[] allCanvas = GetComponentsInChildren<Canvas>(true);
            foreach (Canvas c in allCanvas)
            {
                c.worldCamera = m_EventCamera;
            }
        }

        // Tambahkan parameter highScore
        public void UpdateLoseUI(int totalCoins, int highScore)
        {
            LoseUI loseScript = m_LoseUI.GetComponent<LoseUI>();
            if (loseScript != null)
            {
                loseScript.ShowFinalScore(totalCoins, highScore);
            }
        }
    }
}