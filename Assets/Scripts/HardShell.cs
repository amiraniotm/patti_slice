using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardShell : UsableItem
{
    // Hard shell protects player from one enemy hit

    // Use effect is called from player collision
    public override void UseEffect()
    {
        onUse = true;
        usesLeft -= 1;
        animator.SetTrigger("playerHit");
        StartCoroutine(UsageCoroutine());
    }
    // Follows player, and goes a bit up when used (to complement animation)
    protected override void LateUpdate()
    {
        if(wasTaken) {
            Vector3 newPos = transform.position;

            if(!onUse){
                newPos = new Vector3(
                            player.gameObject.transform.position.x,
                            player.gameObject.transform.position.y + (player.charCollider.size.y / 2),
                            player.gameObject.transform.position.z - 3
                            );
            }else{
                newPos = transform.position + new Vector3(0,0.05f,0);
            }

            transform.position = newPos;
        }
    }

    public override void FinishUse()
    {
        onUse = false;
        StopAllCoroutines();
        CheckUses();
    }
    // Giving some time gap so player isnt defeated by same hit that uses shell
    protected override IEnumerator UsageCoroutine()
    {
        yield return new WaitForSeconds(useTime);

        FinishUse();
    }
}
