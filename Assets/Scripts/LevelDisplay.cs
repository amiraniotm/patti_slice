using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelDisplay : MonoBehaviour
{
    // In-game UI control script

    // References to text displays and panel objects to update
    [SerializeField] private TMP_Text livesText, timeText, pointsText, levelText;
    [SerializeField] public GameObject gameOverPanel, pausePanel, gameOverSign, youWonSign, timePanel;
    [SerializeField] private float resetDelay;
    // Mastercontroller has all info for points, lives, time, etc
    private MasterController masterController;
    // Enables game reset on "Game Over" screen
    private bool onGameOverScreen, canReset;

    private void Awake()
    {
        // Setting crossed references for master controller
        masterController = GameObject.FindGameObjectWithTag("MasterController").GetComponent<MasterController>();
        masterController.SetLevelDisplay(this);
    }

    private void Update()
    {
        // Catching any key input on game over screen to reset game 
        if(canReset && Input.anyKey) {
            masterController.ResetGame();
        }
        
        UpdateInfoDisplays();
        UpdateLevelText();
    }
    // Called to show either "win" or "lose" game over screens
    public void ToggleGameOverScreen()
    {               
        if(masterController.gameOver){
            gameOverSign.SetActive(true);
            youWonSign.SetActive(false);
        } else {
            gameOverSign.SetActive(false);
            youWonSign.SetActive(true);
        }

        onGameOverScreen = !onGameOverScreen;
        gameOverPanel.SetActive(onGameOverScreen);
        // Adding delay for reset to prevent flash GameOver screen
        StartCoroutine(EnableResetCoroutine());
    }
    // Shows and hides panel to dim screen while paused
    public void TogglePausePanel()
    {
        if(pausePanel.activeSelf) {
            pausePanel.SetActive(false);
        } else {
            pausePanel.SetActive(true);
        }
    }
    // Updates lives, formatted time, and points display each frame
    public void UpdateInfoDisplays()
    {
        livesText.text = masterController.livesCount.ToString("0");
        timeText.text = TimeSpan.FromSeconds(masterController.timeCount).ToString("m\\'ss\\'ff");
        pointsText.text = masterController.pointsCount.ToString("0");
    }
    // Level text is always active. Its used to display level info at the start; and pause and other alerts. By default it's empty
    public void UpdateLevelText()
    {
        if(masterController.timeUp && !onGameOverScreen) {
            levelText.text = "Time's up!";
        } else if(!masterController.levelStarted && masterController.changingLevel && !onGameOverScreen) {
            int levelToDisplay = masterController.currentLevelKey + 1;
            int phaseToDisplay = masterController.currentPhaseKey;
            levelText.text = "Level " + levelToDisplay.ToString("0") + " - " + phaseToDisplay.ToString("0");
        } else if(masterController.pauseController.gamePaused) {
            levelText.text = "PAUSE";
        } else if(masterController.startingScroll) {
            levelText.color = Color.yellow;
            levelText.text = "CLIMB!!";
        } else if (onGameOverScreen) {
            levelText.text = "Final Score: " + masterController.pointsCount.ToString("0");
        } else {
            levelText.color = Color.white;
            levelText.text = "";
        }
    }

    private IEnumerator EnableResetCoroutine()
    {
        float resetCounter = resetDelay;

        while (resetCounter > 0){
            resetCounter -= Time.unscaledDeltaTime;
            
            yield return 0;
        }

        canReset = !canReset;
    }
}
