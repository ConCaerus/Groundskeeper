using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class BuildingMortal : Mortal {

    public override void customHitLogic(float knockback, Vector2 origin, bool stun = true) {
        float takeTime = .5f;

        //  show that this got hurt
        GetComponent<SpriteRenderer>().DOKill();
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GetComponent<SpriteRenderer>().color.a);
        GetComponent<SpriteRenderer>().DOColor(new Color(1.0f, 1.0f, 1.0f, GetComponent<SpriteRenderer>().color.a), takeTime);

        if(gameObject.tag == "House")
            FindObjectOfType<CameraMovement>().shake(10f);
    }
}
