using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public float speed = 40.0f;

    // Update is called once per frame
    void Update()
    {
        // Move the bullet to the left relative to the object's local axes
        transform.Translate(transform.right * Time.deltaTime * speed);
    }

    // Collision detection with other 3D objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify the PlayerController about the hit
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.OnBulletHit(); // Call the player's method to handle the bullet hit
            }
        }

        // Destroy the bullet in all cases after collision
        Destroy(gameObject);
    }
}