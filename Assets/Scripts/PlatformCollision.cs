using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformCollision : HittableBlock
{
    // Triggers platform "flipping" event when player hits from below

    private Tilemap tilemap;
    private TileController tileController;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tileController = GameObject.FindGameObjectWithTag("TileController").GetComponent<TileController>();
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        //Detecting if player hits platform from below and triggering flip event
        GameObject collidingObject = collision.gameObject;
        
        string collisionSide = DetectCollisionDirection(collision);
        
        if(collisionSide == "upper" && collidingObject.tag == "Player"){
            FlipTiles(collision);
        }
    }
    // Flip is controlled by a collider above player collision point. If enemy collides, it will be flipped. The collider disappears quickly
    private void FlipTiles(Collision2D collision)
    {
        tileController.SetFlipCollider(collision);      
    }
}
