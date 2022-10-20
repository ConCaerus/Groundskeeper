using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceInstance : Defence {

    public void dealDamage(GameObject obj) {
        if(obj == null)
            return;
        //  removes the obj from the ticking pool if the unit is going to die this time
        if(obj.GetComponent<Mortal>().health <= dmgAmt)
            FindObjectOfType<TickDamager>().removeTick(obj);

        obj.GetComponent<Mortal>().takeDamage(dmgAmt, 0, transform.position, false, false);
    }

    private void Start() {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
    }

    public override void customHitLogic(float knockback, Vector2 origin, bool stun) {
    }

    public override void die() {
        Destroy(gameObject);
    }
}
