using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipbox : HittableBlock
{
    // Boxes that flip every enemy on map

    // References to sprites to change on use, enemy counter to detect current enemies on map, and use counter
    [SerializeField] public List<Sprite> spriteList = new List<Sprite>();
    [SerializeField] private EnemyCounter enemyCounter;
    [SerializeField] private int flipCount;
    //[SerializeField] private AudioClip powSound;
    // Runtime references: sound controller to play SFX, sprite renderer to change sprites at use, and main camera to shake screen on use
    //private SoundController soundController;
    private SpriteRenderer spriteRenderer;
    private CameraMovement mainCamera;

    private void Awake()
    {
        //soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
    }

    private void Update()
    {
        // Disabling box when uses are over
        if(flipCount == 0) {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Use mechanics triggered when hit by player from below
        if(collision.gameObject.tag == "Player"){
            string collisionSide = DetectCollisionDirection(collision);

            if(collisionSide == "upper" && flipCount > 0) {
                enemyCounter.FlipAll();
                flipCount -= 1;
                mainCamera.TriggerShake();
                //soundController.PlaySound(powSound, 0.4f);
                // Makeshift animation by changing sprite on use
                if(flipCount > 0) {
                    ChangeSprite();
                }
            }
        }

    }

    private void ChangeSprite()
    {
        spriteRenderer.sprite = spriteList[flipCount - 1]; 
    }
}
