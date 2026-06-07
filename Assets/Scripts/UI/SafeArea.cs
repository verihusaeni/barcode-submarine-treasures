using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform Panel;

    void Start()
    {
        Panel = GetComponent<RectTransform>();
        UpdateSafeArea();
    }

    void UpdateSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
    }
}