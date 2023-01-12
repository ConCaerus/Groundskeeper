using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {
    InputMaster controls;

    private void Awake() {
        controls = new InputMaster();

        controls.Pause.Toggle.performed += ctx => togglePause();
    }


    public void togglePause() {
        if(FindObjectOfType<OptionsCanvas>() != null)
            return;
        var background = transform.GetChild(0).gameObject;
        if(!background.activeInHierarchy) {
            background.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else {
            background.SetActive(false);
            Time.timeScale = 1.0f;
        }
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
