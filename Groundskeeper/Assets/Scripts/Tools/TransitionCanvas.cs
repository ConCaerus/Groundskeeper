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
        QualitySettings.vSyncCount = GameInfo.getGameOptions().vSync ? 1 : 0; // don't do shit to this
        background.SetActive(true);
        GameInfo.init();
        DOTween.Init();
    }

    private void Start() {
        StartCoroutine(loadedSceneWaiter());
    }

    public void loadScene(string name) {
        if(loader != null)
            return;
        Time.timeScale = 1f;
        TimeInfo.saveTime();
        loader = StartCoroutine(loadSceneWaiter(name));
    }

    IEnumerator loadSceneWaiter(string name) {
        background.SetActive(true);
        yield return new WaitForSeconds(showTime);
        SceneManager.LoadScene(name);
    }

    IEnumerator loadedSceneWaiter() {
        var gb = FindObjectOfType<GameBoard>();
        yield return new WaitForEndOfFrame();
        if(gb != null) {
            while(!gb.loaded)
                yield return new WaitForSeconds(.1f);
        }
        if(FindObjectOfType<PlayerInstance>() != null)
            FindObjectOfType<PlayerInstance>().setCanMove(true);
        else if(FindObjectOfType<PlayerHouseInstance>() != null)
            FindObjectOfType<PlayerHouseInstance>().setCanMove(true);
        background.SetActive(false);
    }
}
