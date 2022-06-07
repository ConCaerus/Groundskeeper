using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionCanvas : MonoBehaviour {
    [SerializeField] float showTime = 1f;
    [SerializeField] GameObject background;

    Coroutine loader = null;


    private void Awake() {
        background.SetActive(true);
        GameInfo.resetVars();
        DOTween.Init();
    }

    private void Start() {
        StartCoroutine(loadedSceneWaiter());
    }

    public void loadScene(string name) {
        if(loader != null)
            return;
        loader = StartCoroutine(loadSceneWaiter(name));
    }

    IEnumerator loadSceneWaiter(string name) {
        background.SetActive(true);
        yield return new WaitForSeconds(showTime);
        SceneManager.LoadScene(name);
    }

    IEnumerator loadedSceneWaiter() {
        yield return new WaitForEndOfFrame();
        background.SetActive(false);
    }
}
