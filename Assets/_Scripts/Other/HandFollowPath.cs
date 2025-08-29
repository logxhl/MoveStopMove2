using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollowPath : MonoBehaviour
{
    public RectTransform handIcon;
    public float speed = 2f;
    public float size = 100f; // kích thước vòng lặp ∞

    private float t;

    void Update()
    {
        t += Time.deltaTime * speed;

        // Công thức ∞ (Lemniscate of Gerono)
        float x = Mathf.Sin(-t) * size;
        float y = Mathf.Sin(-t) * Mathf.Cos(t) * size;

        handIcon.anchoredPosition = new Vector2(x, y);
    }
}