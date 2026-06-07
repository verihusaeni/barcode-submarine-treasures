using UnityEngine;

public class UIFloat : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 10f;   // tinggi gerakan naik turun
    public float frequency = 1f;    // kecepatan gerak

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}