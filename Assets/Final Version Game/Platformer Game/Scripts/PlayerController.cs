using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject nearbyKey;    // Reference to the nearby key object
    private bool isKeyNearby = false; // Track if the player is near the key
    private bool hasKey = false;     // Track if the player has grabbed the key

    public TextMeshProUGUI keyMessage;   // Reference to the TextMeshPro object
    public TextMeshProUGUI flagMessage;  // Reference to the TextMeshPro object for displaying messages when reaching the flag
    public TextMeshProUGUI bulletHitMessage; // Reference to the TextMeshPro object for displaying bullet hit messages

    private float speed = 4f;         // Adjust speed as needed
    private float jumpForce = 4f;     // Initial jump force
    private int jumpCount = 0;        // Track the number of jumps

    public GameManager gameManager;   // Reference to GameManager script
    public GameObject finishMenuPanel; // Reference to the finish menu panel

    // Add public variables for the sound effects
    public AudioClip jumpSound;       // Sound for jumping
    public AudioClip pickupSound;     // General pickup sound (for all items)
    private AudioSource audioSource;  // Reference to the AudioSource component

    // Weapon variables
    public GameObject weaponPrefab;   // Prefab of the shuriken
    public Transform spawnPoint;      // Point from where shurikens will be shot
    private int shurikenCount = 0;    // Counter for shurikens shot
    private float cooldownTime = 5f;  // Cooldown time to reset shuriken functionality
    private bool canShoot = true;     // Flag to control shooting availability
    private float lastShootTime = 0f; // Track time of last shuriken shot
    private bool isFacingRight = true; // Flag to check the direction the player is facing

    public CameraMoving cameraMoving; // Reference to the CameraMoving script

    // Enemy touch count tracking
    private Dictionary<string, int> touchCounts = new Dictionary<string, int>()
    {
        { "Enemy_Blob_S", 0 },   // Track touches with Enemy_Blob_S
        { "Enemy_Blob_M", 0 },   // Track touches with Enemy_Blob_M
        { "Enemy_Blob_Boss", 0 } // Track touches with Enemy_Blob_Boss
    };
    private Dictionary<string, int> touchLimits = new Dictionary<string, int>()
    {
        { "Enemy_Blob_S", 4 },   // Player is destroyed after 4 touches
        { "Enemy_Blob_M", 2 },   // Player is destroyed after 2 touches
        { "Enemy_Blob_Boss", 1 } // Player is destroyed after 1 touch
    };

    private int bulletHitCount = 0; // Track the number of times the player is hit by bullets

    private bool isAtFlag = false; // Track if the player is at the flag

    private Vector3 startingPosition; // Track the player's starting position

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component attached to the player

        startingPosition = transform.position; // Save the starting position of the player

        // Hide the messages initially
        if (keyMessage != null)
        {
            keyMessage.gameObject.SetActive(false);
        }

        if (flagMessage != null)
        {
            flagMessage.gameObject.SetActive(false);
        }

        if (bulletHitMessage != null)
        {
            bulletHitMessage.gameObject.SetActive(false);
        }

        if (finishMenuPanel != null)
        {
            finishMenuPanel.SetActive(false); // Hide the finish menu at the start
        }
    }

    void Update()
    {
        // Movement logic
        float move = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // Move left and face left
            move = -1;
            isFacingRight = false;
            transform.rotation = Quaternion.Euler(0, -90, 0); // Rotate player to face left
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // Move right and face right
            move = 1;
            isFacingRight = true;
            transform.rotation = Quaternion.Euler(0, 90, 0); // Rotate player to face right
        }

        playerRb.velocity = new Vector3(speed * move, playerRb.velocity.y);

        // Jump logic
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (jumpCount < 2)  // Allow jump if within double-jump limit
            {
                float jumpMultiplier = jumpCount == 1 ? 2f : 1f; // Adjust force on second jump
                playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce * jumpMultiplier, playerRb.velocity.z);

                jumpCount++;  // Increment jump count

                // Play the jump sound when the player jumps
                if (audioSource != null && jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound);  // Play the jump sound once
                }
            }
        }

        // Handle key grabbing logic
        if (isKeyNearby && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            GrabKey();
        }

        // Handle flag interaction with key
        if (isAtFlag && hasKey && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            ShowFinishMenu();
        }

        // Shuriken shooting logic
        if (Input.GetKeyDown(KeyCode.Space) && canShoot && isFacingRight)
        {
            float currentTime = Time.time;

            // Allow shooting if shuriken count is less than 3
            if (shurikenCount < 3)
            {
                ShootShuriken();
                shurikenCount++;
                lastShootTime = currentTime;
            }

            // If 3 shurikens are shot, start cooldown
            if (shurikenCount == 3)
            {
                canShoot = false;
                StartCoroutine(ResetShootingCooldown());
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Reset jump count when colliding with ground
        if (other.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }

        // Handle enemy collisions
        if (other.gameObject.CompareTag("Enemy"))
        {
            string enemyName = other.gameObject.name;

            if (touchCounts.ContainsKey(enemyName))
            {
                touchCounts[enemyName]++;
                Debug.Log($"Touched {enemyName}: {touchCounts[enemyName]} times.");

                if (touchCounts[enemyName] >= touchLimits[enemyName])
                {
                    ResetPlayer(); // Reset player instead of destroying
                    Debug.Log("Player reset to starting position!");
                }
            }
        }

        // Collectibles logic
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            gameManager.IncreaseCoinCount();
        }
        else if (other.gameObject.CompareTag("Star"))
        {
            Destroy(other.gameObject);
            gameManager.IncreaseStarCount();
        }
        else if (other.gameObject.CompareTag("Diamond"))
        {
            Destroy(other.gameObject);
            gameManager.IncreaseDiamondCount();
        }
        else if (other.gameObject.CompareTag("Treasure"))
        {
            Destroy(other.gameObject);
            gameManager.IncreaseTreasureCount();
        }

        // Play the pickup sound when any collectible is picked up
        if (other.gameObject.CompareTag("Coin") || other.gameObject.CompareTag("Star") ||
            other.gameObject.CompareTag("Diamond") || other.gameObject.CompareTag("Treasure"))
        {
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            isKeyNearby = true;
            nearbyKey = other.gameObject;

            // Show the message
            if (keyMessage != null)
            {
                keyMessage.gameObject.SetActive(true);
                keyMessage.text = "Press Ctrl+G to grab the Key!";
            }

            Debug.Log("Press Ctrl+G to grab the Key!");
        }
        else if (other.CompareTag("Flag"))
        {
            isAtFlag = true;
            if (hasKey)
            {
                if (flagMessage != null)
                {
                    flagMessage.gameObject.SetActive(true);
                    flagMessage.text = "Use Key (Ctrl+L). Continue to Level-2!";
                }
                Debug.Log("Use Key (Ctrl+L). Continue to Level-2!");
            }
            else
            {
                if (flagMessage != null)
                {
                    flagMessage.gameObject.SetActive(true);
                    flagMessage.text = "Defeat Boss Enemy First!";
                }
                Debug.Log("Defeat Boss Enemy First!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            isKeyNearby = false;
            nearbyKey = null;

            // Hide the message
            if (keyMessage != null)
            {
                keyMessage.gameObject.SetActive(false);
            }

            Debug.Log("You moved away from the Key.");
        }
        else if (other.CompareTag("Flag"))
        {
            isAtFlag = false;
            // Hide the flag message when the player leaves the flag area
            if (flagMessage != null)
            {
                flagMessage.gameObject.SetActive(false);
            }
        }
    }

    private void GrabKey()
    {
        if (nearbyKey != null)
        {
            Destroy(nearbyKey); // Remove the key from the scene
            hasKey = true;     // Set the player as having the key

            // Update the message
            if (keyMessage != null)
            {
                StartCoroutine(DisplayKeyGrabMessage());
            }

            Debug.Log("You have grabbed the Key!");
        }
    }

    private IEnumerator DisplayKeyGrabMessage()
    {
        keyMessage.gameObject.SetActive(true);
        keyMessage.text = "Key grabbed successfully!";
        yield return new WaitForSeconds(2f);
        keyMessage.gameObject.SetActive(false);
    }

    private void ShowFinishMenu()
    {
        if (finishMenuPanel != null)
        {
            finishMenuPanel.SetActive(true); // Display the finish menu
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Finish menu displayed. Game paused.");
        }
    }

    private void ResetPlayer()
    {
        transform.position = startingPosition; // Move player back to starting position

        // Reset the camera position
        FindObjectOfType<CameraMoving>().ResetCameraPosition();
    }

    private void ShootShuriken()
    {
        if (weaponPrefab != null && spawnPoint != null)
        {
            Instantiate(weaponPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Shuriken shot!");
        }
    }

    private IEnumerator ResetShootingCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        shurikenCount = 0;
        canShoot = true;
        Debug.Log("Shuriken cooldown reset. Player can shoot again.");
    }

    // New Method: Handle bullet hits
    public void OnBulletHit()
    {
        bulletHitCount++;
        Debug.Log($"Player hit by bullet {bulletHitCount} times.");

        if (bulletHitCount >= 3)
        {
            if (bulletHitMessage != null)
            {
                bulletHitMessage.gameObject.SetActive(true);
                bulletHitMessage.text = "Click the Pause Button to Restart the Game!";
            }
            Destroy(gameObject); // Destroy the player
        }
    }
}
