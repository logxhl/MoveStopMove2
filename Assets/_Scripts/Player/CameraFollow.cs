using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    [Header("Obstacle Fade Settings")]
    public LayerMask obstacleMask;
    public float fadeAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private List<Renderer> fadeObjs = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            transform.LookAt(target);
        }
        HandleObstacles();
    }

    private void HandleObstacles()
    {
        // Khôi phục alpha cũ
        foreach (var rend in fadeObjs)
        {
            if (rend != null && originalColors.ContainsKey(rend))
            {
                Color c = rend.material.color;
                c.a = Mathf.MoveTowards(c.a, originalColors[rend].a, Time.deltaTime * fadeSpeed);
                rend.material.color = c;
            }
        }
        fadeObjs.Clear();

        // Raycast kiểm tra vật cản
        Vector3 dir = target.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, dir.magnitude, obstacleMask);

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                // Lưu màu gốc
                if (!originalColors.ContainsKey(rend))
                    originalColors[rend] = rend.material.color;

                // Chuyển sang transparent (URP Simple Lit dùng _Surface = 1)
                rend.material.SetFloat("_Surface", 1);
                rend.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                // Giảm alpha
                Color c = rend.material.color;
                c.a = Mathf.MoveTowards(c.a, fadeAlpha, Time.deltaTime * fadeSpeed);
                rend.material.color = c;

                fadeObjs.Add(rend);
            }
        }
    }
}
