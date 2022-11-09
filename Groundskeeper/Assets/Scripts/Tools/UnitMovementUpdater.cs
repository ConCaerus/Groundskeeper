using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementUpdater : MonoBehaviour {


    private void FixedUpdate() {
        foreach(var i in FindObjectsOfType<MonsterInstance>())
            i.updateMovement();
        foreach(var i in FindObjectsOfType<LumberjackInstance>()) {
            i.updateMovement();
        }
    }

    public void addMonster(MonsterInstance monster) {
        StartCoroutine(waitToFindANewTargetForMonster(monster));
    }
    public void addHelper(LumberjackInstance helper) {
        StartCoroutine(waitToFindANewTargetForHelper(helper));
    }


    IEnumerator waitToFindANewTargetForMonster(MonsterInstance monster) {
        yield return new WaitForSeconds(.25f);
        if(monster != null && monster.gameObject != null) {
            monster.updateTarget();
            StartCoroutine(waitToFindANewTargetForMonster(monster));
        }
    }

    IEnumerator waitToFindANewTargetForHelper(LumberjackInstance helper) {
        yield return new WaitForSeconds(.25f);
        if(helper != null && helper.gameObject != null) {
            helper.updateTarget();
            StartCoroutine(waitToFindANewTargetForHelper(helper));
        }
    }
}
