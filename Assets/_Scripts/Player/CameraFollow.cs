using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;   // ✅ để dễ gọi từ PlayerController

    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -10);
    public float smoothSpeed = 5f;

    [Header("Obstacle Fade Settings")]
    public LayerMask obstacleMask;     // Layer tòa nhà/vật cản
    public float sphereRadius = 0.3f;  // bán kính sphere cast
    public float checkInterval = 0.1f; // tần suất check
    [Range(0f, 1f)] public float fadeAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private float _nextCheckTime;
    private readonly Dictionary<Renderer, float> _currentAlpha = new();
    private readonly Dictionary<Renderer, float> _targetAlpha = new();
    private readonly List<Renderer> _hitsThisFrame = new();

    private MaterialPropertyBlock _mpb;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private Vector3 defaultOffset;
    private float defaultScale = 1f;

    private void Awake()
    {
        instance = this;   // ✅ set instance
        _mpb = new MaterialPropertyBlock();
        if (obstacleMask.value == 0)
            obstacleMask = LayerMask.GetMask("Obstruction");
    }

    private void Start()
    {
        defaultOffset = offset;
        if (target != null)
        {
            defaultScale = target.localScale.x;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ===== Dynamic offset theo scale của target =====
        float scaleFactor = target.localScale.x / defaultScale;
        Vector3 dynamicOffset = offset * scaleFactor;

        // ===== Follow target =====
        Vector3 desiredPosition = target.position + dynamicOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);

        // ===== Obstacle fade =====
        if (Time.time >= _nextCheckTime)
        {
            DoOcclusionCheck();
            _nextCheckTime = Time.time + checkInterval;
        }
        UpdateAlphas();
    }

    private void DoOcclusionCheck()
    {
        _hitsThisFrame.Clear();

        Vector3 camPos = transform.position;
        Vector3 dir = target.position - camPos;
        float dist = dir.magnitude;
        if (dist < 0.01f) return;
        dir /= dist;

        var hits = Physics.SphereCastAll(camPos, sphereRadius, dir, dist, obstacleMask, QueryTriggerInteraction.Ignore);

        foreach (var h in hits)
        {
            foreach (var r in h.collider.GetComponentsInChildren<Renderer>())
            {
                _hitsThisFrame.Add(r);
                _targetAlpha[r] = fadeAlpha;
                if (!_currentAlpha.ContainsKey(r)) _currentAlpha[r] = 1f;
            }
        }

        // Renderer không còn bị che → phục hồi
        foreach (var r in _currentAlpha.Keys)
        {
            if (!_hitsThisFrame.Contains(r))
                _targetAlpha[r] = 1f;
        }
    }

    private void UpdateAlphas()
    {
        var keys = new List<Renderer>(_targetAlpha.Keys);
        foreach (var r in keys)
        {
            float cur = _currentAlpha.TryGetValue(r, out var a) ? a : 1f;
            float next = Mathf.MoveTowards(cur, _targetAlpha[r], fadeSpeed * Time.deltaTime);
            _currentAlpha[r] = next;

            ApplyAlpha(r, next);

            // Xóa khỏi danh sách khi đã trở lại trong suốt
            if (Mathf.Approximately(next, 1f) && !_hitsThisFrame.Contains(r))
            {
                _targetAlpha.Remove(r);
            }
        }
    }

    private void ApplyAlpha(Renderer r, float a)
    {
        if (r == null) return;
        r.GetPropertyBlock(_mpb);

        Color baseCol = r.sharedMaterial.HasProperty(BaseColor)
            ? r.sharedMaterial.GetColor(BaseColor)
            : Color.white;

        baseCol.a = a;
        _mpb.SetColor(BaseColor, baseCol);
        r.SetPropertyBlock(_mpb);
    }

    public void ShiftUp(float height)
    {
        offset = defaultOffset + new Vector3(0, height, -3);
    }

    public void ResetOffset()
    {
        offset = defaultOffset;
    }
}
