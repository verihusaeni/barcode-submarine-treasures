using UnityEngine;

public class PlayerControllerAdventure : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0f, v).normalized * speed;
        Vector3 velocity = move;
        velocity.y = rb.linearVelocity.y; // pertahankan kecepatan vertical (gravity)
        rb.linearVelocity = velocity;
    }
}
