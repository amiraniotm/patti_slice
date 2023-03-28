using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    // Controls pausing of game 

    // References to display that dims screen, and item controller to stop item spawn
    [SerializeField] LevelDisplay levelDisplay;
    [SerializeField] ItemController itemController;
    // Master controller to pause main game loop
    private MasterController masterController;
    public bool gamePaused;

    private void Start()
    {
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetPauseController(this);
    }

    void Update()
    {
        // Checking for return key press and checking for in-game conditions
        if(Input.GetKeyDown("return") && masterController.levelStarted && !masterController.gameOver) {
            if(!gamePaused) {
                Time.timeScale = 0;
            } else {
                itemController.StartItems(2.0f);
                Time.timeScale = 1;
            }

            itemController.StopItems();
            levelDisplay.TogglePausePanel();
            //masterController.soundController.TogglePauseMusic();
            gamePaused = !gamePaused;
        }
    }
}
