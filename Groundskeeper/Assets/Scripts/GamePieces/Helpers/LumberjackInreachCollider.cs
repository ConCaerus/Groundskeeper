using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackInreachCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            var li = unit.GetComponent<LumberjackInstance>();

            li.inReach = true;
            li.hasTarget = true;
            li.followingTransform = col.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  an attackable unit entered sights
        if(col.gameObject.tag == "Monster") {
            var li = unit.GetComponent<LumberjackInstance>();

            li.inReach = false;
            li.hasTarget = false;
            li.followingTransform = null;
            StartCoroutine(resetCollider());
        }
    }

    //  resets the collider to check and see if there are still any relevant collisions
    IEnumerator resetCollider() {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForEndOfFrame();
        GetComponent<Collider2D>().enabled = true;
    }
}
