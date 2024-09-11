using UnityEngine;

namespace UnityChan
{
    public class MyCameraController : MonoBehaviour
    {
        public float smooth = 3f;        // Variable for smoothing camera motion
        private Transform player;        // Reference to the player's transform
        private Vector3 offset;          // Offset distance between the camera and the player
private Vector3 initialPosition;

void Start(){
    initialPosition=transform.position;
}

   

        void FixedUpdate()
        {
            if(player==null){
            // Find the player's transform in the scene (assuming the player has a tag "Player")
            GameObject playerEye=GameObject.FindGameObjectWithTag("eye");
            if(playerEye== null)return;
            player = playerEye.transform;

            // Initialize the offset based on the starting distance and direction from the player to the camera
            offset = initialPosition - player.position;

            // Set the camera's initial position and direction based on the offset
            transform.position = player.position + offset;
            transform.LookAt(player);
            }else{
                // Calculate the new camera position based on the player's position and the initial offset
                Vector3 targetPosition = player.position + offset;

                // Smoothly move the camera to the target position
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * smooth);

                // Maintain the initial view direction by keeping the camera looking at the player
                transform.LookAt(player);
            }
        }
    }
}
