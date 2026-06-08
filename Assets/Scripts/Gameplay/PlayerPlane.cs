using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    public class PlayerPlane : MonoBehaviour
    {
        public Vector2 m_Angle = Vector2.zero;
        public Transform m_Base;
        Vector2 m_TurnSpeed = Vector2.zero;

        public GameObject m_ExplodeParticle;

        public static PlayerPlane m_Main;

        [Header("Touch Settings")]
        [Tooltip("Area layar yang diabaikan di tepi bawah (untuk UI buttons)")]
        public float m_DeadZoneBottom = 100f;
        [Tooltip("Jarak minimal drag agar input dihitung")]
        public float m_DragThreshold = 10f;
        [Tooltip("Kecepatan respons terhadap sentuhan (semakin kecil = semakin halus)")]
        public float m_TouchSensitivity = 1f;
        [Tooltip("Kecepatan kembali ke posisi netral saat tidak disentuh")]
        public float m_ReturnSpeed = 5f;

        // Variabel touch
        private int m_TrackedFingerId = -1;
        private Vector2 m_TouchStartPos;
        private Vector2 m_TouchCurrentPos;
        private bool m_IsTouching = false;
        private Vector2 m_TouchInput = Vector2.zero;

        private void Awake()
        {
            m_Main = this;
        }

        void Update()
        {
            float InputX = 0, InputY = 0;

            // =====================
            // TOUCH INPUT
            // =====================
            HandleTouchInput();

            // =====================
            // KEYBOARD INPUT (fallback)
            // =====================
            if (Input.GetKey(KeyCode.A))
                InputX = -1;
            else if (Input.GetKey(KeyCode.D))
                InputX = 1;

            if (Input.GetKey(KeyCode.W))
                InputY = 1;
            else if (Input.GetKey(KeyCode.S))
                InputY = -1;

            // Gabungkan: prioritas touch, jika tidak ada sentuhan pakai keyboard
            if (m_IsTouching && m_TouchInput.magnitude > 0.01f)
            {
                InputX = m_TouchInput.x;
                InputY = m_TouchInput.y;
            }

            // =====================
            // MOVEMENT
            // =====================
            Vector3 movement = 40 * Time.deltaTime * new Vector3(InputX, InputY, 0);

            m_Angle.x = Mathf.Lerp(m_Angle.x, 60.0f * InputX, 5 * Time.deltaTime);
            m_Angle.y = Mathf.Lerp(m_Angle.y, 20.0f * InputY, 5 * Time.deltaTime);

            m_Base.localRotation = Quaternion.Euler(-1f * m_Angle.y, 0, -m_Angle.x);

            transform.position += movement;

            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, 8, 30);
            pos.x = Mathf.Clamp(pos.x, -18, 18);
            pos.z = 0;
            transform.position = pos;
        }

        // =====================
        // COLLISION (UPDATE)
        // =====================
        private void OnTriggerEnter(Collider hit)
        {
            // Cek apakah yang ditabrak adalah Obstacle
            // Mengecek Tag, atau komponen ObstaclePack di objek tersebut, atau di parent-nya (untuk prefab gabungan)
            bool isObstacle = hit.CompareTag("Obstacle") || hit.GetComponent<ObstaclePack>() != null || hit.GetComponentInParent<ObstaclePack>() != null;

            if (isObstacle)
            {
                if (m_ExplodeParticle != null)
                {
                    GameObject obj = Instantiate(m_ExplodeParticle);
                    obj.transform.position = transform.position;
                }

                GameControl.m_Current.HandleGameOver();
                gameObject.SetActive(false);
            }
        }

        private void HandleTouchInput()
        {
            // Cek apakah ada touch aktif
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    // ========================
                    // TOUCH BEGAN - mulai sentuh
                    // ========================
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Abaikan area dead zone di bawah layar (untuk tombol UI)
                        if (touch.position.y < m_DeadZoneBottom)
                            continue;

                        // Simpan finger ID yang sedang dilacak
                        if (m_TrackedFingerId == -1)
                        {
                            m_TrackedFingerId = touch.fingerId;
                            m_TouchStartPos = touch.position;
                            m_TouchCurrentPos = touch.position;
                            m_IsTouching = true;
                        }
                    }

                    // Hanya proses finger yang sedang dilacak
                    if (touch.fingerId != m_TrackedFingerId)
                        continue;

                    // ========================
                    // TOUCH MOVED - sedang drag
                    // ========================
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        m_TouchCurrentPos = touch.position;

                        // Hitung delta dari posisi awal sentuh
                        Vector2 delta = m_TouchCurrentPos - m_TouchStartPos;

                        // Abaikan jika drag terlalu kecil
                        if (delta.magnitude < m_DragThreshold)
                        {
                            m_TouchInput = Vector2.zero;
                            return;
                        }

                        // Normalisasi delta berdasarkan ukuran layar
                        float normalizedX = (delta.x / (Screen.width * 0.5f)) * m_TouchSensitivity;
                        float normalizedY = (delta.y / (Screen.height * 0.5f)) * m_TouchSensitivity;

                        // Clamp antara -1 dan 1
                        m_TouchInput = new Vector2(
                            Mathf.Clamp(normalizedX, -1f, 1f),
                            Mathf.Clamp(normalizedY, -1f, 1f)
                        );
                    }

                    // ========================
                    // TOUCH ENDED / CANCELED - jari diangkat
                    // ========================
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        if (touch.fingerId == m_TrackedFingerId)
                        {
                            ResetTouch();
                        }
                    }
                }
            }
            else
            {
                // Tidak ada sentuhan sama sekali
                ResetTouch();
            }
        }

        private void ResetTouch()
        {
            m_TrackedFingerId = -1;
            m_IsTouching = false;
            m_TouchInput = Vector2.zero;
        }
    }
}