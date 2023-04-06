using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClumsyEnemy : Enemy
{
    // Clumsy enemies are fast, but will auto-flip after a certain amount of time

    // Variables for trip control
    [SerializeField] private float tripTime;
    private float tripCount;
    
    protected override void Update()
    {
        // Counting time to trip
        if(!isSpawning && !isTripped && !flippedVertical) {
            tripCount += Time.deltaTime;
        }
        // Counter runs out, mark for trip
        if(tripCount >= tripTime) {
            isTripped = true;
        }
        // Conditions to ensure trip is allowed only on land 
        if(isGrounded && isTripped && !flippedVertical) {
            FlipVertical();
        }

        base.Update();
    }
    // Overriding flipping funct to reset trip markers
    public override void FlipVertical()
    {
        isTripped = false;
        tripCount = 0.0f;
        
        base.FlipVertical();
    }
    // Overriding unflip so it runs in the opposite direction when unflips
    protected override void Unflip()
    {
        FlipHorizontal();

        base.Unflip();
    }


}
