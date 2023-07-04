using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBounds : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Monster") {
            col.gameObject.GetComponent<MonsterInstance>().die();
            Debug.Log("Got a runner");
        }
    }

    private void Awake() {
        for(int i = 0; i < 20; i++) {
            var tag = LayerMask.LayerToName(i);
            if(tag != "Monster") {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }
    }
}
