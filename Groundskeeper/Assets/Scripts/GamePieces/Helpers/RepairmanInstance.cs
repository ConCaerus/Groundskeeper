using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairmanInstance : HelperInstance {
    Coroutine repairWaiter = null;


    public override void inReachEnterAction(GameObject other) {
        if(repairWaiter == null)
            repairWaiter = StartCoroutine(repairAnim(other.GetComponent<StructureInstance>()));
    }
    public override void inReachExitAction(GameObject other) {
        if(repairWaiter != null)
            StopCoroutine(repairWaiter);
        repairWaiter = null;
    }

    IEnumerator repairAnim(StructureInstance repairing) {
        while(repairing.health < repairing.maxHealth) {
            repairing.heal(5, true);
            yield return new WaitForSeconds(.05f);
        }

        hasTarget = false;
        //  finished healing, so look for another structure
        GetComponentInChildren<SightCollider>().resetCollider();
        GetComponentInChildren<HelperInReachCollider>().resetCollider();
        repairWaiter = null;
    }
}
