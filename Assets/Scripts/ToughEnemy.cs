using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToughEnemy : Enemy
{
    // Enemies that change state on first hit and only get flipped on second hit

    // Adjustable variables: time to change, time it remains altered, marking if explodes and waves for exploding ones
    [SerializeField] protected float changeTime, madTime;
    [SerializeField] protected bool explodes;
    [SerializeField] private GameObject wavePrefab;
    // Control variables for change and state behaviors
    protected bool doChange, isChanging, isMad, doExplode;
    protected float changeCount;

    public override void Spawn()
    {
        // Overriding spawn function to reset state variables
        base.Spawn();
        if(!explodes) {
            isMad = false;
            animator.SetBool("isMad",isMad);
        }
    }

    protected override void Update()
    {
        // Overriding update to trigger state change mid-air after hit when falling
        if(doChange && body.velocity.y < -0.05){
            doChange = false;
            isChanging = true;
            changeCount = changeTime;
            StartCoroutine(ChangeCoroutine());
        }
        // Modifying speeds for mad state, mid-air state change, and regular walk
        if(isMad) {
            walkSpeedMod = 1.5f;
        } else if(isChanging || doExplode) {
            walkSpeedMod = 0.0f;
        } else {
            walkSpeedMod = 1.0f;
        }
        // Only for exploding enemies, explode only when in ground
        if(isGrounded && doExplode) {
            doExplode = false;
            animator.SetTrigger("explode");
            StartCoroutine(ExplodeCoroutine());
        }

        base.Update();
    }  
    // Readies state change on first hit, flips on second
    public override void FlipVertical()
    {
        if(!isMad) {
            Hold();
            Jump();
            doChange = true;
            animator.SetTrigger("change");
        } else {
            base.FlipVertical();
        }
    }
    // Overriding Unflip to reset mad state
    protected override void Unflip()
    {
        base.Unflip();
        isMad = false;
        animator.SetBool("isMad", isMad);
    }
    // Explodes and sets out ElementWaves which alter tiles and apply movement debuffs to player
    protected void Explode()
    {
        // One wave to the right of enemy, one to left
        GameObject waveR = Instantiate(wavePrefab, transform.position, Quaternion.identity);
        GameObject waveL = Instantiate(wavePrefab, transform.position, Quaternion.identity);
        // For left wave edge checkup, invert which way is "front"
        ElementWave waveScript = waveL.GetComponent<ElementWave>();
        waveScript.edgeChecker.direction = "left";
        // Disappear after spawning waves
        Vanish();
    }

    protected IEnumerator ChangeCoroutine()
    {
        // Holding enemy in mid-air while changing so its not so easy to hit them twice in a row
        while(changeCount > 0) {
            body.gravityScale = 0.0f;
            walkSpeedMod = 0.0f;
            changeCount -= Time.deltaTime;
            Hold();

            yield return 0;
        }
        // Setting mad state if not mad
        if(!isMad){
            isMad = true;
            body.gravityScale = 1.0f;
            changeCount = 0.0f;
            isChanging = false;
            animator.SetBool("isMad",isMad);
            //AdjustCollider();
            // Starting auto-state reset coroutine 
            StartCoroutine(CalmDownCoroutine());
        // If already mad and "exploding" enemy, then second hit is instant vanish
        } else if(isMad && explodes) {
            Vanish();
        }
    }

    protected IEnumerator CalmDownCoroutine()
    {
        yield return new WaitForSeconds(madTime);
        // Only resetting state if enemy hasnt been hit again or beaten
        if(!flippedVertical && !isDead){
            // If not exploding enemy, unset mad state
            if(!explodes){
                isMad = false;
                animator.SetBool("isMad",isMad);
            // Otherwise, stop enemy and mark it ready to explode
            }else{
                Hold();
                doExplode = true;
                isDead = true;
            }
        }
    }

    protected IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(changeTime);

        Explode();
    }
}
