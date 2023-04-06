using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    // Controls boss and enemy spawn, and also keeps enemy count for events like game over, stage clear, etc

    // References to spawn points, bosses available, and how often enemies are spawned
    [SerializeField] public SpawnPoint[] spawnPoints;
    [SerializeField] public GameObject[] availableBosses; 
    [SerializeField] private float spawnInterval;
    // References to level for total enemies, current enemies active, and enemy spawn queue
    private Level currentLevel;
    public List<Enemy> currentEnemies = new List<Enemy>();
    public List<string> spawnQ = new List<string>();
    public MasterController masterController;
    // Control variables for active phase and if any spawn point is still spawning
    private int currentPhase;
    public int totalEnemies;

    private void Awake()
    {
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetEnemyCounter(this);
    }

    public void Start()
    {
        // Setting level and phase
        currentLevel = masterController.currentLevel;
        currentPhase = masterController.currentPhaseKey - 1;
        // Getting level enemies and assigning total count and queue
        foreach(KeyValuePair<string,int> newEnemy in currentLevel.levelEnemies[currentPhase]) {
            for (int i = 0; i < newEnemy.Value; i++)
            {
                spawnQ.Add(newEnemy.Key);
            }
        }
        totalEnemies = spawnQ.Count;

        InvokeRepeating("TriggerSpawn", spawnInterval, spawnInterval);
    }

    private void TriggerSpawn()
    {
        foreach(SpawnPoint spawnPoint in spawnPoints){
            // Alternating spawn between spawnpoints
            if(!spawnPoint.readyToSpawn){
                spawnPoint.readyToSpawn = !spawnPoint.readyToSpawn;
            }else{
                // Spawning enemy at top of queue if any on queue
                if(spawnQ.Count > 0) {
                    spawnPoint.SpawnEnemy(spawnQ[0]);
                    spawnQ.RemoveAt(0);
                }
            }
        }
    }
    // Called every time a defeated enemy leaves the screen
    public void EnemyDied(Enemy deadEnemy)
    {
        totalEnemies -= 1;
        currentEnemies.Remove(deadEnemy);
        masterController.AddPoints(deadEnemy.bounty);
        masterController.CheckEnemies();
    }
    // Called when a FlipBox is hit
    public void FlipAll()
    {
        foreach(Enemy enemy in currentEnemies) {
            if(enemy.isGrounded && !enemy.isDefeated) {
                enemy.FlipVertical();
            }
        }
    }
    // Spawning level boss. Bosses are always present but hidden, not pooled
    public void SpawnBoss()
    {
        int bossKey = masterController.currentLevelKey - 1;
        GameObject currentBoss = availableBosses[bossKey];

        currentBoss.SetActive(true); 
    }
}
