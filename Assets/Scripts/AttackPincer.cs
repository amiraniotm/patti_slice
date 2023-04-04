using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPincer : UsableItem
{
    // Usable item that attacks in the imediate front of player

    // Marking use and doubling scale so its easier to use
    public override void UseEffect()
    {
        onUse = true;
        animator.SetTrigger("attack");
        transform.localScale *= 2.0f;
        itemCollider.enabled = true;
        usesLeft -= 1;
        StartCoroutine(UsageCoroutine());
    }
    // Sets animator variables and follows player position
    protected override void LateUpdate()
    {
        if(wasTaken) {
            animator.SetBool("flipped", flippedHorizontal);
            
            Vector3 newPos = new Vector3(
                            player.gameObject.transform.position.x - (player.charCollider.size.x * player.transform.localScale.x),
                            player.gameObject.transform.position.y,
                            player.gameObject.transform.position.z - 3
                            );

            transform.position = newPos;
        }
    }
    // Resetting use variables, checking uses left
    public override void FinishUse()
    {
        onUse = false;
        transform.localScale /= 2.0f;
        itemCollider.enabled = false;
        CheckUses();
        // Stopping all coroutines in case the use was interrupted from outside
        StopAllCoroutines();
    }

    protected override IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        FinishUse();
    }

}
