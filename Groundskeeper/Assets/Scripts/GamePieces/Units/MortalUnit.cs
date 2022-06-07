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
            GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<Movement>().spriteObj.GetComponent<SpriteRenderer>().DOColor(Color.white, takeTime);
            FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Movement>().spriteObj.gameObject);

            if(GetComponent<MonsterInstance>() != null && stunMonster)
                GetComponent<MonsterInstance>().stopMovingForATime(takeTime);
        }
    }
}
