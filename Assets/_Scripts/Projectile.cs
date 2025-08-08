using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    public LayerMask enemyLayer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Debug.Log("Bua va cham enemy: " + other.name);
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
