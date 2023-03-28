using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableItem : Item
{
    // First abstract item class: items consumed on touch
    
    protected abstract void ApplyEffect();

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.gameObject.tag == "Player") {
            //itemController.ItemGot();
            ApplyEffect();
            Vanish();
        } 
    }
}
