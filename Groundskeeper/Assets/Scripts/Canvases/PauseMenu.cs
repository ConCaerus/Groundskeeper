using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MenuCanvas {
    InputMaster controls;
    [SerializeField] GameObject background;
    [SerializeField] Button defaultButt;

    MouseManager mm;

    private void Awake() {
        mm = FindObjectOfType<MouseManager>();
        controls = new InputMaster();
        controls.Pause.Toggle.started += ctx => togglePause();
    }


    public void togglePause() {
        foreach(var i in FindObjectsOfType<MenuCanvas>()) {
            if(i != this && i.isOpen()) {
                i.tryClose();
            }
        }
        if(!background.activeInHierarchy && FindObjectOfType<TransitionCanvas>().finishedLoading)
            tryShow();
        else
            tryClose();
    }

    protected override void show() {
        background.SetActive(true);
        if(!mm.usingKeyboard())
            defaultButt.Select();
        Time.timeScale = 0.0f;
        if(FindObjectOfType<FreeGamepadCursor>() != null)
            FindObjectOfType<FreeGamepadCursor>().showCursor(false, true);
    }
    protected override void close() {
        background.SetActive(false);
        Time.timeScale = 1.0f;
        if(FindObjectOfType<FreeGamepadCursor>() != null)
            FindObjectOfType<FreeGamepadCursor>().showCursor(true, true);
    }

    public void resume() {
        //if(!SteamManager.Initialized || !SteamUtils.IsOverlayEnabled())
        togglePause();
    }

    public void options() {
        //if(!SteamManager.Initialized || !SteamUtils.IsOverlayEnabled()) {
        tryClose();
        FindObjectOfType<OptionsCanvas>().tryShow();
        FindObjectOfType<OptionsCanvas>().setup();
        //}
    }

    public void menu() {
        //if(!SteamManager.Initialized || !SteamUtils.IsOverlayEnabled())
        FindObjectOfType<TransitionCanvas>().loadScene("MainMenu");
    }


    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
}
