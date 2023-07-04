using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairmanInstance : HelperInstance {
    Coroutine repairWaiter = null;

    GameObject curRepairing = null;

    public List<StructureInstance> queuedTargets = new List<StructureInstance>();

    private void Awake() {
        StartCoroutine(doubleChecker());
    }

    public override void inReachEnterAction(GameObject other) {
        if(repairWaiter == null)
            repairWaiter = StartCoroutine(repairAnim(other.GetComponent<StructureInstance>()));
    }
    public override void inReachExitAction(GameObject other) {
        if(repairWaiter != null && other == curRepairing)
            StopCoroutine(repairWaiter);
        repairWaiter = null;
    }

    IEnumerator repairAnim(StructureInstance repairing) {
        curRepairing = repairing.gameObject;
        while(repairing.health < repairing.maxHealth) {
            repairing.heal(8, true);
            yield return new WaitForSeconds(.25f);
        }

        hirc.resetCollider();
        repairWaiter = null;
        curRepairing = null;

        //  checks for queued targets
        while(queuedTargets.Count > 0) {
            bool found = false;
            var temp = queuedTargets[0];
            if(temp != null) {
                found = true;
                hasTarget = true;
                followingTransform = temp.transform;
            }
            queuedTargets.RemoveAt(0);
            if(found)
                break;
        }
    }

    public bool isBusy() {
        return moving || queuedTargets.Count > 0 || repairWaiter != null;
    }

    public void repairStructure(StructureInstance struc) {
        if(queuedTargets.Contains(struc))
            return;
        //  not busy and has nothing to do
        if(!moving && queuedTargets.Count == 0 && repairWaiter == null) {
            hasTarget = true;
            followingTransform = struc.transform;
        }

        //  busy
        else {
            queuedTargets.Add(struc);
        }
    }

    IEnumerator doubleChecker() {
        while(true) {
            if(queuedTargets.Count == 0) {
                yield return new WaitForSeconds(1f);
            }
            //  checks if not repairing
            if(repairWaiter == null) {
                //  checks if should be repairing
                while(!moving && queuedTargets.Count > 0) {
                    bool found = false;
                    var temp = queuedTargets[0];
                    if(temp != null) {
                        found = true;
                        hasTarget = true;
                        followingTransform = temp.transform;
                    }
                    queuedTargets.RemoveAt(0);
                    if(found)
                        break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
