using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hitCount;               // Number of hits required to destroy the enemy
    public float moveDistance = 5f;    // Distance to move back and forth
    public float moveSpeed = 2f;       // Speed of movement
    public GameObject bulletPrefab;    // Bullet prefab to instantiate
    public Transform firePoint;        // Reference to the fire point for bullets
    public GameObject keyPrefab;       // Reference to the key prefab to spawn
    public float yOffset = 1f;         // The vertical offset to spawn the key above the ground

    private Vector3 startPosition;
    private bool isGameOver = false;    // Tracks if the game is over
    private bool movingRight = true;    // Tracks the current direction of movement

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        // Set hit count based on the enemy type
        if (gameObject.name == "Enemy_Blob_S")
        {
            hitCount = 2; // Set hit count for Enemy_Blob_S
        }
        else if (gameObject.name == "Enemy_Blob_M")
        {
            hitCount = 4; // Set hit count for Enemy_Blob_M
        }
        else if (gameObject.name == "Enemy_Blob_Boss")
        {
            hitCount = 8; // Set hit count for Enemy_Blob_Boss
            StartCoroutine(FireBullets()); // Start the firing sequence for the Boss
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        MoveEnemy();
    }

    private void MoveEnemy()
    {
        float targetX = startPosition.x + (movingRight ? moveDistance : -moveDistance);
        float step = moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), step);

        if (Mathf.Approximately(transform.position.x, targetX))
        {
            movingRight = !movingRight;
        }
    }

    // Method to decrease the hit count
    public void DecreaseHitCount()
    {
        hitCount--;

        // Destroy the enemy if the hit count reaches 0
        if (hitCount <= 0)
        {
            SpawnKey();  // Spawn key when enemy is destroyed
            Destroy(gameObject);
        }
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
    }

    // Coroutine to fire bullets with intervals
    private IEnumerator FireBullets()
    {
        while (true)
        {
            // Fire 5 bullets with 1-second interval
            for (int i = 0; i < 5; i++)
            {
                FireBullet();
                yield return new WaitForSeconds(1f);
            }

            // Wait for 3 seconds before firing again
            yield return new WaitForSeconds(3f);
        }
    }

    // Method to instantiate and fire a bullet from the fire point with a fixed rotation
    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Create the bullet at the fire point's position, but with a fixed rotation (x=0, y=0, z=90)
            Quaternion bulletRotation = Quaternion.Euler(0f, 0f, 90f); // Set the fixed rotation to (x=0, y=0, z=90)
            Instantiate(bulletPrefab, firePoint.position, bulletRotation); // Instantiate with fixed rotation
        }
    }

    // Method to spawn the key with an offset above the ground
    private void SpawnKey()
    {
        if (keyPrefab != null)
        {
            // Adjust the spawn position by adding the yOffset to make it appear above the ground
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);

            // Instantiate the key at the adjusted position
            Instantiate(keyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
