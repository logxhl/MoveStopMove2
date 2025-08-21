using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicatorManager : MonoBehaviour
{
    [Header("Setup")]
    public Camera mainCam;
    public RectTransform indicatorsParent;
    public GameObject indicatorPrefab;

    [Header("Settings")]
    public float edgeOffset = 50f;
    private Dictionary<Transform, RectTransform> enemyIndi = new Dictionary<Transform, RectTransform>();

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
        foreach(var kvp in enemyIndi)
        {
            Transform enemy = kvp.Key;
            RectTransform arrowUI = kvp.Value;
            if(enemy == null)
            {
                Destroy(arrowUI.gameObject);
                continue;
            }
            Vector3 screenPos = mainCam.WorldToScreenPoint(enemy.position);
            //Enemy nam trong man hinh
            if(screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height)
            {
                arrowUI.gameObject.SetActive(false);
            }
            else
            {
                arrowUI.gameObject.SetActive(true);
                //Huong tu cam -> enemy
                Vector3 dir = (enemy.position - mainCam.transform.position).normalized;
                dir.y = 0;
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                arrowUI.rotation = Quaternion.Euler(0, 0, -angle);

                //Dat o ria man hinh
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Vector3 fromCenter = (new Vector3(screenPos.x, screenPos.y, 0) - screenCenter).normalized;
                Vector3 arrowPos = screenCenter + fromCenter * ((Screen.height / 2) - edgeOffset);
                arrowPos.x = Mathf.Clamp(arrowPos.x, edgeOffset, Screen.width - edgeOffset);
                arrowPos.y = Mathf.Clamp(arrowPos.y, edgeOffset, Screen.height - edgeOffset);

                arrowUI.position = arrowPos;
            }
        }
    }
}
