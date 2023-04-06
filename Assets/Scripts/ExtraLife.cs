using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : ConsumableItem
{
    // Consumable item that gives one extra life
    
    protected override void ApplyEffect()
    {
        masterController.livesCount += 1;
    }
}
