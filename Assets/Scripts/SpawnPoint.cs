using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // Script to control each Enemy SpawnPoint, alternate spawning, etc

    // Reference to enemy object pool, where all enemy instances will be stored for use at runtime. 
    // EnemyCounter controls all enemy events. ReadyTime is the wait time between spawns
    [SerializeField] private ObjectPool enemyPool;
    [SerializeField] private EnemyCounter enemyCounter;
    // Control variable for spawning at intervals
    public bool readyToSpawn;

    private void Start()
    {
        // Spawn point on the left always spaws first
        if(transform.position.x < 0){
            readyToSpawn = false;
        }else{
            readyToSpawn = true;
        }
    }

    public void SpawnEnemy(string enemyType)
    {
        GameObject enemyToSpawn = enemyPool.GetPooledObject(enemyType); 
        enemyToSpawn.SetActive(true);
        enemyToSpawn.transform.position = transform.position;
        // Setting up initial enemy variables and restarting behavior
        Enemy enemyScript = enemyToSpawn.GetComponent<Enemy>();
        // enemyScript.originPosition = transform.position;
        // enemyScript.spawnPoint = gameObject.GetComponent<SpawnPoint>();
        enemyScript.Spawn();
        // Marking that spawn point was last to spawn
        readyToSpawn = false;
    }
}
