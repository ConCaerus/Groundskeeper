using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BuildingMortal : Mortal {

    public override void customHitLogic(float knockback, Vector2 origin, bool stun = true) {
        float takeTime = .5f;

        //  show that this got hurt
        GetComponent<SpriteRenderer>().DOKill();
        GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<SpriteRenderer>().DOColor(Color.white, takeTime);
    }
}
