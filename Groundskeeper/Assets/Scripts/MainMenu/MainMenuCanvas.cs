using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] GameObject menu, save, key;
    [SerializeField] Button playButton;

    Coroutine starter = null, optionsChecker = null;

    InputMaster controls;

    bool keybHold = false, padHold = false;


    private void Start() {
        key.SetActive(true);
        menu.SetActive(false);
        save.SetActive(false);

        controls = new InputMaster();
        controls.Enable();

        //  does it if keyboard shit is pressed
        controls.InputSwitch.Keyboard.performed += ctx => {
            keybHold = true;
            if(key.activeInHierarchy && starter == null)
                starter = StartCoroutine(waitForEndOfInput());
        };
        controls.InputSwitch.Keyboard.canceled += ctx => {
            keybHold = false;
        };

        //  does it if controller shit is pressed
        controls.InputSwitch.Gamepad.performed += ctx => {
            padHold = true;
            if(key.activeInHierarchy && starter == null)
                starter = StartCoroutine(waitForEndOfInput());
        };
        controls.InputSwitch.Gamepad.canceled += ctx => {
            padHold = false;
        };

        //  note: doesn't do it when mouse shit is pressed

        //  checks if the player pressed the back key
        controls.Player.Cancel.performed += ctx => {
            if(save.activeInHierarchy) {
                save.SetActive(false);
                menu.SetActive(true);
                playButton.Select();
            }
        };
    }

    private void OnDisable() {
        controls.Disable();
    }

    IEnumerator waitForEndOfInput() {
        while(keybHold || padHold)
            yield return new WaitForEndOfFrame();

        //  does stuff
        key.SetActive(false);
        menu.SetActive(true);
        playButton.Select();
    }

    public void openedOptions() {
        if(optionsChecker == null)
            optionsChecker = StartCoroutine(waitForOptionsToClose());
    }

    IEnumerator waitForOptionsToClose() {
        var ops = FindObjectOfType<OptionsCanvas>();

        //  wait for options to completely open
        while(!ops.isOpen())
            yield return new WaitForEndOfFrame();

        //  wait for the options to close
        while(ops.isOpen())
            yield return new WaitForEndOfFrame();


        Debug.Log("here");
        menu.SetActive(true);
        playButton.Select();
        optionsChecker = null;
    }

    public void quit() {
        Application.Quit();
    }
}
