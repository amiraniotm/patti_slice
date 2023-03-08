using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeChecker : MonoBehaviour
{
    // Script to detect platform edges 

    // Adjustables: how far is raycast and how deep it goes
    [SerializeField] private float raycastXGap, raycastYMod;
    // Raycast parameters, set according to sprite size at runtime
    public Vector2 frontRaycastOrigin, backRaycastOrigin, raycastDirection;
    public float frontFace, backFace, raycastMaxDistance;
    // Renderer to get sprite dimensions
    protected Renderer mainRenderer;
    // Platform layer to condition hit to only this layer
    private int platformLayer;
    // Direction to establish which way is front and back
    public string direction = "right";

    private void Awake()
    {
        platformLayer = LayerMask.GetMask("Platforms");
        mainRenderer = GetComponent<Renderer>();

        SetEdgeRaycast();
    }

    private void Update()
    {
        // Updating raycast position each frame
        SetEdgeRaycast();
    }

    protected void SetEdgeRaycast()
    {
        float size = mainRenderer.bounds.size.x;
        // Setting front and back coordinates according to move direction
        if(direction == "left"){
            frontFace = mainRenderer.bounds.min.x - raycastXGap;
            backFace = mainRenderer.bounds.max.x + raycastXGap;
        } else {
            frontFace = mainRenderer.bounds.max.x + raycastXGap;
            backFace = mainRenderer.bounds.min.x - raycastXGap;
        }
        // Setting raycast: it goes from Y center of sprite, at the calculated X distance for front and back; downwards and extends according to sprite size
        frontRaycastOrigin = new Vector2(frontFace, mainRenderer.bounds.center.y);
        backRaycastOrigin = new Vector2(backFace, mainRenderer.bounds.center.y);
        raycastDirection = transform.TransformDirection(Vector2.down);
        raycastMaxDistance = raycastYMod * mainRenderer.bounds.extents.y;
    }
    // Functions to return front and back hit
    public RaycastHit2D CheckBack()
    {
        RaycastHit2D backEdgeHit = Physics2D.Raycast(backRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        Debug.DrawRay(backRaycastOrigin, raycastDirection * raycastMaxDistance, Color.red );
        
        return backEdgeHit;
    }

    public RaycastHit2D CheckFront()
    {
        RaycastHit2D frontEdgeHit = Physics2D.Raycast(frontRaycastOrigin, raycastDirection, raycastMaxDistance, platformLayer);
        Debug.DrawRay(frontRaycastOrigin, raycastDirection * raycastMaxDistance, Color.green ); 

        return frontEdgeHit;
    }
}
