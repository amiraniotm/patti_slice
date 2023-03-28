using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : ConsumableItem
{
    protected override void ApplyEffect()
    {
        masterController.livesCount += 1;
    }
}
