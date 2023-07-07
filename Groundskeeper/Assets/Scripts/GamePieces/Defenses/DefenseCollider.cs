using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCollider : MonoBehaviour {
    TickDamager td;
    DefenseInstance def;

    private void Start() {
        td = FindObjectOfType<TickDamager>();
        def = transform.GetChild(0).GetComponent<DefenseInstance>();

        for(int i = 0; i < 20; i++) {
            var tag = LayerMask.LayerToName(i);
            if(tag != "Monster") {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            //  checks if the reference is still there
            if(def == null) {
                //  checks if there are any more children to use as a replacement reference
                if(transform.childCount > 0)
                    def = transform.GetChild(0).GetComponent<DefenseInstance>();

                //  if there aren't any spare children, this object has zero purpose and should kill itself NOW!
                //  but killing itself might cause problems in the defense spawner script, so just return before doing anything
                else
                    return;
            }
            if(def.target == GameInfo.MonsterType.Both || def.target == col.gameObject.GetComponent<MonsterInstance>().type) {
                //  checks if the monster is already being effected
                var temp = td.getTickInfo(col.gameObject);
                if(temp == null || temp.effect != def.GetComponent<Buyable>().title) {
                    td.addTick(col.gameObject, def);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            //  checks if this is the same type of defense as what the monster already thinks it's on
            //  this prevents tar defense cols from removing incoming spike effects
            var t = td.getTickInfo(col.gameObject);
            if(t != null && t.effect == def.GetComponent<Buyable>().title) {
                td.removeTick(col.gameObject);
            }
        }
    }
}
