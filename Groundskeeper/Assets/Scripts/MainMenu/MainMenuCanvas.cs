using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour {
    [SerializeField] GameObject menu, save, key;
    [SerializeField] Button playButton;

    Coroutine starter = null;


    private void Start() {
        key.SetActive(true);
        menu.SetActive(false);
        save.SetActive(false);
    }

    private void Update() {
        if(key.activeInHierarchy && Input.anyKey && starter == null) {
            starter = StartCoroutine(waitForEndOfInput());
        }
    }

    IEnumerator waitForEndOfInput() {
        while(Input.anyKey)
            yield return new WaitForEndOfFrame();

        //  does stuff
        key.SetActive(false);
        menu.SetActive(true);
        playButton.Select();
    }

    public void quit() {
        Application.Quit();
    }
}
