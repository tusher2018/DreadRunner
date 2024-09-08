using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public float StartTimeScale = 1f;          // Initial time scale (1x normal speed)
             
    public float timeToMakeSpeedDouble=600f;
    public AudioSource audioSource;   
    
    private float speedIncreaseRate;
    public bool isGameRunning = false;   
    public bool isGameOver = false;      

    public GameObject player;                 
        
    public GameObject roadsController;         
    public float idleSpeed = 0f;   
    public Color startColor = Color.red; 
    public Color endColor = Color.yellow;  

    public float forwardSpeed= 1.0f; 
    public float laneOffset =  2f;
    public float animSpeed = 1f;
    public float maxSpeed = 20f;  

    public Font titleFont;   

    private string titleText = "Z O M B I E   L A N D";
    private int fontSize; 
    private bool soundEnabled = true; 
    private bool musicEnabled = true; 
    private bool showSettingsDialog = false;
    private bool showMenuDialog = false;
    private bool showProfileDialog = false;

    private PlayerController playerController; 
    
    public GameObject[] playerPrefabs; // Array to hold different player prefabs
    public int currentPlayerIndex = 0; // To track the currently selected player
    private float moveSpeed = 5f; 


    void AppName(){
    
        GUIStyle style = new GUIStyle();
        style.font = titleFont;
        style.alignment = TextAnchor.UpperCenter;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Use the full screen width
        float xStart = 0; // Start at the very left
        float textWidth = screenWidth; // Use the full screen width for the text area
        float yPos = screenHeight / 50; // Adjust yPos to move it closer or further from the top

        // Determine the font size based on screen width
        style.fontSize = (int)((textWidth / titleText.Length)*2);

        // Calculate the total width needed for the entire title text
        Vector2 totalTextSize = style.CalcSize(new GUIContent(titleText));

        // Center the title within the full screen width area
        float xPos = (textWidth - totalTextSize.x) / 2;

        // Draw each character with the gradient color
        for (int i = 0; i < titleText.Length; i++)
        {
            Vector2 size = style.CalcSize(new GUIContent(titleText[i].ToString()));

            // Calculate the vertical gradient color for this character
            // float t = (yPos + size.y / 2) / screenHeight; 

        float t=0.5f;
            style.normal.textColor = Color.Lerp(startColor, endColor, t);

            // Draw the character
            GUI.Label(new Rect(xPos, yPos, size.x, size.y), titleText[i].ToString(), style);

            // Move the xPos for the next character
            xPos += size.x;
        }
    }

    void playButton()
    {
        GUIStyle style = new GUIStyle
        {
            font = titleFont, // Use your custom font
            alignment = TextAnchor.MiddleCenter // Center text both horizontally and vertically
        };

        string titleText = isGameRunning ? "P a u s e " : "R U N !";

        titleText= isGameOver? "Restart" :titleText;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Define the width and position for the text
        float textWidth = screenWidth * 1 / 5; // Use a portion of the screen width
        float yPos = screenHeight - (screenHeight / 10); // Position closer to the bottom of the screen

        // Determine the font size based on text width
        style.fontSize = (int)((textWidth / titleText.Length)*2);

        // Calculate the total width needed for the entire title text
        Vector2 totalTextSize = style.CalcSize(new GUIContent(titleText));

        // Calculate the position for the text area to align it to the bottom-right
        float xPos = screenWidth - textWidth; // Position at the right edge
        float textAreaXPos = xPos; // X position for clickable area
        float textAreaYPos = yPos - totalTextSize.y / 2; // Y position for clickable area

        Rect textRect = new Rect(textAreaXPos, textAreaYPos, textWidth, totalTextSize.y);

        // Draw each character with the gradient color
        float currentXPos = textAreaXPos; // Start position for each character
        for (int i = 0; i < titleText.Length; i++)
        {
            Vector2 size = style.CalcSize(new GUIContent(titleText[i].ToString()));

            // Calculate the vertical gradient color for this character
            float t = 0.5f; // Fixed gradient value, adjust as needed
            style.normal.textColor = Color.Lerp(startColor, endColor, t);

            // Draw the character
            GUI.Label(new Rect(currentXPos, textAreaYPos, size.x, size.y), titleText[i].ToString(), style);

            // Move the xPos for the next character
            currentXPos += size.x;
        }


            if (isGameOver)
            {
                if (Event.current.type == EventType.MouseDown && textRect.Contains(Event.current.mousePosition))
                {
                    RestartGame();
                }
            }else{


                if (Event.current.type == EventType.MouseDown && textRect.Contains(Event.current.mousePosition))
                {
                    if (isGameRunning)
                    {
                        PauseGame();
                    }
                    else
                    {
                        StartGame();
                    }
                    Event.current.Use(); // Consume the event so it doesn't propagate further
                }
            }




    }



    void OthersButton()
    {
        GUIStyle style = new GUIStyle
        {
            font = titleFont, // Use your custom font
            alignment = TextAnchor.MiddleCenter // Center text both horizontally and vertically
        };

        // Calculate screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Button dimensions
        float buttonWidth = screenWidth * 1 / 3; // Width of the button area
        float buttonHeight = screenHeight / 10; // Height of the button area

        // Positions for the buttons
        float settingsButtonYPos = screenHeight - buttonHeight * 6.4f; // Position for the Settings button (above Store)
        float storeButtonYPos = screenHeight - buttonHeight * 4.2f; // Position for the Store button (upper left, above Maps)
        float mapsButtonYPos = screenHeight - buttonHeight * 2f; // Position for the Maps button (bottom left)
        float menuButtonYPos = screenHeight - buttonHeight * 2f; // Position for the Menu button (bottom center)
        float profileButtonYPos = screenHeight - buttonHeight * 6.4f;

        // Button text
        string menuText = "Player";
        string mapsText = "Maps";
        string storeText = "Store";
        string settingsText = "Settings";
        string profileText = "Profile";

        // Style settings
        style.fontSize = (int)(buttonWidth / menuText.Length);

        // Calculate sizes and positions
        Vector2 menuTextSize = style.CalcSize(new GUIContent(menuText));
        Vector2 mapsTextSize = style.CalcSize(new GUIContent(mapsText));
        Vector2 storeTextSize = style.CalcSize(new GUIContent(storeText));
        Vector2 settingsTextSize = style.CalcSize(new GUIContent(settingsText));
        Vector2 profileTextSize = style.CalcSize(new GUIContent(profileText));

        // Button rectangles
        Rect settingsRect = new Rect(buttonWidth / 6, settingsButtonYPos, buttonWidth, buttonHeight);
        Rect storeRect = new Rect(buttonWidth / 6, storeButtonYPos, buttonWidth, buttonHeight);
        Rect mapsRect = new Rect(buttonWidth / 6, mapsButtonYPos, buttonWidth, buttonHeight);
        Rect menuRect = new Rect((screenWidth - buttonWidth)/1.5f , menuButtonYPos, buttonWidth, buttonHeight);
        Rect profileRect = new Rect((screenWidth - buttonWidth/2) , profileButtonYPos, buttonWidth, buttonHeight);

        // Draw Settings button
        DrawTextButton(settingsRect, settingsText, style);

        // Draw Store button
        DrawTextButton(storeRect, storeText, style);

        // Draw Maps button
        DrawTextButton(mapsRect, mapsText, style);

        // Draw Menu button
        DrawTextButton(menuRect, menuText, style);
        
        // Draw Profile button
        DrawTextButton(profileRect, profileText, style);

        // Check for clicks
        if (Event.current.type == EventType.MouseDown)
        {
            if (settingsRect.Contains(Event.current.mousePosition))
            {
                OpenSettings();
                Event.current.Use(); // Consume the event
            }
            else if (menuRect.Contains(Event.current.mousePosition))
            {
                OpenMenu();
                Event.current.Use(); // Consume the event
            }
            else if (mapsRect.Contains(Event.current.mousePosition))
            {
                OpenMaps();
                Event.current.Use(); // Consume the event
            }
            else if (storeRect.Contains(Event.current.mousePosition))
            {
                OpenStore();
                Event.current.Use(); // Consume the event
            }
             else if (profileRect.Contains(Event.current.mousePosition))
            {
                OpenProfile();
                Event.current.Use(); // Consume the event
            }
        }
    }

    void DrawTextButton(Rect rect, string text, GUIStyle style)
    {
        // Draw each character with the gradient color
        float xPos = rect.x;
        float yPos = rect.y;

        for (int i = 0; i < text.Length; i++)
        {
            Vector2 size = style.CalcSize(new GUIContent(text[i].ToString()));

            // Calculate gradient color
            float t = 0.5f; // Fixed gradient value, adjust as needed
            style.normal.textColor = Color.Lerp(startColor, endColor, t);

            // Draw the character
            GUI.Label(new Rect(xPos, yPos, size.x, size.y), text[i].ToString(), style);

            // Move xPos for the next character
            xPos += size.x;
        }
    }



