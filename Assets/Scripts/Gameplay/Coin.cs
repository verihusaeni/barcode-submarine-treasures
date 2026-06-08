using UnityEngine;

namespace Plane.Gameplay
{
    public class Coin : MonoBehaviour
    {
        // Nilai per koin
        public int coinValue = 100;

        void OnTriggerEnter(Collider other)
        {
            // Cek apakah yang menyentuh adalah Player
            if (other.CompareTag("Player"))
            {
                // Tambah koin ke GameControl
                GameControl.m_Current.AddCoin(coinValue);
                
                // Hancurkan koin ini
                Destroy(gameObject);
            }
        }
    }
}