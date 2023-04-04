using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    // Controls items spawning and despawning, and player-item interaction

    // Runtime objects to pause, locate, validate item position...
    [SerializeField] private PauseController pauseController;
    [SerializeField] private LayerMask platformMask;
    // Name list for weighted item chance and registry
    [SerializeField] private List<string> itemNames = new List<string>();
    //[SerializeField] private Inventory playerInventory;
    //[SerializeField] private AudioClip itemGotSound, enemyCollisionSound, itemAppearSound;
    [SerializeField] private ObjectPool itemPool;
    [SerializeField] private float spawnTime;
    [SerializeField] private int itemLimit = 5;
    // Inner references
    private BoxCollider2D itemZone;
    private MasterController masterController;
    // Weighted chance control
    private Dictionary<string,int> itemWeights = new Dictionary<string, int>();
    //private Dictionary<string,int> originalWeights = new Dictionary<string, int>();
    // control variables for item on-screen
    //private float originalSpawnTime;
    //private List<string> originalItemNames = new List<string>();
    public GameObject currentItem;
    public Item currentItemScript;
    private Vector2 newItemPos;

    private void Start()
    {
        itemZone = GetComponent<BoxCollider2D>();
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetItemController(this);
        // Weighted system: adjust down for less chance of item
        itemWeights.Add("ExtraLife", 80);
        itemWeights.Add("ExtraTime", 80);
        itemWeights.Add("AttackPincer", 50);
        itemWeights.Add("BoomerangPincer", 40);
        itemWeights.Add("HardShell", 50000);

        //originalSpawnTime = spawnTime;
        //originalWeights = itemWeights;
        //originalItemNames = itemNames;
    }
    // Trigger item spawn on and off
    public void StartItems(float startTime)
    {
        InvokeRepeating("SpawnItem", startTime, spawnTime);
    }

    public void StopItems()
    {
        CancelInvoke();
    }
    // Kicks out the item spawn system
    private void SpawnItem()
    {    
        if(itemLimit > 0) {
            StartCoroutine(SetNewItemCoroutine());
        }  
    }

    public int GetWeightedRandomItem()
    {    
        // Creating an array with the item weigths
        int[] weights = new int[itemWeights.Count];
        itemWeights.Values.CopyTo(weights, 0);
        // Randomly generate a counterweight to compare item weights
        int randomWeight = UnityEngine.Random.Range(0, weights.Sum());
        // Loops stops whenever counterweight runs out
        while( randomWeight >= 0) {
            // Iterating items and decreasing counterweight by item weight value
            for (int i = 0; i < weights.Length; ++i)
            {
                randomWeight -= weights[i];
                // If counterweight ran out, return the item index
                if (randomWeight < 0)
                {
                    Debug.Log(weights[i]);
                    return i;
                }
            }        
        }
        // Safe return of the 1st item on the list
        return 0;
    }
    // Clears items control variables (e.g. to start another level)
    public void FlushItems()
    {
        //itemNames = originalItemNames;
        //spawnTime = originalSpawnTime;
        ///itemWeights = originalWeights;
        itemLimit = 5;
    }

    // public void ItemGot()
    // {
    //     masterController.soundController.PlaySound(itemGotSound, 0.2f);
    // }

    // public void EnemyHit()
    // {
    //     masterController.soundController.PlaySound(enemyCollisionSound, 0.4f);
    // }

    
    // public void SetItemsForBoss(int levelKey) 
    // {        
    //     if(levelKey == 1){
    //         itemWeights.Remove("ExtraLife");
    //         itemNames.Remove("ExtraLife");
    //         itemLimit = 50;
    //         spawnTime = 6.0f;
    //     }
    // }
    // Coroutine is always trying to spawn an item until it manages to do so
    private IEnumerator SetNewItemCoroutine()
    {
        // Runcount is used to have an exit to infinite loop
        int runCount = 0;
        bool itemSet = false;
        // Continues until item is spawned
        while(!itemSet) {
            // Index from weighted random system
            int itemIndex = GetWeightedRandomItem();
            // Random position within the item zone to test
            float randomX = Random.Range(itemZone.bounds.min.x, itemZone.bounds.max.x);
            float randomY = Random.Range(itemZone.bounds.min.y, itemZone.bounds.max.y);
            newItemPos = new Vector2(randomX, randomY);
            // Checking if spawn point collides with platforms
            bool isColliding = Physics.CheckSphere(newItemPos, 1f, platformMask, QueryTriggerInteraction.Collide);
            // If spawn coordinates are within bounds and not colliding with platform, spawn item
            if(itemZone.bounds.Contains(newItemPos) && !isColliding) {
                currentItem = itemPool.GetPooledObject(itemNames[itemIndex]);

                if(currentItem != null) {
                    currentItem.SetActive(true);
                    currentItem.transform.position = newItemPos;
                    currentItemScript = currentItem.GetComponent<Item>();
                    currentItemScript.StartVanishCoroutine();
                    //masterController.soundController.PlaySound(itemAppearSound, 0.3f);
                    itemSet = true;
                    itemLimit -= 1;
                }
            } 

            runCount += 1;
            // Exiting loop if 1000 unsuccessful executions 
            if(runCount > 1000) {
                itemSet = true;
            }

            yield return 0;
        }
    }
}
