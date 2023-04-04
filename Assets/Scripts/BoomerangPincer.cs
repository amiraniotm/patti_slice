using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoomerangPincer : UsableItem
{
    // A pincer that is launched horizontally, holds in farthest point and comes back, hits everything in its way!

    // Movement control variables to config on editor (speed and farthest point time)
    [SerializeField] private float throwSpeed, holdTime;
    // 
    private int directionMod;
    private float throwCount, initialPlayerXVelocity;
    private bool comeBack, onHold;
    // Body must be added to this item for efficient velocity manipulation
    private Rigidbody2D body;

    protected override void Awake()
    {
        base.Awake();

        body = GetComponent<Rigidbody2D>();
    }

    public override void UseEffect()
    {
        // Equating the counter to the use time for airborne countdown
        throwCount = useTime;
        onUse = true;
        itemCollider.enabled = true;
        usesLeft -= 1;
        animator.SetBool("isAirborne", true);
        // Considering player x velocity to affect throw velocity
        if(Math.Abs(player.body.velocity.x) > 0.5f) {
            initialPlayerXVelocity = Math.Abs(player.body.velocity.x) + (throwSpeed / 2);
        } else {
            initialPlayerXVelocity = throwSpeed;
        }
    }
    // Overriding colliding function to finish use when pincer gets back to player
    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        base.OnTriggerEnter2D(otherCollider);

        if(otherCollider.gameObject.tag == "Player" && onUse && comeBack) {
            FinishUse();
        }
    }

    protected void Update()
    {
        if(onUse) {
            // Throw counter controls airborne time
            throwCount -= Time.deltaTime;
            // Reducing player velocity influence over time (kind of air friction)
            initialPlayerXVelocity -= 0.05f * Time.deltaTime;
            // Going away from player
            if(!comeBack && !onHold) {
                body.velocity = new Vector2(directionMod * (initialPlayerXVelocity + throwSpeed), body.velocity.y);
            // Coming back to player
            } else if(comeBack && !onHold) { 
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.5f);
            // Holding on farthest point from player
            } else if(onHold) {
                body.velocity = Vector2.zero;
            }
        }
    }

    protected override void LateUpdate()
    {
        if(wasTaken) {
            animator.SetBool("flipped", flippedHorizontal);
            // Direction modifier to adjust speed (left or right bound)
            if(flippedHorizontal) {
                directionMod = -1;
            } else {
                directionMod = 1;
            }
            // Following player only if not in use
            if(!onUse && player != null){
                Vector3 newPos = new Vector3(
                            player.gameObject.transform.position.x - (player.charCollider.size.x * player.transform.localScale.x / 3),
                            player.gameObject.transform.position.y + (player.charCollider.size.y * player.transform.localScale.y / 3),
                            player.gameObject.transform.position.z - 3
                        );
                
                transform.position = newPos;
            // Marking farthest point at half the use time
            } else {
                if((throwCount < (useTime / 2)) && !onHold && !comeBack) {
                    onHold = true;
                    StartCoroutine(UsageCoroutine());
                }
            }
            
        }
    }
    // Resetting use variables when pincer comes back to player
    public override void FinishUse()
    {
        onUse = false;
        itemCollider.enabled = false;
        comeBack = false;
        onHold = false;
        animator.SetBool("isAirborne", false);
        CheckUses();
        StopAllCoroutines();
    }
    // Marks return after hold condition
    protected override IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(holdTime);

        comeBack = true;
        onHold = false;
        animator.SetTrigger("comeBack");
    }
}
