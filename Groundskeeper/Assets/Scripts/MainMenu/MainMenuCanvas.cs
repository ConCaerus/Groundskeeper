using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] GameObject menu, save, key;

    private void Start() {
        key.SetActive(true);
        menu.SetActive(false);
        save.SetActive(false);
    }

    private void Update() {
        if(key.activeInHierarchy && Input.anyKey) {
            key.SetActive(false);
            menu.SetActive(true);
        }
    }

    public void quit() {
        Application.Quit();
    }
}
