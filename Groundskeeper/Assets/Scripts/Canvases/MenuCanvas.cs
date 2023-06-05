using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuCanvas : MonoBehaviour {
    bool open = false;

    Coroutine opener = null;

    InputMaster controls;

    private void Awake() {
        controls = new InputMaster();
        controls.Player.Cancel.canceled += ctx => closeOpenMenus();
    }

    public void tryShow() {
        if(open || opener != null)
            return;

        //  if anything is already open, return
        foreach(var i in FindObjectsOfType<MenuCanvas>()) {
            if(i.isOpen()) {
                return;
            }
        }

        opener = StartCoroutine(waitForClearInput());
    }
    public void tryClose() {
        if(!open) {
            return;
        }
        open = false;
        close();
    }

    void closeOpenMenus() {
        foreach(var i in FindObjectsOfType<MenuCanvas>()) {
            if(i.isOpen()) {
                i.tryClose();
            }
        }
    }


    //  don't fucking make these public
    //  use the try functions above if you want to do shit
    protected abstract void show();
    protected abstract void close();


    public bool isOpen() {
        return open;
    }

    IEnumerator waitForClearInput() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        //  opens
        open = true;
        show();
        opener = null;
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
