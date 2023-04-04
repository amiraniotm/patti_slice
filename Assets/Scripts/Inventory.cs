using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Inventory class for storing player item (only one at a time)

    // Player and Item reference. Only UsableItems get stored on iventory
    private PlayerScript player;
    public UsableItem currentItem;

    private void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    private void Update()
    {
        // Checking for key input to use key-triggered items
        if(Input.GetKeyDown(KeyCode.C)) { 
            // Checking that item is assigned and not on use
            if(currentItem != null && !currentItem.onUse && currentItem.keyTriggered) {
                currentItem.UseEffect();
            } 
            // else if(player.currentProj != null) {
            //     player.ThrowProjectile();
            // }
        }
    }

    public void LoseItem()
    {
        // Lose item after uses ran out or player is defeated
        if(currentItem != null) {
            // If item on use, interrupt use
            if(currentItem.onUse) {
                currentItem.FinishUse();
            }
            currentItem.Vanish();
            currentItem = null;
        }

        // if(player.currentProj != null) {
        //     player.currentProj.pickedUp = false;
        //     player.currentProj.gameObject.SetActive(false);
        //     player.currentProj = null;
        // }
    }
}
