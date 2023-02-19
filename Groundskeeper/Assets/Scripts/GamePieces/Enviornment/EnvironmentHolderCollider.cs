using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentHolderCollider : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player")
            col.GetComponent<PlayerInstance>().slowed = true;
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Player")
            col.GetComponent<PlayerInstance>().slowed = false;

    }
}
