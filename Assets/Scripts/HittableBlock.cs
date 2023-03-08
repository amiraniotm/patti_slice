using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HittableBlock : MonoBehaviour
{
    //Base class for Platforms and FlipBoxes. Detects collision direction

    //Determinining if first contact point with block is made upwards, downwards, or sidewards, using contact average Y normal
    public virtual string DetectCollisionDirection(Collision2D collision) 
    {
        string side;
        float avgNormal = 0.0f;
        ContactPoint2D[] blockHits = collision.contacts;
        
        for (int i = 0; i < blockHits.Length; i++)
        {
            avgNormal += blockHits[i].normal.y;
        }
        avgNormal /= blockHits.Length;

        if(avgNormal > 0.5 || avgNormal < -0.5) {
            if(avgNormal > 0){
                side = "upper";
            }else if(avgNormal < 0){
                side = "lower";
            }else{
                side = "???";
            }
        }else{
            side = "side";
        }
        
        return side;
    }
}
