using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : Character
{
    // Class for all charactes that can harm the player, except bosses. Enemies are flipped (stopped and vulnerable to collision) when hit from below platform by player

    // Name and how many points is an Enemy worth
    [SerializeField] public int bounty;
    [SerializeField] protected float spawningTime, unflipTime;
    // Coroutine references for recovery events, to make sure only one is happening at a time
    private Coroutine lastUnflipCoroutine, lastShakeCoroutine;
    // Position reference to control shake center (enemy shakes before unflipping), and enemy counter reference
    protected Vector2 initialShakePosition;
    protected EnemyCounter enemyCounter;
    // Status variables
    public bool canHover, isShaking, flippedVertical;
    protected float shakeMagnitude = 0.05f;
    
    protected override void Awake()
    {      
        base.Awake();
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
    }

    public virtual void Spawn()
    {
        // Adding enemy to list of active enemies
        if(!enemyCounter.currentEnemies.Contains(this)){
            enemyCounter.currentEnemies.Add(this);
        }
        // Resetting control variables
        isSpawning = true;
        charCollider.enabled = true;
        isDead = false;
        flippedVertical = false;
        onTop = true;
        onBot = false;
        // Flipping animation to fit walking direction
        if(transform.position.x > 0 && maxWalkSpeed > 0){
            FlipHorizontal();
        }
        
        StartCoroutine(FinishSpawnCoroutine());
    }

    protected virtual void Update()
    {
        if(isSpawning) {
            // Suspending enemy and moving it slowly while it spawns
            body.gravityScale = 0.0f;
            walkSpeedMod = 0.3f;
        } else if(!isSpawning) {
            // Increasing gravity on falls for less "floaty" fall
            if(body.velocity.y < -0.1) {
                body.gravityScale = downwardGravity;
            // Setting regular gravity otherwise
            } else {
                body.gravityScale = upwardGravity;
            }
        } else if (isDead) {
            // Increading gravity drastically for enemy to leave screen fast when dead
            body.gravityScale = spawnGravity;
        }
        // Checking movement conditions to call move functions for ground-based enemies
        if(!flippedVertical && !canHover && !isDead && (isGrounded || isFalling)){
            Walk();
        }
        // Setting animator variables after movement is resolved
        animator.SetBool("flippedVertical",flippedVertical);
        animator.SetBool("isGrounded", isGrounded);
    }

    protected void Walk()
    {
        body.velocity = new Vector2(maxWalkSpeed * walkSpeedMod, body.velocity.y);
    }
    
    public override void TriggerOffScreen()
    {
        //if enemy left the screen marked as dead, complete death cycle
        if(isDead) {
            Vanish();
        //if alive and NOT on bot platform, wraparound screen. If on bot, respawn
        } else if(!isSpawning) {
            if(!onBot) {
                screenWrap.GhostSwap();
            } else {
                Respawn();
            }
        }
    }

    protected void Respawn()
    {
        // Adding self to enemy counter queue
        string cleanName = gameObject.name.Replace("(Clone)", "");
        enemyCounter.currentEnemies.Remove(this);
        enemyCounter.spawnQ.Add(cleanName);
        gameObject.SetActive(false);
    }
    
    protected override void OnCollisionExit2D(Collision2D collision) 
    {        
        base.OnCollisionExit2D(collision);
        // Lowering x velocity modifier on fall to avoid "flying" effect
        if(collision.gameObject.tag == "Platforms"){
            walkSpeedMod = 0.5f;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) 
    {        
        base.OnCollisionEnter2D(collision);
        // Resetting x velocity modifier when grounded again
        if(collision.gameObject.tag == "Platforms"){
            walkSpeedMod = 1.0f;
        } 
    }   
    // Called when platform below enemy is hit by player
    public virtual void FlipVertical()
    {
        // Stop walking and Jump
        Hold();
        Jump();
        // Checking if already flipped
        if(!flippedVertical) {
            // Flipping if not, and start unflipping mechanisms
            flippedVertical = true;
            animator.SetTrigger("flip");
            lastUnflipCoroutine = StartCoroutine(UnflipCoroutine());
            lastShakeCoroutine = StartCoroutine(ShakeCoroutine());
        } else {
            // Unflipping if already flipped
            Unflip();
        }
    }
    // Starts the shaking movement when enemy is near unflipping
    protected void StartShaking()
	{
        initialShakePosition = transform.position;
		InvokeRepeating ("Shake", 0f, 0.3f);
	}
    // Alters enemy position for shake effect if enemy has not been killed
    protected void Shake()
	{
        if(!isDead) {
            if( transform.position.x >= initialShakePosition.x ){
                transform.position = new Vector2(initialShakePosition.x - shakeMagnitude, initialShakePosition.y);
            }else if( transform.position.x < initialShakePosition.x ){
                transform.position = new Vector2(initialShakePosition.x + shakeMagnitude, initialShakePosition.y);
            }
        }
	}

	protected void StopShaking()
	{
		CancelInvoke ("Shake");
		transform.position = initialShakePosition;
        isShaking = false;
	}
    // Unflips when flipped enemy is hit from below, or when flipped time runs out
    protected virtual void Unflip()
    {
        StopCoroutine(lastUnflipCoroutine);
        StopCoroutine(lastShakeCoroutine);
        if(isShaking){
            StopShaking();
        }
        flippedVertical = false;
    }
    // Called when flipped enemy is kicked
    public void TriggerDefeat(Rigidbody2D playerBody)
    {
        if(isShaking){
            StopShaking();
        }
        // Disabling collider so enemy falls off screen
        isDead = true;
        charCollider.enabled = false;
        // Setting velocity equal to player's so it follows kick direction
        body.velocity = new Vector2(playerBody.velocity.x * 2, playerBody.velocity.y);
    }
    // Removes enemy when it leaves the screen while dead
    protected void Vanish()
    {
        gameObject.SetActive(false);
        enemyCounter.EnemyDied(this);
    }

    // protected void AdjustCollider()
    // {
    //     Vector3 newSize = new Vector3 ( mainRenderer.bounds.size.x / Math.Abs(transform.localScale.x),
    //                                     mainRenderer.bounds.size.y / Math.Abs(transform.localScale.y),
    //                                     mainRenderer.bounds.size.z / Math.Abs(transform.localScale.z) );

    //     collider.size = newSize;
    // }
    // Restarts normal movement after spawning is over
    private IEnumerator FinishSpawnCoroutine()
    {
        yield return new WaitForSeconds(spawningTime);

        isSpawning = false;
    }
    // Starts shaking when enemy is about to unflip on its own
    private IEnumerator ShakeCoroutine()
    {
        float shakeStart = 2 * unflipTime / 3;
        
        yield return new WaitForSeconds(shakeStart);

        if(!isShaking && !isDead && flippedVertical){
            StartShaking();
            isShaking = true;
        }

    }
    // Unflips enemy on its own
    private IEnumerator UnflipCoroutine()
    {
        yield return new WaitForSeconds(unflipTime);

        if(flippedVertical && !isDead){
            Jump();
            Unflip();
        }
    }
}
