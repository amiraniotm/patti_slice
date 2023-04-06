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
    // Animator for little rustle animation while spawning
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        // Spawn point on the right always spaws first
        if(transform.position.x < 0){
            readyToSpawn = false;
        }else{
            readyToSpawn = true;
        }
    }

    public void SpawnEnemy(string enemyType)
    {
        animator.SetTrigger("spawn");
        animator.SetBool("isSpawning", true);
        GameObject enemyToSpawn = enemyPool.GetPooledObject(enemyType); 
        enemyToSpawn.SetActive(true);
        enemyToSpawn.transform.position = transform.position;
        // Setting up initial enemy variables and restarting behavior
        Enemy enemyScript = enemyToSpawn.GetComponent<Enemy>();
        StartCoroutine(StopAnimationCoroutine(enemyScript.spawningTime));
        enemyScript.Spawn();
        // Marking working spawnpoint was last to spawn
        readyToSpawn = false;
    }
    // Stop animation after spawning is done
    public IEnumerator StopAnimationCoroutine(float enemySpawnTime)
    {
        yield return new WaitForSeconds(enemySpawnTime);

        animator.SetBool("isSpawning", false);
    }
}
