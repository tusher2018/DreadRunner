using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    public Font titleFont;   


    public float animSpeed = 1f;                // Animation playback speed
    public float forwardSpeed = 0f;               // Forward speed
    public float jumpPower = 6.0f;                // Jump power
    public bool useCurves = true;                 // Whether to use Mecanim curves for animation adjustment
    public float useCurvesHeight = 0.5f;          // Height for curve correction
    public float laneChangeSpeed = 10.0f;         // Speed of lane changing
    public float laneOffset = 0.0f;               // Distance between each lane
    
    public AudioSource audioSource;              
    public AudioClip runningSound;                
    public AudioClip jumpingSound;                
    public AudioClip hittingSound;  
    public AudioClip goldPickingSound;  

    public int goldCount = 0;  // Player's gold count
    public int scoreAddOverTime=1;

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




    private int score = 0;     // Player's score




    
    public void AddScore(int points)
    {
        score += points;
    }

    // OnGUI is called for rendering and handling GUI events
    void OnGUI()
    {
        if(gameManager.isGameRunning){
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = Screen.width/24;
        guiStyle.normal.textColor = Color.white; // Set the text color
        
        float textHeight = Screen.height / 10; // Height for each text line
        float textSpacing = Screen.height / 20; // Spacing between text lines
     

        // Draw the Score on the top left corner of the screen
        GUI.Label(new Rect(10, 0, 200, 50), "Score: " + score.ToString(), guiStyle);
float contentStartY =(Screen.height / 10);
        // Draw the Gold count below the Score
        GUI.Label(new Rect(10, contentStartY, 200, 50), "Gold: " + goldCount.ToString(), guiStyle);}



        if(gameManager.isGameOver){
    GUIStyle dialogStyle = new GUIStyle
    {
        font = titleFont,
        alignment = TextAnchor.MiddleCenter,
        
        fontSize = Mathf.RoundToInt(Screen.height / 7f), // Font size for the title
        normal = { textColor = Color.red }
    };

    // Center the dialog on the screen
    float dialogWidth = Screen.width * 0.6f;
    float dialogHeight = Screen.height / 1.3f;
    float dialogX = (Screen.width - dialogWidth) / 2;
    float dialogY = (Screen.height - dialogHeight) / 2; // Adjusted to center vertically

    // Draw dialog background
    GUI.Box(new Rect(dialogX, dialogY, dialogWidth, dialogHeight), "", GUI.skin.box);

    // Draw dialog title
    GUI.Label(new Rect(dialogX, dialogY + (dialogHeight / 6), dialogWidth, dialogHeight / 6), "Game Over", dialogStyle);

   dialogStyle.fontSize = Mathf.RoundToInt(Screen.height / 10f);
dialogStyle.normal.textColor=Color.white;

    // Draw dialog title
    GUI.Label(new Rect(dialogX, dialogY + (dialogHeight / 6)*2.5f, dialogWidth, dialogHeight / 6), "Score: "+score.ToString(), dialogStyle);
      // Draw dialog title
    GUI.Label(new Rect(dialogX, dialogY + (dialogHeight / 6)*4f, dialogWidth, dialogHeight / 6), "Gold: "+goldCount.ToString(), dialogStyle);

        }
    }











    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        cameraObject = GameObject.FindWithTag("MainCamera");
        gameManager = GameObject.FindWithTag("gameManeger").GetComponent<GameManager>();

        orgColHeight = col.height;
        orgVectColCenter = col.center;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = runningSound;
        audioSource.loop = true; 

        targetLanePosition = (currentLane - 1) * laneOffset;
    }

    void FixedUpdate()
    {
        if (forwardSpeed > 0)
        {
            anim.SetFloat("Speed", 1); // Move forward

            if (!audioSource.isPlaying) 
            {
                StartCoroutine(WaitAndPlayRunningSound());  
                    
            }
        }
        else
        {
            anim.SetFloat("Speed", 0); // Idle animation
            if (audioSource.isPlaying) // Stop running sound when not moving
            {
                audioSource.Stop();
            }
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
                   
                   

                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {

                            
                                
                            // col.height = orgColHeight - jumpHeight;
                            // float adjCenterY = orgVectColCenter.y + jumpHeight;
                            // col.center = new Vector3(0, adjCenterY, 0);
                        }
                        else
                        {
                            resetCollider();
                        }
                    }
                }

                
                
            }
        }

        // Handle slide down, sliding, and slide up states
        HandleSlideStates();

        AddScore(scoreAddOverTime);
        
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (2 - 1) * Time.deltaTime;
        }

    }




    public void AddGold(int amount)
    {
        goldCount += amount;
        PlaySound(goldPickingSound, false,1f);  
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
                if (audioSource.isPlaying && audioSource.clip == runningSound)
                {
                    audioSource.Stop();
                }

  
            }
            else
            {
                anim.SetBool("Sliding", false);
                audioSource.Play();
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
                        if (audioSource.isPlaying && audioSource.clip == runningSound)
                        {
                            audioSource.Stop();
                        }
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                      
  
        

                    anim.SetBool("Jump", true);
                    PlaySound(jumpingSound, false,1f);  
                    StartCoroutine(WaitAndPlayRunningSound());  
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


    IEnumerator WaitAndPlayRunningSound()
    {
        yield return new WaitForSeconds(1f);  // Wait for the jump sound to finish + 0.5 seconds delay

        if (forwardSpeed > 0)  // Only play if still moving forward
        {
            PlaySound(runningSound, true, 0.2f);  // Play running sound at 30% volume
        }
    }



    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Handle swipe gestures
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 endTouchPosition = touch.position;
                Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                // Horizontal swipe detection (for lane change)
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (swipeDelta.x > 0 && currentLane < 2) // Swipe Right
                    {
                        currentLane++;
                        targetLanePosition = (currentLane - 1) * laneOffset;
                    }
                    else if (swipeDelta.x < 0 && currentLane > 0) // Swipe Left
                    {
                        currentLane--;
                        targetLanePosition = (currentLane - 1) * laneOffset;
                    }
                }
                // Vertical swipe detection (for jump/slide)
                else
                {
                    if (swipeDelta.y > 0) // Swipe Up (Jump)
                    {
                        if (currentBaseState.nameHash == locoState && !anim.IsInTransition(0))
                        {
                            if (audioSource.isPlaying && audioSource.clip == runningSound)
                            {
                                audioSource.Stop();
                            }
                            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                            anim.SetBool("Jump", true);
                            PlaySound(jumpingSound, false, 1f);
                            StartCoroutine(WaitAndPlayRunningSound());
                        }
                    }
                    else if (swipeDelta.y < 0) // Swipe Down (Slide)
                    {
                        if (currentBaseState.nameHash == locoState && !anim.IsInTransition(0))
                        {
                            anim.SetBool("SlideDown", true);
                            // Optionally adjust the collider size for the slide animation
                            col.height = orgColHeight / 2;
                            col.center = new Vector3(0, orgVectColCenter.y / 2, 0);
                        }
                    }
                }
            }
        }
    }


    void resetCollider()
    {
        col.height = orgColHeight;
        col.center = orgVectColCenter;
    }

    void PlaySound(AudioClip clip, bool loop, float volume)
    {
        audioSource.clip = clip;        // Set the audio clip
        audioSource.loop = loop;        // Set loop mode
        audioSource.volume = volume;    // Set the volume
        audioSource.Play();             // Play the clip
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("killer"))
        {   
            if (audioSource.isPlaying && audioSource.clip == runningSound)
            {
                audioSource.Stop();
            }
            anim.SetBool("Damage", true);
            PlaySound(hittingSound, false,1f);  
            forwardSpeed = 0;
            StartCoroutine(WaitAndEndGame(1f));
        }

        anim.SetBool("Jump", false);
        
    }

    IEnumerator WaitAndEndGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameManager.EndGame(goldCount,score);
    }
}
