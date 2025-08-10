using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleAroundPlayer : MonoBehaviour
{
    public float radius = 4f;
    public int segments = 60;
    public float lineWidth = 0.05f;

    private LineRenderer line;
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.loop = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments;

        DrawCircle();
    }

    void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0.01f, z));
            angle += 2 * Mathf.PI / segments;
        }
    }
}
