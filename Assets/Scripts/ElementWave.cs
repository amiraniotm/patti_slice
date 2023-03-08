using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElementWave : MonoBehaviour
{
    // Waves that come out of exploding ToughEnemies and spread sideways, applying debuff effects to tiles 

    // Type of debuff tile set
    [SerializeField] private TileBase elementTile;
    // Adjustable variables
    [SerializeField] private float moveSpeed, activeTime;
    // References to objects: tile manager to set tiles, edgechecker and screenwrap to check if wave still active
    private TileController tileController;
    public EdgeChecker edgeChecker;
    protected ScreenWrap screenWrap;  

    private void Awake()
    {
        tileController = GameObject.Find("TileController").GetComponent<TileController>();
        edgeChecker = GetComponent<EdgeChecker>();
        screenWrap = GetComponent<ScreenWrap>();
    }

    private void Start()
    {
        // Setting movement direction
        ShiftDirection();
        // Immediately starting self-disappear coroutine
        StartCoroutine(QuenchCoroutine());
    }

    private void Update()
    {   
        RaycastHit2D frontEdgeHit = edgeChecker.CheckFront();
        RaycastHit2D backEdgeHit = edgeChecker.CheckBack();
        // Swapping tiles in back and front of wave
        if(backEdgeHit){
            tileController.SwapTile(backEdgeHit.point, elementTile);
        }
        if(frontEdgeHit){
            tileController.SwapTile(frontEdgeHit.point, elementTile);
        } 
        // Disappearing wave if no more tiles (platform edge)
        if(!frontEdgeHit){
            Quench();
        }  
        // Moving if still active
        float newXpos = transform.position.x + (moveSpeed * Time.deltaTime);
        transform.position = new Vector2(newXpos, transform.position.y);
    }

    private void ShiftDirection()
    {
        // Swaps speed if moving to left
        if(edgeChecker.direction == "left") {
            moveSpeed *= -1;
            transform.localScale *= new Vector2(-1,1);
        }
    }

    protected void Quench()
    {
        // Renews tile list so new debuff tiles are taken into account, and self-destroys
        Destroy(gameObject);
    }
    // Autovanishing after active time elapsed
    protected IEnumerator QuenchCoroutine()
    {
        yield return new WaitForSeconds(activeTime);

        Quench();
    }
}
