using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float speed = 40.0f;

    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the shuriken hits an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DecreaseHitCount(); // Decrease the hit count of the enemy
            }
        }

        // Destroy the shuriken for any collision (enemy or otherwise)
        Destroy(gameObject);
    }
}