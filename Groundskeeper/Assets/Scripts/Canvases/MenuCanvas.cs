using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuCanvas : MonoBehaviour {
    bool open = false;

    Coroutine opener = null;

    public void tryShow() {
        if(open || opener != null)
            return;
        //  close any open shit
        foreach(var i in FindObjectsOfType<MenuCanvas>()) {
            if(i.isOpen()) {
                return;
            }
        }

        opener = StartCoroutine(waitForClearInput());
    }
    public void tryClose() {
        if(!open)
            return;
        open = false;
        close();
    }


    //  don't fucking make these public
    //  use the try functions above if you want to do shit
    protected abstract void show();
    protected abstract void close();


    public bool isOpen() {
        return open;
    }

    IEnumerator waitForClearInput() {
        while(Input.anyKey)
            yield return new WaitForEndOfFrame();
        //  opens
        open = true;
        show();
        opener = null;
    }
}
