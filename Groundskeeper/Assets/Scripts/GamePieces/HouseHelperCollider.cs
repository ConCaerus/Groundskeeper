using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseHelperCollider : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Helper")
            col.gameObject.GetComponent<HelperInstance>().spriteObj.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Helper")
            col.gameObject.GetComponent<HelperInstance>().spriteObj.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Awake() {
        for(int i = 0; i < 20; i++) {
            var tag = LayerMask.LayerToName(i);
            if(tag != "Helper") {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }
    }
}
