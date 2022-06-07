using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Defence : Mortal {

    public string name;
    public int cost;
    public int dmgAmt;
    public float slowAmt;
    public float btwHitTime = 1f;
    public GameInfo.EnemyType target;

    private void OnTriggerStay2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            if(col.gameObject.GetComponent<MonsterInstance>().type == target) {
                col.gameObject.GetComponent<MonsterInstance>().affectedMoveAmt = slowAmt;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            if(col.gameObject.GetComponent<MonsterInstance>().type == target) {
                FindObjectOfType<TickDamager>().addTick(new TickDamager.TickInfo(col.gameObject, dealDamage, 1f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(FindObjectOfType<TickDamager>().contains(col.gameObject)) {
            FindObjectOfType<TickDamager>().removeTick(col.gameObject);
            col.gameObject.GetComponent<MonsterInstance>().affectedMoveAmt = 0f;
        }
    }

    public void dealDamage(GameObject obj) {
        if(obj == null)
            return;
        //  removes the obj from the ticking pool if the unit is going to die this time
        if(obj.GetComponent<Mortal>().health <= dmgAmt)
            FindObjectOfType<TickDamager>().removeTick(obj);

        obj.GetComponent<Mortal>().takeDamage(dmgAmt, 0, transform.position, false, false);
    }
}