public void OpenPlayers(int playerIndex)
{
    // Get the mouse position
    Vector2 mousePosition = Event.current.mousePosition;

    // Define the area for the left arrow button
    Rect leftArrowRect = new Rect(Screen.width/5, Screen.height/2, Screen.width/10, Screen.height/10);

    // Define the area for the right arrow button
    Rect rightArrowRect = new Rect(Screen.width - Screen.width/4, Screen.height/2, Screen.width/10, Screen.height/10);


    // Define a GUIStyle for responsive text
    GUIStyle buttonStyle = new GUIStyle(GUI.skin.box);
    buttonStyle.alignment = TextAnchor.MiddleCenter;

    // Adjust font size based on screen size
    buttonStyle.fontSize = Mathf.Max(10, Screen.width / 30);


    // Check for mouse clicks
    if (Event.current.type == EventType.MouseDown)
    {
        // Check if the left arrow was clicked
        if (leftArrowRect.Contains(mousePosition))
        {
            Debug.Log("Left arrow clicked");
            // Move to the previous player
            changePlayer((playerIndex - 1 + playerPrefabs.Length) % playerPrefabs.Length);

        }

        // Check if the right arrow was clicked
        if (rightArrowRect.Contains(mousePosition))
        {
            Debug.Log("Right arrow clicked");
            // Move to the next player
                changePlayer((playerIndex + 1) % playerPrefabs.Length);
        }
    }

    // Draw the buttons (visual only, no functionality)
    GUI.Box(leftArrowRect, "<",buttonStyle);
    GUI.Box(rightArrowRect, ">",buttonStyle);
}

