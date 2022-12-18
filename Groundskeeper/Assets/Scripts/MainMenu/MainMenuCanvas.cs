using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] GameObject menu, save;

    private void Start() {
        menu.SetActive(true);
        save.SetActive(false);
    }

    public void quit() {
        Application.Quit();
    }
}
