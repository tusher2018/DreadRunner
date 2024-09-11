using UnityEngine;

public class TrueHorror : MonoBehaviour
{
    public float speed = 5f;               // Speed at which the zombie moves towards the player
    public float grabDistance = 15f;       // Distance at which the zombie will grab the player
    public float runStartDistance = 100f;  // Minimum distance at which the zombie starts running
    public float lookSpeed = 2f;           // Speed at which the zombie turns to look at the player
    public float tolerance = 0.1f;         // Tolerance for checking if the player is on the same line
    public Collider[] grabColliders;       // Array of colliders to be activated when grabbing the player

    private Transform player;              // Reference to the player's transform
    private Animator animator;             // Reference to the Animator component
    private bool isRunning = false;        // Whether the zombie is currently running towards the player
    private bool isGrabbing = false;       // Whether the zombie is currently grabbing the player

    void Start()
    {
        // Automatically find the player by tag
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Ensure the player is found
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player object is tagged 'Player'.");
        }

        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Ensure the Animator is found
        if (animator == null)
        {
            Debug.LogError("Animator component not found! Make sure the Animator is attached to the same GameObject as this script.");
        }

        // Deactivate all grab colliders initially
        foreach (Collider col in grabColliders)
        {
            if (col != null) // Check if collider is not null
            {
                col.enabled = false;
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            HandleBehavior();
            LookAtPlayer();
        }else{
             GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        }
    }

    void HandleBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (IsPlayerInLine())
        {
            if (distanceToPlayer <= grabDistance)
            {
                GrabPlayer();
            }
            else if (distanceToPlayer <= runStartDistance)
            {
                StartRunning();
            }
            else
            {
                StopRunning();
            }
        }
        else
        {
            StopRunning();
        }
    }

    bool IsPlayerInLine()
    {
        float difference = Mathf.Abs(transform.position.y - player.position.y); // Calculate the absolute difference
        return difference <= tolerance; // Check if the difference is within the tolerance
    }

    void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // Keep the zombie upright
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }
    }

    void StartRunning()
    {
        if (!isRunning)
        {
            isRunning = true;
            isGrabbing = false; // Ensure grabbing is false
            animator.SetBool("Run", true);
            animator.SetBool("Grab", false);
        }

        // Deactivate grab colliders while running
        foreach (Collider col in grabColliders)
        {
            if (col != null) // Check if collider is not null
            {
                col.enabled = false;
            }
        }

        // Move towards the player if within range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > grabDistance) // Avoid moving if already grabbing
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    void GrabPlayer()
    {
        if (!isGrabbing)
        {
            isGrabbing = true;
            isRunning = false; // Ensure running is false
            animator.SetBool("Grab", true);
            animator.SetBool("Run", false);

            // Activate grab colliders when grabbing
            foreach (Collider col in grabColliders)
            {
                if (col != null) // Check if collider is not null
                {
                    col.enabled = true;
                }
            }
        }
    }

    void StopRunning()
    {
        if (isRunning)
        {
            isRunning = false;
            animator.SetBool("Run", false);
        }

        // Deactivate grab colliders when stopping
        foreach (Collider col in grabColliders)
        {
            if (col != null) // Check if collider is not null
            {
                col.enabled = false;
            }
        }
    }
}
