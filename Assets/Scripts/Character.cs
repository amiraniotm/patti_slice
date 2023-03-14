using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    //Base class for Player, Enemies and Bosses

    //Movement variables serialized for easy adjust 
    [SerializeField] protected float maxWalkSpeed, maxJumpSpeed, upwardGravity, downwardGravity, spawnGravity;
    //[SerializeField] public EnemyCounter enemyCounter;
    //References for platform behavior, graphical and collision components for animations and screen wrapping
    protected PlatformCollision platforms;
    protected Renderer mainRenderer;
    protected BoxCollider2D charCollider; 
    protected SpriteRenderer spriteRenderer;
    protected ScreenWrap screenWrap;
    protected MasterController masterController;   
    protected Animator animator;
    public Rigidbody2D body;  
    //Storing initial position for respawn
    protected Vector2 initialPosition;
    //Runtime status and movement properties
    protected bool flippedHorizontal, isTripped, isJumping, isFalling;
    protected float adjustedJumpSpeed, adjustedWalkSpeed,  walkSpeedMod = 1.0f, jumpSpeedMod = 1.0f;
    public bool isDead, hasGhosts, isGrounded, isSpawning, isSpawned, onTop, onMid, onBot;
    
    protected virtual void Awake()
    {
        //Getting runtime component references
        body = GetComponent<Rigidbody2D>();
        charCollider = GetComponent<BoxCollider2D>();
        screenWrap = GetComponent<ScreenWrap>();
        mainRenderer = GetComponent<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        platforms = GameObject.FindGameObjectWithTag("Platforms").GetComponent<PlatformCollision>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
       
        initialPosition = transform.position;
        adjustedJumpSpeed = maxJumpSpeed;
        adjustedWalkSpeed = maxWalkSpeed;
    }
    //Base jump function. Checks for jump conditions and applies Y velocity if allowed, reduced speed for "death jump". Also marks character as airborne
    protected virtual void Jump()
    {
        adjustedJumpSpeed = jumpSpeedMod * maxJumpSpeed;

        if(isGrounded){
            body.velocity = new Vector2(body.velocity.x, adjustedJumpSpeed);
    
            isJumping = true;
            isGrounded = false;
        }
    }
    //Flips sprite and speed direction if character is walking against default sprite direction (default is left to right)
    protected virtual void FlipHorizontal()
    {
        flippedHorizontal = !flippedHorizontal;
        maxWalkSpeed *= -1;
        transform.localScale *= new Vector2(-1,1);
    }
    //For platform collision, marks as grounded if character collides from above
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //DO I NEED SEVERAL TYPE OF PLATFORM TAGS??
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform" || collision.gameObject.tag == "FloatingPlatform"){
            
            string collisionSide = platforms.DetectCollisionDirection(collision);
            
            if(collisionSide == "upper"){
                isJumping = false;
                isGrounded = true;    
            }    
            
            animator.SetBool("isGrounded", isGrounded); 
        }
    }
    //Marks as airborne when character falls off platform
    protected virtual void OnCollisionExit2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Platforms" || collision.gameObject.tag == "SpawnPlatform") {
            isGrounded = false;    
            animator.SetBool("isGrounded", isGrounded);
        }
    }
    //These two functions detect in what vertical third of the screen the character is. Mainly used for enemy respawn and boss action decision
    protected virtual void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "TopArea") {
            onTop = true;
        }

        if(otherCollider.gameObject.tag == "MidArea") {
            onMid = true;
        }

        if(otherCollider.gameObject.tag == "BotArea") {
            onBot = true;
        }

    }

    protected virtual void OnTriggerExit2D(Collider2D otherCollider) 
    {
        if(otherCollider.gameObject.tag == "TopArea") {
            onTop = false;

        }   

        if(otherCollider.gameObject.tag == "MidArea") {
            onMid = false;

        }
        
        if(otherCollider.gameObject.tag == "BotArea") {
            onBot = false;

        }
    }
    //Stops horizontal movement. Used mainly when NPCs are airborne
    protected virtual void StopWalking()
    {
        body.velocity = new Vector2(0, body.velocity.y);
    }
    //Stops all movement. Mainly for character death
    protected virtual void Hold()
    {
        body.velocity = new Vector2(0, 0);
    }
    //Called when Character has left screen
    public abstract void TriggerOffScreen();
    
    //CHECK IF CAN BE USED FOR LIZARD AND PLAYER!! 
    // protected IEnumerator UntripCoroutine()
    // {
    //     float tripCount = 0.5f;

    //     while(tripCount > 0){
    //         tripCount -= Time.deltaTime;

    //         yield return 0;
    //     }

    //     isTripped = false;
    // }
}
