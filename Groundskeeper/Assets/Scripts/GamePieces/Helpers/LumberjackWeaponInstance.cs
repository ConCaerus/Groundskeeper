using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackWeaponInstance : WeaponInstance {
    LumberjackInstance lji;

    private void Awake() {
        lji = user.GetComponent<LumberjackInstance>();
    }

    public override void movementLogic() {
        if(lji.hasTarget)
            lookAtPos(lji.target);
        else
            lookAtPos((Vector2)user.gameObject.transform.position + Vector2.right);
    }

    public override void shootMonster() {
    }
}
