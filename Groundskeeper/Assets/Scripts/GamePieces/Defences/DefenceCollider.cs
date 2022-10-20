using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceCollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            var def = transform.GetChild(0).GetComponent<DefenceInstance>();
            if(def.target == col.gameObject.GetComponent<MonsterInstance>().type) {
                FindObjectOfType<TickDamager>().addTick(col.gameObject, def);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            FindObjectOfType<TickDamager>().removeTick(col.gameObject);
        }
    }
}
