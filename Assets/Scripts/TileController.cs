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
    [SerializeField] private GameObject platformObject, wallObject;
    [SerializeField] private Tilemap platformsTilemap; 
    [SerializeField] private MapScrollController scrollController;
    [SerializeField] private float flipDuration;
    // Dict to store current tiles and tile types on map
    private Dictionary<TileBase,TileData> dataFromTiles;
    // Runtime objects references
    private Renderer[] platformRenderers;
    // public MasterController masterController;
    // Control variables
    private Vector3Int newTilePosition;
    private bool platformsMoved;
    private BoxCollider2D flipCollider;
    
    private void Awake()
    {
        // Flip collider is used to flip enemies
        flipCollider = GetComponent<BoxCollider2D>();
        // Getting initial tiles on map
        RefreshTileList();
        platformRenderers = platformObject.GetComponentsInChildren<Renderer>();
        //masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        //masterController.SetTileManager(this);
    }

    private void Update()
    {     
        if(scrollController.isScrolling) {
            bool platformsVisible = ArePlatformsVisible();

            if(!platformsVisible && !platformsMoved) {
                platformsMoved = true;
                scrollController.MoveStage();
            } else if(platformsVisible && platformsMoved) {
                platformsMoved = false;
                scrollController.StopPlatformsAndObstacles();
            }
        }
    }

    public void RefreshTileList()
    {
        // Stores each tile for control
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
    // Swap tile is used by element waves, which swap normal tiles for debuff tiles
    public void SwapTile(Vector2 waveHit, TileBase tileToSwap)
    {
        // Detecting cross tiles from collision tile as collision-position is not always reliable, and to swap adjacent tiles too    
        Vector3Int waveTile = platformsTilemap.WorldToCell(waveHit);    
        List<Vector3Int> crossTiles = AnyCrossTiles(waveTile);

        foreach(Vector3Int tilePos in crossTiles) {
            platformsTilemap.SetTile(tilePos, tileToSwap);
        }
    }
    // Used by player to get the speed modifier of tile below
    public float GetTileSpeedMod(Vector2 playerHit)
    {
        Vector3Int playerTile = platformsTilemap.WorldToCell(playerHit);
        List<Vector3Int> crossTiles = AnyCrossTiles(playerTile);
        // Gets modifier from first cross-tile, that should almost always be the one that is below player
        if(crossTiles.Count > 0) {
            TileBase tile = platformsTilemap.GetTile(crossTiles[0]);
            
            return dataFromTiles[tile].speedMod;     
        // Default return in case there are no tiles detected       
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
    // Checks if tile is present at given position
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

    public bool ArePlatformsVisible()
    {
        foreach(var renderer in platformRenderers)
        {
            if(renderer.isVisible)
            {
                return true;
            }
        }

        return false;
    }
    // Finishing flip by disabling flip collider
    private IEnumerator HideFlipColliderCoroutine()
    {
        yield return new WaitForSeconds(flipDuration);

        flipCollider.enabled = false;
    }
}
