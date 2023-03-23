using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBoosterInstance : BuildingInstance {
    private void Start() {
        mortalInit();
        buildingInit();
    }

    public override void aoeEffect(GameObject effected, float amount) {
        //  increases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven * 2.0f;
        Debug.Log("Inc: " + effected.GetComponent<MonsterInstance>().soulsGiven);
    }
    public override void aoeLeaveEffect(GameObject effected, float amount) {
        //  decreases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven;
        Debug.Log("Dec: " + effected.GetComponent<MonsterInstance>().soulsGiven);
    }
}