void changePlayer(int newIndex)
{

 Debug.Log(newIndex);
    currentPlayerIndex = newIndex;
    

    if (player != null)
    {
        // Destroy the current player instance
        Destroy(player);
    }

    // Instantiate the new player prefab
    player = Instantiate(playerPrefabs[currentPlayerIndex], Vector3.zero, Quaternion.identity);
}


    // Methods to handle button actions
    void OpenMenu()
    {
        showMenuDialog=!showMenuDialog;
        showSettingsDialog=false;
        showProfileDialog=false;
    }

    void OpenProfile(){
        showSettingsDialog = false;
        showMenuDialog=false;
        showProfileDialog=true;
    }


    void OpenMaps()
    {
        // Implement maps logic here
    }

    void OpenStore()
    {
        // Implement store logic here
    }

    void OpenSettings()
    {
    showSettingsDialog = true;
    showMenuDialog=false;
    showProfileDialog=false;
    }

int maxCoins=100;
int longestRun=1000;
int maxSurviveTime=10;
int highestScore=1000;
int totalCoin=1000;

    void DrawProfileDialog()
    {
        // Define styles for the dialog box and text labels
        GUIStyle dialogStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.RoundToInt(Screen.height / 10f), // Font size for the title
            normal = { textColor = Color.white }
        };

        GUIStyle textStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = Mathf.RoundToInt(Screen.height / 10f), // Font size for the text (slightly smaller)
            normal = { textColor = Color.white }
        };

        // Center the dialog on the screen
        float dialogWidth = Screen.width * 0.6f;
        float dialogHeight = Screen.height / 1.3f;
        float dialogX = (Screen.width - dialogWidth) / 2;
        float dialogY = (Screen.height - dialogHeight) / 2;

        // Draw dialog background
        GUI.Box(new Rect(dialogX, dialogY, dialogWidth, dialogHeight), "", GUI.skin.box);

        // Draw dialog title
        GUI.Label(new Rect(dialogX, dialogY + (dialogHeight / 32), dialogWidth, dialogHeight / 6), "Profile", dialogStyle);

        // Text dimensions and positions relative to the dialog
        float textWidth = dialogWidth * 0.8f;
        float textHeight = dialogHeight / 10; // Height for each text line
        float textSpacing = dialogHeight / 20; // Spacing between text lines
        float contentStartY = dialogY + (dialogHeight / 7) + textSpacing; // Start position for the text content

        // Calculate font size based on textHeight
        textStyle.fontSize = Mathf.RoundToInt(textHeight / 1.5f); // Adjust as needed for better fit

        // Display profile statistics
        
        GUI.Label(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Max Coins Collected: " + maxCoins, textStyle);
        contentStartY += textHeight + textSpacing/2;
        GUI.Label(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Longest Run: " + longestRun + " meters", textStyle);
        contentStartY += textHeight + textSpacing/2;
        GUI.Label(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Max Survive Time: " + maxSurviveTime + " seconds", textStyle);
        contentStartY += textHeight + textSpacing/2;
        GUI.Label(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Highest Score: " + highestScore, textStyle);
        contentStartY += textHeight + textSpacing/2;
         GUI.Label(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Total Coin: " + totalCoin, textStyle);
        contentStartY += textHeight + textSpacing;
         
        if ( GUI.Button(new Rect(dialogX + (dialogWidth - textWidth) / 2, contentStartY, textWidth, textHeight), "Close"))
        {
            showProfileDialog = false;
        }

        // Additional text statistics can be added in a similar way
    }



    void DrawSettingsDialog()
    {
        // Define styles for the dialog box and buttons
        GUIStyle dialogStyle = new GUIStyle
        {
            // font = titleFont,
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.RoundToInt(Screen.height / 30f), // Font size for the title
            normal = { textColor = Color.white }
        };

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            // font = titleFont,
            alignment = TextAnchor.MiddleCenter,
            normal = { background = MakeTex(2, 2, Color.grey) },
            active = { background = MakeTex(2, 2, Color.grey) }
        };

        // Center the dialog on the screen
        float dialogWidth = Screen.width * 0.6f;
        float dialogHeight = Screen.height / 1.3f;
        float dialogX = (Screen.width - dialogWidth) / 2;
        float dialogY = (Screen.height - dialogHeight) / 2; // Adjusted to center vertically

        // Draw dialog background
        GUI.Box(new Rect(dialogX, dialogY, dialogWidth, dialogHeight), "", GUI.skin.box);

        // Draw dialog title
        GUI.Label(new Rect(dialogX, dialogY + (dialogHeight / 12), dialogWidth, dialogHeight / 6), "Settings", dialogStyle);

        // Button dimensions and positions relative to the dialog
        float buttonWidth = dialogWidth * 0.8f;
        float buttonHeight = dialogHeight / 8; // Height of the button area
        float buttonSpacing = dialogHeight / 12; // Spacing between buttons
        float contentHeight = dialogHeight - (dialogHeight / 6) - (buttonSpacing * 3); // Space for title, padding, and buttons
        int numButtons = 4; // Number of buttons (including the new "Settings" button)
        float totalButtonHeight = numButtons * buttonHeight + (numButtons - 1) * buttonSpacing;
        float buttonStartY = dialogY + (dialogHeight / 12) + (contentHeight - buttonHeight) / 2; // Center buttons vertically

        // Calculate font size based on buttonHeight
        buttonStyle.fontSize = Mathf.RoundToInt(buttonHeight / 2.5f); // Adjust as needed for better fit

        // Sound toggle button
        float buttonX = dialogX + (dialogWidth - buttonWidth) / 2;
        if (GUI.Button(new Rect(buttonX, buttonStartY, buttonWidth, buttonHeight), soundEnabled ? "Sound: ON" : "Sound: OFF", buttonStyle))
        {
            soundEnabled = !soundEnabled;
            ToggleSound(soundEnabled);
        }

        // Music toggle button
        buttonStartY += buttonHeight + buttonSpacing; // Move down for the next button
        if (GUI.Button(new Rect(buttonX, buttonStartY, buttonWidth, buttonHeight), musicEnabled ? "Music: ON" : "Music: OFF", buttonStyle))
        {
            musicEnabled = !musicEnabled;
            ToggleMusic(musicEnabled);
        }

        // Close button
        buttonStartY += buttonHeight + buttonSpacing; // Move down for the close button
        if (GUI.Button(new Rect(buttonX, buttonStartY, buttonWidth, buttonHeight), "Close", buttonStyle))
        {
            showSettingsDialog = false;
        }

        // Additional space below the Close button
        float extraSpace = dialogY + dialogHeight - (buttonStartY + buttonHeight + buttonSpacing);
    
    }
    // Utility function to create a texture for button backgrounds
    Texture2D MakeTex(int width, int height, Color col)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pix = texture.GetPixels();
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        texture.SetPixels(pix);
        texture.Apply();
        return texture;
    }

    void ToggleSound(bool enabled)
    {
        // Implement the logic to turn sound on or off
        Debug.Log("Sound " + (enabled ? "Enabled" : "Disabled"));
    }

    void ToggleMusic(bool enabled)
    {
        // Implement the logic to turn music on or off
        Debug.Log("Music " + (enabled ? "Enabled" : "Disabled"));
    }


    IEnumerator IncreaseSpeedOverTime(float duration, float targetSpeed)
    {
        float initialSpeed = forwardSpeed;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            forwardSpeed = Mathf.Lerp(initialSpeed, targetSpeed, timeElapsed / duration);
            yield return null;  // Wait until the next frame
        }

        forwardSpeed = targetSpeed;  // Ensure the speed is exactly 20 at the end
    }

void Start(){
    changePlayer(0);
}
    void Update()
    {
        if(isGameRunning){
            audioSource.volume = 1f;
            
            // if(Mathf.Exp(speedIncreaseRate * Time.time) * StartTimeScale < 1.3){
            //  Time.timeScale = Mathf.Exp(speedIncreaseRate * Time.time) * StartTimeScale;
            //  }
            
        }

        if (isGameOver)
        {
           // game over hendelment
        }
    }

    void OnGUI()
    {

        if(!isGameRunning && Time.timeScale!=0){
            audioSource.volume = 0.5f;
            
            AppName();
            OthersButton();
            if (showSettingsDialog)
            {
                DrawSettingsDialog();
            }
            if (showMenuDialog)
            {
                OpenPlayers(currentPlayerIndex);
            }
               if (showProfileDialog)
            {
                DrawProfileDialog();
            }
        }
        playButton();
            
    }

    void StartGame()
    {

showMenuDialog=false;
        player = GameObject.FindGameObjectWithTag("Player");
        // Set the initial time scale
        Time.timeScale = StartTimeScale;
        // Calculate the speed increase rate
        speedIncreaseRate = Mathf.Log(2f) / timeToMakeSpeedDouble;


        isGameRunning = true;
        isGameOver = false;
                
            
        player.GetComponent<AudioSource>().volume=0.2f;  
        player.GetComponent<AudioSource>().Play();  
        if (player != null)
        {
            player.SetActive(true);

            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.forwardSpeed = forwardSpeed; 
                playerController.laneOffset = laneOffset;
                playerController.animSpeed = animSpeed;
            }
        }

    

        if (roadsController != null)
        {
            roadsController.SetActive(true);
            roadsController.GetComponent<AllSpawner>().laneOffset=laneOffset;
        }
        StartCoroutine(IncreaseSpeedOverTime(timeToMakeSpeedDouble, maxSpeed));
    }

    void PauseGame()
    {

        isGameRunning = false;
        Time.timeScale = 0; 

        if (player != null)
        {
            player.SetActive(true);

            playerController = player.GetComponent<PlayerController>();
            player.GetComponent<AudioSource>().Stop();
            if (playerController != null)
            {
                playerController.forwardSpeed = idleSpeed;
                playerController.laneOffset = idleSpeed;
                playerController.animSpeed = 0f; 
            }
        }

    

        if (roadsController != null)
        {
            roadsController.SetActive(false);
        }
    }

    public void EndGame()
    {
        audioSource.volume=0.5f;
        isGameRunning = false;
        isGameOver = true;
        Time.timeScale = 0; 
    }

    public void RestartGame()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
