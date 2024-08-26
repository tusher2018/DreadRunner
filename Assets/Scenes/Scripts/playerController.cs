using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float animSpeed = 1.5f;                // Animation playback speed
    public float forwardSpeed = 0f;               // Forward speed
    public float jumpPower = 2.0f;                // Jump power
    public bool useCurves = true;                 // Whether to use Mecanim curves for animation adjustment
    public float useCurvesHeight = 0.5f;          // Height for curve correction
    public float laneChangeSpeed = 10.0f;         // Speed of lane changing
    public float laneOffset = 2.0f;               // Distance between each lane

    private CapsuleCollider col;
    private Rigidbody rb;
    private Vector3 velocity;
    private float orgColHeight;
    private Vector3 orgVectColCenter;
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    private GameObject cameraObject;

    private int currentLane = 1;                  // 0 = left, 1 = middle, 2 = right
    private float targetLanePosition;             // Target position for lane change

    private Vector2 startTouchPosition, swipeDelta;
    private bool isSwiping = false;

    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int slideDownState = Animator.StringToHash("Base Layer.SlideDown");
    static int slidingState = Animator.StringToHash("Base Layer.Sliding");
    static int slideUpState = Animator.StringToHash("Base Layer.SlideUp");

    public GameManager gameManager;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        cameraObject = GameObject.FindWithTag("MainCamera");
        gameManager = GameObject.FindWithTag("gameManeger").GetComponent<GameManager>();

        orgColHeight = col.height;
        orgVectColCenter = col.center;

        targetLanePosition = (currentLane - 1) * laneOffset;
    }

    void FixedUpdate()
    {
        if (forwardSpeed > 0)
        {
            anim.SetFloat("Speed", 1); // Move forward
        }
        else
        {
            anim.SetFloat("Speed", 0); // Idle animation
        }

        anim.SetFloat("Direction", 0); // No need for direction animation anymore
        anim.speed = animSpeed; // Set animation speed

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0); // Get current animation state

        rb.useGravity = true; // Enable gravity by default

        // Automatically move forward
        velocity = new Vector3(0, 0, forwardSpeed);
        velocity = transform.TransformDirection(velocity);

        // Apply movement forward
        transform.localPosition += velocity * Time.fixedDeltaTime;

        // Handle input for both PC and Android
        HandlePCInput();
        HandleTouchInput();

        // Smoothly move the player towards the target lane position
        Vector3 targetPosition = new Vector3(targetLanePosition, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, laneChangeSpeed * Time.fixedDeltaTime);

        // Handle animations for jumping and landing
        if (currentBaseState.nameHash == jumpState)
        {
            if (!anim.IsInTransition(0))
            {
                if (useCurves)
                {
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");

                    if (gravityControl > 0)
                        rb.useGravity = false;

                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHeight - jumpHeight;
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0);
                        }
                        else
                        {
                            resetCollider();
                        }
                    }
                }

                anim.SetBool("Jump", false);
            }
        }

        // Handle slide down, sliding, and slide up states
        HandleSlideStates();
    }

    void HandleSlideStates()
    {
        if (currentBaseState.nameHash == slideDownState)
        {
            // Transition from slide down to sliding
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("SlideDown", false);
                anim.SetBool("Sliding", true);
            }
        }

        if (currentBaseState.nameHash == slidingState)
        {
            // Handle continuous sliding (e.g., holding down the slide key)
            if (Input.GetKey(KeyCode.S))
            {
                anim.SetBool("Sliding", true);
            }
            else
            {
                anim.SetBool("Sliding", false);
                anim.SetBool("SlideUp", true);
            }
        }

        if (currentBaseState.nameHash == slideUpState)
        {
            // Reset after sliding up
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("SlideUp", false);
                resetCollider();
            }
        }
    }

    void HandlePCInput()
    {
        // Handle lane change input using keyboard (A, D keys)
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            currentLane--;
            targetLanePosition = (currentLane - 1) * laneOffset;
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            currentLane++;
            targetLanePosition = (currentLane - 1) * laneOffset;
        }

        // Handle jump input using the W key
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentBaseState.nameHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);
                }
            }
        }

        // Handle slide input using the S key
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentBaseState.nameHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("SlideDown", true);
                    // Optionally adjust the collider size for the slide animation
                    col.height = orgColHeight / 2;
                    col.center = new Vector3(0, orgVectColCenter.y / 2, 0);
                }
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isSwiping = true;
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    swipeDelta = touch.position - startTouchPosition;

                    // Detect horizontal swipe (left/right)
                    if (isSwiping && Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 50 && currentLane < 2)
                        {
                            currentLane++;
                            targetLanePosition = (currentLane - 1) * laneOffset;
                        }
                        else if (swipeDelta.x < -50 && currentLane > 0)
                        {
                            currentLane--;
                            targetLanePosition = (currentLane - 1) * laneOffset;
                        }
                        isSwiping = false;
                    }

                    // Detect vertical swipe (up) for jumping
                    if (isSwiping && swipeDelta.y > 50)
                    {
                        if (currentBaseState.nameHash == locoState)
                        {
                            if (!anim.IsInTransition(0))
                            {
                                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                                anim.SetBool("Jump", true);
                            }
                        }
                        isSwiping = false;
                    }

                    // Detect downward swipe for sliding
                    if (isSwiping && swipeDelta.y < -50)
                    {
                        if (currentBaseState.nameHash == locoState)
                        {
                            if (!anim.IsInTransition(0))
                            {
                                anim.SetBool("SlideDown", true);
                                // Optionally adjust the collider size for the slide animation
                                col.height = orgColHeight / 2;
                                col.center = new Vector3(0, orgVectColCenter.y / 2, 0);
                            }
                        }
                        isSwiping = false;
                    }
                    break;

                case TouchPhase.Ended:
                    isSwiping = false;
                    break;
            }
        }
    }

    void resetCollider()
    {
        col.height = orgColHeight;
        col.center = orgVectColCenter;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("killer"))
        {
            anim.SetBool("Damage", true);
            forwardSpeed = 0;
            StartCoroutine(WaitAndEndGame(1f));
        }
    }

    IEnumerator WaitAndEndGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameManager.EndGame();
    }
}
