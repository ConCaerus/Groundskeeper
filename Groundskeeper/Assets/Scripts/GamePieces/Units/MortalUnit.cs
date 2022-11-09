using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class MortalUnit : Mortal {
    public override void customHitLogic(float knockback, Vector2 origin, bool stunMonster = true) {
        var rb = gameObject.GetComponent<Rigidbody2D>();
        //  take knockback
        if(rb != null) {
            var dir = -(origin - (Vector2)transform.position).normalized;
            rb.AddForce(dir * knockback * 5000f);
        }

        float takeTime = .5f;

        //  show that this got hurt
        if(GetComponent<Movement>() != null) {
            GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().DOKill();
            var a = GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().color.a;
            GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, a);
            GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, a), takeTime);

            foreach(var i in GetComponents<Collider2D>()) {
                if(!i.isTrigger) {
                    FindObjectOfType<LayerSorter>().requestNewSortingLayer(i, GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>());
                    break;
                }
            }

            if(GetComponent<MonsterInstance>() != null && stunMonster)
                GetComponent<MonsterInstance>().stopMovingForATime(takeTime);
        }
    }
}
