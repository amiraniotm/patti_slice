using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPlatform : MonoBehaviour
{
    // Script for the Platform where the player goes down as is spawning

    // Editor vars for player and initial movement 
    [SerializeField] private PlayerScript player;
    [SerializeField] public float spawnTime, holdTime, spawnSpeed;
    // Runtime movement vars
    private bool isHolding;
    private float spawnCount;
    private Vector2 initialPosition;
    private Coroutine activeHideRoutine;

    private void Awake()
    {
        initialPosition = transform.position;
    }
    // Spawn Counter kickstarts movement
    public void SetSpawnCounter()
    {
        spawnCount = spawnTime;
    }

    private void Update()
    {
        // Moving platform down while theres time left on the counter
        if (spawnCount > 0) {            
            spawnCount -= Time.deltaTime;
            float nextYPos = transform.position.y - (spawnSpeed * Time.deltaTime);
            
            transform.position = new Vector2(transform.position.x, nextYPos);
        // When time runs out, release player for movement and start auto-hiding routine
        } else if (!isHolding) {
            isHolding = true;
            player.isSpawning = false;
            activeHideRoutine = StartCoroutine(HideCoroutine());
        }
    }

    public void DoHide()
    {
        // Resetting and hiding platform until next player spawn
        gameObject.SetActive(false);
        isHolding = false;
        transform.position = initialPosition;
        spawnCount = 0;
        player.FinishSpawn();
        // Control for the coroutine in case DoHide is called from player
        if(activeHideRoutine != null) {
            StopCoroutine(activeHideRoutine);
        }
    }

    private IEnumerator HideCoroutine()
    {
        yield return new WaitForSeconds(holdTime);

        DoHide();
    }
}
