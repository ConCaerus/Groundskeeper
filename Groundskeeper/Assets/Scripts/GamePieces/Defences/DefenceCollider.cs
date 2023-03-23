using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceCollider : MonoBehaviour {
    TickDamager td;
    DefenceInstance def;

    private void Start() {
        td = FindObjectOfType<TickDamager>();
        def = transform.GetChild(0).GetComponent<DefenceInstance>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            if(def.target == GameInfo.MonsterType.Both || def.target == col.gameObject.GetComponent<MonsterInstance>().type) {
                td.addTick(col.gameObject, def);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
                //  checks if this is the same type of defence as what the monster already thinks it's on
                //  this prevents tar defence cols from removing incoming spike effects
                var t = td.getTickInfo(col.gameObject);
                if(t != null && t.effect == def.GetComponent<Buyable>().title) {
                    td.removeTick(col.gameObject);
                }
            }
    }
}
