using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperWeaponInstance : WeaponInstance {
    public override void movementLogic() {
        if(user.GetComponent<HelperInstance>().hasTarget)
            lookAtPos(user.GetComponent<HelperInstance>().target);
        else
            lookAtPos(GameObject.FindGameObjectWithTag("Player").transform.position);
    }
}
