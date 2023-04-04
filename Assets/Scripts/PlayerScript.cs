using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerScript : Character
{
    // Player Character control script

    // References for objects which with player interacts
    [SerializeField] private PlayerSpawnPlatform spawnPlatform;
    [SerializeField] private TileController tileController;
    [SerializeField] private PauseController pauseController;
    // Sound references for SFX
    [SerializeField] private AudioClip jumpSound;
    //[SerializeField] private AudioClip enemyCollisionSound;
    // Time that player can be airborne, for extended jump control, and time spawn platform is active on itself
    [SerializeField] private float maxJumpTime;
    //public Projectile currentProj;
    // Variables for ground and platform edge detection
    protected RaycastHit2D groundHit;
    protected Vector2 raycastOrigin, raycastDirection;
    protected float raycastMaxDistance;
    // Runtime components
    private Inventory inventory; 
    // Status variables for item and platform effect
    public bool isShelled, onIce;
    private float currentJumpTimer; 
    
    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory>();
    }
    
    private void Start()
    {                
        Spawn();
    }

    private void Update()
    {      
        //Ignoring all input and movement if game is paused
        if(!pauseController.gamePaused) {
            //Checking if player is on spawn movement (blocks input)
            if(!isSpawning){            
                //Checking ground each frame for tile effects
                SetGroundRaycast();
                CheckGround();
                //Catching inputs only if player is alive
                if(!isDead){
                    //Spacebar pressed input to start extended jump
                    if(Input.GetKey("space")) {
                        Jump();
                    } 
                    //Spacebar UP input to stop extended jump
                    if (Input.GetKeyUp("space")) {
                        isJumping = false;
                    }
                    //Walking mechanics: velocity and sprite flipping
                    float horizontalInput = Input.GetAxis("Horizontal");
                    SetInputVelocity(horizontalInput);
                    FlipHorizontal(horizontalInput);
                }
                //Variable gravity mechanics for more "natural" jump (less "floaty")
                if(body.velocity.y < 0) {
                    body.gravityScale = downwardGravity;
                    isFalling = true;
                } else {
                    body.gravityScale = upwardGravity;
                    isFalling = false;
                }
            //Player spawning movement (linear displacement along with spawn platform)
            } else {
                body.gravityScale = spawnGravity;
            }
            //Setting animation parameters after all movement has been resolved
            bool isWalking = (body.velocity.x > 1.0f || body.velocity.x < -1.0f) && isGrounded;
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isFalling", isFalling && !isGrounded);
            animator.SetBool("isDead",isDead);
        }  
    }

    protected void SetInputVelocity(float horizontalInput)
    {
        //Detecting movement after spawning to hide platform
        if(horizontalInput != 0 && !isSpawned) {
            spawnPlatform.DoHide();
        }
        //Moving if not airborne and not on iced platform
        if(!onIce) {
            body.velocity = new Vector2(horizontalInput * adjustedWalkSpeed, body.velocity.y);
        } 
    }

    protected void FlipHorizontal(float horizontalInput)
    {
        //Flipping player sprite when walking. Using 0.01 for input tolerance
        if((horizontalInput > 0.01f && !flippedHorizontal) || (horizontalInput < -0.01f && flippedHorizontal)){
              transform.localScale *= new Vector2(-1,1);
            flippedHorizontal = !flippedHorizontal;
        }

        // Flipping item if any on inventory
        if(inventory.currentItem != null && !inventory.currentItem.onUse) {
            if((!flippedHorizontal && !inventory.currentItem.flippedHorizontal) || (flippedHorizontal && inventory.currentItem.flippedHorizontal)) {
                inventory.currentItem.transform.localScale *= new Vector2(-1,1);
                inventory.currentItem.flippedHorizontal = !inventory.currentItem.flippedHorizontal;
            }
        }
    }

    protected override void Jump()
    {
        //Initial jump with sound and extended jump setup
        if( isGrounded && !isJumping ) {
            //masterController.soundController.PlaySound(jumpSound, 0.15f);
            currentJumpTimer = maxJumpTime;
            base.Jump();
            animator.SetTrigger("doJump");
        //Extended jump if space key is still pressed
        }else if(isJumping){
            //Consuming extended jump time and setting jump variable to false if jump time ran out
            if( currentJumpTimer > 0 ) {
                body.velocity = Vector2.up * adjustedJumpSpeed * (1.2f + currentJumpTimer);
                currentJumpTimer -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        animator.SetBool("isGrounded", isGrounded);

        if(!isSpawned){
            spawnPlatform.DoHide();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) 
    {
        base.OnCollisionEnter2D(collision);
        // Checking for Enemy collision
        if(collision.gameObject.tag == "Enemies") {
            Enemy collidingEnemy = collision.gameObject.GetComponent<Enemy>();
            //masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);
            // If enemy is flipped, it can be kicked
            if(collidingEnemy.flippedVertical) {
                KickEnemy(collidingEnemy);
            // If enemy is NOT flipped, check for protection item before resolving dead
            }else if(!collidingEnemy.isDead) {
                CheckForShell();
            }
        } else if ( collision.gameObject.tag == "Waves" || collision.gameObject.tag == "Bosses" ) {
           CheckForShell(); 
        }
        // else if ( collision.gameObject.tag == "Projectiles" ) {
        //     Projectile hitProj = collision.gameObject.GetComponent<Projectile>();

        //     if((!hitProj.deactivated && hitProj.thrown && !hitProj.trippable) || !hitProj.throwable) {
        //         CheckForShell(); 
        //     } else if (hitProj.isGrounded && hitProj.thrown && hitProj.trippable) {
        //         hitProj.charCollider.enabled = false;
        //         hitProj.isGrounded = false;
        //         isTripped = true;
        //         StartCoroutine(UntripCoroutine());  
        //     } else if (!hitProj.trippable) {    
        //         PickUpProjectile(hitProj);
        //     } 
    }

    public void KickEnemy(Enemy collidingEnemy)
    {
        collidingEnemy.TriggerDefeat(body);
        animator.SetTrigger("kick");
    }

    private void CheckForShell()
    {
        // Checking if player currently has shell item to defend from collision
        if(inventory.currentItem != null && inventory.currentItem.itemName == "HardShell") {
            inventory.currentItem.UseEffect();
        // If not, player is defeated
        } else {
            TriggerDefeat();
        }
    }

    public void TriggerDefeat()
    {   
        // Marking dead status and doing animation
        isDead = true;
        animator.SetTrigger("died");
        //inventory.LoseItem();
        Hold();
        base.Jump();
        // Disabling collider so player goes off-screen
        charCollider.enabled = false;
    }

    // private void PickUpProjectile(Projectile proj)
    // {
    //     inventory.LoseItem();
    //     currentProj = proj;
    //     currentProj.OnPickup();
    // }

    // public void ThrowProjectile()
    // {
    //     float projXSpeed = 3.0f * body.velocity.x;
    //     currentProj.body.velocity = new Vector2(projXSpeed, 0);
    //     currentProj.telegraphed = true;
    //     currentProj.thrown = true;
    //     currentProj.pickedUp = false;
    //     currentProj = null;
    // }

    private void Spawn() 
    {
        // Setting layer to "spawn" so player goes through ceiling
        int spawnLayer = LayerMask.NameToLayer("Spawn");  
        SetLayer(spawnLayer);
        // Resetting player state
        isSpawning = true;
        isSpawned = false;
        isDead = false;
        isFalling = false;
        isGrounded = true;
        transform.position = initialPosition;
        charCollider.enabled = true;
        animator.SetTrigger("spawn");
        // Starting spawn platform movement
        spawnPlatform.SetSpawnCounter();
    }

    public void FinishSpawn()
    {
        // Setting player to its regular layer
        isSpawned = true;
        int defaultLayer = LayerMask.NameToLayer("Default");  
        SetLayer(defaultLayer);
    }
    //Setting Raycast for ground detection. It points downwards, starting at a distance from front or back and extending a bit outside bottom of sprite
    protected void SetGroundRaycast()
    {
        raycastOrigin = transform.position;
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = 2 * charCollider.bounds.extents.y;
    }
    //Checking which tile type is below player each frame
    private void CheckGround()
    {
        //Raycast for platforms only
        groundHit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastMaxDistance, LayerMask.GetMask("Platforms"));
        //Debug.DrawRay(raycastOrigin, raycastDirection * raycastMaxDistance, Color.red );
        //If tile is underneath, get speed modifier from tile and set movement vars
        if(groundHit){
            walkSpeedMod = tileController.GetTileSpeedMod(groundHit.point);

            if(walkSpeedMod > 0) {
                onIce = false;
            } else {
                walkSpeedMod = 1.0f;
                onIce = true;
            }
        } else {
            walkSpeedMod = 0.65f;
        }       

        adjustedWalkSpeed = maxWalkSpeed * walkSpeedMod;
    }

    public override void TriggerOffScreen()
    {
        //if player left the screen marked as dead, complete death cycle
        if(isDead) {
            masterController.PlayerDied();
            spawnPlatform.gameObject.SetActive(true);
            Spawn();
        //if alive, not spawning or not in scrollPhase; swap with ghost
        } else if(!isSpawning && !masterController.scrollPhase) {
            screenWrap.GhostSwap();
        }
    }

    // private new void OnTriggerEnter2D(Collider2D otherCollider)
    // {
    //     if(otherCollider.gameObject.tag == "PlayArea") {
    //         int defaultLayer = LayerMask.NameToLayer("Default"); 
    //         SetLayer(defaultLayer);
    //     }
    // }
    // Called to move player between layers according to state or level phase
    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
    }
}
