using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unstucker : MonoBehaviour {
    Coroutine unstucker = null;

    [SerializeField] string[] ignoreTags;

    private void OnTriggerStay2D(Collider2D col) {
        if(unstucker == null && (col.gameObject.tag != "Player")) {
            foreach(var i in ignoreTags) {
                if(col.gameObject.tag == i)
                    return;
            }
            unstucker = StartCoroutine(unstuckWaiter(col));
        }
    }

    IEnumerator unstuckWaiter(Collider2D obst) {
        yield return new WaitForSeconds(1.0f);
        while(GetComponent<Collider2D>().IsTouching(obst)) {
            if(obst.gameObject.tag == "Environment")
                obst.GetComponent<Collider2D>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            yield return new WaitForSeconds(.5f);
            if(obst.gameObject.tag == "Environment")
                obst.GetComponent<Collider2D>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
        }

        unstucker = null;
    }
}
