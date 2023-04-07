using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    // MasterController is the overall game controller. It controls scene, screen and sound transition and
    // primary indicators such as score, time, etc; and associated events like game start or game over

    // Editor vars for Title Scene objects
    //[SerializeField] public SoundController soundController;
    [SerializeField] private GameObject instructionsPanel, menuPanel;
    [SerializeField] private CameraMovement mainCamera;
    // Editor vars for Game Scene objects
    [SerializeField] private Level[] availableLevels; 
    // Serializing transition times and lives for easy test config
    [SerializeField] public int levelChangeDuration, phaseChangeDuration, livesCount;
    [SerializeField] public float timeCount;
    // References for Game runtime objects
    public PauseController pauseController;
    public Level currentLevel;
    public LevelDisplay levelDisplay;
    public EnemyCounter enemyCounter;
    public ItemController itemController;
    private MapScrollController scrollController;
    //public GameObject entryPoint, backgroundObject, MDCObject;
    //public TileController tileController;
    // In-game tracking variables
    public bool changingLevel, levelStarted, gameOver, timeUp, startingScroll, scrollPhase, bossPhase;
    public int currentLevelKey, currentPhaseKey, pointsCount;    
    private float phaseChangeTimer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Checking for "escape" key press to close the Instructions screen if open
        if(Input.GetKeyDown("escape") && SceneManager.GetActiveScene().name == "Title") {
            if(instructionsPanel.activeSelf) {
                HideInstructionsPanel();
            } 
        }
        // Starting and updating timer if within a level, not paused and not within scroll phase
        if(levelStarted && !gameOver && pauseController != null && !pauseController.gamePaused && !scrollPhase) {
            timeCount -= Time.deltaTime;
            // Game Over by time up
            if(timeCount <= 0 && !timeUp) {
                timeCount = 0;
                timeUp = true;
                Time.timeScale = 0;
                gameOver = true;
                levelStarted = false;
                StartCoroutine(NextPhaseCoroutine("over"));
            }
        } 
    }
    // Show the instructions screen and hide title screen elements when "instructions" button pressed
    public void ShowInstructionsPanel()
    {
        menuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }
    // Show the Title screen and hide instructions screen
    public void HideInstructionsPanel()
    {
        menuPanel.SetActive(true);
        instructionsPanel.SetActive(false);
    }
    // Setting references to in-game objects only when the objects are created 
    public void SetLevelDisplay(LevelDisplay LDRef)
    {     
        if(levelDisplay == null) {
            levelDisplay = LDRef;
        }
    }
    
    public void SetEnemyCounter(EnemyCounter ECRef)
    {             
        if(enemyCounter == null) {
            enemyCounter = ECRef;
        }
    }

    public void SetPauseController(PauseController PCRef)
    {
        if(pauseController == null) {
            pauseController = PCRef;
        }
    }

    public void SetItemController(ItemController ICRef)
    {     
        if(itemController == null) {
            itemController = ICRef;
        }
    }

    // public void SetTileManager(TileController TMRef)
    // {     
    //     if(tileController == null) {
    //         tileController = TMRef;
    //     }
    // }

    // Sets up for the game and makes scene transition
    public void StartGame()
    {
        Time.timeScale = 0;
        levelStarted = false;
        changingLevel = true;
        // Restarting Level and Phase keys
        currentLevelKey = 0;
        currentPhaseKey = 1;
        // Stopping menu music and starting game music
        // soundController.StopMusic();
        // soundController.SetCurrentMusicClip();
        // soundController.PlayMusic();
        // Setting Level and Level parameters and enemies
        currentLevel = availableLevels[currentLevelKey];
        timeCount = currentLevel.levelTime;
        currentLevel.SetInitialEnemies();
        //entryPoint = null;
        // if(currentLevelKey > 1) {    
        //     tileController.SetLevelTiles(currentLevelKey - 1);
        //     player.PlayerSpawn();
        //     enemyCounter.Start();
        //     itemController.FlushItems();
        //     itemController.StartItems(5.0f);
        // } else {
        // }
        //Loading game scene and starting phase after initial display delay
        SceneManager.LoadScene("Game");
        StartCoroutine(NextPhaseCoroutine("enemy"));
        //StartCoroutine(SetLevelObjectsCoroutine());
    }

    // Adds given points to score after defeating an enemy 
    public void AddPoints(int points)
    {
        pointsCount += points;
    }
    // Called whenever an enemy is defeated
    public void CheckEnemies()
    {
        // Checking for phase or level change conditions
        if(enemyCounter.totalEnemies == 0){
            itemController.StopItems();        
            if(currentPhaseKey < currentLevel.levelPhases) {
                //levelDisplay.timePanel.SetActive(false);
                startingScroll = true;
                StartCoroutine(NextPhaseCoroutine("scroll"));
            } else { 
                Time.timeScale = 0;
                levelStarted = false;
                StartCoroutine(NextPhaseCoroutine("over"));
                // soundController.StopMusic();
                // levelStarted = false;
                // Time.timeScale = 0;
                // changingLevel = true;
            }
        }
    }
    //Removing lives when player is defeated and checking for Game Over by lives ran out
    public void PlayerDefeated()
    {
        livesCount -= 1;
        
        if(livesCount == 0){
            gameOver = true;
            Time.timeScale = 0;
            levelStarted = false;
            StartCoroutine(NextPhaseCoroutine("over"));
            //soundController.StopMusic();
        }
    }
    // Restart game variables after a Game Over screen
    public void ResetGame()
    {
        currentLevelKey = 0;
        //Hiding game over screen
        levelDisplay.ToggleGameOverScreen();
        //Destroying elements that are preserved through scenes so they aren't repeated
        //Destroy(soundController.gameObject);
        Destroy(mainCamera.gameObject);
        //Restarting time and loading title scene
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
        Destroy(gameObject);
    }
    //Finishing the map scrolling phase and starting an Enemy phase
    public void EndScrollPhase()
    {
        scrollPhase = false;
        // Stopping scroll mechanics
        scrollController.EndDisplacement();
        //levelDisplay.timePanel.SetActive(true);
        // Adding extra bonus time per phase
        timeCount += currentLevel.extraPhaseTime;
        // Setting boss battle if next Enemy phase is the last phase of the level
        if(currentPhaseKey == currentLevel.levelPhases) {
            bossPhase = true;
            //enemyCounter.SpawnBoss();
            //itemController.SetItemsForBoss(currentLevelKey);
        // Setting normal Enemy phase if not
        } 
        //else {
            // Restarting enemies and clearing items
            //enemyCounter.Start();
            //itemController.FlushItems();
        //}
        // Restarting item spawn
        //itemController.StartItems(5.0f);
        //Stopping time and setting coroutine for level-phase info display
        StartCoroutine(NextPhaseCoroutine("enemy"));
    }
    //Setting in-game object references. Using a coroutine to wait for objects to become available after scene change
    // private IEnumerator SetLevelObjectsCoroutine()
    // {
    //     //Checking if either of the references is missing to set them
    //     while(entryPoint == null || backgroundObject == null || playerObject == null || MDCObject == null) {
    //         entryPoint = GameObject.FindGameObjectWithTag("EntryPoint");
    //         backgroundObject = GameObject.FindGameObjectWithTag("Background");
    //         playerObject = GameObject.FindGameObjectWithTag("Player");
    //         MDCObject = GameObject.FindGameObjectWithTag("DisplacementController");

    //         yield return 0;
    //     }
    //     //After object references are set, get Script component references
    //     //player = playerObject.GetComponent<PlayerMovement>();
    //     //scrollController = MDCObject.GetComponent<MapDisplacementController>();
    //     //DOES THIS GO HERE???
    //     //scrollController.SetDisplacementObjects(this);
    // }

    // Holds screen or time on phase change:
    // Time is held while displaying level-phase info on phase start
    // Screen is held while starting a scroll phase so the player is not caught in scroll unfairly
    // Screen is held on game over to smoothen game over transition
    private IEnumerator NextPhaseCoroutine(string phaseType)
    {
        phaseChangeTimer = phaseChangeDuration;

        while (phaseChangeTimer > 0)
        {
            phaseChangeTimer -= Time.unscaledDeltaTime;
            
            yield return 0;
        }

        if(phaseType == "enemy") {
            levelStarted = true;
            Time.timeScale = 1;
            itemController.StartItems(5.0f);
        } else if(phaseType == "scroll") {
            if(scrollController == null) {
                scrollController = GameObject.FindGameObjectWithTag("ScrollController").GetComponent<MapScrollController>();
            }
            scrollPhase = true;
            currentPhaseKey += 1;
            startingScroll = false;
            scrollController.StartDisplacement();
        } else if(phaseType == "over") {
            levelDisplay.ToggleGameOverScreen();
        }
            
        phaseChangeTimer = 0.0f;
        changingLevel = false;
    }
}
