using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour {
    [SerializeField] GameObject background;

    private void Awake() {
        background.SetActive(false);
    }


    public void show() {
        background.transform.localScale = Vector3.zero;
        background.SetActive(true);
        background.transform.DOScale(1.0f, .5f);
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
