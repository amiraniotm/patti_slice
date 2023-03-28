using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTime : ConsumableItem
{
    [SerializeField] protected int timeAmount;

    protected override void ApplyEffect()
    {
        masterController.timeCount += timeAmount;
    }
}
