using UnityEngine;
using UnityEngine.UI;

public class SubmarineSlider : MonoBehaviour
{
    [Header("UI")]
    public RectTransform submarineIcon;

    [Header("Progress")]
    [Range(0f, 1f)] public float progress = 0f; // Menambahkan slider di Inspector agar mudah ditest

    [Header("Movement Area")]
    public RectTransform startPoint;
    public RectTransform endPoint;

    void Update()
    {
        MoveSubmarine();
    }

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);
    }

    void MoveSubmarine()
    {
        // Menggunakan anchoredPosition agar pergerakan UI akurat di dalam Canvas
        if (submarineIcon != null && startPoint != null && endPoint != null)
        {
            submarineIcon.anchoredPosition = Vector2.Lerp(
                startPoint.anchoredPosition,
                endPoint.anchoredPosition,
                progress
            );
        }
    }
}