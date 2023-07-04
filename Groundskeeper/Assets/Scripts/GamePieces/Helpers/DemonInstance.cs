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
        if(attackWaiter != null)
            StopCoroutine(attackWaiter);
        attackWaiter = null;
        StartCoroutine(attackChecker());
    }

    IEnumerator attackAnim() {
        if(hasTarget || inReach)
            wi.attack(Vector2.zero, target);   //  attack the fucker
        yield return new WaitForSeconds(getAttackCoolDown());  //  already set to inReach, wait to check if not inreach
        attackWaiter = null;
        StartCoroutine(attackChecker());
    }

    IEnumerator attackChecker() {
        //  checks if should attack based on logic
        if(inReach && !tooClose && attackWaiter == null) {
            attackWaiter = StartCoroutine(attackAnim());
        }
        //  determined that it shouldn't attack and checks again in .5f seconds
        else {
            yield return new WaitForSeconds(.5f);
            if(!hasTarget) {
                //  checks if sees another monster
                if(gb.monsters.Count > 0 && scc.IsTouchingLayers(LayerMask.GetMask("Monster"))) {
                    var closest = gb.monsters.FindClosest(transform.position).transform;
                    //  makes sure that the closest monster is actually a targetable thing
                    if(closest != null && Vector2.Distance(closest.position, transform.position) < sc.maxRad) {
                        hasTarget = true;
                        followingTransform = closest;
                    }
                }
                //sc.resetCollider(this);
            }
            if(!inReach) {
                if(gb.monsters.Count > 0 && hircc.IsTouchingLayers(LayerMask.GetMask("Monster"))) {

                    inReach = false;
                }
            }
            StartCoroutine(attackChecker());
        }
    }
}
