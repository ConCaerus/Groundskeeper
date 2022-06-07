using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceInstance : Defence {

    public override void customHitLogic(float knockback, Vector2 origin, bool stun) {
    }

    public override void die() {
        Destroy(gameObject);
    }
}
