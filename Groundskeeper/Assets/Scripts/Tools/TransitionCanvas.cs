using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionCanvas : MonoBehaviour {
    [SerializeField] float showTime = 1f;
    [SerializeField] Animator anim;

    Coroutine loader = null;

    public bool finishedLoading = false;


    private void Awake() {
        QualitySettings.vSyncCount = GameInfo.getGameOptions().vSync ? 1 : 0; // don't do shit to this
        GameInfo.init();
        DOTween.Init();
        anim.gameObject.SetActive(true);
    }

    private void Start() {
        //  sets the random rotation that tilts the thing
        var rand = Random.Range(0, 2);
        if(rand == 0)
            anim.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        StartCoroutine(loadedSceneWaiter());
    }

    public void loadScene(string name) {
        if(loader != null)
            return;

        Time.timeScale = 1f;
        TimeInfo.saveTime();
        loader = StartCoroutine(loadSceneWaiter(name));
    }

    //  is going to load a new scene
    //  exit
    IEnumerator loadSceneWaiter(string name) {
        finishedLoading = false;
        anim.SetTrigger("transition");
        //  waits for the board to finish saving if it is saving
        if(FindObjectOfType<GameBoard>() != null) {
            yield return new WaitForEndOfFrame();
            var gb = FindObjectOfType<GameBoard>();
            gb.fastSave = true;
            while(gb.saving())
                yield return new WaitForEndOfFrame();
        }
        FindObjectOfType<AudioManager>().reduceAllVolume(showTime);
        yield return new WaitForSeconds(showTime);
        SceneManager.LoadScene(name);
    }

    //  has just loaded the scene
    //  intro
    IEnumerator loadedSceneWaiter() {
        var gb = FindObjectOfType<GameBoard>();
        yield return new WaitForEndOfFrame();
        if(gb != null) {
            while(!gb.loaded)
                yield return new WaitForSeconds(.1f);
        }
        if(FindObjectOfType<PlayerInstance>() != null)
            FindObjectOfType<PlayerInstance>().setCanMove(GameInfo.getNightCount() > 0);
        else if(FindObjectOfType<PlayerHouseInstance>() != null)
            FindObjectOfType<PlayerHouseInstance>().setCanMove(GameInfo.getNightCount() > 0);

        anim.SetTrigger("transition");
        FindObjectOfType<AudioManager>().increaseAllVolume(showTime);
        finishedLoading = true;
    }
}
