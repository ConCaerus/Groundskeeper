using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonInstance : HelperInstance {
    Coroutine attackWaiter = null;

    public override void inReachEnterAction(GameObject other) {
        if(attackWaiter == null)
            attackWaiter = StartCoroutine(attackAnim());
    }
    public override void inReachExitAction(GameObject other) {
    }

    IEnumerator attackAnim() {
        wi.attack(Vector2.zero, target);   //  attack the fucker
        yield return new WaitForSeconds(getAttackCoolDown());  //  already set to inReach, wait to check if not inreach
        attackWaiter = null;
    }
}
