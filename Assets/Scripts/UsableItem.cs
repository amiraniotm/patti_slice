using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : Item
{
    // Abstract item class: items that stay on inventory and can be used

    // Editor vars: how much can an item be used, how long the effect lasts and if its used by keypress
    [SerializeField] public int maxUses;
    [SerializeField] public float useTime;
    [SerializeField] public bool keyTriggered;
    // Runtime vars to check item status and control uses
    public bool flippedHorizontal, onUse;
    public int usesLeft;
    // Inventory, animator and scale references (for items that change scale on use)
    public Inventory playerInventory;
    protected Animator animator;
    // Every usable item has an effect and a late update bind to follow player and others
    public abstract void UseEffect();

    protected abstract void LateUpdate();

    protected override void Awake()
    {
        base.Awake();

        playerInventory = player.gameObject.GetComponent<Inventory>();
        animator = GetComponent<Animator>();
        usesLeft = maxUses;
    }

    protected override void OnTriggerEnter2D(Collider2D otherCollider)
    {
        // If player collides with item and it hasnt been taken, take it
        if(otherCollider.gameObject.tag == "Player" && !wasTaken && !onUse) {
            //itemController.ItemGot();
            // Clear any item on inventory
            playerInventory.LoseItem();
            playerInventory.currentItem = this;
            wasTaken = true;  
            // Collider will be enabled on use
            itemCollider.enabled = false;
        // If enemy collides with item and player has it, defeat enemy
        } else if(otherCollider.gameObject.tag == "Enemies" && wasTaken) {
            //itemController.EnemyHit();
            Enemy enemyScript = otherCollider.gameObject.GetComponent<Enemy>();
            player.KickEnemy(enemyScript);
        } 
        // else if(otherCollider.gameObject.tag == "Bosses" && wasTaken) {
        //     //itemController.EnemyHit();
        //     Boss bossScript = otherCollider.gameObject.GetComponent<Boss>();
        //     bossScript.TakeDamage();
        // }
    }
    // Every item vanishes after running out of uses
    protected virtual void CheckUses()
    {
        if(usesLeft == 0) {
            playerInventory.LoseItem();
        }
    }
    // Overriding vanish to reset use variables
    public override void Vanish()
    {
        base.Vanish();

        onUse = false;
        usesLeft = maxUses;
        itemCollider.enabled = true;
        flippedHorizontal = false;
    }
    // Used to interrupt use in case of player defeat, etc
    public abstract void FinishUse();
    // Usage routine for effects extended on time
    protected abstract IEnumerator UsageCoroutine();
}
