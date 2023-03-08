using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    //Movement of ghosts, which are simple clones of character sprites for smooth screen wrap-around

    //Reference to the wrapping script, the "live" object and script, and renderers for both ghost and original
    public ScreenWrap screenWrap;
    public GameObject originalObject;
    private Character originalCharacter;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer originalSpriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Called from original object after it creates the ghosts and gives em reference
    public void GetOgComponents()
    {
        originalCharacter = originalObject.GetComponent<Character>();
        originalSpriteRenderer = originalObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Following sprite if alive, and self-destructing with original
        if(originalObject != null) {
            FollowOriginal();
        } 
    }

    private void FollowOriginal()
    {
        Vector2 newPosition = transform.position;
        //Preventing ghosts from following when original is unassigned, dead or spawning
        if(originalCharacter != null && !originalCharacter.isDead && !originalCharacter.isSpawning &&
            //Preventing ghosts from following enemies when they go off-screen on bottom part of the map (enemy respawn)
            (originalObject.tag != "Enemies" || (originalObject.tag == "Enemies" && !originalCharacter.onBot))) {
            //Setting ghosts at screen distance from original
            if(transform.position.x > originalObject.transform.position.x){
                newPosition.x = originalObject.transform.position.x + screenWrap.screenWidth;
            } else if(transform.position.x < originalObject.transform.position.x){
                newPosition.x = originalObject.transform.position.x - screenWrap.screenWidth;
            }
            //Y-follow is simple because theres no Y wrap-around
            newPosition.y = originalObject.transform.position.y;
            //Assigning position and graphic properties so Ghost is exactly like original
            transform.position = newPosition;
            transform.rotation = originalObject.transform.rotation;
            transform.localScale = originalObject.transform.localScale;
            spriteRenderer.sprite = originalSpriteRenderer.sprite;
        }
    }
}
