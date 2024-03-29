using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    // Abstract item class: items with one-time effect, consumed on touch
    
    protected abstract void ApplyEffect();

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            //itemController.ItemGot();
            wasTaken = true;
            ApplyEffect();
            Vanish();
        } 
    }
}
