using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    // Controller for Tile interactions such as flipping and speed-mod tiles

    // References to all tiles and tilemaps
    [SerializeField] private List<TileData> tileDatas;
    // [SerializeField] private List<TileBase> availableLevelTiles;
    // [SerializeField] private GameObject platformObject, wallObject;
    [SerializeField] private Tilemap platformsTilemap; 
    //[SerializeField] private MapDisplacementController mapDisController;
    [SerializeField] private float flipDuration;
    // Dict to store current tiles and tile types on map
    private Dictionary<TileBase,TileData> dataFromTiles;
    // // Runtime objects references
    // private Renderer[] platformRenderers;
    // public MasterController masterController;
    // //public BoxCollider2D playerCollider;
    // // Control variables
    private Vector3Int newTilePosition;
    // public bool platformsMoved;
    private BoxCollider2D flipCollider;
    
    private void Awake()
    {
        flipCollider = GetComponent<BoxCollider2D>();
        //platformRenderers = platformObject.GetComponentsInChildren<Renderer>();
        //playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        //masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        //masterController.SetTileManager(this);
        RefreshTileList();
    }

    // private void Update()
    // {     
    //     if(masterController.scrollPhase) {
    //         bool platformsVisible = ArePlatformsVisible();

    //         if(!platformsVisible && !platformsMoved) {
    //             platformsMoved = true;
    //             //mapDisController.MoveStage();
    //         } else if(platformsVisible && platformsMoved) {
    //             platformsMoved = false;
    //             //mapDisController.StopPlatformsAndObstacles();
    //         }
    //     }
    // }

    public void RefreshTileList()
    {
        dataFromTiles = new Dictionary<TileBase,TileData>();

        foreach(var tileData in tileDatas) {
            foreach(var tile in tileData.tiles ) {
                dataFromTiles.Add(tile, tileData);
            }   
        } 
    }

    public void SetFlipCollider(Collision2D collision)
    {
        // Setting flipCollider collider on top side of platform being hit from below by player
        transform.position = new Vector2(collision.contacts[0].point.x , collision.contacts[0].point.y + (platformsTilemap.cellSize.y) );
        flipCollider.enabled = true;
        StartCoroutine(HideFlipColliderCoroutine());        
    }
    // Checks for enemy-flipCollider collision to flip enemies
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemies") {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            enemy.FlipVertical();
        }
    }

    public void SwapTile(Vector2 waveHit, TileBase tileToSwap)
    {
        Vector3Int waveTile = platformsTilemap.WorldToCell(waveHit);        
        //Vector3Int formerTilePosition = new Vector3Int(touchPosition.x - 1, touchPosition.y, touchPosition.z);
        List<Vector3Int> crossTiles = AnyCrossTiles(waveTile);

        foreach(Vector3Int tilePos in crossTiles) {
            platformsTilemap.SetTile(tilePos, tileToSwap);
        }
    }

    public float GetTileSpeedMod(Vector2 playerHit)
    {
        Vector3Int playerTile = platformsTilemap.WorldToCell(playerHit);
        List<Vector3Int> crossTiles = AnyCrossTiles(playerTile);
        //newTilePosition = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);

        if(crossTiles.Count > 0) {
            TileBase tile = platformsTilemap.GetTile(crossTiles[0]);
            
            return dataFromTiles[tile].speedMod;            
        } else {
            return 1.0f;
        } 
    }
    // Checks all rectangularly contiguous spaces in tilemap and returns only the ones with tiles
    private List<Vector3Int> AnyCrossTiles(Vector3Int centerPos)
    {
        List<Vector3Int> iniPosList = new List<Vector3Int>();
        List<Vector3Int> finalPosList = new List<Vector3Int>();

        Vector3Int upperPos = new Vector3Int(centerPos.x, centerPos.y + 1, centerPos.z);
        Vector3Int lowerPos = new Vector3Int(centerPos.x, centerPos.y - 1, centerPos.z);
        Vector3Int rightPos = new Vector3Int(centerPos.x + 1, centerPos.y, centerPos.z);
        Vector3Int leftPos = new Vector3Int(centerPos.x - 1, centerPos.y, centerPos.z); 

        iniPosList.Add(centerPos);
        iniPosList.Add(upperPos);
        iniPosList.Add(lowerPos);
        iniPosList.Add(rightPos);
        iniPosList.Add(leftPos);

        foreach (Vector3Int tilePos in iniPosList) {
            if(platformsTilemap.HasTile(tilePos)) {
                finalPosList.Add(tilePos);
            }
        }

        return finalPosList;
    }

    public bool CheckForTile(Vector2 worldPosition)
    {
        Vector3Int tilePosition = platformsTilemap.WorldToCell(worldPosition);

        return platformsTilemap.HasTile(tilePosition);
    }

    // public void SetLevelTiles(int levelKey)
    // {
    //     wallObject.SetActive(true);

    //     Tilemap[] levelTileMaps = FindObjectsOfType<Tilemap>();
        
    //     foreach(Tilemap map in levelTileMaps) {
    //         BoundsInt bounds = map.cellBounds;
    //         TileBase[] allTiles = map.GetTilesBlock(bounds);

    //         foreach(TileBase tile in allTiles) {
    //             if(tile != null) {
    //                 map.SwapTile(tile, availableLevelTiles[levelKey]);
    //             }
    //         }
    //     }
    // }

    // public bool ArePlatformsVisible()
    // {
    //     foreach(var renderer in platformRenderers)
    //     {
    //         if(renderer.isVisible)
    //         {
    //             return true;
    //         }
    //     }

    //     return false;
    // }

    private IEnumerator HideFlipColliderCoroutine()
    {
        // Finishing flip by disabling flip collider
        yield return new WaitForSeconds(flipDuration);

        flipCollider.enabled = false;
    }
}
