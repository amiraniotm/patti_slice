using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTime : ConsumableItem
{
    // Consumable item that gives some editor-configurable extra time
    [SerializeField] protected int timeAmount;

    protected override void ApplyEffect()
    {
        masterController.timeCount += timeAmount;
    }
}
