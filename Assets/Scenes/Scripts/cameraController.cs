using UnityEngine;

namespace UnityChan
{
    public class MyCameraController : MonoBehaviour
    {
        public float smooth = 3f;        // Variable for smoothing camera motion
        private Transform player;        // Reference to the player's transform
        private Vector3 offset;          // Offset distance between the camera and the player

        void Start()
        {
            // Find the player's transform in the scene (assuming the player has a tag "Player")
            player = GameObject.FindGameObjectWithTag("eye").transform;

            // Initialize the offset based on the starting distance and direction from the player to the camera
            offset = transform.position - player.position;

            // Set the camera's initial position and direction based on the offset
            transform.position = player.position + offset;
            transform.LookAt(player);
        }

        void FixedUpdate()
        {
            // Calculate the new camera position based on the player's position and the initial offset
            Vector3 targetPosition = player.position + offset;

            // Smoothly move the camera to the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * smooth);

            // Maintain the initial view direction by keeping the camera looking at the player
            transform.LookAt(player);
        }
    }
}
