using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour {
    [SerializeField] GameObject background;
    [SerializeField] Selectable firstInSequence;

    bool shown = false;

    private void Awake() {
        background.SetActive(false);
    }


    public void show() {
        if(shown)
            return;
        shown = true;
        background.transform.localScale = Vector3.zero;
        background.SetActive(true);
        background.transform.DOScale(1.0f, .5f);
        firstInSequence.Select();
    }


    //  buttons
    public void restart() {
        FindObjectOfType<TransitionCanvas>().loadScene("Game");
    }

    public void mainMenu() {
        FindObjectOfType<TransitionCanvas>().loadScene("MainMenu");
    }

    public void quit() {
        Application.Quit();
    }
}
