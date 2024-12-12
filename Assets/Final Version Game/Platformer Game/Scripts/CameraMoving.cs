using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player
    private float initialOffsetX;             // Initial horizontal offset between camera and player

    private void Start()
    {
        if (player != null)
        {
            // Calculate the initial horizontal offset between the camera and the player
            initialOffsetX = transform.position.x - player.position.x;
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // Update the camera's position to follow the player horizontally
            transform.position = new Vector3(player.position.x + initialOffsetX, transform.position.y, transform.position.z);
        }
    }

    public void ResetCameraPosition()
    {
        if (player != null)
        {
            // Reset the camera's position to its starting horizontal alignment with the player
            transform.position = new Vector3(player.position.x + initialOffsetX, transform.position.y, transform.position.z);
        }
    }
}
