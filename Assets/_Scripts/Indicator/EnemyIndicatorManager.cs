using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicatorManager : MonoBehaviour
{
    public static EnemyIndicatorManager instance;

    [Header("Setup")]
    public Camera mainCam;
    public RectTransform indicatorsParent;
    public GameObject indicatorPrefab;

    [Header("Settings")]
    public float edgeOffset = 50f;

    private Dictionary<Transform, IndicatorData> enemyIndi = new Dictionary<Transform, IndicatorData>();

    public EnemyAI enemy;
    public TextMeshProUGUI textCoinEnemy;
    public Image indica;
    public Image oval;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (enemyIndi.ContainsKey(enemy.transform)) return;

        GameObject ind = Instantiate(indicatorPrefab, indicatorsParent);
        ind.SetActive(true);

        EnemyIndicator indScript = ind.GetComponent<EnemyIndicator>();
        if (indScript != null)
        {
            indScript.Init(enemy.coinText);
        }

        // TẠO IndicatorData với màu mặc định
        IndicatorData indicatorData = new IndicatorData(ind.GetComponent<RectTransform>(), enemy);
        enemyIndi[enemy.transform] = indicatorData;
    }
    // HÀM CẬP NHẬT MÀU CHO ENEMY ĐÃ ĐĂNG KÝ
    public void UpdateEnemyColor(Transform enemyTransform, Color newColor)
    {
        if (enemyIndi.ContainsKey(enemyTransform))
        {
            IndicatorData data = enemyIndi[enemyTransform];
            ApplyColorToIndicator(data, newColor);
        }
        else
        {
            Debug.LogWarning($"Enemy {enemyTransform.name} not found in indicator registry");
        }
    }

    // Giữ nguyên ApplyColorToIndicator() như cũ
    private void ApplyColorToIndicator(IndicatorData data, Color enemyColor)
    {
        if (data.indica != null)
        {
            Color beforeColor = data.indica.color;
            data.indica.color = enemyColor;
            //Debug.Log($"Indica: {beforeColor} → {data.indica.color}");
        }

        if (data.oval != null)
        {
            Color beforeColor = data.oval.color;
            data.oval.color = enemyColor;
            //Debug.Log($"Oval: {beforeColor} → {data.oval.color}");
        }
    }
    public void UnregisterEnemy(Transform enemy)
    {
        if (!enemyIndi.ContainsKey(enemy)) return;
        Destroy(enemyIndi[enemy].indicatorUI.gameObject);
        enemyIndi.Remove(enemy);
    }

    private void Update()
    {
        // Cập nhật text coin từ enemy
        if (enemy != null && textCoinEnemy != null)
        {
            textCoinEnemy.text = enemy.coinText.text;
        }

        foreach (var kvp in enemyIndi)
        {
            Transform enemy = kvp.Key;
            IndicatorData arrowUI = kvp.Value;

            if (enemy == null)
            {
                Destroy(arrowUI.indicatorUI.gameObject);
                continue;
            }

            Vector3 viewportPos = mainCam.WorldToViewportPoint(enemy.position);

            // Enemy trong màn hình
            if (viewportPos.z > 0 &&
                viewportPos.x > 0 && viewportPos.x < 1 &&
                viewportPos.y > 0 && viewportPos.y < 1)
            {
                arrowUI.indicatorUI.gameObject.SetActive(false);
                continue;
            }

            arrowUI.indicatorUI.gameObject.SetActive(true);

            // vector từ tâm màn hình -> enemy
            Vector2 fromCenter = new Vector2(viewportPos.x - 0.5f, viewportPos.y - 0.5f);

            // Nếu enemy ở sau camera thì đảo hướng
            if (viewportPos.z < 0)
            {
                fromCenter = -fromCenter;
                viewportPos.x = 1f - viewportPos.x;
                viewportPos.y = 1f - viewportPos.y;
            }

            // hướng mũi tên
            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
            arrowUI.indicatorUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

            // đặt ở đúng mép màn hình
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 screenPos = new Vector2(viewportPos.x * Screen.width, viewportPos.y * Screen.height);
            Vector2 dir = (screenPos - screenCenter).normalized;

            float slope = dir.y / dir.x;
            Vector2 edgePos = screenCenter;

            if (dir.x > 0)
                edgePos.x = Screen.width - edgeOffset;
            else
                edgePos.x = edgeOffset;
            edgePos.y = screenCenter.y + slope * (edgePos.x - screenCenter.x);

            if (edgePos.y > Screen.height - edgeOffset || edgePos.y < edgeOffset)
            {
                if (dir.y > 0)
                    edgePos.y = Screen.height - edgeOffset;
                else
                    edgePos.y = edgeOffset;

                edgePos.x = screenCenter.x + (edgePos.y - screenCenter.y) / slope;
            }
            arrowUI.indicatorUI.position = edgePos;
        }
    }
}
