using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    // Controls items spawning and despawning, and player-item interaction

    // Editor variables to pause item spawn and locate and validate item position
    [SerializeField] private PauseController pauseController;
    [SerializeField] private LayerMask platformMask;
    //[SerializeField] private AudioClip itemGotSound, enemyCollisionSound, itemAppearSound;
    [SerializeField] private ObjectPool itemPool;
    [SerializeField] private float spawnTime, weightAdjust;
    [SerializeField] private int itemLimit = 5;
    // Runtime vars: name list for weighted item chance and registry, item zone limits off-screen appearance
    private List<string> itemNames = new List<string>();
    private BoxCollider2D itemZone;
    private MasterController masterController;
    // Weighted chance control
    private Dictionary<string,float> itemWeights = new Dictionary<string, float>();
    //private Dictionary<string,int> originalWeights = new Dictionary<string, int>();
    //private float originalSpawnTime;
    //private List<string> originalItemNames = new List<string>();
    // Control variables for item on-screen
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
        itemWeights.Add("HardShell", 50);

        itemNames = new List<string>(itemWeights.Keys);

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
        float[] weights = new float[itemWeights.Count];
        itemWeights.Values.CopyTo(weights, 0);
        // Randomly generate a counterweight to compare item weights
        float randomWeight = UnityEngine.Random.Range(0, weights.Sum());
        // Loops stops whenever counterweight runs out
        while( randomWeight >= 0) {
            // Iterating items and decreasing counterweight by item weight value
            for (int i = 0; i < weights.Length; ++i)
            {
                randomWeight -= weights[i];
                // If counterweight ran out, return the item index
                if (randomWeight < 0)
                {
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
    // Fairer item limit: sets back item counter if player didnt pick item
    public void CheckItemRestablish(bool itemWasTaken)
    {
        if(!itemWasTaken) {
            itemLimit += 1;
        }
    }
    // SFX functions
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
    // When called is always trying to spawn an item until it manages to do so
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
                string itemToPlace =  itemNames[itemIndex];
                currentItem = itemPool.GetPooledObject(itemToPlace);
                // Adjusting weight down every time an item is placed, to diminish repeated item probability
                itemWeights[itemToPlace] /= weightAdjust;
                // Setting item
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
            // Exiting loop if 1000 unsuccessful executions 
            runCount += 1;
            
            if(runCount > 1000) {
                itemSet = true;
            }

            yield return 0;
        }
    }
}
