using System.Collections;
using System.Collections.Generic;
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
    private Dictionary<Transform, RectTransform> enemyIndi = new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void RegisterEnemy(Transform enemy)
    {
        if (enemyIndi.ContainsKey(enemy)) return;
        GameObject ind = Instantiate(indicatorPrefab, indicatorsParent);
        ind.SetActive(true);
        enemyIndi[enemy] = ind.GetComponent<RectTransform>();
    }

    public void UnregisterEnemy(Transform enemy)
    {
        if(!enemyIndi.ContainsKey(enemy)) return;
        Destroy(enemyIndi[enemy].gameObject);
        enemyIndi.Remove(enemy);
    }
    private void Update()
    {
        foreach (var kvp in enemyIndi)
        {
            Transform enemy = kvp.Key;
            RectTransform arrowUI = kvp.Value;

            if (enemy == null)
            {
                Destroy(arrowUI.gameObject);
                continue;
            }

            Vector3 viewportPos = mainCam.WorldToViewportPoint(enemy.position);

            // Enemy trong màn hình
            if (viewportPos.z > 0 &&
                viewportPos.x > 0 && viewportPos.x < 1 &&
                viewportPos.y > 0 && viewportPos.y < 1)
            {
                arrowUI.gameObject.SetActive(false);
                continue;
            }

            arrowUI.gameObject.SetActive(true);

            // vector từ tâm màn hình -> enemy
            Vector2 fromCenter = new Vector2(viewportPos.x - 0.5f, viewportPos.y - 0.5f);

            // Nếu enemy ở sau camera thì đảo hướng
            if (viewportPos.z < 0)
            {
                fromCenter = -fromCenter;

                // đảo luôn screenPos
                viewportPos.x = 1f - viewportPos.x;
                viewportPos.y = 1f - viewportPos.y;
            }

            // hướng mũi tên
            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
            arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

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

            arrowUI.position = edgePos;


        }
    }


}
