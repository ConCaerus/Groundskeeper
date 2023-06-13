using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class StructureMortal : Mortal {

    public override void hitLogic(float knockback, Vector2 origin, float stunTime) {
        float takeTime = .5f;

        //  show that this got hurt
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GetComponent<SpriteRenderer>().color.a);
        GetComponent<SpriteRenderer>().DOBlendableColor(new Color(1.0f, 1.0f, 1.0f, GetComponent<SpriteRenderer>().color.a), takeTime);

        if(gameObject.tag == "House")
            FindObjectOfType<CameraMovement>().shake(10f);
    }
}
