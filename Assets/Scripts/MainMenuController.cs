using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour, IPointerEnterHandler
{
    // Controller is in charge of all Main Menu interactions like moving-through and selecting options

    // Object and script references for UI and others
    [SerializeField] private Button[] availableButtons;
    //[SerializeField] private SoundController soundController;
    //[SerializeField] private AudioClip optionChangeSound, optionSelectSound;
    [SerializeField] private MasterController masterController;

    //References for current active Button, its Text for highlighting and its index in the button array for wrap-around
    private Button activeButton;
    private TMP_Text activeText;
    private int selectedIndex = 0;

    private void Awake()
    {
        // Setting "PLAY" button highlighted by default
        SetActiveButton();
    }

    private void Update()
    {
        // Key-input menu selection movement, with edge wrap-around
        if(Input.GetKeyDown("up")) {
            if(selectedIndex > 0){
                selectedIndex -= 1;
            } else {
                selectedIndex = availableButtons.Length - 1;
            }
        } else if(Input.GetKeyDown("down")) {
            if(selectedIndex < (availableButtons.Length - 1)) {
                selectedIndex += 1;
            } else {
                selectedIndex = 0;
            }
        }
        // Playing SFX and change active button on move
        if(Input.GetKeyDown("up") || Input.GetKeyDown("down")) {
            SetActiveButton();
        }
        // Return key selects current option
        if(Input.GetKeyDown("return")) {
            activeButton.onClick.Invoke();
            //soundController.PlaySound(optionSelectSound, 0.2f);
        }
    }
    // Sets current active button on selection change
    private void SetActiveButton()
    {
        activeButton = availableButtons[selectedIndex];
        activeText = activeButton.GetComponentInChildren<TMP_Text>();
        activeText.color = Color.white;
        //Fade out other options so selected one stands out
        foreach (Button optButton in availableButtons) {
            if(optButton != activeButton) {
                TMP_Text buttonText = optButton.GetComponentInChildren<TMP_Text>();
                buttonText.color = Color.grey;
            }
        }
        // soundController.PlaySound(optionChangeSound, 0.2f);
    }
    // Functions for event callbacks from UI
    public void PlayGame()
    {
        masterController.StartGame();
    }

    public void ShowInstructions()
    {
        masterController.ShowInstructionsPanel();
    }
    //Mouse-hover option change and selection
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button overButton = eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<Button>();
        selectedIndex = System.Array.IndexOf(availableButtons,overButton);
        SetActiveButton();
    }
}
