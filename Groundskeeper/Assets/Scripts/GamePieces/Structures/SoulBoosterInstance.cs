using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBoosterInstance : StructureInstance {
    private void Start() {
        mortalInit();
        structureInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  increases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven * 2.0f;
    }
    public override void aoeLeaveEffect(GameObject effected) {
        //  decreases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven;
    }
}
