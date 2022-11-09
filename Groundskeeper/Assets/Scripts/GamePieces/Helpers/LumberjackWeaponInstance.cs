using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackWeaponInstance : WeaponInstance {
    public override void movementLogic() {
        if(user.GetComponent<LumberjackInstance>().hasTarget)
            lookAtPos(user.GetComponent<LumberjackInstance>().target);
        else
            lookAtPos((Vector2)user.gameObject.transform.position + Vector2.right);
    }
}
