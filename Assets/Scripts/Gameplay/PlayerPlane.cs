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

        [Header("Touch & Mouse Settings")]
        [Tooltip("Area layar yang diabaikan di tepi bawah (untuk UI buttons)")]
        public float m_DeadZoneBottom = 100f;
        [Tooltip("Jarak minimal drag agar input dihitung")]
        public float m_DragThreshold = 10f;
        [Tooltip("Kecepatan respons terhadap sentuhan/drag (semakin kecil = semakin halus)")]
        public float m_TouchSensitivity = 1f;
        [Tooltip("Kecepatan kembali ke posisi netral saat tidak disentuh")]
        public float m_ReturnSpeed = 5f;

        // Variabel touch
        private int m_TrackedFingerId = -1;
        private Vector2 m_TouchStartPos;
        private Vector2 m_TouchCurrentPos;
        private bool m_IsTouching = false;
        private Vector2 m_TouchInput = Vector2.zero;

        // Variabel mouse (TAMBAHAN)
        private bool m_IsMouseDown = false;
        private Vector2 m_MouseStartPos;
        private Vector2 m_MouseCurrentPos;
        private Vector2 m_MouseInput = Vector2.zero;

        private void Awake()
        {
            m_Main = this;
        }

        void Update()
        {
            float InputX = 0, InputY = 0;

            // =====================
            // INPUT HANDLING
            // =====================
            HandleTouchInput();
            HandleMouseInput(); // Panggil fungsi mouse

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

            // =====================
            // PRIORITAS INPUT: Touch > Mouse > Keyboard
            // =====================
            if (m_IsTouching && m_TouchInput.magnitude > 0.01f)
            {
                InputX = m_TouchInput.x;
                InputY = m_TouchInput.y;
            }
            else if (m_IsMouseDown && m_MouseInput.magnitude > 0.01f)
            {
                InputX = m_MouseInput.x;
                InputY = m_MouseInput.y;
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
        // COLLISION
        // =====================
        private void OnTriggerEnter(Collider hit)
        {
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

        // =====================
        // TOUCH INPUT LOGIC
        // =====================
        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        if (touch.position.y < m_DeadZoneBottom)
                            continue;

                        if (m_TrackedFingerId == -1)
                        {
                            m_TrackedFingerId = touch.fingerId;
                            m_TouchStartPos = touch.position;
                            m_TouchCurrentPos = touch.position;
                            m_IsTouching = true;
                        }
                    }

                    if (touch.fingerId != m_TrackedFingerId)
                        continue;

                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        m_TouchCurrentPos = touch.position;
                        Vector2 delta = m_TouchCurrentPos - m_TouchStartPos;

                        if (delta.magnitude < m_DragThreshold)
                        {
                            m_TouchInput = Vector2.zero;
                            return;
                        }

                        float normalizedX = (delta.x / (Screen.width * 0.5f)) * m_TouchSensitivity;
                        float normalizedY = (delta.y / (Screen.height * 0.5f)) * m_TouchSensitivity;

                        m_TouchInput = new Vector2(
                            Mathf.Clamp(normalizedX, -1f, 1f),
                            Mathf.Clamp(normalizedY, -1f, 1f)
                        );
                    }

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
                ResetTouch();
            }
        }

        private void ResetTouch()
        {
            m_TrackedFingerId = -1;
            m_IsTouching = false;
            m_TouchInput = Vector2.zero;
        }

        // =====================
        // MOUSE INPUT LOGIC (TAMBAHAN)
        // =====================
        private void HandleMouseInput()
        {
            // Jika sedang ada sentuhan di layar (mobile), abaikan mouse agar tidak dobel input
            if (Input.touchCount > 0)
            {
                m_IsMouseDown = false;
                m_MouseInput = Vector2.zero;
                return;
            }

            // Saat klik kiri ditekan (Mouse Down)
            if (Input.GetMouseButtonDown(0))
            {
                // Cek dead zone bawah untuk UI
                if (Input.mousePosition.y < m_DeadZoneBottom)
                    return;

                m_IsMouseDown = true;
                m_MouseStartPos = Input.mousePosition;
                m_MouseCurrentPos = Input.mousePosition;
            }

            // Saat klik kiri ditahan dan digeser (Mouse Drag)
            if (Input.GetMouseButton(0) && m_IsMouseDown)
            {
                m_MouseCurrentPos = Input.mousePosition;
                Vector2 delta = m_MouseCurrentPos - m_MouseStartPos;

                if (delta.magnitude < m_DragThreshold)
                {
                    m_MouseInput = Vector2.zero;
                    return;
                }

                float normalizedX = (delta.x / (Screen.width * 0.5f)) * m_TouchSensitivity;
                float normalizedY = (delta.y / (Screen.height * 0.5f)) * m_TouchSensitivity;

                m_MouseInput = new Vector2(
                    Mathf.Clamp(normalizedX, -1f, 1f),
                    Mathf.Clamp(normalizedY, -1f, 1f)
                );
            }

            // Saat klik kiri dilepas (Mouse Up)
            if (Input.GetMouseButtonUp(0))
            {
                m_IsMouseDown = false;
                m_MouseInput = Vector2.zero;
            }
        }
    }
}