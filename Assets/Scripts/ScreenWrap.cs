using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    // Script that controls the wrap-around of characters leaving the screen and other on screen exit events

    // "Ghosts" are used to make the wrap-around smooth. 
    [SerializeField] private Transform ghostPrefab;
    // Editor vars for non-character sprites, sprites that don't use ghosts, and for sprites that disappear on their own and not when leaving the screen
    [SerializeField] private bool isCharacter, doesLeave, doesDestroy;
    // Control variable for out-of-screen
    public bool isVisible;
    // References to camera for size and sprite dimensions and scripts
    protected CameraMovement mainCamera;
    protected Renderer[] renderers;
    protected Character characterScript;
    public float screenWidth;
    public float screenHeight;
    // Stores ghosts. Since Im only doing X wrap-around, we only need two of them
    protected Transform[] ghosts = new Transform[2];
    
    private void Awake()
    {
        // Getting all renderers in sprite for checkups
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        // Calculating screen dimensions
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();

        screenWidth = mainCamera.screenWidth;
        screenHeight = mainCamera.screenHeight;
        // Spawning ghosts if marked as character
        if(isCharacter) {
            characterScript = GetComponent<Character>();
            CreateGhosts();
        }
    }

    private void Update()
    {
        // Marking if sprite is on-screen
        isVisible = CheckRenderers();
        // Checking wrap and screen-off events if the sprite is not a character, or if character sprite is not spawning; 
        // and only if sprite went off-screen
        if((!isCharacter || !characterScript.isSpawning) && !isVisible) {
            CheckScreenWrap();
        }
    }

    protected void CheckScreenWrap()
    {
        // Characters handle their own off-screen
        if(isCharacter) {
            characterScript.TriggerOffScreen();
        // If marked as leave, object is disabled
        } else if(!doesLeave) {
            gameObject.SetActive(false);
        // And destroyed if marked as selfdestruct
        } else if (doesDestroy) {
            Destroy(gameObject);
        }
    }

    private bool CheckRenderers()
    {
        foreach(var renderer in renderers)
        {
            // If at least one render is visible, return true
            if(renderer.isVisible)
            {
                return true;
            }
        }
        // Otherwise, the object is invisible
        return false;
    }
    // Called from Character original objects to create the ghosts used on screen wrap-around
    public void CreateGhosts()
    {
        // Again, only 2 ghosts because we only do X wrap-around
        for(int i = 0; i < 2; i++)
        {
            ghosts[i] = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
            GhostMovement newGhost = ghosts[i].GetComponent<GhostMovement>();
            //Storing "live" versions of the sprite for ghost to follow
            newGhost.originalObject = gameObject;
            newGhost.screenWrap = this;
            newGhost.GetOgComponents();
        }
        
        PositionGhosts();
    }

    private void PositionGhosts()
    {
        // All ghost positions will be relative to the sprites (this) transform
        // These vectors position ghosts behind the edges of the screen.
        Vector2 rightGhostPosition = new Vector2(transform.position.x + screenWidth, transform.position.y);
        Vector2 leftGhostPosition = new Vector2(transform.position.x - screenWidth, transform.position.y);

        // Right
        ghosts[0].position = rightGhostPosition;
        // Left
        ghosts[1].position = leftGhostPosition;
    }

    public void GhostSwap()
    {
        // Checking which ghosts' middle point has entered the screen in order to swap
        foreach(var ghost in ghosts) {
            if (ghost.position.x < screenWidth && ghost.position.x > -screenWidth) {
                transform.position = ghost.position;
    
                break;
            }
        }
    }
}
