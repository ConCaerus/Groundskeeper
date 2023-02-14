using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MenuCanvas {
    InputMaster controls;
    [SerializeField] GameObject background;

    private void Awake() {
        controls = new InputMaster();

        controls.Pause.Toggle.performed += ctx => togglePause();
    }


    public void togglePause() {
        foreach(var i in FindObjectsOfType<MenuCanvas>()) {
            if(i != this && i.isOpen()) {
                i.tryClose();
            }
        }
        if(!background.activeInHierarchy)
            tryShow();
        else
            tryClose();
    }

    protected override void show() {
        background.SetActive(true);
        Time.timeScale = 0.0f;
    }
    protected override void close() {
        background.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void resume() {
        togglePause();
    }

    public void menu() {
        FindObjectOfType<TransitionCanvas>().loadScene("MainMenu");
    }


    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
}
