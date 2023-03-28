using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnitMovementUpdater : MonoBehaviour {

    public void addHelper(HelperInstance helper) {
        StartCoroutine(waitToFindANewTargetForHelper(helper));
    }
    public void addMonster(MonsterInstance monster) {
        StartCoroutine(updateMovementEachFrame(monster));
    }

    IEnumerator waitToFindANewTargetForHelper(HelperInstance helper) {
        while(helper != null && helper.gameObject != null) {
            helper.updateMovement();
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator updateMovementEachFrame(MonsterInstance monster) {
        while(monster != null && monster.gameObject != null) {
            monster.updateMovement();
            yield return new WaitForFixedUpdate();
        }
    }
}
