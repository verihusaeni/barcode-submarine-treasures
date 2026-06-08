using UnityEngine;

namespace Plane.Gameplay
{
    public class ItemPack : MonoBehaviour
    {
        void Update()
        {
            // Bergerak mundur mengikuti kecepatan game
            transform.position += GameControl.m_Current.m_GameSpeed * Time.deltaTime * Vector3.back;

            // Hancurkan jika sudah jauh di belakang
            if (transform.position.z < -50)
                Destroy(gameObject);
        }
    }
}