using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] GameObject menu, save, key;
    [SerializeField] Button playButton;

    private void Start() {
        key.SetActive(true);
        menu.SetActive(false);
        save.SetActive(false);
    }

    private void Update() {
        if(key.activeInHierarchy && Input.anyKey) {
            key.SetActive(false);
            menu.SetActive(true);
            playButton.Select();
        }
    }

    public void quit() {
        Application.Quit();
    }
}
