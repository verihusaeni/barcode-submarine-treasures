using UnityEngine;

namespace Plane.Gameplay
{
    public class Coin : MonoBehaviour
    {
        public int coinValue = 100;

        [Header("Rotation Settings")]
        [Tooltip("Kecepatan putaran koin (derajat per detik)")]
        public float rotateSpeed = 180f; 
        
        [Tooltip("Sumbu putaran koin. (0,1,0) berarti berputar di sumbu Y atas-bawah")]
        public Vector3 rotateAxis = Vector3.up;

        [Header("Sound Settings")]
        [Tooltip("Suara yang dimainkan saat koin diambil")]
        public AudioClip coinSound; // Tambahkan variabel untuk suara

        void Update()
        {
            // Memutar koin secara terus-menerus
            transform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime, Space.World);
        }

        void OnTriggerEnter(Collider other)
        {
            // Cek apakah yang menyentuh adalah Player
            if (other.CompareTag("Player"))
            {
                // Tambah koin ke GameControl
                GameControl.m_Current.AddCoin(coinValue);

                // Mainkan sound effect sebelum dihancurkan
                if (coinSound != null)
                {
                    // Memutar suara di posisi koin. Suara akan tetap terdengar meski koin sudah dihancurkan.
                    AudioSource.PlayClipAtPoint(coinSound, transform.position);
                }

                // Hancurkan koin ini
                Destroy(gameObject);
            }
        }
    }
}