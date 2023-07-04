using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class MortalUnit : Mortal {
    public override void hitLogic(float knockback, Vector2 origin, float stunTime) {
        if(isDead)
            return;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        //  take knockback
        if(rb != null) {
            var dir = -(origin - (Vector2)transform.position).normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(dir * knockback * 5000f);
        }

        float takeTime = .5f;

        //  show that this got hurt
        if(GetComponent<Movement>() != null) {
            var sr = GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>();
            sr.DOKill();
            sr.color = new Color(1f, 0f, 0f, getStartingColor().a);
            sr.DOColor(getStartingColor(), takeTime);

            foreach(var i in GetComponents<Collider2D>()) {
                if(!i.isTrigger) {
                    FindObjectOfType<LayerSorter>().requestNewSortingLayer(i, GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>());
                    break;
                }
            }

            if(GetComponent<MonsterInstance>() != null && stunTime > 0f) {
                GetComponent<MonsterInstance>().stopMovingForATime(stunTime);
            }
        }
    }
}
